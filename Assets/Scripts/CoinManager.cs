using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class CoinManager : MonoBehaviour
{

    public TextMeshProUGUI coinText;
    public int initialCoins;
    public static CoinManager instance;

    public void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        initialCoins = PlayerPrefs.GetInt("NumberOfCoins",0);

        UpdateCoin(initialCoins);
    }


    public void AddCoin(int coin)
    {
        initialCoins += coin;
        SaveCoin(initialCoins);
        UpdateCoin(initialCoins);
    }

    public void RemoveCoin(int coin)
    {
        initialCoins -= coin;
        SaveCoin(initialCoins);
        UpdateCoin(initialCoins);
    }
    public void SaveCoin(int coin)
    {
        PlayerPrefs.SetInt("NumberOfCoins", coin);
    }

    public void UpdateCoin(int coin)
    {
        coinText.text = "Coin: "+coin.ToString();
    }
    // Update is called once per frame
    
}
