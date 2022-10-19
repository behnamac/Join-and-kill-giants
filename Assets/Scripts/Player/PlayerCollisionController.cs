using Controllers;
using UnityEngine;
using Enemy;

namespace Player
{
    [RequireComponent(typeof(PlayerMoveController))]
    public class PlayerCollisionController : MonoBehaviour
    {
        private PlayerMoveController _playerMove;

        private void Awake()
        {
            _playerMove = GetComponent<PlayerMoveController>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<PlayerMoveController>())
            {
                var otherPlayer = other.GetComponent<PlayerMoveController>();
                if (_playerMove.activeMove && !otherPlayer.activeMove)
                {
                    var parent = transform.parent;
                    otherPlayer.Active(parent, parent, _playerMove.playerContainer);
                }
            }
            else if (other.GetComponent<GiantController>())
            {
                var enemy = other.GetComponent<GiantController>();
                if (!enemy.active)
                {
                    _playerMove.playerContainer.AddGiant(enemy);
                }
            }
            else if(other.gameObject.CompareTag($"Obstcle"))
            {
                _playerMove.Dead();
            }
            else if(other.gameObject.CompareTag($"FinishLine"))
            {
                LevelManager.instance.LevelStageComplete();
            }
            else if(other.gameObject.CompareTag($"X"))
            {
                _playerMove.playerContainer.AddPostLevel();
                other.enabled = false;
            }
        }
    }
}
