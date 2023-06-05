using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace SS
{
    public static class UIManager
    {
        private static Dictionary<string, UIEntity> _entities = new Dictionary<string, UIEntity>();
        private static List<ISaveLoadData> saveLoadInterfaces = new List<ISaveLoadData>();
        private static HashSet<UIRoot> uiRoots = new HashSet<UIRoot>();

        public static T GetUIEntity<T>(string key) where T : UIEntity
        {
            if (_entities.TryGetValue(key, out var entity))
            {
                return entity as T;
            }
            return null;
        }

        public static UIEntity GetUIEntity(string key)
        {
            return GetUIEntity<UIEntity>(key);
        }

        public static bool IsShow(string typeName)
        {
            var e = GetUIEntity(typeName);
            if (e != null)
            {
                return e.IsShow;
            }
            return false;
        }

        public static UIEntity ShowUI(string typeName, params object[] parameters)
        {
            var e = GetUIEntity(typeName);
            if (e != null)
            {
                e.Show(parameters);
            }
            return e;
        }

        public static UIEntity HideUI(string typeName, params object[] parameters)
        {
            var e = GetUIEntity(typeName);
            if (e != null)
            {
                e.Hide(parameters);
            }
            return e;
        }

        public static void Register(UIEntity entity)
        {
            var typeName = entity.TypeName;
            //Debug.Log(typeName);
            _entities.Add(typeName, entity);

            ISaveLoadData sl = entity as ISaveLoadData;
            if (sl != null)
            {
                saveLoadInterfaces.Add(sl);
            }
        }

        public static void Unregister(string  typeName)
        {
            if (_entities.ContainsKey(typeName))
            {
                Unregister(_entities[typeName]);
                _entities.Remove(typeName);
            }
        }

        public static void Unregister(UIEntity entity)
        {
            Assert.IsNotNull(entity);
            _entities.Remove(entity.TypeName);

            ISaveLoadData sl = entity as ISaveLoadData;
            if (sl != null)
            {
                saveLoadInterfaces.Remove(sl);
            }
        }

        public static void Register(UIRoot uiRoot)
        {
            uiRoots.Add(uiRoot);
        }

        public static void Unregister(UIRoot uiRoot)
        {
            uiRoots.Remove(uiRoot);
        }

        public static void SetupUIRoot()
        {
            foreach (var uiRoot in uiRoots)
            {
                uiRoot.Setup();
            }
        }

        public static void Update()
        {
            var updateList = new List<UIEntity>(_entities.Values);
            foreach (var entity in updateList)
            {
                if (entity.IsUpdating)
                    entity.OnUpdate();
            }
        }

        public static void ClearData()
        {
            var list = new List<ISaveLoadData>(saveLoadInterfaces);
            foreach (var sl in list)
            {
                sl?.OnClearData();
            }
        }

        public static void LoadData()
        {
            var list = new List<ISaveLoadData>(saveLoadInterfaces);
            foreach (var sl in list)
            {
                sl?.OnLoadData();
            }
        }

        public static void SaveData()
        {
            var list = new List<ISaveLoadData>(saveLoadInterfaces);
            foreach (var sl in list)
            {
                sl?.OnSaveData();
            }
        }
    }
}
