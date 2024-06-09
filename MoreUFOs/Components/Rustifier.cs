using UnityEngine;
using Logger = MoreUFOs.Modules.Logger;

namespace MoreUFOs.Components
{
	internal class Rustifier : UFOType
	{
		private fedoscript fedo;

		protected override void Start()
		{
			base.Start();
			if (typeof(Rustifier) == MoreUFOs.ForceSpawn || (MoreUFOs.ForceSpawn == null && Random.Range(0, MoreUFOs.Mod.MaxIndex) == index))
			{
				Logger.Log($"Rustifier has spawned", Logger.LogLevel.Debug);
				fedo = gameObject.GetComponent<fedoscript>();
				Color color = new Color(48 / 255f, 15 / 255f, 0 / 255f);

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

				// Disable EMP.
				gameObject.GetComponent<empscript>().range = 0f;
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
			if ((double)num >= 250.0)
				return;
			this.fedo.roadHeightOffset = 20f;
			this.fedo.highspeed = Mathf.Lerp(1f, 250f, Mathf.Clamp01(num / 250f));
			foreach (Collider collider in Physics.OverlapSphere(transform.position, 100f))
			{
				GameObject root = collider.transform.root.gameObject;
				foreach (partconditionscript condition in root.GetComponentsInChildren<partconditionscript>())
				{
					condition.state = 4;
					condition.Refresh();
				}
			}
		}

		protected override void OnCollisionEnter(Collision col)
		{
			base.OnCollisionEnter(col);

			if (col.collider.transform.root == mainscript.M.player.transform.root)
				mainscript.M.player.transform.Translate(0.0f, int.MinValue, 0.0f);
			tosaveitemscript component;
			if (col.collider.gameObject.TryGetComponent<tosaveitemscript>(out component))
			{
				component.removeFromMemory = true;
				Object.Destroy(component.gameObject);
			}
			else
				col.collider.enabled = false;
		}
	}
}
