using UnityEngine;
using Controllers;

namespace Enemy
{
    public class CrownController : MonoBehaviour
    {
        [SerializeField] private int coin;

        public void DestroyCrown(float delay)
        {
            Invoke(nameof(SpawnCoin), delay);
        }

        private void SpawnCoin()
        {
            UiController.instance.AddCoin(coin, transform.position);
            Destroy(gameObject);
        }
    }
}
