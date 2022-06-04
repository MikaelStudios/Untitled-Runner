using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{
    [SerializeField] ControlTut tutToTrigger;
    [Multiline] [SerializeField] string TutText;
    bool triggered = false;
    private void OnTriggerEnter(Collider other)
    {
        if (other.IsPlayer() && !triggered)
        {
            triggered = true;
            TutorialManager.instance.StartTut(tutToTrigger, TutText);

        }
    }
}
