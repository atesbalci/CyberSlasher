using Game.Models;
using UnityEngine;

namespace Game.Views
{
    [RequireComponent(typeof(Renderer))]
    public class Ground : MonoBehaviour
    {
        private const float OffsetSpeed = 3f;
        private const float SpectrumSpeed = 1f;

        public Gradient Spectrum;

        private Material _material;
        private float _curSpectrumOffset;
        private float _curOffset;

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
            }

            texture.Apply(true, true);
            return texture;
        }

        private void Update()
        {
            _material.SetTextureOffset("_MainTex", new Vector2(0f, _curOffset));
            _material.SetTextureOffset("_SpectrumTex", new Vector2(_curSpectrumOffset, 0f));

            _curOffset -= GameTime.DeltaTime * OffsetSpeed;
            _curOffset -= (int)_curOffset;
            _curSpectrumOffset += GameTime.DeltaTime * SpectrumSpeed;
            _curSpectrumOffset -= (int) _curSpectrumOffset;
        }
    }
}
