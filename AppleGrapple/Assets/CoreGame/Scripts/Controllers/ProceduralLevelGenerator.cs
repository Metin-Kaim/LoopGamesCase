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
        public GameObject boundaryPrefab;

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

            for (int x = 0; x < boundaryWidth; x++)
            {
                float posX = x * tileSize - halfW;

                Vector3 topPos = new Vector3(posX, halfH, 0f);
                Instantiate(boundaryPrefab, topPos, Quaternion.identity, boundaryHolder.transform);

                Vector3 bottomPos = new Vector3(posX, -halfH, 0f);
                Instantiate(boundaryPrefab, bottomPos, Quaternion.identity, boundaryHolder.transform);
            }

            for (int y = 1; y < boundaryHeight - 1; y++)
            {
                float posY = y * tileSize - halfH;

                Vector3 leftPos = new Vector3(-halfW, posY, 0f);
                Instantiate(boundaryPrefab, leftPos, Quaternion.identity, boundaryHolder.transform);

                Vector3 rightPos = new Vector3(halfW, posY, 0f);
                Instantiate(boundaryPrefab, rightPos, Quaternion.identity, boundaryHolder.transform);
            }
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