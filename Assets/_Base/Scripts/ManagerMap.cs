using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagerMap : MonoBehaviour
{

    [SerializeField] private GameObject prefabMap;
    private GameObject map;

    public ChunkLoader chunkLoader;

    void Awake()
    {
        Director.Instance.managerMap = this;
    }


    public void SummonMap()
    {
        map = Instantiate( prefabMap, this.transform ) as GameObject;
        chunkLoader = map.transform.GetComponentInChildren<ChunkLoader>();
#if DEBUG
        Debug.Log( "ChunkLoader found: " + chunkLoader.name );
#endif
    }

    private void RemoveMap()
    {
        if (map != null)
        {
            Destroy( map );
            map = null;
        }
    }

    public void Reset()
    {
        RemoveMap();
    }
}
