using NeinLinq.Tests.InjectableQueryData;
using System;
using System.Linq;
using Xunit;

namespace NeinLinq.Tests
{
    public class InjectableQueryTest
    {
        readonly IParameterizedFunctions functions = new ParameterizedFunctionsWithExpression(1);

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
            var query = from d in data.ToInjectable(typeof(IParameterizedFunctions))
                        select functions.VelocityWithoutSibling(d);

            Assert.Throws<InvalidOperationException>(() =>
                query.ToList());
        }

        [Fact]
        public void InjectBaseShouldFailWithoutSibling()
        {
            var baseFunctions = (ParameterizedFunctions)functions;

            var query = from d in data.ToInjectable(typeof(ParameterizedFunctions))
                        select baseFunctions.VelocityWithoutSibling(d);

            Assert.Throws<InvalidOperationException>(() =>
                query.ToList());
        }

        [Fact]
        public void InjectConcreteShouldFailWithoutSibling()
        {
            var concreteFunctions = (ParameterizedFunctionsWithExpression)functions;

            var query = from d in data.ToInjectable(typeof(ParameterizedFunctionsWithExpression))
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
            var query = from d in data.ToInjectable(typeof(IParameterizedFunctions))
                        select functions.VelocityWithConvention(d);

            var result = query.ToList();

            Assert.Equal(new[] { 200.0, .0, .1 }, result);
        }

        [Fact]
        public void InjectBaseShouldSucceedWithConvention()
        {
            var baseFunctions = (ParameterizedFunctions)functions;

            var query = from d in data.ToInjectable(typeof(ParameterizedFunctions))
                        select baseFunctions.VelocityWithConvention(d);

            var result = query.ToList();

            Assert.Equal(new[] { 200.0, .0, .1 }, result);
        }

        [Fact]
        public void InjectConcreteShouldSucceedWithConvention()
        {
            var concreteFunctions = (ParameterizedFunctionsWithExpression)functions;

            var query = from d in data.ToInjectable(typeof(ParameterizedFunctionsWithExpression))
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
            var baseFunctions = (ParameterizedFunctions)functions;

            var query = from d in data.ToInjectable()
                        select baseFunctions.VelocityWithMetadata(d);

            var result = query.ToList();

            Assert.Equal(new[] { 200.0, .0, .1 }, result);
        }

        [Fact]
        public void InjectConcreteShouldSucceedWithMetadata()
        {
            var concreteFunctions = (ParameterizedFunctionsWithExpression)functions;

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
            var baseFunctions = (ParameterizedFunctions)functions;

            var query = from d in data.ToInjectable()
                        select baseFunctions.VelocityWithMethodMetadata(d);

            var result = query.ToList();

            Assert.Equal(new[] { 200.0, .0, .1 }, result);
        }

        [Fact]
        public void InjectConcreteShouldSucceedWithMethodMetadata()
        {
            var concreteFunctions = (ParameterizedFunctionsWithExpression)functions;

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
            var query = from d in data.ToInjectable(typeof(IParameterizedFunctions))
                        select functions.VelocityWithInvalidSiblingResult(d);

            Assert.Throws<InvalidOperationException>(() =>
                query.ToList());
        }

        [Fact]
        public void InjectBaseShouldFailWithInvalidSiblingResult()
        {
            var baseFunctions = (ParameterizedFunctions)functions;

            var query = from d in data.ToInjectable(typeof(ParameterizedFunctions))
                        select baseFunctions.VelocityWithInvalidSiblingResult(d);

            Assert.Throws<InvalidOperationException>(() =>
                query.ToList());
        }

        [Fact]
        public void InjectConcreteShouldFailWithInvalidSiblingResult()
        {
            var concreteFunctions = (ParameterizedFunctionsWithExpression)functions;

            var query = from d in data.ToInjectable(typeof(ParameterizedFunctionsWithExpression))
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
            var query = from d in data.ToInjectable(typeof(IParameterizedFunctions))
                        select functions.VelocityWithInvalidSiblingSignature(d);

            Assert.Throws<InvalidOperationException>(() =>
                query.ToList());
        }

        [Fact]
        public void InjectBaseShouldFailWithInvalidSiblingSignature()
        {
            var baseFunctions = (ParameterizedFunctions)functions;

            var query = from d in data.ToInjectable(typeof(ParameterizedFunctions))
                        select baseFunctions.VelocityWithInvalidSiblingSignature(d);

            Assert.Throws<InvalidOperationException>(() =>
                query.ToList());
        }

        [Fact]
        public void InjectConcreteShouldFailWithInvalidSiblingSignature()
        {
            var concreteFunctions = (ParameterizedFunctionsWithExpression)functions;

            var query = from d in data.ToInjectable(typeof(ParameterizedFunctionsWithExpression))
                        select concreteFunctions.VelocityWithInvalidSiblingSignature(d);

            Assert.Throws<InvalidOperationException>(() =>
                query.ToList());
        }
    }
}
