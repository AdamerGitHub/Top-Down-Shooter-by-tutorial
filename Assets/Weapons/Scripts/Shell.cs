using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shell : MonoBehaviour
{
    public Rigidbody myRigidbody;
    public float forceMin;
    public float forceMax;

    [Header("Red Shell")]
    public bool isHotOnShot;
    public Color red;
    Color initialColdColour;
    Material mat;

    float lifetime = 4f;
    float fadetime = 2f;
    float hotTime = 2f;

    // Start is called before the first frame update

    void Start()
    {
        Destroy(gameObject, lifetime*2);
        mat = GetComponent<Renderer>().material;
        initialColdColour = mat.GetColor("_BaseColor");

        float force = Random.Range(forceMin, forceMax);
        myRigidbody.AddForce(transform.right * force);
        myRigidbody.AddTorque(Random.insideUnitSphere * force);

        StartCoroutine(Fade());
        if (isHotOnShot)
        {
            StartCoroutine(HotFade());
        }
    }
    
    IEnumerator Fade()
    {
        yield return new WaitForSeconds(lifetime);

        float percent = 0;
        float fadeSpeed = 1 / fadetime;
        Color initialColour = mat.color;

        while(percent < 1)
        {
            percent += Time.deltaTime * fadeSpeed;
            mat.SetColor("_BaseColor", Color.Lerp(initialColour, Color.clear, percent));
            yield return null;
        }
    }

    IEnumerator HotFade()
    {
        float percent = 0;
        float coldSpeed = 1 / hotTime;
        Color initialColour = mat.color;
        while (percent < 1)
        {
            percent += Time.deltaTime * coldSpeed;
            mat.SetColor("_BaseColor", Color.Lerp(red, initialColour, percent));
            yield return null;
        }
        yield return true;
    }
}
