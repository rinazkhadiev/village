using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class CameraController : MonoBehaviour, IDragHandler
{
    private Transform _cameraTransform;
    private Vector3 _offset;
    private float _deadYPosition = 20;

    [SerializeField] private float _turnSpeed = 5.0f;
    [SerializeField] private Transform _characterTransform;
    [SerializeField] private Transform _cameraCharacterTransform;

    private float _moveX;
    private float _moveY;


    private void Start()
    {
        _cameraTransform = Camera.main.transform;

        if (AllObjects.Singleton.sv.CameraView == 1)
        {
            for (int i = 0; i < AllObjects.Singleton.CharacterHead.Length; i++)
            {
                AllObjects.Singleton.CharacterHead[i].SetActive(false);
            }
        }
        else if (AllObjects.Singleton.sv.CameraView == 2)
        {
            _offset = new Vector3(_characterTransform.position.x, _characterTransform.position.y + 5f, _characterTransform.position.z + 1.5f);

            for (int i = 0; i < AllObjects.Singleton.CharacterHead.Length; i++)
            {
                AllObjects.Singleton.CharacterHead[i].SetActive(true);
            }
        }
        else
        {
            _offset = new Vector3(_characterTransform.position.x - 4.55f, _characterTransform.position.y + 15.3f, _characterTransform.position.z + 3);

            for (int i = 0; i < AllObjects.Singleton.CharacterHead.Length; i++)
            {
                AllObjects.Singleton.CharacterHead[i].SetActive(true);
            }
        }
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
            if (AllObjects.Singleton.sv.CameraView == 1)
            {
                _cameraTransform.position = new Vector3(_cameraCharacterTransform.position.x, _cameraCharacterTransform.position.y, _cameraCharacterTransform.position.z);
                _cameraTransform.rotation = Quaternion.Euler(_moveY, _moveX, 0);
                Character.Singleton.Transform.rotation = Quaternion.Euler(new Vector3(0, _moveX, 0));
            }
            else if (AllObjects.Singleton.sv.CameraView == 2)
            {
                _cameraTransform.position = Character.Singleton.Transform.position + _offset;
                _cameraTransform.LookAt(Character.Singleton.Transform.position);
                _cameraTransform.rotation = Quaternion.Euler(10, _cameraTransform.eulerAngles.y, 0);
            }
            else
            {
                _cameraTransform.position = Character.Singleton.Transform.position + _offset;
                _cameraTransform.LookAt(Character.Singleton.Transform.position);
                _cameraTransform.rotation = Quaternion.Euler(60, _cameraTransform.eulerAngles.y, 0);

                Vector3 cameraTargetPositionY = new Vector3(_cameraTransform.position.x, 15.3f + (int)Character.Singleton.Transform.position.y, _cameraTransform.position.z);
                _cameraTransform.position = Vector3.Lerp(_cameraTransform.position, cameraTargetPositionY, Time.deltaTime * 2f);
            }

            AllObjects.Singleton.JoyStickBG.transform.rotation = Quaternion.Euler(0, 0, _cameraTransform.eulerAngles.y);
        }
    }

    public virtual void OnDrag(PointerEventData eventData)
    {
        if (AllObjects.Singleton.sv.CameraView == 1)
        {
            _moveY -= eventData.delta.y / 6;
            _moveY = Mathf.Clamp(_moveY, -40, 75);

            _moveX += eventData.delta.x / 6;
            if (_moveX < -360) _moveX += 360;
            if (_moveX > 360) _moveX -= 360;
            _moveX = Mathf.Clamp(_moveX, -360, 360);
        }
        else
        {
            _offset = Quaternion.AngleAxis(eventData.delta.x * _turnSpeed, Vector3.up) * _offset;
        }
    }

    public void CameraViewChange(int view)
    {
        AllObjects.Singleton.sv.CameraView = view;

        if (AllObjects.Singleton.sv.CameraView == 1)
        {
            for (int i = 0; i < AllObjects.Singleton.CharacterHead.Length; i++)
            {
                AllObjects.Singleton.CharacterHead[i].SetActive(false);
            }
        }
        else if (AllObjects.Singleton.sv.CameraView == 2)
        {
            StartCoroutine(CameraViewThird());
        }
        else
        {
            StartCoroutine(CameraViewTop());
        }

        AllObjects.Singleton.SaveUpdate();
    }

    public void CameraViewChangeInPanel(int view)
    {
        AllObjects.Singleton.sv.CameraView = view;

        if (AllObjects.Singleton.sv.CameraView == 1)
        {
            for (int i = 0; i < AllObjects.Singleton.CharacterHead.Length; i++)
            {
                AllObjects.Singleton.CharacterHead[i].SetActive(false);
            }
        }
        else if (AllObjects.Singleton.sv.CameraView == 2)
        {
            _offset = new Vector3(_characterTransform.position.x, _characterTransform.position.y + 5f, _characterTransform.position.z + 1.5f);

            for (int i = 0; i < AllObjects.Singleton.CharacterHead.Length; i++)
            {
                AllObjects.Singleton.CharacterHead[i].SetActive(true);
            }
        }
        else
        {
            _offset = new Vector3(_characterTransform.position.x - 4.55f, _characterTransform.position.y + 15.3f, _characterTransform.position.z + 3);

            for (int i = 0; i < AllObjects.Singleton.CharacterHead.Length; i++)
            {
                AllObjects.Singleton.CharacterHead[i].SetActive(true);
            }
        }

        AllObjects.Singleton.SaveUpdate();
    }

    IEnumerator CameraViewTop()
    {
        AllObjects.Singleton.CharacterIsBusy = true;
        Character.Singleton.Transform.position = Character.Singleton.StartPosition;
        yield return new WaitForSeconds(0.5f);
        AllObjects.Singleton.CharacterIsBusy = false;

        _offset = new Vector3(_characterTransform.position.x - 4.55f, _characterTransform.position.y + 15.3f, _characterTransform.position.z + 3);

        for (int i = 0; i < AllObjects.Singleton.CharacterHead.Length; i++)
        {
            AllObjects.Singleton.CharacterHead[i].SetActive(true);
        }
    }

    IEnumerator CameraViewThird()
    {
        AllObjects.Singleton.CharacterIsBusy = true;
        Character.Singleton.Transform.position = Character.Singleton.StartPosition;
        yield return new WaitForSeconds(0.5f);
        AllObjects.Singleton.CharacterIsBusy = false;

        _offset = new Vector3(_characterTransform.position.x, _characterTransform.position.y + 5f, _characterTransform.position.z + 1.5f);

        for (int i = 0; i < AllObjects.Singleton.CharacterHead.Length; i++)
        {
            AllObjects.Singleton.CharacterHead[i].SetActive(true);
        }
    }
}
