using Assets.CoreGame.Scripts.Enums;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.CoreGame.Scripts.Signals
{
    public class PoolSignals : MonoBehaviour
    {
        public static PoolSignals Instance;

        public Func<PoolType, GameObject> onGetItemFromPool;
        public UnityAction<PoolType, GameObject> onReturnItemToPool;

        private void Awake()
        {
            Instance = this;
        }
    }
}