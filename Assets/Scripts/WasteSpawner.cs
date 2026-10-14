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
    [SerializeField] private float _wasteSpawnTimer = 1f;
    [SerializeField] private GameObject _spawnerContainer;
    private Coroutine _spawnRoutine;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (pizzaBox == null || _spawnerContainer == null)
        {
            Debug.Log("game objects are NULL");
            return;
        }
    }

    public void StartSpawning()
    {
        if (_spawnRoutine == null)
            _spawnRoutine = StartCoroutine(WasteSpawnControl());
    }

    public void StopSpawning()
    {
        if (_spawnRoutine != null)
        {
            StopCoroutine(_spawnRoutine);
            _spawnRoutine = null;
        }
    }

    public int ClearRemainingWaste()
    {
        int count = 0;
        foreach (Transform child in _spawnerContainer.transform)
        {
            if (child.CompareTag("Waste"))
            {
                count++;
                Destroy(child.gameObject);
            }
        }
        return count;
    }

    IEnumerator WasteSpawnControl()
    {
        while (true)
        {
            Vector3 randomPos = new Vector3(Random.Range(-15f, 30f), -10, Random.Range(-20f, 20f));
            GameObject newWaste = Instantiate(pizzaBox, _spawnerContainer.transform);
            GameManager.Instance.RegisterWasteSpawned();
            newWaste.transform.localPosition = randomPos;
            newWaste.transform.localRotation = Quaternion.identity;

            yield return new WaitForSeconds(_wasteSpawnTimer);
        }
    }
}
