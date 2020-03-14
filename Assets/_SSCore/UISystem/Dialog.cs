using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SS
{
	[System.Serializable]
	public class Dialog
	{
		private int dialogID;
		private GameObject dialogObj = null;

		public DialogSystemBase dialogSys;

		public string dialogName;	// dialog type
		public string uiName;
		public bool isPause;

		public string textLabelEvent;
		public string textID;

		public string eventNext = "Dialog-Next";

		public string eventNameYes = "Dialog-Yes";
		public EventArray eventWhenYes;
		public string extraEventYes;

		public string eventNameNo = "Dialog-No";
		public EventArray eventWhenNo;
		public string extraEventNo;

		[HideInInspector]
		public string[] cmd;

		public EventArray eventOnLoad = new EventArray();
		
		public Dialog(int _dialogID, Dialog refDialog, params string[] cmdList)
		{
			dialogID = _dialogID;
			
			cmd = cmdList;

			if (refDialog != null)
			{
				uiName = refDialog.uiName;
				dialogName = refDialog.dialogName;
				textLabelEvent = refDialog.textLabelEvent;
				textID = refDialog.textID;
				eventNameYes = refDialog.eventNameYes;
				eventNameNo = refDialog.eventNameNo;
				eventWhenYes = refDialog.eventWhenYes;
				eventWhenNo = refDialog.eventWhenNo;
				eventNext = refDialog.eventNext;
				eventOnLoad = refDialog.eventOnLoad;
				isPause = refDialog.isPause;
				dialogSys = refDialog.dialogSys;
			}
		}

		public int GetID()
		{
			return dialogID;
		}
		
		protected virtual void DoCommand()
		{
			int cmdLength = cmd.Length;
			if (cmd != null && cmdLength > 0)
			{
				int cmdID = 1;
				switch(cmd[0])
				{
				case DialogSystem.CMD_BROADCAST_EVENT:
					// Parse each 2 string to eventID and paramString
					while (cmdID + 2 <= cmdLength)
					{
                            EventMessage em = new EventMessage(cmd[cmdID], GetDialogObj(), true, cmd[cmdID + 1]);
                            EventManager.Broadcast(em);
						cmdID += 2;
					}
					break;
				default:
					break;
				}
			}
		}
		
		public void Enter()
		{
			// Spawn or open UI
			bool isSpawned = false;
			dialogObj = UISystem.GetObject(uiName);

			// Check if exist (a valid page)
			if (dialogObj)
			{
				UIBase page = dialogObj.GetComponent<UIBase>();
				if (page && page.IsActive())
				{
					dialogObj = null;
				}
			}

			// Spawn one if there is no exist UI
			if (!dialogObj)
			{
				dialogObj = UISystem.Spawn(uiName);
				isSpawned = true;
			}

			// Setup page (QuickPanel)
			if (dialogObj)
			{
				dialogObj.SetActive(true);
				UIBase page = dialogObj.GetComponent<UIBase>();
				if (page)
				{
					page.destroyWhenClose = isSpawned;
					page.Open();
				}
			}
			else
			{
				// Empty UI
				// will destruct when update
			}

			SetText();
			
			Pause(true);

			DoCommand();

			eventOnLoad.Broadcast(dialogObj);
		}

		private void SetText()
		{
			if (!string.IsNullOrEmpty(textLabelEvent))
			{
                EventMessage em = new EventMessage(dialogObj, textLabelEvent, null, true, textID);
				EventManager.SendObjectEvent(em);
			}
		}

		public void SetNext(Dialog prevDialog)
		{
			dialogObj = prevDialog.GetDialogObj();

			// Leave
			prevDialog.Pause(false);

			// Enter
			Pause (true);

			SetText();
		}

		public void Leave()
		{
			if (GetDialogObj() != null)
			{
				DestroyDialogObj();
			}

			Pause(false);
		}

		public void DelDialog()
		{
			if (GetDialogObj() != null)
			{
				DestroyDialogObj();
			}
			Pause(false);
		}

		private void Pause(bool pause)
		{
			if (isPause)
			{
				if (pause)
				{
					TimeManager.Pause(string.Format("Dialog {0}", dialogID));
				}
				else
				{
					TimeManager.Unpause(string.Format("Dialog {0}", dialogID));
				}
			}
		}

		private void DestroyDialogObj()
		{
			if (dialogObj != null)
			{
				UIBase page = dialogObj.GetComponent<UIBase>();
				if (page)
				{
					page.Close();
				}
				else
				{
					GameObject.Destroy(dialogObj);
				}
			}
		}
		
		public void Update()
		{
			if (GetDialogObj() == null)
			{
				if (dialogSys != null)
				{
					dialogSys.RemoveDialog(dialogID);
				}
			}
		}

		public void Remove()
		{
			if (dialogSys != null)
			{
				dialogSys.RemoveDialog(dialogID);
			}
		}

		public GameObject GetDialogObj()
		{
			if (dialogObj == null)
			{
				return null;
			}
			else
			{
				UIBase page = dialogObj.GetComponent<UIBase>();
				if (page && !page.IsActive())
				{
					return null;
				}
			}
			return dialogObj;
		}
	}
}