using RoR2;
using UnityEngine;
using UnityEngine.UI;

namespace HenryMod.Survivors.Henry.Components
{
    // Roy's flame meter on screen (Alex's Binding Blade art): the sword's outline, with a flame arc filling the blade from the
    // pommel toward the tip as the meter grows. The art (dark outline over a see-through grey sword, like the game's own
    // crosshairs) sits behind the flame and its lines on top; the flame's white-hot end rides the leading edge, and the lines
    // turn gold once he's Enflamed (50+). Blue fire on the Awakening skin. Only shown for the local player's own Roy.
    public class RoyMeterHud : MonoBehaviour
    {
        public static Sprite backSprite, outlineSprite, maskSprite;
        public static Texture2D flameTexture, flameTextureBlue;

        // size and place on a 1920x1080 screen: centred just below the crosshair (the ring reaches ~20 px below the aim point);
        // about four times the ring's width (Alex: ring about the size of the sprint chevron, blade scaled to match)
        private const float width = 130f, belowAimPoint = 26f;
        // tilted down toward the tip at the angle Alex drew it
        private const float tilt = 24.5f;

        private RoyController roy;
        private CharacterBody body;
        private GameObject canvasObject;
        private RectTransform fill;
        private RawImage flame;
        private Image outline;
        private float shown;

        public static RoyMeterHud Create(RoyController roy, CharacterBody body)
        {
            if (!outlineSprite || !maskSprite || !flameTexture) return null;
            var hud = roy.gameObject.AddComponent<RoyMeterHud>();
            hud.roy = roy;
            hud.body = body;
            hud.Build();
            return hud;
        }

        private void Build()
        {
            canvasObject = new GameObject("RoyMeterCanvas");
            var canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = -10;
            var scaler = canvasObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.matchWidthOrHeight = 0.5f;

            float height = width * outlineSprite.rect.height / outlineSprite.rect.width;
            RectTransform root = NewRect("RoyMeter", canvasObject.transform);
            root.anchorMin = root.anchorMax = new Vector2(0.5f, 0.5f);
            root.pivot = new Vector2(0.5f, 0.5f);
            root.sizeDelta = new Vector2(width, height);
            root.localRotation = Quaternion.Euler(0f, 0f, -tilt);
            // the tilted sword's highest point sits belowAimPoint under the screen centre
            float rad = tilt * Mathf.Deg2Rad;
            float tiltedHeight = width * Mathf.Sin(rad) + height * Mathf.Cos(rad);
            root.anchoredPosition = new Vector2(0f, -(belowAimPoint + tiltedHeight * 0.5f));

            // the empty sword: Alex's art as drawn
            Image back = NewRect("Empty", root).gameObject.AddComponent<Image>();
            Stretch(back.rectTransform);
            back.sprite = backSprite ? backSprite : maskSprite;
            back.color = backSprite ? Color.white : new Color(0f, 0f, 0f, 0.35f);

            // the flame, clipped to the blade's shape
            Image maskImage = NewRect("Blade", root).gameObject.AddComponent<Image>();
            Stretch(maskImage.rectTransform);
            maskImage.sprite = maskSprite;
            maskImage.gameObject.AddComponent<Mask>().showMaskGraphic = false;

            fill = NewRect("Fill", maskImage.rectTransform);
            fill.anchorMin = new Vector2(0f, 0f);
            fill.anchorMax = new Vector2(0f, 1f);
            fill.pivot = new Vector2(0f, 0.5f);
            fill.sizeDelta = new Vector2(0f, 0f);
            flame = fill.gameObject.AddComponent<RawImage>();
            flame.texture = flameTexture;
            flame.uvRect = new Rect(1f, 0f, -1f, 1f);   // hot end of the arc at the leading edge

            outline = NewRect("Outline", root).gameObject.AddComponent<Image>();
            Stretch(outline.rectTransform);
            outline.sprite = outlineSprite;
        }

        private static RectTransform NewRect(string name, Transform parent)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            return (RectTransform)go.transform;
        }

        private static void Stretch(RectTransform rect)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.sizeDelta = Vector2.zero;
        }

        private void Update()
        {
            if (!canvasObject || !roy) return;
            shown = Mathf.MoveTowards(shown, roy.MeterFraction, Time.deltaTime * 1.5f);
            fill.sizeDelta = new Vector2(width * shown, 0f);

            bool blue = body && body.skinIndex == 1;
            flame.texture = blue && flameTextureBlue ? flameTextureBlue : flameTexture;
            float flicker = 0.85f + 0.15f * Mathf.PerlinNoise(Time.time * 6f, 0f);
            flame.color = new Color(1f, 1f, 1f, flicker);

            Color lineColor = new Color(0.2f, 0.2f, 0.2f, 1f);   // the art's dark outline
            Color enflamed = blue ? new Color(0.6f, 0.85f, 1f) : new Color(1f, 0.8f, 0.35f);
            outline.color = roy.Enflamed ? enflamed : lineColor;
        }

        private void OnDestroy()
        {
            if (canvasObject) Destroy(canvasObject);
        }
    }

    // Roy's crosshair: Alex's ring, its centre orb sitting on the aim point. Built from the standard crosshair (so the HUD
    // treats it like any other) with its own graphics swapped for the ring.
    internal static class RoyCrosshair
    {
        // the orb's centre in the ring image (measured: x 221 of 439, y 128.5 of 399 from the top)
        private static readonly Vector2 orbPivot = new Vector2(221f / 439f, 1f - 128.5f / 399f);
        private const float ringWidth = 32f;   // about the size of the sprint crosshair's chevron

        public static GameObject Create(Sprite ring)
        {
            GameObject standard = RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/Crosshair/StandardCrosshair");
            if (!standard || !ring) return standard;
            GameObject crosshair = R2API.PrefabAPI.InstantiateClone(standard, "RoyCrosshair", false);

            var controller = crosshair.GetComponent<RoR2.UI.CrosshairController>();
            if (controller) controller.spriteSpreadPositions = new RoR2.UI.CrosshairController.SpritePosition[0];
            foreach (Graphic g in crosshair.GetComponentsInChildren<Graphic>(true)) g.enabled = false;

            var go = new GameObject("Ring", typeof(RectTransform));
            go.transform.SetParent(crosshair.transform, false);
            var rect = (RectTransform)go.transform;
            rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = orbPivot;
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = new Vector2(ringWidth, ringWidth * ring.rect.height / ring.rect.width);
            var image = go.AddComponent<Image>();
            image.sprite = ring;
            image.raycastTarget = false;
            image.color = Color.white;   // drawn colours: dark outline, see-through grey inside
            return crosshair;
        }
    }
}
