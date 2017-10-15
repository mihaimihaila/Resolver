namespace Resolver
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public class Resolver : IResolver
    {
        private class DependencyList : IDisposable
        {
            public Dictionary<PropertyInfo, object> Properties { get; private set; }
            public List<PropertyInfo> Resolvables { get; private set; }

            public void Dispose()
            {
                DisposeHelper.ClearCollection(Resolvables);
                Properties.Clear();
            }

            public DependencyList(Dictionary<PropertyInfo, object> properties, List<PropertyInfo> resolvables)
            {
                Properties = properties;
                Resolvables = resolvables;
            }
        }

        private Dictionary<Type, object> dependencies;
        private Dictionary<TypeInfo, DependencyList> resolvableProperties;

        public Resolver()
        {
            dependencies = new Dictionary<Type, object>();
            resolvableProperties = new Dictionary<TypeInfo, DependencyList>();
        }

        public void Register<T>(T objectValue)
        {
            dependencies.Add(typeof(T), objectValue);
        }

        public T Resolve<T>() where T : class
        {
            Type type = typeof(T);
            return Resolve<T>(type);
        }

        public T Resolve<T>(Type type) where T : class
        {
            var typeInfo = type.GetTypeInfo();
            var typedObject = GetNewObject<T>(typeInfo, "Resolver");

            ResolveObjectProperties(typeInfo, typedObject);

            return typedObject;
        }

        public void ResolveProperties(object typedObject)
        {
            var typeInfo = typedObject.GetType().GetTypeInfo();
            ResolveObjectProperties(typeInfo, typedObject);
        }

        public void ManipulateDependencies<T>(Action<T> manipulateDependency) where T : class
        {
            ManipulateDependencies(dependencies.Values.OfType<T>(), manipulateDependency);
        }

        public void ManipulateDependencies<T>(Func<IEnumerable<T>, IEnumerable<T>> manipulateCollection, Action<T> manipulateDependency) where T : class
        {
            var manipuatedCollection = manipulateCollection(dependencies.Values.OfType<T>());
            ManipulateDependencies(manipuatedCollection, manipulateDependency);
        }

        private void ManipulateDependencies<T>(IEnumerable<T> collection, Action<T> manipulateDependency) where T : class
        {
            foreach (var dependency in collection)
            {
                manipulateDependency(dependency);
            }
        }

        private T GetNewObject<T>(TypeInfo typeInfo, string owner) where T : class
        {
            var constructor = typeInfo.DeclaredConstructors.First();
            if (constructor.GetParameters().Count() != 0)
            {
                throw new InvalidOperationException($"Parameterless constructor needed in order to build object of type: {typeInfo.FullName} for owner: {owner}");
            }

            var newObject = constructor.Invoke(new object[0]);

            return (T)newObject;
        }

        private void ResolveObjectProperties(TypeInfo typeInfo, object typedObject)
        {
            if (!resolvableProperties.ContainsKey(typeInfo))
            {
                var properties = new Dictionary<PropertyInfo, object>();
                var resolvables = new List<PropertyInfo>();

                foreach (var declaredProperty in typeInfo.DeclaredProperties)
                {
                    var propertyType = declaredProperty.PropertyType;
                    if (dependencies.ContainsKey(propertyType))
                    {
                        properties.Add(declaredProperty, dependencies[propertyType]);
                    }
                    else
                    {
                        foreach (var attribute in declaredProperty.CustomAttributes)
                        {
                            if (attribute.AttributeType == typeof(Resolvable))
                            {
                                resolvables.Add(declaredProperty);
                            }
                        }
                    }
                }

                lock (this)
                {
                    if (!resolvableProperties.ContainsKey(typeInfo))
                    {
                        resolvableProperties.Add(typeInfo, new DependencyList(properties, resolvables));
                    }
                    else
                    {
                        properties.Clear();
                        resolvables.Clear();
                    }
                }
            }

            foreach (var propertyMap in resolvableProperties[typeInfo].Properties)
            {
                propertyMap.Key.SetValue(typedObject, propertyMap.Value);
            }

            foreach (var propertyMap in resolvableProperties[typeInfo].Resolvables)
            {
                var resolvedPropertyObject = GetNewObject<object>(propertyMap.PropertyType.GetTypeInfo(), typeInfo.FullName);
                ResolveProperties(resolvedPropertyObject);

                propertyMap.SetValue(typedObject, resolvedPropertyObject);
            }

            var baseType = typeInfo.BaseType;

            if (baseType.FullName != "System.Object")
            {
                ResolveObjectProperties(baseType.GetTypeInfo(), typedObject);
            }
        }
    }
}