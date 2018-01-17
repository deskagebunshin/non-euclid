using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelBuilder : MonoBehaviour {
    public Building [] myBuildings;
    public Outdoor[] myOutdoor;
    public List <int> activeBuildings = new List<int>();
    public List<int> activeOutsides = new List<int>();
    public GameObject player;
    public GameObject[] detectors;
    public int currentPos = 0;
    public int currentPosRad = 0;

    public int polarSteps = 12;
    // Use this for initialization
    void Start () {
        int activeBuilding;
        DeactivateAllBuildings();
        activeBuilding = Random.Range(0, myBuildings.Length);
        currentPos = CheckPosition();
        currentPosRad = currentPos;
        MakeCakeSlice(activeBuilding, currentPos - Random.Range(0, 5), true);
        ActivateElementsInBuilding(myBuildings[activeBuilding]);
        activeBuildings.Add(activeBuilding);
    }

    void MakeCakeSlice(int index, int start, bool clockweis)
    {
        int cakeSize = Random.Range(5, 10);
        if (clockweis)
        {
            
            myBuildings[index].begin = start;
            myBuildings[index].end = myBuildings[index].begin + cakeSize;
        }
        else
        {
            myBuildings[index].end = start;
            myBuildings[index].begin = myBuildings[index].begin - cakeSize;
        }
    }

    void DeactivateAllBuildings()
    {
        foreach (Building build in myBuildings)
        {
            foreach (GameObject col in build.collumns)
            {
                col.SetActive(false);
            }

            foreach (GameObject col in build.pieces)
            {
                col.SetActive(false);
            }
        }
    }
    void ActivateElementsInBuilding(Building build)
    {
        foreach (GameObject col in build.collumns)
        {
            col.SetActive(false);
        }

        foreach (GameObject col in build.pieces)
        {
            col.SetActive(false);
        }

        for (int i = build.begin; i < build.end; i++)
        {

            if(build.begin < currentPos - (polarSteps / 2))
            {
                continue;
            }
            if(i == build.begin % polarSteps)
            {
                build.collumns[i].SetActive(true);
            }
            build.pieces[i%polarSteps].SetActive(true);
            if(i > build.begin && i % polarSteps == build.begin)
            {
                break;
            }
            if(i == build.end - 1)
            {
                build.collumns[(i+1)%polarSteps].SetActive(true);
            }
        }

    }


    int CheckPosition()
    {
        RaycastHit hit;

        if (Physics.Raycast(player.transform.position, Vector3.up, out hit))
        {
            for (int i = 0; i < detectors.Length; i++)
            {
                if (hit.transform.gameObject == detectors[i])
                {
                    return i;
                }
            }
        } 

        return -1000000;
    }
    void UpdatePos()
    {
        int pos = CheckPosition();
        if (pos != currentPosRad)
        {
            if (pos == 0)
            {
                if (currentPosRad == 1)
                {
                    currentPos--;
                }
                else
                {
                    currentPos++;
                }
            }
            else if (pos == polarSteps - 1)
            {
                if (currentPosRad == 0)
                {
                    currentPos--;
                }
                else
                {
                    currentPos++;
                }
            }
            else
            {
                if (pos > currentPosRad)
                {
                    currentPos++;
                }
                else
                {
                    currentPos--;
                }

            }
            currentPosRad = pos;
        }
    }

    void CheckVisibleActive()
    {

    }

    // Update is called once per frame
    void Update () {
        UpdatePos();
        CheckVisibleActive();
    }
}
