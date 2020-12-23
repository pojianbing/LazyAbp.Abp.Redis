using System;
using System.Collections.Generic;
using System.Text;
using sample.Localization;
using Volo.Abp.Application.Services;

namespace sample
{
    /* Inherit your application services from this class.
     */
    public abstract class sampleAppService : ApplicationService
    {
        protected sampleAppService()
        {
            LocalizationResource = typeof(sampleResource);
        }
    }
}
