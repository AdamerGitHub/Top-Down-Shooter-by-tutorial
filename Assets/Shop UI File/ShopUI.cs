using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopUI : MonoBehaviour
{
    CurrentGoldUI currentGoldUI;

    public string[] weaponsName;

    GameObject[] allShopItems;
    int[] shopItemsIndex;

    GunController gunController;
    PlayerStats playerStats;
    //int currentGold;

    void Awake()
    {
        playerStats = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();
        //currentGold = playerStats.currentGold;

        gunController = GameObject.FindGameObjectWithTag("Player").GetComponent<GunController>();
        //allShopItems = GameObject.FindGameObjectsWithTag("ShopItemUI");
    }

    void Start()
    {
        //ExitShop();
    }

    public void TryBuyItem(int itemCost, Gun gun)
    {
        if(playerStats.currentGold >= itemCost)
        {
            gunController.EquipGun(gun);
            print(itemCost + ": " + gun + ".");
            playerStats.ChangeCurrentGold(-itemCost);
        }
        else if(playerStats.currentGold < itemCost)
        {
            print("Sry, you poor");
        }
    }

    public void ExitShop()
    {
        gameObject.SetActive(false);
    }
}
