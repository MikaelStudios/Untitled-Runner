using DG.Tweening;
using System.Collections;
using UnityEngine;

public class FallingBox : MonoBehaviour
{
    Rigidbody rb;
    Material mat;
    bool ReadyToFall = true;
    public float FallDelay;
    public float timeToRestore = 5f;
    [SerializeField] float pushdownforce = 1.2f;
    Color defaultColor;
    Vector3 pos;
    Quaternion rot;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        mat = GetComponent<MeshRenderer>().material;
        rb.isKinematic = true;
        defaultColor = mat.GetColor("_BaseColor");
        pos = transform.position;
        rot = transform.rotation;
    }

    // Update is called once per frame
    private void OnCollisionEnter(Collision collision)
    {
        if (ReadyToFall)
        {
            StartCoroutine(DelayToFall(collision.contacts[0].point));
        }
    }

    IEnumerator DelayToFall(Vector3 point)
    {
        mat.SetColor("_BaseColor", Color.blue);
        yield return new WaitForSeconds(FallDelay);
        mat.SetColor("_BaseColor", Color.white);
        rb.isKinematic = false;
        rb.AddForceAtPosition(Vector3.down * pushdownforce * Time.deltaTime, point);
        Timer.Register(timeToRestore / 2, () => mat.SetColor("_BaseColor", defaultColor));
        Timer.Register(timeToRestore, () => Restore());
    }

    void Restore()
    {
        rb.isKinematic = true;
        transform.DOMove(pos, 2).SetEase(Ease.OutQuad);
        transform.DORotateQuaternion(rot, 2).SetEase(Ease.OutQuad);
    }

}
