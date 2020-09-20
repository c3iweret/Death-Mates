using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatapultArm : MonoBehaviour {

    public Transform arm;

    private float rotationAmount = 60f;

    public bool animating = false;
    private int throwTicks = 5;
    private int resetTicks = 50;

    public IEnumerator PlayThrowAnimation()
    {
        if (!animating)
        {
            animating = true;

            // animate the arm throwing
            for (int i = 0; i < throwTicks; i++)
            {
                arm.Rotate(new Vector3(0,0, -rotationAmount / throwTicks));
                yield return new WaitForSeconds(0.01f);
            }

            // animate the arm going back
            for (int i = 0; i < resetTicks; i++)
            {
                arm.Rotate(new Vector3(0, 0, rotationAmount / resetTicks));
                yield return new WaitForSeconds(0.01f);
            }

            animating = false;
        }
    }
}
