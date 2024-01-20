using UnityEngine;

namespace Enemy
{
    public class GiantSliseController : MonoBehaviour
    {
        [SerializeField, Tooltip("Array of Rigidbody objects to be deactivated.")]
        private Rigidbody[] diactiveObjects;

        [SerializeField, Tooltip("Reference to the GiantController component.")]
        private GiantController giant;

        private const string RespawnTag = "Respawn";

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag(RespawnTag))
            {
                if (diactiveObjects != null)
                {
                    foreach (var obj in diactiveObjects)
                    {
                        if (obj != null)
                        {
                            obj.transform.SetParent(null);
                            obj.isKinematic = false;
                            obj.useGravity = true;
                            var collider = obj.GetComponent<Collider>();
                            if (collider != null)
                            {
                                collider.isTrigger = false;
                            }
                        }
                    }
                }

                giant?.DeadSlice();
            }
        }
    }
}
