using System.Collections;
using UnityEngine;

public class IntervalSpawning : MonoBehaviour
{
    [SerializeField] string Tag;
    [SerializeField] float Delay;
    [SerializeField] Transform SpawnPoint;
    // Start is called before the first frame update
    IEnumerator Start()
    {
        yield return new WaitForSeconds(1);
        while (true)
        {
            ObjectPooler.instance.SpawnFromPool(Tag, SpawnPoint.position, Quaternion.identity);
            yield return new WaitForSeconds(Delay);
        }
    }

}
