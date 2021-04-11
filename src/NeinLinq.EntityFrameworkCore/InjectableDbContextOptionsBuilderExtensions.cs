using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

#pragma warning disable CA1508

namespace NeinLinq
{
    /// <summary>
    /// Replaces method calls with lambda expressions.
    /// </summary>
    public static class InjectableDbContextOptionsBuilderExtensions
    {
        /// <summary>
        /// Replaces method calls with lambda expressions.
        /// </summary>
        /// <param name="optionsBuilder">The builder being used to configure the context.</param>
        /// <param name="greenlist">A list of types to inject, whether marked as injectable or not.</param>
        /// <returns>The options builder so that further configuration can be chained.</returns>
        public static DbContextOptionsBuilder WithLambdaInjection(this DbContextOptionsBuilder optionsBuilder, params Type[] greenlist)
        {
            if (optionsBuilder is null)
                throw new ArgumentNullException(nameof(optionsBuilder));
            if (greenlist is null)
                throw new ArgumentNullException(nameof(greenlist));

            var extension = GetOrCreateExtension(optionsBuilder).WithParams(greenlist);
            ((IDbContextOptionsBuilderInfrastructure)optionsBuilder).AddOrUpdateExtension(extension);

            return optionsBuilder;
        }

        private static InjectableDbContextOptionsExtension GetOrCreateExtension(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.Options.FindExtension<InjectableDbContextOptionsExtension>()
                ?? new InjectableDbContextOptionsExtension();
    }
}
