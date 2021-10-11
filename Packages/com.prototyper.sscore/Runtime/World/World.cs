using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using System;

namespace SS
{

	static public class World
	{
		static SortedVoidDelegate onGUISet = new SortedVoidDelegate();
		static SortedVoidDelegate updateSet = new SortedVoidDelegate();
		static SortedVoidDelegate lateUpdateSet = new SortedVoidDelegate();

		// Instantiate and Destroy Management and Object pool and Object Management (Find)

		// Update Management (control the order of non monobehavior script update) (Update/LateUpdate/FixedUpdate)
		#region Update
		public static void RegisterUpdate(Action callback, object key, int order = -1)
		{
			updateSet.Register(callback, key, order);
		}
		public static void UnregisterUpdate(Action callback, object key, int order = -1)
		{
			updateSet.Unregister(callback, key, order);
		}
		static void InvokeUpdate()
		{
			updateSet.Invoke();
		}
		#endregion Update

		#region LateUpdate
		public static void RegisterLateUpdate(Action callback, object key, int order = -1)
		{
			lateUpdateSet.Register(callback, key, order);
		}
		public static void UnregisterLateUpdate(Action callback, object key, int order = -1)
		{
			lateUpdateSet.Unregister(callback, key, order);
		}
		static void InvokeLateUpdate()
		{
			lateUpdateSet.Invoke();
		}
		#endregion LateUpdate

		// OnGUI collection
		#region OnGUI
		public static void RegisterOnGUI(Action callback, object key, int order = -1)
		{
			onGUISet.Register(callback, key, order);
		}
		public static void UnregisterOnGUI(Action callback, object key, int order = -1)
		{
			onGUISet.Unregister(callback, key, order);
		}
		static void InvokeOnGUI()
		{
			onGUISet.Invoke();
		}
		#endregion OnGUI
		
		
		// Initialization Management (control the order of script start)
		public static void Start()
		{
			// TODO: test initialization
		}

		public static void Update()
		{
			InvokeUpdate();
		}
		
		public static void LateUpdate()
		{
			InvokeLateUpdate();
		}
		
		public static void OnGUI()
		{
			InvokeOnGUI();
		}

		// World initalization (create World instance)
		public static void Init()
		{
            new GameObject("_World", typeof(WorldInstance));

        }
	}
}
