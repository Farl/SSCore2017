using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Rendering;
#if USE_URP
using UnityEngine.Rendering.Universal;
#endif

using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;

using Newtonsoft.Json;

using System.IO;
using System.Reflection;
using System.Text;
using System;

namespace SS
{

    public class XRSetup : EditorWindow
    {
        #region Add package
        private static AddRequest addRequest;
        private static RemoveRequest removeRequest;

        public void AddPackageRequest(string packageId)
        {
            if (addRequest != null && !addRequest.IsCompleted)
            {
                Debug.Log("Add request is already in progress.");
                return;
            }
            else
            {
                Debug.Log("Add package: " + packageId);
            }
            addRequest = Client.Add(packageId);
            EditorApplication.update += AddProgress;
        }

        public void RemovePackageRequest(string packageId)
        {
            if (removeRequest != null && !removeRequest.IsCompleted)
            {
                Debug.Log("Remove request is already in progress.");
                return;
            }
            else
            {
                Debug.Log("Remove package: " + packageId);
            }
            removeRequest = Client.Remove(packageId);
            EditorApplication.update += RemoveProgress;
        }

        private static void AddProgress()
        {
            if (addRequest.IsCompleted)
            {
                if (addRequest.Status == StatusCode.Success)
                    Debug.Log("Installed: " + addRequest.Result.packageId);
                else if (addRequest.Status >= StatusCode.Failure)
                    Debug.Log(addRequest.Error.message);

                EditorApplication.update -= AddProgress;
                addRequest = null;
            }
        }

        private static void RemoveProgress()
        {
            if (removeRequest.IsCompleted)
            {
                if (removeRequest.Status == StatusCode.Success)
                    Debug.Log("Removed: " + removeRequest.PackageIdOrName);
                else if (removeRequest.Status >= StatusCode.Failure)
                    Debug.Log(removeRequest.Error.message);

                EditorApplication.update -= RemoveProgress;
                removeRequest = null;
            }
        }
        #endregion

        #region Variables
        private static string viveVersion = "";
        private static XRDevice targetXRDevice = XRDevice.MetaQuest;
        private Vector2 scrollPos;
        #endregion

        [MenuItem("Tools/SS/XR Setup")]
        private static void Open()
        {
            var w = XRSetup.GetWindow<XRSetup>();
            w.titleContent = new GUIContent("XR Setup");
        }


        private static void DrawButton(string label, System.Func<bool> checkFunc = null, System.Action action = null, System.Action cancelAction = null)
        {
            EditorGUILayout.BeginHorizontal();
            if (checkFunc != null && checkFunc.Invoke())
            {
                GUI.color = Color.green;
                GUILayout.Label("✓", GUILayout.Width(20));
                GUI.color = Color.white;
            }
            if (GUILayout.Button(label))
            {
                action?.Invoke();
            }
            if (cancelAction != null && GUILayout.Button("x", GUILayout.Width(20)))
            {
                cancelAction.Invoke();
            }
            EditorGUILayout.EndHorizontal();
        }

        private void OnGUI()
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            {

                DrawButton("Import OpenXR Plugin",
                    checkFunc: () =>
                    {
#if USE_OPENXR
                    return true;
#else
                        return false;
#endif
                    },
                    action: () =>
                    {
                        AddPackageRequest("com.unity.xr.openxr");
                    },
                    cancelAction: () =>
                    {
                        RemovePackageRequest("com.unity.xr.openxr");
                    }
                );

                DrawButton("Import XR Interaction Toolkits",
                    checkFunc: () =>
                    {
#if USE_XRI
                    return true;
#else
                        return false;
#endif
                    },
                    action: () =>
                    {
                        AddPackageRequest("com.unity.xr.interaction.toolkit");
                    },
                    cancelAction: () =>
                    {
                        RemovePackageRequest("com.unity.xr.interaction.toolkit");
                    }
                );

                EditorGUILayout.Separator();

                if (EditorGUILayout.Foldout(true, new GUIContent("Meta")))
                {
                    DrawButton("Import Meta (OpenXR)",
                        checkFunc: () =>
                        {
#if USE_META_OPENXR
                        return true;
#else
                            return false;
#endif
                        },
                        action: () =>
                        {
                            AddPackageRequest("com.unity.xr.meta-openxr");
                        },
                        cancelAction: () =>
                        {
                            RemovePackageRequest("com.unity.xr.meta-openxr");
                        }
                    );
                }

                if (EditorGUILayout.Foldout(true, new GUIContent("Vive")))
                {
                    DrawButton("Import Vive (2-in-one)",
                        checkFunc: () =>
                        {
#if USE_VIVE_OPENXR
                        return true;
#else
                            return false;
#endif
                        },
                        action: () =>
                        {
                            if (string.IsNullOrEmpty(viveVersion))
                            {
                                AddPackageRequest("https://github.com/ViveSoftware/VIVE-OpenXR.git?path=com.htc.upm.vive.openxr");
                            }
                            else
                            {
                                AddPackageRequest($"https://github.com/ViveSoftware/VIVE-OpenXR.git?path=com.htc.upm.vive.openxr#versions/{viveVersion}");
                            }
                        },
                        cancelAction: () =>
                        {
                            RemovePackageRequest("com.htc.upm.vive.openxr");
                        }
                    );
                    viveVersion = EditorGUILayout.TextField(new GUIContent("Package version"), viveVersion);

                    DrawButton("[Legacy] Import Vive (Wave OpenXR)",
                        action: () =>
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
                            AddPackageRequest("com.htc.upm.wave.openxr");
                        }
                    );

                    if (GUILayout.Button("Setup Vive Focus 3 OpenXR"))
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
                }

                EditorGUILayout.Separator();

                if (GUILayout.Button("Add All-in-one URP asset"))
                {
                    AddURPAssetAndRenderer();
                }

                if (GUILayout.Button("Add All-in-one Quality Settings"))
                {
                    AddQualitySetting();
                }

                // Override texture compression format
                EditorUserBuildSettings.overrideTextureCompression = (UnityEditor.Build.OverrideTextureCompression)EditorGUILayout.EnumPopup(new GUIContent("Texture compression format override"), EditorUserBuildSettings.overrideTextureCompression);

                EditorGUILayout.Separator();

                // Run in background toggle
                {
                    PlayerSettings.runInBackground = EditorGUILayout.Toggle("Run in background", PlayerSettings.runInBackground);
                }

                EditorGUILayout.Separator();

                DrawXRBuildSettings();
            }
            EditorGUILayout.EndScrollView();
        }

        private static void DrawXRBuildSettings()
        {
            EditorGUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("XR Build Settings"))
                {
                    Selection.activeObject = XRBuildSettings.GetXRBuildSettings();
                }
                if (GUILayout.Button("XR Management"))
                {
                    SettingsService.OpenProjectSettings("Project/XR Plug-in Management");
                }
            }
            EditorGUILayout.EndHorizontal();
            // Target XR device
            var deviceEnums = typeof(XRDevice).GetEnumValues();
            foreach (var e in deviceEnums)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(e.ToString(), GUILayout.Width(80));
                if (GUILayout.Button("Switch"))
                {
                    BuildPipelineImplementXR.SetXRPlatform((XRDevice)e);
                }
                if (GUILayout.Button("Save"))
                {
                    XRBuildSettings.SaveDeviceSettings((XRDevice)e);
                }
                if (GUILayout.Button("Load"))
                {
                    XRBuildSettings.LoadDeviceSettings((XRDevice)e);
                }
                EditorGUILayout.EndHorizontal();
            }
        }

        private static void SetValue(SerializedProperty property, string key, object value)
        {
            var prop = property.FindPropertyRelative(key);
            if (prop != null)
            {
                if (value is int)
                {
                    prop.intValue = (int)value;
                }
                else if (value is float)
                {
                    prop.floatValue = (float)value;
                }
                else if (value is bool)
                {
                    prop.boolValue = (bool)value;
                }
                else if (value is Vector3)
                {
                    prop.vector3Value = (Vector3)value;
                }
                else if (value is string || value is System.String)
                {
                    prop.stringValue = (string)value;
                }
                else if (value is UnityEngine.Object)
                {
                    prop.objectReferenceValue = (UnityEngine.Object)value;
                }
                else
                {
                    Debug.LogWarning($"Unsupported type {value.GetType()}");
                }
            }
            else
            {
                Debug.LogWarning($"Property {key} not found.");
            }
        }

        private void AddURPAssetAndRenderer()
        {
#if USE_URP
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
#endif
        }

        private static void AddQualitySetting()
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
            SetValue(newQualityLevel, "name", "All-in-One XR");
            SetValue(newQualityLevel, "pixelLightCount", 4);
            SetValue(newQualityLevel, "shadows", 2);
            SetValue(newQualityLevel, "shadowResolution", 2);
            SetValue(newQualityLevel, "shadowProjection", 1);
            SetValue(newQualityLevel, "shadowCascades", 4);
            SetValue(newQualityLevel, "shadowDistance", 150f);
            SetValue(newQualityLevel, "shadowNearPlaneOffset", 3f);
            SetValue(newQualityLevel, "shadowCascade2Split", 0.333333343f);
            SetValue(newQualityLevel, "shadowCascade4Split", new Vector3(0.06666667f, 0.2f, 0.466666669f));
            SetValue(newQualityLevel, "shadowmaskMode", 1);
            SetValue(newQualityLevel, "skinWeights", 4);
            SetValue(newQualityLevel, "textureQuality", 0);
            SetValue(newQualityLevel, "anisotropicTextures", 1);
            SetValue(newQualityLevel, "antiAliasing", 4);
            SetValue(newQualityLevel, "softParticles", true);
            SetValue(newQualityLevel, "softVegetation", true);
            SetValue(newQualityLevel, "realtimeReflectionProbes", true);
            SetValue(newQualityLevel, "billboardsFaceCameraPosition", true);
            SetValue(newQualityLevel, "vSyncCount", 0);
            SetValue(newQualityLevel, "lodBias", 2f);
            SetValue(newQualityLevel, "maximumLODLevel", 0);
            SetValue(newQualityLevel, "streamingMipmapsActive", false);
            SetValue(newQualityLevel, "streamingMipmapsAddAllCameras", true);
            SetValue(newQualityLevel, "streamingMipmapsMemoryBudget", 512f);
            SetValue(newQualityLevel, "streamingMipmapsRenderersPerFrame", 512);
            SetValue(newQualityLevel, "streamingMipmapsMaxLevelReduction", 2);
            SetValue(newQualityLevel, "streamingMipmapsMaxFileIORequests", 1024);
            SetValue(newQualityLevel, "particleRaycastBudget", 4096);
            SetValue(newQualityLevel, "asyncUploadTimeSlice", 2);
            SetValue(newQualityLevel, "asyncUploadBufferSize", 16);
            SetValue(newQualityLevel, "asyncUploadPersistentBuffer", true);
            SetValue(newQualityLevel, "resolutionScalingFixedDPIFactor", 1f);

#if USE_URP
            var asset = AssetDatabase.LoadAssetAtPath<UniversalRenderPipelineAsset>("Assets/Settings/URP-XR-AllInOne.asset");
            if (asset)
            {
                SetValue(newQualityLevel, "customRenderPipeline", asset);
            }
#endif

            // Set all platforms to use new quality level
            var perPlatformProp = qualitySettings.FindProperty("m_PerPlatformDefaultQuality");
            for (int i = 0; i < perPlatformProp.arraySize; i++)
            {
                var platform = perPlatformProp.GetArrayElementAtIndex(i);
                SetValue(platform, "second", index);
            }

            // Apply changes
            qualitySettings.ApplyModifiedProperties();

            QualitySettings.SetQualityLevel(index, true);

            Debug.Log("New quality setting added and set as default for all platforms.");
        }

        public static void AddScopedRegistry(ScopedRegistry pScopeRegistry)
        {
            var manifestPath = Path.Combine(Application.dataPath, "..", "Packages/manifest.json");
            var manifestJson = File.ReadAllText(manifestPath);

            var manifest = JsonConvert.DeserializeObject<ManifestJson>(manifestJson);

            bool doAdd = true;
            foreach (var sr in manifest.scopedRegistries)
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