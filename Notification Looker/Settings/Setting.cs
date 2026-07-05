using Colossal.IO.AssetDatabase;
using Game.Modding;
using Game.Settings;

namespace NotificationLooker.Settings
{
    [FileLocation("ModsSettings/NotificationLooker/NotificationLooker")]
    [SettingsUIGroupOrder(kButtonGroup)]
    [SettingsUIShowGroupName(kButtonGroup)]
    public class Setting : ModSetting
    {
        public const string kSection = "Main";

        public const string kButtonGroup = "Button";

        [SettingsUIHidden]
        public bool MainPanelShow { get; set; }

        [SettingsUIHidden]
        public float MainPanelX { get; set; } = 55f;

        [SettingsUIHidden]
        public float MainPanelY { get; set; } = 175f;

        [SettingsUIHidden]
        public float MainButtonX { get; set; } = 500f;

        [SettingsUIHidden]
        public float MainButtonY { get; set; } = 0f;

        public Setting(IMod mod) : base(mod)
        {

        }

        public override void SetDefaults()
        {
            MainPanelShow = false;
            MainPanelX = 55f;
            MainPanelY = 175f;
            MainButtonX = 500f;
            MainButtonY = 0f;
        }
    }
}
