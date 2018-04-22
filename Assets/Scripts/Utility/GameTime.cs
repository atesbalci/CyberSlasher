using UnityEngine;

namespace Utility
{
    public static class GameTime
    {
        private const float SlashingScale = 0.1f;

        public static bool Slashing { get; set; }

        public static float DeltaTime
        {
            get { return Time.deltaTime * Scale; }
        }

        public static float Scale
        {
            get { return Slashing ? SlashingScale : 1f; }
        }
    }
}
