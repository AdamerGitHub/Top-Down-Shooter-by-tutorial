using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ItemInShop : MonoBehaviour
{
    public int itemCost;
    public Gun gun;

    ShopUI shopUI;

    void Start()
    {
        shopUI = GameObject.FindGameObjectWithTag("ShopUI").GetComponent<ShopUI>();
        SetCostItem();
    }

    void SetCostItem()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject getChild = transform.GetChild(1).GetChild(i).gameObject;
            TextMeshProUGUI changeCost = getChild.GetComponent<TextMeshProUGUI>();
            changeCost.text = "COST " + itemCost;
        }
    }

    public void OnClickOnButton()
    {
        shopUI.TryBuyItem(itemCost, gun);
    }

}
