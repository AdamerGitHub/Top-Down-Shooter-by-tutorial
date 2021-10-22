using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShield : MonoBehaviour
{
    public GameObject barrier;

    LivingEntity barrierEntity;
    public float timeBetweenSpawnBarrier;
    float timer;
    bool barrierON = false;

    void Update()
    {
        if (timer <= timeBetweenSpawnBarrier && !barrierON)
        {
            timer += Time.deltaTime;
        }
        else if (timer >= timeBetweenSpawnBarrier && !barrierON)
        {
            barrierON = true;

            GameObject newBarrier = Instantiate(barrier, transform.position, transform.rotation);
            barrierEntity = newBarrier.GetComponent<LivingEntity>();

            barrierEntity.OnDeath += RebuildBarrier;
            newBarrier.transform.parent = transform;
        }
    }

    void RebuildBarrier()
    {
        timer = 0f;
        barrierON = false;
    }
}
