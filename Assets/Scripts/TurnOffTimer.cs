using UnityEngine;

public class TurnOffTimer : MonoBehaviour, IPooledObject
{
    [SerializeField] float timerToDespawn = 1.1f;
    Timer t;
    public void OnObjectSpawn()
    {
        t = Timer.Register(timerToDespawn, () => gameObject.SetActive(false));
    }
    private void Update()
    {
        if (transform.position.y < -7.1f)
        {
            Timer.Cancel(t);
            gameObject.SetActive(false);
        }
    }
}
