using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell
{
    public bool occupied = false;
    public string type = "empty";

    public bool visited = false;
    public bool[] status = new bool[4];
}
