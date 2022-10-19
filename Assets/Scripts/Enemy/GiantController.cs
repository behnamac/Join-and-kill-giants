using UnityEngine;
using DG.Tweening;

namespace Enemy
{
    public class GiantController : MonoBehaviour
    {
        [SerializeField] private float maxHealth = 100;
        [SerializeField] private float targetDownPos = -3;
        [SerializeField] private Rigidbody crown;

        private float _currntHealth;
        public bool active { get; private set; }

        private void Awake()
        {
            _currntHealth = maxHealth;
        }

        public void Active(Vector3 posMove,Transform parent)
        {
            transform.SetParent(parent);
            transform.DOLocalMove(posMove, 0.3f);
            active = true;
        }

        public void TakeDamage(float value)
        {
            _currntHealth -= value;
            _currntHealth = Mathf.Clamp(_currntHealth, 0, maxHealth);

            var valueDown = Mathf.Abs((_currntHealth / maxHealth) - 1);
            var targetDown = targetDownPos * valueDown;
            
            var thisTransform = transform;
            var thisPosition = thisTransform.localPosition;

            thisPosition.y = targetDown;
            thisTransform.DOLocalMove(thisPosition,0.1f);

            if (_currntHealth <= 0)
            {
                Dead();
            }
        }

        public void DeadSlise()
        {
            var thisTransform = transform;
            thisTransform.SetParent(null);
            thisTransform.DOMoveY(-10, 1).OnComplete(() =>
            {
                Destroy(gameObject);
            });
        }

        // ReSharper disable Unity.PerformanceAnalysis
        public void Dead()
        {
            crown.transform.SetParent(transform.parent);
            crown.isKinematic = false;
            crown.useGravity = true;
            crown.velocity = Vector3.up * 12;
            crown.angularVelocity = Vector3.right * 20;
            
            if (crown.GetComponent<CrownController>())
            {
                crown.GetComponent<CrownController>().DestroyCrown(2);
            }
            else
            {
                Destroy(crown.gameObject);
            }

            Destroy(gameObject);
        }
    }
}
