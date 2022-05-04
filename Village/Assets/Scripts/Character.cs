using UnityEngine;
using System.Collections;

public class Character : MonoBehaviour
{
    public static Character Singleton { get; private set; }
    public Transform Transform { get; private set; }
    public CharacterController CharController { get; set;}

    private float _playerSpeed;
    public float AttackSpeed { get; private set; }
    private float _stepTime;
    private float _gravityValue = -9.81f;
    private Transform _cameraTransform;
    private Vector3 _playerVelocity;
    private bool _isStepping;
    private bool _groundedPlayer;
    private Vector3 _moveDirection;
    private Animator _anim;

    private float _hp = 100;
    private int _hpMax = 100;
    private int _hpRegen = 1;

    private float _hunger = 100;
    private float _hungerTimer;
    private float _hungerTimerValue;

    public float Fatigue = 100;
    private float _fatigueTimer;
    private float _fatigueTimerValue;

    private void Start()
    {
        Singleton = this;
        CharController = GetComponent<CharacterController>();
        Transform = GetComponent<Transform>();
        _cameraTransform = Camera.main.transform;
        _anim = GetComponent<Animator>();

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
        }

        HungerChange(0);

        if (PlayerPrefs.HasKey("Fatigue"))
        {
            Fatigue = PlayerPrefs.GetInt("Fatigue");
        }

        FatigueChange(0);


        _hungerTimerValue = AllObjects.Singleton.HungerTimerValue;
        _fatigueTimerValue = AllObjects.Singleton.FatigueTimerValue;
        AttackSpeed = AllObjects.Singleton.AttackSpeed;
        _playerSpeed = AllObjects.Singleton.PlayerSpeed;
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
            CharController.Move(_moveDirection * _playerSpeed * Time.deltaTime);
        }

        _moveDirection.x = AllObjects.Singleton.JoyController.Horizontal();
        _moveDirection.z = AllObjects.Singleton.JoyController.Vertical();

        if (CharController.velocity.magnitude > 0.5f)
        {
            _anim.Play("Run");
        }
        else
        {
            _anim.Play("Idle");
        }

        if (!_isStepping)
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

        _playerVelocity.y += _gravityValue * Time.deltaTime;
        CharController.Move(_playerVelocity * Time.deltaTime);


        if (Vector3.Angle(Vector3.forward, _moveDirection) > 1f || Vector3.Angle(Vector3.forward, _moveDirection) == 0)
        {
            transform.rotation = Quaternion.LookRotation(Vector3.RotateTowards(transform.forward, _moveDirection, _playerSpeed, 0.0f));
        }
       
        #endregion

        _hungerTimer += Time.deltaTime;
        if (_hungerTimer >= _hungerTimerValue)
        {
            HungerChange(-1);
            _hungerTimer = 0;
        }

        _fatigueTimer += Time.deltaTime;
        if(_fatigueTimer >= _fatigueTimerValue)
        {
            FatigueChange(-1);
            _fatigueTimer = 0;
        }
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
            _hunger += value;
            PlayerPrefs.SetInt("Hunger", (int)_hunger);
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

    public void FatigueChange(int value)
    {
        if (Fatigue > 0)
        {
            Fatigue += value;
            PlayerPrefs.SetInt("Fatigue", (int)Fatigue);
            AllObjects.Singleton.FatigueImage.fillAmount = Fatigue / 100;
            AllObjects.Singleton.FatigueText.text = $"{Fatigue}%";
        }

        if (Fatigue <= 100 && Fatigue >= 50)
        {
            _playerSpeed = AllObjects.Singleton.PlayerSpeed;
            AttackSpeed = AllObjects.Singleton.AttackSpeed;
        }
        else if (Fatigue < 50 && Fatigue >= 25)
        {
            AttackSpeed = AllObjects.Singleton.AttackSpeed * 1.25f;
        }
        else if (Fatigue < 25 && Fatigue >= 10)
        {
            AttackSpeed = AllObjects.Singleton.AttackSpeed * 1.5f;
            _playerSpeed = AllObjects.Singleton.PlayerSpeed / 1.25f;
        }
        else if (Fatigue < 10)
        {
            AttackSpeed = AllObjects.Singleton.AttackSpeed * 2f;
            _playerSpeed = AllObjects.Singleton.PlayerSpeed / 1.5f;
        }
    }
  
    IEnumerator Step()
    {
        _isStepping = true;
        //AllObjects.Singleton.StepAudio.PlayOneShot(AllObjects.Singleton.FirstSteps[Random.Range(0, AllObjects.Singleton.FirstSteps.Length)]);
        yield return new WaitForSeconds(_stepTime);
        _isStepping = false;
    }
}
