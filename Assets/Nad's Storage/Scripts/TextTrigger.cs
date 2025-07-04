using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TextTrigger : MonoBehaviour
{
    public TextMeshPro text;
    public float fadeDuration = 0.2f;
    private bool isOn = false; 
    void Awake()
    {
        Color c = text.color;
        c.a = 0;
        text.color = c;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("player enter"); 
            if(isOn == false) StartCoroutine(fadeInText());
        }
    }

    private IEnumerator fadeInText()
    {
        float elapsedTime = 0f;
        Color c = text.color;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsedTime / fadeDuration);
            c.a = alpha;
            text.color = c;
            yield return null;
        }
        c.a = 1;
        text.color = c;
        isOn = true; 
    }
}