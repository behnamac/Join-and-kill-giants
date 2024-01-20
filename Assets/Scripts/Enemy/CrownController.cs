using UnityEngine;
using Controllers; // Ensure this is the correct namespace
using System.Collections;

namespace Enemy
{
    public class CrownController : MonoBehaviour
    {
        [SerializeField, Tooltip("The amount of coins this crown gives when collected.")]
        private int coin;

        /// <summary>
        /// Destroys the crown after a specified delay and spawns coins.
        /// </summary>
        /// <param name="delay">Time in seconds before the crown is destroyed.</param>
        public void DestroyCrown(float delay)
        {
            StartCoroutine(DestroyCrownAfterDelay(delay));
        }

        private IEnumerator DestroyCrownAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            SpawnCoin();
        }

        private void SpawnCoin()
        {
            if (UiController.Instance != null)
            {
                UiController.Instance.AddCoin(coin, transform.position);
            }
            Destroy(gameObject);
        }
    }
}
