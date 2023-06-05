using UnityEngine;
using System.Collections;

using UnityEditor;
using System.Linq;
using System;

using System.Reflection;

namespace SS.Core
{

    public static class SerializedPropertyUtility
    {
        public static bool IsArrayElement(SerializedProperty prop)
        {
            return (prop.propertyPath[prop.propertyPath.Length - 1] == ']');
        }

        public static int IndexOfArrayElement(SerializedProperty prop)
        {
            if (IsArrayElement(prop))
                return Convert.ToInt32(prop.propertyPath.Substring(prop.propertyPath.LastIndexOf("[")).Replace("[", "").Replace("]", ""));
            else
                return 0;
        }

        public static SerializedProperty GetArrayParentProperty(SerializedProperty property)
        {
            if (IsArrayElement(property) || property.propertyPath.EndsWith(".Array"))
            {
                int arrayStrIdx = property.propertyPath.LastIndexOf(".Array");
                string newPath = property.propertyPath.Substring(0, arrayStrIdx);
                return property.serializedObject.FindProperty(newPath);
            }
            else
            {
                return null;
            }
        }

        public static SerializedProperty GetArrayParentProperty2(SerializedProperty property)
        {
            //Debug.Log("Searching parent of " + property.propertyPath + "...");
            if (!IsArrayElement(property))
                return null;

            // Backup
            var path = property.propertyPath;
            var parentPath = path.Substring(0, path.LastIndexOf(".data["));
            var currProp = property.Copy();
            currProp.Reset();

            // Stupid search
            var prevProp = currProp.Copy();
            bool nextProperty = currProp.Next(true);
            while (nextProperty)
            {
                if (currProp.propertyPath == path)
                {
                    //Debug.Log("... Found " + prevProp.propertyPath);
                    return prevProp;
                }

                // record only array property
                if (currProp.propertyPath == parentPath)
                    prevProp = currProp.Copy();

                nextProperty = currProp.Next(true);
            }
            return null;
        }

        public static object GetParent(SerializedProperty prop)
        {
            var path = prop.propertyPath.Replace(".Array.data[", "[");
            object obj = prop.serializedObject.targetObjects[0];
            var elements = path.Split('.');
            foreach (var element in elements.Take(elements.Length - 1))
            {
                if (element.Contains("["))
                {
                    var elementName = element.Substring(0, element.IndexOf("["));
                    var index = Convert.ToInt32(element.Substring(element.IndexOf("[")).Replace("[", "").Replace("]", ""));
                    obj = GetValue(obj, elementName, index);
                }
                else
                {
                    obj = GetValue(obj, element);
                }
            }
            return obj;
        }

        public static object GetValue(object source, string name)
        {
            if (source == null)
                return null;
            var type = source.GetType();
            var f = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if (f == null)
            {
                var p = type.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (p == null)
                    return null;
                return p.GetValue(source, null);
            }
            return f.GetValue(source);
        }

        public static object GetValue(object source, string name, int index)
        {
            var enumerable = GetValue(source, name) as IEnumerable;
            var enm = enumerable.GetEnumerator();
            while (index-- >= 0)
                enm.MoveNext();
            return enm.Current;
        }

        // Ref: https://answers.unity.com/questions/1347203/a-smarter-way-to-get-the-type-of-serializedpropert.html
        public static object GetValue(this SerializedProperty property)
        {
            System.Type parentType = property.serializedObject.targetObject.GetType();
            System.Reflection.FieldInfo fi = parentType.GetFieldViaPath(property.propertyPath);
            return fi.GetValue(property.serializedObject.targetObject);
        }

        // Ref: https://answers.unity.com/questions/1347203/a-smarter-way-to-get-the-type-of-serializedpropert.html
        public static void SetValue(this SerializedProperty property, object value)
        {
            System.Type parentType = property.serializedObject.targetObject.GetType();
            System.Reflection.FieldInfo fi = parentType.GetFieldViaPath(property.propertyPath);//this FieldInfo contains the type.
            fi.SetValue(property.serializedObject.targetObject, value);
        }

        // Ref: https://answers.unity.com/questions/1347203/a-smarter-way-to-get-the-type-of-serializedpropert.html
        public static System.Type GetType(SerializedProperty property)
        {
            System.Type parentType = property.serializedObject.targetObject.GetType();
            System.Reflection.FieldInfo fi = parentType.GetFieldViaPath(property.propertyPath);
            return fi.FieldType;
        }

        // Ref: https://answers.unity.com/questions/1347203/a-smarter-way-to-get-the-type-of-serializedpropert.html
        public static System.Reflection.FieldInfo GetFieldViaPath(this System.Type type, string path)
        {
            System.Type parentType = type;
            System.Reflection.FieldInfo fi = type.GetField(path);
            string[] perDot = path.Split('.');
            foreach (string fieldName in perDot)
            {
                // Support array and list
                if (fieldName == "Array")
                    continue;
                if (fieldName.StartsWith("data["))
                {
                    var arrayElementType = parentType.GetElementType();
                    if (arrayElementType == null && parentType.IsGenericType)
                    {
                        // Ref: https://stackoverflow.com/questions/1043755/c-sharp-generic-list-t-how-to-get-the-type-of-t
                        parentType = parentType.GetGenericArguments()[0];
                    }
                }
                else
                {
                    fi = parentType.GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    if (fi != null)
                        parentType = fi.FieldType;
                    else
                        return null;
                }
            }
            if (fi != null)
                return fi;
            else return null;
        }
    }
}