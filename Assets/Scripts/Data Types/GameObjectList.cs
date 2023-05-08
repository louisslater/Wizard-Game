using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameObjects
{
    public int rolls = 1;
    public List<GameObject> objects;
}
[System.Serializable]
public class GameObjectList
{
    public List<GameObjects> objectsList;
}

