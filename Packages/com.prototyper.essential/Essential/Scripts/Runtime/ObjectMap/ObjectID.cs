using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SS
{
    public class ObjectID : MonoBehaviour
    {
        [SerializeField] private GameObject altGameObject;
        [SerializeField] private string overrideID;
        [SerializeField] public string groupID;

        public string objectID => string.IsNullOrEmpty(overrideID)? gameObject.name : overrideID;

        public new GameObject gameObject
        {
            get
            {
                if (altGameObject != null)
                {
                    return altGameObject;
                }
                return base.gameObject;
            }
        }

        private void Awake()
        {
            ObjectMap.RegisterObject(gameObject, objectID);
            ObjectMap.RegisterGroup(gameObject, groupID);
        }

        private void OnDestroy()
        {
            ObjectMap.UnregisterObject(gameObject, objectID);
            ObjectMap.UnregisterGroup(gameObject, groupID);
        }
    }

}