using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SS.Core
{
	public class TimeManager : MonoBehaviour
	{
		#region Static
		// Static Begin ===================

		public static bool isAnyFlag = true;
		
		// Singleton
		static TimeManager mInst = null;
		static TimeManager instance
		{
			get
			{
				if (mInst == null)
				{
					Spawn();
				}
				return mInst;
			}
		}
		
		static public bool debugPause = false;
		
		/// <summary>
		/// Real time since startup.
		/// </summary>
		static public float time
		{
			get
			{
				#if UNITY_EDITOR
				if (!Application.isPlaying) return Time.realtimeSinceStartup;
				if (debugPause) return 0;
				#endif
				return instance.mRealTime;
			}
		}
		
		/// <summary>
		/// Real delta time.
		/// </summary>
		static public float deltaTime
		{
			get
			{
				#if UNITY_EDITOR
				if (!Application.isPlaying) return 0f;
				if (debugPause) return 0;
				#endif
				return instance.mRealDelta;
			}
		}

		static public float scaledDeltaTime
		{
			get
			{
				#if UNITY_EDITOR
				if (!Application.isPlaying) return 0f;
				if (debugPause) return 0;
				#endif
				return Time.deltaTime;
			}
		}
		
		static public float adaptedDeltaTime
		{
			get
			{
				#if UNITY_EDITOR
				if (!Application.isPlaying) return 0f;
				if (debugPause) return 0;
				#endif
				return Mathf.Clamp (instance.mRealDelta, 1 / 60.0f, 1 / 3.0f);
			}
		}
		
		// Spawn singleton instance
		static void Spawn ()
		{
			if (mInst != null)
				return;
			GameObject go = new GameObject("_TimeManager");
			DontDestroyOnLoad(go);
			mInst = go.AddComponent<TimeManager>();
			mInst.mRealTime = Time.realtimeSinceStartup;
		}
		
		// Current timeScale
		static public float timeScale
		{
			get
			{
				return instance.currTimeScale;
			}
		}
		
		// Set timeScale with flag
		static public void SetTimeScale(string flag, float timeScale)
		{
			instance.SetTimeScaleInternal(flag,timeScale);
		}
		
		// Pause the world
		static public void Pause(string flag)
		{
			SetTimeScale(flag, 0);
		}
		
		// Unpause the world
		static public void Unpause(string flag)
		{
			SetTimeScale(flag, 1);
		}
		
		// Check if world is paused
		static public bool IsPaused()
		{
			return (instance.currTimeScale == 0);
		}
		
		// Check if world is paused by this flag
		static public bool IsPausedBy(string flag)
		{
			return instance.IsPausedByInternal(flag);
		}
		
		// Add pause flag info from component
		static void AddPauseInfo(string[] _pauseFlag)
		{
			if (_pauseFlag == null)
				return;
			
			if (instance.pauseInfo == null)
				instance.pauseInfo = new Hashtable();
			
			
			for (int i = 0; i < _pauseFlag.Length; i++)
			{
				if (!instance.pauseInfo.ContainsKey(_pauseFlag[i]))
				{
					instance.pauseInfo.Add (_pauseFlag[i], new PauseElement(1));
				}
			}
		}
		// Static End ===================
		#endregion Static
		
		// Define pause element
		public class PauseElement
		{
			public PauseElement(float _timeScale)
			{
				timeScale = _timeScale;
			}
			public float timeScale;
		}
		
		// Toggle of the debug information
		public bool			showDebug = false;
		
		// Pause flag to pause the time (timeScle = 0)
		[Auto]
		public string[]		pauseFlags;
		
		// Manage the current timeScale
		float				currTimeScale = 1;
		
		// Pause information
		Hashtable			pauseInfo = null;
		
		// curr time without time scale
		float				mRealTime = 0f;
		
		// curr deltaTime without time scale
		float				mRealDelta = 0f;
		
		// Update real time
		void Update ()
		{
			float rt = Time.realtimeSinceStartup;
			mRealDelta = rt - mRealTime;
			mRealTime = rt;
		}
		
		// set timeScale for flag
		void SetTimeScaleInternal(string flag, float timeScale)
		{
			// Set pause info
			if (pauseInfo != null)
			{
				bool isContainKey = pauseInfo.ContainsKey(flag);
				if (!isContainKey)
				{
					if (isAnyFlag)
					{
						AddPauseInfo(new string[] {flag});
					}
					else
					{
						return;
					}
				}

				PauseElement pe = pauseInfo[flag] as PauseElement;
				if (pe != null)
				{
					pe.timeScale = timeScale;
				}
				
				// Check flag
				float minTimeScale = 1;
				float maxTimeScale = 1;
				
				foreach (DictionaryEntry entry in pauseInfo)
				{
					PauseElement pEle = entry.Value as PauseElement;
					if (pEle != null)
					{
						minTimeScale = Mathf.Min (minTimeScale, pEle.timeScale);
						maxTimeScale = Mathf.Max (maxTimeScale, pEle.timeScale);
					}
				}
				
				// Set time scale
				if (maxTimeScale > 1)
					currTimeScale = maxTimeScale;
				else
					currTimeScale = Mathf.Max(0, minTimeScale);
				Time.timeScale = currTimeScale;
			}
		}
		
		
		bool IsPausedByInternal(string flag)
		{
			if (pauseInfo != null && pauseInfo.ContainsKey(flag))
			{
				PauseElement pe = pauseInfo[flag] as PauseElement;
				if (pe != null)
				{
					return (pe.timeScale == 0);
				}
			}
			return false;
		}
		
		
		void Awake()
		{
			World.RegisterOnGUI(DrawGUI, this, -1);

			if (mInst == null)
			{
				mInst = this;
				AddPauseInfo(pauseFlags);
			}
			else
			{
				// Duplicate singleton
				AddPauseInfo(pauseFlags);
				Destroy(this);
				return;
			}
			
			// Add default pauseflag for debug
			AddPauseInfo(new string[] {"TimeManager"});
		}

		void OnDestroy()
		{
			World.UnregisterOnGUI(DrawGUI, this, -1);
		}
		
		void DrawGUI()
		{
			if (!showDebug)
				return;
			
			GUILayout.Label(Time.timeScale.ToString());
			foreach (DictionaryEntry entry in pauseInfo)
			{
				PauseElement pEle = entry.Value as PauseElement;
				if (pEle != null)
				{
					GUILayout.BeginHorizontal();
					GUILayout.Label((string)entry.Key);
					GUILayout.Label(" = " + pEle.timeScale);
					GUILayout.EndHorizontal();
				}
			}
		}
		
		// Debug Pause
		[ContextMenu("DebugPause")]
		static void DebugPause()
		{
			TimeManager.Pause("TimeManager");
		}
		
		// Debug Unpause
		[ContextMenu("DebugUnpause")]
		static void DebugUnpause()
		{
			TimeManager.Unpause("TimeManager");
		}
	}

}