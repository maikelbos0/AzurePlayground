﻿using AzurePlayground.Utilities.Container;
using AzurePlayground.Utilities.Reflection;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using Unity;

namespace AzurePlayground.Utilities.Tests.Container {
    [TestClass]
    public sealed class InjectionRegistrarTests {
        public interface ITest1 { }

        [Injectable]
        public sealed class Test1 : ITest1 { }

        public interface ITest2<TValue> { }

        [Injectable]
        public sealed class Test2 : ITest2<Test1> { }

        public interface ITest3<TValue> {
            TValue GetValue();
        }

        [Injectable]
        [TestDecorator1]
        public sealed class Test3a : ITest3<string> {
            public string GetValue() {
                return "Success";
            }
        }

        [Injectable]
        [TestDecorator2]
        [TestDecorator1(Order = 1)]
        public sealed class Test3b : ITest3<int> {
            public int GetValue() {
                return 1;
            }
        }

        public sealed class TestDecorator1Attribute : DecoratorAttribute {
            public TestDecorator1Attribute() : base(typeof(TestDecorator1<>)) {
            }
        }

        public sealed class TestDecorator1<TValue> : Decorator<ITest3<TValue>>, ITest3<TValue> {
            public TValue GetValue() {
                return Handler.GetValue();
            }
        }

        public sealed class TestDecorator2Attribute : DecoratorAttribute {
            public TestDecorator2Attribute() : base(typeof(TestDecorator2<>)) {
            }
        }

        public sealed class TestDecorator2<TValue> : Decorator<ITest3<TValue>>, ITest3<TValue> {
            public TValue GetValue() {
                return Handler.GetValue();
            }
        }

        public IClassFinder GetClassFinderFor<T>() {
            var classFinder = Substitute.For<IClassFinder>();

            classFinder.FindAllClasses().Returns(new Type[] { typeof(T) });

            return classFinder;
        }

        [TestMethod]
        public void InjectionRegistrar_Registers_For_Matching_Interface() {
            var container = new UnityContainer();
            var registrar = new InjectionRegistrar(GetClassFinderFor<Test1>());

            registrar.RegisterTypes(container);

            container.Resolve<ITest1>().Should().BeOfType<Test1>();
        }

        [TestMethod]
        public void InjectionRegistrar_Registers_For_Single_Interface() {
            var container = new UnityContainer();
            var registrar = new InjectionRegistrar(GetClassFinderFor<Test2>());

            registrar.RegisterTypes(container);

            container.Resolve<ITest2<Test1>>().Should().BeOfType<Test2>();
        }

        [TestMethod]
        public void InjectionRegistrar_Throws_Exception_For_Multiple_Interfaces() {
            throw new System.NotImplementedException();
        }

        [TestMethod]
        public void InjectionRegistrar_Throws_Exception_For_Multiple_Types_One_Interface() {
            throw new System.NotImplementedException();
        }

        [TestMethod]
        public void InjectionRegistrar_Registers_Decorators() {
            var container = new UnityContainer();
            var registrar = new InjectionRegistrar(GetClassFinderFor<Test3a>());

            registrar.RegisterTypes(container);

            var decorator = container.Resolve<ITest3<string>>();

            decorator.Should().BeOfType<TestDecorator1<string>>();
            ((TestDecorator1<string>)decorator).Handler.Should().BeOfType<Test3a>();
            decorator.GetValue().Should().Be("Success");
        }

        [TestMethod]
        public void InjectionRegistrar_Registers_Chained_Decorators() {
            var container = new UnityContainer();
            var registrar = new InjectionRegistrar(GetClassFinderFor<Test3b>());

            registrar.RegisterTypes(container);

            var decorator = container.Resolve<ITest3<int>>();

            decorator.Should().BeOfType<TestDecorator1<int>>();
            ((TestDecorator1<int>)decorator).Handler.Should().BeOfType<TestDecorator2<int>>();
            ((TestDecorator2<int>)((TestDecorator1<int>)decorator).Handler).Handler.Should().BeOfType<Test3b>();
            decorator.GetValue().Should().Be(1);
        }

        [TestMethod]
        public void InjectionRegistrar_Throw_Exception_For_Mismatched_Decorator() {
            throw new System.NotImplementedException();
        }
    }
}