using Microsoft.Extensions.DependencyInjection;

namespace Epiksoft.Results
{
    public static class ResultExtensions
    {
        public static IServiceCollection AddResultOptions(this IServiceCollection services, Action<ResultOptions> options)
        {
            var opts = new ResultOptions();
            options(opts);
            Result.Options = opts;

            return services;
        }
    }
}
