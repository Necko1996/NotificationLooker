using System.Collections.Generic;
using Colossal.UI.Binding;
using Game.Rendering;
using Game.Tools;
using Game.UI;
using Unity.Entities;
using NotificationLooker.Settings;

namespace NotificationLooker.Systems
{
    internal partial class ModUISystem : UISystemBase
    {
        private UIUpdateState _state;

        private NotificationCountSystem m_notificationCountSystem;

        private Setting m_Settings;

        // C# to UI bindings for main panel.
        private ValueBinding<Setting> _bindingMainPanelUISettings = new ValueBinding<Setting>(
                UIEventName.GroupName,
                UIEventName.MainPanelUISettings,
                Mod.m_Setting,
                new MainPanelUISettingsWriter()
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
            m_Settings = Mod.m_Setting;

            // Add bindings for UI to C#.
            AddBinding(new TriggerBinding(UIEventName.GroupName, UIEventName.MainButtonClicked, MainButtonClicked));

            AddBinding(this._bindingMainPanelUISettings);
            AddBinding(this._bindingNotificationGroupedBinding);
            AddBinding(this._bindingNotificationItemBinding);

            AddBinding(new TriggerBinding<int,int>(UIEventName.GroupName, UIEventName.NotificationClicked, NotificationClicked));
            AddBinding(new TriggerBinding<float, float>(UIEventName.GroupName, UIEventName.MainPanelMoved, MainPanelMoved));
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();

            if (m_Settings.MainPanelShow && _state.Advance())
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
            m_Settings.MainPanelShow = !m_Settings.MainPanelShow;

            m_Settings.ApplyAndSave();
            // Send new visbility back to UI.
            _bindingMainPanelUISettings.TriggerUpdate();

            if(m_Settings.MainPanelShow)
            {
                _state.ForceUpdate();
                UpdateNotificationGroupedBinding();
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

        private void NotificationClicked(int entityIndex, int entityVersion)
        {
            Entity entity = new Entity
            {
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

        private void MainPanelMoved(float positionX, float positionY)
        {
            m_Settings.MainPanelX = positionX;
            m_Settings.MainPanelY = positionY;

            m_Settings.ApplyAndSave();

            _bindingMainPanelUISettings.TriggerUpdate();
        }
    }
}
