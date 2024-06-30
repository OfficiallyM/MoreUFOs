using System;
using System.Collections.Generic;
using UnityEngine;
using Logger = MoreUFOs.Modules.Logger;

namespace MoreUFOs.Components
{
	internal class Teleporter : UFOType
	{
		private fedoscript fedo;
		private List<GameObject> affected = new List<GameObject>();

		protected override void Start()
		{
			base.Start();
			if (typeof(Teleporter) == MoreUFOs.ForceSpawn || (MoreUFOs.ForceSpawn == null && UnityEngine.Random.Range(0, MoreUFOs.Mod.MaxIndex) == index))
			{
				Logger.Log($"Teleporter has spawned", Logger.LogLevel.Debug);
				fedo = gameObject.GetComponent<fedoscript>();
				Color color = new Color(41 / 255f, 0 / 255f, 63 / 255f);

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
				gameObject.GetComponent<empscript>().range = 400f;
			}
			else
			{
				enabled = false;
				Destroy(this);
			}
		}

		public void Update()
		{
			if (!enabled || fedo == null)
				return;
			this.fedo.roadHeightOffset = 55f;
			float num = Distance2D(transform.position, mainscript.M.player.transform.position);
			if ((double)num >= 400.0)
				return;
			this.fedo.roadHeightOffset = 30f;
			this.fedo.highspeed = Mathf.Lerp(150f, 400f, Mathf.Clamp01(num / 400f));
			foreach (Collider collider in Physics.OverlapSphere(transform.position, 75f))
			{
				GameObject root = collider.transform.root.gameObject;

				tosaveitemscript save = root.GetComponent<tosaveitemscript>();
				if (save == null) continue;
				if (root.GetComponent<buildingscript>() != null) continue;
				if (affected.Contains(root)) continue;

				Vector3[] directions = new Vector3[]
				{
					Vector3.up,
					Vector3.forward,
					Vector3.right,
					Vector3.back,
					Vector3.left
				};

				UnityEngine.Random.InitState((int)DateTime.Now.Ticks);
				int random = UnityEngine.Random.Range(0, directions.Length);
				Vector3 direction = directions[random];
				root.transform.position += direction * UnityEngine.Random.Range(2f, 20f);

				affected.Add(root);
			}
		}
	}
}
