using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadyOrNotController : MonoBehaviour
{
    public GameObject ready;
    public GameObject notReady;

    public void SetStatus(bool status)
    {
        ready.SetActive(status);
        notReady.SetActive(!status);
    }
}
