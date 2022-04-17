/**
 * Spread sheet import
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;
using System.Reflection;

namespace SS
{
    public class SpreadSheetImporter
    {

        public class ClassDefine
        {
            public string className;
            public SpreadSheet.Cell defineCell;
            public List<FieldDefine> fields = new List<FieldDefine>();

            public void Input(SpreadSheet.Cell cell)
            {
                string cellStringValue = cell.ToString();
                if (defineCell != null)
                {
                    if (defineCell.RowIndex == cell.RowIndex && cell.ColumnIndex > defineCell.ColumnIndex)
                    {
                        // Class field
                        fields.Add(new FieldDefine(cellStringValue));
                    }
                }
            }

            public string GenerateClass()
            {
                string result = "\t[System.Serializable]\n";
                result += "\tpublic " + className + '\n';
                result += "\t{\n";
                FieldDefine prevField = null;
                foreach (FieldDefine field in fields)
                {
                    if (field.IsComment)
                        continue;

                    if (prevField == null || field.fieldName != prevField.fieldName)
                    {
                        result += "\t\tpublic " + field.GetFieldString() + ";\n";
                    }
                    prevField = field;
                }
                result += "\t}\n";
                return result;
            }
        }

        public class FieldDefine
        {
            // Fileds
            private string fieldString;
            public string elementTypeName;
            public string typeName;
            public string fieldName;

            // Properties
            public bool IsList
            {
                get { return typeName.StartsWith("List<", StringComparison.InvariantCulture) && typeName.EndsWith(">", StringComparison.InvariantCulture); }
            }
            public bool IsArray
            {
                get { return typeName.EndsWith("[]", StringComparison.InvariantCulture); }
            }
            public bool IsDictionary
            {
                get { return typeName.StartsWith("Dictionary<", StringComparison.InvariantCulture) && typeName.EndsWith(">", StringComparison.InvariantCulture); }
            }
            public bool IsComment
            {
                get { return fieldString.StartsWith("//", StringComparison.InvariantCulture); }
            }

            public List<SpreadSheet.Cell> cellList = new List<SpreadSheet.Cell>();

            public FieldDefine(string str)
            {
                SetFieldString(str);
            }

            public void SetFieldString(string str)
            {
                fieldString = str;
                string[] tokens = str.Split(new char[] { ' ' }, StringSplitOptions.None);
                if (tokens.Length >= 2)
                {
                    typeName = tokens[0];
                    fieldName = tokens[1];
                    if (IsArray)
                    {
                        elementTypeName = typeName.Replace("[]", string.Empty);
                    }
                    else if (IsList)
                    {
                        elementTypeName = typeName.Replace("List<", string.Empty).Replace(">", string.Empty);
                    }
                    else if (IsDictionary)
                    {
                        elementTypeName = typeName.Replace("Dictionary<", string.Empty).Replace(">", string.Empty);
                    }
                }
            }

            public string GetFieldString()
            {
                return fieldString;
            }

            public string GetFieldString_List()
            {
                if (!string.IsNullOrEmpty(elementTypeName))
                {
                    return string.Format("List<{0}> {1}", elementTypeName, fieldName);
                }
                return fieldString;
            }

            public void Import(object element, Type entityType, int i, SpreadSheet.Row row, ClassDefine cd)
            {
                if (!IsComment)
                {
                    SpreadSheet.Cell currCell = row.GetCell(i + 1);
                    ImportValue(element, entityType, fieldName, i, currCell, cd);
                }
            }

            public void Import(ScriptableObject sObj, Type textType, ScriptDefine scriptDefine)
            {
                FieldInfo fi = textType.GetField(fieldName);

                // Is a List<...> filed
                if (fi != null && fi.FieldType.IsGenericType && fi.FieldType.GetGenericTypeDefinition() == typeof(List<>))
                {
                    // new List<'Data'>()
                    var container = Activator.CreateInstance(fi.FieldType);
                    fi.SetValue(sObj, container);

                    Type entityType = fi.FieldType.GetGenericArguments()[0];

                    // Find List.Add(...)
                    MethodInfo mi = fi.FieldType.GetMethod("Add", new Type[] { entityType });
                    if (mi != null)
                    {
                        foreach (var cell in cellList)
                        {
                            var row = cell.Row;

                            // find this 'Data' class define
                            ClassDefine cd = scriptDefine.classDefines.Find((x) => x.className.Replace("class ", string.Empty) == elementTypeName);
                            if (cd != null && row != null)
                            {
                                // new a 'Data' class
                                var element = Activator.CreateInstance(entityType);

                                // for each fileds in this class
                                for (int i = 0; i < cd.fields.Count; i++)
                                {
                                    FieldDefine assetField = cd.fields[i];
                                    assetField.Import(element, entityType, i, row, cd);
                                }

                                // List.Add(element)
                                mi.Invoke(container, new object[] { element });
                            }
                        }
                    }
                }
                else
                {

                }
            }
        }

        public class ScriptDefine
        {
            private string templatePath
            {
                get
                {
                    var guid = AssetDatabase.FindAssets("t:TextAsset SpreadSheetScriptTemplate");
                    return Path.GetFullPath(AssetDatabase.GUIDToAssetPath(guid[0]));
                }
            }
            private string outputPath
            {
                get {
                    // TODO: by project setting
                    return "";
                }
            }

            public List<string> usingNamespaces = new List<string>();
            public string filename;
            public string _classString;
            public string className;

            public string mainClass
            {
                get
                {
                    return _classString;
                }
                set
                {
                    _classString = value;
                    className = _classString.Replace("class", string.Empty).Trim();
                }
            }
            public List<ClassDefine> classDefines = new List<ClassDefine>();
            private ClassDefine currentClass;

            public List<FieldDefine> fields = new List<FieldDefine>();
            private FieldDefine lastField;

            public ScriptDefine()
            {
            }

            public bool Input(SpreadSheet.Cell cell)
            {
                string cellStringValue = cell.ToString();
                if (cell.ColumnIndex == 0)
                {
                    if (cellStringValue.StartsWith("//", System.StringComparison.InvariantCulture))
                    {
                        // comments. Ignore
                        return false;
                    }
                    else if (cellStringValue.StartsWith("class ", System.StringComparison.InvariantCulture))
                    {
                        if (mainClass == null)
                        {
                            mainClass = cellStringValue;
                        }
                        else
                        {
                            // class definition
                            currentClass = new ClassDefine()
                            {
                                className = cellStringValue,
                                defineCell = cell
                            };
                            classDefines.Add(currentClass);
                        }
                    }
                    else if (!string.IsNullOrEmpty(cellStringValue.Trim()) && !cellStringValue.StartsWith("//", StringComparison.InvariantCulture))
                    {
                        // Find the same field
                        FieldDefine fd = fields.Find((x) => x.GetFieldString() == cellStringValue);
                        if (!cellStringValue.Contains(" "))
                        {
                            // Use previos field
                            fd = lastField;
                        }
                        if (fd == null)
                        {
                            fd = new FieldDefine(cellStringValue);
                            fields.Add(fd);
                            lastField = fd;
                        }
                        if (fd != null)
                        {
                            fd.cellList.Add(cell);
                        }
                    }
                }
                else
                {
                    if (currentClass != null)
                    {
                        currentClass.Input(cell);
                    }
                }
                return true;
            }

            public void Import()
            {
                Type textType = SpreadSheetImporter.GetType("SS." + className);

                // new ScriptableObject
                var sObj = ScriptableObject.CreateInstance(textType);

                // For each filed in script
                foreach (FieldDefine fd in fields)
                {
                    fd.Import(sObj, textType, this);
                }
                AssetDatabase.CreateAsset(sObj, Path.Combine("Assets", filename + ".asset"));
            }

            public void GenerateClass()
            {
                string outFilePath = Path.Combine(Path.Combine(Application.dataPath, outputPath), className + ".cs");// + ".txt";

                Debug.Log(templatePath);
                string inFilePath = Path.Combine(Application.dataPath, templatePath);

                using (StreamReader inReader = File.OpenText(inFilePath))
                {
                    using (StreamWriter outWriter = File.CreateText(outFilePath))
                    {
                        while (!inReader.EndOfStream)
                        {
                            string currLine = inReader.ReadLine();

                            string[] tokens = currLine.Split(new char[] { '[', ']' }, StringSplitOptions.None);
                            if (tokens != null && tokens.Length >= 3)
                            {
                                if (tokens[1] == "ScriptComments")
                                {
                                    currLine = currLine.Replace("[ScriptComments]", filename);
                                }
                                else if (tokens[1] == "MainClassName")
                                {
                                    currLine = currLine.Replace("[MainClassName]", mainClass);
                                }
                                else if (tokens[1] == "Classes")
                                {
                                    string classes = string.Empty;
                                    foreach (ClassDefine cd in classDefines)
                                    {
                                        classes += cd.GenerateClass();
                                    }
                                    currLine = currLine.Replace("[Classes]", classes);
                                }
                                else if (tokens[1] == "Properties")
                                {
                                    string propertiesStr = string.Empty;
                                    foreach (FieldDefine p in fields)
                                    {
                                        propertiesStr += "\tpublic " + p.GetFieldString_List() + ";\n";
                                    }
                                    currLine = currLine.Replace("[Properties]", propertiesStr);
                                }
                                else if (tokens[1] == "UsingNamespaces")
                                {
                                    currLine = currLine.Replace("[UsingNamespaces]", "");
                                }
                            }
                            outWriter.WriteLine(currLine);
                        }
                        outWriter.Close();
                    }
                    inReader.Close();
                }
                AssetDatabase.Refresh();
            }
        }

        public static Type GetType(string typeName)
        {
            Type resultType = null;
            //Debug.Log(typeName);
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                resultType = assembly.GetType(typeName);
                if (resultType != null)
                    break;
            }
            //Debug.Log(resultType);
            return resultType;
        }


        static void SetValue(object element, FieldInfo efi, SpreadSheet.Cell currCell)
        {
            if (efi == null || currCell == null)
                return;

            Type fieldType = efi.FieldType;

            if (fieldType == typeof(int) || fieldType.IsEnum)
            {
                int result = currCell.GetValue<int>();

                efi.SetValue(element, result);
            }
            else if (fieldType == typeof(bool))
            {
                bool result = currCell.GetBooleanValue();
                efi.SetValue(element, result);
            }
            else if (fieldType == typeof(float))
            {
                float result = currCell.GetFloatValue();
                efi.SetValue(element, result);
            }
            else if (fieldType == typeof(double))
            {
                double result = currCell.GetDoubleValue();
                efi.SetValue(element, result);
            }
            else if (fieldType == typeof(string))
            {
                string result = currCell.GetStringValue();
                efi.SetValue(element, result);
            }
            else
            {
            }
        }

        // Import value
        static void ImportValue(object element, Type entityType, string fieldName, int i, SpreadSheet.Cell currCell, ClassDefine cd)
        {
            FieldInfo efi = entityType.GetField(fieldName);
            if (efi != null && currCell != null)
            {
                if (efi.FieldType.IsArray)
                {
                    ImportArray(element, efi, i, currCell, cd);
                }
                else
                {
                    SetValue(element, efi, currCell);
                }
            }
        }

        // import Array
        static void ImportArray(object arrayObj, FieldInfo efi, int i, SpreadSheet.Cell currCell, ClassDefine cd)
        {
            Type elementType = efi.FieldType.GetElementType();
            SpreadSheet.Row defineRow = cd.defineCell.Row;
            SpreadSheet.Cell defineCell = defineRow.GetCell(i + 1);

            // Get array
            Array array = efi.GetValue(arrayObj) as Array;
            if (array == null)
            {
                // count array length
                int numElement = 0;
                defineRow.ForEach((countCell) =>
                {
                    if (countCell.ToString() == defineCell.ToString())
                    {
                        numElement++;
                    }
                });

                // new Array
                array = Array.CreateInstance(elementType, numElement);

                // Assign array
                efi.SetValue(arrayObj, array);
            }
            if (array != null)
            {
                int idxElement = 0;
                foreach (var countCell in defineRow)
                {
                    if (countCell == defineCell)
                        break;
                    if (countCell.ToString() == defineCell.ToString())
                    {
                        idxElement++;
                    }
                };

                // set element value
                if (elementType == typeof(int))
                {
                    int result = currCell.GetIntegerValue();
                    array.SetValue(result, idxElement);
                }
                else if (elementType == typeof(bool))
                {
                    bool result = currCell.GetBooleanValue();
                    array.SetValue(result, idxElement);
                }
                else if (elementType == typeof(float))
                {
                    float result = currCell.GetFloatValue();
                    array.SetValue(result, idxElement);
                }
                else if (elementType == typeof(string))
                {
                    string result = currCell.GetStringValue();
                    array.SetValue(result, idxElement);
                }
            }
        }


        private static void ImportSpreadSheet(string path, bool script)
        {
            // Ignore temporary files
            if (path.Contains("~$"))
                return;

            // combine full file path
            string fullPath = Application.dataPath;
            DirectoryInfo di = Directory.GetParent(fullPath);
            fullPath = Path.Combine(di.ToString(), path);
            //Debug.Log(fullPath);

            SpreadSheet spreadSheet = new SpreadSheet();
            spreadSheet.Load(fullPath);
            
            ScriptDefine scriptDefine = null;
            spreadSheet.ForEach(
                eachSheet: (sheet) =>
                {
                    scriptDefine = new ScriptDefine()
                    {
                        filename = sheet.SheetName,
                    };
                },
                eachCell: (cell) =>
                {
                    scriptDefine?.Input(cell);
                }
            );

            if (!script)
            {
                scriptDefine.Import();
            }
            else
            {
                scriptDefine.GenerateClass();
            }
        }

        [MenuItem("Assets/SS Import/Spread sheet as Script")]
        public static void ImportSpreadSheetAsScript()
        {
            // for all selections
            string[] GUIDs = Selection.assetGUIDs;
            if (GUIDs != null)
            {
                foreach (string guid in GUIDs)
                {
                    string path = AssetDatabase.GUIDToAssetPath(guid);
                    //Debug.Log(path);

                    ImportSpreadSheet(path, true);
                }
            }
        }

        [MenuItem("Assets/SS Import/Spread sheet data")]
        public static void ImportSpreadSheetData()
        {
            // for all selections
            string[] GUIDs = Selection.assetGUIDs;
            if (GUIDs != null)
            {
                foreach (string guid in GUIDs)
                {
                    string path = AssetDatabase.GUIDToAssetPath(guid);
                    //Debug.Log(path);

                    ImportSpreadSheet(path, false);
                }
            }
        }
    }

}