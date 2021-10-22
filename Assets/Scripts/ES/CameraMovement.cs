using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    // Получает игрока с тегом "Player"
    private Transform player;
    private Vector3 offset; // Игрок.позиция - Камера.позиция = смещение

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        offset = player.position - transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (player != null)
        {
            transform.position = player.position - offset;
        }
    }
}
