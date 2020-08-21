using UnityEngine;

public class BallRolling : MonoBehaviour, IPooledObject
{
    [SerializeField] private Vector3 RotateSpeed;
    [SerializeField] private Vector3 rayOffset;
    [SerializeField] private float rayLength;
    [SerializeField] private float ForwardSpeed;
    PlayerController player;
    bool stop = false;
    Rigidbody rb;
    bool isBlack;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        //ForwardSpeed = Random.Range(ForwardSpeed, ForwardSpeed + 5);
        RotateSpeed *= ForwardSpeed * 20;
    }

    private void OnEnable()
    {
        isBlack = false;
        stop = false;
    }
    void FixedUpdate()
    {
        if (Physics.Raycast(transform.position + rayOffset, -Vector3.forward, out RaycastHit hitInfo, rayLength))
        {
            if (!hitInfo.collider.CompareTag("Player"))
            {
                if (isBlack)
                {
                    hitInfo.collider.gameObject.SetActive(false);
                    ObjectPooler.instance.SpawnFromPool("Explosion", transform.position + new Vector3(0, 1, 2), Quaternion.identity);
                }
                else
                    stop = true;
            }
            else
            {
                stop = false;
            }
        }
        else
        {
            if (stop == false)
            {
                transform.Rotate(RotateSpeed * Time.fixedDeltaTime, Space.Self);
                rb.MovePosition(transform.position + -Vector3.forward * ForwardSpeed * Time.deltaTime);
            }
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position + rayOffset, -Vector3.forward * rayLength);
    }

    public void OnObjectSpawn()
    {
        if (player == null)
        {
            player = GameMaster.instance.m_pc;
            player.OnDeath += SetActive;
        }
        GetComponent<Rigidbody>().ResetVelocity();
    }
    void SetActive(bool tr)
    {
        Timer.Register(player.RespawnTime - .2f, () => gameObject?.SetActive(false));
    }

}
