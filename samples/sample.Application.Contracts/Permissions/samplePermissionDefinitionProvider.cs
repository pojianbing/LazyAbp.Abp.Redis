using sample.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace sample.Permissions
{
    public class samplePermissionDefinitionProvider : PermissionDefinitionProvider
    {
        public override void Define(IPermissionDefinitionContext context)
        {
            var myGroup = context.AddGroup(samplePermissions.GroupName);

            //Define your own permissions here. Example:
            //myGroup.AddPermission(samplePermissions.MyPermission1, L("Permission:MyPermission1"));
        }

        private static LocalizableString L(string name)
        {
            return LocalizableString.Create<sampleResource>(name);
        }
    }
}
