using System.Xml;
using UnityEngine;

public class OSMNode : OSMDataElement {

    /// <summary>
    /// Node ID
    /// </summary>
	public ulong ID { get; private set; }

    /// <summary>
    /// OSM latitude position
    /// </summary>
    public float Latitude { get; private set; }
    /// <summary>
    /// OSM longitude position
    /// </summary>
    public float Longitude { get; private set; }

    /// <summary>
    /// X coordinate in Unity space
    /// </summary>
    public float XCoord { get; private set; }
    /// <summary>
    /// Y coordinate in Unity space
    /// </summary>
    public float YCoord { get; private set; }

    public Vector3 WorldCoord { get; private set; }

    /// <summary>
    /// Converts an OSMNode into a Unity Vector3
    /// </summary>
    /// <param name="osmNode">Node to convert</param>
    public static implicit operator Vector3(OSMNode osmNode)
    {
        return new Vector3(osmNode.XCoord, 0, osmNode.YCoord);
    }

    /// <summary>
    /// Constructs the OSMNode based on the attributes in the given XmlNode
    /// </summary>
    /// <param name="xmlNode">XmlNode with construction attributes</param>
    public OSMNode(XmlNode xmlNode, Vector3 mapCenter)
    {

        // OSM attributes
        ID = GetAttribute<ulong>("id", xmlNode.Attributes);
        Latitude = GetAttribute<float>("lat", xmlNode.Attributes);
        Longitude = GetAttribute<float>("lon", xmlNode.Attributes);

        // Convertion of OSM position attributes to unity positions
        XCoord = (float)MercatorProjection.lonToX(Longitude);
        YCoord = (float)MercatorProjection.latToY(Latitude);

        WorldCoord = new Vector3(XCoord - mapCenter.x, -mapCenter.y, YCoord - mapCenter.z);
    }
}
