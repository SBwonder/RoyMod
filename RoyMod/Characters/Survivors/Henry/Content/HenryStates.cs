using HenryMod.Survivors.Henry.SkillStates;

namespace HenryMod.Survivors.Henry
{
    public static class HenryStates
    {
        public static void Init()
        {
            Modules.Content.AddEntityState(typeof(SlashCombo));

            Modules.Content.AddEntityState(typeof(Shoot));

            Modules.Content.AddEntityState(typeof(Roll));

            Modules.Content.AddEntityState(typeof(ThrowBomb));

            Modules.Content.AddEntityState(typeof(RoySpawnState));
            Modules.Content.AddEntityState(typeof(RoyDeathState));
            Modules.Content.AddEntityState(typeof(RoyPrimary));
            Modules.Content.AddEntityState(typeof(RoyDoubleEdgeDance));
            Modules.Content.AddEntityState(typeof(RoyJuggle));
            Modules.Content.AddEntityState(typeof(RoyDodge));
            Modules.Content.AddEntityState(typeof(RoyAirFollowUp));
            Modules.Content.AddEntityState(typeof(RoyCounter));
            Modules.Content.AddEntityState(typeof(RoyCounterHit));
            Modules.Content.AddEntityState(typeof(RoyFinalSmash));
            Modules.Content.AddEntityState(typeof(RoyFlareBlade));
            Modules.Content.AddEntityState(typeof(RoyDownAir));
        }
    }
}
