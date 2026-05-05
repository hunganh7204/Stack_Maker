using System;
using UnityEngine;


public class PlayerController : MonoBehaviour
{
    [SerializeField]private InputController inputController;
    enum Direction { Forward, Back, Left, Right }
    private Direction currentDirection;
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private LayerMask floorLayer;
    [SerializeField] private LayerMask wallLayer;

    private bool isMoving = false;
    private Vector3 moveDirection;
    private Vector3 targetPosition;

    [SerializeField]private float tileSize = 1.5f;
    [SerializeField] private Stack stack;

    [Header("Setup")]
    [SerializeField] private float startingY;

    [Header("Animator")]
    [SerializeField] private Animator animator;

    [Header("Rotation")]
    [SerializeField] private Transform characterModel; 
    [SerializeField] private float rotationSpeed = 20f; 

    private bool isGamePlaying = false;


    public void OnInit(Vector3 startPos)
    {
        isGamePlaying = false;
        isMoving = false;
        moveDirection = Vector3.zero;

        transform.position = new Vector3(startPos.x, startingY, startPos.z);
        targetPosition = transform.position;

        stack.ClearStack();

        if (animator != null)
        {
            animator.Rebind();
            animator.Update(0f);
        }
    }

    public void OnPlay()
    {
        isGamePlaying = true;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        inputController.OnSwipe += HandleSwipeDirection;
    }

    // Update is called once per frame
    void Update()
    {
        if(isMoving)
        {
            Move();
        }
    }

    private void HandleSwipeDirection(Vector3 swipeDir)
    {
        if (!isGamePlaying) return;
        if (isMoving) return;
        if (Mathf.Abs(swipeDir.x) > Mathf.Abs(swipeDir.y))
        {
            if (swipeDir.x > 0)
            {
                TryMove(Direction.Right);
            }
            else
            {
                TryMove(Direction.Left);
            }
        }
        else
        {
            if (swipeDir.y > 0)
            {
                TryMove(Direction.Forward);
            }
            else
            {
                TryMove(Direction.Back);
            }
        }
    }

    private void TryMove(Direction direction)
    {
        Vector3 intendedMoveDirection = Vector3.zero;

        switch (direction)
        {
            case Direction.Forward: intendedMoveDirection = Vector3.forward; break;
            case Direction.Back: intendedMoveDirection = Vector3.back; break;
            case Direction.Left: intendedMoveDirection = Vector3.left; break;
            case Direction.Right: intendedMoveDirection = Vector3.right; break;
        }

        CalculateDestination(intendedMoveDirection);
    }

    private void CalculateDestination(Vector3 intendedMoveDirection)
    {
        Vector3 rayOrigin = transform.position + Vector3.up * 0.5f;

        if (Physics.Raycast(rayOrigin, intendedMoveDirection, out RaycastHit hit, 100f, wallLayer))
        {
            Vector3 hitPos = hit.collider.bounds.center;
            Vector3 finalDest;

            if (hit.collider.CompareTag("WInpos"))
            {
                finalDest = new Vector3(hitPos.x, transform.position.y, hitPos.z);
            }
            else
            {
                finalDest = new Vector3(hitPos.x, transform.position.y, hitPos.z) - intendedMoveDirection * tileSize;
            }

            if (Vector3.Distance(finalDest, transform.position) > 0.05f)
            {
                moveDirection = intendedMoveDirection;
                targetPosition = finalDest;
                isMoving = true;
            }
        }

    }

    private void Move()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        if (moveDirection != Vector3.zero && characterModel != null)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            characterModel.rotation = Quaternion.Slerp(characterModel.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        if (Vector3.Distance(transform.position, targetPosition) < 0.001f)
        {
            transform.position = targetPosition;
            isMoving = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Stack"))
        {
            stack.AddStack();
            other.gameObject.SetActive(false);

            if (animator != null)
            {
                animator.SetTrigger("Stack");
            }
        }

        if (other.gameObject.CompareTag("Corner"))
        {
            CornerBlock corner = other.GetComponent<CornerBlock>();
            if(corner != null)
            {
                Vector3 newDir = corner.GetNewDirection(moveDirection);
                if(newDir != Vector3.zero)
                {
                    Vector3 trueCenter = other.bounds.center;
                    Vector3 snapPos = transform.position;
                    snapPos.x = trueCenter.x;
                    snapPos.z = trueCenter.z;
                    transform.position = snapPos;
                    CalculateDestination(newDir);
                   
                }
            }
        }
        if (other.gameObject.CompareTag("Bridge"))
        {
            if (stack.GetStackCount() > 0)
            {
                stack.RemoveStack(other.bounds.center);
                other.gameObject.tag = "Untagged";
            }
            else
            {
                isMoving = false;
                isGamePlaying = false;
                Invoke(nameof(CallShowLoseUI), 0.01f);
            }
        }

        if (other.gameObject.CompareTag("WInpos"))
        {
            Transform standPoint = other.transform.parent.Find("StandPos");
            if (standPoint != null)
            {
                transform.position = new Vector3(standPoint.position.x, transform.position.y, standPoint.position.z);
            }
            ParticleSystem[] winEffects = other.transform.parent.GetComponentsInChildren<ParticleSystem>();
            foreach (ParticleSystem effect in winEffects)
            {
                effect.Play(); 
            }

            if (stack != null)
            {
                stack.ClearStack();
            }

            if (animator != null)
            {
                animator.SetTrigger("Win");
            }
            isMoving = false;
            isGamePlaying = false;
            targetPosition = transform.position;
            Debug.Log("Level Complete!");
            Invoke(nameof(CallShowWinUI), 1.0f);
        }
    }


    private void OnDestroy()
    {
        if (inputController != null)
        {
            inputController.OnSwipe -= HandleSwipeDirection;
        }
    }

    private void CallShowWinUI()
    {
        UIManager.Instance.ShowWinUI();
    }

    private void CallShowLoseUI()
    {
        UIManager.Instance.ShowLoseUI();
    }

}