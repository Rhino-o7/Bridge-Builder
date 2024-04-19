using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Animal : MonoBehaviour
{
    
    Animator animator;
    AudioSource audioSource;
    [SerializeField] AudioClip[] meow;
    [SerializeField] AudioClip found;
    [SerializeField] GameObject soundWave;
    bool playMeow = true;
    void Awake(){
        int rand1 = Random.Range(1,4);
        int rand2 = Random.Range(1,4);
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        animator.SetInteger("CatType", rand1);
        animator.SetInteger("CatAnim", rand2);
    }
    void Start(){
        StartCoroutine(MeowSound());
    }

    void OnTriggerEnter2D(Collider2D c){
        if (c.gameObject.CompareTag("Player")){
            animator.SetTrigger("CatFound");
            LevelMgr.levelMgr.AddCatFound();
            audioSource.PlayOneShot(found);
            gameObject.GetComponent<Collider2D>().enabled = false;
        }
    }

    public void FoundEnd(){
        gameObject.SetActive(false);
    }

    IEnumerator MeowSound(){
        bool first = true;
        while (playMeow){
            if (first){
                first = false;
                yield return new WaitForSeconds(Random.Range(1,5));
            }
            audioSource.PlayOneShot(meow[Random.Range(0,meow.Length)]);
            GameObject g = Instantiate(soundWave, transform);
            yield return new WaitForSeconds(Random.Range(5, 25));
        }
    }

    void OnDestroy(){
        playMeow = false;
    }

    

}
