using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TextTrigger : MonoBehaviour
{
    public TextMeshPro text;
    public float fadeDuration = 0.2f;
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
            fadeInText();
        }
    }

    private void fadeInText()
    {
        float elapsedTime = 0f;
        Color c = text.color;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(0, 1, elapsedTime / fadeDuration);
            c.a = alpha;
            text.color = c; 
        }
    }
}