using System.Collections.Generic;
using UnityEngine;
using Logger = MoreUFOs.Modules.Logger;

namespace MoreUFOs.Components
{
	[DisallowMultipleComponent]
	internal class Tempus : UFOType
	{
		private fedoscript _fedo;
		private List<GameObject> _objects = new List<GameObject>();

		public void Start()
		{
			Logger.Log($"Tempus has spawned", Logger.LogLevel.Debug);
			_fedo = gameObject.GetComponent<fedoscript>();

			Color color = new Color(0f, 0f, 0f);

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
			gameObject.GetComponent<empscript>().range = 0;
		}

		public void Update()
		{
			if (!enabled || _fedo == null) return;

			napszakvaltakozas time = mainscript.M.napszak;
			_fedo.roadHeightOffset = 30f;
			float num = Distance2D(transform.position, mainscript.M.player.transform.position);
			if ((double)num <= 500)
			{
				_fedo.roadHeightOffset = 15f;
				_fedo.highspeed = Mathf.Lerp(1f, 500f, Mathf.Clamp01(num / 500f));

				// Force time to pass quicker.
				time.tekerve = time.tekeresSpeed * Time.fixedDeltaTime * time.tekeresHelp * 0.5f;
				time.tekeres += mainscript.M.napszak.tekerve;
			}
			else
			{
				if (time.tekeres > time.nt + time.dt)
					time.tekeres = time.nt;

				if (time.tekeres > 0)
					time.tekeres -= 1f;
			}

			// Disable the road and posts underneath.
			foreach (Collider collider in Physics.OverlapSphere(transform.position, 40f))
			{
				try
				{
					if (collider.transform.root.name != "G_RoadParent") continue;

					GameObject obj = collider.transform.parent?.gameObject;
					if (obj == null || !obj.activeSelf) continue;

					_objects.Add(obj);
					obj.SetActive(false);
				}
				catch (System.Exception ex)
				{
					Logger.Log($"Tempus error. Details: {ex}");
				}
			}
		}

		public void OnDestroy()
		{
			// Turn everything back on again.
			foreach (GameObject obj in _objects)
			{
				obj.SetActive(true);
			}
		}
	}
}
