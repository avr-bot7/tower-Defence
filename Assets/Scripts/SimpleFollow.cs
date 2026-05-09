using UnityEngine;

public class SimpleFollow : MonoBehaviour
{
    Transform _target;
    float _speed;

    public bool Arrived { get; private set; }

    public void Init(Transform target, float speed)
    {
        _target = target;
        _speed = speed;
    }

    void Update()
    {
        if (_target == null)
        {
            Arrived = true;
            return;
        }

        transform.position = Vector3.MoveTowards(transform.position, _target.position, _speed * Time.deltaTime);
        if (Vector3.Distance(transform.position, _target.position) < 0.3f)
            Arrived = true;
    }
}
