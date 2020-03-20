using System;
using System.IO;

namespace Funnel.Output
{
    public class FileOutput : IOutput
    {
        private FileStream fileStream;
        private StreamWriter streamWriter;
        private bool disposed = false;

        public FileOutput(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentNullException(nameof(path));
            }

            this.fileStream = new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
            this.streamWriter = new StreamWriter(this.fileStream);
        }

        public void Dispose()
        {
            this.disposed = true;
            this.streamWriter.Dispose();
            this.fileStream.Dispose();
        }

        public void Output(string text)
        {
            if (!this.disposed && this.streamWriter != null)
            {
                this.streamWriter.WriteLine(text);
            }
        }
    }
}