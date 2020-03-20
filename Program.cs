using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using YamlDotNet.Serialization;

namespace Funnel
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var config = LoadConfig(args[0]);
            var funnel = new Funnel(config);
            await funnel.Filter();
            SpinWait.SpinUntil(() => funnel.TargetQueue.Count <= 0);
            Thread.Sleep(1000);
            funnel.Dispose();
        }

        static Configuration LoadConfig(string configPath)
        {
            using(var stream = new FileStream(configPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using(var reader = new StreamReader(stream))
                {
                    return new Deserializer().Deserialize<Configuration>(reader.ReadToEnd());
                }
            }
        }
    }
}
