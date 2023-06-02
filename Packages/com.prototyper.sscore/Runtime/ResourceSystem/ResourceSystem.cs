using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if USE_ADDRESSABLES
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
#endif

#if UNITY_EDITOR
using UnityEditor;
#if USE_ADDRESSABLES
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets;
#endif  // USE_ADDRESSABLES
#endif  // UNITY_EDITOR

namespace SS
{
#if !USE_ADDRESSABLES
    [System.Serializable]
    public class AssetReference
    {
        [SerializeField] string m_assetGUID = "";
        [SerializeField] string m_SubObjectName;
        [SerializeField] string m_SubObjectType = null;
        [SerializeField] public string path;
        public AssetReference(string path)
        {
            this.path = path;
        }
    }
#endif

    public static class ResourceSystem
    {
        private static Dictionary<string, OperationHandle> resourceMap = new Dictionary<string, OperationHandle>();
        private static HashSet<AssetReference> assetReferences = new HashSet<AssetReference>();

        public class OperationHandle
        {
#if USE_ADDRESSABLES
            private AsyncOperationHandle asyncOpHandle;
            public OperationHandle(AsyncOperationHandle h)
            {
                asyncOpHandle = h;
            }
            public bool IsDone => (asyncOpHandle.IsValid()) ? asyncOpHandle.IsDone : false;
            public void Unload()
            {
                if (IsDone)
                {
                    Addressables.Release(asyncOpHandle);
                }
            }
            public T Result<T>()
            {
                return (T)asyncOpHandle.Result;
            }
#else
            // TODO:
            private ResourceRequest asyncOpHandle;
            public OperationHandle(ResourceRequest h)
            {
                asyncOpHandle = h;
            }
            public bool IsDone => asyncOpHandle.isDone;
            public void Unload()
            {
                if (IsDone)
                {
                    Resources.UnloadAsset(asyncOpHandle.asset);
                }
            }
            public T Result<T>() where T : UnityEngine.Object
            {
                return (T)asyncOpHandle.asset;
            }
#endif
        }
        public static OperationHandle Load<T>(AssetReference assetRef, System.Action<T> onComplete = null) where T : UnityEngine.Object
        {
            #if USE_ADDRESSABLES
            AsyncOperationHandle<T> ao = Addressables.LoadAssetAsync<T>(assetRef);
            if (onComplete != null)
            {
                ao.Completed += (x) => {
                    onComplete?.Invoke(x.Result);
                };
            }
            #else
            // TODO:
            var ao = Resources.LoadAsync<T>(assetRef.path);
            #endif

            if (!assetReferences.Contains(assetRef))
                assetReferences.Add(assetRef);

            return new OperationHandle(ao);
        }

        public static OperationHandle Load<T>(string resourceName, System.Action<T> onComplete = null) where T : UnityEngine.Object
        {
            if (resourceMap.ContainsKey(resourceName))
            {
                var oh = resourceMap[resourceName];
                onComplete?.Invoke(oh.Result<T>());
                return oh;
            }
            else
            {
                #if USE_ADDRESSABLES
                AsyncOperationHandle<T> ao = Addressables.LoadAssetAsync<T>(resourceName);
                ao.Completed += (x) => { onComplete?.Invoke(x.Result); };
                #else
                var ao = Resources.LoadAsync<T>(resourceName);
                ao.completed += (x) => { onComplete?.Invoke(((ResourceRequest)x).asset as T); };
                #endif
                return new OperationHandle(ao);
            }
        }

        public static T CreateInstance<T>() where T: ScriptableObject
        {
            return ScriptableObject.CreateInstance<T>();
        }

        public static void Unload<T>(T obj)
        {
            #if USE_ADDRESSABLES
            Addressables.Release<T>(obj);
            #else
            Resources.UnloadAsset(obj as Object);
            #endif
        }

        public static void Unload(string resourceName)
        {
            if (resourceMap.TryGetValue(resourceName, out var oh))
            {
                oh.Unload();
            }
        }

        public static void Unload<T>(OperationHandle oh)
        {
            oh.Unload();
        }

#if UNITY_EDITOR
        public static void CreateAsset<T>(T obj, string fullPath) where T: Object
        {
            AssetDatabase.CreateAsset(obj, fullPath);

#if USE_ADDRESSABLES

            string groupName = System.IO.Path.GetDirectoryName(fullPath).Replace('/', '-').Replace('\\', '-');
            var guid = AssetDatabase.AssetPathToGUID(fullPath);
            //Debug.Log(fullPath + " " + guid);

            var settings = AddressableAssetSettingsDefaultObject.Settings;
            List<AddressableAssetGroupSchema> schemas = new List<AddressableAssetGroupSchema>()
            {
                settings.DefaultGroup.Schemas[0],
                settings.DefaultGroup.Schemas[1]
            };

            AddressableAssetGroup group = settings.FindGroup(groupName);
            if (group == null)
            {
                group = settings.CreateGroup(groupName, false, false, false, schemas,
                  typeof(UnityEditor.AddressableAssets.Settings.AddressableAssetGroupSchema));
            }
            settings.CreateOrMoveEntry(guid, group);
#else
            // ...
#endif

            AssetDatabase.Refresh();
        }
#endif
    }
}