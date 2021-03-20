using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateIndicator : Singleton<RotateIndicator> {

    private PlayerInputHandler InputHandler;
    private AngleRotations angleRotations;

    private int horizontalInput;
    private int verticalInput;

    private float up, down, left, right;

    private void Start() {
        InputHandler = GetComponent<PlayerInputHandler>();

        angleRotations.up = 0f;
        angleRotations.right = 90f;
        angleRotations.down = 180f;
        angleRotations.left = 270f;
    }
    private void Update() {
        horizontalInput = InputHandler.DashInputX;
        verticalInput = InputHandler.DashInputY;
    }
    public void rotateObject() {
        if (horizontalInput > 0) {
            this.transform.rotation = Quaternion.Euler(0f, 0f, 45f - angleRotations.right);
        } else if (horizontalInput < 0) {
            this.transform.rotation = Quaternion.Euler(0f, 0f, 45f - angleRotations.left);
        } else if (verticalInput > 0) {
            this.transform.rotation = Quaternion.Euler(0f, 0f, 45f - angleRotations.up);
        } else if (verticalInput < 0) {
            this.transform.rotation = Quaternion.Euler(0f, 0f, 45f - angleRotations.down);
        } else {
            this.transform.rotation = Quaternion.Euler(0f, 0f, 45f); ;
        }
    }
    public struct AngleRotations {
        public float up, down, left, right;
    }



}
