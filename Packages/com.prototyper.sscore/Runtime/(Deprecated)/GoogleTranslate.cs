using UnityEngine;
using System.Collections;
using System.IO;

public class GoogleTranslate : MonoBehaviour
{
	void Awake()
	{
		SSGUI.Register(DrawGUI);
	}
	
	void OnDestroy()
	{
		SSGUI.Unregister(DrawGUI);
	}
	
	void DrawGUI()
	{
		if (GUILayout.Button("TestTTS"))
		{
			StartCoroutine(TestTTS());
		}
	}
	
	IEnumerator TestTTS()
	{
		string url = "http://translate.google.com/translate_tts?" + string.Format("sl={0}&tl={1}&q=\"{2}\"", "en", "en",  WWW.EscapeURL("Hero"));
		Debug.Log(url);
		WWW www = new WWW(url);
		yield return www;
		if (www.error == null)
		{
			GetComponent<AudioSource>().clip = www.GetAudioClip(false, false, AudioType.MPEG);
			GetComponent<AudioSource>().Play();
		}
		else
		{
			Debug.Log(www.error);
		}
	}
}
