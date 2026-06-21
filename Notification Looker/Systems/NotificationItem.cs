using Unity.Entities;

namespace NotificationLooker.Systems
{
    public struct NotificationItem
    {
        public int entityIndex;
        public int entityVersion;
        public string name;
        public string icon;

        public NotificationItem(Entity entity, string name, string icon)
        {
            this.entityIndex = entity.Index;
            this.entityVersion = entity.Version;
            this.name = name;
            this.icon = icon;
        }
    }
}