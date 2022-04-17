using UnityEngine;

public class DistanceSetActive : MonoBehaviour
{
    private Transform Transform;

    [SerializeField] private int _distance;
    [SerializeField] private GameObject _activeObj;

    private void Start()
    {
        Transform = GetComponent<Transform>();
    }

    private void Update()
    {
        if(Vector3.Distance(Character.Singleton.Transform.position, Transform.position) < _distance)
        {
            _activeObj.SetActive(true);
        }
        else
        {
            _activeObj.SetActive(false);
        }
    }
}
