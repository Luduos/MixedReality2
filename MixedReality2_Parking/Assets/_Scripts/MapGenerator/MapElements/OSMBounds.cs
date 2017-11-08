using UnityEngine;
using System.Xml;

/// <summary>
/// Used to display the Bounds of the constructed Map
/// </summary>
public class OSMBounds : OSMDataElement {

    /// <summary>
    /// OSM y-Axis, minimal latitude value
    /// </summary>
	public float MinLatitude { get; private set; }
    /// <summary>
    /// OSM y-Axis, maximal latitude value
    /// </summary>
    public float MaxLatitude { get; private set; }
    /// <summary>
    /// OSM x-Axis, minimal longitude value
    /// </summary>
    public float MinLongitude { get; private set; }
    /// <summary>
    /// OSM y-Axis, maximum longitude value
    /// </summary>
    public float MaxLongitude { get; private set; }

    /// <summary>
    /// The center of the map given in Unity coordinates
    /// </summary>
    public Vector3 Center { get; private set; }

    /// <summary>
    /// Construct the Bounds based on the attributes given in the XmlNode
    /// </summary>
    /// <param name="xmlNode">XmlNode with construction attributes</param>
    public OSMBounds(XmlNode xmlNode)
    {
        // Read the OSM position attributes
        MinLatitude = GetAttribute<float>("minlat", xmlNode.Attributes);
        MaxLatitude = GetAttribute<float>("maxlat", xmlNode.Attributes);
        MinLongitude = GetAttribute<float>("minlon", xmlNode.Attributes);
        MaxLongitude = GetAttribute<float>("maxlon", xmlNode.Attributes);

        // Convert OSM position attributes to Unity positions
        float latitudeAsY= (float)((MercatorProjection.latToY(MinLatitude) + MercatorProjection.latToY(MaxLatitude)) / 2);
        float longitudeAsX= (float)((MercatorProjection.lonToX(MinLongitude) + MercatorProjection.lonToX(MaxLongitude)) / 2);
        Center = new Vector3(longitudeAsX, 0, latitudeAsY);
    }
}
