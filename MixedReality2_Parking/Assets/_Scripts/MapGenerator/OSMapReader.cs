using System.Xml;
using System.Collections.Generic;
using UnityEngine;

public class OSMapReader : MonoBehaviour {

    [SerializeField]
    private TextAsset MapFile;

    [SerializeField]
    private OSMapDebugDisplay MapDebugDisplay;

    [SerializeField]
    private NavigationDebugDisplay NavDebugDisplay;

    [SerializeField]
    private GameObject GroundPlane;

    public OSMapInfo MapInfo;

    public NavigationInfo NavInfo;

    public bool IsReady { get; private set; }

	// Use this for initialization
	void Start () {
        //TextAsset mapFileAsset = Resources.Load<TextAsset>(mapFile.name);

        // init data structures
        MapInfo.Nodes = new Dictionary<ulong, OSMNode>();
        MapInfo.Unknown = new List<OSMWay>();
        MapInfo.Roads = new List<OSMWay>();
        MapInfo.Buildings = new List<OSMWay>();
        MapInfo.ParkSpaces = new List<OSMWay>();

        NavInfo.Nodes = new Dictionary<ulong, HighwayNode>();

        // load xml map file
        XmlDocument xmlMap = new XmlDocument();
        xmlMap.LoadXml(MapFile.text);

        // load different xmlNode types
        SetBounds(xmlMap.SelectSingleNode("/osm/bounds"));
        GetNodes(xmlMap.SelectNodes("/osm/node"));
        GetWays(xmlMap.SelectNodes("/osm/way"));

        // Connect and spawn Debug Displays
        ConnectHighwayNodes();
        MarkParkingEntries();
        NavDebugDisplay.SpawnNavigationNodes(NavInfo);
        NavDebugDisplay.SpawnParkingLotMarkers(MapInfo);
        NavDebugDisplay.SpawnRoads(MapInfo);
        NavDebugDisplay.SpawnBuildings(MapInfo);

        // read and convert map boundaries
        float minx = (float)MercatorProjection.lonToX(MapInfo.Bounds.MinLongitude);
        float maxx = (float)MercatorProjection.lonToX(MapInfo.Bounds.MaxLongitude);
        float miny = (float)MercatorProjection.latToY(MapInfo.Bounds.MinLatitude);
        float maxy = (float)MercatorProjection.latToY(MapInfo.Bounds.MaxLatitude);

        // scale the groundplane to cover the whole map area
        GroundPlane.transform.localScale = new Vector3((maxx - minx) / 2, 1, (maxy - miny) / 2);

        // signalize that the reading of the map file is done
        IsReady = true;
    }

    private void SetBounds(XmlNode xmlNode)
    {
        MapInfo.Bounds = new OSMBounds(xmlNode);
    }

    private void GetNodes(XmlNodeList xmlNodeList)
    {
        foreach(XmlNode xmlNode in xmlNodeList)
        {
            CreateOSMNode(xmlNode);
        }
    }

    private void CreateOSMNode(XmlNode xmlNode)
    {
        OSMNode osmNode = new OSMNode(xmlNode, MapInfo.Bounds.Center);
        MapInfo.Nodes[osmNode.ID] = osmNode;
    }

    private void GetWays(XmlNodeList xmlNodeList)
    {
        foreach(XmlNode xmlNode in xmlNodeList)
        {
            OSMWay osmWay = new OSMWay(xmlNode);
            if (OSMWayType.Highway == osmWay.WayType)
            {
                MapInfo.Roads.Add(osmWay);
                CreateHighwayNodesFromWay(osmWay);
            }
            else if (OSMWayType.Parking == osmWay.WayType)
            {
                MapInfo.ParkSpaces.Add(osmWay);
            }
            else if (OSMWayType.Building == osmWay.WayType)
            {
                MapInfo.Buildings.Add(osmWay);
            }
            else
            {
                MapInfo.Unknown.Add(osmWay);
            }
        }
    }

    private void CreateHighwayNodesFromWay(OSMWay osmWay)
    {
        foreach(ulong nodeID in osmWay.NodeIDs)
        {
            HighwayNode highwayNode;
            NavInfo.Nodes.TryGetValue(nodeID, out highwayNode);
            if(null == highwayNode)
            {
                OSMNode osmNode = MapInfo.Nodes[nodeID];
                highwayNode = new HighwayNode(osmNode, MapInfo.Bounds.Center, osmWay.Name);
                NavInfo.Nodes[highwayNode.ID] = highwayNode;
            }
            else
            {
                if (highwayNode.Name.Equals("") && !osmWay.Name.Equals(""))
                {
                    highwayNode.Name = osmWay.Name;
                    NavInfo.Nodes[highwayNode.ID] = highwayNode;
                }
            } 
        }
    }

    private void MarkParkingEntries()
    {
        foreach(OSMWay parkingWay in MapInfo.ParkSpaces)
        {
            foreach(ulong parkingNodeID in parkingWay.NodeIDs)
            {
                HighwayNode possibleHighwayParkingNode;
                bool foundNode = NavInfo.Nodes.TryGetValue(parkingNodeID, out possibleHighwayParkingNode);
                if (foundNode)
                    possibleHighwayParkingNode.IsParkingLotEntry = true;
            }
        }
    }

    private void ConnectHighwayNodes()
    {
        foreach(OSMWay osmWay in MapInfo.Roads)
        {
            ConnectHighwayNodesFromWay(osmWay);
        }
    }

    private void ConnectHighwayNodesFromWay(OSMWay osmWay)
    {
        if (osmWay.Visible && osmWay.NodeIDs.Count > 0)
        {
            HighwayNode currentNode = NavInfo.Nodes[osmWay.NodeIDs[0]];
            for (int i = 1; i < osmWay.NodeIDs.Count; ++i)
            {
                HighwayNode nextNode = NavInfo.Nodes[osmWay.NodeIDs[i]];
                currentNode.Neighbours.Add(nextNode);
                nextNode.Neighbours.Add(currentNode);
                currentNode = nextNode;
            }
        }
    }
	/*
	// Update is called once per frame
	void Update () {
        if(null != MapDebugDisplay)
        {
            MapDebugDisplay.DisplayWays(MapInfo);
        }
    }
    */
}