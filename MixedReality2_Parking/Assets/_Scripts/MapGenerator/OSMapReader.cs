using System.Xml;
using System.Collections.Generic;
using UnityEngine;

public class OSMapReader : MonoBehaviour {

    [SerializeField]
    private TextAsset MapFile;

    [SerializeField]
    private OSMapDebugDisplay MapDebugDisplay;

    [SerializeField]
    private GameObject GroundPlane;

    public OSMapInfo MapInfo;

    public bool IsReady { get; private set; }

	// Use this for initialization
	void Start () {
        //TextAsset mapFileAsset = Resources.Load<TextAsset>(mapFile.name);

        // init data structures
        MapInfo.Nodes = new Dictionary<ulong, OSMNode>();
        MapInfo.Ways = new List<OSMWay>();

        // load xml map file
        XmlDocument xmlMap = new XmlDocument();
        xmlMap.LoadXml(MapFile.text);

        SetBounds(xmlMap.SelectSingleNode("/osm/bounds"));
        GetNodes(xmlMap.SelectNodes("/osm/node"));
        GetWays(xmlMap.SelectNodes("/osm/way"));

        float minx = (float)MercatorProjection.lonToX(MapInfo.Bounds.MinLongitude);
        float maxx = (float)MercatorProjection.lonToX(MapInfo.Bounds.MaxLongitude);
        float miny = (float)MercatorProjection.latToY(MapInfo.Bounds.MinLatitude);
        float maxy = (float)MercatorProjection.latToY(MapInfo.Bounds.MaxLatitude);

        GroundPlane.transform.localScale = new Vector3((maxx - minx) / 2, 1, (maxy - miny) / 2);

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
            OSMNode osmNode = new OSMNode(xmlNode);
            MapInfo.Nodes[osmNode.ID] = osmNode;
        }
    }

    private void GetWays(XmlNodeList xmlNodeList)
    {
        foreach(XmlNode xmlNode in xmlNodeList)
        {
            OSMWay osmWay = new OSMWay(xmlNode);
            MapInfo.Ways.Add(osmWay);
        }
    }
	
	// Update is called once per frame
	void Update () {
        if(null != MapDebugDisplay)
        {
            MapDebugDisplay.DisplayWays(MapInfo);
        }
    }
}