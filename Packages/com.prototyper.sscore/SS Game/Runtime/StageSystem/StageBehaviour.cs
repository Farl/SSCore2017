using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SS
{
    public class StageBehaviour : MonoBehaviour
    {
        public static IEnumerator CheckStageAwake()
        {
            IsAwake = false;

            new GameObject("StageBehaviour", typeof(StageBehaviour));

            while (!IsAwake)
            {
                yield return null;
            }
        }

        private static bool IsAwake { get; set; } = false;

        private void Awake()
        {
            IsAwake = true;
            Destroy(gameObject);
        }
    }
}
