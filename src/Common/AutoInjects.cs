namespace Common
{
    public interface IAutoInject
    {
    }

    public interface IAutoInjectAsTransient : IAutoInject
    {
    }

    public interface IAutoInjectAsScoped : IAutoInject
    {
    }

    public interface IAutoInjectAsSingleton : IAutoInject
    {
    }

    //escape from IAutoInject auto register
    public interface IAutoInjectIgnore
    {
    }
}