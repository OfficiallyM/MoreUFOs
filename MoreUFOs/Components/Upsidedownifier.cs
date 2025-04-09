using System.Collections.Generic;
using UnityEngine;
using Logger = MoreUFOs.Modules.Logger;

namespace MoreUFOs.Components
{
	[DisallowMultipleComponent]
	internal class Upsidedownifier : UFOType
	{
		private fedoscript fedo;
		private List<GameObject> affected = new List<GameObject>();

		public void Start()
		{
			Logger.Log($"Upsidedownifier has spawned", Logger.LogLevel.Debug);
			fedo = gameObject.GetComponent<fedoscript>();
			Color color = new Color(120 / 255f, 252 / 255f, 5 / 255f);

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
		}

		public void Update()
		{
			if (!enabled || fedo == null)
				return;
			this.fedo.roadHeightOffset = 55f;
			float num = Distance2D(transform.position, mainscript.M.player.transform.position);
			if ((double)num >= 400.0)
				return;
			this.fedo.roadHeightOffset = 20f;
			this.fedo.highspeed = Mathf.Lerp(200f, 400f, Mathf.Clamp01(num / 400f));
			foreach (Collider collider in Physics.OverlapSphere(transform.position, 40f))
			{
				GameObject root = collider.transform.root.gameObject;

				if (root.GetComponent<tosaveitemscript>() == null) continue;
				if (root.GetComponent<buildingscript>() != null) continue;
				if (affected.Contains(root)) continue;

				root.transform.Rotate(Vector3.left, 180);
				affected.Add(root);
			}
		}
	}
}
