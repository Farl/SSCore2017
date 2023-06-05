using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SS.Core
{
	public class DialogSystemBase
	{
		public enum DialogPriority
		{
			LOW,
			MEDIUM,
			HIGH
		}

		private static int nextDialog = 0;
		private List<Dialog> dialogList = new List<Dialog>();
		private Dialog prevDialog = null;

		public int RequestDialog(string dialogType, string dialogTextID, string extraEventYes, string extraEventNo, string eventNext, EventArray onLoadEvent, params string[] cmdArray)
		{
			Dialog refDialog = null;

			int currDialogID = nextDialog;
			
			// Find dialog define in DialogSettings
			foreach (Dialog d in DialogSettings.Instance.dialogDefine)
			{
				if (d.dialogName == dialogType)
				{
					refDialog = d;
					break;
				}
			}
			
			Dialog newDialog = new Dialog(currDialogID, refDialog, cmdArray);
			
			// Override
			newDialog.dialogSys = this;
			
			if (onLoadEvent != null)
				newDialog.eventOnLoad += onLoadEvent;
			if (!string.IsNullOrEmpty(dialogTextID))
				newDialog.textID = dialogTextID;
			if (!string.IsNullOrEmpty(extraEventYes))
				newDialog.extraEventYes = extraEventYes;
			if (!string.IsNullOrEmpty(extraEventNo))
				newDialog.extraEventNo = extraEventNo;
			if (!string.IsNullOrEmpty(eventNext))
				newDialog.eventNext = eventNext;
			// Override
			
			dialogList.Add(newDialog);
			
			nextDialog++;
			
			return currDialogID;
		}
		
		public bool RemoveDialog(int id)
		{
			foreach (Dialog dialog in dialogList)
			{
				if (dialog.GetID() == id)
				{
					// TODO: 
					dialogList.Remove(dialog);
					return true;
				}
			}
			return false;
		}

		public void ClearDialog()
		{
			// TODO:

			foreach (Dialog dialog in dialogList)
			{
				if(dialog != null)
				{
					EventManager.RemoveEventListener(dialog.eventNameYes, OnEventYes, null);
					EventManager.RemoveEventListener(dialog.eventNameNo, OnEventNo, null);
					EventManager.RemoveEventListener(dialog.eventNext, OnEventNext, null);

					dialog.DelDialog();
				}
			}
			
			dialogList.Clear();

			prevDialog = null;
		}

		public bool Empty()
		{
			return dialogList.Count <= 0;
		}

		private Dialog GetCurrDialog()
		{
			if (dialogList.Count <= 0)
			{
				return null;
			}
			return dialogList[0];
		}

		public void Update()
		{
			Dialog currDialog = GetCurrDialog();
			if (currDialog != null || prevDialog != null)
			{
				if (prevDialog != currDialog)
				{
					bool useTheSameUI = false;

					// Check if different dialog use the same UI
					if (prevDialog != null && currDialog != null)
					{
						// Farl: add check dialog object to prevent missing dialog
						if (prevDialog.dialogName == currDialog.dialogName && prevDialog.GetDialogObj() != null)
						{
							useTheSameUI = true;
						}
					}

					if (!useTheSameUI)
					{
						// Dialog Change
						if (prevDialog != null)
						{							
							EventManager.RemoveEventListener(prevDialog.eventNameYes, OnEventYes);
							EventManager.RemoveEventListener(prevDialog.eventNameNo, OnEventNo);
							EventManager.RemoveEventListener(prevDialog.eventNext, OnEventNext);

							prevDialog.Leave();

							prevDialog = null;
						}
						
						if (currDialog != null)
						{							
							// Spawn UI
							EventManager.AddEventListener(currDialog.eventNameYes, OnEventYes);
							EventManager.AddEventListener(currDialog.eventNameNo, OnEventNo);
							EventManager.AddEventListener(currDialog.eventNext, OnEventNext);

							// Setup new dialog
							currDialog.Enter ();
						}
						prevDialog = currDialog;
					}
					else
					{
						// Continue use the same UI
						currDialog.SetNext(prevDialog);

						prevDialog = currDialog;
					}	
				}
				// Update
				if (currDialog != null)
				{
					currDialog.Update();
				}
			}
		}

		public void DrawGUI()
		{
			GUILayout.Label(string.Format("{0} dialogs.", dialogList.Count));
		}

		private void OnEventYes(EventMessage em, ref object paramRef)
        {
			Dialog currDialog = GetCurrDialog();
			if (currDialog != null)
			{
				currDialog.eventWhenYes.Broadcast(em.origSrc);

				if (currDialog.extraEventYes != null)
				{
                    EventMessage em2 = new EventMessage(currDialog.extraEventYes, em.origSrc, em.paramBool, em.paramString, _paramExtra: em.paramExtra);
                    EventManager.Broadcast(em);
				}
			}
		}
		
		private void OnEventNo(EventMessage em, ref object paramRef)
        {
			Dialog currDialog = GetCurrDialog();
			if (currDialog != null)
			{
				currDialog.eventWhenNo.Broadcast(em.origSrc);

				if (currDialog.extraEventNo != null)
                {
                    EventMessage em2 = new EventMessage(currDialog.extraEventNo, em.origSrc, em.paramBool, em.paramString, _paramExtra: em.paramExtra);
                    EventManager.Broadcast(em);
                }
			}
		}
		
		private void OnEventNext(EventMessage em, ref object paramRef)
		{
			prevDialog.Remove();
		}
	}
}