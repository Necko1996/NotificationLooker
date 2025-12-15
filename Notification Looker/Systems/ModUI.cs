using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Colossal.UI.Binding;
using Game.Tools;
using Game.UI;

namespace Notification_Looker.Systems
{
    internal partial class ModUI : UISystemBase
    {
        private NotificationCountSystem _notificationCountSystem;

        private const string MenuOpen = "MenuOpen";
        private const string NotificationCountData = "NotifcationCountData";

        private ValueBinding<bool> _panelOpenBinding = new ValueBinding<bool>(Mod.Name, MenuOpen, false);

        protected override void OnCreate()
        {
            base.OnCreate();

            AddBinding(this._panelOpenBinding);
            AddBinding(new TriggerBinding<bool>(Mod.Name, MenuOpen, open => { this._panelOpenBinding.Update(open); }));
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();

            if(this._panelOpenBinding.value)
            {
                this._notificationCountSystem = World.GetOrCreateSystemManaged<NotificationCountSystem>();
            }
        }
    }
}
