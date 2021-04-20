using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dissolve : MonoBehaviour
{
    Material material;

    bool isDissolving = false;
    float fade = 1f;

    private void Start() {
        material = GetComponent<SpriteRenderer>().material;
    }
    private void Update() {
        if (Input.GetKeyDown(KeyCode.R)) {
            isDissolving = true;
        }
        if (isDissolving) {
            fade -= Time.deltaTime;

            if(fade <= 0f){
                fade = 0f;
                isDissolving = false;
            }

            material.SetFloat("_Fade", fade);
        }
    }
}
