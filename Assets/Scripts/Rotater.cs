using UnityEngine;

public class Rotater : MonoBehaviour
{

    public enum ROTATIONAXIS
    {
        xAxis,
        yAxis,
        zAxis
    }

    public float speed;

    public ROTATIONAXIS rotationAxis;

    private void Start()
    {
        speed *= 100;
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if (rotationAxis == ROTATIONAXIS.xAxis)
        {
            transform.Rotate(new Vector3(1, 0, 0) * Time.fixedDeltaTime * speed);
        }

        else if (rotationAxis == ROTATIONAXIS.yAxis)
        {
            transform.Rotate(new Vector3(0, 1, 0) * Time.fixedDeltaTime * speed);
        }

        else if (rotationAxis == ROTATIONAXIS.zAxis)
        {
            transform.Rotate(new Vector3(0, 0, 1) * Time.fixedDeltaTime * speed);
        }
    }
}
