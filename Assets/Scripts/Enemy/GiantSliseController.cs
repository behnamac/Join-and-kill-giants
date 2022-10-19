using UnityEngine;

namespace Enemy
{
    public class GiantSliseController : MonoBehaviour
    {
        [SerializeField] private Rigidbody[] diactiveObjects;
        [SerializeField] private GiantController giant;
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Respawn"))
            {
                for (int i = 0; i < diactiveObjects.Length; i++)
                {
                    diactiveObjects[i].transform.SetParent(null);
                    diactiveObjects[i].isKinematic = false;
                    diactiveObjects[i].useGravity = true;
                    diactiveObjects[i].GetComponent<Collider>().isTrigger = false;
                }
                giant.DeadSlise();
            }
        }
    }
}
