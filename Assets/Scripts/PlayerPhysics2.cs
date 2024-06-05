using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class PlayerPhysics2 : MonoBehaviour, ICustomMessageTarget, IUIStuffs
{

    public LayerMask contactWallLayer;
    public LayerMask stageLayer;
    public float rollDuration = 0.6f;
    private bool isRolling;
    public Transform pivot;
    public Transform ghost;
    public Collider ghostBox;
    public GameObject UI;
    public GameObject otherPlayer;

    private Collider[] colliders;
    private bool betweenLevels = false;
    private bool playing = true;


    // Start is called before the first frame update
    void Start()
    {
        colliders = new Collider[32];
        isRolling = false;
    }

    void OnLook(InputValue movementValue)
    {
        Vector2 movementVector = movementValue.Get<Vector2>();
        if (Mathf.Abs(movementVector.x) < 1f && Mathf.Abs(movementVector.y) < 1f)
            return;

        StartCoroutine(RollToDirection(VectorToDir(movementVector)));
    }

    private IEnumerator RollToDirection(Direction d)
    {
        if (!isRolling && !betweenLevels && playing)
        {
            isRolling = true;

            float angle = 90f;
            Vector3 axis = GetAxis(d);
            Vector3 direction = GetDirectionVector(d);
            Vector2 pivotOffset = GetPivotOffset(d);

            pivot.position = transform.position + (direction * pivotOffset.x) + (Vector3.down * pivotOffset.y);
            print(pivot.position);

            CopyTransformData(transform, ghost);
            ghost.RotateAround(pivot.position, axis, angle);
            bool collided = CheckForCollisions();

            if (!collided)
            {
                float elapsedTime = 0f;

                while (elapsedTime < rollDuration)
                {
                    elapsedTime += Time.deltaTime;

                    transform.RotateAround(pivot.position, axis, (angle * (Time.deltaTime / rollDuration)));
                    yield return null;
                }

                if ((ghost.transform.position - transform.position).magnitude < 0.1)
                    CopyTransformData(ghost, transform);
            }

            bool overEdge = CheckForFall();
            if (!overEdge)
                isRolling = false;
            else
                Debug.Log("Fall detected!");
        }
    }

    public void CopyTransformData(Transform source, Transform target)
    {
        target.localPosition = source.localPosition;
        target.localEulerAngles = source.localEulerAngles;
    }

    private Direction VectorToDir(Vector2 direction)
    {

        if (direction.x > 0)
            return Direction.Left;
        if (direction.x < 0)
            return Direction.Right;
        if (direction.y > 0)
            return Direction.Down;
        if (direction.y < 0)
            return Direction.Up;

        return Direction.Neutral;
    }

    private Vector3 GetAxis(Direction direction)
    {
        switch (direction)
        {
            case Direction.Left:
                return Vector3.forward;
            case Direction.Up:
                return Vector3.right;
            case Direction.Right:
                return Vector3.back;
            case Direction.Down:
                return Vector3.left;
            default:
                return Vector3.zero;
        }
    }

    private Vector3 GetDirectionVector(Direction direction)
    {
        switch (direction)
        {
            case Direction.Left:
                return Vector3.left;
            case Direction.Up:
                return Vector3.forward;
            case Direction.Right:
                return Vector3.right;
            case Direction.Down:
                return Vector3.back;
            default:
                return Vector3.zero;
        }
    }

    private Vector2 GetPivotOffset(Direction direction)
    {
        Vector2 pivotOffset = Vector2.zero;
        Vector2 center = transform.GetComponent<BoxCollider>().size / 2f;

        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.up, out hit, 200f, contactWallLayer))
        {
            switch (hit.collider.name)
            {
                case "X":
                    if (direction == Direction.Left || direction == Direction.Right)
                        pivotOffset = new Vector2(center.y, center.x);
                    else
                        pivotOffset = Vector2.one * center.x;
                    break;
                case "Y":
                    pivotOffset = center;
                    break;
                case "Z":
                    if (direction == Direction.Up || direction == Direction.Down)
                        pivotOffset = new Vector2(center.y, center.x);
                    else
                        pivotOffset = Vector2.one * center.x;
                    break;
            }
        }

        return pivotOffset;
    }

    private bool CheckForCollisions()
    {

        int count = Physics.OverlapSphereNonAlloc(ghostBox.transform.position, 3f, colliders);

        for (int i = 0; i < count; i++)
        {
            var collider = colliders[i];

            if (collider == ghostBox)
                continue;

            Vector3 otherPos = collider.transform.position;
            Quaternion otherRot = collider.transform.rotation;

            Vector3 direction;
            float distance;

            bool overlapped = Physics.ComputePenetration(
                ghostBox, ghostBox.transform.position, ghostBox.transform.rotation,
                collider, otherPos, otherRot,
                out direction, out distance
            );

            if (overlapped)
            {
                print("overlap with " + collider.name + ": " + direction + " " + distance);
                if (distance > 0.2f)
                    return true;
            }
        }
        return false;
    }

    private bool CheckForFall()
    {
        if (betweenLevels) return false;

        Transform[] objs = GetComponentsInChildren<Transform>();

        foreach (Transform obj in objs)
        {
            if (betweenLevels) return false;

            if (obj.tag != "FallDetector")
                continue;

            RaycastHit stage;

            if (Physics.Raycast(obj.transform.position, Vector3.down, out stage, 5f, stageLayer))
            {
                continue;
            }

            ExecuteEvents.Execute<IUIStuffs>(UI, null, (x, y) => x.FellOff());
            ExecuteEvents.Execute<IUIStuffs>(otherPlayer, null, (x, y) => x.FellOff());
            playing = false;
            return true;
        }

        return false;
    }

    public void NextStage()
    {
        betweenLevels = true;
    }

    public void TransitionDone()
    {
        betweenLevels = false;
    }

    public void FellOff()
    {
        playing = false;
    }

    enum Direction
    {
        Left, Right, Up, Down, Neutral
    }
}
