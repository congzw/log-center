namespace Common.Logs.LogCenter
{
    public static class ReportLogExtensions
    {
        public static ReportLogResult Validate(this ReportLogArgs args)
        {
            var validate = ReportLogArgs.Validate(args, out var message);
            return ReportLogResult.Create(validate, message, args);
        }
    }
}