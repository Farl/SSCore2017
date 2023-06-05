using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SS.Core
{
	public class DialogSystem : MonoBehaviour
	{
		
		public const string CMD_BROADCAST_EVENT = "CMD_BROADCAST_EVENT";
		
		[System.Serializable]
		public class Dialog
		{
			int dialogID;
			public string dialogName;
			public GameObject dialogPanel;
			public string[] uiDefaultLockers;
			public string textLabelEvent;
			public string textID;
			GameObject dialogObj = null;
			public string eventNameYes = "DialogEventYes";
			public EventArray eventWhenYes;
			public string extraEventYes;
			public string eventNameNo = "DialogEventNo";
			public EventArray eventWhenNo;
			public string extraEventNo;
			public bool bPause;
			public string[] blockUI;
			public string[] cmd;
			
			public Dialog(int _dialogID, Dialog refDialog, params string[] cmdList)
			{
				dialogID = _dialogID;
				if (refDialog != null)
				{
					dialogName = refDialog.dialogName;
					dialogPanel = refDialog.dialogPanel;
					uiDefaultLockers = refDialog.uiDefaultLockers;
					textLabelEvent = refDialog.textLabelEvent;
					textID = refDialog.textID;
					eventNameYes = refDialog.eventNameYes;
					eventNameNo = refDialog.eventNameNo;
					eventWhenYes = refDialog.eventWhenYes;
					eventWhenNo = refDialog.eventWhenNo;
					bPause = refDialog.bPause;
					blockUI = refDialog.blockUI;
					
					cmd = cmdList;
				}
			}
			public int GetID()
			{
				return dialogID;
			}
			
			void DoCommand()
			{
				int cmdLength = cmd.Length;
				if (cmd != null && cmdLength > 0)
				{
					int cmdID = 1;
					switch(cmd[0])
					{
					case CMD_BROADCAST_EVENT:
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
				// Spawn UI
				if (dialogPanel != null)
				{
					dialogObj = EventToSpawner.Spawn(dialogPanel, null, null);
				}
				// Change Text
				if (textLabelEvent != null && textLabelEvent != "")
				{
                    EventMessage em = new EventMessage(textLabelEvent, DialogSystem.dialogSys.gameObject, true, textID);
					EventManager.Broadcast(em);
				}
				// Do command
				DoCommand();
			}
			public void SetNext(GameObject obj)
			{
				dialogObj = obj;
				if (textLabelEvent != null && textLabelEvent != "")
                {
                    EventMessage em = new EventMessage(textLabelEvent, DialogSystem.dialogSys.gameObject, true, textID);
                    EventManager.Broadcast(em);
				}

                {
                    EventMessage em = new EventMessage("OpenDialog", DialogSystem.dialogSys.gameObject, true, string.Empty);
                    EventManager.Broadcast(em);
                }
				
			}
			public void Leave()
			{
				if (GetDialogObj() != null)
				{
					DestroyDialogObj();
                }
                EventMessage em = new EventMessage("FinishDialog", DialogSystem.dialogSys.gameObject, true, string.Empty);
                EventManager.Broadcast(em);
			}
			public void DelDialog()
			{
				if (GetDialogObj() != null)
				{
					DestroyDialogObj();
				}
				
			}
			
			// Farl: destroy object function (for recyclePool)
			void DestroyDialogObj()
			{
				GameObject.Destroy(dialogObj);
			}
			
			public void Update()
			{
				if (GetDialogObj() == null)
				{
					RemoveDialog(dialogID);
				}
			}
			public void Remove()
			{
				RemoveDialog(dialogID);
			}
			
			// Farl: GetDialogObj for RecyclePool
			public GameObject GetDialogObj()
			{
				if (dialogObj == null)
				{
					return null;
				}
				return dialogObj;
			}
		}
		static DialogSystem dialogSys = null;
		
		public string uiLockEvent = "UIGameLocker";
		public string pauseEvent = "GamePause";
		private const string eventCoversationNextID = "ConversationNext";
		private const string strDialogYes = "DialogYes";
		private const string strDialogNo = "DialogNo";
		
		[Auto]
		public Dialog[] dialogArray;
		
		List<Dialog> dialogList = new List<Dialog>();
		int nextDialogID = 0;
		static Dialog prevDialog = null;
		static bool bRemoveAllDialog = false;
		
		public static int RequestDialog(string dialogName, string dialogTextID, string extraEventYes, string extraEventNo, params string[] cmdArray)
		{
			if (dialogSys == null || dialogSys.dialogArray == null)
				return -1;
			
			foreach (Dialog dialog in dialogSys.dialogArray)
			{
				if (dialogName == dialog.dialogName)
				{
					int currDialogID = dialogSys.nextDialogID;
					Dialog newDialog = new Dialog(currDialogID, dialog, cmdArray);
					
					// Override
					if (dialogTextID != null)
						newDialog.textID = dialogTextID;
					if (extraEventYes != null)
						newDialog.extraEventYes = extraEventYes;
					if (extraEventNo != null)
						newDialog.extraEventNo = extraEventNo;
					
					dialogSys.nextDialogID++;
					dialogSys.dialogList.Add(newDialog);
					return currDialogID;
				}
			}
			return -1;
		}
		
		public static void RemoveAllDialog()
		{
			bRemoveAllDialog = true;
			
			//PauseAndLock(false,false);
		}
		public static bool RemoveDialog(int dialogID)
		{
			foreach (Dialog dialog in dialogSys.dialogList)
			{
				if (dialog.GetID() == dialogID)
				{
					dialogSys.dialogList.Remove(dialog);
					return true;
				}
			}
			return false;
		}
		public static bool AnyDialogExist()
		{
			return (dialogSys.dialogList.Count > 0);
		}
		
		void Awake()
		{
			if (dialogSys != null)
				Destroy(dialogSys);
			else
				dialogSys = this;
		}
		
		void OnDestroy()
		{
			if (dialogSys == this)
				dialogSys = null;
		}
		
		Dialog GetCurrDialog()
		{
			if (dialogList.Count <= 0)
				return null;
			return dialogList[0];
		}
		
		void OnEventYes(EventMessage em, ref object paramRef)
		{
			Dialog currDialog = GetCurrDialog();
			if (currDialog != null)
			{
				currDialog.eventWhenYes.Broadcast(em.origSrc);
				if (currDialog.extraEventYes != null)
                {
                    EventMessage emExta = new EventMessage(currDialog.extraEventYes, em.origSrc, em.paramBool, em.paramString, _paramExtra: em.paramExtra);
                    EventManager.Broadcast(emExta);
                }
			}
		}
		
		void OnEventNo(EventMessage em, ref object paramRef)
        {
			Dialog currDialog = GetCurrDialog();
			if (currDialog != null)
			{
				currDialog.eventWhenNo.Broadcast(dialogSys);
				if (currDialog.extraEventNo != null)
                {
                    EventMessage emExta = new EventMessage(currDialog.extraEventNo, em.origSrc, em.paramBool, em.paramString, _paramExtra: em.paramExtra);
                    EventManager.Broadcast(emExta);
                }
			}
		}
		void OnEventNext(EventMessage em, ref object paramRef)
        {
			prevDialog.Remove();
		}
		void PauseAndLock(bool setValue,bool bPause)
		{
			if (pauseEvent != null && bPause)
            {
                EventMessage em = new EventMessage(pauseEvent, this, setValue);
                EventManager.Broadcast(em);
            }
			if (uiLockEvent != null)
            {
                EventMessage em = new EventMessage(uiLockEvent, this, setValue);
                EventManager.Broadcast(em);
            }
		}
		
		// Update is called once per frame
		void Update () {
			if(bRemoveAllDialog)
			{
				foreach (Dialog dialog in dialogSys.dialogList)
				{
					if(dialog != null)
					{
						EventManager.RemoveEventListener(dialog.eventNameYes, OnEventYes, null);
						EventManager.RemoveEventListener(dialog.eventNameNo, OnEventNo, null);
						EventManager.RemoveEventListener(eventCoversationNextID, OnEventNext, null);
						dialog.DelDialog();
					}
				}
				dialogSys.dialogList.Clear();
				prevDialog = null;
				dialogSys.nextDialogID = 0;
				PauseAndLock(false,true);
				bRemoveAllDialog = false;
			}
			else
			{
				Dialog currDialog = GetCurrDialog();
				if (currDialog != null || prevDialog != null)
				{
					if (prevDialog != currDialog)
					{
						bool usesame = false;
						if(prevDialog != null && currDialog != null)
						{
							// Farl: add check dialog object to prevent missing dialog
							if(prevDialog.dialogName == currDialog.dialogName && prevDialog.GetDialogObj() != null)
							{
								usesame = true;
							}
						}
						if(usesame == false)
						{
							// Dialog Change
							if (prevDialog != null)
							{
								// UnPause and UnLock
								PauseAndLock(false,prevDialog.bPause);
								
								EventManager.RemoveEventListener(prevDialog.eventNameYes, OnEventYes, null);
								EventManager.RemoveEventListener(prevDialog.eventNameNo, OnEventNo, null);
								EventManager.RemoveEventListener(eventCoversationNextID, OnEventNext, null);
								prevDialog.Leave ();
								prevDialog = null;
							}
							
							if (currDialog != null)
							{
								// Pause and Lock
								PauseAndLock(true,currDialog.bPause);
								
								// Spawn UI
								EventManager.AddEventListener(currDialog.eventNameYes, OnEventYes, null);
								EventManager.AddEventListener(currDialog.eventNameNo, OnEventNo, null);
								EventManager.AddEventListener(eventCoversationNextID, OnEventNext, null);
								// Setup new dialog
								currDialog.Enter ();
							}
							prevDialog = currDialog;
						}
						else
						{
							currDialog.SetNext(prevDialog.GetDialogObj());
							prevDialog = currDialog;
						}	
					}
					// Update
					if (currDialog != null)
						currDialog.Update();
				}
			}
		}
	}

}