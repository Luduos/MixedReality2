using System.Xml;
using System.Collections.Generic;
using UnityEngine;

public class HighwayNode {

    /// <summary>
    /// Node ID
    /// </summary>
	public ulong ID { get; private set; }
    
    /// <summary>
    /// Position
    /// </summary>
    public Vector3 Position { get; private set; }


    /// <summary>
    /// All Nodes this Node is connected to
    /// </summary>
    public List<HighwayNode> Neighbours { get; set; }

    public bool IsParkingLotEntry { get; set; }

    public string Name { get; set; }

    public HighwayNode(XmlNode xmlNode, Vector3 MapCenter)
    {
        // OSM attributes
        ID = XMLHelper.GetAttribute<ulong>("id", xmlNode.Attributes);
        float Latitude = XMLHelper.GetAttribute<float>("lat", xmlNode.Attributes);
        float Longitude = XMLHelper.GetAttribute<float>("lon", xmlNode.Attributes);

        // Convertion of OSM position attributes to unity positions
        float XCoord = (float)MercatorProjection.lonToX(Longitude);
        float YCoord = (float)MercatorProjection.latToY(Latitude);

        Position = new Vector3(XCoord, 0.0f, YCoord) - MapCenter;
        IsParkingLotEntry = false;
        Neighbours = new List<HighwayNode>();
    }

    public HighwayNode(OSMNode osmNode, Vector3 MapCenter, string Name)
    {
        ID = osmNode.ID;

        float XCoord = osmNode.XCoord;
        float YCoord = osmNode.YCoord;

        Position = new Vector3(XCoord, 0.0f, YCoord) - MapCenter;
        Neighbours = new List<HighwayNode>();
        this.Name = Name;
    }

}
