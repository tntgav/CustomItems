using AdminToys;
using InventorySystem.Items;
using InventorySystem.Items.Pickups;
using LabApi.Features.Wrappers;
using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CustomItems
{
    public class CustomItem
    {
        //STATIC THINGS

        public static List<CustomItem> Customs = new List<CustomItem>();

        private static Dictionary<ushort, LightSourceToy> ItemGlows = new Dictionary<ushort, LightSourceToy>();
        public static List<ushort> Ids { get { return Customs.Select(a => a.Item.ItemSerial).ToList(); } }
        public static void HandleCustom(ItemBase item, Player plr)
        {
            if (item == null) { return; }
        }

        public static void AddGlow(ItemPickupBase i)
        {
            if (Ids.Contains(i.Info.Serial))
            {
                CustomItem citem = Customs.Find(p => p.Item.ItemSerial == i.Info.Serial);
                ItemGlows.Add(i.Info.Serial, Helpers.LightSource(i.transform, citem.Type.UseColor, citem.Type.GlowRange, citem.Type.GlowIntensity));
            }
        }

        public static void DeleteGlow(ItemPickupBase i)
        {
            if (ItemGlows.ContainsKey(i.Info.Serial))
            {
                LightSourceToy glow = ItemGlows[i.Info.Serial];
                NetworkServer.Destroy(glow.gameObject);
                ItemGlows.Remove(i.Info.Serial);
            }
        }

        //INSTANCE THINGS

        readonly ItemBase Item;
        readonly CustomItemType Type;

        public CustomItem(CustomItemType type, ItemBase item, ushort serial)
        {
            Type = type;
            Item = item;
            Customs.Add(this);
        }

        public void UseCustom(Player owner)
        {
            Type.Use(owner, Item);
        }
    }

    public class CustomItemType
    {
        public static Dictionary<string, CustomItemType> Types = new Dictionary<string, CustomItemType>();

        public ItemType ItemType;
        public TriggerTypes Trigger;
        public string InternalName;
        public string Name;
        public string Description;
        public Color UseColor;
        public bool Glow;
        public float GlowRange;
        public float GlowIntensity;
        public Action<Player, ItemBase> Use;

        public bool AllowReloading;
        public bool RegenerateAmmo;
        public int RegenerateAmount;
        public float RegenerateRate;

        public CustomItemType(ItemType itemType, TriggerTypes trigger, string internalName, string name, string description, Color useColor, Action<Player, ItemBase> use, float glowRange, float glowIntensity) //glow enabled
        {
            ItemType = itemType;
            Trigger = trigger;
            InternalName = internalName;
            Name = name;
            Description = description;
            UseColor = useColor;
            Glow = true;
            GlowRange = glowRange;
            GlowIntensity = glowIntensity;
            Use = use;
            Types.Add(internalName, this);
            
        }

        public CustomItemType(ItemType itemType, TriggerTypes trigger, string internalName, string name, string description, Color useColor, Action<Player, ItemBase> use) //no glow
        {
            ItemType = itemType;
            Trigger = trigger;
            InternalName = internalName;
            Name = name;
            Description = description;
            UseColor = useColor;
            Glow = false;
            GlowRange = 0;
            GlowIntensity = 0;
            Use = use;
            Types.Add(internalName, this);
        }

        public void AmmoSettings(bool reload, bool regen, int regena, float regenr)
        { //bad code practice probably but i didnt want 1 trillion variables in a single constructor...
            AllowReloading = reload;
            RegenerateAmmo = regen;
            RegenerateAmount = regena;
            RegenerateRate = regenr;
            if (Types.ContainsKey(InternalName)) { Types[InternalName] = this; } else { Types.Add(InternalName, this); } //it should always have it but i'd like to be safe
        }
    }

    public enum TriggerTypes //unfinished
    {
        Shoot,
        Hit,
        TakeDamage,
        Die,
        SpawnWave,
        PerFrame,
        Explode,
    }
}
