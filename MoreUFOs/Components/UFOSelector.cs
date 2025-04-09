using System;
using UnityEngine;

namespace MoreUFOs.Components
{
	internal class UFOSelector : MonoBehaviour
	{
		public void Start()
		{
			if (MoreUFOs.ForceSpawn != null)
			{
				gameObject.AddComponent(MoreUFOs.ForceSpawn);
				return;
			}

			int index = UnityEngine.Random.Range(0, MoreUFOs.Mod.MaxIndex);
			if (index < MoreUFOs.UFOTypes.Length)
			{
				Type ufoType = MoreUFOs.UFOTypes[index];
				gameObject.AddComponent(ufoType);
			}
		}
	}
}
