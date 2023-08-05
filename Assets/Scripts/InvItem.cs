using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
[CreateAssetMenu(menuName = "Scriptable object/Item")]

public class InvItem : ScriptableObject
{
    public Sprite image;
    public ItemType type;
    public ActionType actiontype;
    public Vector2Int range = new Vector2Int(5, 4);
    public bool stackable = true;

}

public enum ItemType
{
    NormalItem,
    Equipment
}

public enum ActionType
{
    InvItemEffects,
    EquipmentUse
}
