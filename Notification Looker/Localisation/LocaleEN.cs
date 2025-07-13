using System.Collections.Generic;
using Colossal;
using Colossal.IO.AssetDatabase;
using Game.Modding;
using Game.Settings;
using Game.UI;
using Game.UI.Widgets;
using Setting = Notification_Looker.Settings.Setting;

namespace Notification_Looker.Localisation
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
                { m_Setting.GetSettingsLocaleID(), "Notification Looker" },
                { m_Setting.GetOptionTabLocaleID(Setting.kSection), "MainEN" },

                { m_Setting.GetOptionGroupLocaleID(Setting.kButtonGroup), "Buttons1EN" },
                //{ m_Setting.GetOptionGroupLocaleID(Setting.kToggleGroup), "Toggle" },
                //{ m_Setting.GetOptionGroupLocaleID(Setting.kSliderGroup), "Sliders" },
                //{ m_Setting.GetOptionGroupLocaleID(Setting.kDropdownGroup), "Dropdowns" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.Button)), "Button2EN" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.Button)), $"EN-Simple single button. It should be bool property with only setter or use [{nameof(SettingsUIButtonAttribute)}] to make button from bool property with setter and getter" },

                //{ m_Setting.GetOptionLabelLocaleID(nameof(Setting.ButtonWithConfirmation)), "Button with confirmation" },
                //{ m_Setting.GetOptionDescLocaleID(nameof(Setting.ButtonWithConfirmation)), $"Button can show confirmation message. Use [{nameof(SettingsUIConfirmationAttribute)}]" },
                //{ m_Setting.GetOptionWarningLocaleID(nameof(Setting.ButtonWithConfirmation)), "is it confirmation text which you want to show here?" },

                //{ m_Setting.GetOptionLabelLocaleID(nameof(Setting.Toggle)), "Toggle" },
                //{ m_Setting.GetOptionDescLocaleID(nameof(Setting.Toggle)), $"Use bool property with setter and getter to get toggable option" },

                //{ m_Setting.GetOptionLabelLocaleID(nameof(Setting.IntSlider)), "Int slider" },
                //{ m_Setting.GetOptionDescLocaleID(nameof(Setting.IntSlider)), $"Use int property with getter and setter and [{nameof(SettingsUISliderAttribute)}] to get int slider" },

                //{ m_Setting.GetOptionLabelLocaleID(nameof(Setting.IntDropdown)), "Int dropdown" },
                //{ m_Setting.GetOptionDescLocaleID(nameof(Setting.IntDropdown)), $"Use int property with getter and setter and [{nameof(SettingsUIDropdownAttribute)}(typeof(SomeType), nameof(SomeMethod))] to get int dropdown: Method must be static or instance of your setting class with 0 parameters and returns {typeof(DropdownItem<int>).Name}" },

                //{ m_Setting.GetOptionLabelLocaleID(nameof(Setting.EnumDropdown)), "Simple enum dropdown" },
                //{ m_Setting.GetOptionDescLocaleID(nameof(Setting.EnumDropdown)), $"Use any enum property with getter and setter to get enum dropdown" },

                //{ m_Setting.GetEnumValueLocaleID(Setting.SomeEnum.Value1), "Value 1" },
                //{ m_Setting.GetEnumValueLocaleID(Setting.SomeEnum.Value2), "Value 2" },
                //{ m_Setting.GetEnumValueLocaleID(Setting.SomeEnum.Value3), "Value 3" },

            };
        }

        public void Unload()
        {

        }
    }
}
