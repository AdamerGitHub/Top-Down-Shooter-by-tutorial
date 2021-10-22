using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Gun : MonoBehaviour
{
    public enum FireMode {Auto, Burst, Single, LOL};
    public FireMode fireMode;

    public Transform[] projectileSpawn;                    // Contain an all projectiles transforms.
    public Projectile projectile;                          // Contain a current projectile (Set a projectile in the inspector)
    public float msBetweenShots = 100f;                    // milliseconds between a shot
    public float muzzleVelocity = 35f;                     // a projectile speed/velocity
    public int burstCount;                                 // how many a projectiles need to spawn (When burst mode ON)
    public int projectilesPerMag;                          // how many a projectiles in your magazine
    public float reloadTime = 0.3f;                        // how long time a weapon reload

    [Header("Recoil")]
    public Vector2 kickMinMax = new Vector2(0.5f,0.2f);    // Recoil backforce by a shot array
    public Vector2 recoilAngleMinMax = new Vector2(3f,5f); // Recoild Angle by a shot array
    public float recoilMoveSettleTime = 0.1f;              // FIX ME. how fast a weapon reach initial position after shot
    public float recoilRotationSettleTime = 0.1f;          // FIX ME. how fast a weapon reach initial angle after shot

    [Header("Effects")]
    public Transform shell;
    public Transform[] shellEjection;
    public float maxReloadAngle = 30f;

    [Header("SoundEffects")]
    public AudioSource audioSource;
    public AudioClip fireSound;
    public float fireSoundVolume = .5f;
    public float fireSoundMinPlayTime = 0.79f;

    MuzzleFlash muzzleFlash;

    bool triggerReleasedSinceLastShot;
    int shotsRemainingInBurst;
    int projectilesRemainingInMag;
    bool isReloading;

    Vector3 recoilSmoothDampVelocity;
    float recoilRotSmoothDampVelocity;
    float recoilAngel;

    void Start()
    {
        muzzleFlash = GetComponent<MuzzleFlash>();
        shotsRemainingInBurst = burstCount;
        projectilesRemainingInMag = projectilesPerMag;
    }

    float nextShotTime;

    void LateUpdate()
    {
        //Animate recoil
        transform.localPosition = Vector3.SmoothDamp(transform.localPosition, Vector3.zero, ref recoilSmoothDampVelocity, recoilMoveSettleTime);
        //recoilAngel = Mathf.SmoothDamp(recoilAngel, 0, ref recoilRotSmoothDampVelocity, recoilRotationSettleTime);
        //transform.localEulerAngles = transform.localEulerAngles + Vector3.left * recoilAngel;

        if (!isReloading && projectilesRemainingInMag == 0)
        {
            Reload();
        }
    }

    void PlaySound(AudioClip sound, float volume, float minPlayTime)
    {
        audioSource.PlayOneShot(sound, volume);
    }

    void Shoot()
    {
        if(!isReloading && Time.time > nextShotTime && projectilesRemainingInMag > 0)
        {
            if (fireMode == FireMode.Burst)
            {
                if(shotsRemainingInBurst == 0)
                {
                    return;
                }
                shotsRemainingInBurst--;
            }
            else if(fireMode == FireMode.Single)
            {
                if (!triggerReleasedSinceLastShot)
                {
                    return;
                }
            }

            for (int i = 0; i < projectileSpawn.Length; i++)
            {
                if(projectilesRemainingInMag == 0)
                {
                    break;
                }

                projectilesRemainingInMag--;
                nextShotTime = Time.time + msBetweenShots / 1000;
                Projectile newProjectile = Instantiate(projectile, projectileSpawn[i].position, projectileSpawn[i].rotation);
                newProjectile.SetSpeed(muzzleVelocity);

                //PLAY SOUND

                PlaySound(fireSound, fireSoundVolume, fireSoundMinPlayTime);
            }

            for(int i = 0; i < shellEjection.Length; i++)
            {
                Instantiate(shell, shellEjection[i].position, shellEjection[i].rotation);
            }
            muzzleFlash.Activate();
            transform.localPosition -= Vector3.forward * Random.Range(kickMinMax.x, kickMinMax.y);
            recoilAngel += Random.Range(recoilAngleMinMax.x, recoilAngleMinMax.y);
            recoilAngel = Mathf.Clamp(recoilAngel, 0, 30);
        }
    }

    public void Reload()
    {
        if(!isReloading && projectilesRemainingInMag != projectilesPerMag)
        {
            StartCoroutine(AnimateReload());
        }
    }

    IEnumerator AnimateReload()
    {
        isReloading = true;
        yield return new WaitForSeconds(0.2f);

        float reloadSpeed = 1 / reloadTime;
        float percent = 0;
        Vector3 initialRot = transform.localEulerAngles;

        while (percent < 1)
        {
            percent += Time.deltaTime * reloadSpeed;
            float interpolation = (-Mathf.Pow(percent, 2) + percent) * 4;
            float reloadAngle = Mathf.Lerp(0, maxReloadAngle, interpolation);
            transform.localEulerAngles = initialRot + Vector3.left * reloadAngle;

            yield return null;
        }

        isReloading = false;
        projectilesRemainingInMag = projectilesPerMag;
    }

    public void Aim(Vector3 aimPoint)
    {
        if (!isReloading)
        {
            transform.LookAt(aimPoint);
        }
    }
    
    public void OnTriggerHold()
    {
        Shoot();
        triggerReleasedSinceLastShot = false;
    }

    public void OnTriggerRelease()
    {
        triggerReleasedSinceLastShot = true;
        shotsRemainingInBurst = burstCount;
    }
}
