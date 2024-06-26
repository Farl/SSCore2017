﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SS.Core
{
	public class DialogSettings : DataAsset<DialogSettings>
	{
		#if UNITY_EDITOR
		[MenuItem("Tools/SS Core/Edit Dialog Settings")]
		static void Edit()
		{
			EditNow();
		}
		#endif

		[Auto]
		public List<Dialog> dialogDefine = new List<Dialog>();
	}
}