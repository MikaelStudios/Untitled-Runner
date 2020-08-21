using System.Collections;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    [Header("RagDoll Death")]
    //[SerializeField] GameObject RagdollObject;
    [SerializeField] string RagHeadName = "head1_head";
    [SerializeField] float RagDollForce;

    Animator anim;
    PlayerController PC;
    GroundCheck GC;

    int h = 0;
    // Start is called before the first frame update
    IEnumerator Start()
    {
        RagDollForce = Random.Range(RagDollForce, RagDollForce + 5);
        yield return null;
        anim = GetComponent<Animator>();
        PC = GetComponent<PlayerController>();
        GC = GetComponentInChildren<GroundCheck>();
        if (anim == null || PC == null)
            enabled = false;

        PC.OnJump += () => anim.SetTrigger("Jump");
        PC.OnJump += () => anim.SetBool("JumpEnd", false);

        PC.OnLanded += Landing;

        PC.OnSlideBegin += () => anim.SetBool("Slide", true);
        PC.OnSlideEnd += () => anim.SetBool("Slide", false);

        PC.OnVault += () => anim.SetTrigger("Vault");
        PC.OnDeath += Death;

        GC.GroundedUpdated += GroundUpdate;
        GameMaster.instance.OnGameStart += () => anim.SetBool("Idle", false);

        PC.OnWin += () => anim.SetTrigger("Win");
        PC.OnWin += () => PC.enabled = false;
        PC.OnWin += () => enabled = false;

        PC.OnWallRun += () => anim.SetBool("WallRun", true);
        PC.OnWallRunEnd += () => anim.SetBool("WallRun", false);

        PC.AirBoost += () => anim.SetTrigger("Boost");
        PC.Stumble += () => anim.SetTrigger("Stumble");
        if (PC.enabled)
            anim.SetBool("Idle", false);
    }
    private void OnEnable()
    {
        if (PC != null && PC.enabled)
            anim.SetBool("Idle", false);
    }
    void Landing(float height)
    {
        if (height < .85f)
            h = 0;
        else if (height > .85f && height < 1.8f)
            h = 1;
        else if (height > 1.8f)
            h = 2;
        anim.SetInteger("Int_Landing", h);
        anim.SetBool("JumpEnd", true);
    }

    void GroundUpdate(bool isGrounded)
    {
        anim.SetBool("Grounded", isGrounded);
    }

    void Death(bool Final)
    {

        GameObject Rag = ObjectPooler.instance.SpawnFromPool("Ragdoll", transform.position, transform.rotation);
        Transform[] RagChild = Rag.GetComponentsInChildren<Transform>(true);
        Transform[] MyChildren = gameObject.GetComponentsInChildren<Transform>(true);
        Rigidbody ragHead = null;
        for (int i = 0; i < RagChild.Length; i++)
        {
            if (string.Equals(RagChild[i].name, MyChildren[i].name))
            {
                RagChild[i].transform.position = MyChildren[i].transform.position;
                RagChild[i].transform.rotation = MyChildren[i].transform.rotation;
                if (string.Equals(RagChild[i].name, RagHeadName))
                    ragHead = RagChild[i].GetComponent<Rigidbody>();
                if (RagChild[i].GetComponent<Rigidbody>() != null)
                    RagChild[i].GetComponent<Rigidbody>().ResetVelocity();
            }
        }
        if (ragHead != null)
        {
            ragHead.ResetVelocity();
            ragHead.AddForce((-transform.forward + (transform.up * Random.Range(.1f, 1))) * 100 * Time.fixedDeltaTime * RagDollForce, ForceMode.Impulse);
        }
        gameObject.SetActive(false);
    }
}
