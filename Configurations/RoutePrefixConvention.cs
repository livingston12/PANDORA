using Pandora.Core.Utils;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Routing;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace Pandora.Configurations
{
    [ExcludeFromCodeCoverage]
    public class RoutePrefixConvention : IApplicationModelConvention
    {
        private readonly IRouteTemplateProvider route;
        public RoutePrefixConvention(IRouteTemplateProvider route)
        {
            this.route = route;
        }

        public void Apply(ApplicationModel application)
        {
            Check.NotNull(application, nameof(application));
            DoApply(application);
        }

        private void DoApply(ApplicationModel application)
        {
            var explorerSelectors = application
                                        .Controllers
                                        .SelectMany(c => c.Selectors,
                                            (parent, child) => (
                                                parent.ApiExplorer,
                                                Selector: child
                                            )
                                        );

            foreach (var (ApiExplorer, Selector) in explorerSelectors)
            {
                var routePrefix = new AttributeRouteModel(route);
                var stringBuilder = new StringBuilder()
                                        .Append(routePrefix.Template)
                                        .Append("v1/");
                routePrefix.Template = stringBuilder.ToString();

                Selector.AttributeRouteModel = Selector.AttributeRouteModel != null
                    ?
                        AttributeRouteModel
                            .CombineAttributeRouteModel(
                                routePrefix,
                                Selector.AttributeRouteModel) :
                        routePrefix;
            }
        }
    }
}