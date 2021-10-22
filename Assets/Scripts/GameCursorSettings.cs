using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCursorSettings : MonoBehaviour
{
    GameObject crosshairsGM;

    bool checkShopUI;

    void Start()
    {
        crosshairsGM = GameObject.FindGameObjectWithTag("Crosshairs");
        Cursor.visible = false;
    }

    void Update()
    {
        IsCursorOverUI();
    }

    // Player.script use this path of the scipt
    public bool IsMouseOverUI()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }

    public void IsCursorOverUI()
    {
        if (IsMouseOverUI())
        {
            Cursor.visible = true;
            crosshairsGM.SetActive(false);
        }
        else
        {
            crosshairsGM.SetActive(true);
            Cursor.visible = false;
        }
    }
}