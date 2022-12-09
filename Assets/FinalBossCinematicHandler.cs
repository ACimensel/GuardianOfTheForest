using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FinalBossCinematicHandler : MonoBehaviour
{
    [SerializeField] GameObject guardian;
    [SerializeField] GameObject guardianSleep;
    [SerializeField] GameObject guardianCamera;
    [SerializeField] GameObject boss;
    [SerializeField] GameObject bossIdle;
    [SerializeField] GameObject cinematic;
    [SerializeField] Animator treeAnim;
    [SerializeField] float cinematicWaitTime = 2f;
    [SerializeField] Image im;

    private PersistantData PD;
    private Animator bossAnim;
    private bool fadeOut = false;

    private void Awake() {
        PD = GameObject.Find("PersistantData").GetComponent<PersistantData>();
        bossAnim = cinematic.GetComponent<Animator>();
    }

    void Start()
    {
        if(!PD.finalBossCinematicPlayed){
            boss.SetActive(false);
            guardian.GetComponent<Guardian>().Freeze();
            StartCoroutine("WaitForCinematic");
        }
    }

    void Update()
    {
        if (cinematic && bossAnim.GetCurrentAnimatorStateInfo(0).IsName("bosscinem") && bossAnim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f){
            Destroy(cinematic); 

            PD.finalBossCinematicPlayed = true;
            boss.SetActive(true);

            guardian.GetComponent<SpriteRenderer>().enabled = true;
            Guardian gScr = guardian.GetComponent<Guardian>();
            gScr.isFrozen = false;
            gScr.isMovementEnabled = true;
        }
        if(fadeOut){
            fadeOut = false;
            StartCoroutine("FadeOut");
        }
    }
    
    public void EndGame()
    {
        Debug.Log("END GAME");
        StartCoroutine("FinalScene");
    }

    IEnumerator WaitForCinematic()
    {
        yield return new WaitForSeconds(cinematicWaitTime);

        guardian.GetComponent<SpriteRenderer>().enabled = false;
        bossIdle.SetActive(false);
        cinematic.SetActive(true);
    }

    IEnumerator FinalScene()
    {
        guardian.GetComponent<Guardian>().Freeze();
        guardian.GetComponent<Guardian>().enabled = false;
        treeAnim.enabled = true;
        yield return new WaitForSeconds(2f);

        guardianSleep.transform.position = guardian.transform.position;
        guardianSleep.SetActive(true);
        guardian.GetComponent<SpriteRenderer>().enabled = false;
        yield return new WaitForSeconds(4f);

        fadeOut = true;
    }

    IEnumerator FadeOut()
    {
        Color color;
        float duration = 1f;
        
        for (float t = 0f; t < duration; t += Time.deltaTime) {
            float normalizedTime = t / duration;
            color = Color.Lerp(new Color(0, 0, 0, 0), new Color(0, 0, 0, 1), normalizedTime);
            im.color = color;

            yield return null;
        }
        color = new Color(0, 0, 0, 1);

        yield return new WaitForSeconds(1f);
        // maybe good job text

        Destroy(PD);
        SceneManager.LoadScene("StartMenu");
    }
}
