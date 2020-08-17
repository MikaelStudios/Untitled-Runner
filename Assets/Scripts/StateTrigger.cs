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

    public enum State
    {
        Win, Lose
    }

}

