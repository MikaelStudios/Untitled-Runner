using DG.Tweening;
using Hellmade.Sound;
using Lean.Gui;
using System;
using System.Collections;
using System.Collections.Generic;
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

    [Header("LOSE CRASHES PARAMETERS")]
    [SerializeField] float delayToLosePopUp = 2;

    [Header("VICTORY CUTSCENE SETTINGS")]
    [SerializeField] float WinTransitTime;
    [SerializeField] float DelayToPopUp = 1.5f;

    [Header("WINDOWS")]
    [SerializeField] LeanWindow m_loseWindow;
    [SerializeField] LeanWindow m_winWindow;
    [SerializeField] GameObject HomeScreen;

    [Header("SOUND")]
    [SerializeField] Toggle Sound;
    [SerializeField] AudioClip[] Sound_Count;
    [SerializeField] AudioClip Go;

    [Header("CHARACTER SELECT")]
    public GameObject[] Characters;
    public CharacterBluePrint[] characterArray;
    [SerializeField] float TweenTime;
    List<GameObject> instantiedCharacters = new List<GameObject>();
    public int selectedCharIndex = 0;
    bool startCharSelect = false;
    public bool gameStart=false;
    public bool countDown = false;
    public event Action OnGameStart = delegate { };
    public event Action OnHome = delegate { };
    public event Action OnPause = delegate { };
    public event Action OnResume = delegate { };
    public static GameMaster instance;

    Camera mainCamera;
    Transform player;
    bool isTransiting = true;
    public PlayerController m_pc;
    Vector3 C_CameraOffset;
    Vector3 orgStartPos;
    static bool Home = true;

    private void Awake()
    {
        instance = this;

    }

    private void Start()
    {
        mainCamera = Camera.main;
        selectedCharIndex = PlayerPrefs.GetInt("SelectedChar", 0);
        orgStartPos = SpawnManager.instance.StartPos;
        player = Instantiate(Characters[selectedCharIndex], orgStartPos, Quaternion.identity).transform;
        m_pc = player.GetComponent<PlayerController>();

        OnGameStart += () => m_pc.enabled = true;

        m_pc.OnDeath += Death;

        m_pc.OnWin += Victory;

        if (CutsceneCamera != null)
            C_CameraOffset = CutsceneCamera.transform.position - player.position;
        else
            isTransiting = false;

        if (!Home)
        {
            CutsceneCamera.gameObject.SetActive(false);
            isTransiting = false;
            OnGameStart();
            HomeScreen.SetActive(false);
        }

        // UI STUFF
        if (Sound != null)
        {
            Sound.isOn = PlayerPrefs.GetInt("SoundSet", 1) == 0 ? true : false;
            TurnSound(Sound);
        }
    }

    public void Intro()
    {
        Timer.Register(.2f, () => IntroCamerasTransition());

        if (startCharSelect)
        {
            for (int i = 0; i < instantiedCharacters.Count; i++)
            {
                if (i != selectedCharIndex)
                {
                    instantiedCharacters[i].SetActive(false);
                }
                else
                {
                    instantiedCharacters[i].GetComponent<Rigidbody>().isKinematic = false;
                    instantiedCharacters[i].transform.SetPositionZ(-10.5f);
                }
            }
            PlayerPrefs.SetInt("SelectedChar", selectedCharIndex);
        }
        m_pc = player.GetComponent<PlayerController>();
        Camera.main.GetComponent<CameraFollow>().UpdateTarget(player);
    }

    void IntroCamerasTransition()
    {
        if (startCharSelect)
            isTransiting = true;
        CutsceneCamera.transform.DOMove(mainCamera.transform.position, CameraTransitDuration).SetEase(TransitEase);
        //CutsceneCamera.transform.DOLookAt(player.position, CameraTransitDuration);
        Timer.Register(CameraTransitDuration + .1f, () => CutsceneCamera.gameObject.SetActive(false));
        Timer.Register(CameraTransitDuration / 2, () => isTransiting = false);
        Timer.Register(CameraTransitDuration / 2, () => CutsceneCamera.transform.DORotateQuaternion(mainCamera.transform.rotation, CameraTransitDuration / 2).SetEase(TransitEase));
        Timer.Register(CameraTransitDuration + .25f, () => OnGameStart());
        CountDownSeq();
    }

    public void StartCharacterSelect()
    {
        startCharSelect = true;
        isTransiting = false;
        float z = -9;
        orgStartPos = player.position;
        for (int i = 0; i < Characters.Length; i++)
        {
            if (i != selectedCharIndex)
            {
                instantiedCharacters.Add(Instantiate(Characters[i], orgStartPos.WithZ(i < selectedCharIndex ? -12.5f : z), Quaternion.identity));
                instantiedCharacters[i].GetComponent<Rigidbody>().isKinematic = true;
            }
            else
                instantiedCharacters.Add(player.gameObject);

        }

    }

    public void Previous()
    {
        if (selectedCharIndex == 0)
            return;
        selectedCharIndex--;
        instantiedCharacters[selectedCharIndex].GetComponent<Rigidbody>().isKinematic = true;
        instantiedCharacters[selectedCharIndex].transform.DOMove(orgStartPos, TweenTime).OnComplete(() => instantiedCharacters[selectedCharIndex].GetComponent<Rigidbody>().isKinematic = false);
        instantiedCharacters[selectedCharIndex + 1].transform.DOMove(orgStartPos.WithZ(-9f), TweenTime);

        instantiedCharacters[selectedCharIndex + 1].GetComponent<Rigidbody>().isKinematic = true;
        player = instantiedCharacters[selectedCharIndex].transform;

    }
    public void Next()
    {
        if (selectedCharIndex == Characters.Length - 1)
            return;
        selectedCharIndex++;
        instantiedCharacters[selectedCharIndex].transform.DOMove(orgStartPos, TweenTime).OnComplete(() => instantiedCharacters[selectedCharIndex].GetComponent<Rigidbody>().isKinematic = false);

        instantiedCharacters[selectedCharIndex - 1].transform.DOMove(orgStartPos.WithZ(-12.5f), TweenTime);
        instantiedCharacters[selectedCharIndex - 1].GetComponent<Rigidbody>().isKinematic = true;

        player = instantiedCharacters[selectedCharIndex].transform;
    }
    void CountDownSeq()
    {
        StartCoroutine(StartRace());
    }

    IEnumerator StartRace()
    {
        
        CountDownText.gameObject.SetActive(true);
        countDown = true;
        for (int i = 3; i >= 0; i--)
        {
            if (i == 0)
            {
                CountDownText.text = "<size=100>GO!!!</size>";
                EazySoundManager.PlaySound(Go);
            }
            else
            {
                CountDownText.text = i.ToString();
                EazySoundManager.PlaySound(Sound_Count[i - 1]);
            }
            CountDownText.rectTransform.DOScale(0, 0);
            CountDownText.rectTransform.DOScale(1, .7f).SetEase(Ease.OutBounce);
            yield return new WaitForSeconds(1);
        }
        gameStart = true;
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
    void Death(bool Final)
    {
        if (Final)
            Timer.Register(delayToLosePopUp, () => m_loseWindow.TurnOn());
    }

    private void LateUpdate()
    {
        if (isTransiting)
            CutsceneCamera.transform.LookAt(player);
    }

    public void Restart()
    {
        Home = false;
        Time.timeScale = 1;
        OnResume();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void HomeButton()
    {
        Home = true;
        OnHome();

        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Pause()
    {
        OnPause();
        Time.timeScale = 0;
    }

    public void Resume()
    {
        Time.timeScale = 1;
        OnResume();
    }

    public void TurnSound(Toggle t)
    {
        if (t.isOn)
        {
            EazySoundManager.GlobalMusicVolume = 0;
            EazySoundManager.GlobalSoundsVolume = 0;
            PlayerPrefs.SetInt("SoundSet", 0);
        }
        else
        {
            TurnOnSound();
            PlayerPrefs.SetInt("SoundSet", 1);
        }
    }

    public void TurnOnSound()
    {
        EazySoundManager.GlobalMusicVolume = 1;
        EazySoundManager.GlobalSoundsVolume = 1;
    }
}
