using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomBehaviour : MonoBehaviour
{
    public GameObject[] entrances;
    public GameObject roomBottomCorner;
    public GameObject roomTopCorner;
    public GameObjectList propList = new GameObjectList();
    public GameObject roomContents;

    public void UpdateRoom()
    {
        foreach(GameObjects props in propList.objectsList)
        {
            foreach(GameObject prop in props.objects)
            {
                prop.SetActive(false);
            }
            while(props.rolls > 0)
            {
                var randomProp = props.objects[Random.Range(0, props.objects.Count)];
                var propScript = randomProp.GetComponent<Prop>();
                if (Random.value < propScript.spawnChance)
                {
                    randomProp.SetActive(true);
                    break;
                }
                else
                {
                    props.rolls -= 1;
                }
            }
        }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
    }

        /*
        for(int i = 0; i < status.Length; i++)
        {
            doors[i].SetActive(status[i]);
            walls[i].SetActive(!status[i]);
        }
        */

    }
}
