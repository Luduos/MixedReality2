using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour {

    [SerializeField]
    private int RandomSeed;

    [SerializeField]
    private OSMapReader MapReader;

    private HighwayNode CurrentNode;

    private IEnumerator Start()
    {
        while (!MapReader.IsReady)
        {
            yield return null;
        }

        // Choose random first node
        List<HighwayNode> nodes = Enumerable.ToList(MapReader.NavInfo.Nodes.Values);
        int size = MapReader.NavInfo.Nodes.Count;
        Random.InitState(RandomSeed);
        CurrentNode = nodes[Random.Range(0, size)];

        this.transform.position = CurrentNode.Position + new Vector3(0.0f, 2.0f, 0.0f);
    }

    private void Update()
    {
        
    }


}
