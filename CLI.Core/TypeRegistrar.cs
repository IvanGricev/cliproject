using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;
using System;

namespace CLI.Core
{
    public sealed class SpectreTypeRegistrar : ITypeRegistrar
    {
        private readonly IServiceCollection _services;

        public SpectreTypeRegistrar(IServiceCollection services)
        {
            _services = services;
        }

        public ITypeResolver Build()
        {
            var provider = _services.BuildServiceProvider();
            return new SpectreTypeResolver(provider);
        }

        public void Register(Type service, Type implementation)
        {
            _services.AddSingleton(service, implementation);
        }

        public void RegisterInstance(Type service, object implementation)
        {
            _services.AddSingleton(service, implementation);
        }

        public void RegisterLazy(Type service, Func<object> factory)
        {
            _services.AddSingleton(service, (provider) => factory());
        }
    }

    public sealed class SpectreTypeResolver : ITypeResolver, IDisposable
    {
        private readonly IServiceProvider _provider;

        public SpectreTypeResolver(IServiceProvider provider)
        {
            _provider = provider;
        }

        public object Resolve(Type type)
        {

            return _provider.GetService(type);
        }

        public void Dispose()
        {
            if (_provider is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
    }
}

