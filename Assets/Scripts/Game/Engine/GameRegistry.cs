﻿using System.Collections.Generic;
using UnityEngine;

namespace Game.Engine
{
    public static class GameRegistry
    {
        public static List<ISlashable> Slashables { get; }
        public static List<Enemy> Enemies { get; }

        static GameRegistry()
        {
            Slashables = new List<ISlashable>();
            Enemies = new List<Enemy>();
        }

        public static void Spawn(MonoBehaviour obj)
        {
            var slashable = obj as ISlashable;
            var enemy = obj as Enemy;
            if (slashable != null)
            {
                Slashables.Add(slashable);
            }

            if (enemy != null)
            {
                Enemies.Add(enemy);
            }
        }

        public static void Remove(MonoBehaviour obj)
        {
            var slashable = obj as ISlashable;
            var enemy = obj as Enemy;
            if (slashable != null)
            {
                Slashables.Remove(slashable);
            }

            if (enemy != null)
            {
                Enemies.Remove(enemy);
            }
        }
    }
}