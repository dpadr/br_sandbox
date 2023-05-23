using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BR_EntityManager : MonoBehaviourPun
{
    public static BR_EntityManager Instance;

    private List<BR_Entity> _entities;
    public List<BR_Entity> GetEntities() { return _entities; }

    [PunRPC]
    public void AddEntity(BR_Entity entity)
    {
        if(!_entities.Contains(entity))
        {
            _entities.Add(entity);
        }
    }

    [PunRPC]
    public void RemoveEntity(BR_Entity entity)
    {
        _entities.Remove(entity);
    }

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    private void LateUpdate()
    {
        foreach (BR_Entity entity in _entities)
        {
            if(entity.CurrentState == BR_Entity.EntityState.Destroy)
            {
                Destroy(entity);
            }
        }
    }
}
