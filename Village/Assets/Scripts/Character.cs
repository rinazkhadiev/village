using UnityEngine;
using System.Collections;

public class Character : MonoBehaviour
{
    public static Character Singleton { get; private set; }
    public Transform Transform { get; set; }
    public CharacterController CharController { get; set; }

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
    private Vector3 _startPosition;

    public float Hp { get; private set; } = 100;
    public int HpMax { get; private set; } = 100;
    public bool IsDead { get; set; }
    private int _hpRegen = 1;

    public float Hunger {get;private set;} = 100;
    private float _hungerTimer;
    private float _hungerTimerValue;

    public float Fatigue {get;set;}= 100;
    private float _fatigueTimer;
    private float _fatigueTimerValue;

    private void Start()
    {
        Singleton = this;
        CharController = GetComponent<CharacterController>();
        Transform = GetComponent<Transform>();
        _cameraTransform = Camera.main.transform;
        _anim = GetComponent<Animator>();

        if (PlayerPrefs.HasKey("Hunger"))
        {
            Hunger = PlayerPrefs.GetInt("Hunger");
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
        _startPosition = Transform.position;

        if (PlayerPrefs.HasKey("Class"))
        {
            if (PlayerPrefs.GetInt("Class") == 1)
            {
                HpMax = 120;
                HealthChange(20);
                _hpRegen = 2;
            }
            else if (PlayerPrefs.GetInt("Class") == 2)
            {
                AllObjects.Singleton.PlayerDamage = 1.5f;
                _hungerTimerValue *= 2;
            }
            else
            {
                _playerSpeed *= 1.25f;
                AllObjects.Singleton.StoneTakeMax /= 0.8f;
                AllObjects.Singleton.TreeTakeMax /= 0.8f;
            }
        }
    }

    private void Update()
    {
        if (!IsDead)
        {
            _cameraTransform.position = new Vector3(Transform.position.x - 5, _cameraTransform.position.y, Transform.position.z - 5.5f);
            _cameraTransform.rotation = Quaternion.Euler(new Vector3(60, 45, 0));

            Vector3 cameraTargetPositionY = new Vector3(_cameraTransform.position.x, 15 + (int)Transform.position.y, _cameraTransform.position.z);
            _cameraTransform.position = Vector3.Lerp(_cameraTransform.position, cameraTargetPositionY, Time.deltaTime * 2f);
        }

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
        if (!IsDead)
        {
            if (AllObjects.Singleton.CharacterIsBusy)
            {
                _anim.Play(AllObjects.Singleton.WhichAnimation);
            }
            else
            {
                if (AllObjects.Singleton.CharacterIsAttack)
                {
                    _anim.Play("Attack");
                }
                else
                {
                    _fatigueTimer += Time.deltaTime;
                    if (CharController.velocity.magnitude > 0.5f)
                    {
                        _anim.Play("Run");


                        if (_fatigueTimer >= _fatigueTimerValue)
                        {
                            FatigueChange(-1);
                            _fatigueTimer = 0;
                        }
                    }
                    else
                    {
                        _anim.Play("Idle");

                        if (_fatigueTimer >= _fatigueTimerValue)
                        {
                            FatigueChange(1);
                            _fatigueTimer = 0;
                        }
                    }
                }
            }
        }
        else
        {
            _anim.Play("Death");
        }

        if (!_isStepping)
        {
            if (CharController.velocity.magnitude > 4f)
            {
                _stepTime = 0.4f;
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
     
	if (!AllObjects.Singleton.CharacterIsBusy)
        {
	    CharController.Move(_playerVelocity * Time.deltaTime);
        }

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
    }

    public void Respawn()
    {
           StartCoroutine(TeleportWait());
    }

    IEnumerator TeleportWait()
    {
            Hp = HpMax;
            AllObjects.Singleton.HpImage.fillAmount = Hp / HpMax;
            AllObjects.Singleton.HpText.text = $"{Hp}%";
            AllObjects.Singleton.PoorPanel.SetActive(false);
            IsDead = false;

        AllObjects.Singleton.CharacterIsBusy = true;
        Transform.position = _startPosition;
        yield return new WaitForSeconds(0.5f);
        AllObjects.Singleton.CharacterIsBusy = false;
    }

    public void HealthChange(int value)
    {
        if (Hp > 0)
        {
            Hp += value;

            if(Hp > HpMax)
            {
                Hp = HpMax;
	    }

            AllObjects.Singleton.HpImage.fillAmount = Hp / HpMax;
            AllObjects.Singleton.HpText.text = $"{Hp}%";
        }
        else
        {
            AllObjects.Singleton.PoorPanel.SetActive(true);
            IsDead = true;
        }
    }

    public void HungerChange(int value)
    {
        if (Hunger > 0)
        {
            Hunger += value;
            if (Hunger > 100)
            {
                Hunger = 100;
            }
            PlayerPrefs.SetInt("Hunger", (int)Hunger);
            AllObjects.Singleton.HungerImage.fillAmount = Hunger / 100;
            AllObjects.Singleton.HungerText.text = $"{Hunger}%";
        }

        if(Hunger <= 100 && Hunger >= 50)
        {
            if(Hp < HpMax)
            {
                HealthChange(_hpRegen);
            }
        }
        else if (Hunger < 50 && Hunger >= 25)
        {
            _hungerTimerValue = AllObjects.Singleton.HungerTimerValue * 1.5f;
            HealthChange(-1);
        }
        else if(Hunger < 25 && Hunger >= 10)
        {
            _hungerTimerValue = AllObjects.Singleton.HungerTimerValue * 2f;
            HealthChange(-2); 
        }
        else if(Hunger < 10)
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
            if (Fatigue > 100)
            {
                Fatigue = 100;
            }
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "NeedABridge")
        {
            StartCoroutine(NeedABridge());
        }
    }

    IEnumerator Step()
    {
        _isStepping = true;
        AllObjects.Singleton.StepAudio.PlayOneShot(AllObjects.Singleton.FirstSteps[Random.Range(0, AllObjects.Singleton.FirstSteps.Length)]);
        yield return new WaitForSeconds(_stepTime);
        _isStepping = false;
    }

    IEnumerator NeedABridge()
    {
        AllObjects.Singleton.NeedABridgeText.SetActive(true);
        yield return new WaitForSeconds(3);
        AllObjects.Singleton.NeedABridgeText.SetActive(false);

    }
}
