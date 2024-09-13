using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Globalization;

namespace SS
{
    public class TMPProceduralAnimation : MonoBehaviour
    {
        [SerializeField]
        protected TMP_Text textComp;
        [SerializeField]
        protected string linkID;
        [SerializeField]
        protected float updateInterval = 0.01f;

        protected bool IsAnimating { get; set; } = false;
        protected bool IsChangeByAnimation { get; set; } = false;

        protected virtual TMP_VertexDataUpdateFlags updateFlags => TMP_VertexDataUpdateFlags.None;

        protected virtual void Awake()
        {
            // Subscribe to event fired when text object has been regenerated.
            TMPro_EventManager.TEXT_CHANGED_EVENT.Add(OnTextChanged);
        }

        protected virtual void OnDestroy()
        {
            TMPro_EventManager.TEXT_CHANGED_EVENT.Remove(OnTextChanged);
        }

        protected virtual void OnEnable()
        {
        }

        protected virtual void OnDisable()
        {
            StopAllCoroutines();
            IsAnimating = false;
        }

        private void OnTextChanged(Object obj)
        {
            if (obj != textComp)
                return;
            if (IsChangeByAnimation || this.enabled == false)
                return;

            StopAllCoroutines();
            IsAnimating = false;

            var textInfo = textComp.textInfo;
            var linkInfos = textInfo.linkInfo;
            TMP_LinkInfo linkInfo = new TMP_LinkInfo()
            {
                linkIdFirstCharacterIndex = 0,
                linkIdLength = textInfo.characterCount
            };
            
            bool doAnimate = false;
            if (linkInfos != null)
            {
                if (string.IsNullOrEmpty(linkID))
                {
                    doAnimate = true;
                }
                else
                {
                    // Check linkCount to make sure the latest link info
                    // https://forum.unity.com/threads/tmp-textinfo-linkinfo-does-not-update-after-changing-text.1070729/#post-7463906
                    for (var i = 0; i < linkInfos.Length && i < textInfo.linkCount; i++)
                    {
                        var li = linkInfos[i];
                        if (li.GetLinkID().Equals(linkID))
                        {
                            linkInfo = li;
                            doAnimate = true;
                            break;
                        }
                    }
                }
            }
            if (doAnimate)
            {
                StartCoroutine(AnimationCoroutine(textInfo, linkInfo));
            }
            else
            {
                ResetText();
            }
        }

        protected virtual void ResetText()
        {
            if (textComp == null)
                return;

            IsChangeByAnimation = true;
            textComp.ForceMeshUpdate();
            IsChangeByAnimation = false;
        }

        protected virtual IEnumerator AnimationCoroutine(TMP_TextInfo textInfo, TMP_LinkInfo linkInfo)
        {
            if (textComp == null)
                yield break;

            IsAnimating = true;

            while (IsAnimating)
            {
                if (updateFlags != TMP_VertexDataUpdateFlags.None)
                {
                    // New function which pushes (all) updated vertex data to the appropriate meshes when using either the Mesh Renderer or CanvasRenderer.
                    IsChangeByAnimation = true; // Prevent infinity recursive
                    textComp.UpdateVertexData(updateFlags);
                    IsChangeByAnimation = false;
                }
            }
        }
    }
}
