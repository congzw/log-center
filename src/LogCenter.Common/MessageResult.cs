namespace LogCenter.Common
{
    public class MessageResult
    {
        public string Message { get; set; }
        public bool Success { get; set; }
        public object Data { get; set; }

        public static MessageResult Create(bool success, string message, object data = null)
        {
            return new MessageResult() { Success = success, Message = message, Data = data };
        }
        public static MessageResult CreateSuccess(string message, object data = null)
        {
            return new MessageResult() { Success = true, Message = message, Data = data };
        }
        public static MessageResult CreateFail(string message, object data = null)
        {
            return new MessageResult() { Success = false, Message = message, Data = data };
        }
    }
}
