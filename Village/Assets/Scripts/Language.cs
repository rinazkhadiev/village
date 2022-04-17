using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Language : MonoBehaviour
{
    [SerializeField] private string _ru;
    [SerializeField] private string _en;

    private Text _current;
    private TextMesh _currentMesh;

    private void Start()
    {
        if(GetComponent<Text>() != null)
        {
            _current = GetComponent<Text>();

            if (PlayerPrefs.HasKey("Language"))
            {
                if (PlayerPrefs.GetString("Language") == "ru")
                {
                    _current.text = _ru;
                }
                else if (PlayerPrefs.GetString("Language") == "en")
                {
                    _current.text = _en;
                }
            }
            else
            {
                if (Application.systemLanguage == SystemLanguage.Russian)
                {
                    PlayerPrefs.SetString("Language", "ru");
                    _current.text = _ru;
                }
                else
                {
                    PlayerPrefs.SetString("Language", "en");
                    _current.text = _en;
                }
            }
        }
       
        if(GetComponent<TextMesh>() != null)
        {
            _currentMesh = GetComponent<TextMesh>();

            if (PlayerPrefs.HasKey("Language"))
            {
                if (PlayerPrefs.GetString("Language") == "ru")
                {
                    _currentMesh.text = _ru;
                }
                else if (PlayerPrefs.GetString("Language") == "en")
                {
                    _currentMesh.text = _en;
                }
            }
            else
            {
                if (Application.systemLanguage == SystemLanguage.Russian)
                {
                    PlayerPrefs.SetString("Language", "ru");
                    _currentMesh.text = _ru;
                }
                else
                {
                    PlayerPrefs.SetString("Language", "en");
                    _currentMesh.text = _en;
                }
            }
        }
    }

    public void SetLanguage(string language)
    {
        PlayerPrefs.SetString("Language", language);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
