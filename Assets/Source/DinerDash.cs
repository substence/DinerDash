using UnityEngine;
using System.Collections;

//Entry point for app
public class DinerDash : MonoBehaviour
{
    public GameObject defaultSprite;
    public GameObject unpathableSprite;
    public GameObject characterSprite;
    const int WIDTH = 10;
    const int HEIGHT = 5;
    const int SIZE = 90;
    NodeMap map;
    GameObject character;//No need to be a custom object for now

    void Start ()
    {
        //create map
        map = new NodeMap();
        map.defaultNodeGraphic = defaultSprite;
        map.defaultUnpathableGraphic = unpathableSprite;
        map.Initialize(WIDTH, HEIGHT, SIZE);

        //set unpathable nodes
        map.SetPathingAt(2, 1, false);
        map.SetPathingAt(2, 2, false);
        map.SetPathingAt(2, 3, false);
        map.SetPathingAt(3, 3, false);
        map.SetPathingAt(4, 3, false);
        map.SetPathingAt(5, 3, false);
        map.SetPathingAt(6, 3, false);
        map.SetPathingAt(7, 3, false);
        map.SetPathingAt(7, 2, false);
        map.SetPathingAt(7, 1, false);
        map.SetPathingAt(7, 0, false);

        //create and place character
        if (characterSprite)
        {
            character = (GameObject)Instantiate(characterSprite, new Vector3(0, 0, 0), Quaternion.identity);
            map.SetOccupantAt(2, 2, character);
        }

        map.Build();
        map.OnNodeClicked += OnNodeClicked;
    }

    void OnNodeClicked(int x, int y)
    {
        map.MoveOccupantToNodeAt(x, y, character);
    }
}
