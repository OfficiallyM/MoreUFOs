using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Logger = MoreUFOs.Modules.Logger;

namespace MoreUFOs.Components
{
	[DisallowMultipleComponent]
	internal class Mothership : UFOType
	{
		private fedoscript fedo;
		private List<Collider> affected = new List<Collider>();

		public void Start()
		{
			Logger.Log($"Mothership has spawned", Logger.LogLevel.Debug);
			fedo = gameObject.GetComponent<fedoscript>();

			// Set material emission colour.
			foreach (MeshRenderer renderer in gameObject.GetComponentsInChildren<MeshRenderer>())
			{
				if (!renderer.name.ToLower().Contains("body"))
				{
					renderer.material.color = Color.magenta;
					renderer.material.SetColor("_EmissionColor", Color.magenta);
				}
			}

			// Set light colour.
			Light light = gameObject.GetComponentInChildren<Light>();
			light.color = Color.magenta;
			light.range *= 10f;

			// Set scale.
			transform.localScale *= 20f;

			// Set EMP range.
			gameObject.GetComponent<empscript>().range = 500f;
		}

		public void Update()
		{
			if (!enabled || fedo == null) return;

			this.fedo.roadHeightOffset = 150f;
			float num = Distance2D(transform.position, mainscript.M.player.transform.position);
			if ((double)num >= 500.0)
				return;
			this.fedo.roadHeightOffset = 100f;
			this.fedo.highspeed = Mathf.Lerp(1f, 500f, Mathf.Clamp01(num / 500f));
		}

		public void FixedUpdate()
		{
			Collider[] colliders = Physics.OverlapSphere(transform.position, 250f);
			foreach (Collider collider in colliders)
			{
				if (collider.transform.root.gameObject == gameObject) continue;

				if (collider.attachedRigidbody != null && !affected.Contains(collider))
				{
					collider.attachedRigidbody.useGravity = false;
					collider.attachedRigidbody.AddForce(750f / collider.attachedRigidbody.mass * Vector3.up, ForceMode.Acceleration);
					affected.Add(collider);
				}
			}

			var excludedIds = new HashSet<int>(colliders.Select(c => c.GetInstanceID()));
			var enableGravity = affected.Where(c => !excludedIds.Contains(c.GetInstanceID()));

			List<Collider> remove = new List<Collider>();
			foreach (Collider collider in enableGravity)
			{
				if (collider != null && collider.attachedRigidbody != null)
				{
					collider.attachedRigidbody.useGravity = true;
					collider.attachedRigidbody.AddForce(5f / collider.attachedRigidbody.mass * Vector3.down, ForceMode.Acceleration);
				}
				remove.Add(collider);
			}

			foreach (Collider collider in remove)
				affected.Remove(collider);
		}
	}
}
