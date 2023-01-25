using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 4;
    [SerializeField] private ItemSpawner _spawner;
    [SerializeField] private GameRestarter _restarter;
    [SerializeField] private PointsDisplayer _pointsDisplayer;
    [SerializeField] private Text _goalText;
    [SerializeField] private PlayerHeadController _headController;
    [SerializeField] private Transform _hand;
    [SerializeField] private Transform _cart;
    [SerializeField] private CameraZoom _zoom;

    [SerializeField] private AudioSource _winAudio;
    [SerializeField] private AudioSource _loseAudio;

    private Animator _playerAnimator;

    private readonly Dictionary<FoodItemType, byte> _counters 
        = new Dictionary<FoodItemType, byte>();

    private FoodItemType _goalType;
    private byte _goalCount;
    private bool _isWaiting;
    private Coroutine _currentCoroutine;

    private void Start()
    {
        var foodTypes = System.Enum.GetValues(typeof(FoodItemType));

        foreach (FoodItemType type in foodTypes)
        {
            _counters.Add(type, 0);
        }

        _goalType = (FoodItemType)foodTypes.GetValue(Random.Range(1, foodTypes.Length));
        _goalCount = (byte)Random.Range(3, 6);
        _goalText.text = $"Collect {_goalCount} {_goalType}s";

        _playerAnimator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began)
        {
            TryCollectFruit(Input.touches[0].position);
        }
        else if (Input.GetMouseButtonDown(0))
        {
            TryCollectFruit(Input.mousePosition);
        }
    }

    public void ContinueAction()
    {
        _isWaiting = false;
    }

    private void TryCollectFruit(Vector3 position)
    {
        Ray ray = Camera.main.ScreenPointToRay(position);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider != null && hit.collider.TryGetComponent(out FoodItem item))
            {
                print(item + " clicked");
                
                if (_currentCoroutine == null)
                    _currentCoroutine = StartCoroutine(GoToTarget(item));
            }
        }
    }

    private IEnumerator GoToTarget(FoodItem target)
    {
        _playerAnimator.SetFloat(AnimationConstants.SpeedPropertyName, 2f);

        bool targetReached = false;
        while (target && target.isActiveAndEnabled)
        {
            float targetZ = target.transform.position.z + 1f;
            Vector3 pos = transform.position;

            if (pos.z < targetZ) 
                pos.z += _moveSpeed * Time.deltaTime;
            else
                pos.z -= _moveSpeed * Time.deltaTime;

            transform.position = pos;

            if (MathUtil.Approximately(pos.z, targetZ))
            {
                targetReached = true;
                break;
            }

            //if (Physics.Raycast(transform.position, Vector3.forward, out RaycastHit hit))
            //{
            //    break;
            //}

            yield return null;
        }

        Rigidbody rigidbody = target.GetComponent<Rigidbody>();
        _playerAnimator.SetFloat(AnimationConstants.SpeedPropertyName, 0f);

        if (!target.isActiveAndEnabled || !targetReached)
        {
            yield break;
        }

        _playerAnimator.SetTrigger(AnimationConstants.StartPickUpTriggerName);
        _isWaiting = true;
        
        while (_isWaiting)
        {
            yield return null;
        }

        rigidbody.isKinematic = true;
        target.enabled = false;
        target.transform.parent = _hand;
        target.transform.localPosition = Vector3.zero;
        //target.transform.rotation = Quaternion.identity;

        _playerAnimator.SetTrigger(AnimationConstants.ContinuePickUpTriggeName);
        _isWaiting = true;
        while (_isWaiting)
        {
            yield return null;
        }

        target.transform.parent = null;
        rigidbody.isKinematic = false;
        rigidbody.velocity = new Vector3(Random.Range(0.3f, 0.6f), 1, 1) * Random.Range(1.5f, 1.8f);
        _playerAnimator.SetTrigger(AnimationConstants.EndPickUpTriggerName);

        if (target.ItemType == _goalType)
            _pointsDisplayer.ShowFloatingPoint(Camera.main.WorldToScreenPoint(target.transform.position));
        _counters[target.ItemType]++;

        _isWaiting = true;
        while (_isWaiting)
        {
            yield return null;
        }

        rigidbody.isKinematic = true;
        target.transform.parent = _cart;

        CheckConditions();
        _currentCoroutine = null;
    }

    private void CheckConditions()
    {
        foreach (var counter in _counters)
        {
            if (counter.Value >= _goalCount)
            {
                _spawner.Stop();
                _zoom.ZoomToTarget(transform);
                enabled = false;

                if (counter.Key.Equals(_goalType))
                    StartCoroutine(DeclareWin());
                else
                    StartCoroutine(DeclareLose());
            }
        }
    }

    private IEnumerator DeclareWin()
    {
        _winAudio.Play();

        _playerAnimator.SetTrigger(AnimationConstants.DanceTriggerName);
        _isWaiting = true;
        while (_isWaiting) 
        {
            yield return null;
        }

        _restarter.ShowGameOverDialog("You won, good job!");
    }

    private IEnumerator DeclareLose()
    {
        _loseAudio.Play();

        _playerAnimator.SetTrigger(AnimationConstants.CryTriggerName);
        _isWaiting = true;
        while (_isWaiting)
        {
            yield return null;
        }

        _restarter.ShowGameOverDialog("You lose :(" + System.Environment.NewLine + "You collected wrong items.");
    }
}
