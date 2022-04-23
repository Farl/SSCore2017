using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SS
{

	public class LoadingUI : UIBaseSingleton<LoadingUI>
	{

        public static bool IsFinished()
        {
            return InitManager.IsEmpty();
        }
	}

}