using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Singleton { get; private set; }

    [SerializeField] private Image _loadBarImage;
    [SerializeField] private Text _loadText;
    [SerializeField] private GameObject _continueButton;
    [SerializeField] private Image _newGameButton;

    private void Start()
    {
        Singleton = this;

        if(_continueButton != null)
        {
            if (PlayerPrefs.HasKey("Class"))
            {
                _continueButton.SetActive(true);
                _newGameButton.color = new Color(_newGameButton.color.r, _newGameButton.color.g, _newGameButton.color.b, 0.5f);
            }
        }
    }

    public void StartScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void StartNewGame(int characterClass)
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.SetInt("Class", characterClass);
        SceneManager.LoadScene("Prologue");
    }

    public void SceneReload()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void StartPlay()
    {
        _loadBarImage.gameObject.SetActive(true);
        _loadText.gameObject.SetActive(true);
        StartCoroutine(AsyncLoad());
    }

    public void OpenSite(string url)
    {
        Application.OpenURL(url);
    }

    IEnumerator AsyncLoad()
    {
        AsyncOperation opertaion = SceneManager.LoadSceneAsync("Play");
        while (!opertaion.isDone)
        {
            float progress = opertaion.progress / 0.9f;
            _loadBarImage.fillAmount = progress;
            _loadText.text = string.Format("{0:0}%", progress * 100);
            yield return null;
        }
    }
}
