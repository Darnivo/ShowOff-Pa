using System.Collections;
using Unity.Mathematics;
using UnityEngine;

public class DissolveEffect : MonoBehaviour
{
    private Material objMaterial;
    private Coroutine currentCoroutine;
    public bool startInvisible = false;
    private void Awake()
    {
        objMaterial = GetComponent<Renderer>().material;
        if (!startInvisible) objMaterial.SetFloat("_DissolveAmount", 0f);
    }

    public void dissolveIn(float duration)
    {
        StartDissolve(1f, 0f, duration);

    }
    public void dissolveOut(float duration)
    {
        StartDissolve(0f, 1f, duration);

    }
    private void StartDissolve(float startValue, float endValue, float duration)
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }
        currentCoroutine = StartCoroutine(DissolveCoroutine(startValue, endValue, duration));
    }

    private IEnumerator DissolveCoroutine(float startValue, float endValue, float duration)
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            float value = Mathf.Lerp(startValue, endValue, t);
            objMaterial.SetFloat("_DissolveAmount", value);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        objMaterial.SetFloat("_DissolveAmount", endValue);
    }

    public void callDissolveInAfterDelay(float duration)
    {
        StartCoroutine(dissolveInAfterDelay(0.3f, duration));
    }

    private IEnumerator dissolveInAfterDelay(float delayTime, float duration)
    {
        yield return new WaitForSeconds(delayTime);
        dissolveIn(duration);

    }

}