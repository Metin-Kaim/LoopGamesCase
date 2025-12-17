using System;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.CoreGame.Scripts.Signals
{
    public class GameSignals : MonoBehaviour
    {
        public static GameSignals Instance;

        public Func<Vector2> onGetGameArea;
        public Func<float> onGetTileSize;
        public Func<Vector2> onGetGameAreaBoundary;
        public Func<Vector2> onGetRandomPointInGameBoundary;
        public UnityAction onGameEnded;
        public UnityAction<GameObject> onEnemyDied;

        public Func<Vector2, Vector2> onSetThePositionWithinTheBoundaries;

        private void Awake()
        {
            Instance = this;
        }
    }
}