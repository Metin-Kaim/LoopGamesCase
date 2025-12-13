using System;
using UnityEngine;

namespace Assets.CoreGame.Scripts.Signals
{
    public class GameSignals : MonoBehaviour
    {
        public static GameSignals Instance;

        public Func<Vector2> GetGameArea;
        public Func<float> GetTileSize;
        public Func<Vector2> GetGameAreaBoundary;

        private void Awake()
        {
            Instance = this;
        }
    }
}