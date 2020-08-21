using DG.Tweening;
using UnityEngine;

public class Teleporter : MonoBehaviour
{
    [SerializeField] Transform ExitPortal;
    [SerializeField] float WarpTime = .1f;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            BeginWarp(other.transform);
        }
    }
    void BeginWarp(Transform t)
    {
        t.DOScale(0, WarpTime).OnComplete(() => EndWarp(t));
    }
    void EndWarp(Transform t)
    {
        t.transform.position = ExitPortal.position.WithX(t.transform.position.x);
        t.DOScale(1, WarpTime);
    }
}
