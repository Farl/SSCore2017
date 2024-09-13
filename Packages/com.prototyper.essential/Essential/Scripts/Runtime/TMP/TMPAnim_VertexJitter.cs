using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace SS
{
    public class TMPAnim_VertexJitter : TMPProceduralAnimation
    {
        public enum AnimationMode
        {
            None = -1,
            Random = 0,
            Custom = 1,
        }

        [System.Flags]
        public enum Dimension
        {
            X = 1 << 0,
            Y = 1 << 1,
            Z = 1 << 2
        }

        [System.Serializable]
        public class AnimationData
        {
            [Header("Basic")]
            public AnimationMode mode = AnimationMode.Random;
            public Dimension dimension = 0;
            public WrapMode wrapMode = WrapMode.Default;
            public Vector3 valueFrom = new Vector3(0, 0, 0);
            public Vector3 valueTo = new Vector3(0, 0, 0);
            public float speed = 1.0f;
            public float scale = 1.0f;
            [Range(-1f, 1f)]
            public float timeOffset = 0.1f;
            [Header("Custom")]
            public AnimationCurve curve;

            private float[] randoms = null;
            private const int randomCount = 1024;

            private void RandomInitialize()
            {
                if (mode == AnimationMode.Random && randoms == null)
                {
                    randoms = new float[randomCount];
                    for (var i = 0; i < randomCount; i++)
                    {
                        randoms[i] = Random.Range(0.0f, 1.0f);
                    }
                }
            }

            public float UpdateFactor(float time)
            {
                time = (time * speed);
                switch (wrapMode)
                {
                    case WrapMode.PingPong:
                        return Mathf.PingPong(time, 1.0f);
                    case WrapMode.Loop:
                        while (time > 1.0f)
                        {
                            time -= 1.0f;
                        }
                        while (time < 0.0f)
                        {
                            time += 1.0f;
                        }
                        return time;
                    default:
                    case WrapMode.Default:
                        while (time > 1.0f)
                        {
                            time -= 1.0f;
                        }
                        return time;
                    case WrapMode.Once:
                    case WrapMode.ClampForever:
                        return Mathf.Clamp01(time);
                }
            }

            public Vector3 Update(float time, int characterIndex)
            {
                if (mode == AnimationMode.None)
                    return Vector3.zero;

                Vector3 returnValue = Vector3.zero;
                for (int i = 0; i < 3; i++)
                {
                    if ((((int)dimension) & (1 << i)) == 0)
                    {
                        continue;
                    }
                    switch (mode)
                    {
                        default:
                        case AnimationMode.Random:
                            RandomInitialize();
                            //returnValue[i] = Random.Range(randomRange.x, randomRange.y);
                            int randomIdx = (i + characterIndex * 3 + (int)time) % randomCount;
                            returnValue[i] = randoms[randomIdx] * (valueTo[i] - valueFrom[i]) + valueFrom[i];
                            break;

                        case AnimationMode.Custom:
                            float factor = UpdateFactor(time + characterIndex * timeOffset);

                            if (curve != null)
                            {
                                factor = curve.Evaluate(factor);
                            }

                            returnValue[i] = Mathf.Lerp(valueFrom[i], valueTo[i], factor);
                            break;
                    }
                }
                return returnValue * scale;
            }
        }

        [SerializeField] private AnimationData positionAnim;
        [SerializeField] private AnimationData rotationAnim;

        protected override TMP_VertexDataUpdateFlags updateFlags => base.updateFlags;

        protected override IEnumerator AnimationCoroutine(TMP_TextInfo textInfo, TMP_LinkInfo linkInfo)
        {
            if (textComp == null)
                yield break;

            IsAnimating = true;

            // Force the text object to update right away so we can have geometry to modify right from the start.
            IsChangeByAnimation = true;
            textComp.ForceMeshUpdate();
            IsChangeByAnimation = false;

            Matrix4x4 matrix;

            float timer = 0;

            // Cache the vertex data of the text object as the Jitter FX is applied to the original position of the characters.
            TMP_MeshInfo[] cachedMeshInfo = textInfo.CopyMeshInfoVertexData();

            int startCharacter = linkInfo.linkTextfirstCharacterIndex;
            int characterCount = linkInfo.linkTextLength;

            while (IsAnimating && characterCount > 0)
            {
                for (int i = 0; i < characterCount; i++)
                {
                    var currCharIdx = startCharacter + i;
                    TMP_CharacterInfo charInfo = textInfo.characterInfo[currCharIdx];

                    // Skip characters that are not visible and thus have no geometry to manipulate.
                    if (!charInfo.isVisible)
                        continue;

                    // Get the index of the material used by the current character.
                    int materialIndex = textInfo.characterInfo[currCharIdx].materialReferenceIndex;

                    // Get the index of the first vertex used by this text element.
                    int vertexIndex = textInfo.characterInfo[currCharIdx].vertexIndex;

                    // Get the cached vertices of the mesh used by this text element (character or sprite).
                    Vector3[] sourceVertices = cachedMeshInfo[materialIndex].vertices;

                    // Determine the center point of each character at the baseline.
                    //Vector2 charMidBasline = new Vector2((sourceVertices[vertexIndex + 0].x + sourceVertices[vertexIndex + 2].x) / 2, charInfo.baseLine);

                    // Determine the center point of each character.
                    Vector2 charMidBasline = (sourceVertices[vertexIndex + 0] + sourceVertices[vertexIndex + 2]) / 2;

                    // Need to translate all 4 vertices of each quad to aligned with middle of character / baseline.
                    // This is needed so the matrix TRS is applied at the origin for each character.
                    Vector3 offset = charMidBasline;

                    Vector3[] destinationVertices = textInfo.meshInfo[materialIndex].vertices;

                    destinationVertices[vertexIndex + 0] = sourceVertices[vertexIndex + 0] - offset;
                    destinationVertices[vertexIndex + 1] = sourceVertices[vertexIndex + 1] - offset;
                    destinationVertices[vertexIndex + 2] = sourceVertices[vertexIndex + 2] - offset;
                    destinationVertices[vertexIndex + 3] = sourceVertices[vertexIndex + 3] - offset;

                    var positionOffset = positionAnim.Update(timer, i);
                    var rotationOffset = rotationAnim.Update(timer, i);

                    matrix = Matrix4x4.TRS(positionOffset, Quaternion.Euler(rotationOffset), Vector3.one);

                    destinationVertices[vertexIndex + 0] = matrix.MultiplyPoint3x4(destinationVertices[vertexIndex + 0]);
                    destinationVertices[vertexIndex + 1] = matrix.MultiplyPoint3x4(destinationVertices[vertexIndex + 1]);
                    destinationVertices[vertexIndex + 2] = matrix.MultiplyPoint3x4(destinationVertices[vertexIndex + 2]);
                    destinationVertices[vertexIndex + 3] = matrix.MultiplyPoint3x4(destinationVertices[vertexIndex + 3]);

                    destinationVertices[vertexIndex + 0] += offset;
                    destinationVertices[vertexIndex + 1] += offset;
                    destinationVertices[vertexIndex + 2] += offset;
                    destinationVertices[vertexIndex + 3] += offset;
                }

                // Push changes into meshes
                for (int i = 0; i < textInfo.meshInfo.Length; i++)
                {
                    textInfo.meshInfo[i].mesh.vertices = textInfo.meshInfo[i].vertices;
                    textComp.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
                }

                float deltaTime = Mathf.Max(0, updateInterval);
                timer += deltaTime;
                yield return new WaitForSeconds(deltaTime);
            }
        }
    }
}
