using System;
using TLDLoader;
using UnityEngine;
using Logger = MoreUFOs.Modules.Logger;
using MoreUFOs.Components;
using System.Linq;

namespace MoreUFOs
{
	public class MoreUFOs : Mod
	{
		// Mod meta stuff.
		public override string ID => "M_MoreUFOs";
		public override string Name => "More UFOs";
		public override string Author => "M-";
		public override string Version => "1.3.0";

		internal static MoreUFOs Mod;

		internal static bool Debug = false;
		internal static Type ForceSpawn;
		internal static readonly Type[] UFOTypes = new Type[]
		{
			typeof(Trans),
			typeof(Red),
			typeof(Mothership),
			typeof(FluidStealer),
			typeof(Rustifier),
			typeof(PartDropper),
			typeof(Upsidedownifier),
			typeof(Teleporter),
			typeof(Sandstorms),
			typeof(Rainbow),
			typeof(Tumbleweeds),
			typeof(Tempus),
		};
		internal int MaxIndex = -1;

		internal float MaxScreenWidth;
		internal float MaxScreenHeight;

		public MoreUFOs()
		{
			Mod = this;

			Logger.Init();

			// Double max index to give MoreUFOs a 50% chance of being able to spawn.
			MaxIndex = UFOTypes.Length * 2;
		}

		public override void Config()
		{
			SettingAPI setting = new SettingAPI(this);
			Debug = setting.GUICheckbox(Debug, "Debug mode", 10, 10);
		}

		public override void OnLoad()
		{
			// Attempt to get the screen width.
			MaxScreenWidth = Screen.width;
			MaxScreenHeight = Screen.height;
			if (mainscript.M.SettingObj.S.IResolutionX > MaxScreenWidth)
			{
				MaxScreenWidth = mainscript.M.SettingObj.S.IResolutionX;
				MaxScreenHeight = mainscript.M.SettingObj.S.IResolutionY;
			}

			// Add components to the UFO spawn prefab.
			GameObject fedoSpawn = mainscript.M.player.gameObject.GetComponent<fedospawnscript>().prefab;
			fedoSpawn.AddComponent<UFOSelector>();
		}

		public override void OnGUI()
		{
			if (!Debug) return;

			if (mainscript.M.menu.Menu.activeSelf) return;

			GUI.Button(new Rect(MaxScreenWidth - 340, MaxScreenHeight - 20, 20, 20), "<");
			GUI.Button(new Rect(MaxScreenWidth - 320, MaxScreenHeight - 20, 300, 20), $"Force UFO: {(ForceSpawn == null ? "Random": ForceSpawn.Name)} (ENTER)");
			GUI.Button(new Rect(MaxScreenWidth - 20, MaxScreenHeight - 20, 20, 20), ">");
		}

		public override void Update()
		{
			if (!Debug) return;

			if (Input.GetKeyDown(KeyCode.LeftArrow))
			{
				if (ForceSpawn == null)
				{
					ForceSpawn = UFOTypes.Last();
					return;
				}

				int index = Array.FindIndex(UFOTypes, t => t == ForceSpawn) - 1;
				if (index == -1)
					ForceSpawn = null;
				else
					ForceSpawn = UFOTypes[index];
			}

			if (Input.GetKeyDown(KeyCode.RightArrow))
			{
				if (ForceSpawn == null)
				{
					ForceSpawn = UFOTypes.First();
					return;
				}

				int index = Array.FindIndex(UFOTypes, t => t == ForceSpawn) + 1;
				if (index > UFOTypes.Length - 1)
					ForceSpawn = null;
				else
					ForceSpawn = UFOTypes[index];
			}

			if (Input.GetKeyDown(KeyCode.Return))
			{
				mainscript.M.player.gameObject.GetComponent<fedospawnscript>().DoSpawn();
			}
		}
	}
}
