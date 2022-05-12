using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class Prologue : MonoBehaviour
{
    // The best prologue script in the world.

    [SerializeField] private GameObject[] _prologueImages;
    [SerializeField] private GameObject[] _prologueTexts;

    [SerializeField] private int _nextDialogTimerValue;

    [SerializeField] private float _writingSpeed;

    private string _currentText;
    private int _currentImage;
    private float _nextDiaglogTimer;

    private void Update()
    {
        _prologueImages[_currentImage].transform.localScale = new Vector3(_prologueImages[_currentImage].transform.localScale.x + Time.deltaTime / 100, _prologueImages[_currentImage].transform.localScale.y + Time.deltaTime / 100);

        _nextDiaglogTimer += Time.deltaTime;

        if(_nextDiaglogTimer >= _nextDialogTimerValue)
        {
            NextDialog();
        }
    }

    public void NextDialog()
    {
        if (_currentImage < _prologueImages.Length - 1)
        {
            for (int i = 0; i < _prologueTexts.Length; i++)
            {
                _prologueTexts[i].SetActive(false);
            }

            _prologueImages[_currentImage].SetActive(false);

            _currentImage++;

            _prologueImages[_currentImage].SetActive(true);
            _prologueTexts[_currentImage].SetActive(true);

            _currentText = _prologueTexts[_currentImage].GetComponent<Text>().text;
            _prologueTexts[_currentImage].GetComponent<Text>().text = null;
            StartCoroutine(TextWriting());

            _nextDiaglogTimer = 0;
        }
        else
        {
            SceneManager.LoadScene("Play");
        }
    }

    IEnumerator TextWriting()
    {
        for (int i = 0; i < _currentText.Length; i++)
        {
            _prologueTexts[_currentImage].GetComponent<Text>().text += _currentText[i];
            yield return new WaitForSeconds(_writingSpeed);
        }
    }
}