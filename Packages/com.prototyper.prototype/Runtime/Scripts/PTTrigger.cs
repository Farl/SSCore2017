using UnityEngine;
using System.Collections;

public class PTTrigger : MonoBehaviour {

    protected virtual Color GetGizmosMeshColor(bool focused)
    {
        if (focused)
            return new Color(0f, 1f, 0f, 0.25f);
        else
            return new Color(0f, 1f, 0f, 0.5f);
    }

    protected virtual Color GetGizmosWireColor(bool focused)
    {
        if (focused)
            return new Color(0f, 1f, 0f, 0.5f);
        else
            return new Color(0f, 1f, 0f, 0.5f);
    }

	protected virtual void OnDrawGizmos()
	{
		Collider[] colliders;
		colliders = GetComponents<Collider> ();

        Gizmos.matrix = transform.localToWorldMatrix;
        foreach (Collider c in colliders) {
			if (c.GetType() == typeof(BoxCollider)) {
				BoxCollider bc = (BoxCollider)c;

                //Gizmos.color = GetGizmosWireColor(false);
                //Gizmos.DrawWireCube (transform.position + bc.center, bc.size);
                Gizmos.color = GetGizmosMeshColor(false);
				Gizmos.DrawCube (Vector3.zero + bc.center, bc.size);
			}
			else if (c.GetType() == typeof(SphereCollider)) {
				SphereCollider sc = (SphereCollider)c;
				//Gizmos.color = GetGizmosWireColor(false);
                //Gizmos.DrawWireSphere (transform.position + sc.center, sc.radius);
				Gizmos.color = GetGizmosMeshColor(false);
                Gizmos.DrawSphere (Vector3.zero + sc.center, sc.radius);
			}
		}
	}

    protected virtual void OnDrawGizmosSelected()
	{
		Collider[] colliders;
		colliders = GetComponents<Collider> ();

        Gizmos.matrix = transform.localToWorldMatrix;
        foreach (Collider c in colliders) {
			if (c.GetType() == typeof(BoxCollider)) {
				BoxCollider bc = (BoxCollider)c;
				Gizmos.color = GetGizmosWireColor(true);
				Gizmos.DrawWireCube (Vector3.zero + bc.center, bc.size);
				Gizmos.color = GetGizmosMeshColor(true);
				Gizmos.DrawCube (Vector3.zero + bc.center, bc.size);
			}
			else if (c.GetType() == typeof(SphereCollider)) {
				SphereCollider sc = (SphereCollider)c;
				Gizmos.color = GetGizmosWireColor(true);
                Gizmos.DrawWireSphere (Vector3.zero + sc.center, sc.radius);
				Gizmos.color = GetGizmosMeshColor(true);
                Gizmos.DrawSphere (Vector3.zero + sc.center, sc.radius);
			}
		}
    }

    protected virtual void OnTriggerEnter(Collider coll)
    {

    }

    protected virtual void OnTriggerExit(Collider coll)
    {

    }

    protected virtual void OnTriggerStay(Collider coll)
    {

    }
}
