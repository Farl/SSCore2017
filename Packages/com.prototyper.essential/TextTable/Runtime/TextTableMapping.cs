using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.Text;

namespace SS
{
    public class TextTableMapping : EntityBase, ITextTableMapping
    {

        #region OSFont

        private static bool isInitOSFont = false;
        private static Font _osDefaultFont = null;
        private static TMP_FontAsset _osDefaultFontAsset = null;
        private static HashSet<TMP_FontAsset> _initedFontAssets = new HashSet<TMP_FontAsset>();

        private static void InitOSFont()
        {
            if (!isInitOSFont)
            {
                GetOSDefaultFontAsset();
            }
            isInitOSFont = true;
        }

        private static string GetOSDefaultFontName()
        {
#if UNITY_EDITOR_OSX
            return "Arial Unicode";
#elif UNITY_EDITOR
            return "MSJH.ttc";
#elif UNITY_ANDROID
            return "NotoSansCJK";
#elif UNITY_IOS
            return "PingFang";
#else
            return null;
#endif
        }
        private static Font GetOSDefaultFont()
        {
            if (_osDefaultFont != null)
                return _osDefaultFont;

            // (TEST) Get installed font name
            string[] fontNames = Font.GetOSInstalledFontNames();
            var fontStr = new StringBuilder($"Fonts Name ({fontNames.Length}):\n");
            foreach (var fn in fontNames)
            {
                fontStr.AppendLine(fn);
            }
            Debug.Log(fontStr.ToString());
            fontStr.Clear();

            // Get the file path of all OS fonts.
            string[] fontPaths = Font.GetPathsToOSFonts();
            fontStr = new StringBuilder($"Fonts Paths ({fontPaths.Length}):");
            foreach (var fp in fontPaths)
            {
                fontStr.AppendLine(fp);
            }
            Debug.Log(fontStr.ToString());

            Debug.Log($"Current OS default font name is {GetOSDefaultFontName()}");

            List<string> fontPathList = new List<string>(fontPaths);
            var fontPath = fontPathList.Find((x) =>
            {
                // TODO: for each OS
                return x.IndexOf(GetOSDefaultFontName(), System.StringComparison.InvariantCultureIgnoreCase) >= 0;
            });
            Debug.Log($"Current OS default font path is {fontPath}");
            _osDefaultFont = new Font(fontPath);
            return _osDefaultFont;
        }
        private static TMP_FontAsset GetOSDefaultFontAsset()
        {
            if (_osDefaultFontAsset != null)
                return _osDefaultFontAsset;

            var osFont = GetOSDefaultFont();
            if (osFont != null)
            {
                // Create new font asset using this OS font.
                _osDefaultFontAsset = TMP_FontAsset.CreateFontAsset(osFont);
                _osDefaultFontAsset.name = osFont.name;
            }
            return _osDefaultFontAsset;
        }
        #endregion

        #region Variable text manager
        private static Dictionary<string, List<TextTableMapping>> _mappings = new Dictionary<string, List<TextTableMapping>>();
        private static Dictionary<string, Func<string>> _variableGetters = new Dictionary<string, Func<string>>();
        public static void SetVariableText(string variableID, string text)
        {
            if (string.IsNullOrEmpty(variableID))
                return;
            // Get TextTableMapping list
            if (_mappings.TryGetValue(variableID, out var list))
            {
                if (list != null)
                {
                    foreach (var textMapping in list)
                    {
                        textMapping.SetText(text);
                    }
                }
            }
        }

        public static void RegisterVariableGetter(string variableID, Func<string> func)
        {
            if (func == null || string.IsNullOrEmpty(variableID))
                return;
            if (!_variableGetters.TryAdd(variableID, func))
            {
                Debug.LogError($"Duplicate variable getter {variableID}");
            }
        }

        public static void UnregisterVariableGetter(string variableID)
        {
            if (string.IsNullOrEmpty(variableID))
                return;
            _variableGetters.Remove(variableID);
        }

        public static bool TryGetVariableText(string variableID, out string text)
        {
            text = null;
            if (string.IsNullOrEmpty(variableID))
                return false;
            if (_variableGetters.TryGetValue(variableID, out var func))
            {
                text = func?.Invoke();
                return true;
            }
            return false;
        }

        private static void RegisterVariable(TextTableMapping textMapping)
        {
            if (textMapping == null || string.IsNullOrEmpty(textMapping.variableID))
                return;
            if (!_mappings.ContainsKey(textMapping.variableID))
            {
                _mappings.Add(textMapping.variableID, new List<TextTableMapping>());
            }
            var list = _mappings[textMapping.variableID];
            list.Add(textMapping);
        }

        private static void UnregisterVariable(TextTableMapping textMapping)
        {
            if (textMapping == null || string.IsNullOrEmpty(textMapping.variableID))
                return;
            if (_mappings.ContainsKey(textMapping.variableID))
            {
                var list = _mappings[textMapping.variableID];
                if (list != null)
                {
                    list.Remove(textMapping);
                }
                if (list.Count <= 0)
                {
                    _mappings.Remove(textMapping.variableID);
                }
            }
        }
        #endregion

        [SerializeField]
        private TMP_Text textComp;
        public TMP_Text TextComponent
        {
            get {
                if (!textComp)
                    textComp = GetComponent<TMP_Text>();
                return textComp;
            }

        }

        [SerializeField]
        private string textID;

        [SerializeField]
        private string variableID;

        //[SerializeField]
        //private bool updateWhenOverride = false;

        private bool isInit;

        private string origFontName;

        #region Override

        public override void OnRunnerStart()
        {
            base.OnRunnerStart();
        }

        private void Intialize()
        {
            if (isInit)
                return;

            InitOSFont();

            if (TextComponent != null && TextComponent.font != null)
            {
                // Backup original font asset name
                origFontName = TextComponent.font.name;

                // Add fallback font by using OS fonts
                if (_osDefaultFontAsset && !_initedFontAssets.Contains(TextComponent.font))
                {
                    TextComponent.font.fallbackFontAssetTable.Add(_osDefaultFontAsset);
                    _initedFontAssets.Add(TextComponent.font);
                }
            }
            isInit = true;
        }

        public override void OnRestart()
        {
            base.OnRestart();

            UpdateTextFromTextID();
        }

        private void UpdateTextFromTextID()
        {
            if (string.IsNullOrEmpty(textID))
            {
                return;
            }

            if (TextComponent)
            {
                Action<string, string> callback = null;
                /*
                if (updateWhenOverride)
                {
                    callback = (k, v) =>
                    {
                        //Debug.Log($"{k}={v}");
                        ManualUpdateText();
                    };
                    updateWhenOverride = false;
                }
                */

                if (TextTable.TryGetTextWithNotify(textID, out var text, callback))
                {
                    TextComponent.text = text;
                }
                else {
                    TextComponent.text = $"{textID}";
                }
            }
        }

        protected override void OnEntityAwake()
        {
            base.OnEntityAwake();
            RegisterVariable(this);
            TextTable.Register(this);
        }

        protected override void OnEntityDestroy()
        {
            base.OnEntityDestroy();
            UnregisterVariable(this);
            TextTable.Unregister(this);
        }

        private void SetupFontAsset()
        {
            if (TextComponent == null)
                return;
            if (string.IsNullOrEmpty(origFontName))
                return;

            if (TextTable.TryGetFontAsset(origFontName, out var fontAsset))
            {
                if (fontAsset != null)
                {
                    TextComponent.font = fontAsset;
                }
            }
        }

        private void OnEnable()
        {
            Intialize();
            ManualUpdateText();
        }

        private void OnDisable()
        {
            
        }

        #endregion

        public void OnLanguageChanged()
        {
            ManualUpdateText();
        }

        public void SetText(string text)
        {
            if (TextComponent)
            {
                TextComponent.text = text;
            }
        }

        public void SetText(float single)
        {
            SetText(single.ToString());
        }

        public void SetTextID(string textID)
        {
            if (textID != this.textID)
            {
                this.textID = textID;
            }
            else
            {
            }
            UpdateTextFromTextID();
        }

        public void ManualUpdateText()
        {
            if (TryGetVariableText(variableID, out var text))
            {
                SetText(text);
            }
            else
            {
                UpdateTextFromTextID();
            }

            SetupFontAsset();
        }

        internal void SetColor(Color color)
        {
            if (TextComponent)
            {
                TextComponent.color = color;
            }
        }
    }
}
