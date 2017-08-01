using UnityEngine;
using System.Collections.Generic;

//Entry point for app
public class DinerDash : MonoBehaviour
{
    public GameObject defaultSprite;
    public GameObject unpathableSprite;
    public GameObject characterSprite;
    const int WIDTH = 10;
    const int HEIGHT = 5;
    const int SIZE = 90;
    const int TABLE_COUNT = 10;
    NodeMap map;
    NodeOccupant character;

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
            character = new NodeOccupant();
            character.graphic = (GameObject)Instantiate(characterSprite, new Vector3(0, 0, 0), Quaternion.identity);
            map.SetNodeOccupantAt(1, 1, character);
        }

        map.Build();
        map.OnNodeClicked += OnNodeClicked;
    }

    void Update()
    {
        if (map != null)
        {
            map.Update();
        }
    }

    void OnNodeClicked(int x, int y)
    {
        Node characterNode = character.owner;
        if (characterNode != null)
        {
            List<Node> path = map.DebugPath(characterNode.x, characterNode.y, x, y);
            character.path = path;
        }
    }

    void Scramble(int tableCount)
    {
        map.ClearMap();
        for (int i = 0; i < tableCount; i++)
        {
            map.SetPathingAt(Random.Range(0, WIDTH), Random.Range(0, HEIGHT), false);
        }
        map.Build();
    }

    //click handler for button
    public void OnScrambleButtonClicked()
    {
        Scramble(TABLE_COUNT);
    }
}
