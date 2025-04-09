using UnityEngine;
using Logger = MoreUFOs.Modules.Logger;

namespace MoreUFOs.Components
{
	[DisallowMultipleComponent]
	internal class Trans : UFOType
	{
		private fedoscript fedo;
		private bool swapped = false;

		public void Start()
		{
			Logger.Log($"Trans has spawned", Logger.LogLevel.Debug);
			fedo = gameObject.GetComponent<fedoscript>();

			// Set material emission colour.
			foreach (MeshRenderer renderer in gameObject.GetComponentsInChildren<MeshRenderer>())
			{
				string name = renderer.name.ToLower();
				switch (name)
				{
					case "cabin":
						renderer.material.color = new Color(0.96078f, 0.66275f, 0.72157f);
						renderer.material.SetColor("_EmissionColor", new Color(0.96078f, 0.66275f, 0.72157f));
						break;
					case "lights":
						renderer.material.color = Color.white;
						renderer.material.SetColor("_EmissionColor", Color.white);
						break;
				}
			}

			// Set light colour.
			Light light = gameObject.GetComponentInChildren<Light>();
			light.color = new Color(0.35686f, 0.80784f, 0.98039f);
			light.range *= 1.5f;

			// Disable EMP.
			gameObject.GetComponent<empscript>().range = 0;
		}

		public void Update()
		{
			if (!enabled || fedo == null) return;

			this.fedo.roadHeightOffset = 65f;
			float num = Distance2D(transform.position, mainscript.M.player.transform.position);
			if ((double)num >= 150.0)
				return;
			this.fedo.roadHeightOffset = 15f;
			this.fedo.highspeed = Mathf.Lerp(1f, 150f, Mathf.Clamp01(num / 150f));
			if (swapped) return;
			if (Distance2D(transform.position, mainscript.M.player.transform.position) <= 50)
			{
				playermodeloutfitscript outfit = mainscript.M.player.outfit;
				int max = outfit.characters.Length - 1;
				int newChar = outfit.selectedCharacter + 1;
				if (newChar > max)
					newChar = 0;
				mainscript.M.player.outfit.selectedCharacter = newChar;
				outfit.refresh = true;
				outfit.SetRandom(false);
				swapped = true;
			}
		}
	}
}
