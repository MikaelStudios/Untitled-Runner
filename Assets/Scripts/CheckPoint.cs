using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerController>().AddCheckPoint(new Vector3(other.transform.position.x, transform.position.y + 1, transform.position.z - 2));
        }
    }
}
