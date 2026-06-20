using System.Collections.Generic;
using System.Linq;
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
        public readonly List<NotificationItem> notificationList = new();

        private readonly List<KeyValuePair<Entity, int>> sortedNotifications = new List<KeyValuePair<Entity, int>>();

        public struct NotificationItem
        {
            public int entityIndex;
            public int entityVersion;
            public string name;
            public string icon;
            public int count;

            public NotificationItem(Entity entity, string name, string icon, int count)
            {
                this.entityIndex = entity.Index;
                this.entityVersion = entity.Version;
                this.name = name;
                this.icon = icon;
                this.count = count;
            }
        }

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
            sortedNotifications.Clear();
            listNotificaiton.Clear();
            notificationList.Clear();
            var total = 0;

            NativeArray<ArchetypeChunk> nativeArray = iconQuery.ToArchetypeChunkArray(Allocator.TempJob);
            var prefabRefTypeHandle = GetComponentTypeHandle<PrefabRef>();

            for (int i = 0; i < nativeArray.Length; i++)
            {
                NativeArray<PrefabRef> nativeArray2 = nativeArray[i].GetNativeArray(ref prefabRefTypeHandle);

                for (int j = 0; j < nativeArray2.Length; j++, total++)
                {
                    Entity prefab = nativeArray2[j].m_Prefab;

                    if (listNotificaiton.TryGetValue(prefab, out int num))
                    {
                        listNotificaiton[prefab] = num + 1;
                    }
                    else
                    {
                        listNotificaiton.Add(prefab, 1);
                    }
                }
            }

            nativeArray.Dispose();
            sortedNotifications.AddRange(listNotificaiton);
            sortedNotifications.Sort((a, b) => b.Value.CompareTo(a.Value));

            for (int i = 0; i < sortedNotifications.Count; i++)
            {
                string prefabName = string.Empty;
                string prefabIcon = string.Empty;

                if (prefabSystem.TryGetPrefab<NotificationIconPrefab>(sortedNotifications[i].Key, out NotificationIconPrefab prefab))
                {
                    prefabName = prefab.name;
                    prefabIcon = prefab.m_Icon.name;
                }

                notificationList.Add(new NotificationItem(sortedNotifications[i].Key, prefabName, prefabIcon, sortedNotifications[i].Value));
            }

//            if (sortedItems.Any())
//            {
//                foreach (var item in sortedItems)
//                {
//                    Mod.log.Info($"{prefabSystem.GetPrefab<NotificationIconPrefab>(item.Key).name} | {item.Value}");
//                }
//            }
//
//            Mod.log.Info($"--------------------------------");
//
//            if (notificationList.Any())
//            {
//                foreach (NotificationItem item in notificationList)
//                {
//                    Mod.log.Info($"{item.name} | {item.icon} | {item.count} | Entity: {item.entityIndex}:{item.entityVersion}");
//                }
//            }
//
//            Mod.log.Info($"Total: {total}");
//            Mod.log.Info($"--------------------------------");
//            Mod.log.Info($"--------------------------------");
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            listNotificaiton.Clear();
            notificationList.Clear();
            sortedNotifications.Clear();
        }
    }
}
