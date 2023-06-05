using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace SS
{
    public class BuildInfoText : MonoBehaviour
    {
        [SerializeField]
        private float displayTime = 2f;
        [SerializeField]
        private float fadeOutTime = 0.3f;
        [SerializeField]
        private TextTableMapping text;

        [SerializeField]
        private CanvasGroup canvasGroup;

        [System.Diagnostics.Conditional("FINAL")]
        private void CheckFinal(ref string result)
        {
            result += "F";
        }
        [System.Diagnostics.Conditional("DISABLE_NFT")]
        private void CheckNFT(ref string result)
        {
            result += "n";
        }
        [System.Diagnostics.Conditional("DISABLE_STEP_COUNT")]
        private void CheckStep(ref string result)
        {
            result += "s";
        }

        protected void Awake()
        {
            var buildInfo = Resources.Load<BuildInfo>("BuildInfo");
            if (buildInfo)
            {
                DebugMenu.AddButton("Build Info", buildInfo.userName, null, null);
                DebugMenu.AddButton("Build Info", buildInfo.buildTime, null, null);
                DebugMenu.AddButton("Build Info", $"{buildInfo.version}({buildInfo.versionCode})", null, null);

                var defs = "_";
                foreach (var sym in buildInfo.defineSymbols)
                {
                    if (sym.Length > 0)
                        defs += sym[0];
                }
                CheckFinal(ref defs);
                CheckNFT(ref defs);
                CheckStep(ref defs);


                text?.SetText($"{buildInfo.version}({buildInfo.versionCode}) {buildInfo.buildTime}{defs}");
                if (canvasGroup)
                    canvasGroup.alpha = 0;
            }
        }

        public void OnShowText()
        {
            StopAllCoroutines();
            StartCoroutine(ShowText());
        }

        private IEnumerator ShowText()
        {
            float timer = 0;
            for (timer = 0; timer < displayTime; timer += Time.unscaledDeltaTime)
            {
                if (canvasGroup)
                {
                    canvasGroup.alpha = 1;
                }
                yield return null;
            }
            if (fadeOutTime > 0)
            {
                for (timer = 0; timer < fadeOutTime; timer += Time.unscaledDeltaTime)
                {
                    if (canvasGroup)
                    {
                        canvasGroup.alpha = 1 - Mathf.Clamp01(timer / fadeOutTime);
                    }
                    yield return null;
                }
            }
            if (canvasGroup)
                canvasGroup.alpha = 0;
        }
    }
}
