using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CurrentGoldUI : MonoBehaviour
{
    ShopUI shopUI;

    PlayerStats playerStats;
    public TextMeshProUGUI currentGoldTMP;

    void Awake()
    {
        shopUI = GameObject.FindGameObjectWithTag("ShopUI").GetComponent<ShopUI>();
        playerStats = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();
    }

    void Start()
    {
        currentGoldTMP = gameObject.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        ChangeCurrentGoldUI();
    }

    public void ChangeCurrentGoldUI()
    {
        currentGoldTMP.text = playerStats.currentGold.ToString();
    }
}
