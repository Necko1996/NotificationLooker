using System.Collections.Generic;
using Colossal.UI.Binding;

namespace NotificationLooker.Systems
{
    internal class NotificationListWriter : IWriter<List<NotificationCountSystem.NotificationItem>>
    {
        public void Write(IJsonWriter writer, List<NotificationCountSystem.NotificationItem> value)
        {
            writer.ArrayBegin(value.Count);

            foreach (NotificationCountSystem.NotificationItem item in value)
            {
                writer.TypeBegin("notificationItem");

                writer.PropertyName("entityIndex");
                writer.Write(item.entityIndex);

                writer.PropertyName("entityVersion");
                writer.Write(item.entityVersion);

                writer.PropertyName("name");
                writer.Write(item.name);

                writer.PropertyName("icon");
                writer.Write(item.icon);

                writer.PropertyName("count");
                writer.Write(item.count);

                writer.TypeEnd();
            }

            writer.ArrayEnd();
        }
    }
}