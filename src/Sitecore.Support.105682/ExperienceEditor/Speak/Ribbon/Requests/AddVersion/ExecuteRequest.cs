namespace Sitecore.Support.ExperienceEditor.Speak.Ribbon.Requests.AddVersion
{
    using Sitecore;
    using Sitecore.Data.Items;
    using Sitecore.Diagnostics;
    using Sitecore.ExperienceEditor.Speak.Server.Contexts;
    using Sitecore.ExperienceEditor.Speak.Server.Requests;
    using Sitecore.ExperienceEditor.Speak.Server.Responses;
    using System;

    public class ExecuteRequest : PipelineProcessorRequest<ItemContext>
    {
        public override PipelineProcessorResponseValue ProcessRequest()
        {
            base.RequestContext.ValidateContextItem();
            Item item = base.RequestContext.Item;
            if (!Sitecore.Context.IsAdministrator && (!item.Access.CanWrite() || (!item.Locking.CanLock() && !item.Locking.HasLock())))
            {
                return new PipelineProcessorResponseValue();
            }
            string[] parameters = new string[] { AuditFormatter.FormatItem(item) };
            Log.Audit(this, "Add version: {0}", parameters);
            using (new EnforceVersionPresenceDisabler())
            {
                item.Versions.AddVersion();
            }
            return new PipelineProcessorResponseValue { Value = Sitecore.Context.Site.GetCookieKey("sc_date") };
        }
    }
}
