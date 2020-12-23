using sample.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace sample.Controllers
{
    /* Inherit your controllers from this class.
     */
    public abstract class SampleController : AbpController
    {
        protected SampleController()
        {
            LocalizationResource = typeof(SampleController);
        }
    }
}