using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class Animal : MonoBehaviour
{
    private NavMeshAgent _navMesh;
    private Transform _transform;
    private Animator _anim;

    private float _attackCounter;

    private int _hp;

    private bool _isDead;
    private float _dieCounter;
    private bool _isRespawning;
    private bool _fullDead;

    private void Start()
    {
        _transform = GetComponent<Transform>();
        _anim = GetComponent<Animator>();
        _hp = Random.Range(AllObjects.Singleton.AnimalHpMin, AllObjects.Singleton.AnimalHpMax);
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
                if (Vector3.Distance(Character.Singleton.Transform.position, _transform.position) > AllObjects.Singleton.AnimalDistance)
                {
                    _navMesh.isStopped = true;
                    _anim.Play("eat");
                }
                else if (Vector3.Distance(Character.Singleton.Transform.position, _transform.position) < AllObjects.Singleton.AnimalDistance && Vector3.Distance(Character.Singleton.Transform.position, _transform.position) > 2)
                {
                    _navMesh.isStopped = false;
                    _navMesh.SetDestination(Character.Singleton.Transform.position);
                    _anim.Play("run");
                }
                else if (Vector3.Distance(Character.Singleton.Transform.position, _transform.position) < 2)
                {
                    _navMesh.isStopped = true;
                    _anim.Play("attack");
                    Attack();
                    _attackCounter += Time.deltaTime;
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
        _hp--;
        if (_hp <= 0)
        {
            _dieCounter = 0;
            _isDead = true;
            _anim.Play("die");
            UserInterface.Singleton.DoTask((int)Tasks.eda);
        }
    }

    IEnumerator Respawn()
    {
        _fullDead = true;
        yield return new WaitForSeconds(Random.Range(60, 120));
        _isRespawning = true;
        _isDead = false;
        _fullDead = false;
        _hp = Random.Range(2, 4);
    }

    IEnumerator AddNavMesh()
    {
        yield return new WaitForSeconds(5);
        _navMesh = gameObject.AddComponent<NavMeshAgent>();
        _navMesh.speed = AllObjects.Singleton.AnimalSpeed;
    }
}
