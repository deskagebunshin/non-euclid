using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class swapper : MonoBehaviour {
    public GameObject [] detectors = new GameObject [8];
    public GameObject[] pieces1 = new GameObject[8];
    public GameObject[] pieces2 = new GameObject[8];
    public GameObject[] pieces3 = new GameObject[8];
    public GameObject player;
    private int currentPos;
         
    // Use this for initialization
	void Start () {
        for (int i = 0; i < 8; i++)
        {
            int index = i % 3;
            switch (index)
            {
                case 0:
                    pieces1[i].SetActive(true);
                    pieces2[i].SetActive(false);
                    pieces3[i].SetActive(false);
                    break;
                case 1:
                    pieces1[i].SetActive(false);
                    pieces2[i].SetActive(true);
                    pieces3[i].SetActive(false);
                    break;
                case 2:
                    pieces1[i].SetActive(false);
                    pieces2[i].SetActive(false);
                    pieces3[i].SetActive(true);
                    break;
            }
        }
	}
	
	// Update is called once per frame
	void Update () {
        CheckPosition();
		
	}

    void CheckPosition() {
        RaycastHit hit;

        if (Physics.Raycast(player.transform.position, -Vector3.up, out hit))
        {
            for (int i = 0; i < detectors.Length; i++) {
                if(hit.transform.gameObject == detectors[i])
                {
                    if(currentPos != i)
                    {
                        CheckSwapPos(i);
                    }
                }
            }
        }
            
    }
    void SwapPiece(int i, int index)
    {
        Debug.Log("PIECE SWAPPED " + i + " " + index);
        i = Random.Range(0, 4);
        switch (i)
        {
            case 0:
                pieces1[index].SetActive(true);
                pieces2[index].SetActive(false);
                pieces3[index].SetActive(false);
                break;
            case 1:
                pieces1[index].SetActive(false);
                pieces2[index].SetActive(true);
                pieces3[index].SetActive(false);
                break;
            case 2:
                pieces1[index].SetActive(false);
                pieces2[index].SetActive(false);
                pieces3[index].SetActive(true);
                break;
        }
    }

    void CheckSwapPos(int index)
    {
        if(currentPos == 7)
        {
            if(index == 0)
            {
                SwapPiece(0, 4);
            } else
            {
                SwapPiece(0, 2);
            } 
        } else if (currentPos == 0)
        {
            if (index == 7)
            {
                SwapPiece(0, 3);
            }
            else
            {
                SwapPiece(0, 5);
            }
        } else
        {
            int x;
            if (index > currentPos)
            {
                x = index + 4;
                x = x % 8;
            }
            else
            {

                x = index - 4;
                if (x < 0)
                {
                    x = 8 + x;
                } else
                {
                    x = x % 8;
                }
            }
            SwapPiece(0, x);
        }
        
        currentPos = index;
    }

}
