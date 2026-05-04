using UnityEngine;

public class CornerBlock : MonoBehaviour
{
    [SerializeField] private enum CornerType{TopLeft, TopRight, BottomLeft, BottomRight};
    [Tooltip("Select the type of corner for this block")]
    [SerializeField] private CornerType cornerType;

    public Vector3 GetNewDirection(Vector3 incomingDirection)
    {
        incomingDirection.Normalize();

        switch (cornerType)
        {
            case CornerType.TopLeft:
                if (incomingDirection == Vector3.forward)
                {
                    Debug.Log("Incoming: " + incomingDirection + " New: " + Vector3.right);
                    return Vector3.right;
                }
                if (incomingDirection == Vector3.left)
                {
                    Debug.Log("Incoming: " + incomingDirection + " New: " + Vector3.back);
                    return Vector3.back;
                }
                break;

            case CornerType.TopRight:
                if(incomingDirection == Vector3.forward)
                {
                    Debug.Log("Incoming: " + incomingDirection + " New: " + Vector3.left);
                    return Vector3.left;
                }
                if (incomingDirection == Vector3.right)
                {
                    Debug.Log("Incoming: " + incomingDirection + " New: " + Vector3.back);
                    return Vector3.back;
                }
                break;
            case CornerType.BottomLeft:
                if(incomingDirection == Vector3.back)
                {
                    Debug.Log("Incoming: " + incomingDirection + " New: " + Vector3.right);
                    return Vector3.right;
                }
                if (incomingDirection == Vector3.left)
                {
                        Debug.Log("Incoming: " + incomingDirection + " New: " + Vector3.forward);
                    return Vector3.forward;
                }
                break;
            case CornerType.BottomRight:
                if(incomingDirection == Vector3.back)
                {
                    Debug.Log("Incoming: " + incomingDirection + " New: " + Vector3.left);
                    return Vector3.left;
                }
                if (incomingDirection == Vector3.right)
                {
                    Debug.Log("Incoming: " + incomingDirection + " New: " + Vector3.forward);
                    return Vector3.forward;
                }
                break;
        }

        return Vector3.zero;
    }
}
