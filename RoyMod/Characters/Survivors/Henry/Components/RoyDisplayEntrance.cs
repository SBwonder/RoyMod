using HenryMod.Survivors.Henry.SkillStates;
using UnityEngine;

namespace HenryMod.Survivors.Henry.Components
{
    // Character select: when Roy's display model appears, he plays the same warp-in entrance as at the start of a stage.
    // The lobby applies the chosen skin some time after the model appears, so the light's colour keeps following the
    // model for the length of the entrance: if the Awakening meshes turn on, the warm light is swapped for the blue one.
    internal class RoyDisplayEntrance : MonoBehaviour
    {
        private const float authoredScale = 1.9f / 1.49f;  // RoyEntryWarp is sized for Roy at his in-game scale

        private GameObject awakeningMeshes;
        private GameObject warp;
        private bool warpIsBlue;
        private float age;

        private void Awake()
        {
            Transform awakening = transform.Find("AwakeningSkin");
            awakeningMeshes = awakening ? awakening.gameObject : null;
        }

        private void OnEnable()
        {
            Animator animator = GetComponent<Animator>();
            if (animator)
            {
                // the display has no body driving the animator, so keep the Body layer on the ground once the entrance ends
                animator.SetBool("isGrounded", true);
                animator.Play("Entry", animator.GetLayerIndex("Body"), 0f);
                RoySound.Play("se_roy_appear01", gameObject);
            }
            age = 0f;
            SpawnWarp(IsAwakening());
        }

        private bool IsAwakening() => awakeningMeshes && awakeningMeshes.activeSelf;

        private void Update()
        {
            age += Time.deltaTime;
            if (age > RoySpawnState.duration) return;
            if (IsAwakening() != warpIsBlue) SpawnWarp(!warpIsBlue);
        }

        private void SpawnWarp(bool blue)
        {
            if (warp) Destroy(warp);
            warpIsBlue = blue;
            GameObject prefab = RoySpawnState.WarpFor(blue ? 1 : 0);
            if (!prefab) return;
            warp = Instantiate(prefab, transform.position, transform.rotation);
            warp.transform.localScale = Vector3.one * (transform.lossyScale.x / authoredScale);
            // pick up where the entrance already is, so a late swap doesn't restart the light from the beginning
            if (age > 0f) foreach (ParticleSystem ps in warp.GetComponentsInChildren<ParticleSystem>()) ps.Simulate(age, false, true, false);
            foreach (ParticleSystem ps in warp.GetComponentsInChildren<ParticleSystem>()) ps.Play(false);
            Destroy(warp, RoySpawnState.duration + 1f - age);
        }

        private void OnDisable()
        {
            if (warp) Destroy(warp);
        }
    }
}
