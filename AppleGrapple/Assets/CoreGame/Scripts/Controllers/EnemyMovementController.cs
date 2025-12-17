using Assets.CoreGame.Scripts.Signals;
using UnityEngine;

namespace Assets.CoreGame.Scripts.Controllers
{
    public class EnemyMovementController : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 2f;
        [SerializeField] private Transform visual;
        [SerializeField] private LayerMask bubbleLayer;
        [SerializeField] private LayerMask characterLayers;

        private bool _isLeft;
        private Vector2 _targetPos;
        private float _stoppingDistance;
        private bool _isStopped;
        private Animator _animator;
        private bool _canMove;

        public bool IsStopped => _isStopped;
        public LayerMask BubbleLayer => bubbleLayer;
        public LayerMask CharacterLayers => characterLayers;

        private void Start()
        {
            _animator = GetComponent<Animator>();
            SetTargetPosition(transform.position);
            _canMove = true;
        }

        private void Update()
        {
            if (!_canMove) return;
            MoveTowardsTarget();
        }

        private void MoveTowardsTarget()
        {
            if (Vector3.Distance(transform.position, _targetPos) < _stoppingDistance)
            {
                _isStopped = true;
                _animator.SetBool("walk", false);
                return;
            }
            _isStopped = false;

            Vector2 currentPosition = transform.position;
            Vector2 direction = (_targetPos - currentPosition).normalized;

            _isLeft = direction.x < 0;
            visual.rotation = Quaternion.Euler(Vector2.up * (_isLeft ? 180 : 0));

            transform.position += new Vector3(direction.x, direction.y, 0) * moveSpeed * Time.deltaTime;

            _animator.SetBool("walk", true);
        }

        public void SetTargetPosition(Vector2 newTargetPos, float stoppingDistance = 0.1f)
        {
            newTargetPos = GameSignals.Instance.onSetThePositionWithinTheBoundaries.Invoke(newTargetPos);

            _targetPos = newTargetPos;
            _stoppingDistance = stoppingDistance;
        }

        public void SetNewRandomPosition()
        {
            Vector2 randomDirection = Random.insideUnitCircle.normalized;
            float randomDistance = Random.Range(2f, 5f);
            Vector2 targetPos = (randomDirection * randomDistance) + (Vector2)transform.position;

            targetPos = GameSignals.Instance.onSetThePositionWithinTheBoundaries.Invoke(targetPos);

            SetTargetPosition(targetPos);
        }

        public void SetCanMove(bool value)
        {
            _canMove = value;
            _animator.SetBool("walk", value);
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(_targetPos, 0.1f);
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, _targetPos);
        }
#endif
    }
}