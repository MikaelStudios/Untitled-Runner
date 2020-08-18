using System.Collections;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    [Header("RagDoll Death")]
    [SerializeField] GameObject RagdollObject;
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
        if (PC.enabled)
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

    void Death()
    {

        GameObject Rag = Instantiate(RagdollObject, transform.position, Quaternion.identity);
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
            }
        }
        if (ragHead != null)
        {
            ragHead.AddForce(-transform.forward * 100 * Time.fixedDeltaTime * RagDollForce, ForceMode.Impulse);
        }
        gameObject.SetActive(false);
    }
}
