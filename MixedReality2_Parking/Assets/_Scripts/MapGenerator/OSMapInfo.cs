using System.Collections.Generic;

/// <summary>
/// Strucutre containing all the necessary OSMap Info
/// </summary>
public struct OSMapInfo
{
    public OSMBounds Bounds;
    public Dictionary<ulong, OSMNode> Nodes;
    public List<OSMWay> Ways;
}
