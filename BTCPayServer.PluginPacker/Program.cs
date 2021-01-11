using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using BTCPayServer.Abstractions.Contracts;

namespace BTCPayServer.PluginPacker
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 3)
            {
                Console.WriteLine("Usage: btcpay-plugin [directory of compiled plugin] [name of plugin] [packed plugin output directory]");
                return;
            }
            var directory = args[0];
            var name = args[1];
            var outputDir = args[2];
            var outputFile = Path.Combine(outputDir, name);
            var rootDLLPath = Path.Combine(directory, name +".dll");
            if (!File.Exists(rootDLLPath) )
            {
                throw new Exception($"{rootDLLPath} could not be found");
            }

            var assembly = Assembly.LoadFrom(rootDLLPath);
            var extension = GetAllExtensionTypesFromAssembly(assembly).FirstOrDefault();
            if (extension is null)
            {
                throw new Exception($"{rootDLLPath} is not a valid plugin");
            }

            var loadedPlugin = (IBTCPayServerPlugin)Activator.CreateInstance(extension);
            var json = JsonSerializer.Serialize(loadedPlugin);
            Directory.CreateDirectory(outputDir);
            if (File.Exists(outputFile + ".grspay"))
            {
                File.Delete(outputFile + ".grspay");
            }
            ZipFile.CreateFromDirectory(directory, outputFile + ".grspay", CompressionLevel.Optimal, false);
            File.WriteAllText(outputFile + ".grspay.json", json);
            Console.WriteLine($"Created {outputFile}.grspay at {directory}");
        }

        private static Type[] GetAllExtensionTypesFromAssembly(Assembly assembly)
        {
            return assembly.GetTypes().Where(type =>
                typeof(IBTCPayServerPlugin).IsAssignableFrom(type) &&
                !type.IsAbstract).ToArray();
        }
    }
}
