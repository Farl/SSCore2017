using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace SS
{
    public class TMPAnim_ColorBand : TMPProceduralAnimation
    {
        [SerializeField] private bool showDebugInfo;
        [SerializeField]
        private float speed = 1f;
        [SerializeField]
        private float width = 1f;
        [SerializeField]
        private Color color = Color.gray;

        protected override TMP_VertexDataUpdateFlags updateFlags => TMP_VertexDataUpdateFlags.Colors32;

        protected override void OnDisable()
        {
            base.OnDisable();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
        }

        protected override void ResetText()
        {
            base.ResetText();

            if (textComp == null)
                return;

            if (showDebugInfo)
                Debug.Log($"ResetText {textComp.text}");

            IsChangeByAnimation = true; // Prevent infinity recursive
            textComp.UpdateVertexData(updateFlags);
            IsChangeByAnimation = false;
        }

        protected override IEnumerator AnimationCoroutine(TMP_TextInfo textInfo, TMP_LinkInfo linkInfo)
        {
            if (textComp == null)
                yield break;

            IsAnimating = true;

            // Force the text object to update right away so we can have geometry to modify right from the start.
            IsChangeByAnimation = true;
            textComp.ForceMeshUpdate();
            IsChangeByAnimation = false;

            Color32 c0 = textComp.color;

            int startCharacter = linkInfo.linkTextfirstCharacterIndex;
            int characterCount = linkInfo.linkTextLength;
            float animCharacterIndex = 0; // Unit is character

            float time = Time.realtimeSinceStartup;
            float deltaTime = 0;

            while (IsAnimating)
            {
                bool isDirty = false;

                for (var i = 0; i < characterCount; i++)
                {
                    var offset = animCharacterIndex - i;
                    var diff = Mathf.Min(Mathf.Abs(offset), Mathf.Abs(offset + characterCount), Mathf.Abs(offset - characterCount));
                    float factor = Mathf.Clamp01(diff / Mathf.Max(0.01f, width));
                    isDirty |= DoAnimate(textInfo, i + startCharacter, Color32.Lerp(color, c0, Mathf.SmoothStep(0, 1, factor)));
                }

                if (isDirty)
                {
                    // New function which pushes (all) updated vertex data to the appropriate meshes when using either the Mesh Renderer or CanvasRenderer.
                    IsChangeByAnimation = true; // Prevent infinity recursive
                    textComp.UpdateVertexData(updateFlags);
                    IsChangeByAnimation = false;

                    // This last process could be done to only update the vertex data that has changed as opposed to all of the vertex data but it would require extra steps and knowing what type of renderer is used.
                    // These extra steps would be a performance optimization but it is unlikely that such optimization will be necessary.
                }

                yield return new WaitForSecondsRealtime(Mathf.Max(0f, updateInterval));

                deltaTime = Time.realtimeSinceStartup - time;
                time = Time.realtimeSinceStartup;
                animCharacterIndex = (animCharacterIndex + speed * deltaTime) % characterCount;
            }
        }

        // return is dirty
        protected virtual bool DoAnimate(TMP_TextInfo textInfo, int currentCharacter, Color32 c0)
        {
            // Only change the vertex color if the text element is visible.
            if (textInfo.characterInfo[currentCharacter].isVisible)
            {
                // Get the index of the material used by the current character.
                int materialIndex = textInfo.characterInfo[currentCharacter].materialReferenceIndex;

                // Get the vertex colors of the mesh used by this text element (character or sprite).
                var newVertexColors = textInfo.meshInfo[materialIndex].colors32;

                // Get the index of the first vertex used by this text element.
                int vertexIndex = textInfo.characterInfo[currentCharacter].vertexIndex;

                newVertexColors[vertexIndex + 0] = c0;
                newVertexColors[vertexIndex + 1] = c0;
                newVertexColors[vertexIndex + 2] = c0;
                newVertexColors[vertexIndex + 3] = c0;
                return true;
            }
            return false;
        }
    }
}
