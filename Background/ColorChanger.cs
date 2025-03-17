using System.Drawing;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace AVTemp.Background
{
    public class ColorChanger : TimedBehaviour
    {
        public override void Start()
        {
            base.Start();
            gameObjectRenderer = base.GetComponent<Renderer>();
            Update();
        }

        public override void Update()
        {
            base.Update();
            if (colors != null)
            {
                if (timeBased)
                {
                    color = colors.Evaluate((Time.time / 2f) % 1);
                }
                if (isRainbow)
                {
                    float h = (Time.frameCount / 180f) % 1f;
                    color = UnityEngine.Color.HSVToRGB(h, 1f, 1f);
                }
                gameObjectRenderer.material.color = color;
            }
        }

        public Renderer gameObjectRenderer;
        public Gradient colors = null;
        public Color32 color;
        public bool timeBased = true;
        public bool isRainbow = false;
        public bool isPastelRainbow = false;
        public bool isEpileptic = false;
        public bool isMonkeColors = false;
        public Renderer renderer;
    }
}
