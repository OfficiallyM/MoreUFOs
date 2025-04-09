using UnityEngine;
using Logger = MoreUFOs.Modules.Logger;

namespace MoreUFOs.Components
{
	[DisallowMultipleComponent]
	internal class Rustifier : UFOType
	{
		private fedoscript _fedo;

		public void Start()
		{
			Logger.Log($"Rustifier has spawned", Logger.LogLevel.Debug);
			_fedo = gameObject.GetComponent<fedoscript>();
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

		public void Update()
		{
			if (!enabled || _fedo == null)
				return;
			this._fedo.roadHeightOffset = 55f;
			float num = Distance2D(transform.position, mainscript.M.player.transform.position);
			if ((double)num >= 250.0)
				return;
			this._fedo.roadHeightOffset = 20f;
			this._fedo.highspeed = Mathf.Lerp(1f, 250f, Mathf.Clamp01(num / 250f));
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
	}
}
