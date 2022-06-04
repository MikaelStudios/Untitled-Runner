using UnityEngine;
class StateTrigger : MonoBehaviour
{
    [SerializeField] State GameState;
    [SerializeField] string PoolTag = "Hit";
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController PC = other.GetComponent<PlayerController>();
            if (PC.speedBoostDefence && GameState == State.Lose)
            {
                Death();
                return;
            }
            switch (GameState)
            {
                case State.Win:
                    PC.Win();
                    break;
                case State.Lose:
                    PC.Death();
                    break;
            }
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            PlayerController PC = collision.collider.GetComponent<PlayerController>();
            if (PC.speedBoostDefence && GameState == State.Lose)
            {
                Death();
                return;
            }
            switch (GameState)
            {
                case State.Win:
                    PC.Win();
                    break;
                case State.Lose:
                    PC.Death();
                    break;
            }
        }
    }
    void Death()
    {
        ObjectPooler.instance.SpawnFromPool(PoolTag, transform.position, Quaternion.identity);
        gameObject.SetActive(false);
    }
    public enum State
    {
        Win, Lose
    }

}

