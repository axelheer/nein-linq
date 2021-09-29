using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.DependencyInjection;

namespace NeinLinq
{
    internal class RewriteDbContextOptionsExtension : IDbContextOptionsExtension
    {
        private readonly ExpressionVisitor[] rewriters;

        public RewriteDbContextOptionsExtension(params ExpressionVisitor[] rewriters)
        {
            this.rewriters = rewriters;
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

                // Add Rewrite factory for actual implementation
                services[index] = new ServiceDescriptor(
                    descriptor.ServiceType,
                    typeof(RewriteQueryTranslationPreprocessorFactory<>)
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

            _ = services.AddSingleton(new RewriteQueryTranslationPreprocessorOptions(rewriters));
        }

        public RewriteDbContextOptionsExtension WithRewriter(ExpressionVisitor rewriter)
            => new(rewriters.Append(rewriter).ToArray());

        public void Validate(IDbContextOptions options)
        {
            // nothing to do here
        }

        private class ExtensionInfo : DbContextOptionsExtensionInfo
        {
            public ExtensionInfo(IDbContextOptionsExtension extension)
                : base(extension)
            {
            }

            public new RewriteDbContextOptionsExtension Extension
                => (RewriteDbContextOptionsExtension)base.Extension;

            public override bool IsDatabaseProvider
                => false;

            public override string LogFragment
                => string.Join(", ", Extension.rewriters.Select(r => $"Rewriter={r.GetType().FullName}"));

#if NET6_0_OR_GREATER

            public override int GetServiceProviderHashCode()
                => 0;

            public override bool ShouldUseSameServiceProvider(DbContextOptionsExtensionInfo other)
                => true;

#else

            public override long GetServiceProviderHashCode()
                => 0;

#endif

            public override void PopulateDebugInfo(IDictionary<string, string> debugInfo)
            {
                var rewriters = Extension.rewriters;
                for (var index = 0; index < rewriters.Length; index++)
                {
                    debugInfo[$"RewriteQuery:Rewriter:{index}:Type"] = rewriters[index].GetType().FullName!;
                    debugInfo[$"RewriteQuery:Rewriter:{index}:Info"] = rewriters[index].ToString()!;
                }
            }
        }
    }
}
