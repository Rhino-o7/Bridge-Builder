using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Animal : MonoBehaviour
{
    //void OnColl
    Animator animator;
    void Awake(){
        int rand1 = Random.Range(1,4);
        int rand2 = Random.Range(1,4);
        animator = GetComponent<Animator>();
        animator.SetInteger("CatType", rand1);
        animator.SetInteger("CatAnim", rand2);
    }

    void OnTriggerEnter2D(Collider2D c){
        if (c.gameObject.CompareTag("Player")){
            print("PET FOUND");
            LevelMgr.levelMgr.AddCatFound();
            gameObject.GetComponent<Collider2D>().enabled = false;
        }
    }
}
