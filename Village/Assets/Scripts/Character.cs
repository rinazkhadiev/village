using UnityEngine;
using System.Collections;

public class Character : MonoBehaviour
{
    public static Character Singleton { get; private set; }
    public Transform Transform { get; private set; }
    public CharacterController CharController { get; set;}

    [SerializeField] private float _stepTime;
    [SerializeField] private float _playerSpeed = 2.0f;
    [SerializeField] private float _jumpHeight = 1.0f;
    [SerializeField] private float _gravityValue = -9.81f;

    private Transform _cameraTransform;
    private Vector3 _playerVelocity;
    private bool _isStepping;
    private bool _groundedPlayer;
    private bool _isJumping;
    private Vector3 _moveDirection;
    private int _hp = 100;

    private void Start()
    {
        Singleton = this;
        CharController = GetComponent<CharacterController>();
        Transform = GetComponent<Transform>();
        _cameraTransform = Camera.main.transform;
    }

    private void Update()
    {
        _cameraTransform.position = new Vector3(Transform.position.x - 5, 15, Transform.position.z - 5.5f);

        _groundedPlayer = CharController.isGrounded;
        if (_groundedPlayer && _playerVelocity.y < 0)
        {
            _playerVelocity.y = 0f;
        }

        if (!AllObjects.Singleton.CharacterIsBusy)
        {
            CharController.Move(_moveDirection * Time.deltaTime);
            CharController.Move(_playerVelocity * Time.deltaTime);
        }

        _moveDirection = Vector3.zero;
        _moveDirection.x = AllObjects.Singleton.JoyController.Horizontal() * _playerSpeed;
        _moveDirection.z = AllObjects.Singleton.JoyController.Vertical() * _playerSpeed;

        if (Vector3.Angle(Vector3.forward, _moveDirection) > 1f || Vector3.Angle(Vector3.forward, _moveDirection) == 0)
        {
            transform.rotation = Quaternion.LookRotation(Vector3.RotateTowards(transform.forward, _moveDirection, _playerSpeed, 0.0f));
        }

        if (!_isStepping && !_isJumping)
        {
            if (CharController.velocity.magnitude > 4f)
            {
                _stepTime = 0.5f;
                StartCoroutine(Step());
            }
            else if (CharController.velocity.magnitude > 2f && CharController.velocity.magnitude < 4f)
            {
                _stepTime = 0.8f;
                StartCoroutine(Step());
            }
            else if (CharController.velocity.magnitude < 2f && CharController.velocity.magnitude > 0.1f)
            {
                _stepTime = 1.5f;
                StartCoroutine(Step());
            }
        }

        if (_isJumping && _groundedPlayer)
        {
            _playerVelocity.y += Mathf.Sqrt(_jumpHeight * -3.0f * _gravityValue);
        }

        _playerVelocity.y += _gravityValue * Time.deltaTime;
    }

    public void Jump()
    {
        StartCoroutine(JumpWait());
    }

    public void HealthChange(int value)
    {
        _hp += value;
        AllObjects.Singleton.HpText.text = $"{_hp}%";
    }

    IEnumerator JumpWait()
    {
        _isJumping = true;
        yield return new WaitForSeconds(0.1f);
        _isJumping = false;
    }

    IEnumerator Step()
    {
        _isStepping = true;
        //AllObjects.Singleton.StepAudio.PlayOneShot(AllObjects.Singleton.FirstSteps[Random.Range(0, AllObjects.Singleton.FirstSteps.Length)]);
        yield return new WaitForSeconds(_stepTime);
        _isStepping = false;
    }
}