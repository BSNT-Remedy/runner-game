using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using Input = InputWrapper.Input;

public class Rotate : MonoBehaviour {
    public float rotationSpeed = 50f;

    private void Update() {
        if (Input.touchCount > 0) {
            Touch touch = Input.GetTouch(0);
            float touchDeltaZ = touch.deltaPosition.x;
            transform.Rotate(0, -touchDeltaZ * rotationSpeed * Time.deltaTime, 0);
        }
    }
}