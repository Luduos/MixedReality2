using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour {

    [SerializeField]
    private float PlayerTravelSpeed = 10.0f;

    [SerializeField]
    private float RotationSpeed = 20.0f;

    [SerializeField]
    private Vector3 PlayerOffset = new Vector3(0.0f, 2.0f, 0.0f);

    [SerializeField]
    private int RandomSeed;

    [SerializeField]
    private OSMapReader MapReader;

    [SerializeField]
    private DisplayController Display;

    [SerializeField]
    private GameObject PlayerCarMesh;
    [SerializeField]
    private Camera PlayerCamera;

    private bool IsTraveling = false;
    private static readonly float EPSILON = 0.1f;

    private List<OSMWayFindingInfo> sortedParkingSpaces = new List<OSMWayFindingInfo>();
    private OSMWayFindingInfo selectedParkingSpace;
    
    /// <summary>
    /// Node the player is currently located at or traveling from
    /// </summary>
    private HighwayNode CurrentNode;
    /// <summary>
    /// Node that the player is currently looking at or traveling to
    /// </summary>
    private HighwayNode NextNode;

    /// <summary>
    /// The Node that the player is trying to reach. Can be any node on the whole graph
    /// </summary>
    private HighwayNode TargetNode;

    private IEnumerator Start()
    {
        while (!MapReader.IsReady)
        {
            yield return null;
        }

        // Choose random first node
        List<HighwayNode> nodes = Enumerable.ToList(MapReader.NavInfo.Nodes.Values);
        int size = MapReader.NavInfo.Nodes.Count;
        Random.InitState(RandomSeed);
        CurrentNode = nodes[Random.Range(0, size)];

        // choose random next node
        int neighbourCount = CurrentNode.Neighbours.Count;
        NextNode = CurrentNode.Neighbours[Random.Range(0, neighbourCount)];
        TurnForward();

        this.transform.position = CurrentNode.Position + PlayerOffset;
    }

    private void Update()
    {
        selectedParkingSpace.Distance = (this.transform.position - selectedParkingSpace.WayCenter).magnitude;
        Display.UpdateCurrentTargetDisplay(selectedParkingSpace);
        Display.UpdateCurrentLocationDisplay(CurrentNode.Name);
        if (IsTraveling)
            return;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Continue();
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            TurnForward();
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            TurnBackward();
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            TurnLeft();
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            TurnRight();
        }
    }

    /// <summary>
    /// Travels toward next Node depending on the PlayerTravelSpeed
    /// </summary>
    /// <returns></returns>
    private IEnumerator TravelToNextCrossRoads()
    {
        IsTraveling = true;
        Vector3 vecToNextNode = NextNode.Position + PlayerOffset - this.transform.position;
        while(vecToNextNode.sqrMagnitude > EPSILON)
        {
            // while we have not reached the next node, continue traveling to the next node
            this.transform.position += vecToNextNode.normalized * Time.deltaTime * PlayerTravelSpeed;
            vecToNextNode = NextNode.Position + PlayerOffset - this.transform.position;
            yield return null;
        }
        // if we have reached next node, but it isn't a crossroads or a dead end --> continue
        if(2 == NextNode.Neighbours.Count)
        {
            // set the next node to NOT be the current node
            HighwayNode newNextNode = CurrentNode == NextNode.Neighbours[0] ? NextNode.Neighbours[1] : NextNode.Neighbours[0];
            CurrentNode = NextNode;
            NextNode = newNextNode;
            // Continue with the traveling
            StartCoroutine(TurnToNextNode());
            StartCoroutine(TravelToNextCrossRoads());
        }
        else
        {
            // if we reach deadend or crossroads, turn to the next node that's nearest to the forward direction
            CurrentNode = NextNode;
            IsTraveling = false;
        }
        yield return null;
    }

    /// <summary>
    /// Turns toward the next Node depending on RotationSpeed
    /// </summary>
    /// <returns></returns>
    private IEnumerator TurnToNextNode()
    {

        Transform rotationTransform = PlayerCarMesh.transform;
        Vector3 vecToNextNode = (NextNode.Position + PlayerOffset - this.transform.position).normalized;
        // dot product between the player forward vector and the vector pointing to our next node
        float dotForwardToTravelDir = Vector3.Dot(vecToNextNode, rotationTransform.forward);
        // while we are not aligned with the target
        // Rotate around the vector that's pointing up / down, depending on whether we have to rotate left or right
        Quaternion targetRotation = Quaternion.LookRotation(vecToNextNode, new Vector3(0.0f, 1.0f, 0.0f));
        while (dotForwardToTravelDir < 0.99f)
        {
            // linear interpolation to the next node position, depending on delta time and rotation speed
            rotationTransform.rotation = Quaternion.Lerp(rotationTransform.rotation, targetRotation, Time.deltaTime * RotationSpeed);

            // update values to make it possible to switch directions while turning
            // update the vector to the next node
            vecToNextNode = (NextNode.Position + PlayerOffset - this.transform.position).normalized;
            // update targetRotation
            targetRotation = Quaternion.LookRotation(vecToNextNode, new Vector3(0.0f, 1.0f, 0.0f));
            // update dot product
            dotForwardToTravelDir = Vector3.Dot(vecToNextNode, rotationTransform.forward);
            yield return null;
        }
        rotationTransform.rotation = targetRotation;
        yield return null;
    }

    

    // left / right
    // For left and right finding we are searching for the lowest dot product on the left / right axis
    private void FindNewNextNodeHorizontal(Vector3 axisToSearchOn)
    {
        if(CurrentNode.Neighbours.Count < 1)
        {
            return;
        }
        HighwayNode nearestNeighbour = CurrentNode.Neighbours[0];
        float currentBestDotProduct = float.MaxValue;
        foreach (HighwayNode neighbour in CurrentNode.Neighbours)
        {
            Vector3 toNeighBour = (neighbour.Position - CurrentNode.Position).normalized;
            float dotOnSearchAxis = Vector3.Dot(axisToSearchOn, toNeighBour);
            if(dotOnSearchAxis < currentBestDotProduct)
            {
                nearestNeighbour = neighbour;
                currentBestDotProduct = dotOnSearchAxis;
            }
        }
        NextNode = nearestNeighbour;
    }

  
    /// <summary>
    /// For up and down neighbour finding we are searching for the highest dot product on the up / down axis
    /// </summary>
    /// <param name="axisToSearchOn"></param>
    private void FindNewNextNodeVertical(Vector3 axisToSearchOn)
    {
        if (CurrentNode.Neighbours.Count < 1)
        {
            return;
        }
        HighwayNode nearestNeighbour = CurrentNode.Neighbours[0];
        float currentBestDotProduct = 0.0f;
        foreach (HighwayNode neighbour in CurrentNode.Neighbours)
        {
            Vector3 toNeighBour = (neighbour.Position - CurrentNode.Position).normalized;
            float dotOnSearchAxis = Vector3.Dot(axisToSearchOn, toNeighBour);
            if (dotOnSearchAxis > 0.0f && dotOnSearchAxis > currentBestDotProduct)
            {
                nearestNeighbour = neighbour;
                currentBestDotProduct = dotOnSearchAxis;
            }
        }
        NextNode = nearestNeighbour;
    }



    /*******************     interface functions for speech input        ******************************/

    public void SearchParkingSpaces()
    {
        Display.HideCompass();
        sortedParkingSpaces = null;
        Navigator.SortParkingSpaces(this.transform.position, MapReader.MapInfo, out sortedParkingSpaces);

        Display.ShowSelectableParkingSpaces(sortedParkingSpaces);
    }

    public void Select(int selectedNumber)
    {
        if(null != sortedParkingSpaces)
        {
            if(selectedNumber <= Display.GetValidParkingSpaceOptionCount() && selectedNumber <= sortedParkingSpaces.Count)
            {
                selectedParkingSpace = sortedParkingSpaces[selectedNumber - 1];
                Display.ShowCompassTo(selectedParkingSpace.WayCenter);
                Display.HideSelectableParkingSpaces();
            }
            else
            {
                Display.ShowInvalidOptionSelectionNotify();
            }
        }
    }

    public void HideParkingSpaceDisplay()
    {
        sortedParkingSpaces = null;
        Display.HideSelectableParkingSpaces();
        Display.HideCompass();
    }

    public void Continue()
    {
        if (IsTraveling)
        {
            Display.ShowStillTravelingNotification();
            return;
        }
        if (CurrentNode == NextNode)
        {
            Display.OnNoNextTargetSelected();
        }
        else
        {
            StartCoroutine(TravelToNextCrossRoads());
        }
    }

    public void TurnLeft()
    {
        if (IsTraveling)
        {
            Display.ShowStillTravelingNotification();
            return;
        }
        FindNewNextNodeHorizontal(PlayerCarMesh.transform.right);
        StartCoroutine(TurnToNextNode());
    }

    public void GoLeft()
    {
        if (IsTraveling)
        {
            Display.ShowStillTravelingNotification();
            return;
        }
        TurnLeft();
        Continue();
    }

    public void TurnRight()
    {
        if (IsTraveling)
        {
            Display.ShowStillTravelingNotification();
            return;
        }
        FindNewNextNodeHorizontal(-PlayerCarMesh.transform.right);
        StartCoroutine(TurnToNextNode());
    }

    public void GoRight()
    {
        if (IsTraveling)
        {
            Display.ShowStillTravelingNotification();
            return;
        }
        TurnRight();
        Continue();
    }

    public void TurnForward()
    {
        if (IsTraveling)
        {
            Display.ShowStillTravelingNotification();
            return;
        }
        FindNewNextNodeVertical(PlayerCarMesh.transform.forward);
        StartCoroutine(TurnToNextNode());
    }

    public void GoForward()
    {
        if (IsTraveling)
        {
            Display.ShowStillTravelingNotification();
            return;
        }
        TurnForward();
        Continue();
    }

    public void TurnBackward()
    {
        if (IsTraveling)
        {
            Display.ShowStillTravelingNotification();
            return;
        }
        FindNewNextNodeVertical(-PlayerCarMesh.transform.forward);
        StartCoroutine(TurnToNextNode());
    }

    public void GoBackward()
    {
        if (IsTraveling)
        {
            Display.ShowStillTravelingNotification();
            return;
        }
        TurnBackward();
        Continue();
    }
}
