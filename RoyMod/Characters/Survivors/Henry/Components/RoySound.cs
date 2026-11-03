using RoR2;
using System.IO;
using UnityEngine;

namespace HenryMod.Survivors.Henry.Components
{
    // Roy's Smash sound effects. The game plays sound through Wwise (Unity's own audio is switched off), so Alex's .wav files
    // are built into a Wwise sound bank (RoyMod\Wwise\RoySound, bank RoyBank: one "Play_<file name>" event per sound) that's
    // embedded in the dll and handed to the game through R2API's SoundAPI.
    internal static class RoySound
    {
        public static void Load()
        {
            using (Stream stream = typeof(RoySound).Assembly.GetManifestResourceStream("RoyBank.bnk"))
            {
                if (stream == null) { Log.Warning("RoySound: RoyBank.bnk isn't embedded"); return; }
                var bytes = new byte[stream.Length];
                stream.Read(bytes, 0, bytes.Length);
                R2API.SoundAPI.SoundBanks.Add(bytes);
            }
        }

        // name = the sound file's name, e.g. "se_roy_hit_s"; plays from that object (local to this client)
        public static void Play(string name, GameObject source)
        {
            if (source) Util.PlaySound("Play_" + name, source);
        }
    }
}
