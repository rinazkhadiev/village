using UnityEngine.AI;
using System.Collections;
using UnityEngine;

public class Son : MonoBehaviour
{
    private NavMeshAgent _navMesh;
    private Transform _transform;
    private Animator _anim;

    private void Start()
    {
        _transform = GetComponent<Transform>();
        _anim = GetComponent<Animator>();
        StartCoroutine(AddNavMesh());
    }

    private void Update()
    {
        if (_navMesh != null && AllObjects.Singleton.SonWithCharacter)
        {
            if (Vector3.Distance(Character.Singleton.Transform.position, _transform.position) > 15)
            {
                _navMesh.isStopped = true;
                _anim.Play("eat");
            }
            else if (Vector3.Distance(Character.Singleton.Transform.position, _transform.position) < 15 && Vector3.Distance(Character.Singleton.Transform.position, _transform.position) > 2)
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
        else
        {
                _anim.Play("eat");
        }
    }

    IEnumerator AddNavMesh()
    {
        yield return new WaitForSeconds(5);
        _navMesh = gameObject.AddComponent<NavMeshAgent>();
        _navMesh.speed = 5;
    }
}
