using System.Collections.Generic;
using Colossal.UI.Binding;

namespace NotificationLooker.Systems
{
    internal class NotificationGroupedListWriter : IWriter<List<NotificationGrouped>>
    {
        public void Write(IJsonWriter writer, List<NotificationGrouped> value)
        {
            writer.ArrayBegin(value.Count);

            foreach (NotificationGrouped item in value)
            {
                writer.TypeBegin("notificationGrouped");

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