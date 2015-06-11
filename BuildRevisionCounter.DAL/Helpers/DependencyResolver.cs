using System;
using BuildRevisionCounter.DAL.Repositories;
using MongoDB.Driver;
using Ninject;

namespace BuildRevisionCounter.DAL.Helpers
{
    public class DependencyResolver
    {
        private static IDependencyResolver dependencyResolver;

        static DependencyResolver()
        {
            dependencyResolver = new DefaultDependencyResolver();
        }

        public static IDependencyResolver Current
        {
            get
            {
                return dependencyResolver;
            }
        }

        private sealed class DefaultDependencyResolver : IDependencyResolver
        {
            private readonly IKernel kernel;  // Ninject kernel

            public DefaultDependencyResolver()
            {
                kernel = new StandardKernel();
                LoadBindings();
            }

            public T Get<T>()
            {
                return kernel.Get<T>();
            }

            private void LoadBindings()
            {
                kernel.Bind<IMongoClient>().To<MongoClient>();
            }
        }
    }
    public interface IDependencyResolver { }
}
