using System;
using System.Collections;
using UnityEngine;

namespace Assets.CoreGame.Scripts.Signals
{
    public class PlayerSignals : MonoBehaviour
    {
        public static PlayerSignals Instance;

        public Func<Vector2, bool> onGetIsPointCloseToThePlayer;

        private void Awake()
        {
            Instance = this;
        }
    }
}