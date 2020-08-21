using UnityEngine;

public class DragScript : MonoBehaviour
{
    [SerializeField] float speedOfTheBall = 4;

    Rigidbody rb;
    bool isHeldDown = false;
    Camera cam;
    LineRenderer lr;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        cam = Camera.main;
        Input.simulateMouseWithTouches = true;
        tag = "Player";
        lr = GetComponent<LineRenderer>();
        lr.positionCount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out RaycastHit hitInfo, Mathf.Infinity))
        {
            if (hitInfo.collider.CompareTag("Player"))
            {
                isHeldDown = true;
                lr.positionCount = 2;
            }
        }
        if (Input.GetMouseButton(0))
        {
            if (isHeldDown && Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out RaycastHit hitInfo2, Mathf.Infinity))
            {
                Vector3 mp = hitInfo2.point;
                mp.y = transform.position.y;
                lr.SetPosition(0, mp);

                //lr.SetPosition(0, mp);
                lr.SetPosition(1, transform.position);
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            if (isHeldDown && Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out RaycastHit hitInfo3, Mathf.Infinity))
            {
                Vector3 mousePos = hitInfo3.point;
                mousePos.y = transform.position.y;
                Vector3 force = (-transform.position + mousePos).normalized;
                rb.AddForce(force * speedOfTheBall * 100 * Time.fixedDeltaTime, ForceMode.Impulse);
                isHeldDown = false;
                lr.positionCount = 0;
            }
        }
    }
}
