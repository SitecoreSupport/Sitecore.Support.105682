using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.ExperienceEditor.Speak.Server.Contexts;
using Sitecore.ExperienceEditor.Speak.Server.Requests;
using Sitecore.ExperienceEditor.Speak.Server.Responses;
using Sitecore.Pipelines.HasPresentation;
using Sitecore.Shell.Applications.WebEdit.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sitecore.Support.ExperienceEditor.Speak.Ribbon.Requests.Insert
{
    public class InsertRequest : PipelineProcessorRequest<ItemNameAndTemplateContext>
    {
        public override PipelineProcessorResponseValue ProcessRequest()
        {
            base.RequestContext.ValidateContextItem();
            Assert.IsNotNullOrEmpty(base.RequestContext.Name, "Could not get item name for request args:{0}", new object[]
            {
                base.Args.Data
            });
            Assert.IsNotNullOrEmpty(base.RequestContext.TemplateItemId, "The template id:{0} is null or empty, request args:{1}", new object[]
            {
                base.RequestContext.TemplateItemId,
                base.Args.Data
            });
            New @new = new New();
            using (new EnforceVersionPresenceDisabler())
            {
                BranchItem branchItem = base.RequestContext.Item.Database.Branches[ShortID.Parse(base.RequestContext.TemplateItemId).ToID().ToString(), base.RequestContext.Item.Language];
                Assert.IsNotNull(branchItem, typeof(BranchItem));
                @new.ExecuteCommand(base.RequestContext.Item, branchItem, base.RequestContext.Name);
                Client.Site.Notifications.Disabled = true;
                Item item = Sitecore.Context.Workflow.AddItem(base.RequestContext.Name, branchItem, base.RequestContext.Item);
                Client.Site.Notifications.Disabled = false;
                if (item == null)
                {
                    return new PipelineProcessorResponseValue();
                }
                @new.PolicyBasedUnlock(item);
                return new PipelineProcessorResponseValue
                {
                    Value = new
                    {
                        itemId = (HasPresentationPipeline.Run(item) ? item.ID.ToString() : base.RequestContext.ItemId)
                    }
                };
            }                
        }
    }
}