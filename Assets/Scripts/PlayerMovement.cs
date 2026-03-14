using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    [SerializeField] bool isRunning;

    void Update()
    {
        if(isRunning == false)
        {
            isRunning = true;
            StartCoroutine(AddDistance());
        }
    }

    IEnumerator AddDistance()
    {
        yield return new WaitForSeconds(0.15f);
        MasterInfo.distanceRun +=1;
        isRunning = false;
    }
}
