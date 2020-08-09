using UnityEngine;
using UnityEditor;
using System.Collections;

public class KeystorePass : MonoBehaviour
{
	[MenuItem("SS/Keystore/Set Prototyper")]
	public static void KeystorePrototyper()
	{
		SetKeystore("lifestyle");
	}
	
	[MenuItem("SS/Keystore/Set SS")]
	public static void KeystoreSS()
	{
		SetKeystore("ss");
	}
	
	public static void SetKeystore(string pwd)
	{
		PlayerSettings.Android.keyaliasPass = pwd;
		PlayerSettings.Android.keystorePass = pwd;
		PlayerSettings.keyaliasPass = pwd;
		PlayerSettings.keystorePass = pwd;
	}
}
