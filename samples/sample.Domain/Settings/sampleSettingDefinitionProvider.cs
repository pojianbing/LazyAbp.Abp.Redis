using Volo.Abp.Settings;

namespace sample.Settings
{
    public class sampleSettingDefinitionProvider : SettingDefinitionProvider
    {
        public override void Define(ISettingDefinitionContext context)
        {
            //Define your own settings here. Example:
            //context.Add(new SettingDefinition(sampleSettings.MySetting1));
        }
    }
}
