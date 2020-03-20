using System;

namespace Funnel.Output
{
    public interface IOutput : IDisposable
    {
        void Output(string text);
    }
}