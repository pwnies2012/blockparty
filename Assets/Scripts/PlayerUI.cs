using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    private UI UIinstance;
    // Update is called once per frame
    void Start()
    {
        UIinstance = GameObject.Find("PlayerUI").GetComponent<UI>();
    }

    private void OnTriggerEnter(Collider other)
    {
        UIinstance.trigger(gameObject.name, other);
    }

    private void OnTriggerExit(Collider other)
    {
        UIinstance.untrigger(gameObject.name, other);
    }
}
