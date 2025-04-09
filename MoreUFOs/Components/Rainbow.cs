using UnityEngine;
using Logger = MoreUFOs.Modules.Logger;

namespace MoreUFOs.Components
{
	[DisallowMultipleComponent]
	internal class Rainbow : UFOType
	{
		private fedoscript _fedo;
		private Color[] _colours = new Color[]
		{
			new Color(1f, 0f, 0f),
			new Color(1f, 0.5f, 0f),
			new Color(1f, 1f, 0f),
			new Color(0f, 1f, 0f),
			new Color(0f, 0f, 1f),
			new Color(0.29f, 0f, 0.51f),
			new Color(0.56f, 0f, 1f),
		};
		private int _colourIndex = 0;
		private float _timer = 0f;

		public void Start()
		{
			Logger.Log($"Rainbow has spawned", Logger.LogLevel.Debug);
			_fedo = gameObject.GetComponent<fedoscript>();

			InvokeRepeating("UpdateLights", 0, 0.1f);

			// Set EMP range.
			gameObject.GetComponent<empscript>().range = 300f;
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
			if (_timer >= 1f)
			{
				foreach (Collider collider in Physics.OverlapSphere(transform.position, 50f))
				{
					GameObject root = collider.transform.root.gameObject;
					foreach (partconditionscript condition in root.GetComponentsInChildren<partconditionscript>())
					{
						condition.state = 0;
						condition.Paint(_colours[Random.Range(0, _colours.Length)]);
						condition.Refresh(true);
					}
				}
				_timer = 0f;
			}
		}

		private void UpdateLights()
		{
			_colourIndex++;
			if (_colourIndex >= _colours.Length)
				_colourIndex = 0;
			int bottomIndex = (_colourIndex + 1) % _colours.Length;
			int lightsIndex = (_colourIndex + 2) % _colours.Length;

			// Set material emission colour.
			foreach (MeshRenderer renderer in gameObject.GetComponentsInChildren<MeshRenderer>())
			{
				string name = renderer.name.ToLower();
				switch (name)
				{
					case "cabin":
						renderer.material.color = _colours[_colourIndex];
						renderer.material.SetColor("_EmissionColor", _colours[_colourIndex]);
						break;
					case "lights":
						renderer.material.color = _colours[bottomIndex];
						renderer.material.SetColor("_EmissionColor", _colours[bottomIndex]);
						break;
				}
			}

			// Set light colour.
			Light light = gameObject.GetComponentInChildren<Light>();
			light.color = _colours[lightsIndex];
			float pulse = (Mathf.Sin(Time.time * 2f) + 1f) / 2f;
			light.range = Mathf.Lerp(100, 200, pulse);
		}
	}
}
