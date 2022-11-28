using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeTreeColor : MonoBehaviour
{
    // 6D9AAE // Dark
    // FFFFFF // Light
    private Color light = new Color(1f, 1f, 1f);
    private Color dark = new Color(109/255f, 154/255f, 174/255f);

    private bool isDark = false;
    private float colorDuration = 0.5f;

    private SpriteRenderer spriteRenderer;

    private void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        isDark = true;
        spriteRenderer.color = dark;
    }


    public void SetColorLight(){
        if(isDark){
            isDark = false;
            // spriteRenderer.color = light;
            StartCoroutine(MakeLight(spriteRenderer));
        }
    }

    public void SetColorDark(){
        if(!isDark){
            isDark = true;
            // spriteRenderer.color = dark;
            StartCoroutine(MakeDark(spriteRenderer));
        }
    }

    
    IEnumerator MakeDark(SpriteRenderer sr)
    {
        float t = 0;
        while (t < colorDuration){
            t += Time.deltaTime;
            sr.color = Color.Lerp(light, dark, t / colorDuration);
            yield return null;
        }
    }
    
    IEnumerator MakeLight(SpriteRenderer sr)
    {
        float t = 0;
        while (t < colorDuration){
            t += Time.deltaTime;
            sr.color = Color.Lerp(dark, light, t / colorDuration);
            yield return null;
        }
    }
}
