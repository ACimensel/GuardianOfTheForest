using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TeleportSkill : MonoBehaviour
{
    [SerializeField] float stayAliveTime = 10f;
    [SerializeField] AudioClip warpSpound;
    [SerializeField] AudioClip growSound;

    private GameObject guardian = null;
    private Guardian script = null;
    private AudioSource audSrc;

    public bool hasPair = false;
    public Vector3 pairPos;

    void Awake()
    {
        guardian = GameObject.Find("Guardian");
        if (guardian != null) script = guardian.GetComponent<Guardian>();

        audSrc = GetComponent<AudioSource>();
    }

    // void OnTriggerEnter2D(Collider2D col){
    //     string layerName = LayerMask.LayerToName(col.gameObject.layer);
    //     if (layerName == "PlayerBolt"){}
    // }

    public void PlayWarpSound()
    {
        audSrc.clip = warpSpound;
        audSrc.Play();
    }

    public void UpdateTeleportPair(bool _hasPair, Vector3 _pairPos)
    {
        hasPair = _hasPair;
        pairPos = _pairPos;

        GetComponent<SpriteRenderer>().color = (hasPair) ? new Color(233 / 255f, 255 / 255f, 174 / 255f) : new Color(1f, 1f, 1f);
    }

    public void Destroy()
    {
        StartCoroutine(DestroyAfterXSeconds(stayAliveTime));
    }

    public int PortalPosition()
    {
        if (!hasPair) return -1;

        if (transform.position.x < pairPos.x) return 0; // this teleport is to the left of its partner
        else if (transform.position.x > pairPos.x) return 1; // this teleport is to the right of its partner
        else return 2; // they are in the same position
    }

    IEnumerator DestroyAfterXSeconds(float stayAliveTime)
    {
        yield return new WaitForSeconds(stayAliveTime);

        Destroy(gameObject.GetComponent<BoxCollider2D>());

        if (script != null) script.DequeueTeleport();

        audSrc.clip = growSound;
        audSrc.Play();
        GetComponent<Animator>().SetBool("Destroy", true);
    }
}
