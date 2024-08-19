using UnityEngine;

public class ChipGrid
{
    public int Width { get; private set; }
    public int Height { get; private set; }
    public GameObject[,] GridArray { get; private set; }
    public readonly ChipFactory ChipFactory;

    public ChipGrid(int width, int height, GameObject[] chipPrefabs)
    {
        ChipFactory = new ChipFactory(chipPrefabs);
        Width = width;
        Height = height;
        GridArray = new GameObject[width, height];
        InitializeGridObjects();
    }
    
    private void InitializeGridObjects()
    {
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                int randomChipType = Random.Range(0, ChipFactory.ChipPrefabs.Length);
                GridArray[x, y] = ChipFactory.CreateChip(randomChipType, new Vector3(x, y, 0));
            }
        }
    }
}