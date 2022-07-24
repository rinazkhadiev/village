using UnityEngine;

public class CameraShake : MonoBehaviour
{
    private float _value = 0.25f;
    private float _speed = 2.5f;
    private float _distation;
    private Vector3 _startPos;
    private Vector3 _rotation = Vector3.zero;

    private Transform _transform;

    private void Start()
    {
        _transform = GetComponent<Transform>();
        _startPos = transform.position;
    }

    private void Update()
    {
        if (AllObjects.Singleton.sv.CameraView == 1)
        {
            _distation += (transform.position - _startPos).magnitude;
            _startPos = transform.position;
            _rotation.z = Mathf.Sin(_distation * _speed) * _value;
            _transform.eulerAngles = _rotation + Character.Singleton.Transform.eulerAngles;
        }
    }
}
