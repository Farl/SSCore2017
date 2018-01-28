using UnityEngine;
using System.Collections;

public class AnimationTools : MonoBehaviour {
	float totalTime = 0;

	// Use this for initialization
	void Start () {
		foreach (AnimationState state in GetComponent<Animation>())
		{
			state.enabled = false;
		}
	}
	
	// Update is called once per frame
	void LateUpdate () {
		totalTime += Time.deltaTime;
		foreach (AnimationState state in GetComponent<Animation>())
		{
			state.enabled = state.weight > 0;
			if (state.enabled)
			{
				state.time += Time.deltaTime * state.speed;
			}
		}
		GetComponent<Animation>().Sample();
		foreach (AnimationState state in GetComponent<Animation>())
		{
			state.enabled = false;
		}
	}

	// On GUI
	void OnGUI()
	{
		GUILayout.Label(totalTime.ToString());
		GUILayout.Label(Time.realtimeSinceStartup.ToString());
		Time.timeScale = GUILayout.HorizontalSlider(Time.timeScale, 0.0f, 1.0f, GUILayout.Width(100));
		if (GetComponent<Animation>() != null)
		{
			foreach (AnimationState state in GetComponent<Animation>())
			{
				GUILayoutOption layout = GUILayout.Width(100);
				GUILayout.BeginHorizontal(layout);
				state.weight = GUILayout.HorizontalSlider(state.weight, 0.0f, 1.0f, layout);
				GUILayout.Toggle(state.enabled, state.name, layout);
				state.normalizedTime = GUILayout.HorizontalSlider(state.normalizedTime, 0.0f, 1.0f, layout);
				state.time = float.Parse(GUILayout.TextField(state.time.ToString(), layout));
				GUILayout.Label(state.speed + "/" + state.normalizedSpeed, layout);
				state.speed = GUILayout.HorizontalSlider(state.speed, 0.0f, 1.0f, layout);
				GUILayout.EndHorizontal();
			}
			GetComponent<Animation>().Sample();
		}
	}
}
