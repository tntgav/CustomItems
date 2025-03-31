using CommandSystem;
using LabApi.Features.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Logger = LabApi.Features.Console.Logger;

namespace CustomItems.Commands
{

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class AllCustoms : ICommand
    {

        public string Command => "AllCustoms";

        public string[] Aliases => new string[] { "ListCustoms", "LC" };

        public string Description => "Lists every custom item and their properties. Does not include what the custom item actually does, unfortunately.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            List<string> args = arguments.ToList();
            
            Logger.Info(string.Join(", ", args));

            string built = string.Empty;

            foreach (CustomItemType t in CustomItemType.Types.Values)
            {
                built += $"\n{t.Name} ({t.InternalName}) is a {t.ItemType} that runs on {t.Trigger}, does {(!t.Glow ? "Not" : string.Empty)} glow {(t.Glow ? $"with color {t.UseColor.ToHex()}" : string.Empty)}, and has a decription of: \"{t.Description}\"";
            }

            response = built;
            return true;
        }

    }
}
