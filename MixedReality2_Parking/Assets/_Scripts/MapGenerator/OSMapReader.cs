using System.Xml;
using System.Collections.Generic;
using UnityEngine;

public class OSMapReader : MonoBehaviour {

    [SerializeField]
    private TextAsset MapFile;

    [SerializeField]
    private GameObject GroundPlane;

    public OSMBounds Bounds { get; private set; }

    public Dictionary<ulong, OSMNode> Nodes { get; private set; }

    public List<OSMWay> Ways { get; private set; }

    public bool IsReady { get; private set; }

	// Use this for initialization
	void Start () {
        //TextAsset mapFileAsset = Resources.Load<TextAsset>(mapFile.name);

        // init data structures
        Nodes = new Dictionary<ulong, OSMNode>();
        Ways = new List<OSMWay>();

        // load xml map file
        XmlDocument xmlMap = new XmlDocument();
        xmlMap.LoadXml(MapFile.text);

        SetBounds(xmlMap.SelectSingleNode("/osm/bounds"));
        GetNodes(xmlMap.SelectNodes("/osm/node"));
        GetWays(xmlMap.SelectNodes("/osm/way"));

        float minx = (float)MercatorProjection.lonToX(Bounds.MinLongitude);
        float maxx = (float)MercatorProjection.lonToX(Bounds.MaxLongitude);
        float miny = (float)MercatorProjection.latToY(Bounds.MinLatitude);
        float maxy = (float)MercatorProjection.latToY(Bounds.MaxLatitude);

        GroundPlane.transform.localScale = new Vector3((maxx - minx) / 2, 1, (maxy - miny) / 2);


        IsReady = true;
    }

    private void SetBounds(XmlNode xmlNode)
    {
        Bounds = new OSMBounds(xmlNode);
    }

    private void GetNodes(XmlNodeList xmlNodeList)
    {
        foreach(XmlNode xmlNode in xmlNodeList)
        {
            OSMNode osmNode = new OSMNode(xmlNode);
            Nodes[osmNode.ID] = osmNode;
        }
    }

    private void GetWays(XmlNodeList xmlNodeList)
    {
        foreach(XmlNode xmlNode in xmlNodeList)
        {
            OSMWay osmWay = new OSMWay(xmlNode);
            Ways.Add(osmWay);
        }
    }
	
	// Update is called once per frame
	void Update () {
        foreach (OSMWay currentWay in Ways)
        {
            if (currentWay.Visible)
            {
                Color color = Color.green;
                if (OSMWayType.Highway == currentWay.WayType)
                {
                    color = Color.red;
                }else if(OSMWayType.Building == currentWay.WayType)
                {
                    color = Color.blue;
                }
                for (int i = 1; i < currentWay.NodeIDs.Count; i++)
                {
                    OSMNode p1 = Nodes[currentWay.NodeIDs[i - 1]];
                    OSMNode p2 = Nodes[currentWay.NodeIDs[i]];

                    Vector3 v1 = p1 - Bounds.Center;
                    Vector3 v2 = p2 - Bounds.Center;

                    Debug.DrawLine(v1, v2, color);
                }
            }
        }
    }
}