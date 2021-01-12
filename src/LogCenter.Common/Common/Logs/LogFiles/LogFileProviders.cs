using Microsoft.Extensions.FileProviders;

namespace Common.Logs.LogFiles
{
    public interface ILogFileProvider : IFileProvider
    {
    }

    public class LogFileProvider : CompositeFileProvider, ILogFileProvider
    {
        public LogFileProvider(PhysicalFileProvider physicalFileProvider) : base(physicalFileProvider)
        {
        }
    }
}
