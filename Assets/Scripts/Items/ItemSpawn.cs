using UnityEngine;

public class ItemSpawn : MonoBehaviour
{
    public Transform[] _ItemSpawnPoints;
    public GameObject[] _PossibleItems;

    private void Awake()
    {
        foreach(Transform t in _ItemSpawnPoints)
        {
            Instantiate(GetItem(Random.Range(0, 100)), t.position, Quaternion.identity);
        }
    }

    private GameObject GetItem(float f)
    {
        if (f < 10) return _PossibleItems[0];
        else if (f < 60) return _PossibleItems[1];
        else return _PossibleItems[2];
    }

}
