using Pandora.Core.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using System.Diagnostics.CodeAnalysis;

namespace Pandora.Configurations
{
    [ExcludeFromCodeCoverage]
    public static class MvcOptionsExtensions
    {
        public static void UseGeneralRoutePrefix(this MvcOptions options, IRouteTemplateProvider router)
        {
            Check.NotNull(options, nameof(options));
            options.Conventions.Add(new RoutePrefixConvention(router));
        }

        public static void UseGeneralRoutePrefix(this MvcOptions options, string prefix)
        {
            Check.NotNull(options, nameof(options));
            options.UseGeneralRoutePrefix(new RouteAttribute(prefix));
        }
    }
}
