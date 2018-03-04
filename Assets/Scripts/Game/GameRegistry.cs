using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public static class GameRegistry
    {
        public static List<ISlashable> Slashables { get; private set; }

        static GameRegistry()
        {
            Slashables = new List<ISlashable>();
        }
    }
}
