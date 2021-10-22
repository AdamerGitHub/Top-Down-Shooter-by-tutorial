using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBarrier : MonoBehaviour
{
    public Vector3 scaleEnd;

    Vector3 scaleChange;

    void Awake()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, scaleEnd, Time.deltaTime * 5);
    }
}
