using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scaler : MonoBehaviour
{
    private Vector3 normalScale;
    public float scaleTime = 1f;

    void Awake()
    {
        normalScale = transform.localScale;
    }

    IEnumerator Scale(float newScale)
    {
        Vector3 endScale = normalScale * newScale;
        Vector3 startScale = transform.localScale;
        float lerpVal = 0;
        while(lerpVal < 1)
        {
            transform.localScale = Vector3.Lerp(startScale, endScale, lerpVal);
            lerpVal += Time.deltaTime / scaleTime;
            yield return null;
        }
        transform.localScale = endScale;
    }

    public void StartScale(float scale)
    {
        StopCoroutine("Scale");
        StartCoroutine(Scale(scale));
    }

}
