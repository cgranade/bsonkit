// Copyright ⓒ Christopher Granade.
// Licensed under the MIT License.

using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;

namespace BsonKit
{
    class Program
    {
        internal static Stream StreamForInputFilename(string? filename) =>
            filename == "-" || filename == null
            ? System.Console.OpenStandardInput()
            : File.OpenRead(filename);

        internal static Command BsonToJsonCommand =>
            new Command("to-json", "Converts a BSON file or stream to JSON.")
            {
                new Argument("filename")
                {
                    ArgumentType = typeof(string),
                    Description = "Name of file to load BSON from, or - to use stdin.",
                    Arity = ArgumentArity.ZeroOrOne
                },

                new Option("--pretty")
                {
                    Description = "Pretty-prints output JSON."
                }
            }
            .WithHandler(
                CommandHandler.Create<string, bool>(
                    (filename, pretty) =>
                    {
                        using var stream = StreamForInputFilename(filename);
                        using var reader = new BsonDataReader(stream);

                        var serializer = new JsonSerializer();

                        var data = serializer.Deserialize(reader);
                        var dataAsJson = JsonConvert.SerializeObject(
                            data,
                            pretty ? Formatting.Indented : Formatting.None
                        );

                        System.Console.WriteLine(dataAsJson);

                    }
                )
            );

        internal static Command JsonToBsonCommand =>
            new Command("from-json", "Converts a JSON file or stream to BSON.")
            {
                new Argument("filename")
                {
                    ArgumentType = typeof(string),
                    Description = "Name of file to load BSON from, or - to use stdin.",
                    Arity = ArgumentArity.ZeroOrOne
                }
            }
            .WithHandler(
                CommandHandler.Create<string>(
                    (filename) =>
                    {
                        using var stream = StreamForInputFilename(filename);
                        using var reader = new StreamReader(stream);

                        var serializer = new JsonSerializer();
                        var data = serializer.Deserialize(reader, typeof(object));

                        var ms = new MemoryStream();
                        using var writer = new BsonDataWriter(ms);
                        serializer.Serialize(writer, data);

                        ms.WriteTo(System.Console.OpenStandardOutput());
                    }
                )
            );

        static async Task<int> Main(string[] args) =>
            await new RootCommand
            {
                BsonToJsonCommand,
                JsonToBsonCommand
            }
            .InvokeAsync(args);
    }
}
