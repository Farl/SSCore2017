using UnityEngine;
using System.Collections;

namespace SS
{	
	public class UIBlocker : MonoBehaviour
	{
		public string UIID = "";
		public string[] blockUIs = null;
		
		public void Setup(string _UIID, string[] _blockUIs)
		{
			if (_blockUIs != null)
			{
				blockUIs = _blockUIs;
			}
			UIID = _UIID;
		}

        public void ForceStart()
        {
            Start();
        }
	
		// Use this for initialization
		void Start()
		{
			// then, block otherwise
			UISystem.Block(this);
		}
		
		void OnDestroy()
		{
			// unblock other UI block by me
			UISystem.Unblock(this);
		}

        public void Lock()
        {

        }
        public void Unlock()
        {

        }
	}
}

