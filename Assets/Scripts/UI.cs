using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using TMPro;
//using System.Numerics;

public class UI : MonoBehaviour, IUIStuffs
{
    public TextMeshProUGUI MenuText;
    public GameObject PlayButton;
    public TextMeshProUGUI WinText;
    public GameObject ReplayButton;
    public GameObject Player1;
    public GameObject Player2;
    public GameObject Camera;
    private bool redwin;
    private bool bluewin;

    private int stage;
    public GameObject platform1;
    public GameObject platform2;
    public GameObject platform3;
    public GameObject platform4;
    public GameObject platform5;

    void Start()
    {
        MenuText.gameObject.SetActive(false);
        PlayButton.gameObject.SetActive(false);
        WinText.gameObject.SetActive(false);
        ReplayButton.gameObject.SetActive(false);
        stage = 1;
        
    }

    public void Hide()
    {
        WinText.gameObject.SetActive(false);
        ReplayButton.gameObject.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        /*
        if (other.gameObject.CompareTag("Goal"))
        {
            won = true;
            WinText.gameObject.SetActive(true);
            ReplayButton.gameObject.SetActive(true);
        }
        */
    }

    public void trigger(string name, Collider other)
    {
        Debug.Log("Collision detected with: " + other.gameObject.name);
        if (other.gameObject.CompareTag("Goal") && name == "Player1")
        {
            redwin = true;
            Debug.Log("Red player reached the goal.");
        }

        else if (other.gameObject.CompareTag("BlueGoal") && name == "Player2")
        {
            bluewin = true;
            Debug.Log("Blue player reached the goal.");
        }
        Debug.Log("Redwin: " + redwin + " Bluewin: " + bluewin);

        if (bluewin && redwin)
        {
            ExecuteEvents.Execute<ICustomMessageTarget>(Player1, null, (x, y) => x.NextStage());
            //ExecuteEvents.Execute<ICustomMessageTarget>(Player2, null, (x, y) => x.NextStage());
            ExecuteEvents.Execute<ICustomMessageTarget>(Camera, null, (x, y) => x.NextStage());

            Debug.Log("Both players reached the goal.");
            GameObject currStage = null;
            switch (stage)
            {
                case 1:
                    currStage = platform1;
                    break;
                case 2:
                    currStage = platform2;
                    break;
                case 3:
                    currStage = platform3;
                    break;
                case 4:
                    currStage = platform4;
                    break;
                case 5:
                    currStage = platform5;
                    break;
                default:
                    Debug.Log("what the fuck");
                    break;
            }
            stage++;

            MeshCollider[] collision = currStage.GetComponentsInChildren<MeshCollider>();
            foreach (MeshCollider mc in collision)
            {
                mc.enabled = false;
            }

            bluewin = false;
            redwin = false;
        }
    }

    public void untrigger(string name, Collider other)
    {
        if (other.gameObject.CompareTag("Goal") && name == "Player1")
        {
            redwin = false;
            Debug.Log("Red player left the goal.");
        }
        else if (other.gameObject.CompareTag("BlueGoal") && name == "Player2")
        {
            bluewin = false;
            Debug.Log("Blue player left the goal.");
        }
        Debug.Log("Redwin: " + redwin + " Bluewin: " + bluewin);
    }

    public void FellOff()
    {
        WinText.gameObject.SetActive(true);
        ReplayButton.gameObject.SetActive(true);
    }

    public void Replay()
    {
        SceneManager.LoadScene("Level1");
    }

}