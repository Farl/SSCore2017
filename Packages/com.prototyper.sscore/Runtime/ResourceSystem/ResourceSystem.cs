using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets;
#endif

namespace SS
{
    public static class ResourceSystem
    {
        private static Dictionary<string, OperationHandle> resourceMap = new Dictionary<string, OperationHandle>();
        private static HashSet<AssetReference> assetReferences = new HashSet<AssetReference>();

        public class OperationHandle
        {
            private AsyncOperationHandle asyncOpHandle;
            public OperationHandle(AsyncOperationHandle h)
            {
                asyncOpHandle = h;
            }

            public bool IsDone => (asyncOpHandle.IsValid()) ? asyncOpHandle.IsDone : false;

            public void Unload()
            {
                if (IsDone)
                    Addressables.Release(asyncOpHandle);
            }

            public T Result<T>()
            {
                return (T)asyncOpHandle.Result;
            }
        }
        public static OperationHandle Load<T>(AssetReference assetRef, System.Action<T> onComplete = null) where T : UnityEngine.Object
        {
            AsyncOperationHandle<T> ao = Addressables.LoadAssetAsync<T>(assetRef);
            if (onComplete != null)
            {
                ao.Completed += (x) => {
                    onComplete?.Invoke(x.Result);
                };
            }
            if (!assetReferences.Contains(assetRef))
                assetReferences.Add(assetRef);
            return new OperationHandle(ao);
        }

        public static OperationHandle Load<T>(string resourceName, System.Action<T> onComplete = null) where T : UnityEngine.Object
        {
            //return Resources.Load<T>(resourceName);


            if (resourceMap.ContainsKey(resourceName))
            {
                var oh = resourceMap[resourceName];
                onComplete?.Invoke(oh.Result<T>());
                return oh;
            }
            else
            {
                AsyncOperationHandle<T> ao = Addressables.LoadAssetAsync<T>(resourceName);
                ao.Completed += (x) => { onComplete?.Invoke(x.Result); };
                return new OperationHandle(ao);
            }
        }

        public static T CreateInstance<T>() where T: ScriptableObject
        {
            return ScriptableObject.CreateInstance<T>();
        }

        public static void Unload<T>(T obj)
        {
            Addressables.Release<T>(obj);
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

        public static void CreateAsset<T>(T obj, string fullPath) where T: Object
        {
#if UNITY_EDITOR
            string groupName = System.IO.Path.GetDirectoryName(fullPath).Replace('/', '-').Replace('\\', '-');

            AssetDatabase.CreateAsset(obj, fullPath);
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
#endif
        }
    }
}