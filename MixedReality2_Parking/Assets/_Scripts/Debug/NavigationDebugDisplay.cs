using UnityEngine;
using System.Collections.Generic;

public class NavigationDebugDisplay : MonoBehaviour {

    [SerializeField]
    private GameObject HighwayNodeDebugPrefab;

    [SerializeField]
    private Material ParkingSpaceMaterial;

    [SerializeField]
    private Material RoadMaterial;

    [SerializeField]
    private Material BuildingMaterial;

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
                debugDisplay.transform.position = node.Position + new Vector3(0.0f, 2.0f, 0.0f);

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

    public void SpawnParkingLotMarkers(OSMapInfo mapInfo)
    {
        foreach(OSMWay parkSpace in mapInfo.ParkSpaces)
        {
            BuildingBuilder.CreateBuilding(parkSpace, mapInfo, ParkingSpaceMaterial);  
        }
    }

   public void SpawnRoads(OSMapInfo mapInfo)
    {
        foreach(OSMWay road in mapInfo.Roads)
        {
            RoadBuilder.CreateRoad(road, mapInfo, RoadMaterial);
        }
    }

    public void SpawnBuildings(OSMapInfo mapInfo)
    {
        foreach(OSMWay building in mapInfo.Buildings)
        {
            BuildingBuilder.CreateBuilding(building, mapInfo, BuildingMaterial);
        }
    }

    
}
