using Microsoft.VisualStudio.TestTools.UnitTesting;
using ResolverCore;
using System;

namespace ResolverTests
{
    [TestClass]
    public class ResolverTests
    {
        [TestMethod]
        public void Map_WhenResolvingDependency_ShouldResolveDependency()
        {
            var resolver = new Resolver();
            var result = resolver.Resolve<DummyClass>();
            Assert.IsNotNull(result.Field);
            Assert.AreEqual(typeof(DependencyParent), result.Field.GetType());
        }

        [TestMethod]
        public void Map_WhenRegisteringIncorrectMappedDependency_ShouldVerifyThatTypesAreCompatible()
        {
            var resolver = new Resolver();
            try
            {
                resolver.Map<DependencyChild, DependencyParent>();
                Assert.Fail();
            }
            catch (ArgumentException)
            {
            }
        }

        [TestMethod]
        public void Map_WhenRegisteringCorrectMappedDependency_ShouldVerifyThatTypesAreCompatible()
        {
            var resolver = new Resolver();
            resolver.Map<DependencyParent, DependencyChild>();
        }

        [TestMethod]
        public void Map_WhenResolvingMappedDependency_ShouldResolveDependency()
        {
            var resolver = new Resolver();
            resolver.Map<DependencyParent, DependencyChild>();
            var result = resolver.Resolve<DummyClass>();
            Assert.IsNotNull(result.Field);
            Assert.AreEqual(typeof(DependencyChild), result.Field.GetType());
        }
    }

    class DummyClass
    {
        [Resolvable]
        public DependencyParent Field { get; set; }
    }

    class DependencyParent
    {
    }

    class DependencyChild : DependencyParent
    {
    }
}