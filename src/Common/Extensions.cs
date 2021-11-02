using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Common.Utilities;

namespace Common
{
    public static partial class Extensions
    {
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> enumerable)
        {
            return enumerable == null || !enumerable.Any();
        }

        public static string GetHashCodeMessage<T>(this T instance)
        {
            if (instance == null)
            {
                return $"{typeof(T).FullName}[@]NULL";
            }
            return $"{instance.GetType().FullName}[@]{instance.GetHashCode()}";
        }
        
        public static List<string> MySplit(this string value)
        {
            return ExtendHelper.Instance.MySplit(value);
        }

        public static string MyJoin(this  IEnumerable<string> values)
        {
            return ExtendHelper.Instance.MyJoin(values);
        }

        public static bool MyEquals(this string value, string value2, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
        {
            return ExtendHelper.Instance.MyEquals(value,value2, comparison);
        }

        public static bool MyContains(this IEnumerable<string> values, string toCheck, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
        {
            return ExtendHelper.Instance.MyContains(values, toCheck, comparison);
        }

        public static string MyFind(this IEnumerable<string> values, string toCheck, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
        {
            return ExtendHelper.Instance.MyFind(values, toCheck, comparison);
        }

        public static Task<T> AsTask<T>(this T value)
        {
            return FromResult(value);
        }

        public static Task<T> FromResult<T>(T result)
        {
#if NET40
            var tcs = new TaskCompletionSource<T>();
            tcs.SetResult(result);
            return tcs.Task;
#else
            return Task.FromResult(result);
#endif
        }

        public static T WithPropertyValue<T, TValue>(this T target, Expression<Func<T, TValue>> memberExpression, TValue value)
        {
            if (memberExpression.Body is MemberExpression memberSelectorExpression)
            {
                var property = memberSelectorExpression.Member as PropertyInfo;
                if (property != null)
                {
                    property.SetValue(target, value, null);
                }
            }

            return target;
        }

        #region hack for deplay

        public static Task AsDelay(this TimeSpan value)
        {
            return Delay(value);
        }

        public static Task Delay(TimeSpan span)
        {
#if NET40
            return Delay(span, CancellationToken.None);
#else
            return Task.Delay(span);
#endif
        }

        public static Task Delay(TimeSpan span, CancellationToken token)
        {
#if NET40
            return Delay((int)span.TotalMilliseconds, token);
#else
            return Task.Delay(span, token);
#endif
        }

        private static Task Delay(int milliseconds, CancellationToken token)
        {
            var tcs = new TaskCompletionSource<object>();
            token.Register(() => tcs.TrySetCanceled());
            var timer = new Timer(_ => tcs.TrySetResult(null));
            timer.Change(milliseconds, -1);
            return tcs.Task;
        }

        #endregion
    }
}
