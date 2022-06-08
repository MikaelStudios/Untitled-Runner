using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class ShopManager : MonoBehaviour
{
    public CharacterScript[] characterArray;
    public Button buyButton;
    public TextMeshProUGUI nameText, speedText, jumpText, climbText;
    int selectedCharIndex = 0;
    bool startCharSelect = false;
    int currentIndex;

    // Start is called before the first frame update
    void Start()
    {
        foreach (CharacterScript character in characterArray)
        {

            if (character.name == " Generic Girl")
            {
                character.isUnlocked = true;
            }
            else
            {
                character.isUnlocked = PlayerPrefs.GetInt(character.name, 0) == 0 ? false : true;
            }
        }

    }


    public void UnlockCharacter()
    {

        CharacterScript character =characterArray[GameMaster.instance.selectedCharIndex];

        PlayerPrefs.SetInt(character.name, 2);

        PlayerPrefs.SetInt("SelectedChar", GameMaster.instance.selectedCharIndex);


    }

    public void SelectedCharacter()
    {
        currentIndex = GameMaster.instance.selectedCharIndex;
        foreach (GameObject character in GameMaster.instance.Characters)
            character.SetActive(false);
        GameMaster.instance.Characters[GameMaster.instance.selectedCharIndex].SetActive(true);
    }
    // Update is called once per frame
    void Update()
    {
        UpdateUI();
    }

    public void UpdateUI()
    {
        CharacterScript character = characterArray[GameMaster.instance.selectedCharIndex];
        if(character.isUnlocked)
        {

            buyButton.gameObject.SetActive(false);

        }
        else
        {
            buyButton.gameObject.SetActive(true);
         //   buyButton.GetComponentInChildren<TextMeshProUGUI>().text = "Buy-" + character.Price;
            nameText.text = character.Name;
            speedText.text = character.speed.ToString();
            jumpText.text = character.jumpHeight.ToString();
            climbText.text = character.climbingSpeed.ToString();
        }
    }
    
}
