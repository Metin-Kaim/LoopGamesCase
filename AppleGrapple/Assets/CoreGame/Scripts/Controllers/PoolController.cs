using Assets.CoreGame.Scripts.Enums;
using Assets.CoreGame.Scripts.Signals;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.CoreGame.Scripts.Controllers
{
    public class PoolController : MonoBehaviour
    {
        [SerializeField] List<PoolInfo> pools;

        private Dictionary<PoolType, PoolInfo> _poolDictionary;

        private void Awake()
        {
            InitPools();
        }

        private void OnEnable()
        {
            PoolSignals.Instance.onGetItemFromPool += OnGetItemFromPool;
            PoolSignals.Instance.onReturnItemToPool += OnReturnItemToPool;
        }
        private void OnDisable()
        {
            PoolSignals.Instance.onGetItemFromPool -= OnGetItemFromPool;
            PoolSignals.Instance.onReturnItemToPool -= OnReturnItemToPool;
        }

        private GameObject OnGetItemFromPool(PoolType poolType)
        {
            if (!_poolDictionary.TryGetValue(poolType, out PoolInfo pool))
            {
                Debug.LogError($"Pool type {poolType} not found!");
                return null;
            }

            if (pool.Pool.Count == 0)
            {
                IncreasePoolSize(pool);
            }

            GameObject item = pool.Pool.Dequeue();
            item.SetActive(true);
            return item;
        }

        private void OnReturnItemToPool(PoolType poolType, GameObject item)
        {
            if (!_poolDictionary.TryGetValue(poolType, out PoolInfo pool))
            {
                Debug.LogError($"Pool type {poolType} not found!");
                Destroy(item);
                return;
            }

            item.SetActive(false);
            item.transform.SetParent(transform);
            pool.Pool.Enqueue(item);
        }

        private void InitPools()
        {
            _poolDictionary = new Dictionary<PoolType, PoolInfo>();

            foreach (var pool in pools)
            {
                pool.Pool = new Queue<GameObject>();
                _poolDictionary[pool.PoolType] = pool;

                for (int i = 0; i < pool.InitialSize; i++)
                {
                    CreatePooledObject(pool);
                }
            }
        }
        private GameObject CreatePooledObject(PoolInfo pool)
        {
            GameObject obj = Instantiate(pool.Prefab, transform);
            obj.SetActive(false);
            pool.Pool.Enqueue(obj);
            return obj;
        }
        private void IncreasePoolSize(PoolInfo pool)
        {
            for (int i = 0; i < pool.IncrementSize; i++)
            {
                CreatePooledObject(pool);
            }
        }
    }

    [System.Serializable]
    public class PoolInfo
    {
        public PoolType PoolType;
        public GameObject Prefab;
        public byte InitialSize;
        public byte IncrementSize;
        public Queue<GameObject> Pool;
    }
}