using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using SS.Core;

public class PTPlayerController : PTController {
	public Camera mainCamera;
	public LayerMask groundLayer;
	public GameObject followDummy;

    protected override void Kill()
    {
        base.Kill();
        LevelSystem.ReloadLevel();
    }

    void SetFollowPosition(Vector3 pos)
    {
        if (followDummy == null)
        {
            followDummy = new GameObject("Follow-Dummy");
        }
        if (followDummy != null)
        {
            followDummy.transform.position = pos;
        }
    }

    // Update is called once per frame
    void Update () {
		targetVec = Vector3.zero;
		targetObj = null;

        // Camera
        float camHorizontal = ControllerSystem.GetAxis(ControllerSystem.Axis.Stick_R_X);
        float camVertical = ControllerSystem.GetAxis(ControllerSystem.Axis.Stick_R_Y);
        camTargetVec = new Vector3(camHorizontal, camVertical, 0);

        float horizontal = ControllerSystem.GetAxis(ControllerSystem.Axis.Stick_L_X);
        float vertical = ControllerSystem.GetAxis(ControllerSystem.Axis.Stick_L_Y);

        if (mainCamera) {
			if (Input.GetMouseButton (0)) {
				Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
				RaycastHit hitInfo;
				if (Physics.Raycast (ray, out hitInfo, 1000, groundLayer.value)) {
                    SetFollowPosition(hitInfo.point);
                }
			}
            else
            {
                if (Mathf.Abs(horizontal) > 0.1f || Mathf.Abs(vertical) > 0.1f)
                {
                    Vector3 pos = transform.position;
                    Vector3 faceDir = (horizontal * mainCamera.transform.right + vertical * mainCamera.transform.forward).normalized;
                    faceDir.y = 0;
                    SetFollowPosition(pos + faceDir.normalized);
                }
            }
		}

		if (followDummy) {
			if ((followDummy.transform.position - transform.position).magnitude < 0.02f) {
				Destroy (followDummy);
			} else {
				targetVec = followDummy.transform.position - transform.position;
				targetObj = followDummy;
			}
		}
	}
}
