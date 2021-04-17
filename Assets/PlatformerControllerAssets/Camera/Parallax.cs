using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Parallax : MonoBehaviour {
    private float lengthx, lengthy, startPosx, startPosy;
    [SerializeField] private float parallaxStrengthx, parallaxStrengthy;
    [SerializeField] private Camera cam;

    private void Start() {
        startPosx = transform.position.x;
        lengthx = GetComponent<SpriteRenderer>().bounds.size.x;

        startPosy = transform.position.y;
        lengthy = GetComponent<SpriteRenderer>().bounds.size.y;
    }

    private void FixedUpdate() {
        float tempx = cam.transform.position.x * (1 - parallaxStrengthx);
        float distx = cam.transform.position.x * parallaxStrengthx;

        float tempy = cam.transform.position.y * (1 - parallaxStrengthy);
        float disty = cam.transform.position.y * parallaxStrengthy;

        transform.position = new Vector3(startPosx + distx, startPosy + disty, transform.position.z);

        if (tempx > startPosx + lengthx) startPosx += lengthx;
        else if (tempx < startPosx - lengthx) startPosx -= lengthx;

        if (tempy > startPosy + lengthy) startPosy += lengthy;
        else if (tempy < startPosy - lengthy) startPosy -= lengthy;
    }
}
