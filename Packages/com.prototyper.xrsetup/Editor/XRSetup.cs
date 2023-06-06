using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;

using Newtonsoft.Json;

using System.IO;
using System.Reflection;
using System.Text;

namespace SS
{

    public class XRSetup : EditorWindow
    {
        static AddRequest request;

        [MenuItem("Tools/SS/XR Setup")]
        private static void Open()
        {
            var w = XRSetup.GetWindow<XRSetup>();
            w.titleContent = new GUIContent("XR Setup");
        }

        static void Progress()
        {
            if (request.IsCompleted)
            {
                if (request.Status == StatusCode.Success)
                    Debug.Log("Installed: " + request.Result.packageId);
                else if (request.Status >= StatusCode.Failure)
                    Debug.Log(request.Error.message);

                EditorApplication.update -= Progress;
            }
        }

        private void OnGUI()
        {
            if (GUILayout.Button("Import OpenXR Plugin"))
            {
                // Import plug-in com.unity.xr.openxr
                request = Client.Add("com.unity.xr.openxr");
                EditorApplication.update += Progress;
            }

            if (GUILayout.Button("Import XR Interaction Toolkits"))
            {
                request = Client.Add("com.unity.xr.interaction.toolkit");
                EditorApplication.update += Progress;
            }

            EditorGUILayout.Separator();

            if (GUILayout.Button("Install Vive Plugin"))
            {
                // Install plugin
                // https://developer.vive.com/resources/openxr/openxr-mobile/tutorials/unity/how-install-vive-wave-openxr-plugin/

                // Add scope
                // https://forum.unity.com/threads/how-to-add-a-scoped-registry-using-code.1077758/
                //internal static AddScopedRegistryRequest AddScopedRegistry(string registryName, string url, string[] scopes)
                AddScopedRegistry(new ScopedRegistry
                {
                    name = "Vive",
                    url = "https://npm-registry.vive.com",
                    scopes = new string[] {
                    "com.htc.upm"
                }
                });

                // Add package
                request = Client.Add("com.htc.upm.wave.openxr");
                EditorApplication.update += Progress;
            }

            if (GUILayout.Button("Setup Focus 3 OpenXR"))
            {
                // https://developer.vive.com/resources/openxr/openxr-mobile/tutorials/unity/getting-started-openxr-mobile/

                // switch platform Android

                // landscale left
                PlayerSettings.allowedAutorotateToLandscapeLeft = true;
                PlayerSettings.allowedAutorotateToLandscapeRight = false;
                PlayerSettings.allowedAutorotateToPortrait = false;
                PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
                PlayerSettings.defaultInterfaceOrientation = UIOrientation.LandscapeLeft;

                // OpenGLES 3
                PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.Android, false);
                PlayerSettings.SetGraphicsAPIs(BuildTarget.Android, new GraphicsDeviceType[] { GraphicsDeviceType.OpenGLES3 });
            }

            EditorGUILayout.Separator();

            if (GUILayout.Button("Add All-in-one URP asset"))
            {
                // Add new URP assets and renderer
                
                // URP Asset
                var asset = ScriptableObject.CreateInstance<UniversalRenderPipelineAsset>();
                AssetDatabase.CreateAsset(asset, "Assets/Settings/URP-XR-AllInOne.asset");
                

                // Renderer data
                var rendererData = ScriptableRendererData.CreateInstance<UniversalRendererData>();
                AssetDatabase.CreateAsset(rendererData, "Assets/Settings/URP-XR-AllInOne-Renderer.asset");

                // Serialize to set value
                SerializedObject serializedAsset = new SerializedObject(asset);
                SerializedProperty rendererDataList = serializedAsset.FindProperty("m_RendererDataList");
                rendererDataList.GetArrayElementAtIndex(0).objectReferenceValue = rendererData;
                // m_DefaultRendererIndex: 0
                // m_SupportsHDR: 0
                // m_MainLightShadowsSupported: 1
                // m_MainLightRenderingMode: 1
                // m_AdditionalLightShadowsSupported: 0
                // m_AdditionalLightsRenderingMode: 0
                // m_ColorGradingLutSize: 16
                // m_MSAA: 4
                serializedAsset.FindProperty("m_DefaultRendererIndex").intValue = 0;
                serializedAsset.FindProperty("m_SupportsHDR").intValue = 0;
                serializedAsset.FindProperty("m_MainLightShadowsSupported").intValue = 1;
                serializedAsset.FindProperty("m_MainLightRenderingMode").intValue = 1;
                serializedAsset.FindProperty("m_AdditionalLightShadowsSupported").intValue = 0;
                serializedAsset.FindProperty("m_AdditionalLightsRenderingMode").intValue = 0;
                serializedAsset.FindProperty("m_ColorGradingLutSize").intValue = 16;
                serializedAsset.FindProperty("m_MSAA").intValue = 4;

                // Set renderer data
                SerializedObject serializedRendererData = new SerializedObject(rendererData);
                // m_DepthPrimingMode: 1
                //  postProcessData: {fileID: 11400000, guid: 41439944d30ece34e96484bdb6645b55, type: 2}
                // m_IntermediateTextureMode: 0
                serializedRendererData.FindProperty("m_DepthPrimingMode").intValue = 1;
                serializedRendererData.FindProperty("postProcessData").objectReferenceValue = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath("41439944d30ece34e96484bdb6645b55"), typeof(PostProcessData));
                serializedRendererData.FindProperty("m_IntermediateTextureMode").intValue = 0;
                
                serializedRendererData.ApplyModifiedProperties();
                serializedAsset.ApplyModifiedProperties();

                AssetDatabase.SaveAssets();
            }

            if (GUILayout.Button("Add All-in-one Quality Settings"))
            {
                // Get current quality settings
                SerializedObject qualitySettings = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/QualitySettings.asset")[0]);
                SerializedProperty qualityProperty = qualitySettings.FindProperty("m_QualitySettings");

                // Create new quality level
                int index = qualityProperty.arraySize;
                qualityProperty.InsertArrayElementAtIndex(index);
                SerializedProperty newQualityLevel = qualityProperty.GetArrayElementAtIndex(index);

                /**
                    name: All-in-One XR
                    pixelLightCount: 4
                    shadows: 2
                    shadowResolution: 2
                    shadowProjection: 1
                    shadowCascades: 4
                    shadowDistance: 150
                    shadowNearPlaneOffset: 3
                    shadowCascade2Split: 0.33333334
                    shadowCascade4Split: {x: 0.06666667, y: 0.2, z: 0.46666667}
                    shadowmaskMode: 1
                    skinWeights: 4
                    textureQuality: 0
                    anisotropicTextures: 1
                    antiAliasing: 4
                    softParticles: 1
                    softVegetation: 1
                    realtimeReflectionProbes: 1
                    billboardsFaceCameraPosition: 1
                    vSyncCount: 0
                    lodBias: 2
                    maximumLODLevel: 0
                    streamingMipmapsActive: 0
                    streamingMipmapsAddAllCameras: 1
                    streamingMipmapsMemoryBudget: 512
                    streamingMipmapsRenderersPerFrame: 512
                    streamingMipmapsMaxLevelReduction: 2
                    streamingMipmapsMaxFileIORequests: 1024
                    particleRaycastBudget: 4096
                    asyncUploadTimeSlice: 2
                    asyncUploadBufferSize: 16
                    asyncUploadPersistentBuffer: 1
                    resolutionScalingFixedDPIFactor: 1
                **/
                newQualityLevel.FindPropertyRelative("name").stringValue = "All-in-One XR";
                newQualityLevel.FindPropertyRelative("pixelLightCount").intValue = 4;
                newQualityLevel.FindPropertyRelative("shadows").intValue = 2;
                newQualityLevel.FindPropertyRelative("shadowResolution").intValue = 2;
                newQualityLevel.FindPropertyRelative("shadowProjection").intValue = 1;
                newQualityLevel.FindPropertyRelative("shadowCascades").intValue = 4;
                newQualityLevel.FindPropertyRelative("shadowDistance").floatValue = 150f;
                newQualityLevel.FindPropertyRelative("shadowNearPlaneOffset").floatValue = 3f;
                newQualityLevel.FindPropertyRelative("shadowCascade2Split").floatValue = 0.333333343f;
                newQualityLevel.FindPropertyRelative("shadowCascade4Split").vector3Value = new Vector3(0.06666667f, 0.2f, 0.466666669f);
                newQualityLevel.FindPropertyRelative("shadowmaskMode").intValue = 1;
                newQualityLevel.FindPropertyRelative("skinWeights").intValue = 4;
                newQualityLevel.FindPropertyRelative("textureQuality").intValue = 0;
                newQualityLevel.FindPropertyRelative("anisotropicTextures").intValue = 1;
                newQualityLevel.FindPropertyRelative("antiAliasing").intValue = 4;
                newQualityLevel.FindPropertyRelative("softParticles").boolValue = true;
                newQualityLevel.FindPropertyRelative("softVegetation").boolValue = true;
                newQualityLevel.FindPropertyRelative("realtimeReflectionProbes").boolValue = true;
                newQualityLevel.FindPropertyRelative("billboardsFaceCameraPosition").boolValue = true;
                newQualityLevel.FindPropertyRelative("vSyncCount").intValue = 0;
                newQualityLevel.FindPropertyRelative("lodBias").floatValue = 2f;
                newQualityLevel.FindPropertyRelative("maximumLODLevel").intValue = 0;
                newQualityLevel.FindPropertyRelative("streamingMipmapsActive").boolValue = false;
                newQualityLevel.FindPropertyRelative("streamingMipmapsAddAllCameras").boolValue = true;
                newQualityLevel.FindPropertyRelative("streamingMipmapsMemoryBudget").floatValue = 512f;
                newQualityLevel.FindPropertyRelative("streamingMipmapsRenderersPerFrame").intValue = 512;
                newQualityLevel.FindPropertyRelative("streamingMipmapsMaxLevelReduction").intValue = 2;
                newQualityLevel.FindPropertyRelative("streamingMipmapsMaxFileIORequests").intValue = 1024;
                newQualityLevel.FindPropertyRelative("particleRaycastBudget").intValue = 4096;
                newQualityLevel.FindPropertyRelative("asyncUploadTimeSlice").intValue = 2;
                newQualityLevel.FindPropertyRelative("asyncUploadBufferSize").intValue = 16;
                newQualityLevel.FindPropertyRelative("asyncUploadPersistentBuffer").boolValue = true;
                newQualityLevel.FindPropertyRelative("resolutionScalingFixedDPIFactor").floatValue = 1f;

                var asset = AssetDatabase.LoadAssetAtPath<UniversalRenderPipelineAsset>("Assets/Settings/URP-XR-AllInOne.asset");
                if (asset)
                {
                    newQualityLevel.FindPropertyRelative("customRenderPipeline").objectReferenceValue = asset;
                }
                
                // Set all platforms to use new quality level
                var perPlatformProp = qualitySettings.FindProperty("m_PerPlatformDefaultQuality");
                for (int i = 0; i < perPlatformProp.arraySize; i++)
                {
                    var platform = perPlatformProp.GetArrayElementAtIndex(i);
                    platform.FindPropertyRelative("second").intValue = index;
                }
                
                // Apply changes
                qualitySettings.ApplyModifiedProperties();

                QualitySettings.SetQualityLevel(index, true);

                Debug.Log("New quality setting added and set as default for all platforms.");
            }

            if (GUILayout.Button("Check Texture compression format (PlayerSettings)"))
            {
                EditorUserBuildSettings.overrideTextureCompression = UnityEditor.Build.OverrideTextureCompression.NoOverride;
                
            }

            EditorGUILayout.Separator();

            if (GUILayout.Button("Switch to XRDevice.Focus"))
            {
                BuildPipelineImplementXR.SetXRPlatform(XRDevice.ViveFocus);
            }

            if (GUILayout.Button("Setup to XRDevice.MetaQuest"))
            {
                BuildPipelineImplementXR.SetXRPlatform(XRDevice.MetaQuest);
            }
        }

        public static void AddScopedRegistry(ScopedRegistry pScopeRegistry)
        {
            var manifestPath = Path.Combine(Application.dataPath, "..", "Packages/manifest.json");
            var manifestJson = File.ReadAllText(manifestPath);

            var manifest = JsonConvert.DeserializeObject<ManifestJson>(manifestJson);

            bool doAdd = true;
            foreach(var sr in manifest.scopedRegistries)
            {
                if (sr.name.Equals(pScopeRegistry.name))
                {
                    Debug.LogError($"Duplicate scope registry name {sr.name}");
                    doAdd = false;
                }
            }

            if (doAdd)
            {
                manifest.scopedRegistries.Add(pScopeRegistry);
                File.WriteAllText(manifestPath, JsonConvert.SerializeObject(manifest, Formatting.Indented));
            }
        }


        public class ScopedRegistry
        {
            public string name;
            public string url;
            public string[] scopes;
        }

        public class ManifestJson
        {
            public Dictionary<string, string> dependencies = new Dictionary<string, string>();

            public List<ScopedRegistry> scopedRegistries = new List<ScopedRegistry>();
        }
    }

}