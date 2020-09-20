using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlDisplayClose : MonoBehaviour
{
	void Update ()
    {
		if (Input.GetButton("Cancel"))
        {
            gameObject.SetActive(false);
        }
	}
}
