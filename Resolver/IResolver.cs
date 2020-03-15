using System;
using System.Collections.Generic;

namespace ResolverCore
{
    public interface IResolver
    {
        void Register<T>(T objectValue);

        T Resolve<T>() where T : class;
        T Resolve<T>(Type t) where T : class;
        void Map<From, To>();

        void ResolveProperties(object typedObject);

        void ManipulateDependencies<T>(Action<T> manipulateDependency) where T : class;
        void ManipulateDependencies<T>(Func<IEnumerable<T>, IEnumerable<T>> manipulateCollection, Action<T> manipulateDependency) where T : class;
    }
}