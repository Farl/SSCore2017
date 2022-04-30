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

        private void CreateStageButtons()
        {
            if (!DebugMenu.Instance)
                return;

            if (stages != null)
            {
                var templateGO = DebugMenu.Instance.CreateButtonList();
                foreach (var stage in stages)
                {
                    var id = stage.name;

                    DebugMenu.Instance.AddButtonListButton(templateGO, () =>
                    {
                        SetNextStage(id);
                    },
                    id);
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

            // Test
            FadeUI.FadeTask fadeTask = FadeUI.StartFade(Color.black, 3f, 0f, 3f, false);
            fadeTask.SetPauseAt(FadeUI.State.Remain);
            yield return new WaitForSecondsRealtime(3.0f);

            EventManager.Broadcast(new EventMessage("UISystem.Open(LoadingUI)", this, true));

            // Leave
            if (currStage)
            {
                currStage.OnLeave();
                while (currStage.IsActive)
                {
                    yield return null;
                }
            }

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

            while (!LoadingUI.IsFinished())
            {
                yield return null;
            }

            EventManager.Broadcast(new EventMessage("UISystem.Open(LoadingUI)", this, false));

            // Fade
            fadeTask.Resume();

            _nextStageName = null;
        }
    }

}
