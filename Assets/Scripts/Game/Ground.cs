using UnityEngine;

namespace Game
{
    [RequireComponent(typeof(Renderer))]
    public class Ground : MonoBehaviour
    {
        public Gradient Spectrum;

        private Material _material;

        private void Start()
        {
            _material = GetComponent<Renderer>().material;
            _material.SetTexture("_SpectrumTex", GenerateTexture());
        }

        private Texture2D GenerateTexture()
        {
            var texture = new Texture2D(512, 1);
            for (var i = 0; i < texture.width; i++)
            {
                texture.SetPixel(i, 0, Spectrum.Evaluate((float) i / texture.width));
                Debug.Log(Spectrum.Evaluate((float)i / texture.width));
            }

            texture.Apply(true, true);
            return texture;
        }
    }
}
