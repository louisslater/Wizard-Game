using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomBehaviour : MonoBehaviour
{
    public GameObject[] walls;
    public GameObject[] doors;
    public GameObject[] entrances;
    public Vector2 roomSize;
    public GameObjectList propList = new GameObjectList();


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
        /*
        for(int i = 0; i < status.Length; i++)
        {
            doors[i].SetActive(status[i]);
            walls[i].SetActive(!status[i]);
        }
        */
        
    }
}
