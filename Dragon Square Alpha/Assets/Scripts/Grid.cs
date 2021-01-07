using System.Collections;
using System; 
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;

public class Grid
{
    private const int emptyLayer = 14, rockLayer = 12, lavaLayer = 10;

    private LevelLayout level;
    private int width;
    private int height;
    private Pathfinding pathf;
    private PathNode[,] gridArray;
    public Vector2 originPosition;
    private SpriteRenderer[,] tileArray;
    public float cellsize; //1 = to match the snake 
    public List<OnMouseOver> mouseOvers; 
    public static float topLimit = 4.2f;
    private static int[] rotations = {0, 90, 180, 270};

    public Grid(int width, int height, float cellsize, Vector2 originPosition, Pathfinding pathf) 
    {
        level = GameObject.Find("Level").GetComponent<Level>().layout; 
        this.width = width;
        this.height = height;
        this.cellsize = cellsize;
        this.originPosition = originPosition;
        this.pathf = pathf;
        mouseOvers = new List<OnMouseOver>();

        tileArray = new SpriteRenderer[width, height];
        gridArray = new PathNode[width, height];

        for (int x = 0; x < GetWidth(); x++)
        {
            for (int y = 0; y < GetHeight(); y++)
            {
                GameObject tile = new GameObject();
                tile.transform.position = GetWorldPosition(x, y) + new Vector2(cellsize / 2, cellsize / 2);
                tileArray[x, y] = tile.AddComponent<SpriteRenderer>();

                if (level.rows[y].columns[x] == 0)
                {
                    tile.name = "EmptyTile";
                    gridArray[x, y] = new PathNode(x, y);
                    tileArray[x, y].gameObject.AddComponent<OnMouseOver>();
                    tileArray[x, y].sprite = GameManager.ReturnBGTile(); //sprite of each tile is random 
                    tileArray[x, y].gameObject.layer = emptyLayer;
                    tileArray[x, y].gameObject.AddComponent<BoxCollider2D>();
                    tileArray[x, y].sortingLayerName = ("Tiles");
                }
                else if (level.rows[y].columns[x] == 1)
                {
                    tile.name = "SolidTile";
                    gridArray[x, y] = new PathNode(x, y, false);
                    tileArray[x, y].gameObject.AddComponent<OnMouseOverRed>();
                    tileArray[x, y].sprite = GameManager.ReturnFGTile(); //sprite of each tile is random 
                    tileArray[x, y].gameObject.layer = rockLayer;
                    tileArray[x, y].transform.rotation = Quaternion.Euler(0, 0, rotations[UnityEngine.Random.Range(0, rotations.Length)]);
                    tileArray[x, y].tag = "Wall";
                    tileArray[x, y].gameObject.AddComponent<BoxCollider2D>();
                }
                else
                {
                    tile.name = "DangerTile";
                    gridArray[x, y] = new PathNode(x, y);
                    GameObject lavaObject = tileArray[x, y].gameObject;
                    lavaObject.AddComponent<OnMouseOverRed>();
                    tileArray[x, y].sprite = GameManager.ReturnDangerTile(); //sprite of each tile is random 
                    lavaObject.layer = rockLayer;
                    lavaObject.tag = "Wall";
                    lavaObject.transform.rotation = Quaternion.Euler(0, 0, rotations[UnityEngine.Random.Range(0, rotations.Length)]);
                    lavaObject.AddComponent<BoxCollider2D>();
                    GameObject lavaCore = new GameObject();
                    lavaCore.transform.parent = lavaObject.transform;
                    lavaCore.transform.position = new Vector3(lavaObject.transform.position.x, lavaObject.transform.position.y, 1);
                    CircleCollider2D core = lavaCore.AddComponent<CircleCollider2D>();
                    core.radius = 0.15f;
                    lavaCore.layer = lavaLayer;
                    lavaCore.tag = "Lava";

                }
            }
        }
    }

    public void SetUpNeighbors() //precalculates all neighbors 
    {
        for (int x = 0; x < GetWidth(); x++)
        {
            for (int y = 0; y < GetHeight(); y++)
            {
                gridArray[x, y].neighbourList = pathf.GetNeighbours(gridArray[x, y]);
            }
        }
    }

    public int GetWidth() => gridArray.GetLength(0);

    public int GetHeight() => gridArray.GetLength(1);

    public Vector2 GetWorldPosition(int x, int y)
    {
        return new Vector2(x, y) * cellsize + originPosition;
    }

    public void GetXY(Vector2 worldPosition, out int x, out int y)
    {
        x = Mathf.FloorToInt((worldPosition - originPosition).x / cellsize);
        y = Mathf.FloorToInt((worldPosition - originPosition).y / cellsize);
        x = Mathf.Clamp(x, 0, width - 1);
        y = Mathf.Clamp(y, 0, height - 1);
    }

    public void SetGridNode(int x, int y, PathNode node)
    {
        if (x >= 0 && y >= 0 && x < width && y < height)
        {
            gridArray[x, y] = node;
        }
    }

    public void SetGridObject(Vector2 worldPosition, PathNode node)
    {
        GetXY(worldPosition, out int x, out int y);
        SetGridNode(x, y, node);
    }

    public PathNode GetGridNode(int x, int y)
    {
        if (x >= 0 && y >= 0 && x < width && y < height)
        {
            return gridArray[x, y];
        }
        return default;
    }

    public PathNode GetGridNode(Vector3 worldPosition)
    {
        GetXY(worldPosition, out int x, out int y);
        return GetGridNode(x, y);
    }

    public bool CheckPlacement(int x, int y) //if there is a free space 
    {
        int tile = level.rows[y].columns[x];
        if (tile == 0)
            return true;
        return false;
    }

    public Vector2 MatchToTheGrid(Vector3 worldPosition)
    {
        GetXY(worldPosition, out int x, out int y);
        Vector2 newPosition = GetWorldPosition(x, y) + (Vector2.one * cellsize / 2);
        return newPosition;
    }
}


