using UnityEngine;
using System.Collections;

public class WasteSpawner : MonoBehaviour
{
    [Header("Waste Objects")]
    [SerializeField] private GameObject pizzaBox;
    [SerializeField] private GameObject orangeMold;
    [SerializeField] private GameObject cannedFood;
    [SerializeField] private GameObject sodaCan;

    [Header("Spawn-related Variables")]
    private float _wasteSpawnTimer = 1f;
    [SerializeField] private GameObject _spawnerContainer;
    private bool _isSpawning = false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (pizzaBox == null || _spawnerContainer == null)
        {
            Debug.Log("game objects are NULL");
        }

        _isSpawning = true;
        StartCoroutine(WasteSpawnControl());
    }


    IEnumerator WasteSpawnControl()
    {
        while (_isSpawning)
        {
            Vector3 randomPos = new Vector3(Random.Range(-15, 30), -10, Random.Range(-20, 20));
            GameObject newWaste = Instantiate(pizzaBox, _spawnerContainer.transform);
            newWaste.transform.localPosition = randomPos;
            newWaste.transform.localRotation = Quaternion.identity;

            yield return new WaitForSeconds(_wasteSpawnTimer);
        }
    }
}
