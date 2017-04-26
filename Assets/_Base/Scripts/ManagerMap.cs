using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagerMap : MonoBehaviour
{

    [SerializeField] private GameObject prefabMap;
    private GameObject map;

    void Awake()
    {
        Director.Instance.managerMap = this;
    }


    public void SummonMap()
    {
        map = Instantiate(prefabMap, this.transform) as GameObject;
    }

    private void RemoveMap()
    {
        if (map != null)
        {
            Destroy(map);
            map = null;
        }
    }

    public void Reset()
    {
        RemoveMap();
    }
}
