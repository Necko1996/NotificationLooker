using Colossal.UI.Binding;
using NotificationLooker.Settings;

namespace NotificationLooker.Systems
{
    internal class MainPanelUISettingsWriter : IWriter<Setting>
    {
        public void Write(IJsonWriter writer, Setting value)
        {
            writer.TypeBegin("mainPanelUISettings");

            writer.PropertyName("mainPanelShow");
            writer.Write(value.MainPanelShow);

            writer.PropertyName("mainPanelX");
            writer.Write(value.MainPanelX);

            writer.PropertyName("mainPanelY");
            writer.Write(value.MainPanelY);

            writer.TypeEnd();
        }
    }
}