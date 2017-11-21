using UnityEngine;
using System.Collections.Generic;
/// <summary>
/// Debug Display for OSMaps
/// </summary>
public class OSMapDebugDisplay : MonoBehaviour{

    [SerializeField]
    private Color HighwayColor = Color.red;

    [SerializeField]
    private Color BuildingColor = Color.green;

    [SerializeField]
    private Color ParkingColor = Color.blue;

    [SerializeField]
    private Color UnknownColor = Color.black;

    public void DisplayWays(OSMapInfo mapInfo)
    {
        DrawWayCollection(mapInfo, mapInfo.Roads, HighwayColor);
        DrawWayCollection(mapInfo, mapInfo.ParkSpaces, ParkingColor);
        DrawWayCollection(mapInfo, mapInfo.Buildings, BuildingColor);
        DrawWayCollection(mapInfo, mapInfo.Unknown, UnknownColor);
    }

    private void DrawWayCollection(OSMapInfo mapInfo, List<OSMWay> wayCollection, Color color)
    {
        foreach (OSMWay currentWay in wayCollection)
        {
            if (currentWay.Visible)
            {                
                DrawWay(mapInfo, currentWay, color);
            }
        }
    }

    private void DrawWay(OSMapInfo mapInfo, OSMWay way, Color color)
    {
        for (int i = 1; i < way.NodeIDs.Count; i++)
        {
            OSMNode p1 = mapInfo.Nodes[way.NodeIDs[i - 1]];
            OSMNode p2 = mapInfo.Nodes[way.NodeIDs[i]];

            Vector3 v1 = p1 - mapInfo.Bounds.Center;
            Vector3 v2 = p2 - mapInfo.Bounds.Center;

            Debug.DrawLine(v1, v2, color);            
        }
    }
}
