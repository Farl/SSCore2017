using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "RandomTile", menuName = "RandomTile")]
public class RandomTile : ScriptableObject
{
    [Flags]
    public enum Direction
    {
       X = 1 << 0,
       Minus_Z = 1 << 1,
       Minus_X = 1 << 2,
       Z = 1 << 3,
    }
    [Serializable]
    public class Tile
    {
        public Tile(Tile t)
        {
            gameObject = t.gameObject;
            direction = t.direction;
        }
        public GameObject gameObject;

        public Direction direction;

        [NonSerialized]
        public Tile[] adjTiles = new Tile[4];
    }

    public Tile[] tiles;
}
