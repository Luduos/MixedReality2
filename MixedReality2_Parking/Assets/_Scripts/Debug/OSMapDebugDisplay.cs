using UnityEngine;

/// <summary>
/// Debug Display for OSMaps
/// </summary>
public class OSMapDebugDisplay : MonoBehaviour{

    [SerializeField]
    private Color HighwayColor = Color.red;

    [SerializeField]
    private Color BuildingColor = Color.blue;

    [SerializeField]
    private Color UnknownColor = Color.black;

    public void DisplayWays(OSMapInfo mapInfo)
    {
        foreach (OSMWay currentWay in mapInfo.Ways)
        {
            if (currentWay.Visible)
            {
                Color color = UnknownColor;
                if (OSMWayType.Highway == currentWay.WayType)
                {
                    color = HighwayColor;
                }
                else if (OSMWayType.Building == currentWay.WayType)
                {
                    color = BuildingColor;
                }
                for (int i = 1; i < currentWay.NodeIDs.Count; i++)
                {
                    OSMNode p1 = mapInfo.Nodes[currentWay.NodeIDs[i - 1]];
                    OSMNode p2 = mapInfo.Nodes[currentWay.NodeIDs[i]];

                    Vector3 v1 = p1 - mapInfo.Bounds.Center;
                    Vector3 v2 = p2 - mapInfo.Bounds.Center;

                    Debug.DrawLine(v1, v2, color);
                }
            }
        }
    }
}
