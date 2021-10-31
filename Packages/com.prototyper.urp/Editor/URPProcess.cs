using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Build;
using System.IO;
using UnityEditor.Rendering;

[InitializeOnLoad]
public class URPProcess : IPreprocessShaders
{
    static URPProcess()
    {
        // UniversalRenderPipeline.cs editorResource, our own UniversalRenderPipelineEditorResources
        FindAndReplace("bf2edee5c58d82540a51f03df9d42094", "a3d8d823eedde654bb4c11a1cfaf1abb", "b423cb80e2e434d18b987793a927257a");
        // ShaderUtil.cs shader s_ShaderGUIDs[0](Lit) s_ShaderGUIDs[1](SimpleLit)
        FindAndReplace("a59ed397e0917cd4aa1de2bb1495fead", "933532a4fcc9baf4fa0491de14d08ed7", "8d2bb70cbf9db8d4da26e15b26e74248");
        AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);

    }

    public int callbackOrder => 0;

    public void OnProcessShader(Shader shader, ShaderSnippetData snippet, IList<ShaderCompilerData> data)
    {
        if (AssetDatabase.GUIDFromAssetPath(AssetDatabase.GetAssetPath(shader)).ToString().Equals("933532a4fcc9baf4fa0491de14d08ed7"))
        //if (shader.name.Contains("Universal Render Pipeline/Lit"))
        {
            Debug.LogWarning(AssetDatabase.GetAssetPath(shader));
            Debug.LogWarning(AssetDatabase.GUIDFromAssetPath(AssetDatabase.GetAssetPath(shader)));
   
            data.Clear();
        }
    }

    static void FindAndReplace(string guid, string find, string replace)
    {
        var p = AssetDatabase.GUIDToAssetPath(guid);
        Debug.Log(p);
        var fp = Path.GetFullPath(p);
        Debug.Log(fp);

        var text = File.ReadAllText(fp);
        text = text.Replace(find, replace);
        File.WriteAllText(fp, text);
    }
}