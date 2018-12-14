using Sitecore;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.ExperienceEditor.Speak.Server.Contexts;
using Sitecore.ExperienceEditor.Speak.Server.Requests;
using Sitecore.ExperienceEditor.Speak.Server.Responses;
using Sitecore.Pipelines.HasPresentation;
using Sitecore.Shell.Applications.WebEdit.Commands;
using System;

namespace Sitecore.Support.ExperienceEditor.Speak.Ribbon.Requests.Insert
{
    public class InsertRequest : PipelineProcessorRequest<ItemNameAndTemplateContext>
    {
        public override PipelineProcessorResponseValue ProcessRequest()
        {
            base.RequestContext.ValidateContextItem();
            object[] args = new object[] { base.Args.Data };
            Assert.IsNotNullOrEmpty(base.RequestContext.Name, "Could not get item name for request args:{0}", args);
            object[] objArray2 = new object[] { base.RequestContext.TemplateItemId, base.Args.Data };
            Assert.IsNotNullOrEmpty(base.RequestContext.TemplateItemId, "The template id:{0} is null or empty, request args:{1}", objArray2);
            New new2 = new New();

            using (new EnforceVersionPresenceDisabler())
            {
                BranchItem item = base.RequestContext.Item.Database.Branches[ShortID.Parse(base.RequestContext.TemplateItemId).ToID().ToString(), base.RequestContext.Item.Language];
                Assert.IsNotNull(item, typeof(BranchItem));
                new2.ExecuteCommand(base.RequestContext.Item, item, base.RequestContext.Name);
                Client.Site.Notifications.Disabled = true;
                Item item2 = Context.Workflow.AddItem(base.RequestContext.Name, item, base.RequestContext.Item);
                Client.Site.Notifications.Disabled = false;
                if (item2 == null)
                {
                    return new PipelineProcessorResponseValue();
                }
                new2.PolicyBasedUnlock(item2);
                return new PipelineProcessorResponseValue { Value = new { itemId = HasPresentationPipeline.Run(item2) ? item2.ID.ToString() : base.RequestContext.ItemId } };
            }
        }
    }
}