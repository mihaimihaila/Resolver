using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace Resolver.Tests
{
    [TestClass]
    public class ResolverTests
    {
        [TestMethod]
        public async Task Map_WhenResolvingDependency_ShouldResolveDependency()
        {
            var resolver = new Resolver();
            var result = resolver.Resolve<DummyClass>();
            Assert.IsNotNull(result.Field);
            Assert.AreEqual(typeof(DependencyParent), result.Field.GetType());
        }

        [TestMethod]
        public async Task Map_WhenRegisteringIncorrectMappedDependency_ShouldVerifyThatTypesAreCompatible()
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
        public async Task Map_WhenRegisteringCorrectMappedDependency_ShouldVerifyThatTypesAreCompatible()
        {
            var resolver = new Resolver();
            resolver.Map<DependencyParent, DependencyChild>();
        }

        [TestMethod]
        public async Task Map_WhenResolvingMappedDependency_ShouldResolveDependency()
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
