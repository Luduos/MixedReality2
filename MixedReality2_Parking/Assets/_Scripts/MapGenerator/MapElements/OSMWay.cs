using System.Xml;
using System.Collections.Generic;


/// <summary>
/// OSM data object, which describes a collection of OSMNodes describing a shape or a road
/// </summary>
public class OSMWay : OSMDataElement
{

    /// <summary>
    /// Way ID
    /// </summary>
	public ulong ID { get; private set; }

    /// <summary>
    /// Whether or not a Way should be visible. Ways with Visible equal to false
    /// will probably be sorted out.
    /// </summary>
    public bool Visible { get; private set; }

    /// <summary>
    /// The name of the way
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// The waytype
    /// </summary>
    public OSMWayType WayType { get; private set; }

    /// <summary>
    /// List of Node IDs which describe the way
    /// </summary>
    public List<ulong> NodeIDs { get; private set; }

    /// <summary>
    /// Height of the structure
    /// </summary>
    public float Height { get; private set; }

    /// <summary>
    /// Number of lanes in case of a road
    /// </summary>
    public int Lanes { get; private set; }

    public OSMWay(XmlNode xmlNode)
    {
        NodeIDs = new List<ulong>();
        Height = 10.0f;
        Lanes = 1;
        Name = "";

        // Get OML data
        ID = GetAttribute<ulong>("id", xmlNode.Attributes);
        Visible = GetAttribute<bool>("visible", xmlNode.Attributes);

        // Get connected OML Nodes
        XmlNodeList nodeList = xmlNode.SelectNodes("nd");
        foreach(XmlNode node in nodeList)
        {
            ulong nodeReference = GetAttribute<ulong>("ref", node.Attributes);
            NodeIDs.Add(nodeReference);
        }

        XmlNodeList tags = xmlNode.SelectNodes("tag");
        foreach(XmlNode tag in tags)
        {
            string tagKey = GetAttribute<string>("k", tag.Attributes);
            switch (tagKey)
            {
                case "building:levels":
                    {
                        Height = 10.0f * GetAttribute<float>("v", tag.Attributes);
                    }
                    break;
                case "height":
                    {
                        Height = GetAttribute<float>("v", tag.Attributes);
                    }
                    break;
                case "building":
                    {
                        string value = GetAttribute<string>("v", tag.Attributes);
                        if (value.Equals("parking"))
                        {
                            WayType = OSMWayType.Parking;
                        }
                        else
                        {
                            WayType = OSMWayType.Building;
                        }
                    }
                    break;
                case "highway":
                    {
                        WayType = OSMWayType.Highway;
                    }
                    break;
                case "amenity":
                    {
                        string value = GetAttribute<string>("v", tag.Attributes);
                        if (value.Equals("parking"))
                        {
                            WayType = OSMWayType.Parking;
                        }
                        break;
                    }
                case "name":
                    {
                        Name = GetAttribute<string>("v", tag.Attributes);
                    }
                    break;
                case "lanes":
                    {
                        Lanes = GetAttribute<int>("v", tag.Attributes);
                    }
                    break;
            }
        }
    }
}

public enum OSMWayType
{
    Unknown,
    Highway,
    Parking,
    Building
}
