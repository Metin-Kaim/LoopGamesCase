using Assets.CoreGame.Scripts.Signals;
using UnityEngine;

namespace Assets.CoreGame.Scripts.Controllers
{
    public class PlayerMovementController : MonoBehaviour
    {
        [SerializeField] private Joystick joystick;
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private Transform visual;

        private Animator _animator;
        private Vector2 _gameAreaBoundary;
        private bool _isLeft;
        private bool _isMoving;

        private void Start()
        {
            _gameAreaBoundary = GameSignals.Instance.GetGameAreaBoundary.Invoke();
            _animator = GetComponent<Animator>();
        }

        private void Update()
        {
            Vector2 direction = joystick.Direction;

            if (direction == Vector2.zero)
            {
                if (!_isMoving) return;

                _isMoving = false;
                _animator.SetBool("walk", false);
                return;
            }
            else
            {
                if (!_isMoving)
                {
                    _isMoving = true;
                    _animator.SetBool("walk", true);
                }
            }

            _isLeft = direction.x < 0;

            visual.rotation = Quaternion.Euler(Vector2.up * (_isLeft ? 180 : 0));

            direction.Normalize();

            transform.position += new Vector3(direction.x, direction.y, 0) * moveSpeed * Time.deltaTime;

            transform.position = new Vector3(
                Mathf.Clamp(transform.position.x, -_gameAreaBoundary.x / 2f + 1, _gameAreaBoundary.x / 2f - 1),
                Mathf.Clamp(transform.position.y, -_gameAreaBoundary.y / 2f + 1, _gameAreaBoundary.y / 2f - 1),
                transform.position.z
            );
        }
    }
}