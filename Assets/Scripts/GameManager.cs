using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    //do the states
    public States state;

    //Maze
    private MazeGenerator _mazeGenerator;

    [Header("GO")]
    public List<Seeker> seekers = new List<Seeker>();
    [SerializeField] private GameObject SeekerGO;
    [SerializeField] private GameObject EndPointGO;

    [Header("State Materials")] 
    [SerializeField] private Material disabled;
    [SerializeField] public Material stuck;
    
    //vars
    public Vector3 pointA;
    public Vector3 pointB;
    public UnityEvent FoundEndPoint;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        FoundEndPoint.AddListener(FoundEndPointMethod);
        _mazeGenerator = GetComponent<MazeGenerator>();
        StartState(States.GenerateMaze);
    }

    public void StartState(States newState)
    {
        switch (newState)
        {
            case States.GenerateMaze:
                state = States.GenerateMaze;
                GetComponent<MazeGenerator>().StartMaze();
                break;
            case States.SetTwoRandomPoints:
                state = States.SetTwoRandomPoints;
                SetPoints();
                //do something
                break;
            case States.FindPathBetweenPoints:
                state = States.FindPathBetweenPoints;
                InstantiatePathFinders();
                //do something
                break;
            case States.Finish:
                state = States.Finish;
                DisableSeekers();
                //do something
                break;
            default:
                Debug.LogWarning("Tragic accident");
                break;
        }
    }

    private void DisableSeekers()
    {
        foreach (var seeker in seekers)
        {
            seeker.enabled = false;
            seeker.GetComponent<MeshRenderer>().material = disabled;
        }
    }
    
    private void InstantiatePathFinders()
    {
        //Instantiate Seeker
        GameObject godSeeker = Instantiate(SeekerGO, pointA, Quaternion.identity);
        godSeeker.GetComponent<Seeker>().FillInfo(SeekerGO);
        seekers.Add(godSeeker.GetComponent<Seeker>());
        
        //Instantiate EndPoint
        Instantiate(EndPointGO, pointB, Quaternion.identity);

        //Activate them - chyba ma byc aktywowane na przycisk - jedno klikniecie ma byc jeden krok
    }

    private void FoundEndPointMethod()
    {
        StartState(States.Finish);
    }
    
    private void SetPoints()
    {
        Vector3[] corners = _mazeGenerator.Corners;
        float yPos = _mazeGenerator.FixedYPos;
        pointA = new Vector3((int)Random.Range(corners[0].x, corners[1].x), yPos, (int)Random.Range(corners[0].z, corners[1].z));
        pointB = new Vector3((int)Random.Range(corners[0].x, corners[1].x), yPos, (int)Random.Range(corners[0].z, corners[1].z));

        pointA.z += 0.55f; //0.55f is fixed value for maze shifted towards +z
        pointB.z += 0.55f;
        
        StartState(States.FindPathBetweenPoints);
    }
}

public enum States
{
    GenerateMaze,
    SetTwoRandomPoints,
    FindPathBetweenPoints,
    Finish
}

public enum PathFindingEnum
{
    Seeker,
    EndPoint
}