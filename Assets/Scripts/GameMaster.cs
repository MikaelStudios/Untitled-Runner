﻿using DG.Tweening;
using Lean.Gui;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameMaster : MonoBehaviour
{
    [Header("INTRO CUTSCENE SETTINGS")]
    [SerializeField] Camera CutsceneCamera;
    [SerializeField] float CameraTransitDuration;
    [SerializeField] Ease TransitEase;
    [SerializeField] Text CountDownText;
    float TimeToCountdown;

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

        if (CutsceneCamera != null)
            C_CameraOffset = CutsceneCamera.transform.position - player.position;
        else
            isTransiting = false;
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
        CountDownSeq();
    }

    void CountDownSeq()
    {
        StartCoroutine(StartRace());
    }

    IEnumerator StartRace()
    {
        CountDownText.gameObject.SetActive(true);
        for (int i = 3; i >= 0; i--)
        {
            if (i == 0)
                CountDownText.text = "<size=100>GO!!!</size>";
            else
                CountDownText.text = i.ToString();
            CountDownText.rectTransform.DOScale(0, 0);
            CountDownText.rectTransform.DOScale(1, .7f).SetEase(Ease.OutBounce);
            yield return new WaitForSeconds(1);
        }
        CountDownText.gameObject.SetActive(false);
    }

    void Victory()
    {
        if (CutsceneCamera != null)
        {
            CutsceneCamera.transform.position = mainCamera.transform.position;
            isTransiting = true;
            CutsceneCamera.gameObject.SetActive(true);
            Vector3 newPos = player.position + C_CameraOffset;
            CutsceneCamera.transform.DOMove(newPos, WinTransitTime);
        }
        Timer.Register(WinTransitTime + DelayToPopUp, () => m_winWindow.TurnOn());
    }

    private void LateUpdate()
    {
        if (isTransiting)
            CutsceneCamera.transform.LookAt(player);
    }
    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
