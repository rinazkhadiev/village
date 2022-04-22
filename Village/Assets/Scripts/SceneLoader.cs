using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Singleton { get; private set; }

    [SerializeField] private Image _loadBarImage;
    [SerializeField] private Text _loadText;

    private void Start()
    {
        Singleton = this;
    }

    public void SceneStart(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void SceneReload()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void SceneStartToLoading(int partNumber)
    {
        PlayerPrefs.SetInt("Part", partNumber);
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
