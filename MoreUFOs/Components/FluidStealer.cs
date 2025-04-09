using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Logger = MoreUFOs.Modules.Logger;

namespace MoreUFOs.Components
{
	[DisallowMultipleComponent]
	internal class FluidStealer : UFOType
	{
		private fedoscript _fedo;
		private List<GameObject> _affected = new List<GameObject>();

		public void Start()
		{
			Logger.Log($"FluidStealer has spawned", Logger.LogLevel.Debug);
			_fedo = gameObject.GetComponent<fedoscript>();

			// Set material emission colour.
			foreach (MeshRenderer renderer in gameObject.GetComponentsInChildren<MeshRenderer>())
			{
				if (!renderer.name.ToLower().Contains("body"))
				{
					renderer.material.color = Color.yellow;
					renderer.material.SetColor("_EmissionColor", Color.yellow);
				}
			}

			// Set light colour.
			Light light = gameObject.GetComponentInChildren<Light>();
			light.color = Color.yellow;
			light.range *= 1.5f;

			// Set EMP range.
			gameObject.GetComponent<empscript>().range = 500f;
		}

		public void Update()
		{
			if (!enabled || _fedo == null) return;

			this._fedo.roadHeightOffset = 75f;
			float num = Distance2D(transform.position, mainscript.M.player.transform.position);
			if ((double)num >= 250.0)
				return;
			this._fedo.roadHeightOffset = 20f;
			this._fedo.highspeed = Mathf.Lerp(1f, 250f, Mathf.Clamp01(num / 250f));

			foreach (Collider collider in Physics.OverlapSphere(transform.position, 80f))
			{
				GameObject gameObject = collider.gameObject;
				
				if (_affected.Contains(gameObject)) continue;

				tankscript[] tanks = gameObject.GetComponentsInChildren<tankscript>().Distinct().ToArray();
				foreach (tankscript tank in tanks)
				{
					// Don't steal the player's blood/piss/shit.
					if (tank.gameObject.name.ToLower() == "player" || tank.transform.parent?.name.ToLower() == "head") continue;

					List<mainscript.fluid> fluids = new List<mainscript.fluid>(tank.F.fluids);

					tank.F.fluids.Clear();

					foreach (var fluid in fluids)
					{
						tank.F.ChangeOne(Random.Range(0.1f, fluid.amount), fluid.type);
					}
				}
				_affected.Add(gameObject);
			}
		}
	}
}
