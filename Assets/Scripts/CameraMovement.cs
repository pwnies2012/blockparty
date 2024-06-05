using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraMovement : MonoBehaviour, ICustomMessageTarget
{
    private int currStage = 1;
    private Vector3 PLATFORM_1_COORDS = new Vector3(10, 11, 9);
    private Vector3 PLATFORM_2_COORDS = new Vector3(14, 3, 16);
    private Vector3 PLATFORM_3_COORDS = new Vector3(13.5f, -9, 22);
    private Vector3 PLATFORM_4_COORDS = new Vector3(11, -20, 36);
    private Vector3 PLATFORM_5_COORDS = new Vector3(12, -31, 42);

    public float moveDuration = 2f;
    public GameObject player1;
    public GameObject player2;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void NextStage()
    {
        StartCoroutine(MoveToNextStage());
    }

    private IEnumerator MoveToNextStage()
    {
        currStage++;

        Vector3 moveVec;
        switch (currStage)
        {
            case 2:
                moveVec = PLATFORM_2_COORDS;
                break;
            case 3:
                moveVec = PLATFORM_3_COORDS;
                break;
            case 4:
                moveVec = PLATFORM_4_COORDS;
                break;
            case 5:
                moveVec = PLATFORM_5_COORDS;
                break;
            default:
                moveVec = PLATFORM_1_COORDS;
                break;
        }
        moveVec -= transform.position;
        Debug.Log(moveVec);

        float elapsedTime = 0f;
        while (elapsedTime < moveDuration)
        {
            elapsedTime += Time.deltaTime;
            Debug.Log(moveVec * (Time.deltaTime / moveDuration));
            transform.Translate(moveVec * (Time.deltaTime / moveDuration), Space.World);
            yield return null;
        }

        ExecuteEvents.Execute<ICustomMessageTarget>(player1, null, (x, y) => x.TransitionDone());
        //ExecuteEvents.Execute<ICustomMessageTarget>(player2, null, (x, y) => x.TransitionDone());
    }

    public void TransitionDone()
    {

    }

}
