using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelBuilder : MonoBehaviour {
    public Building [] myBuildings;
    public Outdoor[] myOutdoor;
    public List <Building> activeBuildings = new List<Building>();
    public List<Outdoor> activeOutsides = new List<Outdoor>();
    public GameObject player;
    public GameObject[] detectors;
    public int currentPos = 0;
    public int currentPosRad = 0;
    public bool inside = true;

    public int polarSteps = 12;
    // Use this for initialization
    void Start () {
        int activeBuilding;
        DeactivateAll();
        activeBuilding = Random.Range(0, myBuildings.Length);
        currentPos = CheckPosition();
        currentPosRad = currentPos;
        MakeCakeSlice(activeBuilding, currentPos - Random.Range(0, 5), true, true);
        ActivateElementsInBuilding(myBuildings[activeBuilding]);
        activeBuildings.Add(myBuildings[activeBuilding]);
    }

    void MakeCakeSlice(int index, int start, bool clockweis, bool building)
    {
        int cakeSize = Random.Range(4, 10);
        if (building)
        {
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
        } else
        {
            if (clockweis)
            {

                myOutdoor[index].begin = start;
                myOutdoor[index].end = myBuildings[index].begin + cakeSize;
            }
            else
            {
                myOutdoor[index].end = start;
                myOutdoor[index].begin = myBuildings[index].begin - cakeSize;
            }
        }

    }

    void DeactivateAll()
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

            if(i < currentPos - (polarSteps / 2) + 1)
            {
                continue;
            }
            if(i == build.begin % polarSteps)
            {
                build.collumns[i].SetActive(true);
            }
            build.pieces[i%polarSteps].SetActive(true);
            if(i > build.begin || i > currentPos + (polarSteps/2)-1 )
            {
                break;
            }
            if(i == build.end - 1)
            {
                build.collumns[(i+1)%polarSteps].SetActive(true);
            }
        }

    }

    void ActivateElementsInOutdoor(Outdoor build)
    {

        foreach (GameObject col in build.pieces)
        {
            col.SetActive(false);
        }

        for (int i = build.begin; i < build.end; i++)
        {

            if (build.begin < currentPos - (polarSteps / 2))
            {
                continue;
            }

            build.pieces[i % polarSteps].SetActive(true);
            if (i > build.begin && i % polarSteps == build.begin)
            {
                break;
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
    bool UpdatePos()
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
            return true;
        }
        return false;
    }

    bool CheckInside()
    {
        for(int i = 0; i < activeBuildings.Count; i++)
        {
            if(activeBuildings[i].begin < currentPos && activeBuildings[i].end > currentPos)
            {
                inside = true;
                return inside;
            }
        }
        inside = false;
        return inside;
    }

    void DeactivateActive(int index, bool building) {
        if (building)
        {
            foreach (GameObject piece in activeBuildings[index].pieces)
            {
                piece.SetActive(false);
            }
        } else
        {
            foreach (GameObject piece in activeOutsides[index].pieces)
            {
                piece.SetActive(false);
            }
        }
 
    }


    void CheckVisibleActive()
    {
        if (CheckInside())
        {
            for(int i = 0; i < activeOutsides.Count; i++)
            {
                if (activeOutsides[i].begin > currentPos && activeOutsides[i].begin > currentPos + (polarSteps / 2))
                {
                    DeactivateActive(i, false);
                    activeOutsides.Remove(activeOutsides[i]);
                }
                if (activeOutsides[i].end < currentPos && activeOutsides[i].end < currentPos - (polarSteps / 2))
                {
                    activeOutsides.Remove(activeOutsides[i]);
                    DeactivateActive(i, false);
                }
            }
            if (activeOutsides.Count < 2)
            {
                if (activeBuildings[0].end < currentPos + (polarSteps / 2) - 1)
                {
                    if (activeOutsides.Count == 0)
                    {
                        CreateNewSlice(currentPos + (polarSteps / 2) - 1, false, false);
                    }
                    else if (activeOutsides[0].begin > currentPos)
                    {
                        CreateNewSlice(currentPos + (polarSteps / 2) - 1, false, false);
                    }
                }
                if (activeOutsides[0].begin > currentPos - (polarSteps / 2) + 1)
                {
                    if (activeOutsides.Count == 0)
                    {
                        CreateNewSlice(currentPos - (polarSteps / 2) + 1, true, false);
                    }
                    else if (activeOutsides[0].end < currentPos)
                    {
                        CreateNewSlice(currentPos - (polarSteps / 2) + 1, true, false);
                    }
                }
            }
        } else
        {

            for (int i = 0; i < activeBuildings.Count; i++)
            {
                if (activeBuildings[i].begin > currentPos && activeBuildings[i].begin > currentPos + (polarSteps / 2))
                {
                    activeBuildings.Remove(activeBuildings[i]);
                    DeactivateActive(i, true);
                }
                if (activeBuildings[i].end < currentPos && activeBuildings[i].end < currentPos - (polarSteps / 2))
                {
                    activeBuildings.Remove(activeBuildings[i]);
                    DeactivateActive(i, true);
                }
            }
            if(activeBuildings.Count < 2 )
            {
                if(activeOutsides[0].end < currentPos + (polarSteps/2) - 1)
                {
                    if(activeBuildings.Count == 0)
                    {
                        CreateNewSlice(currentPos + (polarSteps / 2) - 1, false, true);
                    } else if (activeBuildings[0].begin > currentPos)
                    {
                        CreateNewSlice(currentPos + (polarSteps / 2) - 1, false, true);
                    }
                }
                if (activeOutsides[0].begin > currentPos - (polarSteps / 2) + 1)
                {
                    if (activeBuildings.Count == 0)
                    {
                        CreateNewSlice(currentPos - (polarSteps / 2) + 1, true, true);
                    }
                    else if (activeBuildings[0].end < currentPos)
                    {
                        CreateNewSlice(currentPos - (polarSteps / 2) + 1, true, true);
                    }
                }
            }
        }
    }

    
    void CreateNewSlice(int start, bool clockwise, bool building)
    {
        if (building)
        {
            int rand = Random.Range(0, myBuildings.Length);
            if (myBuildings[rand].index == activeBuildings[0].index)
            {
                rand = (rand + 1) % myBuildings.Length;
            }
            MakeCakeSlice(rand, start, clockwise, true);
            ActivateElementsInBuilding(myBuildings[rand]);
            activeBuildings.Add(myBuildings[rand]);
        } else
        {
            int rand = Random.Range(0, myBuildings.Length);
            if (myOutdoor[rand].index == activeOutsides[0].index)
            {
                rand = (rand + 1) % myBuildings.Length;
            }
            MakeCakeSlice(rand, start, clockwise, false);
            ActivateElementsInBuilding(myBuildings[rand]);
            activeOutsides.Add(activeOutsides[rand]);
        }

    }

    void UpdateVisible()
    {
  
        foreach(Building build in activeBuildings)
        {
            for (int i = build.begin; i < build.end; i++)
            {

                if (i < currentPos - (polarSteps / 2) + 1)
                {
                    build.collumns[i % polarSteps].SetActive(false);
                    build.pieces[i % polarSteps].SetActive(false);
                    continue;
                }
                if (i == build.begin % polarSteps)
                {
                    build.collumns[i%polarSteps].SetActive(true);
                }
                if (i > currentPos + (polarSteps / 2) - 1)
                {
                    build.collumns[i % polarSteps].SetActive(false);
                    build.pieces[i % polarSteps].SetActive(false);
                    continue;
                }
                build.pieces[i % polarSteps].SetActive(true);
                if (i == build.end - 1)
                {
                    build.collumns[(i + 1) % polarSteps].SetActive(true);
                }
            }

        }


    }
    // Update is called once per frame
    void Update () {
        if (UpdatePos()){
            CheckVisibleActive();
            UpdateVisible();
        }
    }
}
