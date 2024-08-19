using UnityEngine;

public class ChipFactory
{
    public readonly GameObject[] ChipPrefabs;
    
    public ChipFactory(GameObject[] chipPrefabs)
    {
        ChipPrefabs = chipPrefabs;
    }
    
    public GameObject CreateChip(int chipType, Vector3 position)
    {
        return Object.Instantiate(ChipPrefabs[chipType], position, Quaternion.identity);
    }
}