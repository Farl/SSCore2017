using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SS
{
    public class ObjectID : MonoBehaviour
    {
        [SerializeField] private string overrideID;

        private void Awake()
        {
            ObjectMap.Register(gameObject, overrideID);
        }

        private void OnDestroy()
        {
            ObjectMap.Unregister(gameObject, overrideID);
        }
    }

}