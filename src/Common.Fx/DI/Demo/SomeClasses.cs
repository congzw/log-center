using System;
using System.Collections.Generic;
using System.Linq;

namespace Common.Fx.DI.Demo
{
    public class SomeClass
    {
    }
    public interface ITestGenericProvider : IAutoInjectAsTransient
    {
    }
    public interface ITestGenericProvider<T> : ITestGenericProvider
    {
        T TheClass { get; set; }
    }
    public class TestGenericProvider<T> : ITestGenericProvider<T>
    {
        public T TheClass { get; set; }
    }
    public class TestGenericA : ITestGenericProvider<SomeClass>
    {
        public SomeClass TheClass { get; set; }
    }
    public class TestGenericB : ITestGenericProvider<SomeClass>
    {
        public SomeClass TheClass { get; set; }
    }
    public class TestGenericC : ITestGenericProvider<TestGenericC>
    {
        public TestGenericC TheClass { get; set; }
    }
    public static class TestGenericExtensions
    {
        public static string GetAutoInjectDescription(this Type serviceType)
        {
            if (serviceType == null)
            {
                return "<<NULL>>";
            }
            return serviceType.GetFriendlyNameWithNamespace();
        }

        public static List<string> GetAutoInjectDescriptions(this IEnumerable<IAutoInject> service)
        {
            return service.Select(x => x.GetType().GetAutoInjectDescription()).ToList();
        }
    }
}