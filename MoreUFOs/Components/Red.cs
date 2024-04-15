using UnityEngine;
using Logger = MoreUFOs.Modules.Logger;

namespace MoreUFOs.Components
{
	internal class Red : UFOType
	{
		private fedoscript fedo;

		protected override void Start()
		{
			base.Start();
			if (typeof(Red) == MoreUFOs.ForceSpawn || (MoreUFOs.ForceSpawn == null && Random.Range(0, MoreUFOs.Mod.MaxIndex) == index))
			{
				Logger.Log($"Red has spawned", Logger.LogLevel.Debug);
				fedo = gameObject.GetComponent<fedoscript>();

				// Set material emission colour.
				foreach (MeshRenderer renderer in gameObject.GetComponentsInChildren<MeshRenderer>())
				{
					if (!renderer.gameObject.name.ToLower().Contains("body"))
					{
						renderer.material.color = Color.red;
						renderer.material.SetColor("_EmissionColor", Color.red);
					}
				}

				// Set light colour.
				Light light = gameObject.GetComponentInChildren<Light>();
				light.color = Color.red;
				light.range *= 1.5f;

				// Set EMP range.
				gameObject.GetComponent<empscript>().range = 1500f;
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
			this.fedo.roadHeightOffset = 75f;
			float num = Distance2D(transform.position, mainscript.M.player.transform.position);
			if ((double)num >= 250.0)
				return;
			this.fedo.roadHeightOffset = 10f;
			this.fedo.highspeed = Mathf.Lerp(1f, 250f, Mathf.Clamp01(num / 250f));
			foreach (Collider collider in Physics.OverlapSphere(this.transform.position, 25f))
			{
				if (collider.attachedRigidbody != null)
				{
					Vector3 vector3 = this.transform.position - collider.transform.position;
					collider.attachedRigidbody.AddForce(25f * vector3.normalized * Mathf.Abs(25f - Distance3D(this.transform.position, collider.transform.position)));
					if (mainscript.M.player.lastCar != null && mainscript.M.player.lastCar.transform.root == collider.transform.root)
						mainscript.M.player.lastCar.DamageStuff(Mathf.Min(mainscript.M.crashSpeedMinBlood * 0.9f, mainscript.M.crashSpeedMinHorn * 0.9f));
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
