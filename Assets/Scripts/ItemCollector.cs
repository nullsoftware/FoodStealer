using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AudioSource))]
public class ItemCollector : MonoBehaviour
{
    [SerializeField] private ItemSpawner _spawner;

    private Animator _animator;
    private AudioSource _itemCollectedAudio;
    

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _itemCollectedAudio = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out FoodItem item))
        {
            _spawner.ReturnObject(item);
            OnItemCollected();
        }
    }

    public void OnItemCollected()
    {
        _animator.SetTrigger(AnimationConstants.ItemCollectedTriggerName);
        _itemCollectedAudio.Play();
    }
}
