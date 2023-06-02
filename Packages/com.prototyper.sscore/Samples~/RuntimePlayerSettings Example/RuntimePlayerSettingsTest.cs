using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SS
{
    public class TestTest: DataAsset<TestTest>
    {

    }

    public class RuntimePlayerSettingsTest : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
            var inst = RuntimePlayerSettings.Instance;
            Debug.Log(RuntimePlayerSettings.IsLoading);
            Debug.Log(TestTest.IsLoading);
            var inst2 = TestTest.Instance;
            if (inst == null)
            {
                Debug.Log("null");
            }
            else
            {
                Debug.Log($"{inst.scenes.Count}");
                enabled = false;
            }
        }
    }
}
