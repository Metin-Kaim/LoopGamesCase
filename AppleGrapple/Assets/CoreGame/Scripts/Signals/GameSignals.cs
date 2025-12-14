using System;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.CoreGame.Scripts.Signals
{
    public class GameSignals : MonoBehaviour
    {
        public static GameSignals Instance;

        public Func<Vector2> GetGameArea;
        public Func<float> GetTileSize;
        public Func<Vector2> GetGameAreaBoundary;
        public Func<Vector2> GetRandomPointInGameBoundary;
        public UnityAction onGameEnded;
        public UnityAction<GameObject> onEnemyDied;

        public Func<Vector2, Vector2> SetThePositionWithinTheBoundaries { get; internal set; }

        private void Awake()
        {
            Instance = this;
        }
    }
}