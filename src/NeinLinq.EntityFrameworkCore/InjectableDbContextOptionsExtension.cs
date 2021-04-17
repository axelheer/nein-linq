using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.DependencyInjection;

#pragma warning disable CA1307
#pragma warning disable CA1508
#pragma warning disable CA1822

namespace NeinLinq
{
    internal class InjectableDbContextOptionsExtension : IDbContextOptionsExtension
    {
        private readonly Type[] greenlist;

        public InjectableDbContextOptionsExtension(params Type[] greenlist)
        {
            this.greenlist = greenlist ?? throw new ArgumentNullException(nameof(greenlist));
        }

        public DbContextOptionsExtensionInfo Info
            => new ExtensionInfo(this);

        public void ApplyServices(IServiceCollection services)
        {
            if (services is null)
                throw new ArgumentNullException(nameof(services));

            for (var index = services.Count - 1; index >= 0; index--)
            {
                var descriptor = services[index];
                if (descriptor.ServiceType != typeof(IQueryTranslationPreprocessorFactory))
                    continue;
                if (descriptor.ImplementationType is null)
                    continue;

                // Add Injectable factory for actual implementation
                services[index] = new ServiceDescriptor(
                    descriptor.ServiceType,
                    typeof(InjectableQueryTranslationPreprocessorFactory<>)
                        .MakeGenericType(descriptor.ImplementationType),
                    descriptor.Lifetime
                );

                // Add actual implementation as it is
                services.Add(
                    new ServiceDescriptor(
                        descriptor.ImplementationType,
                        descriptor.ImplementationType,
                        descriptor.Lifetime
                    )
                );
            }

            _ = services.AddSingleton(new InjectableQueryTranslationPreprocessorOptions(greenlist));
        }

        public InjectableDbContextOptionsExtension WithParams(Type[] greenlist)
        {
            if (greenlist is null)
                throw new ArgumentNullException(nameof(greenlist));

            return new InjectableDbContextOptionsExtension(greenlist);
        }

        public void Validate(IDbContextOptions options)
        {
        }

        private class ExtensionInfo : DbContextOptionsExtensionInfo
        {
            public ExtensionInfo(IDbContextOptionsExtension extension)
                : base(extension)
            {
            }

            public override bool IsDatabaseProvider
                => false;

            public override string LogFragment
                => "LambdaInjection";

            public override long GetServiceProviderHashCode()
                => "LambdaInjection".GetHashCode();

            public override void PopulateDebugInfo(IDictionary<string, string> debugInfo)
            {
            }
        }
    }
}
