using UnityEngine;

namespace MoreUFOs.Components
{
	internal class UFOType : MonoBehaviour
	{
		protected float Distance2D(Vector3 from, Vector3 to) => Mathf.Abs(from.x - to.x) + Mathf.Abs(from.z - to.z);

		protected float Distance3D(Vector3 from, Vector3 to) => Mathf.Abs(from.x - to.x) + Mathf.Abs(from.y - to.y) + Mathf.Abs(from.z - to.z);
		  
		protected virtual void OnCollisionEnter(Collision col)
		{
			if (col.collider.attachedRigidbody == null) return;
		}
	}
}
