using EntityStates;
using HenryMod.Survivors.Henry.Components;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace HenryMod.Survivors.Henry.SkillStates
{
    // Roy's arrival instead of the drop pod: his Smash entrance (drops in, lands kneeling, rises, raises his sword)
    // inside a warp of light (RoyEntryWarp: light column, circling motes that light the area, ground ring, landing flash).
    // He stays hidden for a moment first, so the stage's fade-in from black doesn't cut off the start of the entrance.
    public class RoySpawnState : BaseState
    {
        public static float startDelay = 1f;
        public static float duration = 2f;
        public static GameObject warpEffectPrefab;      // warm light, default skin
        public static GameObject warpEffectPrefabBlue;  // blue light, Awakening skin (blue flames)

        private CharacterModel characterModel;
        private bool hidden;
        private bool started;

        public static GameObject WarpFor(int skinIndex)
        {
            return skinIndex > 0 && warpEffectPrefabBlue ? warpEffectPrefabBlue : warpEffectPrefab;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            Transform model = GetModelTransform();
            characterModel = model ? model.GetComponent<CharacterModel>() : null;
            if (characterModel)
            {
                characterModel.invisibilityCount++;
                hidden = true;
            }
            if (NetworkServer.active)
            {
                characterBody.AddTimedBuff(RoR2Content.Buffs.HiddenInvincibility, startDelay + duration);
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (!started && fixedAge >= startDelay)
            {
                started = true;
                Reveal();
                PlayAnimation("Body", "Entry");
                if (isAuthority) RoySound.Play("se_roy_appear01", gameObject);
                GameObject warp = WarpFor((int)characterBody.skinIndex);
                if (warp)
                {
                    GameObject fx = Object.Instantiate(warp, characterBody.footPosition, Quaternion.identity);
                    Object.Destroy(fx, duration + 1f);
                }
            }
            if (fixedAge >= startDelay + duration && isAuthority)
            {
                outer.SetNextStateToMain();
            }
        }

        private void Reveal()
        {
            if (hidden && characterModel) characterModel.invisibilityCount--;
            hidden = false;
        }

        public override void OnExit()
        {
            Reveal();
            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Death;
        }
    }
}
