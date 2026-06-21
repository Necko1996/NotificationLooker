using Unity.Entities;

namespace NotificationLooker.Systems
{
    public struct NotificationGrouped
    {
        public int entityIndex;
        public int entityVersion;
        public string name;
        public string icon;
        public int count;

        public NotificationGrouped(Entity entity, string name, string icon, int count)
        {
            this.entityIndex = entity.Index;
            this.entityVersion = entity.Version;
            this.name = name;
            this.icon = icon;
            this.count = count;
        }
    }
}