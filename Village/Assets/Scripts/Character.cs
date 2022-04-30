using UnityEngine;
using System.Collections;

public class Character : MonoBehaviour
{
    public static Character Singleton { get; private set; }
    public Transform Transform { get; private set; }
    public CharacterController CharController { get; set;}

    private float _stepTime;
    private float _gravityValue = -9.81f;
    private Transform _cameraTransform;
    private Vector3 _playerVelocity;
    private bool _isStepping;
    private bool _groundedPlayer;
    private bool _isJumping;
    private Vector3 _moveDirection;

    private float _hp = 100;
    private int _hpMax = 100;
    private int _hpRegen = 1;


    private float _hunger;
    private float _hungerTimer;
    private float _hungerTimerValue;
    

    private void Start()
    {
        Singleton = this;
        CharController = GetComponent<CharacterController>();
        Transform = GetComponent<Transform>();
        _cameraTransform = Camera.main.transform;
        _hungerTimerValue = AllObjects.Singleton.HungerTimerValue;

        if (PlayerPrefs.HasKey("Class"))
        {
            if(PlayerPrefs.GetInt("Class") == 1)
            {
                _hpMax = 120;
                HealthChange(20);
                _hpRegen = 2;
            }
        }

        if (PlayerPrefs.HasKey("Hunger"))
        {
            _hunger = PlayerPrefs.GetInt("Hunger");
            AllObjects.Singleton.HungerImage.fillAmount = _hunger / 100;
            AllObjects.Singleton.HungerText.text = $"{_hunger}%";
        }
        else
        {
            PlayerPrefs.SetInt("Hunger", 100);
            _hunger = PlayerPrefs.GetInt("Hunger");
            AllObjects.Singleton.HungerImage.fillAmount = _hunger / 100;
            AllObjects.Singleton.HungerText.text = $"{_hunger}%";
        }
    }

    private void Update()
    {
        _cameraTransform.position = new Vector3(Transform.position.x - 5, 15, Transform.position.z - 5.5f);

        #region Movement
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
        _moveDirection.x = AllObjects.Singleton.JoyController.Horizontal() * AllObjects.Singleton.PlayerSpeed;
        _moveDirection.z = AllObjects.Singleton.JoyController.Vertical() * AllObjects.Singleton.PlayerSpeed;

        if (Vector3.Angle(Vector3.forward, _moveDirection) > 1f || Vector3.Angle(Vector3.forward, _moveDirection) == 0)
        {
            transform.rotation = Quaternion.LookRotation(Vector3.RotateTowards(transform.forward, _moveDirection, AllObjects.Singleton.PlayerSpeed, 0.0f));
        }

        if (_isJumping && _groundedPlayer)
        {
            _playerVelocity.y += Mathf.Sqrt(AllObjects.Singleton.JumpHeight * -3.0f * _gravityValue);
        }

        _playerVelocity.y += _gravityValue * Time.deltaTime;

        #endregion

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

        _hungerTimer += Time.deltaTime;
        if (_hungerTimer >= _hungerTimerValue)
        {
            HungerChange(-1);
            _hungerTimer = 0;
        }
    }

    public void Jump()
    {
        StartCoroutine(JumpWait());
    }

    public void HealthChange(int value)
    {
        if (_hp > 0)
        {
            _hp += value;
            AllObjects.Singleton.HpImage.fillAmount = _hp / _hpMax;
            AllObjects.Singleton.HpText.text = $"{_hp}%";
        }
    }

    public void HungerChange(int value)
    {
        if (_hunger > 0)
        {
            PlayerPrefs.SetInt("Hunger", (int)_hunger + value);
            _hunger = PlayerPrefs.GetInt("Hunger");
            AllObjects.Singleton.HungerImage.fillAmount = _hunger / 100;
            AllObjects.Singleton.HungerText.text = $"{_hunger}%";
        }

        if(_hunger <= 100 && _hunger >= 50)
        {
            if(_hp < _hpMax)
            {
                HealthChange(_hpRegen);
            }
        }
        else if (_hunger < 50 && _hunger >= 25)
        {
            _hungerTimerValue = AllObjects.Singleton.HungerTimerValue * 1.5f;
            HealthChange(-1);
        }
        else if(_hunger < 25 && _hunger >= 10)
        {
            _hungerTimerValue = AllObjects.Singleton.HungerTimerValue * 2f;
            HealthChange(-2); 
        }
        else if(_hunger < 10)
        {
            _hungerTimerValue = AllObjects.Singleton.HungerTimerValue * 3f;
            HealthChange(-3);
        }
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
