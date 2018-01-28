using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SS
{
	public class UICamera : MonoBehaviour
	{
		public static ArrayList	uiCameras = new ArrayList();
		public static UICamera	currUICam;

		Camera			currCam;
		GameObject		currObj;
		GameObject		pressObj;
		StringFlag		locker = new StringFlag();

		#region Locker
		public void Lock(string flag)
		{
			locker.AddFlag(flag);
			enabled = false;
		}

		public void Unlock(string flag)
		{
			locker.RemoveFlag(flag);
			if (locker.IsEmpty())
				enabled = true;
		}
		#endregion

		void Awake()
		{
			currCam = GetComponent<Camera>();
			currUICam = this;

			uiCameras.Add (this);
		}

		void Destroy()
		{
			uiCameras.Remove (this);
		}

		// Use this for initialization
		void Start ()
		{
		}
		
		// Update is called once per frame
		void Update ()
		{
			if (currCam == null)
			{
				currCam = GetComponent<Camera>();
			}
			if (currCam == null)
				return;
			
			object _ret = null;
			currObj = null;
			
			Vector2 screenPos = Input.mousePosition;
			
			// Construct a ray from the current mouse coordinates
			Ray ray = currCam.ScreenPointToRay (screenPos);
			RaycastHit raycastInfo;

			
			//Debug.DrawRay(ray.origin, ray.direction * 1000);
			if (Physics.Raycast (ray, out raycastInfo, 1000, ~gameObject.layer))
			{
				currObj = raycastInfo.collider.gameObject;
			}
			
			// PRESS true
			if (Input.GetMouseButtonDown(0))
			{
                if (currObj != null)
                {
                    pressObj = currObj;
                    EventMessage em = new EventMessage(currObj, "UICamera-Press", this, true, null, Vector2.zero, true, new object[] { screenPos });
                    EventManager.SendObjectEvent(em, ref _ret);
				}
			}
			// PRESS false (CLICK)
			else if (Input.GetMouseButtonUp(0))
			{
				if (currObj != null)
				{
					pressObj = null;
                    EventMessage em = new EventMessage(currObj, "UICamera-Press", this, false, null, Vector2.zero, true, new object[] { screenPos });
                    EventManager.SendObjectEvent(em, ref _ret);
                }
			}
			// PRESS hold
			else if (Input.GetMouseButton(0))
			{
				if (currObj != pressObj)
				{
					if (pressObj != null)
                    {
                        EventMessage em = new EventMessage(currObj, "UICamera-Leave", this, true, null, Vector2.zero, true, new object[] { screenPos });
                        EventManager.SendObjectEvent(em, ref _ret);
                    }
					pressObj = currObj;
					if (currObj != null)
                    {
                        EventMessage em = new EventMessage(currObj, "UICamera-Leave", this, false, null, Vector2.zero, true, new object[] { screenPos });
                        EventManager.SendObjectEvent(em, ref _ret);
                    }
				}
				else
				{
					if (currObj != null)
                    {
                        EventMessage em = new EventMessage(currObj, "UICamera-Down", this, true, null, Vector2.zero, true, new object[] { screenPos });
                        EventManager.SendObjectEvent(em, ref _ret);
                    }
				}
			}
			// Hover
			else
			{
				if (currObj != null)
				{
				}
				else
				{
					// Do not cast any object
					if (pressObj != null)
                    {
                        EventMessage em = new EventMessage(pressObj, "UICamera-Leave", this, true, null, Vector2.zero, true, new object[] { screenPos });
                        EventManager.SendObjectEvent(em, ref _ret);
						pressObj = currObj;
					}
				}
			}
		}
		
		#if UNITY_EDITOR
		/*
		void OnGUI()
		{
			GUILayout.Label("Current " + (currObj == null? "null":currObj.name));
			GUILayout.Label("Press " + (pressObj == null? "null":pressObj.name));
		}
		*/
		#endif
	}
	
}