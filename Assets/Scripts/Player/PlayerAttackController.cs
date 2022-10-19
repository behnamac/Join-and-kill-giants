using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(PlayerMoveController))]
    public class PlayerAttackController : MonoBehaviour
    {
        [SerializeField] private float attackDelay;
        [SerializeField] private float attackDamage;
        
        private PlayerMoveController _playerMove;
        private float _currentDelay;
        private void Awake()
        {
            _currentDelay = attackDelay;
            _playerMove = GetComponent<PlayerMoveController>();
        }

        private void Update()
        {
            if (!_playerMove.playerContainer) return;
            
            if (_playerMove.playerContainer.giant)
            {
                Attack();
            }
        }

        private void Attack()
        {
            _currentDelay -= Time.deltaTime;
            if (_currentDelay <= 0)
            {
                var enemy = _playerMove.playerContainer.giant;
                enemy.TakeDamage(attackDamage);
                _currentDelay = attackDelay;
            }

        }
    }
}
