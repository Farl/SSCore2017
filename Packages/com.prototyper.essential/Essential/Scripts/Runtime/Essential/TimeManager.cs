using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if ENABLE_INPUT_SYSTEM && USE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace SS
{
	/// <summary>
	/// Time Manager
	/// </summary>
	public static class TimeManager
	{
#region Public
		// Define pause element
		public class PauseElement
		{
			public float timeScale;
		}

		// Toggle of the debug information
		public static bool showDebug = false;

		// Pause flag to pause the time (timeScle = 0)
		public static string[] pauseFlags;
		public static bool isAnyFlag = true;

		public static bool debugPause = false;

		/// <summary>
		/// Real time since startup.
		/// </summary>
		public static float time
		{
			get
			{
#if UNITY_EDITOR
				if (!Application.isPlaying) return Time.realtimeSinceStartup;
				if (debugPause) return 0;
#endif
				return mRealTime;
			}
		}

		/// <summary>
		/// Real delta time.
		/// </summary>
		static public float unscaledDeltaTime
		{
			get
			{
#if UNITY_EDITOR
				if (!Application.isPlaying) return 0f;
				if (debugPause) return 0;
#endif
				return mRealDelta;
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
				return Mathf.Clamp(mRealDelta, 1 / 60.0f, 1 / 3.0f);
			}
		}

		static public float debugTimeScaleMultiplier
        {
            get
            {
				return _debugTimeScaleMultiplier;
            }
			set
			{
				if (value < 0)
					value = 1;
				if (_debugTimeScaleMultiplier != value)
				{
					_debugTimeScaleMultiplier = value;
					SetCurrentTimeScale(currTimeScale);
				}
			}
        }

		// Current timeScale
		static public float timeScale
		{
			get
			{
				return currTimeScale;
			}
		}

		// Set timeScale with flag
		static public void SetTimeScale(string flag, float timeScale)
		{
			SetTimeScaleInternal(flag, timeScale);
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

		static public void UnpauseAll()
        {
			foreach (var entry in pauseInfo)
			{
				Unpause(entry.Key);
			}
        }

		// Check if world is paused
		static public bool IsPaused()
		{
			return (currTimeScale == 0);
		}

		// Check if world is paused by this flag
		static public bool IsPausedBy(string flag)
		{
			return IsPausedByInternal(flag);
		}
#endregion

#region Private

		private static float _debugTimeScaleMultiplier = 1;

		// Manage the current timeScale
		private static float currTimeScale = 1;

		// curr time without time scale
		static float mRealTime
		{
			get
			{
				return Time.realtimeSinceStartup;
			}
		}

		// curr deltaTime without time scale
		static float mRealDelta
		{
			get
			{
				return Time.unscaledDeltaTime;
			}
		}

		// Pause information
		private static Dictionary<string, PauseElement> pauseInfo = new Dictionary<string, PauseElement>();

		// Add pause flag info from component
		static void AddPauseInfo(string[] _pauseFlag)
		{
			if (_pauseFlag == null)
				return;

			for (int i = 0; i < _pauseFlag.Length; i++)
			{
				if (!pauseInfo.ContainsKey(_pauseFlag[i]))
				{
					pauseInfo.Add(_pauseFlag[i], new PauseElement() { timeScale = 1 });
				}
			}
		}

		// set timeScale for flag
		static void SetTimeScaleInternal(string flag, float timeScale)
		{
			// Set pause info
			if (pauseInfo != null)
			{
				bool isContainKey = pauseInfo.ContainsKey(flag);
				if (!isContainKey)
				{
					if (isAnyFlag)
					{
						AddPauseInfo(new string[] { flag });
					}
					else
					{
						return;
					}
				}

				var pe = pauseInfo[flag];
				if (pe != null)
				{
					pe.timeScale = timeScale;
				}

				// Check flag
				float minTimeScale = 1;
				float maxTimeScale = 1;

				foreach (var entry in pauseInfo)
				{
					var pEle = entry.Value;
					if (pEle != null)
					{
						minTimeScale = Mathf.Min(minTimeScale, pEle.timeScale);
						maxTimeScale = Mathf.Max(maxTimeScale, pEle.timeScale);
					}
				}

				// Set time scale
				if (maxTimeScale > 1)
					SetCurrentTimeScale(maxTimeScale);
				else
					SetCurrentTimeScale(Mathf.Max(0, minTimeScale));
			}
		}

		private static void SetCurrentTimeScale(float value)
		{
			currTimeScale = value;
			Time.timeScale = currTimeScale * debugTimeScaleMultiplier;
		}

		static bool IsPausedByInternal(string flag)
		{
			if (pauseInfo != null && pauseInfo.ContainsKey(flag))
			{
				var pe = pauseInfo[flag];
				if (pe != null)
				{
					return (pe.timeScale == 0);
				}
			}
			return false;
		}

		[RuntimeInitializeOnLoadMethod]
		static void Initialized()
		{
			DebugMenu.onMenuToggle += (b) =>
			{
				SetTimeScale(nameof(TimeManager), b? DebugMenu.TimeScale: 1);
			};

			string kSpeedUp = "Speed Up";
			string kSpeedDown = "Speed Down";
			string kSpeedReset = "Speed Reset";
			string kSpeedPause = "Speed Pause";

			System.Action speedUp = () => { debugTimeScaleMultiplier += 1; };
			System.Action speedDown = () => { debugTimeScaleMultiplier /= 2; };
			System.Action speedReset = () => { debugTimeScaleMultiplier = 1; };
			System.Action speedPause = () => { debugTimeScaleMultiplier = (debugTimeScaleMultiplier == 0) ? 1 : 0; };

			DebugMenu.AddButton(page: "Advance", label: "Dump TimeManager", onClick: (obj) => { Dump(); });
			DebugMenu.AddButton(page: "Advance", label: "TimeManager Unpause", onClick: (obj) => { Dump(); });
			DebugMenu.AddButton(page: "Advance", label: kSpeedUp, onClick: (obj) => { speedUp(); });
			DebugMenu.AddButton(page: "Advance", label: kSpeedDown, onClick: (obj) => { speedDown(); });
			DebugMenu.AddButton(page: "Advance", label: kSpeedReset, onClick: (obj) => { speedReset(); });
			DebugMenu.AddButton(page: "Advance", label: kSpeedPause, onClick: (obj) => { speedPause(); });


#if ENABLE_INPUT_SYSTEM && USE_INPUT_SYSTEM
			InputActionMap debugActionMap = new InputActionMap("TimeManager");

			var actionList = new List<InputAction>();

			var action = debugActionMap.AddAction(kSpeedUp, type: InputActionType.Button);
			action.AddBinding("<Keyboard>/equals");
			action.AddBinding("<Keyboard>/numpadPlus");
			action.started += (cc) => { speedUp(); };
			actionList.Add(action);

			action = debugActionMap.AddAction(kSpeedDown, type: InputActionType.Button);
			action.AddBinding("<Keyboard>/minus");
			action.AddBinding("<Keyboard>/numpadMinus");
			action.started += (cc) => { speedDown(); };
			actionList.Add(action);

			action = debugActionMap.AddAction(kSpeedReset, type: InputActionType.Button);
			action.AddBinding("<Keyboard>/slash");
			action.AddBinding("<Keyboard>/numpadDivide");
			action.started += (cc) => { speedReset(); };
			actionList.Add(action);

			action = debugActionMap.AddAction(kSpeedPause, type: InputActionType.Button);
			action.AddBinding("<Keyboard>/0");
			action.AddBinding("<Keyboard>/pause");
			action.started += (cc) => { speedPause(); };
			actionList.Add(action);

			foreach (var a in actionList)
            {
				a.Enable();
            }
#endif
		}

		private static void Dump()
        {
			var str = string.Empty;
			foreach (var entry in pauseInfo)
            {
				str = str + $"{entry.Key} = {entry.Value.timeScale}\n";
            }
			Debug.Log(str);
        }

		private static void DrawGUI()
		{
			if (!showDebug)
				return;

			GUILayout.Label(Time.timeScale.ToString());
			foreach (var entry in pauseInfo)
			{
				var pEle = entry.Value;
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
		static void DebugPause()
		{
			TimeManager.Pause("TimeManager");
		}

		// Debug Unpause
		static void DebugUnpause()
		{
			TimeManager.Unpause("TimeManager");
		}
#endregion

	}

}