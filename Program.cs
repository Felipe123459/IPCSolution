using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace IpcPipeline
{
    class Program
    {
        static async Task Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Usage: dotnet run -- [generator|transformer|consumer|run-pipeline]");
                return;
            }
            //Switch statement to let the user pick which component to execute
            switch (args[0].ToLower())
            {
                case "generator":
                    await RunGenerator();
                    break;
                case "transformer":
                    await RunTransformer();
                    break;
                case "consumer":
                    await RunConsumer();
                    break;
                case "run-pipeline":
                    await RunPipeline();
                    break;
                default:
                    Console.WriteLine($"Unknown command: {args[0]}");
                    break;
            }
        }

        //Generates fruit data and writes it to standard output
        static async Task RunGenerator()
        {
            Console.WriteLine("Generator started...");

            string[] fruitData =
            {
                "apple,5,red",
                "banana,7,yellow",
                "orange,4,orange",
                "grape,12,purple",
                "strawberry,9,red",
                "blueberry,15,blue",
                "kiwi,8,green"
            };
            //Get a StreamWriter for stdout to output the data
            using var writer = new StreamWriter(Console.OpenStandardOutput());//Write the data to the stdout
            foreach (var entry in fruitData)
            {
                await writer.WriteLineAsync(entry);
                Console.Error.WriteLine($"Generated: {entry}");
                await Task.Delay(500);//Simulate processing time
            }

            Console.Error.WriteLine("Generator finished.");
        }

        //Reads fruit data, transforms it, and sends it to standard output
        static async Task RunTransformer()
        {
            Console.Error.WriteLine("Transformer started...");

            using var reader = new StreamReader(Console.OpenStandardInput());
            using var writer = new StreamWriter(Console.OpenStandardOutput());//Create readers and writers 

            string? line;
            while ((line = await reader.ReadLineAsync()) != null)
            {
                var parts = line.Split(',');

                if (parts.Length < 3)
                {
                    Console.WriteLine($"Skipping invalid input: {line}");
                    continue;
                }

                string fruit = parts[0].ToUpper();
                int count = int.TryParse(parts[1], out var c) ? c * 2 : 0;//Default goes to 0 if invalid
                string color = parts[2];

                string transformed = $"{fruit},{count},{color}";
                await writer.WriteLineAsync(transformed);
                Console.Error.WriteLine($"Transformed: {line} -> {transformed}");
            }

            Console.Error.WriteLine("Transformer finished.");
        }

        //Reads transformed data and prints the results
        static async Task RunConsumer()
        {
            Console.WriteLine("Consumer started...");
            Console.WriteLine("Results:");

            using var reader = new StreamReader(Console.OpenStandardInput());//Create a reader for the stdin from the previous stage
            int totalCount = 0;

            string? line;
            while ((line = await reader.ReadLineAsync()) != null)
            {
                var parts = line.Split(',');

                if (parts.Length < 3) continue;

                string fruit = parts[0];
                int count = int.Parse(parts[1]);
                string color = parts[2];

                Console.WriteLine($"Fruit: {fruit}, Count: {count}, Color: {color}");
                totalCount += count;
            }

            Console.WriteLine($"Total items processed: {totalCount}");
            Console.WriteLine("Consumer finished.");
        }

        //Runs the full pipeline using subprocesses
        static async Task RunPipeline()
        {
            Console.WriteLine("Starting pipeline...");

            //Start consumer process
            var consumerProc = StartProcess("consumer");

            //Start transformer process
            var transformerProc = StartProcess("transformer", redirectOutput: true);

            using (var transformerInput = transformerProc.StandardInput)
            using (var transformerOutput = transformerProc.StandardOutput)
            {
                //Pipe transformer output to consumer
                var pipeTask = Task.Run(async () =>
                {
                    string? line;
                    while ((line = await transformerOutput.ReadLineAsync()) != null)
                    {
                        await consumerProc.StandardInput.WriteLineAsync(line);
                    }
                    consumerProc.StandardInput.Close();
                });

                //Generate and send data to transformer
                string[] fruitData =
                {
                    "apple,5,red",
                    "banana,7,yellow",
                    "orange,4,orange",
                    "grape,12,purple",
                    "strawberry,9,red",
                    "blueberry,15,blue",
                    "kiwi,8,green"
                };
                //Send each data 
                foreach (var entry in fruitData)
                {
                    await transformerInput.WriteLineAsync(entry);
                    Console.WriteLine($"Pipeline sent: {entry}");
                    await Task.Delay(500);
                }

                transformerInput.Close();
                await pipeTask;//Make sure all of the processes are done 

                consumerProc.WaitForExit();
                transformerProc.WaitForExit();//Exit the programs 
            }

            Console.WriteLine("Pipeline finished.");
        }

        //Helper function to start a subprocess
        private static Process StartProcess(string command, bool redirectOutput = false)
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"run -- {command}",
                UseShellExecute = false,
                RedirectStandardInput = true,
                RedirectStandardOutput = redirectOutput,
                CreateNoWindow = true
            };

            var process = new Process { StartInfo = startInfo };
            process.Start();
            return process;
        }
    }
}