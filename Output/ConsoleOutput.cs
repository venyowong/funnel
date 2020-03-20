using System;

namespace Funnel.Output
{
    public class ConsoleOutput : IOutput
    {
        public void Dispose()
        {
        }

        public void Output(string text)
        {
            Console.WriteLine(text);
        }
    }
}