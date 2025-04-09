using UnityEngine;
using Logger = MoreUFOs.Modules.Logger;

namespace MoreUFOs.Components
{
	[DisallowMultipleComponent]
	internal class Sandstorms : UFOType
	{
		private fedoscript _fedo;
		private float _timePassed = 0;
		private temporaryTurnOffGeneration _temp;

		public void Start()
		{
			Logger.Log($"Sandstorms has spawned", Logger.LogLevel.Debug);
			_fedo = gameObject.GetComponent<fedoscript>();
			Color color = new Color(73 / 255f, 73 / 255f, 73 / 255f);

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

			_temp = mainscript.M.menu.GetComponentInChildren<temporaryTurnOffGeneration>();
		}

		public void Update()
		{
			if (_temp == null) return;    

            if (!enabled || _fedo == null)
				return;
			_fedo.roadHeightOffset = 100f;
			float num = Distance2D(transform.position, mainscript.M.player.transform.position);
			if ((double)num >= 600.0)
				return;
			_fedo.roadHeightOffset = 20f;
			_fedo.highspeed = Mathf.Lerp(100f, 400f, Mathf.Clamp01(num / 400f));

			_timePassed += Time.deltaTime;
			if (_timePassed > 7f)
			{
				_temp.sandstormspawn.SpawnAt(_fedo.transform.position);
				_timePassed = 0;
			}
		}
	}
}
