using System.Collections.Generic;

namespace Common
{
    public static partial class Extensions
    {
        public static void SetBagValue<TKey, TValue>(this IDictionary<TKey, TValue> bags, TKey key, TValue value)
        {
            bags[key] = value;
        }

        public static TValue GetBagValue<TKey, TValue>(this IDictionary<TKey, TValue> bags, TKey key, TValue defaultValue, bool autoFill = false)
        {
            if (bags.TryGetValue(key, out var value))
            {
                return value;
            }

            if (autoFill)
            {
                bags[key] = defaultValue;
            }
            return defaultValue;
        }

        public static void SetBagValue<TKey, TValue>(this IDictionary<string, object> bags, string key, TValue value)
        {
            bags[key] = value;
        }

        public static TValue GetBagValue<TValue>(this IDictionary<string, object> bags, string key, TValue defaultValue, bool autoFill = false)
        {
            if (bags.TryGetValue(key, out var value))
            {
                if (value is TValue theValue)
                {
                    return theValue;
                }

                //fix JArray Or JObject convert bugs!
                return value.ConvertAs(true, defaultValue);
            }

            if (autoFill)
            {
                bags[key] = defaultValue;
            }
            return defaultValue;
        }
    }
}
