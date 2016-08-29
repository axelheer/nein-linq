using NeinLinq.Tests.InjectableQueryData;
using System;
using System.Linq;
using Xunit;

namespace NeinLinq.Tests
{
    public class InjectableQueryTest
    {
        readonly IFunctions functions = new ConcreteFunctions(1);

        readonly IQueryable<Dummy> data;

        public InjectableQueryTest()
        {
            data = new[]
            {
                new Dummy { Id = 1, Name = "Asdf", Distance = 66, Time = .33 },
                new Dummy { Id = 2, Name = "Narf", Distance = 0, Time = 3.14 },
                new Dummy { Id = 3, Name = "Qwer", Distance = 8, Time = 64 }
            }
            .AsQueryable();
        }

        [Fact]
        public void InjectStaticShouldFailWithoutSibling()
        {
            var query = from d in data.ToInjectable(typeof(Functions))
                        select d.VelocityWithoutSibling();

            Assert.Throws<InvalidOperationException>(() =>
                query.ToList());
        }

        [Fact]
        public void InjectInterfaceShouldFailWithoutSibling()
        {
            var query = from d in data.ToInjectable(typeof(IFunctions))
                        select functions.VelocityWithoutSibling(d);

            Assert.Throws<InvalidOperationException>(() =>
                query.ToList());
        }

        [Fact]
        public void InjectBaseShouldFailWithoutSibling()
        {
            var baseFunctions = (FunctionsBase)functions;

            var query = from d in data.ToInjectable(typeof(FunctionsBase))
                        select baseFunctions.VelocityWithoutSibling(d);

            Assert.Throws<InvalidOperationException>(() =>
                query.ToList());
        }

        [Fact]
        public void InjectConcreteShouldFailWithoutSibling()
        {
            var concreteFunctions = (ConcreteFunctions)functions;

            var query = from d in data.ToInjectable(typeof(ConcreteFunctions))
                        select concreteFunctions.VelocityWithoutSibling(d);

            Assert.Throws<InvalidOperationException>(() =>
                query.ToList());
        }

        [Fact]
        public void InjectStaticShouldSucceedWithConvention()
        {
            var query = from d in data.ToInjectable(typeof(Functions))
                        select d.VelocityWithConvention();

            var result = query.ToList();

            Assert.Equal(new[] { 200.0, .0, .125 }, result);
        }

        [Fact]
        public void InjectInterfaceShouldSucceedWithConvention()
        {
            var query = from d in data.ToInjectable(typeof(IFunctions))
                        select functions.VelocityWithConvention(d);

            var result = query.ToList();

            Assert.Equal(new[] { 200.0, .0, .1 }, result);
        }

        [Fact]
        public void InjectBaseShouldSucceedWithConvention()
        {
            var baseFunctions = (FunctionsBase)functions;

            var query = from d in data.ToInjectable(typeof(FunctionsBase))
                        select baseFunctions.VelocityWithConvention(d);

            var result = query.ToList();

            Assert.Equal(new[] { 200.0, .0, .1 }, result);
        }

        [Fact]
        public void InjectConcreteShouldSucceedWithConvention()
        {
            var concreteFunctions = (ConcreteFunctions)functions;

            var query = from d in data.ToInjectable(typeof(ConcreteFunctions))
                        select concreteFunctions.VelocityWithConvention(d);

            var result = query.ToList();

            Assert.Equal(new[] { 200.0, .0, .1 }, result);
        }

        [Fact]
        public void InjectStaticShouldSucceedWithMetadata()
        {
            var query = from d in data.ToInjectable()
                        select d.VelocityWithMetadata();

            var result = query.ToList();

            Assert.Equal(new[] { 200.0, .0, .125 }, result);
        }

        [Fact]
        public void InjectInterfaceShouldSucceedWithMetadata()
        {
            var query = from d in data.ToInjectable()
                        select functions.VelocityWithMetadata(d);

            var result = query.ToList();

            Assert.Equal(new[] { 200.0, .0, .1 }, result);
        }

        [Fact]
        public void InjectBaseShouldSucceedWithMetadata()
        {
            var baseFunctions = (FunctionsBase)functions;

            var query = from d in data.ToInjectable()
                        select baseFunctions.VelocityWithMetadata(d);

            var result = query.ToList();

            Assert.Equal(new[] { 200.0, .0, .1 }, result);
        }

        [Fact]
        public void InjectConcreteShouldSucceedWithMetadata()
        {
            var concreteFunctions = (ConcreteFunctions)functions;

            var query = from d in data.ToInjectable()
                        select concreteFunctions.VelocityWithMetadata(d);

            var result = query.ToList();

            Assert.Equal(new[] { 200.0, .0, .1 }, result);
        }

        [Fact]
        public void InjectStaticShouldSucceedWithTypeAndMethodMetadata()
        {
            var query = from d in data.ToInjectable()
                        select d.VelocityWithTypeAndMethodMetadata();

            var result = query.ToList();

            Assert.Equal(new[] { 200.0, .0, .125 }, result);
        }

        [Fact]
        public void InjectStaticShouldSucceedWithTypeMetadata()
        {
            var query = from d in data.ToInjectable()
                        select d.VelocityWithTypeMetadata();

            var result = query.ToList();

            Assert.Equal(new[] { 200.0, .0, .125 }, result);
        }

        [Fact]
        public void InjectStaticShouldSucceedWithMethodMetadata()
        {
            var query = from d in data.ToInjectable()
                        select d.VelocityWithMethodMetadata();

            var result = query.ToList();

            Assert.Equal(new[] { 200.0, .0, .125 }, result);
        }

        [Fact]
        public void InjectInterfaceShouldSucceedWithMethodMetadata()
        {
            var query = from d in data.ToInjectable()
                        select functions.VelocityWithMethodMetadata(d);

            var result = query.ToList();

            Assert.Equal(new[] { 200.0, .0, .1 }, result);
        }

        [Fact]
        public void InjectBaseShouldSucceedWithMethodMetadata()
        {
            var baseFunctions = (FunctionsBase)functions;

            var query = from d in data.ToInjectable()
                        select baseFunctions.VelocityWithMethodMetadata(d);

            var result = query.ToList();

            Assert.Equal(new[] { 200.0, .0, .1 }, result);
        }

        [Fact]
        public void InjectConcreteShouldSucceedWithMethodMetadata()
        {
            var concreteFunctions = (ConcreteFunctions)functions;

            var query = from d in data.ToInjectable()
                        select concreteFunctions.VelocityWithMethodMetadata(d);

            var result = query.ToList();

            Assert.Equal(new[] { 200.0, .0, .1 }, result);
        }

        [Fact]
        public void InjectStaticShouldFailWithInvalidSiblingResult()
        {
            var query = from d in data.ToInjectable(typeof(Functions))
                        select d.VelocityWithInvalidSiblingResult();

            Assert.Throws<InvalidOperationException>(() =>
                query.ToList());
        }

        [Fact]
        public void InjectInterfaceShouldFailWithInvalidSiblingResult()
        {
            var query = from d in data.ToInjectable(typeof(IFunctions))
                        select functions.VelocityWithInvalidSiblingResult(d);

            Assert.Throws<InvalidOperationException>(() =>
                query.ToList());
        }

        [Fact]
        public void InjectBaseShouldFailWithInvalidSiblingResult()
        {
            var baseFunctions = (FunctionsBase)functions;

            var query = from d in data.ToInjectable(typeof(FunctionsBase))
                        select baseFunctions.VelocityWithInvalidSiblingResult(d);

            Assert.Throws<InvalidOperationException>(() =>
                query.ToList());
        }

        [Fact]
        public void InjectConcreteShouldFailWithInvalidSiblingResult()
        {
            var concreteFunctions = (ConcreteFunctions)functions;

            var query = from d in data.ToInjectable(typeof(ConcreteFunctions))
                        select concreteFunctions.VelocityWithInvalidSiblingResult(d);

            Assert.Throws<InvalidOperationException>(() =>
                query.ToList());
        }

        [Fact]
        public void InjectStaticShouldFailWithInvalidSiblingSignature()
        {
            var query = from d in data.ToInjectable(typeof(Functions))
                        select d.VelocityWithInvalidSiblingSignature();

            Assert.Throws<InvalidOperationException>(() =>
                query.ToList());
        }

        [Fact]
        public void InjectInterfaceShouldFailWithInvalidSiblingSignature()
        {
            var query = from d in data.ToInjectable(typeof(IFunctions))
                        select functions.VelocityWithInvalidSiblingSignature(d);

            Assert.Throws<InvalidOperationException>(() =>
                query.ToList());
        }

        [Fact]
        public void InjectBaseShouldFailWithInvalidSiblingSignature()
        {
            var baseFunctions = (FunctionsBase)functions;

            var query = from d in data.ToInjectable(typeof(FunctionsBase))
                        select baseFunctions.VelocityWithInvalidSiblingSignature(d);

            Assert.Throws<InvalidOperationException>(() =>
                query.ToList());
        }

        [Fact]
        public void InjectConcreteShouldFailWithInvalidSiblingSignature()
        {
            var concreteFunctions = (ConcreteFunctions)functions;

            var query = from d in data.ToInjectable(typeof(ConcreteFunctions))
                        select concreteFunctions.VelocityWithInvalidSiblingSignature(d);

            Assert.Throws<InvalidOperationException>(() =>
                query.ToList());
        }

        [Fact]
        public void InjectStaticToInstanceShouldFail()
        {
            var query = from d in data.ToInjectable(typeof(MixedFunctions))
                        select MixedFunctions.VelocityStaticToInstance(d);

            Assert.Throws<InvalidOperationException>(() =>
                query.ToList());
        }

        [Fact]
        public void InjectInstanceToStaticShouldFail()
        {
            var mixedFunctions = new MixedFunctions(1);

            var query = from d in data.ToInjectable(typeof(MixedFunctions))
                        select mixedFunctions.VelocityInstanceToStatic(d);

            Assert.Throws<InvalidOperationException>(() =>
                query.ToList());
        }

        [Fact]
        public void InjectPropertyShouldSucceedWithInternalMethod()
        {
            var query = from d in data.ToInjectable()
                        select d.VelocityInternal;

            var result = query.ToList();

            Assert.Equal(new[] { 200.0, .0, .125 }, result);
        }

        [Fact]
        public void InjectPropertyShouldSucceedWithInternalMethodWithGetter()
        {
            var query = from d in data.ToInjectable()
                        select d.VelocityInternalWithGetter;

            var result = query.ToList();

            Assert.Equal(new[] { 200.0, .0, .125 }, result);
        }

        [Fact]
        public void InjectPropertyShouldSucceedWithExternalMethod()
        {
            var query = from d in data.ToInjectable()
                        select d.VelocityExternal;

            var result = query.ToList();

            Assert.Equal(new[] { 200.0, .0, .125 }, result);
        }

        [Fact]
        public void InjectPropertyShouldSucceedWithExternalMethodWithGetter()
        {
            var query = from d in data.ToInjectable()
                        select d.VelocityExternalWithGetter;

            var result = query.ToList();

            Assert.Equal(new[] { 200.0, .0, .125 }, result);
        }

        [Fact]
        public void InjectPropertyShouldSucceedWithConvention()
        {
            var query = from d in data.ToInjectable(typeof(Dummy))
                        select d.VelocityWithConvention;

            var result = query.ToList();

            Assert.Equal(new[] { 200.0, .0, .125 }, result);
        }

        [Fact]
        public void InjectPropertyShouldSucceedWithMetadata()
        {
            var query = from d in data.ToInjectable()
                        select d.VelocityWithMetadata;

            var result = query.ToList();

            Assert.Equal(new[] { 200.0, .0, .125 }, result);
        }
    }
}
