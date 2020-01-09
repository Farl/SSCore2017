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


        public override void OnInit()
        {
            base.OnInit();

            foreach (var stage in stages)
            {
                stage.Init(this);
            }

            if (stages.Count > 0 && stages[0] != null)
            {
                SetNextStage(stages[0]);
            }
        }

        private void Update()
        {
            if (currStage)
            {
                currStage.Update();
            }
        }

        private void SetNextStage(Stage nextStage)
        {
            if (nextStage != null)
            {
                changeStageCoroutine = StartCoroutine(ChangeStage(nextStage));
            }
        }

        private void SetNextStage(string nextStageName)
        {
            // Check can enter
            if (_nextStageName != null)
            {
                return;
            }

            // Find next stage
            SetNextStage(stages.Find(x => x.name == nextStageName));
        }

        IEnumerator ChangeStage(Stage nextStage)
        {
            // Start
            _nextStageName = nextStage.name;

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

            _nextStageName = null;
        }
    }

}
