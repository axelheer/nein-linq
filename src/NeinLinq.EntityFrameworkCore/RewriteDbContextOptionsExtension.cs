using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace NeinLinq;

#pragma warning disable EF1001

internal sealed class RewriteDbContextOptionsExtension : IDbContextOptionsExtension
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

        var adapterAdded = 0;

        for (var index = services.Count - 1; index >= 0; index--)
        {
            var descriptor = services[index];
            if (descriptor.ServiceType != typeof(IQueryCompiler))
                continue;
            if (descriptor.ImplementationType is null)
                continue;

            // Add Compiler adapter for actual implementation
            services[index] = new ServiceDescriptor(
                descriptor.ServiceType,
                typeof(EntityQueryCompilerAdapter<>)
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

            adapterAdded++;
        }

        if (adapterAdded == 0)
            throw new InvalidOperationException("Unable to create rewrite adapter for actual query compiler. Please configure your query provider first!");

        _ = services.AddSingleton(new EntityQueryCompilerAdapterOptions(rewriters));
    }

    public RewriteDbContextOptionsExtension WithRewriter(ExpressionVisitor rewriter)
        => new(rewriters.Append(rewriter).ToArray());

    public void Validate(IDbContextOptions options)
    {
        // nothing to do here
    }

    private sealed class ExtensionInfo : DbContextOptionsExtensionInfo
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

        public override int GetServiceProviderHashCode()
            => 0;

        public override bool ShouldUseSameServiceProvider(DbContextOptionsExtensionInfo other)
            => true;

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

#pragma warning restore EF1001
