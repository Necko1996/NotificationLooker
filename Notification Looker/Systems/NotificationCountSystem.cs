using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;
using Colossal.Serialization.Entities;
using Colossal.UI;
using Game;
using Game.Common;
using Game.Notifications;
using Game.Prefabs;
using Game.UI.InGame;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Notification_Looker.Systems
{
    public partial class NotificationCountSystem : GameSystemBase
    {
        private PrefabSystem prefabSystem;
        private EntityQuery iconQuery;
        private EntityQuery notificationIconDisplayDataQuery;

        private EntityQuery pollutionNotificationParameterQuery;
        private readonly Dictionary<Entity, int> EntityDictionary = new();

        protected override void OnCreate()
        {
            base.OnCreate();

            prefabSystem = World.GetOrCreateSystemManaged<PrefabSystem>();

            // Active Icons
            iconQuery = GetEntityQuery(new ComponentType[] {
                ComponentType.ReadOnly<Icon>(),
                ComponentType.Exclude<Deleted>()
            });

            // All possible Icons
            notificationIconDisplayDataQuery = GetEntityQuery(
               new ComponentType[]
               {
                    ComponentType.ReadOnly<NotificationIconDisplayData>(),
               }
            );
        }

        public override int GetUpdateInterval(SystemUpdatePhase phase)
        {
            return 512;
        }

        protected override void OnUpdate()
        {
            EntityDictionary.Clear();
            var total = 0;

            NativeArray<ArchetypeChunk> nativeArray = iconQuery.ToArchetypeChunkArray(Allocator.TempJob);
            var prefabRefTypeHandle = GetComponentTypeHandle<PrefabRef>();

            for (int i = 0; i < nativeArray.Length; i++)
            {
                NativeArray<PrefabRef> nativeArray2 = nativeArray[i].GetNativeArray(ref prefabRefTypeHandle);

                for (int j = 0; j < nativeArray2.Length; j++, total++)
                {
                    Entity prefab = nativeArray2[j].m_Prefab;

                    if (EntityDictionary.TryGetValue(prefab, out int num))
                    {
                        EntityDictionary[prefab] = num + 1;
                    }
                    else
                    {
                        EntityDictionary.Add(prefab, 1);
                    }
                }
            }

            nativeArray.Dispose();
            var orderedEntityDictionary = EntityDictionary.OrderByDescending(x => x.Value);

            if (EntityDictionary.Any())
            {
                foreach (var item in orderedEntityDictionary)
                {
                    Mod.log.Info($"{prefabSystem.GetPrefab<NotificationIconPrefab>(item.Key).name} | {item.Value}");
                }
            }

            Mod.log.Info($"Total: {total}");
            Mod.log.Info($"--------------------------------");
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            EntityDictionary.Clear();

            EntityManager.DestroyEntity(notificationIconDisplayDataQuery);
            EntityManager.DestroyEntity(iconQuery);
        }

        public void DebugNotificationIconPrefab(EntityQuery query)
        {
            Mod.log.Info($"Debug NotificationIconPrefab");
            
            var entityArray = query.ToEntityArray(Allocator.TempJob);

            foreach (var item in entityArray)
            {
                Mod.log.Info($"{prefabSystem.GetPrefab<NotificationIconPrefab>(item)}");
            }

            entityArray.Dispose();

            Mod.log.Info($"Debug NotificationIconPrefab completed");
        }
    }
}
