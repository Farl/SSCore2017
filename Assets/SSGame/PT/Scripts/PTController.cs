using UnityEngine;
using System.Collections;
using SS;
public class PTController : MonoBehaviour {

	public GameObject targetObj;
	public Vector3 targetVec;
	public bool bClimb;
	public bool bAction;

    public GameObject camTargetObj;
    public Vector3 camTargetVec;

    protected virtual void Awake()
    {

    }

    protected virtual void Start()
    {

    }

    protected virtual void OnDestroy()
    {

    }

    protected virtual void OnEnable()
    {
        EventManager.AddEventListener("Kill", OnKill, gameObject);
    }

    protected virtual void OnDisable()
    {
        EventManager.RemoveEventListener("Kill", OnKill, gameObject);
    }

    protected virtual void OnKill(EventMessage em, ref object paramRef)
    {
        Kill();
    }

    protected virtual void Kill()
    {

    }
}
