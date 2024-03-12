using UnityEngine;

public class CameraScript : MonoBehaviour
{
    [SerializeField] private float followLerp;

    [SerializeField] private Transform followTransform;
    
    

    private void FixedUpdate()
    {
        var position = transform.position;
        
        position =
            Vector2.Lerp(position, followTransform.position, followLerp * Time.fixedDeltaTime);

        var pos = position;
        pos.z = -10;
        position = pos;
        transform.position = position;
    }
}