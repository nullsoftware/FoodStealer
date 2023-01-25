using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FoodItem : MonoBehaviour
{
    [SerializeField] private FoodItemType _itemType;

    private Rigidbody _rigidbody;

    public FoodItemType ItemType => _itemType;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();

        Refresh();
    }

    private void Update()
    {
        Vector3 velocity = _rigidbody.velocity;
        velocity.z = 1; // [TODO]: add property for speed modifier
        _rigidbody.velocity = velocity;
    }

    public void Refresh()
    {
        transform.Rotate(new Vector3(0, 1, 0) * Random.Range(0, 180));
    }

    public override string ToString()
    {
        return "Food: " + _itemType.ToString();
    }
}
