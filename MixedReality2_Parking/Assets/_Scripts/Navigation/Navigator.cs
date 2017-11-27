using UnityEngine;
using System.Collections.Generic;

public struct OSMWayFindingInfo
{
    public float Distance;
    // Used to check, when we have reached the parking space
    public float NearestNodeDistance;
    public Vector3 WayCenter;
    public OSMWay Way;
}

public class Navigator {
    public static void SortParkingSpaces(Vector3 position, OSMapInfo mapInfo, out List<OSMWayFindingInfo> orderedParkingSpaces)
    {
        orderedParkingSpaces = new List<OSMWayFindingInfo>();
        foreach (OSMWay pSpace in mapInfo.ParkSpaces)
        {
            OSMWayFindingInfo info = new OSMWayFindingInfo();
            info.WayCenter = CalcWayCenter(mapInfo, pSpace);
            info.Distance = (info.WayCenter - position).magnitude;
            info.Way = pSpace;
            info.NearestNodeDistance = GetNearestNodeDistanceFromCenter(mapInfo, info);
            orderedParkingSpaces.Add(info);
        }
        orderedParkingSpaces.Sort(new OSMWayDistanceToPosDescendingSorter());
    }

    private static float GetNearestNodeDistanceFromCenter(OSMapInfo mapInfo, OSMWayFindingInfo pSpace)
    {
        float nearestNodeDistance = float.MaxValue;
        foreach (ulong nodeId in pSpace.Way.NodeIDs)
        {
            float distanceSqr= (mapInfo.Nodes[nodeId].WorldCoord - pSpace.WayCenter).sqrMagnitude;
            if(distanceSqr < nearestNodeDistance)
            {
                nearestNodeDistance = distanceSqr;
            }
        }
        return Mathf.Sqrt(nearestNodeDistance);
    }

    private static Vector3 CalcWayCenter(OSMapInfo mapInfo, OSMWay pSpace)
    {
        Vector3 wayCenter = new Vector3();

        foreach (ulong nodeId in pSpace.NodeIDs)
        {
            wayCenter += mapInfo.Nodes[nodeId].WorldCoord;
        }
        wayCenter /= pSpace.NodeIDs.Count;
        return wayCenter;
    }
    private class OSMWayDistanceToPosDescendingSorter : IComparer<OSMWayFindingInfo>
    { 
        public int Compare(OSMWayFindingInfo x, OSMWayFindingInfo y)
        {
            if (x.Distance > y.Distance)
                return 1;
            if (x.Distance < y.Distance)
                return -1;
            else
                return 0;
        }
    }

}
