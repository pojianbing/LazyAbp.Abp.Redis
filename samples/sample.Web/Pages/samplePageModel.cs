using sample.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace sample.Web.Pages
{
    /* Inherit your PageModel classes from this class.
     */
    public abstract class samplePageModel : AbpPageModel
    {
        protected samplePageModel()
        {
            LocalizationResourceType = typeof(sampleResource);
        }
    }
}