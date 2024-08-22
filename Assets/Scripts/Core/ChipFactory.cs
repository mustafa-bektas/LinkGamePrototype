using UnityEngine;

namespace Core
{
    public class ChipFactory
    {
        public GameObject[] ChipPrefabs { get; private set; }

        public ChipFactory(GameObject[] chipPrefabs)
        {
            ChipPrefabs = chipPrefabs;
        }
        
        public GameObject CreateChip(int chipType, Vector3 position)
        {
            return Object.Instantiate(ChipPrefabs[chipType], position, Quaternion.identity);
        }
    }
}