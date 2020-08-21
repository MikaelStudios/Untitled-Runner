using DG.Tweening;
using System.Collections;
using UnityEngine;

public class IntervaledPopping : MonoBehaviour
{
    [SerializeField] Vector3 endPos;
    [SerializeField] bool x = true;
    [SerializeField] bool y = true;
    [SerializeField] bool z = true;
    [SerializeField] bool add;
    [Space]
    [SerializeField] float delayToStart = 0;
    [SerializeField] float intervalSpawning = 1;
    [SerializeField] float spawnTweenTime = .1f;
    [SerializeField] float delay = 1f;
    Vector3 initialPos;
    // Start is called before the first frame update
    void Start()
    {
        initialPos = transform.localPosition;
        if (add)
            endPos += initialPos;
        if (!x)
            endPos.x = initialPos.x;
        if (!y)
            endPos.y = initialPos.y;
        if (!z)
            endPos.z = initialPos.z;
        StartCoroutine(IntervalPops());
    }

    IEnumerator IntervalPops()
    {
        yield return new WaitForSeconds(delayToStart);
        while (true)
        {
            transform.DOLocalMove(endPos, spawnTweenTime);
            yield return new WaitForSeconds(spawnTweenTime + delay);
            transform.DOLocalMove(initialPos, spawnTweenTime);
            yield return new WaitForSeconds(spawnTweenTime);
            yield return new WaitForSeconds(intervalSpawning);
        }
    }

    private void OnApplicationQuit()
    {
        StopAllCoroutines();
        transform.DOKill();
    }
}

