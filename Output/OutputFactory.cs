namespace Funnel.Output
{
    public static class OutputFactory
    {
        public static IOutput CreateOutput(string output)
        {
            if (output?.IndexOf("console") == 0)
            {
                return new ConsoleOutput();
            }
            else if (output?.IndexOf("file") == 0)
            {
                var path = output.Substring("file:".Length)?.Trim();
                if (!string.IsNullOrWhiteSpace(path))
                {
                    return new FileOutput(path);
                }
            }

            return null;
        }
    }
}