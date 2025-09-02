using Sharprompt;

namespace ConsoleAppAzureBlobStorage.Inputs;

public static class InputHelper
{
    public static string GetTestEndpoint()
    {
        string answer;
        do
        {
            answer = Prompt.Input<string>(options =>
            {
                options.Message = "Informe a URL para testes (ENTER para default)";
                options.DefaultValue = "https://baconipsum.com/api/?type=meat-and-filler";
            });
            Console.WriteLine();
        }
        while (string.IsNullOrWhiteSpace(answer));
        return answer.Trim();
    }
}