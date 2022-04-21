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
        public class OperationHandle
        {
            private AsyncOperationHandle asyncOpHandle;
            public OperationHandle(AsyncOperationHandle h)
            {
                asyncOpHandle = h;
            }

            public bool IsDone => (asyncOpHandle.IsValid()) ? asyncOpHandle.IsDone : false;
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
            return new OperationHandle(ao);
        }

        public static OperationHandle Load<T>(string resourceName, System.Action<T> onComplete = null) where T : UnityEngine.Object
        {
            //return Resources.Load<T>(resourceName);

            AsyncOperationHandle<T> ao = Addressables.LoadAssetAsync<T>(resourceName);

            ao.Completed += (x) => { onComplete?.Invoke(x.Result); };
            return new OperationHandle(ao);
        }

        public static T CreateInstance<T>() where T: ScriptableObject
        {
            return ScriptableObject.CreateInstance<T>();
        }

        public static void CreateAsset<T>(T obj, string fullPathWithoutExt, string ext) where T: Object
        {
#if UNITY_EDITOR
            string groupName = System.IO.Path.GetDirectoryName(fullPathWithoutExt).Replace('/', '-').Replace('\\', '-');

            AssetDatabase.CreateAsset(obj, fullPathWithoutExt);
            var guid = AssetDatabase.AssetPathToGUID(fullPathWithoutExt);

            Debug.Log(fullPathWithoutExt + " " + guid);

            var settings = AddressableAssetSettingsDefaultObject.Settings;
            List<AddressableAssetGroupSchema> schemas = new List<AddressableAssetGroupSchema>()
            {
                settings.DefaultGroup.Schemas[0],
                settings.DefaultGroup.Schemas[1]
            };

            AddressableAssetGroup group = settings.FindGroup(groupName);
            if (group == null)
                settings.CreateGroup(groupName, false, false, false, schemas,
                typeof(UnityEditor.AddressableAssets.Settings.AddressableAssetGroupSchema));
            settings.CreateOrMoveEntry(guid, group);
#endif
        }
    }
}