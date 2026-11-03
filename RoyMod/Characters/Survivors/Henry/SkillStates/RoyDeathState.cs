using EntityStates;
using HenryMod.Survivors.Henry.Components;
using RoR2;
using UnityEngine;

namespace HenryMod.Survivors.Henry.SkillStates
{
    // Roy's death, Fire Emblem style (Alex): instead of a ragdoll he's held in a hurt pose (Smash's flinch, frozen), the blow
    // sound plays as he falls to 0, then he fades to white (with the "dead" sound) and dissolves away.
    public class RoyDeathState : GenericCharacterDeath
    {
        private const float holdPoseAt = 0.15f;     // how far into the flinch the pose is frozen
        private const float fadeStart = 0.8f, whiteDuration = 0.9f, dissolveDuration = 0.5f;

        private Animator animator;
        private Renderer[] renderers;
        private readonly System.Collections.Generic.List<(Material material, Color color)> tinted = new System.Collections.Generic.List<(Material, Color)>();
        private bool frozen, fading;

        public override void OnEnter()
        {
            base.OnEnter();
            animator = GetModelAnimator();
            if (characterMotor) characterMotor.velocity = Vector3.zero;
        }

        public override void PlayDeathSound()
        {
            RoySound.Play("btl_blow2", gameObject);
        }

        public override void PlayDeathAnimation(float crossfadeDuration = 0.1f)
        {
            if (!animator) return;
            animator.CrossFadeInFixedTime("Flinch3", 0.05f, animator.GetLayerIndex("Flinch"), 0f);
        }

        // the body stays until the fade has finished
        public override bool shouldAutoDestroy => fixedAge >= fadeStart + whiteDuration + dissolveDuration;

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (characterMotor && characterMotor.isGrounded) characterMotor.velocity = Vector3.zero;

            if (!frozen && fixedAge >= holdPoseAt && animator)
            {
                frozen = true;
                animator.speed = 0f;
            }
            if (!fading && fixedAge >= fadeStart) StartFade();
        }

        public override void Update()
        {
            base.Update();
            if (!fading || renderers == null) return;

            float t = age - fadeStart;
            float white = Mathf.Clamp01(t / whiteDuration);
            float fade = 1f - Mathf.Clamp01((t - whiteDuration) / dissolveDuration);
            foreach (var (material, color) in tinted) if (material) material.SetColor("_Color", Color.Lerp(color, Color.white, white));
            foreach (Renderer r in renderers)
            {
                if (!r) continue;
                foreach (Material m in r.materials)
                {
                    if (m.HasProperty("_EmColor")) m.SetColor("_EmColor", Color.white * white * 2f);
                    if (m.HasProperty("_EmPower")) m.SetFloat("_EmPower", white * 2f);
                    if (m.HasProperty("_EmissionColor")) { m.EnableKeyword("_EMISSION"); m.SetColor("_EmissionColor", Color.white * white * 2f); }
                    if (m.HasProperty("_Fade")) m.SetFloat("_Fade", fade);   // Hopoo shader's dithered fade-out
                }
                if (fade <= 0f) r.enabled = false;
            }
        }

        private void StartFade()
        {
            fading = true;
            RoySound.Play("btl_dead1", gameObject);
            Transform model = cachedModelTransform;
            if (!model) return;
            // CharacterModel rewrites the renderers' properties every frame; stop it so the fade sticks
            CharacterModel characterModel = model.GetComponent<CharacterModel>();
            if (characterModel) characterModel.enabled = false;
            RoyFlameSword sword = model.GetComponentInChildren<RoyFlameSword>();
            if (sword) sword.gameObject.SetActive(false);
            renderers = model.GetComponentsInChildren<Renderer>();
            foreach (Renderer r in renderers)
                foreach (Material m in r.materials)
                    if (m.HasProperty("_Color")) tinted.Add((m, m.GetColor("_Color")));
        }
    }
}
