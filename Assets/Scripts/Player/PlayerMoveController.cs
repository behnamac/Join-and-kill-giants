using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;

namespace Player
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class PlayerMoveController : MonoBehaviour
    {
        [SerializeField] private Renderer meshRenderer;
        [SerializeField] private Color activeColor = Color.blue;

        private NavMeshAgent _navMeshAgent;
        private Transform _targetMove;
        public bool activeMove { get; private set; }
        public PlayerContainerController playerContainer { get; set; }

        private void Awake()
        {
            _navMeshAgent = GetComponent<NavMeshAgent>();
        }

        private void Update()
        {
            if (activeMove)
            {
                Move();
            }
        }

        private void Move()
        {
            _navMeshAgent.SetDestination(_targetMove.position);
        }

        public void Active(Transform parent,Transform target, PlayerContainerController container)
        {
            transform.SetParent(parent);
            meshRenderer.material.DOColor(activeColor, 0.6f);
            _targetMove = target;
            playerContainer = container;
            activeMove = true;
        }

        public void Dead()
        {
            playerContainer.CheckNumberPlayers();
            Destroy(gameObject);
        }
    }
}
