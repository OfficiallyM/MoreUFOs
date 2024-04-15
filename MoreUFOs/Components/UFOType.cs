using System;
using UnityEngine;

namespace MoreUFOs.Components
{
	internal class UFOType : MonoBehaviour
	{
		protected int index = -1;

		protected virtual void Start()
		{
			index = Array.FindIndex(MoreUFOs.UFOTypes, t => t == this.GetType());
		}

		protected float Distance2D(Vector3 from, Vector3 to) => Mathf.Abs(from.x - to.x) + Mathf.Abs(from.z - to.z);

		protected float Distance3D(Vector3 from, Vector3 to) => Mathf.Abs(from.x - to.x) + Mathf.Abs(from.y - to.y) + Mathf.Abs(from.z - to.z);
		  
		protected virtual void OnCollisionEnter(Collision col)
		{
			if (col.collider.attachedRigidbody == null) return;
		}
	}
}
