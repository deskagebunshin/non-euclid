using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Building
{
    public int index;
    public GameObject[] pieces;
    public GameObject[] collumns;
    public int begin;
    public int end;
}
