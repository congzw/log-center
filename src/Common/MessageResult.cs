using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Common
{
    public class MessageResult
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool Success { get; set; } = false;

        /// <summary>
        /// 提示消息
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// 附加数据
        /// </summary>
        public virtual object Data { get; set; } = null;

        /// <summary>
        /// 附加细节（字典）
        /// </summary>
        public IDictionary<string, object> Details { get; set; } = new ConcurrentDictionary<string, object>(StringComparer.OrdinalIgnoreCase);

        public static MessageResult Create(bool success, string message, object data = null)
        {
            return new MessageResult()
            {
                Success = success,
                Message = message,
                Data = data
            };
        }

        public static MessageResult CreateSuccess(string message, object data = null)
        {
            return new MessageResult() { Success = true, Message = message, Data = data };
        }

        public static MessageResult CreateFail(string message, object data = null)
        {
            return new MessageResult() { Success = false, Message = message, Data = data };
        }

        public static MessageResult ValidateResult(bool success = false, string successMessage = "验证通过", string failMessage = "验证失败")
        {
            var vr = new MessageResult
            {
                Message = success ? successMessage : failMessage,
                Success = success
            };
            return vr;
        }
    }

    public static class MessageResultExtension
    {
        public static MessageResult ToSingleResult(this IEnumerable<MessageResult> results)
        {
            if (results == null) throw new ArgumentNullException(nameof(results));
            var items = results.ToList();
            if (items.Count == 1)
            {
                return items[0];
            }

            var success = items.Count(x => x.Success);
            var total = items.Count;
            var msg = $"Success/Total: {success}/{total}";

            var messageResult = new MessageResult();
            messageResult.Data = items;
            messageResult.Message = msg;
            messageResult.Success = success == total && items.Count > 0;
            return messageResult;
        }
    }
}
