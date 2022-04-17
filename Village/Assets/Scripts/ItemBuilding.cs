using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;

public class ItemBuilding : MonoBehaviour
{
    [Header("������ � ���������")]
    [SerializeField] private GameObject _buttoninInventory;

    [Header("����� ���������")] 
    [SerializeField] private int _buildNumber;

    [Header("������� Y ������")] 
    [SerializeField] private float _yPosition;

    [Header("���������")] 
    [SerializeField] private TextMeshProUGUI _rockNeedText;
    [SerializeField] private int _rockNeed;
    [SerializeField] private TextMeshProUGUI _treeNeedText;
    [SerializeField] private int _treeNeed;

    [Header("����� ���������")]
    [SerializeField] private float _buildTime;
    [SerializeField] private Slider _timerSlider;
    [SerializeField] private TextMeshProUGUI _timerText;
    private float _timer;

    [Header("����� ������")]
    [SerializeField] private GameObject _errorText;

    private void Start()
    {
        if (PlayerPrefs.HasKey("Save"))
        {
            if (AllObjects.Singleton.sv.BuildsActives[_buildNumber])
            {
                Destroy(_buttoninInventory);
            }
        }

        _rockNeedText.text = _rockNeed.ToString();
        _treeNeedText.text = _treeNeed.ToString();
    }

    public void Build()
    {
        if (_rockNeed <= AllObjects.Singleton.sv.Rock && _treeNeed <= AllObjects.Singleton.sv.Tree)
        {
            StartCoroutine(BuildStart());
        }
        else
        {
            StartCoroutine(BuildError());
        }
    }

    private void Update()
    {
        if(_timer > 0)
        {
            _timer -= Time.deltaTime;
            _timerSlider.value += Time.deltaTime;
            _timerText.text = $"{(int)(_timer / 60)} min";
        }
    }

    IEnumerator BuildStart()
    {
        _timer = _buildTime;

        _timerSlider.gameObject.SetActive(true);
        _timerSlider.maxValue = _buildTime;

        yield return new WaitForSeconds(_buildTime);

        AllObjects.Singleton.TeleportBuild = AllObjects.Singleton.Buildes[_buildNumber];
        AllObjects.Singleton.TeleportBuildY = _yPosition;
        AllObjects.Singleton.TeleportPanel.SetActive(true);

        AllObjects.Singleton.sv.Rock -= _rockNeed;
        AllObjects.Singleton.sv.Tree -= _treeNeed;

        AllObjects.Singleton.sv.BuildsActives[_buildNumber] = true;
        PlayerPrefs.SetString("Save", JsonUtility.ToJson(AllObjects.Singleton.sv));
        AllObjects.Singleton.SaveUpdate();

        Destroy(_buttoninInventory);
    }

    IEnumerator BuildError()
    {
        _errorText.SetActive(true);

        _rockNeedText.color = Color.red;
        _treeNeedText.color = Color.red;

        yield return new WaitForSeconds(0.5f);

        _rockNeedText.color = Color.white;
        _treeNeedText.color = Color.white;

        yield return new WaitForSeconds(0.5f);

        _rockNeedText.color = Color.red;
        _treeNeedText.color = Color.red;

        yield return new WaitForSeconds(0.5f);

        _rockNeedText.color = Color.white;
        _treeNeedText.color = Color.white;
        _errorText.SetActive(false);
    }
}
