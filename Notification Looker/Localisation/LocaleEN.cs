using System.Collections.Generic;
using Colossal;
using Colossal.IO.AssetDatabase;
using Game.Modding;
using Game.Settings;
using Game.UI;
using Game.UI.Widgets;
using Setting = NotificationLooker.Settings.Setting;

namespace NotificationLooker.Localisation
{
    public class LocaleEN : IDictionarySource
    {
        private readonly Setting m_Setting;

        public LocaleEN(Setting setting)
        {
            m_Setting = setting;
        }

        public IEnumerable<KeyValuePair<string, string>> ReadEntries(IList<IDictionaryEntryError> errors, Dictionary<string, int> indexCounts)
        {
            return new Dictionary<string, string>
            {
                { m_Setting.GetSettingsLocaleID(), "NotificationLooker" },
                { m_Setting.GetOptionTabLocaleID(Setting.kSection), "Notifications1" },

                { m_Setting.GetOptionGroupLocaleID(Setting.kButtonGroup), "Notifications2" },
            };
        }

        public void Unload()
        {

        }
    }
}
