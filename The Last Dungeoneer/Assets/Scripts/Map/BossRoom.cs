﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BossRoom : Room {

    // Constructor
    public BossRoom(int width, int height, int x, int y, int size)
    {
        tileSize = size;
        roomWidth = width;
        roomHeight = height;
        gridPosX = x;
        gridPosY = y;

        // Generate room tiles
        CreateRoom();
    }

    // Generate room tile according to the item room layout
    public override void CreateRoom()
    {
        for (int i = 0; i < roomHeight; i++)
        {
            List<string> subList = new List<string>();
            for (int j = 0; j < roomWidth; j++)
            {
                string tile = "Null";
                placeFloor(ref tile);
                placeWall(ref tile, j);
                placeCorner(ref tile, j);
                //placeBoss(ref tile, j);

                subList.Add(tile);
            }
            tabTiles.Add(subList);
        }
    }
}
