using Assets.CoreGame.Scripts.Signals;
using UnityEngine;

namespace Assets.CoreGame.Scripts.Controllers
{
    public class ProceduralLevelGenerator : MonoBehaviour
    {
        [Header("Ground Settings")]
        public int groundWidth = 15;
        public int groundHeight = 15;
        public float tileSize = 1f;
        public WeightedTile[] groundTiles;

        [Header("Boundary Settings")]
        public int boundaryWidth = 7;
        public int boundaryHeight = 7;
        public GameObject boundaryMainFence;
        public GameObject boundaryVerticalFence;
        public GameObject boundaryHorizontalFence;

        void Awake()
        {
            GenerateGround();
            GenerateBoundary();
        }
        private void OnEnable()
        {
            GameSignals.Instance.GetGameAreaBoundary += OnGetGameAreaBoundary;
            GameSignals.Instance.GetGameArea += OnGetGameArea;
            GameSignals.Instance.GetTileSize += OnGetTileSize;
        }
        private float OnGetTileSize()
        {
            return tileSize;
        }
        private Vector2 OnGetGameArea()
        {
            return new Vector2(groundWidth, groundHeight);
        }
        private Vector2 OnGetGameAreaBoundary()
        {
            return new Vector2(boundaryWidth * tileSize, boundaryHeight * tileSize);
        }
        private void OnDisable()
        {
            GameSignals.Instance.GetGameAreaBoundary -= OnGetGameAreaBoundary;
            GameSignals.Instance.GetGameArea -= OnGetGameArea;
            GameSignals.Instance.GetTileSize -= OnGetTileSize;
        }

        private void GenerateBoundary()
        {
            GameObject boundaryHolder = new GameObject("Boundary");
            boundaryHolder.transform.parent = transform;
            boundaryHolder.transform.localPosition = Vector3.zero;

            float halfW = (boundaryWidth - 1) * tileSize / 2f;
            float halfH = (boundaryHeight - 1) * tileSize / 2f;

            for (int x = 0; x < boundaryWidth * 2 - 1; x++)
            {
                float posX = x * tileSize / 2 - halfW;

                Vector3 topPos = new Vector3(posX, halfH, 0f);
                GameObject topMainFence = Instantiate(boundaryMainFence, topPos, Quaternion.identity, boundaryHolder.transform);
                topMainFence.GetComponent<SpriteRenderer>().sortingOrder -= 2 * (boundaryHeight * 2 - 1);

                Vector3 bottomPos = new Vector3(posX, -halfH, 0f);
                Instantiate(boundaryMainFence, bottomPos, Quaternion.identity, boundaryHolder.transform);

                if (x == 0) continue;

                Vector2 topHorizontalPos = new Vector2(posX - (tileSize / 4.25f), halfH);
                GameObject topHorizontalFence = Instantiate(boundaryHorizontalFence, topHorizontalPos, Quaternion.identity, boundaryHolder.transform);

                Vector2 bottomHorizontalPos = new Vector2(posX - (tileSize / 4.25f), -halfH);
                Instantiate(boundaryHorizontalFence, bottomHorizontalPos, Quaternion.identity, boundaryHolder.transform);

            }

            for (int y = 1; y < boundaryHeight * 1.5f - 1; y++)
            {
                float posY = y * tileSize / 1.5f - halfH;

                Vector3 leftPos = new Vector3(-halfW, posY, 0f);
                GameObject leftMainFence = Instantiate(boundaryMainFence, leftPos, Quaternion.identity, boundaryHolder.transform);
                leftMainFence.GetComponent<SpriteRenderer>().sortingOrder -= 2 * y;

                Vector3 rightPos = new Vector3(halfW, posY, 0f);
                GameObject rightMainFence = Instantiate(boundaryMainFence, rightPos, Quaternion.identity, boundaryHolder.transform);
                rightMainFence.GetComponent<SpriteRenderer>().sortingOrder -= 2 * y;

                Vector2 leftVerticalPos = new Vector2(-halfW, posY - (tileSize / 5f));
                GameObject leftVerticalFence = Instantiate(boundaryVerticalFence, leftVerticalPos, Quaternion.identity, boundaryHolder.transform);
                leftVerticalFence.GetComponent<SpriteRenderer>().sortingOrder -= 2 * y - 1;

                Vector2 rightVerticalPos = new Vector2(halfW, posY - (tileSize / 5f));
                GameObject rightVerticalFence = Instantiate(boundaryVerticalFence, rightVerticalPos, Quaternion.identity, boundaryHolder.transform);
                rightVerticalFence.GetComponent<SpriteRenderer>().sortingOrder -= 2 * y - 1;
            }

            float leftVerticalPosEndY = (boundaryHeight - 1) * tileSize - halfH - (tileSize / 5f);

            Vector2 leftVerticalPosEnd = new Vector2(-halfW, leftVerticalPosEndY);
            GameObject leftVerticalEndFence = Instantiate(boundaryVerticalFence, leftVerticalPosEnd, Quaternion.identity, boundaryHolder.transform);
            leftVerticalEndFence.GetComponent<SpriteRenderer>().sortingOrder -= (int)(2 * (boundaryHeight * 1.5f - 2) + 1);

            Vector2 rightVerticalPosEnd = new Vector2(halfW, leftVerticalPosEndY);
            GameObject rightVerticalEndFence = Instantiate(boundaryVerticalFence, rightVerticalPosEnd, Quaternion.identity, boundaryHolder.transform);
            rightVerticalEndFence.GetComponent<SpriteRenderer>().sortingOrder -= (int)(2 * (boundaryHeight * 1.5f - 2) + 1);
        }


        void GenerateGround()
        {
            GameObject groundHolder = new GameObject("Ground");
            groundHolder.transform.parent = transform;
            groundHolder.transform.localPosition = Vector3.zero;

            float totalWeight = 0f;
            foreach (var tile in groundTiles)
                totalWeight += tile.weight;

            float halfW = (groundWidth - 1) * tileSize / 2f;
            float halfH = (groundHeight - 1) * tileSize / 2f;

            for (int x = 0; x < groundWidth; x++)
            {
                for (int y = 0; y < groundHeight; y++)
                {
                    GameObject prefab = GetWeightedRandomPrefab(totalWeight);
                    Vector3 pos = new Vector3(x * tileSize - halfW, y * tileSize - halfH, 0f);
                    Instantiate(prefab, pos, Quaternion.identity, groundHolder.transform);
                }
            }
        }

        GameObject GetWeightedRandomPrefab(float totalWeight)
        {
            float r = Random.value * totalWeight;
            float cumulative = 0f;

            foreach (var tile in groundTiles)
            {
                cumulative += tile.weight;
                if (r <= cumulative)
                    return tile.prefab;
            }

            // Fallback
            return groundTiles[groundTiles.Length - 1].prefab;
        }

        [System.Serializable]
        public class WeightedTile
        {
            public GameObject prefab;
            public float weight;
        }

#if UNITY_EDITOR
        // Draw preview of generated level in the scene view
        private void OnDrawGizmos()
        {
            // Draw ground tiles grid
            Gizmos.color = new Color(0f, 1f, 0f, 0.6f);
            float halfGW = (groundWidth - 1) * tileSize / 2f;
            float halfGH = (groundHeight - 1) * tileSize / 2f;

            for (int x = 0; x < groundWidth; x++)
            {
                for (int y = 0; y < groundHeight; y++)
                {
                    Vector3 pos = transform.position + new Vector3(x * tileSize - halfGW, y * tileSize - halfGH, 0f);
                    Gizmos.DrawWireCube(pos, new Vector3(tileSize, tileSize, 0.01f));
                }
            }

            // Draw boundary rectangle
            Gizmos.color = new Color(1f, 0f, 0f, 0.9f);
            Vector3 boundaryCenter = transform.position;
            Vector3 boundarySize = new Vector3(boundaryWidth * tileSize, boundaryHeight * tileSize, 0.01f);
            Gizmos.DrawWireCube(boundaryCenter, boundarySize);

            // Optionally draw boundary tiles positions
            Gizmos.color = new Color(1f, 0.5f, 0f, 0.6f);
            float halfBW = (boundaryWidth - 1) * tileSize / 2f;
            float halfBH = (boundaryHeight - 1) * tileSize / 2f;

            for (int x = 0; x < boundaryWidth; x++)
            {
                float posX = x * tileSize - halfBW;
                Vector3 topPos = transform.position + new Vector3(posX, halfBH, 0f);
                Vector3 bottomPos = transform.position + new Vector3(posX, -halfBH, 0f);
                Gizmos.DrawWireCube(topPos, new Vector3(tileSize, tileSize, 0.01f));
                Gizmos.DrawWireCube(bottomPos, new Vector3(tileSize, tileSize, 0.01f));
            }

            for (int y = 1; y < boundaryHeight - 1; y++)
            {
                float posY = y * tileSize - halfBH;
                Vector3 leftPos = transform.position + new Vector3(-halfBW, posY, 0f);
                Vector3 rightPos = transform.position + new Vector3(halfBW, posY, 0f);
                Gizmos.DrawWireCube(leftPos, new Vector3(tileSize, tileSize, 0.01f));
                Gizmos.DrawWireCube(rightPos, new Vector3(tileSize, tileSize, 0.01f));
            }
        }
#endif
    }
}