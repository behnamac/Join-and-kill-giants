using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;

namespace Player
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class PlayerMoveController : MonoBehaviour
    {
        [SerializeField, Tooltip("Renderer for the player mesh.")]
        private Renderer meshRenderer;

        [SerializeField, Tooltip("Color to change to when the player is active.")]
        private Color activeColor = Color.blue;

        private NavMeshAgent _navMeshAgent;
        private Transform _targetMove;

        public bool ActiveMove { get; private set; }
        public PlayerContainerController PlayerContainer { get; private set; }

        private void Awake()
        {
            _navMeshAgent = GetComponent<NavMeshAgent>();
            PlayerContainer = FindObjectOfType<PlayerContainerController>();
        }

        private void Update()
        {
            if (ActiveMove)
            {
                Move();
            }
        }

        private void Move()
        {
            if (_targetMove != null)
            {
                _navMeshAgent.SetDestination(_targetMove.position);
            }
        }

        /// <summary>
        /// Activates the player and sets its target and container.
        /// </summary>
        public void Active(Transform parent, Transform target, PlayerContainerController container)
        {
            transform.SetParent(parent);
            meshRenderer.material.DOColor(activeColor, 0.6f);
            _targetMove = target;
            PlayerContainer = container;
            ActiveMove = true;
        }

        /// <summary>
        /// Handles the player's death and checks the number of remaining players.
        /// </summary>
        public void Dead()
        {
            if (PlayerContainer != null)
            {
                PlayerContainer.CheckNumberPlayers();
            }
            Destroy(gameObject);
        }
    }
}
