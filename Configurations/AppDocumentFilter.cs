using Pandora.Core.Utils;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Pandora.Configurations
{
    [ExcludeFromCodeCoverage]
    public class AppDocumentFilter : IDocumentFilter
    {
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            Check.NotNull(swaggerDoc, nameof(swaggerDoc));

            var paths = new OpenApiPaths();
            foreach (var path in swaggerDoc.Paths)
            {
                var key = path.Key
                            .Replace(
                                "v{version}",
                                swaggerDoc.Info.Version,
                                StringComparison.CurrentCultureIgnoreCase
                            );
                paths.TryAdd(key, path.Value);
            }
            swaggerDoc.Paths = paths;
        }
    }
}