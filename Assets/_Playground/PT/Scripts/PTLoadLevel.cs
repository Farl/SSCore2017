using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class PTLoadLevel : MonoBehaviour {
	public string sceneName;
	// Use this for initialization
	void Start () {
		SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
