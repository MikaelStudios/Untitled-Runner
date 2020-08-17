using DG.Tweening;
using Lean.Gui;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMaster : MonoBehaviour
{
    [Header("INTRO CUTSCENE SETTINGS")]
    [SerializeField] Camera CutsceneCamera;
    [SerializeField] float CameraTransitDuration;
    [SerializeField] Ease TransitEase;

    [Header("VICTORY CUTSCENE SETTINGS")]
    [SerializeField] float WinTransitTime;
    [SerializeField] float DelayToPopUp = 1.5f;

    [Header("WINDOWS")]
    [SerializeField] LeanWindow m_loseWindow;
    [SerializeField] LeanWindow m_winWindow;

    public event Action OnGameStart = delegate { };
    public static GameMaster instance;

    Camera mainCamera;
    Transform player;
    bool isTransiting = true;
    PlayerController m_pc;
    Vector3 C_CameraOffset;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        mainCamera = Camera.main;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        m_pc = player.GetComponent<PlayerController>();

        OnGameStart += () => m_pc.enabled = true;
        m_pc.OnDeath += () => Timer.Register(2, () => m_loseWindow.TurnOn());
        m_pc.OnWin += Victory;
        C_CameraOffset = CutsceneCamera.transform.position - player.position;
    }

    public void Intro()
    {
        Timer.Register(.2f, () => IntroCamerasTransition());
    }

    void IntroCamerasTransition()
    {
        CutsceneCamera.transform.DOMove(mainCamera.transform.position, CameraTransitDuration).SetEase(TransitEase);
        //CutsceneCamera.transform.DOLookAt(player.position, CameraTransitDuration);
        Timer.Register(CameraTransitDuration + .1f, () => CutsceneCamera.gameObject.SetActive(false));
        Timer.Register(CameraTransitDuration / 2, () => isTransiting = false);
        Timer.Register(CameraTransitDuration / 2, () => CutsceneCamera.transform.DORotateQuaternion(mainCamera.transform.rotation, CameraTransitDuration / 2).SetEase(TransitEase));
        Timer.Register(CameraTransitDuration + .25f, () => OnGameStart());
    }

    void Victory()
    {
        CutsceneCamera.transform.position = mainCamera.transform.position;
        isTransiting = true;
        CutsceneCamera.gameObject.SetActive(true);
        Vector3 newPos = player.position + C_CameraOffset;
        CutsceneCamera.transform.DOMove(newPos, WinTransitTime);
        Timer.Register(WinTransitTime + DelayToPopUp, () => m_winWindow.TurnOn());
    }

    private void LateUpdate()
    {
        if (isTransiting)
            CutsceneCamera.transform.LookAt(player);
    }
    public void Restart()
    {
        SceneManager.LoadScene(0);
    }
}
