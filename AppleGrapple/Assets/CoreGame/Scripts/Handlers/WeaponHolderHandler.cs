using Assets.CoreGame.Scripts.Enums;
using Assets.CoreGame.Scripts.Signals;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.CoreGame.Scripts.Handlers
{
    public class WeaponHolderHandler : MonoBehaviour
    {
        [Header("References")]
        public Transform weaponPoint;
        public ErasableGround erasableGround;

        [Header("Settings")]
        public float radius = 1.5f;
        public float smoothSpeed = 8f;
        public short initialSwordCount = 2;
        public float swordScaleAnimationDuration = 0.5f;

        private List<SwordHandler> _swords = new List<SwordHandler>();

        public int SwordCount => _swords.Count;

        private void Awake()
        {
            if (erasableGround == null)
            {
                erasableGround = GameObject.FindGameObjectWithTag("ScratchCard")?.GetComponent<ErasableGround>();
            }
        }

        private void Start()
        {
            for (int i = 0; i < initialSwordCount; i++)
            {
                IncreaseSword(false);
            }
        }

        public void SwordBubbleCollected(GameObject bubble)
        {
            SwordBubbleSignals.Instance.onSwordBubbleCollected?.Invoke(bubble);
            IncreaseSword();
        }

        void Update()
        {
            UpdateSwordPositions();
            RotateSwordsToCharacter();

            ScratchCard();
        }

        private void ScratchCard()
        {
            //Vector3 pos = Camera.main.WorldToScreenPoint(transform.position);
            erasableGround.EraseAtWorldPosition(transform.position);

            foreach (var sword in _swords)
            {
                //Vector3 swordPos = Camera.main.WorldToScreenPoint(sword.transform.position);
                //scratchCard.ManuelScratch(swordPos);
                erasableGround.EraseAtWorldPosition(sword.transform.position);
            }
        }

        public void DecreaseSword(SwordHandler sword)
        {
            if (_swords.Contains(sword))
            {
                _swords.Remove(sword);
                PoolSignals.Instance.onReturnItemToPool?.Invoke(PoolType.Sword, sword.gameObject);
            }
        }

        public void IncreaseSword(bool playAnimation = true)
        {
            //Get sword from pool
            SwordHandler sword = PoolSignals.Instance.onGetItemFromPool?.Invoke(PoolType.Sword).GetComponent<SwordHandler>();
            sword.transform.parent = weaponPoint;
            sword.transform.localPosition = Vector3.zero;
            sword.Init(this);

            _swords.Add(sword);

            if (playAnimation)
                sword.transform.DOScale(0, swordScaleAnimationDuration).From().SetEase(Ease.Linear);
        }

        private void UpdateSwordPositions()
        {
            int count = _swords.Count;
            if (count == 0) return;

            float angleStep = 360f / count;

            for (int i = 0; i < count; i++)
            {
                float angle = angleStep * i * Mathf.Deg2Rad;

                Vector2 targetPos =
                    new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;

                _swords[i].transform.localPosition = Vector2.Lerp(
                    _swords[i].transform.localPosition,
                    targetPos,
                    Time.deltaTime * smoothSpeed
                );
            }
        }

        private void RotateSwordsToCharacter()
        {
            if (weaponPoint == null) return;

            foreach (var sword in _swords)
            {
                Vector2 dir = (weaponPoint.position - sword.transform.position).normalized;

                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                sword.transform.rotation = Quaternion.Euler(0, 0, angle - 180f);
            }
        }
    }
}