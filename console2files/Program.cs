using CommandLine;
using System.Text;
using System.Text.Unicode;
using System.Web;

namespace console2files
{
    class Program
    {
        static void Main(string[] args)
        {
            var result = Parser.Default.ParseArguments<CommandLineOptions>(args);

            var exitCode = result
                .MapResult(
                    options =>
                    {
                        bool doEncoding = options.ConsoleEncoding != options.FilesEncoding;
                        Encoding? consoleEncoding = null;
                        Encoding? filesEncoding = null;
                        if (doEncoding)
                        {
                            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                            consoleEncoding = Encoding.GetEncoding(options.ConsoleEncoding); 
                            filesEncoding = Encoding.GetEncoding(options.FilesEncoding);
                        }

                        var outputs = options.Files
                            .Select(file => new StreamWriter(file))
                            .ToList();

                        while (Console.ReadLine() is { } line)
                        {
                            string encline = options.UrlDecode
                                ? line.HackDecode()
                                : line;

                            if (doEncoding)
                                filesEncoding!.GetString(consoleEncoding!.GetBytes(encline));
                            if (options.ConsoleWrite)
                                Console.WriteLine(line);
                            var tasks = outputs.Select(async o =>
                            {
                                await o.WriteLineAsync(encline);
                                await o.FlushAsync();
                            });
                            Task.WhenAll(tasks).Wait();
                        }

                        outputs.ForEach(o => o.Close());


                        return 0;
                    },
                    _ => 1);

            if (exitCode > 0) Environment.Exit(exitCode);
        }
    }

    class CommandLineOptions
    {
        [Option("console-write", Required = false, Default = true, HelpText = "output to console")]
        public bool ConsoleWrite { get; set; }

        [Option("hackdecode", Required = false, Default = false, HelpText = "foo")]
        public bool UrlDecode { get; set; }

        [Option("files", Separator = ',', Required = true, HelpText = "files to write to")]
        public IEnumerable<string> Files { get; set; }

        [Option("console-encoding", Required = false, Default = "UTF-8", HelpText = "Console text encoding, see https://learn.microsoft.com/ru-ru/dotnet/api/system.text.encoding?view=net-7.0")]
        public string ConsoleEncoding { get; set; }

        [Option("files-encoding", Required = false, Default = "UTF-8", HelpText = "Files text encoding, see https://learn.microsoft.com/ru-ru/dotnet/api/system.text.encoding?view=net-7.0")]
        public string FilesEncoding { get; set; }
    }
}