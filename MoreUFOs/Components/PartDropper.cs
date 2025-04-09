using System.Collections.Generic;
using UnityEngine;
using Logger = MoreUFOs.Modules.Logger;

namespace MoreUFOs.Components
{
	[DisallowMultipleComponent]
	internal class PartDropper : UFOType
	{
		private fedoscript fedo;
		private List<GameObject> affected = new List<GameObject>();

		public void Start()
		{
			Logger.Log($"PartDropper has spawned", Logger.LogLevel.Debug);
			fedo = gameObject.GetComponent<fedoscript>();
			Color color = new Color(3 / 255f, 65 / 255f, 219 / 255f);

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
			gameObject.GetComponent<empscript>().range = 800f;
		}

		public void Update()
		{
			if (!enabled || fedo == null)
				return;
			this.fedo.roadHeightOffset = 55f;
			float num = Distance2D(transform.position, mainscript.M.player.transform.position);
			if ((double)num >= 250.0)
				return;
			this.fedo.roadHeightOffset = 17f;
			this.fedo.highspeed = Mathf.Lerp(1f, 250f, Mathf.Clamp01(num / 250f));
			foreach (Collider collider in Physics.OverlapSphere(transform.position, 40f))
			{
				GameObject root = collider.transform.root.gameObject;

				foreach (partslotscript slot in root.GetComponentsInChildren<partslotscript>())
				{
					if (slot.part == null) continue;

					GameObject part = slot.part.gameObject;
					if (affected.Contains(part)) continue;

					// 1/4 chance of dropping the part.
					System.Random random = new System.Random();
					if (random.Next(4) == 0)
						slot.part.FallOFf();

					affected.Add(part);
				}
			}
		}
	}
}
