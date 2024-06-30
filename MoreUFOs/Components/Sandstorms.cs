using System.Collections.Generic;
using UnityEngine;
using Logger = MoreUFOs.Modules.Logger;

namespace MoreUFOs.Components
{
	internal class Sandstorms : UFOType
	{
		private fedoscript fedo;
		private float timePassed = 0;
		private temporaryTurnOffGeneration temp;

		protected override void Start()
		{
			base.Start();
			if (typeof(Sandstorms) == MoreUFOs.ForceSpawn || (MoreUFOs.ForceSpawn == null && Random.Range(0, MoreUFOs.Mod.MaxIndex) == index))
			{
				Logger.Log($"Sandstorms has spawned", Logger.LogLevel.Debug);
				fedo = gameObject.GetComponent<fedoscript>();
				Color color = new Color(73 / 255f, 73 / 255f, 73 / 255f);

				// Set material emission colour.
				foreach (MeshRenderer renderer in gameObject.GetComponentsInChildren<MeshRenderer>())
				{
					if (!renderer.gameObject.name.ToLower().Contains("body"))
					{
						renderer.material.color = color;
						renderer.material.SetColor("_EmissionColor", color);
					}
				}

				// Set light colour.
				Light light = gameObject.GetComponentInChildren<Light>();
				light.color = color;

				// Set EMP range.
				gameObject.GetComponent<empscript>().range = 300f;

				temp = mainscript.M.menu.GetComponentInChildren<temporaryTurnOffGeneration>();
			}
			else
			{
				enabled = false;
				Destroy(this);
			}
		}

		public void Update()
		{
			if (temp == null) return;    

            if (!enabled || fedo == null)
				return;
			fedo.roadHeightOffset = 100f;
			float num = Distance2D(transform.position, mainscript.M.player.transform.position);
			if ((double)num >= 600.0)
				return;
			fedo.roadHeightOffset = 20f;
			fedo.highspeed = Mathf.Lerp(100f, 400f, Mathf.Clamp01(num / 400f));

			timePassed += Time.deltaTime;
			if (timePassed > 7f)
			{
				temp.sandstormspawn.SpawnAt(fedo.transform.position);
				timePassed = 0;
			}
		}
	}
}
