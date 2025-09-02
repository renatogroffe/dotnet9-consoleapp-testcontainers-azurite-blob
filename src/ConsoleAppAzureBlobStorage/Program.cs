using Azure.Storage.Blobs;
using ConsoleAppAzureBlobStorage.Inputs;
using ConsoleAppAzureBlobStorage.Utils;
using Serilog;
using System.Text;
using Testcontainers.Azurite;

var logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("testcontainers-azuretablestorage.tmp")
    .CreateLogger();
logger.Information("***** Iniciando testes com Testcontainers + Azure Blob Storage *****");

Console.WriteLine();
logger.Information("Diversas APIs publicas para testes: https://github.com/public-apis/public-apis");

Console.WriteLine();
var testEndpoint = InputHelper.GetTestEndpoint();

CommandLineHelper.Execute("docker container ls",
    "Containers antes da execucao do Testcontainers...");

var azuriteContainer = new AzuriteBuilder()
  .WithImage("mcr.microsoft.com/azure-storage/azurite:3.35.0")
  .Build();
await azuriteContainer.StartAsync();

CommandLineHelper.Execute("docker container ls",
    "Containers apos execucao do Testcontainers...");

var connectionStringTableStorage = azuriteContainer.GetConnectionString();
const string containerName = "demoarquivos";
logger.Information($"Connection String = {connectionStringTableStorage}");
logger.Information($"Blob Endpoint = {azuriteContainer.GetBlobEndpoint()}");
logger.Information($"Blob a ser utilizado nos testes = {containerName}");

var blobClient = new BlobServiceClient(connectionStringTableStorage);
var blobContainerClient = blobClient.CreateBlobContainer(containerName).Value;
logger.Information($"Blob container {containerName} criado com sucesso!");
using var httpClient = new HttpClient();

Console.WriteLine();
const int maxBlobs = 5;
for (int i = 1; i <= maxBlobs; i++)
{
    var response = await httpClient.GetAsync(testEndpoint);
    response.EnsureSuccessStatusCode();
    logger.Information("Notificacao enviada com sucesso!");
    var content = await response.Content.ReadAsStringAsync();
    logger.Information($"Dados recebidos = {content}");

    var blobName = $"apiresponse-{i:00}.txt";
    using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
    await blobContainerClient.UploadBlobAsync(blobName, stream);
    logger.Information($"Blob '{blobName}' gravado com sucesso no container '{containerName}'.");

    logger.Information("Pressione ENTER para continuar...");
    Console.ReadLine();
}

logger.Information("Testes concluidos com sucesso!");
logger.Information("Pressione ENTER para encerrar a aplicacao...");
Console.ReadLine();