using UnityEngine;

namespace Controllers
{
    public class CameraController : MonoBehaviour
    {
        public static CameraController Instance { get; private set; }

        [SerializeField] private Transform target;
        [SerializeField] private Vector3 offset;
        [SerializeField] private float followSpeed = 0.1f;
        [SerializeField] private bool xPositionLock;
        [SerializeField] private bool isTargetLook;

        private void Initialize()
        {
            // Set the default offset if target is not null
            if (target != null)
            {
                offset = transform.position - target.position;
            }
            else
            {
                Debug.LogWarning("Target is not assigned to CameraController");
            }
        }

        private void SmoothFollow()
        {
            if (target == null) return;

            var targetPos = target.position + offset;
            if (xPositionLock)
            {
                targetPos.x = transform.position.x;
            }

            transform.position = Vector3.Lerp(transform.position, targetPos, followSpeed);

            if (isTargetLook)
            {
                transform.LookAt(target);
            }
        }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            Initialize();
        }

        private void LateUpdate()
        {
            SmoothFollow();
        }
    }
}
