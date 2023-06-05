using UnityEngine;
using System.Collections;

namespace SS.Core
{
    public class DontDestroyOnLoad : MonoBehaviour
    {

        // Use this for initialization
        void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }
    }

}
