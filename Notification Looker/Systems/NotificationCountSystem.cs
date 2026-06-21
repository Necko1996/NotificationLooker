using System.Collections.Generic;
using Game;
using Game.Common;
using Game.Notifications;
using Game.Prefabs;
using Unity.Collections;
using Unity.Entities;

namespace NotificationLooker.Systems
{
    public partial class NotificationCountSystem : GameSystemBase
    {
        private PrefabSystem prefabSystem;
        private EntityQuery iconQuery;

        public readonly Dictionary<Entity, int> listNotificaiton = new();
        public readonly List<NotificationGrouped> notificationGroupedList = new();
        public readonly List<NotificationItem> notificationItemList = new();

        protected override void OnCreate()
        {
            base.OnCreate();

            prefabSystem = World.GetOrCreateSystemManaged<PrefabSystem>();

            // Active Icons
            iconQuery = GetEntityQuery(new ComponentType[] {
                ComponentType.ReadOnly<Icon>(),
                ComponentType.Exclude<Deleted>()
            });
        }

        public override int GetUpdateInterval(SystemUpdatePhase phase)
        {
            return 512;
        }

        protected override void OnUpdate()
        {
            listNotificaiton.Clear();

            notificationGroupedList.Clear();
            notificationItemList.Clear();

            var prefabRefHandle = GetComponentTypeHandle<PrefabRef>(true);
            //var iconHandle = GetComponentTypeHandle<Icon>(true);
            var entityTypeHandle = GetEntityTypeHandle();

            using (var chunks = iconQuery.ToArchetypeChunkArray(Allocator.TempJob))
            {
                for (int i = 0; i < chunks.Length; i++)
                {
                    var chunk = chunks[i];

                    var prefabRefs = chunk.GetNativeArray(ref prefabRefHandle);
                    //var icons = chunk.GetNativeArray(ref iconHandle);
                    var entities = chunk.GetNativeArray(entityTypeHandle);

                    for (int j = 0; j < prefabRefs.Length; j++)
                    {
                        Entity prefab = prefabRefs[j].m_Prefab;
                        Entity instanceEntity = entities[j];

                        //Unity.Mathematics.float3 location = icons[j].m_Location;

                        if (prefabSystem.TryGetPrefab<NotificationIconPrefab>(prefab, out var prefab2))
                        {
                            string prefabName = prefab2.name ?? string.Empty;
                            string prefabIcon = (prefab2.m_Icon != null) ? prefab2.m_Icon.name : string.Empty;

                            notificationItemList.Add(new NotificationItem(instanceEntity, prefabName, prefabIcon));
                        }

                        if (!listNotificaiton.TryGetValue(prefab, out int count))
                        {
                            listNotificaiton.Add(prefab, 1);
                        }
                        else
                        {
                            listNotificaiton[prefab] = count + 1;
                        }
                    }
                }
            }

            foreach (var item in listNotificaiton)
            {
                string prefabName = string.Empty;
                string prefabIcon = string.Empty;

                if (prefabSystem.TryGetPrefab<NotificationIconPrefab>(item.Key, out var prefab))
                {
                    prefabName = prefab.name ?? string.Empty;
                    prefabIcon = (prefab.m_Icon != null) ? prefab.m_Icon.name : string.Empty;
                }

                notificationGroupedList.Add(new NotificationGrouped(item.Key, prefabName, prefabIcon, item.Value));
            }

            // 3. Sort the final list directly in descending order (Using static zero-allocation delegate)
            notificationGroupedList.Sort(CompareNotificationItemsDescending);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            listNotificaiton.Clear();

            notificationGroupedList.Clear();
            notificationItemList.Clear();
        }

        private static int CompareNotificationItemsDescending(NotificationGrouped a, NotificationGrouped b)
        {
            int countComparison = b.count.CompareTo(a.count);

            if (countComparison != 0)
            {
                return countComparison;
            }

            return string.Compare(a.name, b.name, System.StringComparison.Ordinal);
        }
    }
}
