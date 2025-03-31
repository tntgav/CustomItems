using CommandSystem;
using InventorySystem;
using InventorySystem.Items;
using InventorySystem.Items.Pickups;
using Mirror;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using LabApi.Features.Wrappers;

namespace CustomItems.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class DropCustomItem : ICommand
    {

        public string Command => "DropCustomItem";

        public string[] Aliases => new string[] { "DropCustom", "drc" };

        public string Description => "Drops a custom item at YOUR feet, no current parameters to drop it at someone else.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            List<string> args = arguments.ToList();
            Dictionary<string, CustomItemType> types = CustomItemType.Types;

            string todrop = args[0].ToLower();
            
            Player commandsender = Player.Get(sender);
            Dictionary<string, CustomItemType> newtypes = new Dictionary<string, CustomItemType>();
            foreach (KeyValuePair<string, CustomItemType> kvp in types)
            {
                newtypes.Add(kvp.Key.ToLower(), kvp.Value);
            }

            if (newtypes.Keys.Contains(todrop)) {
                //player input a valid custom item
                CustomItemType custom = newtypes[todrop];
                ItemBase ibase = InventoryItemLoader.AvailableItems[custom.ItemType];
                ItemPickupBase ipb = Helpers.CreatePickup(commandsender.Position, ibase, Vector3.zero);
                new CustomItem(custom, ibase, ibase.ItemSerial); //really weird way to do it, i know..
                NetworkServer.Spawn(ipb.gameObject);
            }

            response = "Something went wrong, you likely didn't input a valid custom item!";
            return false;
        }

    }
}
