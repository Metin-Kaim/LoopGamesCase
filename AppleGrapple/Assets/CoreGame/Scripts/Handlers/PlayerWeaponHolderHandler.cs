using Assets.CoreGame.Scripts.Signals;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.CoreGame.Scripts.Handlers
{
    public class PlayerWeaponHolderHandler : MonoBehaviour
    {
        [Header("References")]
        public Transform orbitParent;
        public GameObject swordPrefab;

        [Header("Settings")]
        public float radius = 1.5f;
        public float smoothSpeed = 8f;
        public short initialSwordCount = 2;
        public float swordScaleAnimationDuration = 0.5f;

        private List<Transform> swords = new List<Transform>();

        private void Awake()
        {
            SetSwordCount(initialSwordCount, false);
        }

        private void OnEnable()
        {
            SwordBubbleSignals.Instance.onSwordBubbleCollected += OnSwordBubbleCollected;
        }
        private void OnSwordBubbleCollected(GameObject bubble)
        {
            SetSwordCount(swords.Count + 1);
        }
        private void OnDisable()
        {
            SwordBubbleSignals.Instance.onSwordBubbleCollected -= OnSwordBubbleCollected;
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.K))
            {
                SetSwordCount(swords.Count + 1);
            }
            if (Input.GetKeyDown(KeyCode.L))
            {
                SetSwordCount(Mathf.Max(0, swords.Count - 1));
            }

            UpdateSwordPositions();
            RotateSwordsToPlayer();
        }

        public void SetSwordCount(int count, bool playAnimation = true)
        {
            while (swords.Count > count)
            {
                Destroy(swords[swords.Count - 1].gameObject);
                swords.RemoveAt(swords.Count - 1);
            }

            while (swords.Count < count)
            {
                var sword = Instantiate(swordPrefab, orbitParent);
                swords.Add(sword.transform);

                if (playAnimation)
                    sword.transform.DOScale(0, swordScaleAnimationDuration).From().SetEase(Ease.Linear);
            }
        }

        private void UpdateSwordPositions()
        {
            int count = swords.Count;
            if (count == 0) return;

            float angleStep = 360f / count;

            for (int i = 0; i < count; i++)
            {
                float angle = angleStep * i * Mathf.Deg2Rad;

                Vector2 targetPos =
                    new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;

                swords[i].localPosition = Vector2.Lerp(
                    swords[i].localPosition,
                    targetPos,
                    Time.deltaTime * smoothSpeed
                );
            }
        }

        private void RotateSwordsToPlayer()
        {
            if (orbitParent == null) return;

            foreach (var sword in swords)
            {
                Vector2 dir = (orbitParent.position - sword.position).normalized;

                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                sword.rotation = Quaternion.Euler(0, 0, angle - 180f);
            }
        }
    }
}