using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundWave : MonoBehaviour
{
    [SerializeField] float expandRate = 1f;
    [SerializeField] float decayRate = 0.1f;
    float alpha = 1;
    SpriteRenderer spriteRenderer;

    void Awake(){
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = new Color(255,255,255,alpha);
        transform.localScale = Vector3.zero;

    }

    void Update(){
        if (alpha <= 0){
            Destroy(gameObject);
        }
        transform.localScale += new Vector3(expandRate,expandRate,expandRate) * Time.deltaTime;
        alpha -= decayRate * Time.deltaTime;
        spriteRenderer.color = new Color(255,255,255,alpha);
    }
}