using System;
using System.Collections;
using System.Collections.Generic;
using Colossal.IO.AssetDatabase;
using Colossal.Logging;
using Game;
using Game.Modding;
using Game.SceneFlow;
using Game.UI.Editor;
using Notification_Looker.Localisation;
using Notification_Looker.Settings;
using UnityEngine.InputSystem;

namespace Notification_Looker
{
    public class Mod : IMod
    {
        public static ILog log = LogManager.GetLogger($"{nameof(Notification_Looker)}.{nameof(Mod)}").SetShowsErrorsInUI(false);
        private Setting m_Setting;

        public void OnLoad(UpdateSystem updateSystem)
        {
            log.Info(nameof(OnLoad));

            if (GameManager.instance.modManager.TryGetExecutableAsset(this, out var asset))
            {
                log.Info($"Current mod asset at {asset.path}");
            }

            m_Setting = new Setting(this);
            m_Setting.RegisterInOptionsUI();

            // Load Localisations
            IDictionary<string, Colossal.IDictionarySource> localisation = new Dictionary<string, Colossal.IDictionarySource>();
            localisation.Add("en-US", new LocaleEN(m_Setting));
            localisation.Add("de-DE", new LocaleDE(m_Setting));

            foreach (string key in localisation.Keys)
            {
                GameManager.instance.localizationManager.AddSource(key, localisation[key]);
            }

            AssetDatabase.global.LoadSettings(nameof(Notification_Looker), m_Setting, new Setting(this));
        }

        public void OnDispose()
        {
            log.Info(nameof(OnDispose));
            if (m_Setting != null)
            {
                m_Setting.UnregisterInOptionsUI();
                m_Setting = null;
            }
        }
    }
}
