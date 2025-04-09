using UnityEngine;
using Logger = MoreUFOs.Modules.Logger;

namespace MoreUFOs.Components
{
	[DisallowMultipleComponent]
	internal class Tumbleweeds : UFOType
	{
		private fedoscript _fedo;
		private float _timer = 0f;

		public void Start()
		{
			Logger.Log($"Tumbleweeds has spawned", Logger.LogLevel.Debug);
			_fedo = gameObject.GetComponent<fedoscript>();

			Color color = new Color(1f, 0.647f, 0f);

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

			_fedo.roadHeightOffset = 65f;
			float num = Distance2D(transform.position, mainscript.M.player.transform.position);
			if ((double)num >= 150.0)
				return;
			_fedo.roadHeightOffset = 15f;
			_fedo.highspeed = Mathf.Lerp(1f, 150f, Mathf.Clamp01(num / 150f));

			_timer += Time.deltaTime;
			if (_timer >= 0.05f)
			{
				Vector3 basePosition = transform.position;
				Vector3 offset = new Vector3(Random.Range(-50f, 50f), Random.Range(-12f, -14f), Random.Range(-50f, 50f));
				Vector3 spawnPosition = basePosition + offset;

				Instantiate(itemdatabase.d.gthumbleweed, spawnPosition, transform.rotation);
				_timer = 0f;
			}
		}
	}
}
