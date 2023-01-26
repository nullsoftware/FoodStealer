using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    private const byte MaxItemRepeats = 2;

    [SerializeField] private Transform[] _spawnPositions;
    [SerializeField] private GameObject[] _templates;
    [SerializeField] private float _startDelay = 6;
    [SerializeField] private float _minDelay = 2;
    [SerializeField] private float _maxDelay = 4;
    [SerializeField] private Animator _audioAnimator;

    private readonly Dictionary<FoodItemType, GameObject> _spawnableItems
        = new Dictionary<FoodItemType, GameObject>();

    private readonly List<FoodItem> _backlog
        = new List<FoodItem>();

    private FoodItemType _lastSpawnedType;
    private byte _lastSpawnedTypeRepeats;

    private void Start()
    {
        foreach (GameObject obj in _templates)
        {
            _spawnableItems.Add(obj.GetComponent<FoodItem>().ItemType, obj);
        }

        StartCoroutine(SpawnObjects());
    }

    public void Stop()
    {
        StopAllCoroutines();
        _audioAnimator.SetTrigger(AnimationConstants.MuteTriggerName);
    }

    public void ReturnObject(FoodItem item)
    {
        item.gameObject.SetActive(false);

        _backlog.Add(item);
    }

    private FoodItem RequestObject(FoodItemType foodType)
    {
        FoodItem item = _backlog.FirstOrDefault(t => t.ItemType == foodType);

        if (item != null)
        {
            _backlog.Remove(item);

            return item;
        }
        else
        {
            return Instantiate(_spawnableItems[foodType].gameObject)
                .GetComponent<FoodItem>();
        }
    }

    private IEnumerator SpawnObjects()
    {
        yield return new WaitForSeconds(_startDelay);

        FoodItem item;

        while (true)
        {
            FoodItemType requestedType = _spawnableItems.Keys.ToArray()[Random.Range(0, _spawnableItems.Count)];

            if (requestedType == _lastSpawnedType)
            {
                if (_lastSpawnedTypeRepeats >= MaxItemRepeats)
                    continue;
                else
                    _lastSpawnedTypeRepeats++;
            }
            else
            {
                _lastSpawnedType = requestedType;
                _lastSpawnedTypeRepeats = 1;
            }

            item = RequestObject(requestedType);

            item.Refresh();
            item.transform.position = _spawnPositions[Random.Range(0, _spawnPositions.Length)].position;
            item.gameObject.SetActive(true);

            yield return new WaitForSeconds(Random.Range(_minDelay, _maxDelay));
        }
    }
}
