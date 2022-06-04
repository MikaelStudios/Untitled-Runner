using UnityEngine;

public class PortalTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SpawnManager.instance.NextSet();
            Extensions.Debug("SpawnNew");
        }
    }
}
