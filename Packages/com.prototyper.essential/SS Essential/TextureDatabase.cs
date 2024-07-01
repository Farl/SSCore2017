// Store texture in local storage
// by Farl
// Ref: https://answers.unity.com/questions/1718871/saving-and-loading-tofrom-android.html
// Ref: https://answers.unity.com/questions/1569275/how-to-save-textures-to-ios-devices-ask-for-help.html

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using PlayerPrefs = SS.PlayerPrefs;

namespace SS
{
    public class TextureDatabase
    {
        #region Public
        public static void ClearTextureInfo()
        {
            // Remove all
            var textureDatabaseStr = PlayerPrefs.GetString($"{keyPrefix}", "");
            var tokens = textureDatabaseStr.Split(',', System.StringSplitOptions.RemoveEmptyEntries);
            textureDatabaseStr = "";
            foreach (var hash in tokens)
            {
                RemovePrefs(hash);
                RemoveTextureFileFromHash(hash);
            }
            PlayerPrefs.SetString($"{keyPrefix}", "");
        }

        public static void RemoveTexture(string path)
        {
            if (string.IsNullOrEmpty(path))
                return;
            var hash = path.GetHashCode();
            RemoveTextureInfo(hash);
            RemoveTextureFileFromHash(hash.ToString());
        }

        public static Texture2D GetTexture(string path, bool useMipmap = true)
        {
            if (string.IsNullOrEmpty(path))
                return null;

            var hash = path.GetHashCode();

            Texture2D tex = null;

            // First try get from runtime database
            if (runtimeDatabase.TryGetValue(hash, out tex))
            {
            }

            if (tex == null)
            {
                // Get from file cache
                if (!GetTextureInfo(hash, out var originalPath, out var width, out var height, out var textureFormatValue))
                    return null;

                var localPath = Path.Combine(Application.persistentDataPath, $"{hash}{extension}");
                byte[] bytes = null;
                try
                {
                    bytes = File.ReadAllBytes(localPath);
                }
                catch (System.Exception e)
                {
                    Debug.LogException(e);
                }

                if (bytes == null)
                    return null;

                tex = new Texture2D(width, height, (TextureFormat)textureFormatValue, useMipmap);
                tex.LoadImage(bytes);
                tex.name = path;
                if (useMipmap)
                {
                    tex.Apply(true, true);
                }
            }

            return tex;
        }

        public static void SetTexture(string path, Texture2D texture, bool fileCache = true)
        {
            if (texture == null || string.IsNullOrEmpty(path))
                return;
            var hash = path.GetHashCode();

            // File cache
            if (fileCache && texture.isReadable)
            {
                SetTextureInfo(hash, path, texture);

                var bytes = texture.EncodeToPNG();
                var localPath = Path.Combine(Application.persistentDataPath, $"{hash}{extension}");
                try
                {
                    File.WriteAllBytes(localPath, bytes);
                }
                catch (System.Exception e)
                {
                    Debug.LogException(e);
                }
            }

            // Runtime cache
            {
                if (runtimeDatabase.ContainsKey(hash))
                {
                    runtimeDatabase[hash] = texture;
                }
                else
                {
                    runtimeDatabase.Add(hash, texture);
                    // Use too many textures in memory
                    if (runtimeDatabase.Count > runtimeDBSize)
                    {
                        removeList.Clear();
                        foreach (var kvp in runtimeDatabase)
                        {
                            removeList.Add(kvp.Key);
                            // Remove half of limit
                            if (removeList.Count > runtimeDBSize / 2)
                                break;
                        }
                        foreach (var k in removeList)
                        {
                            runtimeDatabase.Remove(k);
                        }
                    }
                }
            }
        }

        public static Sprite CreateSprite(Texture texture, bool useMipMap = true)
        {
            if (texture == null)
                return null;

            Texture2D tex2D = texture as Texture2D;

            if (tex2D == null)
                return null;

            Texture2D targetTex2D = tex2D;

            if (useMipMap && tex2D.mipmapCount <= 1)    // mipmapCount include base level
            {
                // Generate mip map
                CopyTexture(tex2D, useMipMap: useMipMap, makeNoLongerReadable: true);
            }

            var sprite = Sprite.Create(targetTex2D, new Rect(Vector2.zero, new Vector2(targetTex2D.width, targetTex2D.height)), new Vector2(0.5f, 0.5f));
            sprite.name = texture.name;
            return sprite;
        }

        public static Texture2D CopyTexture(Texture2D texture, bool useMipMap, bool makeNoLongerReadable)
        {
            if (texture == null) return null;
            Texture2D targetTex2D = texture;

            // Generate mip map
            // Ref: https://forum.unity.com/threads/generate-mipmaps-at-runtime-for-a-texture-loaded-with-unitywebrequest.644842/#post-7571809
            targetTex2D = new Texture2D(texture.width, texture.height, texture.format, mipChain: useMipMap);
            targetTex2D.SetPixelData(texture.GetRawTextureData<byte>(), 0);
            targetTex2D.Apply(useMipMap, makeNoLongerReadable);

            return targetTex2D;
        }

        /// <summary>
        /// Reference from: http://jon-martin.com/?p=114
        /// </summary>
        /// <param name="source"></param>
        /// <param name="targetWidth"></param>
        /// <param name="targetHeight"></param>
        /// <param name="useMipMap"></param>
        /// <param name="makeNoLongerReadable"></param>
        /// <returns></returns>
        public static Texture2D ScaleTexture(Texture2D source, int targetWidth, int targetHeight, bool useMipMap, bool makeNoLongerReadable)
        {
            if (source == null)
                return null;
            if (targetWidth <= 0 && targetHeight <= 0)
                return null;
            if (targetWidth <= 0)
                targetWidth = targetHeight * source.width / source.height;
            else if (targetHeight <= 0)
                targetHeight = targetWidth * source.height / source.width;

            Texture2D result = new Texture2D(targetWidth, targetHeight, source.format, useMipMap);
            Color[] rpixels = result.GetPixels(0);
            float incX = (1.0f / (float)targetWidth);
            float incY = (1.0f / (float)targetHeight);
            for (int px = 0; px < rpixels.Length; px++)
            {
                rpixels[px] = source.GetPixelBilinear(incX * ((float)px % targetWidth), incY * ((float)Mathf.Floor(px / targetWidth)));
            }
            result.SetPixels(rpixels, 0);
            result.Apply(updateMipmaps: useMipMap, makeNoLongerReadable: makeNoLongerReadable);
            return result;
        }
        #endregion

        #region Private
        private const string keyPrefix = "TextureDatabase";
        private const string extension = ".dat";
        private const int runtimeDBSize = 100;
        private static Dictionary<int, Texture2D> runtimeDatabase = new Dictionary<int, Texture2D>();
        private static List<int> removeList = new List<int>();

        private static void RemovePrefs(string hash)
        {
            PlayerPrefs.DeleteKey($"{keyPrefix}{hash}");
            PlayerPrefs.DeleteKey($"{keyPrefix}{hash}_W");
            PlayerPrefs.DeleteKey($"{keyPrefix}{hash}_H");
            PlayerPrefs.DeleteKey($"{keyPrefix}{hash}_Format");
        }

        private static void AddPrefs(string hash, string originalPath, Texture2D texture)
        {
            PlayerPrefs.SetString($"{keyPrefix}{hash}", originalPath);
            PlayerPrefs.SetInt($"{keyPrefix}{hash}_W", texture.width);
            PlayerPrefs.SetInt($"{keyPrefix}{hash}_H", texture.height);
            PlayerPrefs.SetInt($"{keyPrefix}{hash}_Format", (int)texture.format);
        }

        private static void RemoveTextureInfo(int hash)
        {
            // Remove
            RemovePrefs(hash.ToString());
            var textureDatabaseStr = PlayerPrefs.GetString($"{keyPrefix}", "");
            var tokens = textureDatabaseStr.Split(',', System.StringSplitOptions.RemoveEmptyEntries);
            textureDatabaseStr = "";
            var hashStr = hash.ToString();
            foreach (var token in tokens)
            {
                if (token != hashStr)
                    textureDatabaseStr += $",{token}";
            }
            PlayerPrefs.SetString($"{keyPrefix}", $"{textureDatabaseStr}");
        }

        private static void SetTextureInfo(int hash, string originalPath, Texture2D texture)
        {
            AddPrefs(hash.ToString(), originalPath, texture);

            // Append
            var textureDatabaseStr = PlayerPrefs.GetString($"{keyPrefix}", "");
            PlayerPrefs.SetString($"{keyPrefix}", $"{textureDatabaseStr},{hash}");
        }

        private static bool GetTextureInfo(int hash, out string originalPath, out int width, out int height, out int textureFormatValue)
        {
            var textureDatabaseStr = PlayerPrefs.GetString($"{keyPrefix}", "");
            if (string.IsNullOrEmpty(textureDatabaseStr))
            {
                originalPath = null;
                width = height = textureFormatValue = 0;
                return false;
            }

            originalPath = PlayerPrefs.GetString($"{keyPrefix}{hash}");
            width = PlayerPrefs.GetInt($"{keyPrefix}{hash}_W", -1);
            height = PlayerPrefs.GetInt($"{keyPrefix}{hash}_H", -1);
            textureFormatValue = PlayerPrefs.GetInt($"{keyPrefix}{hash}_Format", -1);

            return !string.IsNullOrEmpty(originalPath) && width > 0 && height > 0 && textureFormatValue >= 0;
        }

        private static void RemoveTextureFileFromHash(string hash)
        {
            var localPath = Path.Combine(Application.persistentDataPath, $"{hash}{extension}");
            try
            {
                File.Delete(localPath);
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
            }
        }
        #endregion
    }
}
