using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(PlayerMoveController))]
    public class PlayerAttackController : MonoBehaviour
    {
        [SerializeField, Tooltip("Delay between attacks in seconds.")]
        private float attackDelay;

        [SerializeField, Tooltip("Damage dealt by each attack.")]
        private float attackDamage;

        private PlayerMoveController _playerMove;
        private float _currentDelay;

        private void Awake()
        {
            _playerMove = GetComponent<PlayerMoveController>();

            if (_playerMove == null)
            {
                Debug.LogError("PlayerMoveController component not found on the player.");
                enabled = false;
                return;
            }

            _currentDelay = attackDelay;
        }

        private void Update()
        {
            if (_playerMove.PlayerContainer == null || _playerMove.PlayerContainer.Giant == null) return;

            Attack();
        }

        /// <summary>
        /// Handles the attack logic, applying damage to the enemy with a delay.
        /// </summary>
        private void Attack()
        {
            _currentDelay -= Time.deltaTime;
            if (_currentDelay <= 0)
            {
                _playerMove.PlayerContainer.Giant.TakeDamage(attackDamage);
                _currentDelay = attackDelay;
            }
        }
    }
}
