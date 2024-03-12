using UnityEngine;

public class BasicMovement : MonoBehaviour
{
    [SerializeField] private float speed = 4f;


    private float _horizontalInput;
    private float _verticalInput;


    private void Update()
    {
        _horizontalInput = Input.GetAxis("Horizontal");
        _verticalInput = Input.GetAxis("Vertical");
    }

    private void FixedUpdate()
    {
        transform.position += new Vector3(_horizontalInput, _verticalInput, 0) * (speed * Time.fixedDeltaTime);
    }
}