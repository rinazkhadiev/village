using UnityEngine;
using UnityEngine.EventSystems;

public class CameraController : MonoBehaviour /*, IDragHandler*/
{
    private Transform _cameraTransform;
    private float _deadYPosition = 20;

    //private float _moveX;
    //private float _moveY;

    //[SerializeField] private float _sensitivity = 6f;

    private void Start()
    {
        _cameraTransform = Camera.main.transform;
    }
    private void Update()
    {
        if (Character.Singleton.IsDead)
        {
            _deadYPosition += Time.deltaTime;
            _cameraTransform.position = new Vector3(_cameraTransform.position.x, _deadYPosition, _cameraTransform.position.z);
            _cameraTransform.rotation = Quaternion.Euler(new Vector3(90, 0, 0));
        }
    } 

    //public void OnDrag(PointerEventData eventData)
    //{
    //    _moveY -= eventData.delta.y / _sensitivity;
    //    _moveY = Mathf.Clamp(_moveY, -40, 40);

    //    _moveX += eventData.delta.x / _sensitivity;
    //    if (_moveX < -360) _moveX += 360;
    //    if (_moveX > 360) _moveX -= 360;
    //    _moveX = Mathf.Clamp(_moveX, -360, 360);
    //}
}
