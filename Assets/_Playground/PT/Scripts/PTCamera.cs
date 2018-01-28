using UnityEngine;
using System.Collections;

public class PTCamera : MonoBehaviour {
	public static PTCamera main;
	public new Camera camera;

	public Vector3 followAngle;
	public float followDist;
	public Transform targetTrans;

	enum Type
	{
		FOLLOW,
		SECURITY,
	}
	private Type type = Type.FOLLOW;

	// Use this for initialization
	void Start () {
		main = this;
	}
	
	// Update is called once per frame
	void Update () {
		UpdateCamera ();
    }

    void UpdateCamera()
    {
        switch (type)
        {
            case Type.FOLLOW:
                {
                    if (targetTrans)
                    {
                        PTController controller = targetTrans.GetComponent<PTController>();
                        if (controller)
                        {
                            if (controller.camTargetVec.magnitude > 0.1f)
                            {
                                followAngle.y += controller.camTargetVec.x;
                                followAngle.x += controller.camTargetVec.y;
                            }
                        }
                        Vector3 destPos = targetTrans.position;
                        Vector3 offsetVec = Quaternion.Euler(followAngle) * new Vector3(0, 0, followDist);
                        destPos -= offsetVec;

                        transform.position = destPos;
                        transform.LookAt(targetTrans);
                    }
                }
                break;
            case Type.SECURITY:
                break;
        }
    }
}
