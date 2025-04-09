using UnityEngine;
using Logger = MoreUFOs.Modules.Logger;

namespace MoreUFOs.Components
{
	[DisallowMultipleComponent]
	internal class Red : UFOType
	{
		private fedoscript _fedo;

		public void Start()
		{
			Logger.Log($"Red has spawned", Logger.LogLevel.Debug);
			_fedo = gameObject.GetComponent<fedoscript>();

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

		public void Update()
		{
			if (!enabled || _fedo == null)
				return;
			this._fedo.roadHeightOffset = 75f;
			float num = Distance2D(transform.position, mainscript.M.player.transform.position);
			if ((double)num >= 250.0)
				return;
			this._fedo.roadHeightOffset = 10f;
			this._fedo.highspeed = Mathf.Lerp(1f, 250f, Mathf.Clamp01(num / 250f));
			foreach (Collider collider in Physics.OverlapSphere(transform.position, 25f))
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
