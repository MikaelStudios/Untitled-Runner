using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class ChiYaChanEmotion : MonoBehaviour {
	public enum Emotion{Normal,Smile,Crazy,WinkLeft,WinkRight,Blink};

	[SerializeField]private Emotion emotion;
	public bool autoBlink=true;
	public float blinkIntervalMin=0.5f;
	public float blinkIntervalMax=5f;
	public float blinkTime=0.1f;

	[SerializeField]private GameObject eye_blink,eye_normalL,eye_normalR,eye_smileL,eye_smileR;

	private float switchBlinkCountdown;
	private bool isBlinking=false;


	public void SetEmotion(Emotion emotion)
	{
		this.emotion = emotion;
		if(!isBlinking)
			ApplyEmotion (emotion);
	}
	void OnEnable(){
		eye_blink=transform.Find ("eye_blink").gameObject;
		eye_normalL=transform.Find ("eye_normalL").gameObject;
		eye_normalR=transform.Find ("eye_normalR").gameObject;
		eye_smileL=transform.Find ("eye_smileL").gameObject;
		eye_smileR=transform.Find ("eye_smileR").gameObject;
		SetEmotion (Emotion.Normal);
	}
	void OnValidate()
	{
		SetEmotion (emotion);
	}
	void Start()
	{
		//switchBlinkCountdown = Mathf.Exp(Random.Range (Mathf.Log(blinkIntervalMin), Mathf.Log(blinkIntervalMax)));
		switchBlinkCountdown = Random.Range (blinkIntervalMin, blinkIntervalMax);
	}
	void Update()
	{
		if (Application.isPlaying) {
			if (autoBlink) {
				switchBlinkCountdown -= Time.deltaTime;
				if (switchBlinkCountdown <= 0) {
					if (isBlinking) {
						isBlinking = false;
						ApplyEmotion (emotion);
						//switchBlinkCountdown = Mathf.Exp(Random.Range (Mathf.Log(blinkIntervalMin), Mathf.Log(blinkIntervalMax)));
						switchBlinkCountdown = Random.Range (blinkIntervalMin, blinkIntervalMax);
					} else {
						isBlinking = true;
						ApplyEmotion (Emotion.Blink);
						switchBlinkCountdown = blinkTime;
					}
				}
			}
		}
	}
	private void ApplyEmotion(Emotion emotion)
	{
		switch (emotion) {
		case Emotion.Normal:
			eye_blink.SetActive (false);
			eye_normalL.SetActive (true);
			eye_normalR.SetActive (true);
			eye_smileL.SetActive (false);
			eye_smileR.SetActive (false);
			break;
		case Emotion.Blink:
		case Emotion.Smile:
			eye_blink.SetActive (false);
			eye_normalL.SetActive (false);
			eye_normalR.SetActive (false);
			eye_smileL.SetActive (true);
			eye_smileR.SetActive (true);
			break;
		case Emotion.Crazy:
			eye_blink.SetActive (true);
			eye_normalL.SetActive (false);
			eye_normalR.SetActive (false);
			eye_smileL.SetActive (false);
			eye_smileR.SetActive (false);
			break;
		case Emotion.WinkLeft:
			eye_blink.SetActive (false);
			eye_normalL.SetActive (false);
			eye_normalR.SetActive (true);
			eye_smileL.SetActive (true);
			eye_smileR.SetActive (false);
			break;
		case Emotion.WinkRight:
			eye_blink.SetActive (false);
			eye_normalL.SetActive (true);
			eye_normalR.SetActive (false);
			eye_smileL.SetActive (false);
			eye_smileR.SetActive (true);
			break;
		}
	}
}
