using UnityEngine;

public class NavigationDebugDisplay : MonoBehaviour {

    [SerializeField]
    private GameObject HighwayNodeDebugPrefab;

    [SerializeField]
    private float Width = 3.0f;

    [SerializeField]
    private Color NormalHighwayLinkColor = Color.yellow;

    [SerializeField]
    private Color DeadEndColor = Color.red;

    [SerializeField]
    private Color CrossroadsColor = Color.green;

	public void SpawnNavigationNodes(NavigationInfo navInfo)
    {
        foreach(HighwayNode node in navInfo.Nodes.Values)
        {
            GameObject gameObject = GameObject.Instantiate(HighwayNodeDebugPrefab);
            MeshRenderer debugDisplay = gameObject.GetComponent<MeshRenderer>();
            Color color = Color.black;
            if (null != debugDisplay)
            {
                debugDisplay.transform.localScale = new Vector3(3.0f, 3.0f, 3.0f);
                debugDisplay.transform.position = node.Position;

                if (node.Neighbours.Count > 2)
                {
                    color = CrossroadsColor;
                }
                else if (node.Neighbours.Count == 2)
                {
                    color = NormalHighwayLinkColor;
                }
                else
                {
                    color = DeadEndColor;
                }
                debugDisplay.material.color = color;
            }   

            
        }
    }
}
