using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace BuildMate
{
	public class BuildMate : Mod
	{
		public static ModKeybind[] hotkey = new ModKeybind[3];
        public override void Load()
        {
			hotkey[0] = KeybindLoader.RegisterKeybind(this, "Open world edit menu", "O");
			hotkey[1] = KeybindLoader.RegisterKeybind(this, "World edit command", "X");
            hotkey[2] = KeybindLoader.RegisterKeybind(this, "Clear selection", Keys.Tab);
		}
        public override void Unload()
        {
			GlobalWorld.menuX = 125;
			GlobalWorld.menuY = 75;
        }
    }
}
