using System;
using System.Collections.Generic;
using System.Linq;

namespace Common.Utilities
{
    public interface IExtendHelper
    {
        string MyJoin(IEnumerable<string> values);
        List<string> MySplit(string value);
        bool MyEquals(string value, string value2, StringComparison comparison = StringComparison.OrdinalIgnoreCase);
        bool MyContains(IEnumerable<string> values, string toCheck, StringComparison comparison = StringComparison.OrdinalIgnoreCase);
        string MyFind(IEnumerable<string> values, string toCheck, StringComparison comparison = StringComparison.OrdinalIgnoreCase);
        bool MyCodesContains(string configCodes, string argsCodes, bool configCodesEmptyReturn = false, bool argsCodesEmptyReturn = false);
        bool MyCodesStartWith(string configCodes, string argsCodes, bool configCodesEmptyReturn = false, bool argsCodesEmptyReturn = false);
    }

    public class ExtendHelper : IExtendHelper
    {
        #region auto resolve from di or default

        [LazySingleton]
        public static IExtendHelper Instance => LazySingleton.Instance.Resolve(() => new ExtendHelper());

        #endregion

        public char[] Splitter { get; set; } = {',', '，'};

        public string MyJoin(IEnumerable<string> values)
        {
            return string.Join(',', values.Select(x => x));
        }

        public List<string> MySplit(string value)
        {
            return value.Split(Splitter, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToList();
        }
        
        public bool MyEquals(string value, string value2, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
        {
            var valueFix = string.Empty;
            if (!string.IsNullOrWhiteSpace(value))
            {
                valueFix = value.Trim();
            }

            var value2Fix = string.Empty;
            if (!string.IsNullOrWhiteSpace(value2))
            {
                value2Fix = value2.Trim();
            }

            return valueFix.Equals(value2Fix, comparison);
        }

        public bool MyContains(IEnumerable<string> values, string toCheck, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
        {
            foreach (var value in values)
            {
                if (value.MyEquals(toCheck, comparison))
                {
                    return true;
                }
            }
            return false;
        }

        public string MyFind(IEnumerable<string> values, string toCheck, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
        {
            foreach (var value in values)
            {
                if (value.MyEquals(toCheck, comparison))
                {
                    return value;
                }
            }
            return null;
        }
        
        /// <summary>
        ///  "x,y","X,Z" => true
        /// </summary>
        /// <param name="configCodes"></param>
        /// <param name="argsCodes"></param>
        /// <param name="configCodesEmptyReturn"></param>
        /// <param name="argsCodesEmptyReturn"></param>
        /// <returns></returns>
        public bool MyCodesContains(string configCodes, string argsCodes, bool configCodesEmptyReturn = false, bool argsCodesEmptyReturn = false)
        {
            if (string.IsNullOrWhiteSpace(configCodes))
            {
                return configCodesEmptyReturn;
            }

            if (string.IsNullOrWhiteSpace(argsCodes))
            {
                return argsCodesEmptyReturn;
            }

            var needItems = configCodes.MySplit();
            var argsItems = argsCodes.MySplit();
            foreach (var argsItem in argsItems)
            {
                if (needItems.MyContains(argsItem))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// "/a", "/A,/B" => true
        /// "/a", "/A/a" => true
        /// "/a", "/A/b" => true
        /// </summary>
        /// <param name="configCodes"></param>
        /// <param name="argsCodes"></param>
        /// <param name="configCodesEmptyReturn"></param>
        /// <param name="argsCodesEmptyReturn"></param>
        /// <returns></returns>
        public bool MyCodesStartWith(string configCodes, string argsCodes, bool configCodesEmptyReturn = false, bool argsCodesEmptyReturn = false)
        {
            if (string.IsNullOrWhiteSpace(configCodes))
            {
                return configCodesEmptyReturn;
            }

            if (string.IsNullOrWhiteSpace(argsCodes))
            {
                return argsCodesEmptyReturn;
            }

            var configItems = configCodes.MySplit();
            var argsItems = argsCodes.MySplit();
            foreach (var argsItem in argsItems)
            {
                if (configItems.Any(configItem => argsItem.StartsWith(configItem, StringComparison.OrdinalIgnoreCase)))
                {
                    return true;
                }
            }
            return false;
        }
    }

    #region some extensions

    public class NeedRoleCheck
    {
        public string NeedRoles { get; set; }
        public string ArgsRoles { get; set; }
        public bool Allowed()
        {
            return ExtendHelper.Instance.MyCodesContains(NeedRoles, ArgsRoles, true, false);
        }

        public static NeedRoleCheck Create(string needRoles, string argsRoles)
        {
            return new NeedRoleCheck { NeedRoles = needRoles, ArgsRoles = argsRoles };
        }
    }

    public class RouteStartWithCheck
    {
        public string ConfigRoutes { get; set; }
        public string ArgsRoutes { get; set; }
        public bool StartWith()
        {
            return ExtendHelper.Instance.MyCodesStartWith(ConfigRoutes, ArgsRoutes, false, false);
        }
        public static RouteStartWithCheck Create(string configRoutes, string argsRoles)
        {
            return new RouteStartWithCheck { ConfigRoutes = configRoutes, ArgsRoutes = argsRoles };
        }
    }
    

    #endregion
}
