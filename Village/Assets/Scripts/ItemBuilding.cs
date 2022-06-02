using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;

public class ItemBuilding : MonoBehaviour
{
    [Header("Кнопка в инвентаре")]
    [SerializeField] private GameObject _buttoninInventory;

    [Header("Номер постройки")] 
    [SerializeField] private int _buildNumber;

    [Header("Позиция Y игрока")] 
    [SerializeField] private float _yPosition;

    [Header("Материалы")] 
    [SerializeField] private Text _rockNeedText;
    [SerializeField] private int _rockNeed;
    [SerializeField] private Text _treeNeedText;
    [SerializeField] private int _treeNeed;

    [Header("Время постройки")]
    [SerializeField] private float _buildTime;
    [SerializeField] private Slider _timerSlider;
    [SerializeField] private Text _timerText;
    private float _timer;

    [Header("Текст ошибки")]
    [SerializeField] private GameObject _errorText;

    [Header("Телепорт нужен?")]
    [SerializeField] private bool _needTeleport;

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

    public void BuildBridge()
    {
        if (AllObjects.Singleton.sv.BrigdeParts >= 4)
        {
            StartCoroutine(BuildStart());
        }  
        else
        {
            StartCoroutine(BridgeError());
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

        if(Vector3.Distance(Character.Singleton.Transform.position, AllObjects.Singleton.Buildes[(int)Builds.bridge].transform.position) < 5 && !AllObjects.Singleton.sv.BuildsActives[(int)Builds.bridge])
        {
            AllObjects.Singleton.BridgeBuildButton.SetActive(true);
        }
        else
        {
            AllObjects.Singleton.BridgeBuildButton.SetActive(false);
        }
    }

    IEnumerator BuildStart()
    {
        _timer = _buildTime;
        _timerSlider.gameObject.SetActive(true);
        _timerSlider.maxValue = _buildTime;

        AllObjects.Singleton.sv.Rock -= _rockNeed;
        AllObjects.Singleton.sv.Tree -= _treeNeed;
        AllObjects.Singleton.SaveUpdate();

        yield return new WaitForSeconds(_buildTime);

        if (_needTeleport)
        {
            AllObjects.Singleton.TeleportBuild = AllObjects.Singleton.Buildes[_buildNumber];
            AllObjects.Singleton.TeleportBuildY = _yPosition;
            AllObjects.Singleton.TeleportPanel.SetActive(true);
            AllObjects.Singleton.sv.BuildsActives[_buildNumber] = true;
        }

        switch (_buildNumber)
        {
            case (int)Builds.crafttable:
                UserInterface.Singleton.DoTask((int)Tasks.crafttable);
                break;
            case (int)Builds.garden:
                UserInterface.Singleton.DoTask((int)Tasks.garden);
                break;
            case (int)Builds.barn:
                UserInterface.Singleton.DoTask((int)Tasks.barn);
                break;
            case (int)Builds.axe:
                UserInterface.Singleton.DoTask((int)Tasks.axe);
                break;
            case (int)Builds.pickaxe:
                UserInterface.Singleton.DoTask((int)Tasks.pickaxe);
                break;
            case (int)Builds.bridge:
                UserInterface.Singleton.DoTask((int)Tasks.bridge);
                break;
            case (int)Builds.house:
                UserInterface.Singleton.DoTask((int)Tasks.main_home);
                break;

        }

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

    IEnumerator BridgeError()
    {
        AllObjects.Singleton.BridgePartText.text = $"{AllObjects.Singleton.sv.BrigdeParts}/4";
        AllObjects.Singleton.BridgePartText.gameObject.SetActive(true);
        yield return new WaitForSeconds(5);
        AllObjects.Singleton.BridgePartText.gameObject.SetActive(false);
    }
}
