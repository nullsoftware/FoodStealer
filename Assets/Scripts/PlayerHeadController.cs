using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SkinnedMeshRenderer))]
public class PlayerHeadController : MonoBehaviour
{
    [SerializeField] private Mesh _cross;
    [SerializeField] private Mesh _angry;
    [SerializeField] private Mesh _talk;

    private Mesh _default;
    private SkinnedMeshRenderer _meshRenderer;
    private PlayerEmotion _currentEmotion;

    public PlayerEmotion Emotion
    {
        get => GetEmotion();
        set => SetEmotion(value);
    }

    private void Start()
    {
        _meshRenderer = GetComponent<SkinnedMeshRenderer>();
        _default = _meshRenderer.sharedMesh;
    }

    private PlayerEmotion GetEmotion() => _currentEmotion;

    private void SetEmotion(PlayerEmotion emotion)
    {
        if (emotion.Equals(_currentEmotion))
        {
            return;
        }

        switch (emotion)
        {
            case PlayerEmotion.Cross:
                _meshRenderer.sharedMesh = _cross;
                break;
            case PlayerEmotion.Angry:
                _meshRenderer.sharedMesh = _angry;
                break;
            case PlayerEmotion.Talk:
                _meshRenderer.sharedMesh = _talk;
                break;
            default:
                _meshRenderer.sharedMesh = _default;
                break;
        }

        _currentEmotion = emotion;
    }
}
