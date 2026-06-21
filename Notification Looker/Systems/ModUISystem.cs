using System.Collections.Generic;
using Colossal.UI.Binding;
using Game.Rendering;
using Game.Tools;
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

        private ValueBinding<List<NotificationGrouped>> _bindingNotificationGroupedBinding = new ValueBinding<List<NotificationGrouped>>(
                UIEventName.GroupName,
                UIEventName.NotificationGroupedData,
                new List<NotificationGrouped>(),
                new NotificationGroupedListWriter()
            );

        private ValueBinding<List<NotificationItem>> _bindingNotificationItemBinding = new ValueBinding<List<NotificationItem>>(
                UIEventName.GroupName,
                UIEventName.NotificationItemData,
                new List<NotificationItem>(),
                new NotificationItemListWriter()
            );

        protected override void OnCreate()
        {
            base.OnCreate();

            _state = UIUpdateState.Create(World, 512);

            m_notificationCountSystem = World.GetOrCreateSystemManaged<NotificationCountSystem>();

            // Add bindings for UI to C#.
            AddBinding(new TriggerBinding(UIEventName.GroupName, UIEventName.MainButtonClicked, MainButtonClicked));

            AddBinding(this._bindingMainPanelUISettings);
            AddBinding(this._bindingNotificationGroupedBinding);
            AddBinding(this._bindingNotificationItemBinding);

            AddBinding(new TriggerBinding<int,int>(UIEventName.GroupName, UIEventName.NotificationClicked, NotificationClicked));
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();

            if (Mod.ShowPanel && _state.Advance())
            {
                UpdateNotificationGroupedBinding();
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
                UpdateNotificationGroupedBinding();
            }
        }

        private void NotificationClicked(int entityIndex, int entityVersion)
        {
            Entity entity = new Entity { 
                Index = entityIndex, 
                Version = entityVersion 
            };

            if (EntityManager.Exists(entity))
            {
                var toolSystem = World.GetOrCreateSystemManaged<ToolSystem>();
                var cameraSystem = World.GetOrCreateSystemManaged<CameraUpdateSystem>();

                toolSystem.selected = entity;

                if (cameraSystem != null)
                {
                    cameraSystem.orbitCameraController.followedEntity = entity;
                    cameraSystem.orbitCameraController.TryMatchPosition(cameraSystem.activeCameraController);
                    cameraSystem.activeCameraController = cameraSystem.orbitCameraController;
                }
            }
        }

        private void UpdateNotificationGroupedBinding()
        {
            _bindingNotificationGroupedBinding.Update(
               new List<NotificationGrouped>(
                    m_notificationCountSystem.notificationGroupedList
               )
            );

            _bindingNotificationItemBinding.Update(
                new List<NotificationItem>(
                    m_notificationCountSystem.notificationItemList
                )
            );
        }
    }
}
