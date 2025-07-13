using System.Collections.Generic;
using Colossal;
using Colossal.IO.AssetDatabase;
using Game.Modding;
using Game.Settings;
using Game.UI;
using Game.UI.Widgets;

namespace Notification_Looker.Settings
{
    [FileLocation(nameof(Notification_Looker))]
    [SettingsUIGroupOrder(kButtonGroup)]
    [SettingsUIShowGroupName(kButtonGroup)]
    public class Setting : ModSetting
    {
        public const string kSection = "Main";

        public const string kButtonGroup = "Button";
        //public const string kToggleGroup = "Toggle";
        //public const string kSliderGroup = "Slider";
        //public const string kDropdownGroup = "Dropdown";

        public Setting(IMod mod) : base(mod)
        {

        }

        public override void SetDefaults()
        {
            throw new System.NotImplementedException();
        }

        [SettingsUISection(kSection, kButtonGroup)]
        public bool Button { set { Mod.log.Info("Button clicked"); } }

        //[SettingsUIButton]
        //[SettingsUIConfirmation]
        //[SettingsUISection(kSection, kButtonGroup)]
        //public bool ButtonWithConfirmation { set { Mod.log.Info("ButtonWithConfirmation clicked"); } }

        //[SettingsUISection(kSection, kToggleGroup)]
        //public bool Toggle { get; set; }

        //[SettingsUISlider(min = 0, max = 100, step = 1, scalarMultiplier = 1, unit = Unit.kDataMegabytes)]
        //[SettingsUISection(kSection, kSliderGroup)]
        //public int IntSlider { get; set; }

        //[SettingsUIDropdown(typeof(Setting), nameof(GetIntDropdownItems))]
        //[SettingsUISection(kSection, kDropdownGroup)]
        //public int IntDropdown { get; set; }

        //[SettingsUISection(kSection, kDropdownGroup)]
        //public SomeEnum EnumDropdown { get; set; } = SomeEnum.Value1;



        //public DropdownItem<int>[] GetIntDropdownItems()
        //{
        //    var items = new List<DropdownItem<int>>();

        //    for (var i = 0; i < 3; i += 1)
        //    {
        //        items.Add(new DropdownItem<int>()
        //        {
        //            value = i,
        //            displayName = i.ToString(),
        //        });
        //    }

        //    return items.ToArray();
        //}

        //public enum SomeEnum
        //{
        //    Value1,
        //    Value2,
        //    Value3,
        //}
    }
}
