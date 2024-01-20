using UnityEngine;
using DG.Tweening;

namespace Enemy
{
    public class GiantController : MonoBehaviour
    {
        [SerializeField, Tooltip("Maximum health of the giant.")]
        private float maxHealth = 100;

        [SerializeField, Tooltip("The position to which the giant moves down when taking damage.")]
        private float targetDownPos = -3;

        [SerializeField, Tooltip("Reference to the crown's Rigidbody.")]
        private Rigidbody crown;

        private float _currentHealth;
        public bool Active { get; private set; }

        private void Awake()
        {
            _currentHealth = maxHealth;
        }

        /// <summary>
        /// Activates the giant and moves it to a specified position.
        /// </summary>
        public void Activate(Vector3 posMove, Transform parent)
        {
            transform.SetParent(parent);
            transform.DOLocalMove(posMove, 0.3f);
            Active = true;
        }

        /// <summary>
        /// Applies damage to the giant and moves it downwards.
        /// </summary>
        public void TakeDamage(float value)
        {
            _currentHealth -= value;
            _currentHealth = Mathf.Clamp(_currentHealth, 0, maxHealth);

            var valueDown = Mathf.Abs((_currentHealth / maxHealth) - 1);
            var targetDown = targetDownPos * valueDown;

            var thisPosition = transform.localPosition;
            thisPosition.y = targetDown;
            transform.DOLocalMove(thisPosition, 0.1f);

            if (_currentHealth <= 0)
            {
                Dead();
            }
        }

        /// <summary>
        /// Handles the giant's slicing death animation.
        /// </summary>
        public void DeadSlice()
        {
            transform.SetParent(null);
            transform.DOMoveY(-10, 1).OnComplete(OnDeathComplete);
        }

        /// <summary>
        /// Handles the giant's death, releasing the crown.
        /// </summary>
        public void Dead()
        {
            if (crown != null)
            {
                crown.transform.SetParent(transform.parent);
                crown.isKinematic = false;
                crown.useGravity = true;
                crown.velocity = Vector3.up * 12;
                crown.angularVelocity = Vector3.right * 20;

                var crownController = crown.GetComponent<CrownController>();
                if (crownController != null)
                {
                    crownController.DestroyCrown(2);
                }
                else
                {
                    Destroy(crown.gameObject);
                }
            }

            Destroy(gameObject);
        }

        private void OnDeathComplete()
        {
            Destroy(gameObject);
        }
    }
}
