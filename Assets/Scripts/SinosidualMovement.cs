using UnityEngine;

public class SinosidualMovement : MonoBehaviour
{
    [SerializeField] float baseSpeed;
    [SerializeField] float ampitude = 1;
    [SerializeField] Vector3 direction;
    [SerializeField] bool isLocal = false;
    private void Start()
    {
        if (baseSpeed == 0)
            baseSpeed = Random.Range(-4.1f, 4.1f);
    }
    // Update is called once per frame
    void Update()
    {
        if (isLocal)
            transform.localPosition += direction * Mathf.Sin(Time.realtimeSinceStartup * ampitude) * Time.deltaTime * baseSpeed;
        else
            transform.position += direction * Mathf.Sin(Time.realtimeSinceStartup * ampitude) * Time.deltaTime * baseSpeed;

    }
}
