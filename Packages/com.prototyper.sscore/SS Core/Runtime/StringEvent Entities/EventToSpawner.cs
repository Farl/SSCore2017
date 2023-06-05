using UnityEngine;
using System.Collections;
using SS;
#if USE_ADDRESSABLES
using UnityEngine.AddressableAssets;
#endif

namespace SS.Core
{
    public class EventToSpawner : EventListener
    {

        public AssetReference m_spawnRef;
        public string m_spawnResource;
        public GameObject m_spawnObj;

        private GameObject spawnTemplate;
        private GameObject spawnedObj;

        private void OnLoadComplete(GameObject go)
        {
            if (go)
            {
                spawnTemplate = go;
            }
            if (spawnTemplate)
            {
                spawnedObj = Spawn(m_spawnObj, null, transform);
            }
        }

        protected override void OnEvent(bool paramBool)
        {
            if (paramBool)
            {
                if (spawnTemplate != null)
                    return;

                if (m_spawnRef != null)
                {
                    ResourceSystem.Load<GameObject>(m_spawnRef, OnLoadComplete);
                }
                else if (string.IsNullOrEmpty(m_spawnResource))
                {
                    ResourceSystem.Load<GameObject>(m_spawnResource, OnLoadComplete);
                }
                else if (m_spawnObj != null)
                {
                    OnLoadComplete(m_spawnObj);
                }

            }
            else
            {
                if (spawnedObj)
                {
                    Destroy(spawnedObj);
                }
            }
        }

        public static GameObject Spawn(GameObject spawnObj, Transform parent = null, Transform transform = null)
        {
            GameObject spawnedObj = null;
            spawnedObj = (GameObject)GameObject.Instantiate(spawnObj, transform.position, transform.rotation);
            if (parent)
            {
                spawnedObj.transform.SetParent(parent, true);
            }
            if (transform)
            {
                spawnedObj.transform.localRotation = transform.localRotation;
                spawnedObj.transform.localScale = transform.localScale;
            }
            return spawnedObj;
        }
    }

}