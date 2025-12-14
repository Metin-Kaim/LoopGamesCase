using Assets.CoreGame.Scripts.Abstract;
using Assets.CoreGame.Scripts.Controllers;
using Assets.CoreGame.Scripts.Enums;
using Assets.CoreGame.Scripts.Handlers;
using Assets.CoreGame.Scripts.Signals;
using Assets.CoreGame.Scripts.StateMachine.Enemy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.CoreGame.Scripts.Managers
{
    public class EnemyStateManager : MonoBehaviour
    {
        [SerializeField] private EnemyStateType currentStateType;
        [SerializeField] private EnemyMovementController enemyMovementController;
        [SerializeField] private WeaponHolderHandler weaponHolderHandler;

        private AbsEnemyStateMachine _currentState;
        private EnemyIdleState _idleState;
        private EnemyWanderState _wanderState;
        private EnemyEscapeState _escapeState;
        private EnemyChaseState _chaseState;

        private WeaponHolderHandler _target;

        private Dictionary<EnemyStateType, AbsEnemyStateMachine> _stateDictionary;

        public WeaponHolderHandler Target { get => _target; set => _target = value; }

        private void Awake()
        {
            _wanderState = new EnemyWanderState(this, enemyMovementController, weaponHolderHandler);
            _escapeState = new EnemyEscapeState(this, enemyMovementController, weaponHolderHandler);
            _chaseState = new EnemyChaseState(this, enemyMovementController, weaponHolderHandler);
            _idleState = new EnemyIdleState(this, enemyMovementController, weaponHolderHandler);

            _stateDictionary = new Dictionary<EnemyStateType, AbsEnemyStateMachine>
            {
                { EnemyStateType.Wander, _wanderState },
                { EnemyStateType.Escape, _escapeState },
                { EnemyStateType.Chase, _chaseState },
                { EnemyStateType.Idle, _idleState },
            };
        }

        private void OnEnable()
        {
            GameSignals.Instance.onGameEnded += () => ChangeState(EnemyStateType.Idle);
        }
        private void OnDisable()
        {
            GameSignals.Instance.onGameEnded -= () => ChangeState(EnemyStateType.Idle);
        }

        private IEnumerator Start()
        {
            yield return new WaitForSeconds(0.1f);
            ChangeState(EnemyStateType.Wander);
        }

        private void Update()
        {
            _currentState?.Tick();
        }

        public void ChangeState(EnemyStateType stateType)
        {
            if (_stateDictionary.TryGetValue(stateType, out var nextState))
            {
                _currentState = nextState;
                currentStateType = stateType;
                _currentState.Enter();
            }
            else
            {
                Debug.LogError($"State {stateType} not found in state dictionary.");
            }
        }

#if UNITY_EDITOR
        public void OnDrawGizmos()
        {
            _currentState?.OnDrawGizmos();
        }
#endif
    }
}