using Controllers;
using UnityEngine;
using Enemy;

namespace Player
{
    [RequireComponent(typeof(PlayerMoveController))]
    public class PlayerCollisionController : MonoBehaviour
    {
        private PlayerMoveController _playerMove;

        private const string ObstacleTag = "Obstacle";
        private const string FinishLineTag = "FinishLine";
        private const string PostLevelTag = "X";

        private void Awake()
        {
            _playerMove = GetComponent<PlayerMoveController>();
        }

        private void OnTriggerEnter(Collider other)
        {
            HandlePlayerCollision(other);
            HandleGiantCollision(other);
            HandleObstacleCollision(other);
            HandleFinishLineCollision(other);
            HandlePostLevelCollision(other);
        }

        private void HandlePlayerCollision(Collider other)
        {
            var otherPlayerMove = other.GetComponent<PlayerMoveController>();
            if (otherPlayerMove != null && _playerMove.ActiveMove && !otherPlayerMove.ActiveMove)
            {
                var parent = transform.parent;
                otherPlayerMove.Active(parent, parent, _playerMove.PlayerContainer);
            }
        }

        private void HandleGiantCollision(Collider other)
        {
            var enemy = other.GetComponent<GiantController>();
            if (enemy != null && !enemy.Active)
            {
                _playerMove.PlayerContainer.AddGiant(enemy);
            }
        }

        private void HandleObstacleCollision(Collider other)
        {
            if (other.gameObject.CompareTag(ObstacleTag))
            {
                _playerMove.Dead();
            }
        }

        private void HandleFinishLineCollision(Collider other)
        {
            if (other.gameObject.CompareTag(FinishLineTag))
            {
                LevelManager.Instance.LevelStageComplete();
            }
        }

        private void HandlePostLevelCollision(Collider other)
        {
            if (other.gameObject.CompareTag(PostLevelTag))
            {
                _playerMove.PlayerContainer.AddPostLevel();
                other.enabled = false;
            }
        }
    }
}
