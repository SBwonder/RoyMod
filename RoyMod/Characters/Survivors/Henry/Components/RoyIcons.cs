using System.IO;
using UnityEngine;

namespace HenryMod.Survivors.Henry.Components
{
    // Alex's drawn icons (RoyMod\Icons\*.png, 256x256), embedded in the dll so they can change without a bundle rebuild
    internal static class RoyIcons
    {
        public static bool Has(string fileName) => typeof(RoyIcons).Assembly.GetManifestResourceInfo("Icons." + fileName) != null;

        public static Texture2D LoadTexture(string fileName)
        {
            using (Stream stream = typeof(RoyIcons).Assembly.GetManifestResourceStream("Icons." + fileName))
            {
                if (stream == null) { Log.Warning($"RoyIcons: {fileName} isn't embedded"); return null; }
                var bytes = new byte[stream.Length];
                stream.Read(bytes, 0, bytes.Length);
                var texture = new Texture2D(2, 2, TextureFormat.RGBA32, false) { name = Path.GetFileNameWithoutExtension(fileName) };
                texture.LoadImage(bytes);
                texture.wrapMode = TextureWrapMode.Clamp;
                return texture;
            }
        }

        public static Sprite LoadSprite(string fileName)
        {
            Texture2D texture = LoadTexture(fileName);
            return texture ? Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f)) : null;
        }
    }
}
