using UnityEngine;
public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform Target;
    [SerializeField] private float XSmootingTime = .125f;
    [SerializeField] private float YSmootingTime = .125f;
    [SerializeField] private float ZSmootingTime = .125f;
    private Vector3 Distance;
    // Start is called before the first frame update
    private void Start()
    {
        Distance = -Target.position + transform.position;
        if (Target == null)
            Target = GameObject.FindGameObjectWithTag("Player").transform;
    }
    public void UpdateTarget(Transform t)
    {
        Target = t;
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        //transform.Follow(Target.position + Distance, false, false, true, true, SmootingTime);
        transform.SmoothAxisFollow(Target.position + Distance, XSmootingTime, YSmootingTime, ZSmootingTime);
    }
}
