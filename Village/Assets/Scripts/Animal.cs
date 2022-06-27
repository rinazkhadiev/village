using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class Animal : MonoBehaviour
{
    [SerializeField] private int _distance = 7;

    private NavMeshAgent _navMesh;
    private Transform _transform;
    private Animator _anim;

    private float _attackCounter;

    public float Hp { get; private set; }

    public float MaxHP { get; private set; }

    private bool _isDead;
    private float _dieCounter;
    private bool _isRespawning;
    private bool _fullDead;

    private void Start()
    {
        _transform = GetComponent<Transform>();
        _anim = GetComponent<Animator>();
        Hp = Random.Range(AllObjects.Singleton.AnimalHpMin, AllObjects.Singleton.AnimalHpMax);
        MaxHP = Hp;
        StartCoroutine(AddNavMesh());
    }

    private void Update()
    {
        if (_navMesh != null)
        {
            if (_navMesh.baseOffset < 0 && _isRespawning)
            {
                _navMesh.baseOffset += Time.deltaTime * 0.5f;
                if (_navMesh.baseOffset >= 0)
                {
                    _isRespawning = false;
                }
            }

            if (!_isDead)
            {
                if (Character.Singleton.IsDead)
                {
                    if (Vector3.Distance(Character.Singleton.Transform.position, _transform.position) > _distance)
                    {
                        _navMesh.isStopped = true;
                        _anim.Play("eat");
                    }
                }
                else
                {
                    if (Vector3.Distance(Character.Singleton.Transform.position, _transform.position) > _distance)
                    {
                        _navMesh.isStopped = true;
                        _anim.Play("eat");
                    }
                    else if (Vector3.Distance(Character.Singleton.Transform.position, _transform.position) < _distance && Vector3.Distance(Character.Singleton.Transform.position, _transform.position) > 3)
                    {
                        _navMesh.isStopped = false;
                        _navMesh.SetDestination(Character.Singleton.Transform.position);
                        _anim.Play("run");
                    }
                    else if (Vector3.Distance(Character.Singleton.Transform.position, _transform.position) < 3)
                    {
                        _navMesh.isStopped = true;
                        _anim.Play("attack");
                        Attack();
                        _attackCounter += Time.deltaTime;
                    }
                }
            }
            else if (_isDead && !_fullDead)
            {
                _dieCounter += Time.deltaTime;
                if (_dieCounter > 2 && _dieCounter < 4)
                {
                    _navMesh.baseOffset -= Time.deltaTime;
                }
                else if (_dieCounter > 4)
                {
                    StartCoroutine(Respawn());
                }
            }
        }
    }

    public void Attack()
    {
        if (_attackCounter > AllObjects.Singleton.AnimalAttackSpeed)
        {
            Character.Singleton.HealthChange(Random.Range(AllObjects.Singleton.AnimalMaxDamage, AllObjects.Singleton.AnimalMinDamage));
            _attackCounter = 0;
        }
    }

    public void TakeDamage()
    {
        Hp -= AllObjects.Singleton.PlayerDamage;
        if (Hp <= 0)
        {
            _dieCounter = 0;
            _isDead = true;
            _anim.Play("die");
            UserInterface.Singleton.DoTask((int)Tasks.eda);
            AllObjects.Singleton.sv.MakedMeets[0]++;
            AllObjects.Singleton.SaveUpdate();

            Tutorial.Singleton.DoStep(ref Tutorial.Singleton.Hunting, (int)Steps.Hunting);
        }
    }

    IEnumerator Respawn()
    {
        _fullDead = true;
        yield return new WaitForSeconds(Random.Range(60, 120));
        _isRespawning = true;
        _isDead = false;
        _fullDead = false;
        Hp = Random.Range(2, 4);
        MaxHP = Hp;
    }

    IEnumerator AddNavMesh()
    {
        yield return new WaitForSeconds(5);
        _navMesh = gameObject.AddComponent<NavMeshAgent>();
        _navMesh.speed = AllObjects.Singleton.AnimalSpeed;
    }
}
