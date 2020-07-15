namespace LogCenter
{
    public class ReportLogResult
    {
        public string Message { get; set; }
        public bool Success { get; set; }
        public object Data { get; set; }

        public static ReportLogResult Create(bool success, string message, object data = null)
        {
            return new ReportLogResult() { Success = success, Message = message, Data = data };
        }
        public static ReportLogResult CreateSuccess(string message, object data = null)
        {
            return new ReportLogResult() { Success = true, Message = message, Data = data };
        }
        public static ReportLogResult CreateFail(string message, object data = null)
        {
            return new ReportLogResult() { Success = false, Message = message, Data = data };
        }
    }
}
