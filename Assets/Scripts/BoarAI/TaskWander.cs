using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BehaviorTree;

public class TaskWander : Node
{
    private Transform _transform;
    private float _waitTime = 1f; //seconds
    private float _waitCounter = 0f;
    private bool _waiting = false;
    private float _minWanderDist = 1f;
    private float _maxWanderDist = 4f;
    private Vector3 _wanderTarget;

    public TaskWander(Transform transform)
    {
        _transform = transform;
    }

    public override NodeState Evaluate()
    {
        Debug.LogWarning("t: " + _transform + "w: " + _wanderTarget);
        
        
        if (_waiting)
        {
            _waitCounter += Time.deltaTime;
            if (_waitCounter >= _waitTime)
            {
                _waiting = false;
            }
        }
        else
        {
            if(Vector3.Distance(_wanderTarget, _transform.position) < 0.01f)
            {
                _transform.position = _wanderTarget;
                _waitCounter = 0f;
                _waiting = true;
                Vector3 dir = Vector3.zero;
                Vector2 dir2 = Random.insideUnitCircle.normalized;
                dir.x = dir2.x;
                dir.z = dir2.y;
                dir.y = _transform.position.y;
                _wanderTarget = _transform.position + dir * Random.Range(_minWanderDist, _maxWanderDist);
            }
            else
            {
                _transform.position = Vector3.MoveTowards(
                    _transform.position,
                   _wanderTarget,
                   BoarBT.speed * Time.deltaTime);
                //_transform.LookAt(_wanderTarget);
            }
        }
            state = NodeState.RUNNING;
        return state;
    }
}