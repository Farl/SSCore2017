using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ObjectMap
{
    #region Classes / Enums
    private class ObjectData
    {
        public string name;
        public GameObject gameObject;
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

    public static void Register(GameObject gameObject, string name)
    {
        if (gameObject == null)
            return;
        if (string.IsNullOrEmpty(name))
            name = gameObject.name;
        if (objectMap.TryAdd(name, new ObjectData { gameObject= gameObject, name = name }))
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
    #endregion

    #region Private/Protected
    private static Dictionary<string, ObjectData> objectMap = new Dictionary<string, ObjectData>();


    #endregion
}
