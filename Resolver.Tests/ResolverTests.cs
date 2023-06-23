using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ResolverCore;

namespace ResolverTests
{
    [TestClass]
    public class ResolverTests
    {
        [TestMethod]
        public void Resolve_WhenResolvingDependency_ShouldResolveDependency()
        {
            // Arrange
            var resolver = new Resolver();

            // Act
            var result = resolver.Resolve<DummyClassWithResolvableProperty>();

            // Assert
            Assert.IsNotNull(result.Field);
            Assert.AreEqual(typeof(DependencyImplementation), result.Field.GetType());
        }

        [TestMethod]
        public void Resolve_WhenResolvingMappedDependency_ShouldResolveDependency()
        {
            // Arrange
            var resolver = new Resolver();
            resolver.Register<IDependency, DependencyImplementation>();

            // Act
            var result = resolver.Resolve<DummyClassWithUnspecifiedProperty>();

            // Assert
            Assert.IsNotNull(result.Field);
            Assert.AreEqual(typeof(DependencyImplementation), result.Field.GetType());
        }

        [TestMethod]
        public void Register_WhenRegisteringCorrectMappedDependency_ShouldVerifyThatTypesAreCompatible()
        {
            // Arrange
            var resolver = new Resolver();

            // Act
            resolver.Register<IDependency, DependencyImplementation>();

            // Assert
            // Did not crash
        }

        [TestMethod]
        public void Register_WhenRegisteringIncoCorrectMappedDependency_ShouldVerifyThatTypesAreCompatible()
        {
            // Arrange
            var resolver = new Resolver();
            try
            {
                // Act
                resolver.Register<IDependency, Implementation>();
                Assert.Fail("Should have thrown an ArgumentException");
            }
            catch (ArgumentException)
            {
            }

            // Assert
            // Assert.Fail was not called
        }

        [TestMethod]
        public void Register_WhenResolvingMappedDependency_ShouldResolveDependency()
        {
            var resolver = new Resolver();
            resolver.Register<IDependency, DependencyImplementation>();
            var result = resolver.Resolve<DummyClassWithUnspecifiedProperty>();
            Assert.IsNotNull(result.Field);
            Assert.AreEqual(typeof(DependencyImplementation), result.Field.GetType());
        }
    }

    class DummyClassWithUnspecifiedProperty
    {
        [Resolvable]
        public IDependency Field { get; set; }
    }

    class DummyClassWithResolvableProperty
    {
        [Resolvable]
        public DependencyImplementation Field { get; set; }
    }

    interface IDependency
    {
    }

    class DependencyImplementation : IDependency
    {
    }

    class Implementation
    {
    }
}