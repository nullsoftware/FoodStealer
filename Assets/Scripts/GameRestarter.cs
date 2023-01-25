using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameRestarter : MonoBehaviour
{
    [SerializeField] private GameObject _gameOverDialog;
    [SerializeField] private Text _gameOverText;

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ShowGameOverDialog(string message)
    {
        _gameOverText.text = message;
        _gameOverDialog.SetActive(true);
    }
}
