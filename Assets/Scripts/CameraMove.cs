using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public Transform targetPosition;

    public void MoveCamera()
    {
        Camera.main.transform.position = targetPosition.position;
        Camera.main.transform.rotation = targetPosition.rotation;
    }
}
