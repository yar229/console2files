using System.Text;
using System.Text.Unicode;

namespace console2files_test
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var encRu = Encoding.GetEncoding(1251);
            var encUtf = Encoding.UTF8;
            string text = "Русский текст";
            int i = 0;
            while (!Console.KeyAvailable)
            {
                Thread.Sleep(1000);
                Console.WriteLine($"{i++} RU: {encRu.GetString(encUtf.GetBytes(text))}    UTF: {text}");
            }

            Console.WriteLine("Finished");
        }
    }
}