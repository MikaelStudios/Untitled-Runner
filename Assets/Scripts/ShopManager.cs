using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class ShopManager : MonoBehaviour
{
    public CharacterBluePrint[] characterArray;
    public Button buyButton;
    public TextMeshProUGUI nameText, speedText, jumpText, climbText;

    public int coins;
    public int currentIndex;
    public Button selectedButton;
    public static ShopManager instance;

    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {

        coins = CoinManager.instance.initialCoins;
        foreach (CharacterBluePrint character in characterArray)
        {

            if (character.Name == "Lulu")
            {
                character.isUnlocked = true;

                Debug.Log(character.isUnlocked);
            }
            else
            {
                character.isUnlocked = PlayerPrefs.GetInt(character.Name, 0) == 0 ? false : true;
                Debug.Log(character.isUnlocked);

            }
        }

    }

    private void LateUpdate()
    {
        currentIndex = GameMaster.instance.selectedCharIndex;
        CharacterBluePrint character = characterArray[currentIndex];
        if (character.isUnlocked)
        {
            selectedButton.gameObject.SetActive(true);

        }

        else
        {
            selectedButton.gameObject.SetActive(false);
        }
    }
    public float climbSpeed;
    public void UpdateUI()
    {
        CharacterBluePrint character = characterArray[currentIndex];

        if (character.isUnlocked)
        {

            buyButton.gameObject.SetActive(false);
        }

        else
        {
            climbSpeed = character.Speed;
            buyButton.gameObject.SetActive(true);
            buyButton.GetComponentInChildren<TextMeshProUGUI>().text = "Buy-" + character.Price;
            nameText.text = character.Name;
            speedText.text = character.Speed.ToString();
            jumpText.text = character.Jump.ToString();
            climbText.text = character.Climb.ToString();
            if (character.Price <= PlayerPrefs.GetInt("NumberOfCoins", coins))
            {
                buyButton.interactable = true;
            }
            else
            {
                buyButton.interactable = false;
            }
        }

    }
    private void Update()
    {
        UpdateUI();
    }
    public void UnLockCharacters()
    {

        CharacterBluePrint character = characterArray[currentIndex];
        PlayerPrefs.SetInt(character.Name, 1);
        PlayerPrefs.SetInt("SelectedChar", currentIndex);
        character.isUnlocked = true;
        PlayerPrefs.SetInt("NumberOfCoins", PlayerPrefs.GetInt("NumberOfCoins", coins) - character.Price);

    }

    public void SelectedVehicles()
    {
        currentIndex = GameMaster.instance.selectedCharIndex;

        GameMaster.instance.Characters[currentIndex].SetActive(true);
        CharacterBluePrint character = characterArray[currentIndex];
        Debug.Log(GameMaster.instance.Characters[GameMaster.instance.selectedCharIndex] + "is selected");




    }


}
