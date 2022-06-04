using DG.Tweening;
using Hellmade.Sound;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioClip BackGroundMusic;
    [SerializeField] AudioMixerGroup BackGroundMixer;
    Audio BG;
    // Start is called before the first frame update
    void Start()
    {
        EazySoundManager.IgnoreDuplicateMusic = true;
        int a = EazySoundManager.PrepareMusic(BackGroundMusic, 1, true, true, .1f, 0);
        BG = EazySoundManager.GetMusicAudio(a);
        if (!BG.IsPlaying)
            BG.Play();
        BG.AddMixerGroup(BackGroundMixer);

        GameMaster.instance.OnHome += () => BG.Stop();
        GameMaster.instance.OnHome += BGmusicHighPass;
        GameMaster.instance.OnResume += BGmusicHighPass;
        GameMaster.instance.OnPause += BGmusicLowPass;

        if (TutorialManager.instance != null)
        {
            TutorialManager.instance.OnTutBegin += BGmusicLowPass;
            TutorialManager.instance.OnTutEnd += BGmusicHighPass;
        }
    }

    private void BGmusicLowPass()
    {
        BackGroundMixer.audioMixer.DOSetFloat("lowPass", 400.00f, .35f).SetUpdate(true);
    }

    private void BGmusicHighPass()
    {
        BackGroundMixer.audioMixer.DOSetFloat("lowPass", 22000.00f, .35f).SetUpdate(true);
    }
}
