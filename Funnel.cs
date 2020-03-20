using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Funnel.Output;

namespace Funnel
{
    public class Funnel : IDisposable
    {
        private Configuration config;

        private Regex lineHeadRegex;

        public ConcurrentQueue<string> TargetQueue = new ConcurrentQueue<string>();

        private List<IOutput> outputs = new List<IOutput>();

        private Task consumerTask;

        private CancellationTokenSource tokenSource = new CancellationTokenSource();

        private List<Regex> keywords;

        public Funnel(Configuration config)
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            this.config = config;
            this.keywords = this.config.Keywords?.Select(k => new Regex(k)).ToList();
            this.consumerTask = Task.Run(this.Output, this.tokenSource.Token);
            if (!string.IsNullOrWhiteSpace(this.config.LineHead))
            {
                this.lineHeadRegex = new Regex(this.config.LineHead);
            }

            if (this.config.Outputs?.Any() ?? false)
            {
                this.config.Outputs.ForEach(o =>
                {
                    var output = OutputFactory.CreateOutput(o);
                    if (output != null)
                    {
                        this.outputs.Add(output);
                    }
                });
            }
        }

        public async Task Filter()
        {
            if (this.config.Inputs == null || this.config.Inputs.Count <= 0)
            {
                return;
            }

            if (this.config.Limitation > 0)
            {
                foreach (var input in this.config.Inputs)
                {
                    await this.Filter(input);
                }
            }
            else
            {
                var tasks = this.config.Inputs.Select(input => this.Filter(input));
                Task.WaitAll(tasks.ToArray());
            }
        }

        public async Task Filter(string path)
        {
            try
            {
                using(var source = File.OpenRead(path))
                {
                    StreamReader reader = null;
                    if (!string.IsNullOrWhiteSpace(this.config.Encoding))
                    {
                        reader = new StreamReader(source, Encoding.GetEncoding(this.config.Encoding), true);
                    }
                    else
                    {
                        reader = new StreamReader(source, true);
                    }
                    using(reader)
                    {
                        await Filter(reader, path);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"过滤 {path} 时，发生异常：{e.Message}\n{e.StackTrace}");
            }
        }
        
        public void Dispose()
        {
            try
            {
                this.outputs.ForEach(o => o.Dispose());
                this.tokenSource.Cancel();
                this.tokenSource.Dispose();
                this.consumerTask?.Dispose();
            }
            catch{}
        }

        private void Output()
        {
            while (!this.tokenSource.IsCancellationRequested)
            {
                if (this.TargetQueue.Count <= 0)
                {
                    SpinWait.SpinUntil(() => this.TargetQueue.Count > 0);
                }

                while(this.TargetQueue.TryDequeue(out string text))
                {
                    this.outputs.AsParallel().ForAll(output => output?.Output(text));
                }
            }
        }

        private async Task Filter(StreamReader reader, string path)
        {
            var lines = 0m;
            var time = DateTime.Now;
            var sb = new StringBuilder();
            var line = await reader.ReadLineAsync();
            string text = null;
            while (line != null)
            {
                lines++;

                if (NextBlock(line))
                {
                    text = sb.ToString();
                    if (!Sifting(text))
                    {
                        Consume(text);
                    }

                    sb.Clear();
                }
                if (sb.Length > 0)
                {
                    sb.Append(Environment.NewLine);
                }
                sb.Append(line);

                if (config.Limitation > 0)
                {
                    var timespan = DateTime.Now - time;
                    if (lines > (decimal)(timespan.TotalSeconds + 1) * config.Limitation)
                    {
                        Thread.Sleep(1000 - timespan.Milliseconds);
                    }
                }

                line = await reader.ReadLineAsync();
            }

            text = sb.ToString();
            if (!Sifting(text))
            {
                Consume(text);
            }

            Console.WriteLine($"{lines} lines in {path}, time consumed: {DateTime.Now - time}");
        }


        private bool Sifting(string text)
        {
            if (string.IsNullOrWhiteSpace(text) || this.keywords == null || this.keywords.Count <= 0)
            {
                return false;
            }

            return !this.keywords.AsParallel().Any(k => k.IsMatch(text));
        }

        private bool NextBlock(string text)
        {
            if (!this.config.Multiline || this.lineHeadRegex == null)
            {
                return true;
            }

            return this.lineHeadRegex.IsMatch(text);
        }

        private void Consume(string text)
        {
            this.TargetQueue.Enqueue(text);
        }
    }
}