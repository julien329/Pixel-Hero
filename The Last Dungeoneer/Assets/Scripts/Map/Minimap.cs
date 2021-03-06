﻿using UnityEngine;
using System.Collections;

public class Minimap : MonoBehaviour {

    public GameObject panel;
    public GameObject door;
    public GameObject itemIcon;
    public GameObject bossIcon;
    public GameObject bossKeyIcon;
    public Transform player;

    private BaseMap mapSettings;
    private string[,] minimap;
    private GameObject[,] tabPanels;
    private int playerPositionX;
    private int playerPositionY;

    private int roomGridX;
    private int roomGridY;
    private int roomHeight;
    private int roomWidth;
    private float posCorrectionX;
    private float posCorrectionY;

    void Start ()
    {
        mapSettings = GameObject.Find("Map").GetComponent<BaseMap>();
        roomWidth = mapSettings.roomWidth * mapSettings.tileSize;
        roomHeight = mapSettings.roomHeight * mapSettings.tileSize;
        roomGridX = mapSettings.roomGridX;
        roomGridY = mapSettings.roomGridY;
        posCorrectionX = (roomWidth % 2 == 0) ? 1 : 0;
        posCorrectionY = (roomHeight % 2 == 0) ? 1 : 0;

        InstanciatePanels();
        InitialiseMinimap();

        StartCoroutine("PositionUpdate");
    }
	
    // Update minimap ccroding to player position
    IEnumerator PositionUpdate()
    {
        while (true)
        {
            // Get player position in the room grid.
            playerPositionX = (int)(player.position.x / roomWidth);
            playerPositionY = (int)(player.position.y / roomHeight);

            // If player position is a not visited room, and he is not in a inexisting room, update the minimap
            if (!OutOfBounds(playerPositionY, playerPositionX) && minimap[playerPositionY, playerPositionX] != "null" && minimap[playerPositionY, playerPositionX] != "fullVisible")
            {
                revealNeighbors(playerPositionY, playerPositionX);
                revealDoors(playerPositionY, playerPositionX);
            }
            // Update minimap every .1 second
            yield return new WaitForSeconds(.1f);
        }
    }

    // Scan tabRoom array to know the map layout and store it in minimap array as notVisible
    void InitialiseMinimap()
    {
        // Declare array same size a tabRoom array.
        minimap = new string[roomGridY, roomGridX];

        // Scan map for existing rooms
        for (int i = 0; i < roomGridY; i++)
        {
            for(int j = 0; j < roomGridX; j++)
            {
                if(mapSettings.tabRooms[i,j] != null)
                    minimap[i, j] = "notVisible";
                else
                    minimap[i, j] = "null";
            }
        }
        //Reveal neightbors and door of the starting room in the middle of the map
        revealNeighbors((roomGridY / 2), (roomGridX / 2));
        revealDoors((roomGridY / 2), (roomGridX / 2));
    }

    // Reveal neighbors of a room in grey color on the minimap
    void revealNeighbors(int i, int j)
    {
        // If player is a L33T HAXOR and goes out of map, update minimap when he comes back
        if(minimap[i, j] == "notVisible")
            revealIcons(i, j);

        // Get current room and set it to full visible and white color.
        tabPanels[i, j].GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f);
        minimap[i, j] = "fullVisible";

        //Check for room above if there is a door connecting with it. If so, set to semiVisible and grey color.
        if (!OutOfBounds(i + 1, j) && (mapSettings.tabRooms[i, j].getDoor(0)) && (minimap[i + 1, j] == "notVisible"))
        {
            minimap[i + 1, j] = "semiVisible";
            tabPanels[i + 1, j].GetComponent<SpriteRenderer>().color = new Color(0.5f, 0.5f, 0.5f);
            revealIcons(i + 1, j);
        }
        //Check for room under if there is a door connecting with it. If so, set to semiVisible and grey color.
        if (!OutOfBounds(i - 1, j) && (mapSettings.tabRooms[i, j].getDoor(1)) && (minimap[i - 1, j] == "notVisible"))
        {
            minimap[i - 1, j] = "semiVisible";
            tabPanels[i - 1, j].GetComponent<SpriteRenderer>().color = new Color(0.5f, 0.5f, 0.5f);
            revealIcons(i - 1, j);
        }
        //Check for room to the right if there is a door connecting with it. If so, set to semiVisible and grey color.
        if (!OutOfBounds(i, j + 1) && (mapSettings.tabRooms[i, j].getDoor(3)) && (minimap[i, j + 1] == "notVisible"))
        {
            minimap[i, j + 1] = "semiVisible";
            tabPanels[i, j + 1].GetComponent<SpriteRenderer>().color = new Color(0.5f, 0.5f, 0.5f);
            revealIcons(i, j + 1);
        }
        //Check for room to the left if there is a door connecting with it. If so, set to semiVisible and grey color.
        if (!OutOfBounds(i, j - 1) && (mapSettings.tabRooms[i, j].getDoor(2)) && (minimap[i, j - 1] == "notVisible"))
        {
            minimap[i, j - 1] = "semiVisible";
            tabPanels[i, j - 1].GetComponent<SpriteRenderer>().color = new Color(0.5f, 0.5f, 0.5f);
            revealIcons(i, j - 1);
        }
    }

    // Reveal doors of a room on the minimap
    void revealDoors(int i, int j)
    {
        // Check for door up. If so, instanciate the door on the minimap and set it as child of this object.
        if (mapSettings.tabRooms[i, j].getDoor(0))
        {
            GameObject doorClone = (GameObject)Instantiate(door, new Vector2((j * roomWidth) + (roomWidth / 2f) - posCorrectionX, (i + 1) * roomHeight - posCorrectionY), Quaternion.identity);
            doorClone.name = "NorthDoor";
            doorClone.transform.localScale = new Vector3(4, 2);
            doorClone.transform.parent = tabPanels[i, j].transform;
        }
        // Check for door down. If so, instanciate the door on the minimap and set it as child of this object.
        if (mapSettings.tabRooms[i, j].getDoor(1))
        {
            GameObject doorClone = (GameObject)Instantiate(door, new Vector2((j * roomWidth) + (roomWidth / 2f) - posCorrectionX, (i * roomHeight) - posCorrectionY), Quaternion.identity);
            doorClone.name = "SouthDoor";
            doorClone.transform.localScale = new Vector3(4, 2);
            doorClone.transform.parent = tabPanels[i, j].transform;
        }
        // Check for door left. If so, instanciate the door on the minimap and set it as child of this object.
        if (mapSettings.tabRooms[i, j].getDoor(2))
        {
            GameObject doorClone = (GameObject)Instantiate(door, new Vector2((j * roomWidth) - posCorrectionX, (i * roomHeight) + (roomHeight / 2f) - posCorrectionY), Quaternion.identity);
            doorClone.name = "WestDoor";
            doorClone.transform.localScale = new Vector3(2, 4);
            doorClone.transform.parent = tabPanels[i, j].transform;
        }
        // Check for door right. If so, instanciate the door on the minimap and set it as child of this object.
        if (mapSettings.tabRooms[i, j].getDoor(3))
        {
            GameObject doorClone = (GameObject)Instantiate(door, new Vector2(((j + 1) * roomWidth) - posCorrectionX, (i * roomHeight) + (roomHeight / 2f) - posCorrectionY), Quaternion.identity);
            doorClone.name = "EastDoor";
            doorClone.transform.localScale = new Vector3(2, 4);
            doorClone.transform.parent = tabPanels[i, j].transform;
        }
    }

    // Reveal minimap icons on special rooms.
    void revealIcons(int i, int j)
    {
        // If the room is an ItemRoom
        if(mapSettings.tabRooms[i, j].GetType() == typeof(ItemRoom))
        {
            // Instanciate copy of original prefab
            GameObject itemIconClone = (GameObject)Instantiate(itemIcon, new Vector2((j * roomWidth) + (roomWidth / 2f) - posCorrectionX, (i * roomHeight) + (roomHeight / 2f) - posCorrectionY), Quaternion.identity);
            itemIconClone.transform.parent = transform;
        }
        // If the room is an BossRoom
        if (mapSettings.tabRooms[i, j].GetType() == typeof(BossRoom))
        {
            // Instanciate copy of original prefab
            GameObject bossIconClone = (GameObject)Instantiate(bossIcon, new Vector2((j * roomWidth) + (roomWidth / 2f) - posCorrectionX, (i * roomHeight) + (roomHeight / 2f) - posCorrectionY), Quaternion.identity);
            bossIconClone.transform.parent = transform;
        }
        // If the room is an KeyRoom
        if (mapSettings.tabRooms[i, j].GetType() == typeof(KeyRoom))
        {
            // Instanciate copy of original prefab
            GameObject bossKeyIconClone = (GameObject)Instantiate(bossKeyIcon, new Vector2((j * roomWidth) + (roomWidth / 2f) - posCorrectionX, (i * roomHeight) + (roomHeight / 2f) - posCorrectionY), Quaternion.identity);
            bossKeyIconClone.transform.parent = transform;
        }

    }

    // Instanciate minimap room panels
    void InstanciatePanels()
    {
        // Declare array of panel corresponding to the layout of the real map
        tabPanels = new GameObject[roomGridY, roomGridX];

        // Scan the real map for existing rooms
        for (int i = 0; i < roomGridY; i++)
        {
            for (int j = 0; j < roomGridX; j++)
            {
                // If a room is found, instanciate the panel at correct position, set it as children of this object and store it in the array for future use.
                if (mapSettings.tabRooms[i, j] != null)
                {
                    GameObject panelClone = (GameObject)Instantiate(panel, new Vector2((j * roomWidth) + (roomWidth / 2f) - posCorrectionX, (i * roomHeight) + (roomHeight / 2f) - posCorrectionY), Quaternion.identity);
                    panelClone.name = "MinimapPanel[" + i + "," + j + "]";
                    panelClone.transform.localScale = new Vector3(roomWidth - 1, roomHeight - 1, 1);
                    panelClone.transform.parent = transform;
                    tabPanels[i, j] = panelClone;
                }
            }
        }
    }

    // Check if the given position is out of bounds
    bool OutOfBounds(int i, int j)
    {
        // Check at every limit of the array
        if (i < 0 || i >= roomGridY || j < 0 || j >= roomGridX)
            return true;
        return false;
    }
}
