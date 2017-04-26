using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ChunkLoader : MonoBehaviour
{

    public GameObject chunkInit;
    public GameObject chunkFinish;
    public GameObject[] chunkMid;
    public float chunkHeight = 20f;
    public float chunkHeightFirst = 8f;

    public int chunkTotal = 10; // After this number of chunks, add finish
    public int chunkMax = 3; // This is the max number of chunks generated at a time

    // Chunks currently on play
    //public GameObject[] chunks; // This is were I will place the chunks
    public List<GameObject> chunks; // This is were I will place the chunks


    // Use this for initialization
    void Start()
    {
        chunks = new List<GameObject>();
        Init();
    }

    private void Init()
    {
        // Load beginning
        chunks.Add( Instantiate( chunkInit, new Vector3( 0f, chunkHeightFirst, 0f ), Quaternion.identity, transform ) );

        // and add the rest of chunks
        for (int i = chunks.Count; i < chunkMax; i++)
        {
            LoadAndAddChunk();
        }
    }

    private void LoadAndAddChunk()
    {

        float currentMaxHeight = chunks[chunks.Count-1].transform.position.y;
        float height = currentMaxHeight + chunkHeight;
        chunks.Add( Instantiate( chunkMid[Random.Range( 0, chunkMid.Length )], new Vector3( 0f, height, 0f ), Quaternion.identity, transform ) );
        //newChunk.transform.SetPositionAndRotation();
    }

    public void LoadNextChunk()
    {
        // Remove the one left behind
        Destroy( chunks[0] );
        chunks.RemoveAt( 0 );

        // And then add a new one
        LoadAndAddChunk();
    }


    #region Collisions
    //private void OnTriggerExit2D( Collider2D collision )
    //{
    //    if (collision.CompareTag( "Player" ))
    //    {
    //        Debug.Log( "OnTriggerExit2D collision.CompareTag( Player ))" );
    //        LoadNextChunk();
    //    }
    //}
    #endregion
}
