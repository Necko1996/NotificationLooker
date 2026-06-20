using System.Collections.Generic;
using Colossal.UI.Binding;
using Game.UI;
using Unity.Entities;

namespace NotificationLooker.Systems
{
    internal partial class ModUISystem : UISystemBase
    {
        private UIUpdateState _state;

        private NotificationCountSystem m_notificationCountSystem;

        // C# to UI bindings for main panel.
        private ValueBinding<bool> _bindingMainPanelUISettings = new ValueBinding<bool>(
                UIEventName.GroupName,
                UIEventName.MainPanelUISettings,
                Mod.ShowPanel
            );

        private ValueBinding<List<NotificationCountSystem.NotificationItem>> _bindingNotificationBinding = new ValueBinding<List<NotificationCountSystem.NotificationItem>>(
                UIEventName.GroupName,
                UIEventName.NotificationData,
                new List<NotificationCountSystem.NotificationItem>(),
                new NotificationListWriter()
            );

        protected override void OnCreate()
        {
            base.OnCreate();

            _state = UIUpdateState.Create(World, 512);

            m_notificationCountSystem = World.GetOrCreateSystemManaged<NotificationCountSystem>();

            // Add bindings for UI to C#.
            AddBinding(new TriggerBinding(UIEventName.GroupName, UIEventName.MainButtonClicked, MainButtonClicked));

            AddBinding(this._bindingMainPanelUISettings);
            AddBinding(this._bindingNotificationBinding);
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();

            if (Mod.ShowPanel && _state.Advance())
            {
                UpdateNotificationBinding();
            }
        }


        /// <summary>
        /// Event callback for main button clicked.
        /// </summary>
        private void MainButtonClicked()
        {
            // Toggle main panel visibility.
            Mod.ShowPanel = !Mod.ShowPanel;

            // Send new visbility back to UI.
            _bindingMainPanelUISettings.Update(Mod.ShowPanel);

            if(Mod.ShowPanel)
            {
                _state.ForceUpdate();
                UpdateNotificationBinding();
            }
        }

        private void UpdateNotificationBinding()
        {
            _bindingNotificationBinding.Update(
               new List<NotificationCountSystem.NotificationItem>(
                    m_notificationCountSystem.notificationList
               )
            );
        }
    }
}
