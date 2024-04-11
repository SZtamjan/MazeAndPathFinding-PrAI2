using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seeker : MonoBehaviour
{
    private SeekerStates currentState = SeekerStates.AbleToSeek;
    
    private GameObject seekerPrefab;
    private bool mainSeeker;

    public void FillInfo(GameObject prefab, bool godSeeker, GameObject prevSeeker)
    {
        seekerPrefab = prefab;
        mainSeeker = godSeeker;
        GetComponent<StorePreviousSeeker>().previousSeeker = prevSeeker;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            CheckNext();
        }
    }

    public void CheckNext()
    {
        if (currentState == SeekerStates.AbleToSeek)
        {
            MoveTo(transform.position + Vector3.forward);
            MoveTo(transform.position + Vector3.back);
            MoveTo(transform.position + Vector3.left);
            MoveTo(transform.position + Vector3.right);
        }

        if (GameManager.Instance.state != States.Finish && !mainSeeker) GetComponent<MeshRenderer>().material = GameManager.Instance.stuck;
        currentState = SeekerStates.UnableToSeek;
    }

    private void MoveTo(Vector3 movePos)
    {
        Vector3 dir = movePos - transform.position;
        Ray ray = new Ray(transform.position, dir);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 1f))
        {//hit sciana, albo obiekty
            switch (hit.collider.gameObject.tag)
            {
                case "Wall":
                    //Do nothing
                    return;
                case "EndPoint":
                    GameManager.Instance.lastSeeker = gameObject;
                    GameManager.Instance.StartState(States.Finish);
                    return;
            }
        }
        else
        {//nic nie ma na drodze
            //zespawnuj nastepnego seekera
            GameObject seeker = Instantiate(seekerPrefab, movePos, Quaternion.identity);
            seeker.GetComponent<Seeker>().FillInfo(seekerPrefab, false, gameObject);
            GameManager.Instance.seekers.Add(seeker.GetComponent<Seeker>());
        }
    }
}

public enum SeekerStates
{
    AbleToSeek,
    UnableToSeek
}