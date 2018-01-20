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
    public bool initalizePos = false;
    public int polarSteps = 12;
    // Use this for initialization
    void Start () {
        int activeBuilding;
        DeactivateAll();
        activeBuilding = Random.Range(0, myBuildings.Length);
        currentPos = CheckPosition();
        currentPosRad = currentPos;
        MakeCakeSlice(activeBuilding, currentPos - Random.Range(0, 5), true, true);
        //ActivateElementsInBuilding(myBuildings[activeBuilding]);
        activeBuildings.Add(myBuildings[activeBuilding]);
        UpdateVisible();
        CheckVisibleActive();
        UpdateVisible();

    }

    void MakeCakeSlice(int index, int start, bool clockweis, bool building)
    {
        int cakeSize = Random.Range(6, 10);
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
        foreach (Outdoor build in myOutdoor)
        {
            foreach (GameObject col in build.pieces)
            {
                col.SetActive(false);
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
        if (!initalizePos)
        {
            if (player.transform.position != Vector3.zero)
            {
                Debug.Log("initializing pos");
                currentPos = pos;
                currentPosRad = pos;
                initalizePos = true;
            }

        }
        else if (pos != currentPosRad)
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

    void DeactivateActive(int index, bool building) {
        if (building)
        {
            foreach (GameObject col in activeBuildings[index].collumns)
            {
                col.SetActive(false);
            }
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
        if(activeBuildings.Count > 0)
        {
            for (int i = 0; i < activeBuildings.Count; i++)
            {
                if (activeBuildings[i].begin > currentPos + (polarSteps / 2) - 1)
                {
                    activeBuildings.Remove(activeBuildings[i]);
                }else if (activeBuildings[i].end < currentPos - (polarSteps / 2) + 1)
                {
                    activeBuildings.Remove(activeBuildings[i]);
                }
            }
        }

        if(activeOutsides.Count > 0)
        {
            for (int i = 0; i < activeOutsides.Count; i++)
            {
                if (activeOutsides[i].begin > currentPos + (polarSteps / 2) - 1)
                {
                    activeOutsides.Remove(activeOutsides[i]);
                } else if (activeOutsides[i].end < currentPos - (polarSteps / 2) + 1)
                {
                    activeOutsides.Remove(activeOutsides[i]);
                }
            }
        }

        int upperLimit = 0;
        bool loweIsBuilding = true;
        bool upperIsBuilding = true;
        int lowerLimit = 0;
        bool init = false;
        foreach(Building build in activeBuildings)
        {
            if(init == false)
            {
                init = true;
                upperLimit = build.end;
                lowerLimit = build.begin;
                upperIsBuilding = true;
                continue;
            }
            if(lowerLimit > build.begin)
            {
                lowerLimit = build.begin;
                loweIsBuilding = true;
            }
            if(upperLimit < build.end)
            {
                upperLimit = build.end;
                upperIsBuilding = true;
            }
        }
        foreach (Outdoor build in activeOutsides)
        {
            if (init == false)
            {
                init = true;
                upperLimit = build.end;
                lowerLimit = build.begin;
                upperIsBuilding = false;
                continue;
            }
            if (lowerLimit > build.begin)
            {
                lowerLimit = build.begin;
                loweIsBuilding = false;
            }
            if (upperLimit < build.end)
            {
                upperLimit = build.end;
                upperIsBuilding = false;
            }
        }
        if(lowerLimit > currentPos - (polarSteps/2) + 1)
        {
            CreateNewSlice(lowerLimit - 1, false, !loweIsBuilding);
        }
        if(upperLimit < currentPos + (polarSteps/2) - 1)
        {
            CreateNewSlice(upperLimit + 1, true, !upperIsBuilding);
        }
    }


    void CreateNewSlice(int start, bool clockwise, bool building)
    {
        if (building)
        {
            int rand = Random.Range(0, myBuildings.Length);
            if(activeBuildings.Count > 0)
            {
                if (myBuildings[rand].index == activeBuildings[0].index)
                {
                    rand = mod((rand + 1), myBuildings.Length);
                }
            }
 
            MakeCakeSlice(rand, start, clockwise, true);
            activeBuildings.Add(myBuildings[rand]);
        } else
        {
            int rand = Random.Range(0, myOutdoor.Length);
            Debug.Log(rand);
            if(activeOutsides.Count > 0)
            {
                if (myOutdoor[rand].index == activeOutsides[0].index)
                {
                    rand = mod((rand + 1), myBuildings.Length);
                }
            }

            MakeCakeSlice(rand, start, clockwise, false);
            activeOutsides.Add(myOutdoor[rand]);
        }

    }

    int mod(int x, int m)
    {
        return (x % m + m) % m;
    }

    void UpdateVisible()
    {
  
        foreach(Building build in activeBuildings)
        {
            for (int i = build.begin; i < build.end + 1; i++)
            {

                if (i < currentPos - (polarSteps / 2) + 1)
                {
                    build.collumns[mod(i, polarSteps)].SetActive(false);
                    build.pieces[mod(i, polarSteps)].SetActive(false);
                    continue;
                }
                if (i == build.begin )
                {
                    Debug.Log(mod(i, polarSteps));
                    build.collumns[mod(i, polarSteps)].SetActive(true);
                }
                if (i == build.end)
                {
                    build.collumns[mod((i + 1), polarSteps)].SetActive(true);
                }
                if (i > currentPos + (polarSteps / 2) - 1)
                {
                    build.collumns[mod(i + 1, polarSteps)].SetActive(false);
                    build.pieces[mod(i, polarSteps)].SetActive(false);
                    continue;
                }
                build.pieces[mod(i, polarSteps)].SetActive(true);
 
            }

        }
        foreach (Outdoor build in activeOutsides)
        {
            for (int i = build.begin; i < build.end + 1; i++)
            {
                if (i < currentPos - (polarSteps / 2) + 1)
                {
                    build.pieces[mod(i, polarSteps)].SetActive(false);
                    continue;
                }

                if (i > currentPos + (polarSteps / 2) - 1)
                {
                    build.pieces[mod(i, polarSteps)].SetActive(false);
                    continue;
                }
                build.pieces[mod(i, polarSteps)].SetActive(true);
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
