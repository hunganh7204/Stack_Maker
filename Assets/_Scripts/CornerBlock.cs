using UnityEngine;

public class CornerBlock : MonoBehaviour
{
    [SerializeField] private enum CornerType{TopLeft, TopRight, BottomLeft, BottomRight};
    [Tooltip("Select the type of corner for this block")]
    [SerializeField] private CornerType cornerType;

    public Vector3 GetNewDirection(Vector3 incomingDirection)
    {      

        switch (cornerType)
        {
            case CornerType.TopLeft:
                if (Vector3.Dot(incomingDirection, Vector3.forward) > 0.9f)
                {
                    return Vector3.right;
                }
                if (Vector3.Dot(incomingDirection, Vector3.left) > 0.9f)
                {
                    return Vector3.back;
                }
                break;

            case CornerType.TopRight:
                if (Vector3.Dot(incomingDirection, Vector3.forward) > 0.9f)
                {
                    return Vector3.left;
                }
                if (Vector3.Dot(incomingDirection, Vector3.right) > 0.9f)
                {
                    return Vector3.back;
                }
                break;
            case CornerType.BottomLeft:
                if (Vector3.Dot(incomingDirection, Vector3.back) > 0.9f)
                {
                    return Vector3.right;
                }
                if (Vector3.Dot(incomingDirection, Vector3.left) > 0.9f)
                {
                    return Vector3.forward;
                }
                break;
            case CornerType.BottomRight:
                if (Vector3.Dot(incomingDirection, Vector3.back) > 0.9f)
                {
                    return Vector3.left;
                }
                if (Vector3.Dot(incomingDirection, Vector3.right) > 0.9f)
                {
                    return Vector3.forward;
                }
                break;
        }

        return Vector3.zero;
    }
}
