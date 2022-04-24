using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace SS
{
    public class TextTableMapping : MonoBehaviour, ITextTableLanguage
    {
        [SerializeField]
        private string textID;

        [SerializeField]
        private Text textComp;

        [SerializeField]
        private TMP_Text textCompPro;

        [SerializeField]
        private bool enableOSFallback = true;

        public void OnTextTableLanguageChanged(SystemLanguage language, SystemLanguage origLanguage)
        {
            SetupText();
        }

        private static string GetOSDefaultFontName()
        {
#if UNITY_EDITOR
            return "MSJH.ttc";
#elif UNITY_ANDROID
            return "NotoSansCJK";
#elif UNITY_IOS
            return "SF Pro";
#else
            return null;
#endif
        }

        private static Font _osDefaultFont = null;
        private static Font GetOSDefaultFont()
        {
            if (_osDefaultFont != null)
                return _osDefaultFont;

            // Get the file path of all OS fonts.
            string[] fontPaths = Font.GetPathsToOSFonts();
            List<string> fontNames = new List<string>();
            foreach (var fp in fontPaths)
            {
                fontNames.Add(fp);
                Debug.Log(fp);
            }
            var fontPath = fontNames.Find((x) =>
            {
                // TODO: for each OS
                return x.IndexOf(GetOSDefaultFontName(), System.StringComparison.InvariantCultureIgnoreCase) >= 0;
            });
            _osDefaultFont = new Font(fontPath);
            return _osDefaultFont;
        }

        private static TMP_FontAsset _osDefaultFontAsset = null;
        private static TMP_FontAsset GetOSDefaultFontAsset()
        {
            if (_osDefaultFontAsset != null )
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

        private void CheckCompoenent()
        {
            if (textCompPro != null)
            {
                GetOSDefaultFontAsset();
                if (_osDefaultFontAsset != null &&
                    !textCompPro.font.fallbackFontAssetTable.Contains(_osDefaultFontAsset))
                {
                    Debug.LogError($"Font {_osDefaultFontAsset.name}");
                    textCompPro.font.fallbackFontAssetTable.Add(_osDefaultFontAsset);
                }
            }

            // Check component
            if (textCompPro == null)
            {
                textCompPro = GetComponent<TMP_Text>();
            }
            if (textCompPro == null && textComp == null)
            {
                textComp = GetComponent<Text>();
            }
        }

        private void SetupText()
        {
            CheckCompoenent();

            if (string.IsNullOrEmpty(textID))
                return;

            string result = null;
            if (!TextTable.TryGetText(textID, out result))
            {
                result = $"[{textID}]";
            }

            if (textCompPro != null)
            {
                textCompPro.text = result;
            }
            else if (textComp != null)
            {
                textComp.text = result;
            }
        }

        // Start is called before the first frame update
        void OnEnable()
        {
            // Register to TextTable
            TextTable.Register(this);
            SetupText();
        }
    }
}
