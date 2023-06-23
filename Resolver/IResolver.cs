using System;
using System.Collections.Generic;

namespace ResolverCore
{
    public interface IResolver
    {
        /// <summary>
        /// Registers a dependency to a dependency value.
        /// Resolve and ResolveProperties will populate any property of type T with the objectValue.
        /// </summary>
        /// <typeparam name="T">The dependency type</typeparam>
        /// <param name="objectValue">The resolved dependency value</param>
        void Register<T>(T objectValue);

        /// <summary>
        /// Registers a dependency to a dependency type.
        /// Resolve and ResolveProperties will populate any property of type From with a new instance of type To.
        /// </summary>
        /// <typeparam name="From">The dependency type</typeparam>
        /// <typeparam name="To">The resolved dependency type</typeparam>
        void Register<From, To>();

        /// <summary>
        /// Instantiates a new object of type T and resolves all its properties. 
        /// Use the Register methods to register dependencies.
        /// </summary>
        /// <typeparam name="T">The type of the object</typeparam>
        /// <returns>A new object of type T with all its properties resolved.</returns>
        T Resolve<T>() where T : class;

        /// <summary>
        /// Resolves all object properties. 
        /// Use the Register methods to register dependencies.
        /// </summary>
        /// <param name="typedObject">The object to be resolved</param>
        void ResolveProperties(object typedObject);

        void TransformDependencies<T>(Action<T> manipulateDependency) where T : class;
        void TransformDependencies<T>(Func<IEnumerable<T>, IEnumerable<T>> manipulateCollection, Action<T> manipulateDependency) where T : class;
    }
}