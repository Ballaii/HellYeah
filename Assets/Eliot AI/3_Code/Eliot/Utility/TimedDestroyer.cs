using System.Collections;
using UnityEngine;

namespace Eliot.Utility
{
    public class TimedDestroyer : MonoBehaviour
    {
        // Destroy Object Time
        public float destroyTimer = 1.0f;

        // Start is called before the first frame update
        void Start()
        {
            StartCoroutine("DestroyGSObject");
        }

        IEnumerator DestroyGSObject()
        {
            yield return new WaitForSeconds(destroyTimer);
            Destroy(gameObject);
        }
    }
}

