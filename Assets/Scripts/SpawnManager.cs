using DG.Tweening;
using System.Collections;
using UnityEngine;
public class SpawnManager : MonoBehaviour
{
    public GameObject Portal;
    public Tracks[] availableFields;
    Track[] avaiableTracksOfCurrDiff;
    public Difficulty currentDifficulty = Difficulty.Easy;

    [Header("Sky Box Settings")]
    [SerializeField] Material SkyBox;
    [SerializeField] float ColorChange = 2;

    int numberNeededToSpawn = 0;
    Track lastTrack;
    GameObject currentPortal;


    [Header("Debug Property")]
    [SerializeField] Vector3 startPos = Vector3.zero;
    int CurrentTrack = 0;
    public static SpawnManager instance;
    Light Sun;

    public Vector3 StartPos { get => startPos; set => startPos = value; }

    private void Awake()
    {
        instance = this;
    }
    IEnumerator Start()
    {

        if (availableFields.Length == 0)
            yield break;
        availableFields.Shuffle();
        Tracks CurrentTerrain = availableFields[CurrentTrack];
        CurrentTrack++;

        SkyBox.DOColor(CurrentTerrain.sky_DownColor, "_Bottom", 0);
        SkyBox.DOColor(CurrentTerrain.sky_UpColor, "_Top", 0);
        Sun = GameObject.Find("Directional Light").GetComponent<Light>();
        Sun.DOColor(CurrentTerrain.sky_DownColor, 0);

        if (CurrentTrack > availableFields.Length - 1)
            CurrentTrack = 0;
        avaiableTracksOfCurrDiff = CurrentTerrain.GetTracksOfDifficulty(currentDifficulty);
        numberNeededToSpawn = Random.Range(3, avaiableTracksOfCurrDiff.Length);
        avaiableTracksOfCurrDiff.Shuffle();
        yield return null;
        for (int i = 0; i < 3; i++)
        {
            GameObject t = Instantiate(avaiableTracksOfCurrDiff[i].m_track);
            numberNeededToSpawn--;
            t.transform.position = startPos;
            if (t.GetComponent<TrackScript>() == null)
                t.AddComponent<TrackScript>();
            if (i <= 1)
                startPos = t.GetComponent<TrackScript>().GetNextPos(avaiableTracksOfCurrDiff[i + 1].m_track);
            else
                startPos = t.GetComponent<TrackScript>().GetNextPos(Portal, true);
        }
        //avaiableTracksOfCurrDiff.
        //for (int i = 0; i < NumberToSpawn; i++)
        //{
        //    Instantiate(Terrain, Vector3.zero + new Vector3(0, 0, i * Zoffset), Quaternion.Euler(0, 90, 0));
        //}
    }
    public void NextObject()
    {
        if (numberNeededToSpawn <= 0)
        {
            currentPortal = Instantiate(Portal);
            currentPortal.transform.position = startPos;
            currentPortal.GetComponent<TrackScript>().GameStartTrue();
            numberNeededToSpawn = 1;
        }
    }
    public void NextSet()
    {

        if (CurrentTrack > availableFields.Length - 1)
        {
            availableFields.Shuffle();
            CurrentTrack = 0;
        }

        Tracks CurrentTerrain = availableFields[CurrentTrack];
        SkyBox.DOColor(CurrentTerrain.sky_DownColor, "_Bottom", ColorChange);
        SkyBox.DOColor(CurrentTerrain.sky_UpColor, "_Top", ColorChange);
        Sun.DOColor(CurrentTerrain.sky_DownColor, ColorChange);
        CurrentTrack++;

        avaiableTracksOfCurrDiff = CurrentTerrain.GetTracksOfDifficulty(currentDifficulty);
        numberNeededToSpawn = Random.Range(3, avaiableTracksOfCurrDiff.Length);
        avaiableTracksOfCurrDiff.Shuffle();

        for (int i = 0; i < 3; i++)
        {
            GameObject t = Instantiate(avaiableTracksOfCurrDiff[i].m_track);
            numberNeededToSpawn--;
            if (startPos == currentPortal.transform.position && i <= 1)
                startPos = currentPortal.GetComponent<TrackScript>().GetNextPos(t, true);
            t.transform.position = startPos;
            if (t.GetComponent<TrackScript>() == null)
                t.AddComponent<TrackScript>().GameStartTrue();
            else
                t.GetComponent<TrackScript>().GameStartTrue();
            if (i <= 1)
                startPos = t.GetComponent<TrackScript>().GetNextPos(avaiableTracksOfCurrDiff[i + 1].m_track);
            else
                startPos = t.GetComponent<TrackScript>().GetNextPos(Portal, true);
        }
    }
}
