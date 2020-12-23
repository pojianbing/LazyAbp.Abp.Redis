using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared.Components;
using Volo.Abp.DependencyInjection;

namespace sample.Web
{
    [Dependency(ReplaceServices = true)]
    public class sampleBrandingProvider : DefaultBrandingProvider
    {
        public override string AppName => "sample";
    }
}
