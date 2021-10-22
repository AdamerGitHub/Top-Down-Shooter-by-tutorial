using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public int startGold;
    [HideInInspector] public int currentGold = 0;

    void Start()
    {
        ChangeCurrentGold(startGold);
    }

    public void ChangeCurrentGold(int plusGold)
    {
        currentGold += plusGold;
    }
}
