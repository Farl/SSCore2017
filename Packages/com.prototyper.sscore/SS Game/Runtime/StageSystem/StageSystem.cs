using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SS
{

    public class StageSystem : InitBehaviour
    {
        public List<Stage> stages = new List<Stage>();

        private Stage currStage;
        private string _nextStageName;
        Coroutine changeStageCoroutine;

        private static StageSystem _instance;

        public override int InitOrder => 1; // After DebugMenu

        private void CreateStageButtons()
        {
            if (!DebugMenu.Instance)
                return;

            if (stages != null)
            {
                var bl = DebugMenu.Instance.GetButtonList("Stage");
                if (bl != null)
                {
                    foreach (var stage in stages)
                    {
                        var id = stage.name;

                        bl.AddButtonListButton(() =>
                        {
                            SetNextStage(id);
                        },
                        id);
                    }
                }
            }
        }


        public override void OnInit()
        {
            base.OnInit();

            if (_instance == null)
            {
                _instance = this;

                // Create debug menu
                CreateStageButtons();

                foreach (var stage in stages)
                {
                    stage.Init(this);
                }

                if (stages.Count > 0 && stages[0] != null)
                {
                    currStage = stages[0];
                }
            }
            else
            {
                Destroy(this);
            }
        }

        private void OnDestroy()
        {
            if (_instance == this)
            {
                _instance = null;
            }
        }

        private void Update()
        {
            if (currStage)
            {
                currStage.Update();
            }
        }

        public static bool SetNextStage(string nextStageName)
        {
            if (_instance)
            {
                _instance._SetNextStage(nextStageName);
                return true;
            }
            return false;
        }

        private void _SetNextStage(Stage nextStage)
        {
            if (nextStage != null && currStage.CanEnter(nextStage))
            {
                changeStageCoroutine = StartCoroutine(ChangeStage(nextStage));
            }
        }

        private void _SetNextStage(string nextStageName)
        {
            // Check can enter
            if (!string.IsNullOrEmpty(nextStageName))
            {
                // Find next stage
                Stage nextStage = stages.Find(x => x.name == nextStageName);
                _SetNextStage(nextStage);
            }
        }

        IEnumerator ChangeStage(Stage nextStage)
        {
            // Start
            _nextStageName = nextStage.name;

            var fadeTime = (currStage) ? currStage.leaveBlackOutTime : 3.0f;

            // (Testing) Black out begin
            var fadeTask = FadeUI.StartFade(Color.black,
                fadeTime, 0f, fadeTime, false);
            fadeTask.SetPauseAt(FadeUI.State.Remain);

            yield return new WaitForSecondsRealtime(fadeTime);

            // Open loading UI
            EventManager.Broadcast(new EventMessage("UISystem.Open(LoadingUI)", this, true));

            // TODO: Check loading UI state
            while (LoadingUI.Instance && (!LoadingUI.Instance.IsShow && !LoadingUI.Instance.IsActive))
            {
                yield return null;
            }

            // (Testing) Black out end
            fadeTask.Resume();

            // Leave
            if (currStage)
            {
                currStage.OnLeave();
                while (currStage.IsActive)
                {
                    yield return null;
                }
            }

            fadeTime = (nextStage) ? nextStage.enterBlackOutTime : 3.0f;

            // Enter
            if (nextStage)
            {
                nextStage.OnEnter();
                while (!nextStage.IsActive)
                {
                    yield return null;
                }
                currStage = nextStage;
            }

            // Magic number. Wait InitBehaviour.Awake()
            for (var i = 0; i < 5; i++)
            {
                yield return null;
            }

            while (!InitManager.IsEmpty())
            {
                yield return null;
            }

            // (Testing) Black out begin
            fadeTask = FadeUI.StartFade(Color.white,
                fadeTime, 0f, fadeTime, false);
            fadeTask.SetPauseAt(FadeUI.State.Remain);

            yield return new WaitForSecondsRealtime(fadeTime);

            // (Testing) Black out end
            fadeTask.Resume();

            // Close loding UI
            EventManager.Broadcast(new EventMessage("UISystem.Open(LoadingUI)", this, false));

            _nextStageName = null;
        }
    }

}
