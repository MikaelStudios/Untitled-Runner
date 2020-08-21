using UnityEngine;

public class JumpAndSpeedBoost : MonoBehaviour
{
    [SerializeField] private Vector3 Force;
    [SerializeField] private float ForceMultiplier;
    [SerializeField] private float AddedSpeed;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            bool t = false;
            InputManager.instance.OnTouchBeign += () => t = true;
            if (t)
                other.gameObject.GetComponent<Rigidbody>().ResetVelocity();
            other.gameObject.GetComponent<PlayerController>().SpeedBoost(Force * ForceMultiplier, AddedSpeed);

        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            bool t = false;
            InputManager.instance.OnTap += () => t = true;
        }
    }
}
