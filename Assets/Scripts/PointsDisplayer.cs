using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PointsDisplayer : MonoBehaviour
{
    [SerializeField] private Text _template;
    [SerializeField] private float _animationVisibilityDelay = 0.5f;
    [SerializeField] private float _animationFadeOutDelay = 0.5f;
    [SerializeField] private float _animationSpeed = 200f;

    private readonly Queue<Text> _backlog = new Queue<Text>();

    public void ShowFloatingPoint(Vector2 displayPos)
    {
        Text txt;

        if (_backlog.Count > 0)
        {
            txt = _backlog.Dequeue();
        }
        else
        {
            txt = Instantiate(_template, transform);
        }

        txt.rectTransform.position = displayPos;

        StartCoroutine(AnimateText(txt));
    }

    private IEnumerator AnimateText(Text txt)
    {
        txt.color = new Color(1, 1, 1);
        txt.gameObject.SetActive(true);
        CanvasGroup canvasGroup = txt.GetComponent<CanvasGroup>();
        float endTime = Time.time + _animationVisibilityDelay;

        while (Time.time <= endTime)
        {
            Vector3 pos = txt.rectTransform.position;
            pos.y += _animationSpeed * Time.unscaledDeltaTime;
            txt.rectTransform.position = pos;

            yield return null;
        }

        endTime = Time.time + _animationFadeOutDelay;
        float startTime = Time.time;

        while (Time.time <= endTime)
        {
            Vector3 pos = txt.rectTransform.position;
            pos.y += _animationSpeed * Time.unscaledDeltaTime;
            txt.rectTransform.position = pos;
            canvasGroup.alpha = 1f - Mathf.InverseLerp(startTime, endTime, Time.time);

            yield return null;
        }

        txt.gameObject.SetActive(false);
        canvasGroup.alpha = 1;
        _backlog.Enqueue(txt);
    }
}
