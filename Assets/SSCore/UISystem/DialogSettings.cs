using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace SS
{
	public class DialogSettings : DataAsset<DialogSettings>
	{
		#if UNITY_EDITOR
		[MenuItem("SS/Edit Dialog Settings")]
		static void Edit()
		{
			EditNow();
		}
		#endif

		[Auto]
		public List<Dialog> dialogDefine = new List<Dialog>();
	}
}