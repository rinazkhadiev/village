using UnityEngine;
using UnityEngine.EventSystems;

public class CameraController : MonoBehaviour, IDragHandler
{
    private Transform _cameraTransform;
    private Vector3 _offset;
    private float _deadYPosition = 20;

    [SerializeField] private float _turnSpeed = 5.0f;
    [SerializeField] private Transform _characterTransform;

    private void Start()
    {
        _cameraTransform = Camera.main.transform;
        _offset = new Vector3(_characterTransform.position.x - 4.55f, _characterTransform.position.y + 15.3f, _characterTransform.position.z + 3);
    }

    private void Update()
    {
        if (Character.Singleton.IsDead)
        {
            _deadYPosition += Time.deltaTime;
            _cameraTransform.position = new Vector3(_cameraTransform.position.x, _deadYPosition, _cameraTransform.position.z);
            _cameraTransform.rotation = Quaternion.Euler(new Vector3(90, 0, 0));
        }
        else
        {
            _cameraTransform.position = Character.Singleton.Transform.position + _offset;
            
            _cameraTransform.LookAt(Character.Singleton.Transform.position);
            _cameraTransform.rotation = Quaternion.Euler(60, _cameraTransform.eulerAngles.y, 0);
           
            Vector3 cameraTargetPositionY = new Vector3(_cameraTransform.position.x, 15.3f + (int)Character.Singleton.Transform.position.y, _cameraTransform.position.z);
            _cameraTransform.position = Vector3.Lerp(_cameraTransform.position, cameraTargetPositionY, Time.deltaTime * 2f);
            
            AllObjects.Singleton.JoyStickBG.transform.rotation = Quaternion.Euler(0, 0, _cameraTransform.eulerAngles.y);
        }
    }

    public virtual void OnDrag(PointerEventData eventData)
    {
        _offset = Quaternion.AngleAxis(eventData.delta.x * _turnSpeed, Vector3.up) * _offset;
    }
}
