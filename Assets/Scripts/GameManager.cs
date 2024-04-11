using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
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
    [NonSerialized] public GameObject lastSeeker;
    private GameObject mainSeeker;

    [Header("State Materials")] 
    [SerializeField] private Material pathMaterial;
    [SerializeField] private Material mainSeekerMaterial;
    [SerializeField] private Material disabled;
    [SerializeField] public Material stuck;

    [Header("ShowPath")] 
    public List<GameObject> pathSeekers;

    //vars
    public Vector3 pointA;
    public Vector3 pointB;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
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
                break;
            case States.FindPathBetweenPoints:
                state = States.FindPathBetweenPoints;
                InstantiatePathFinders();
                break;
            case States.Finish:
                state = States.Finish;
                DisableSeekers();
                ShowPath();
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

    private void ShowPath()
    {
        ChangeMaterialToShowPath(lastSeeker);
        
        while(lastSeeker.GetComponent<StorePreviousSeeker>().previousSeeker != null)
        {
            lastSeeker = lastSeeker.GetComponent<StorePreviousSeeker>().previousSeeker;
            if(GameObject.ReferenceEquals(lastSeeker,mainSeeker)) return;
            ChangeMaterialToShowPath(lastSeeker);
        }
    }

    private void ChangeMaterialToShowPath(GameObject obj)
    {
        obj.GetComponent<MeshRenderer>().material = pathMaterial;
    }
    
    private void InstantiatePathFinders()
    {
        //Instantiate Seeker
        mainSeeker = Instantiate(SeekerGO, pointA, Quaternion.identity);
        mainSeeker.GetComponent<Seeker>().FillInfo(SeekerGO,true,null);
        mainSeeker.GetComponent<MeshRenderer>().material = mainSeekerMaterial;
        //seekers.Add(seeker.GetComponent<Seeker>());
        
        //Instantiate EndPoint
        Instantiate(EndPointGO, pointB, Quaternion.identity);
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
    Finish,
}

public enum PathFindingEnum
{
    Seeker,
    EndPoint
}