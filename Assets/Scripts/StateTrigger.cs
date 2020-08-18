using UnityEngine;
class StateTrigger : MonoBehaviour
{
    [SerializeField] State GameState;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController PC = other.GetComponent<PlayerController>();
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

    public enum State
    {
        Win, Lose
    }

}

