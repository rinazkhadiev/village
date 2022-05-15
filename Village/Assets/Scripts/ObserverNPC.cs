using UnityEngine.AI;
using System.Collections;
using UnityEngine;

public class ObserverNPC : MonoBehaviour
{
    private NavMeshAgent _navMesh;
    private Transform _transform;
    private Animator _anim;

    private void Start()
    {
        _transform = GetComponent<Transform>();
        _anim = GetComponent<Animator>();
        StartCoroutine(AddNavMesh());

        if (AllObjects.Singleton.sv.WifeIsFree)
        {
            AllObjects.Singleton.Wife.transform.position = AllObjects.Singleton.WifeHomePosition.position;
        }
    }

    private void Update()
    {
        if (!AllObjects.Singleton.sv.WifeIsFree && AllObjects.Singleton.WifeWithCharacter)
        {
            if (_navMesh != null)
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
                    _anim.Play("eat");
                }
            }
        }
    }

    public void StopDistanation()
    {
        _navMesh.isStopped = true;
    }

    IEnumerator AddNavMesh()
    {
        yield return new WaitForSeconds(5);
        _navMesh = gameObject.AddComponent<NavMeshAgent>();
        _navMesh.speed = AllObjects.Singleton.AnimalSpeed;

        if (!AllObjects.Singleton.sv.WifeIsFree)
        {
            gameObject.SetActive(false);
        }
    }
}
