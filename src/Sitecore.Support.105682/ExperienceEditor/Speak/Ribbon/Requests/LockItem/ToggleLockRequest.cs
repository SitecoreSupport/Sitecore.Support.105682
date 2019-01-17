using Sitecore.Data.Items;
using Sitecore.ExperienceEditor.Speak.Server.Contexts;
using Sitecore.ExperienceEditor.Speak.Server.Requests;
using Sitecore.ExperienceEditor.Speak.Server.Responses;
using Sitecore.Web;
using System;

namespace Sitecore.Support.ExperienceEditor.Speak.Ribbon.Requests.LockItem
{
    public class ToggleLockRequest : PipelineProcessorRequest<ItemContext>
    {
        private void HandleVersionCreating(Item finalItem)
        {
            if (base.RequestContext.Item.Version.Number != finalItem.Version.Number)
            {
                WebUtil.SetCookieValue(base.RequestContext.Site.GetCookieKey("sc_date"), string.Empty, DateTime.MinValue);
            }
        }

        public override PipelineProcessorResponseValue ProcessRequest()
        {
            base.RequestContext.ValidateContextItem();
            using (new EnforceVersionPresenceDisabler())
            {
                Item finalItem = this.SwitchLock(base.RequestContext.Item);
                this.HandleVersionCreating(finalItem);
                return new PipelineProcessorResponseValue
                {
                    Value = new
                    {
                        Locked = finalItem.Locking.IsLocked(),
                        Version = finalItem.Version.Number
                    }
                };
            }
        }

        protected Item SwitchLock(Item item)
        {
            if (item.Locking.IsLocked())
            {
                item.Locking.Unlock();
                return item;
            }
            if (!Context.User.IsAdministrator)
            {
                return Context.Workflow.StartEditing(item);
            }
            item.Locking.Lock();
            return item;
        }
    }
}
