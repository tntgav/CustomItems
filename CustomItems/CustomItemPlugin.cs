using AdminToys;
using HarmonyLib;
using InventorySystem.Items.Pickups;
using LabApi.Loader.Features.Plugins;
using MEC;
using Mirror;
using RueI;
using System;
using UnityEngine;
using LabApi.Events.Handlers;
using LabApi.Features;
using LabApi.Loader.Features.Plugins;
using System;

namespace CustomItems
{
    public class CustomItemPlugin : Plugin
    {

        internal static CustomItemPlugin Instance { get; private set; }

        public override string Name { get; } = "CustomItems";

        public override string Description { get; } = "Implements a custom item system";

        public override string Author { get; } = "Niacat";

        public override Version Version { get; } = new Version(0, 1);

        public override Version RequiredApiVersion { get; } = new Version(LabApiProperties.CompiledVersion);

        public EventsHandler

        public override void Enable()
        {
            Instance = this;

            EventsHandlers = new EventsHandlers();
            EventManager.RegisterEvents(EventHandlers);
            EventManager.RegisterAllEvents(EventHandlers); //no clue if i gotta do this but its just 1 extra line so it doesnt matter that much lmao
            EventManager.RegisterEvents(this);
            RueIMain.EnsureInit();
            Harmony _harmony;
            _harmony = new Harmony("com.tpd.patches");
            _harmony.PatchAll();

            ItemPickupBase.OnPickupAdded += CustomItem.AddGlow;
            ItemPickupBase.OnPickupDestroyed += CustomItem.DeleteGlow;
            ServerConsole.AddLog("!! CUSTOM ITEM SYSTEM LOADED SUCCESSFULLY !!", ConsoleColor.Green);

            new CustomItemType(ItemType.SCP2176, TriggerTypes.TakeDamage, "planula", "Planula", "Heals you by 15 HP when hit.", Color.blue, (p, i) =>
            {
                p.Heal(15);
            }, 1.3f, 1.3f);

            new CustomItemType(ItemType.GrenadeFlash, TriggerTypes.Explode, "potatoitem", "Inverse Flashbang", "Sucks up all the light on explosion", Color.blue, (p, i) =>
            {
                LightSourceToy light = Helpers.LightSource(i.transform.position, new Color(-100f, -100f, -100f), 100f, 100f);
                Timing.CallDelayed(2f, () =>
                {
                    NetworkServer.Destroy(light.gameObject);
                });
            }, 1.3f, 1.3f);
        }


    }
}
