using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlashingText : MonoBehaviour
{
    private Text text;
    [Range(0.1f, 1)]
    public float flashSpeed = .5f;

    private void Awake()
    {
        text = GetComponent<Text>();
        //StartCoroutine(FlashText());
    }


    IEnumerator FlashText()
    {
        while (true)
        {
            text.enabled = !text.enabled;
            yield return new WaitForSeconds(flashSpeed);
        }
    }

    private void OnEnable()
    {
        StartCoroutine(FlashText());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

}
