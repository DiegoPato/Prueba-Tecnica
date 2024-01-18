using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIhud : MonoBehaviour
{
    [SerializeField] private CollectibleManager collectable;
    [SerializeField] private TextMeshProUGUI coinsText;
    [SerializeField] private TextMeshProUGUI winText;

    private int Coin = 0;
    private int maxCoins = 3;

    void Awake()
    {
        collectable = GameObject.Find("Player").GetComponent<CollectibleManager>();
    }

    void OnEnable()
    {
        collectable.onCoinCollected += addCoin;
    }

    void OnDisable()
    {
        collectable.onCoinCollected -= addCoin;
    }

    void addCoin()
    {
        Coin++;
        if (Coin >= maxCoins)
        {
            coinsText.gameObject.SetActive(false);
            winText.gameObject.SetActive(true);
        }
        else
        {
            coinsText.text = "Coins: " + Coin.ToString();
        }
    }
}
