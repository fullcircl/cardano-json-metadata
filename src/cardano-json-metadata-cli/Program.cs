using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CardanoJsonMetadata.Cli
{
    class Program
    {
        public static Stream? _inputStream = null;
        public static Stream? _outputStream = null;

        static async Task<int> Main(string[] args)
        {
            var rootCommand = new RootCommand
            {
            };
            rootCommand.Description = "Cardano JSON Metadata CLI";

            var inputFileOption = new Option<FileInfo>(
                "--input-file",
                "The file to parse."
            ).ExistingOnly();
            inputFileOption.AddAlias("-i");

            var outputFileOption = new Option<FileInfo>(
                "--output-file",
                "The file to write."
            );
            outputFileOption.AddAlias("-o");

            var directionOption = new Option<ConversionDirection>(
                "--direction",
                "The direction to convert"
            );
            directionOption.IsRequired = true;
            directionOption.AddAlias("-d");

            var convertCommand = new Command("convert")
            {
                inputFileOption,
                outputFileOption,
                directionOption,
            };
            convertCommand.Description = "Convert formats.";

            convertCommand.Handler = CommandHandler.Create<FileInfo, FileInfo, ConversionDirection>(async (inputFile, outputFile, direction) =>
            {
                if (inputFile != null)
                {
                    _inputStream = File.OpenRead(inputFile.FullName);
                }

                if (_inputStream == null)
                {
                    throw new Exception("No input provided.");
                }

                if (outputFile != null)
                {
                    _outputStream = File.OpenWrite(outputFile.FullName);
                }

                if (_outputStream == null)
                {
                    _outputStream = Console.OpenStandardOutput();
                }

                TxMetadata metadata;
                switch (direction)
                {
                    case ConversionDirection.FromSchema:

                        using (var sr = new StreamReader(_inputStream))
                        {
                            metadata = TxMetadata.Deserialize(await sr.ReadToEndAsync());
                        }

                        using (var sw = new StreamWriter(_outputStream))
                        {
                            await sw.WriteAsync(metadata.ToJson());
                            await sw.FlushAsync();
                            sw.Close();
                        }

                        break;
                    case ConversionDirection.ToSchema:

                        using (var sr = new StreamReader(_inputStream))
                        {
                            metadata = TxMetadata.FromJson(await sr.ReadToEndAsync());
                        }

                        using (var sw = new StreamWriter(_outputStream))
                        {
                            await sw.WriteAsync(metadata.Serialize());
                            await sw.FlushAsync();
                            sw.Close();
                        }
                        break;
                    default:
                        throw new InvalidOperationException($"Unexpected value for [{nameof(direction)}]: {direction}");
                }
            });
            rootCommand.Add(convertCommand);

            var commandLineBuilder = new CommandLineBuilder(rootCommand);
            commandLineBuilder.UseMiddleware(async (context, next) =>
            {
                if (context.Console.IsInputRedirected)
                {
                    _inputStream = Console.OpenStandardInput();
                }

                if (context.Console.IsOutputRedirected)
                {
                    _outputStream = Console.OpenStandardOutput();
                }

                await next(context);
            });

            commandLineBuilder.UseDefaults();
            var parser = commandLineBuilder.Build();
            // Parse the incoming args and invoke the handler
            return await parser.InvokeAsync(args);
        }
    }
}
