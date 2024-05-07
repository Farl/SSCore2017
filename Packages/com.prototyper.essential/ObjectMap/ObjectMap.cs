using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SS
{
    public static class ObjectMap
    {
        #region Classes / Enums
        private class ObjectData
        {
            public string name;
            public GameObject gameObject;
        }

        private class GroupData
        {
            public bool active = true;
            public List<GameObject> objects;
            public bool debugGroup = false;
        }
        #endregion

        #region Public

        public static GameObject GetGameObjectByName(string name)
        {
            if (string.IsNullOrEmpty(name)) return null;
            if (objectMap.TryGetValue(name, out var obj))
            {
                return obj.gameObject;
            }
            return null;
        }

        public static T GetComponentByName<T>(string name)
        {
            var go = GetGameObjectByName(name);
            if (go)
            {
                return go.GetComponent<T>();
            }
            return default(T);
        }

        public static void RegisterObject(GameObject gameObject, string objectID)
        {
            Register(gameObject, objectID);
        }

        public static void UnregisterObject(GameObject gameObject, string objectID)
        {
            Unregister(gameObject, objectID);
        }

        public static void Register(GameObject gameObject, string name)
        {
            if (gameObject == null)
                return;
            if (string.IsNullOrEmpty(name))
                name = gameObject.name;
            if (objectMap.TryAdd(name, new ObjectData { gameObject = gameObject, name = name }))
            {

            }
        }

        public static void Unregister(GameObject gameObject, string name)
        {
            if (gameObject == null)
                return;
            if (string.IsNullOrEmpty(name))
                name = gameObject.name;
            if (objectMap.TryGetValue(name, out ObjectData obj))
            {
                if (obj.gameObject == gameObject)
                {
                    objectMap.Remove(name);
                }
            }
        }

        public static void RegisterGroup(GameObject gameObject, string groupName)
        {
            if (string.IsNullOrEmpty(groupName))
                return;
            if (groupMap.TryGetValue(groupName, out GroupData group))
            {
                if (group.objects.Contains(gameObject))
                    return;
                group.objects.Add(gameObject);
            }
            else
            {
                group = new GroupData() { objects = new List<GameObject> { gameObject } };
                groupMap.Add(groupName, group);
            }
        }

        public static void UnregisterGroup(GameObject gameObject, string groupName)
        {
            if (string.IsNullOrEmpty(groupName))
                return;
            if (groupMap.TryGetValue(groupName, out GroupData group))
            {
                group.objects.Remove(gameObject);
                group.objects.RemoveAll(o => o == null);
                if (group.objects.Count == 0)
                {
                    if (group.debugGroup)
                    {
                        DebugMenu.Remove(groupName);
                    }
                    groupMap.Remove(groupName);
                }
            }
        }

        public static void EnableDebugGroup(string groupName)
        {
            if (string.IsNullOrEmpty(groupName))
                return;
            if (groupMap.TryGetValue(groupName, out GroupData group))
            {
                if (group.debugGroup)
                    return;
                group.debugGroup = true;
                DebugMenu.AddToggle("ObjectGroup", groupName,
                    () => { return group.active; },
                    (active) => {
                        group.active = active;
                        foreach (var obj in group.objects)
                        {
                            if (obj == null)
                                continue;
                            obj.SetActive(group.active);
                        }
                    }
                );
            }
            else
            {
                groupMap.Add(groupName, new GroupData { debugGroup = true });
            }
        }
        #endregion

        #region Private/Protected
        private static Dictionary<string, ObjectData> objectMap = new Dictionary<string, ObjectData>();
        private static Dictionary<string, GroupData> groupMap = new Dictionary<string, GroupData>();

        #endregion
    }
}