using Footprinting;
using InventorySystem.Items.Pickups;
using InventorySystem.Items.ThrowableProjectiles;
using InventorySystem.Items;
using Mirror;
using LabApi.Features.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using CustomPlayerEffects;
using AdminToys;
using LabApi.Features.Wrappers;
using ThrowableItem = InventorySystem.Items.ThrowableProjectiles.ThrowableItem;

namespace CustomItems
{
    internal static class Helpers
    {

        public static void SpawnActive(
                    this ThrowableItem item,
                    Vector3 position,
                    float fuseTime = -1f,
                    Player owner = null,
                    Vector3 velocity = new Vector3()
        )
        {
            TimeGrenade grenade = (TimeGrenade)UnityEngine.Object.Instantiate(item.Projectile, position, Quaternion.identity);
            if (fuseTime >= 0)
                grenade._fuseTime = fuseTime;
            grenade.NetworkInfo = new PickupSyncInfo(item.ItemTypeId, item.Weight, item.ItemSerial);
            grenade.PreviousOwner = new Footprint(owner != null ? owner.ReferenceHub : ReferenceHub.HostHub);
            PickupStandardPhysics phys = grenade.PhysicsModule as PickupStandardPhysics;
            phys.Rb.velocity = velocity;
            NetworkServer.Spawn(grenade.gameObject);
            grenade.ServerActivate();

            
        }

        public static ThrowableItem CreateThrowable(ItemType type, Player player = null) => (player != null ? player.ReferenceHub : ReferenceHub.HostHub)
            .inventory.CreateItemInstance(new ItemIdentifier(type, ItemSerialGenerator.GenerateNext()), false) as ThrowableItem;

        public static ItemPickupBase CreatePickup(Vector3 position, ItemBase prefab, Vector3 rotation)
        {
            ItemPickupBase clone = UnityEngine.Object.Instantiate(prefab.PickupDropModel, position, Quaternion.identity);
            clone.NetworkInfo = new PickupSyncInfo(prefab.ItemTypeId, prefab.Weight);
            clone.PreviousOwner = new Footprint(ReferenceHub.HostHub);
            clone.transform.rotation = Quaternion.Euler(rotation);
            return clone;
        }
        public static void AddEffect<T>(Player player, byte intensity, int addedDuration = 0) where T : StatusEffectBase
        {
            foreach (StatusEffectBase effect in player.ReferenceHub.playerEffectsController.AllEffects)
            {
                if (effect.GetType() == typeof(T))
                {
                    byte inten = effect.Intensity;
                    float duration = effect.Duration;
                    byte newIntensity = Math.Min(Convert.ToByte(intensity + inten), Convert.ToByte(200));
                    player.ReferenceHub.playerEffectsController.ChangeState<T>(newIntensity, duration + addedDuration);
                    ServerConsole.AddLog($"{player.Nickname} has been given/added {effect.name} of intensity {newIntensity} for {duration + addedDuration} seconds");
                }
            }
        }


        public static PrimitiveObjectToy Primitive(Vector3 pos, Vector3 scale, Vector3 rotation, Color clr, PrimitiveType primtype, bool collision = true)
        {
            foreach (GameObject value in NetworkClient.prefabs.Values)
            {
                if (value.TryGetComponent(out PrimitiveObjectToy toy))
                {
                    //instantiate the cube
                    PrimitiveObjectToy prim = UnityEngine.Object.Instantiate(toy, pos, Quaternion.Euler(rotation));
                    prim.PrimitiveType = primtype;
                    prim.MaterialColor = clr;
                    prim.transform.localScale = scale;
                    prim.PrimitiveFlags = PrimitiveFlags.Visible;
                    prim.gameObject.AddComponent<BoxCollider>();
                    prim.GetComponent<BoxCollider>().isTrigger = false;
                    prim.GetComponent<BoxCollider>().center = pos;
                    prim.GetComponent<BoxCollider>().size = scale;
                    prim.GetComponent<BoxCollider>().enabled = true;

                    if (collision) { prim.PrimitiveFlags = PrimitiveFlags.Collidable | PrimitiveFlags.Visible; } else { prim.PrimitiveFlags = PrimitiveFlags.Visible; }

                    NetworkServer.Spawn(prim.gameObject);
                    Logger.Info("Object spawned!");
                    return prim;
                }
            }
            return null;
        }

        public static LightSourceToy LightSource(Vector3 position, Color clr, float range, float intensity) // POSITION OVERLOAD (Keeps the light source stationary.)
        {
            
            Dictionary<uint, GameObject>.ValueCollection.Enumerator Enumerator = NetworkClient.prefabs.Values.GetEnumerator();
            while (Enumerator.MoveNext())
            {
                if (Enumerator.Current.TryGetComponent(out LightSourceToy adminToy))
                {
                    try
                    {
                        LightSourceToy light = UnityEngine.Object.Instantiate(adminToy, position, Quaternion.identity);
                        light.LightColor = clr;
                        light.LightRange = range;
                        light.LightIntensity = intensity;

                        NetworkServer.Spawn(light.gameObject);
                        return light;
                    }
                    catch (Exception e)
                    {
                        Log.Error($"{e}");
                    }

                }
            }
            return null;
        }

        public static LightSourceToy LightSource(Transform trans, Color clr, float range, float intensity) //TRANSFORM OVERLOAD
        {
            //Light sources, if given a transform, will stay attached to the gameobject that the transform is a child of.
            //This is useful for making certain things glow without respawning the glow every frame.
            Dictionary<uint, GameObject>.ValueCollection.Enumerator Enumerator = NetworkClient.prefabs.Values.GetEnumerator();
            while (Enumerator.MoveNext())
            {
                if (Enumerator.Current.TryGetComponent(out LightSourceToy adminToy))
                {
                    try
                    {
                        LightSourceToy light = UnityEngine.Object.Instantiate(adminToy, trans);
                        light.LightColor = clr;
                        light.LightRange = range;
                        light.LightIntensity = intensity;

                        NetworkServer.Spawn(light.gameObject);
                        return light;
                    }
                    catch (Exception e)
                    {
                        Log.Error($"{e}");
                    }

                }
            }
            return null;
        }

    }
}
