using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class UserInterface : MonoBehaviour
{
    public static UserInterface Singleton { get; private set; }

    private GameObject _currentItem;
    private int _currentI;
    private int _takingTimer;

    private float _attackTimer;

    [SerializeField] private float _barnTimerValue;
    [SerializeField] private float _gardenTimerValue;

    private float _barnTimer;
    private float _gardenTimer;

    private void Start()
    {
        Singleton = this;
    }

    private void Update()
    {
        #region TakeButton
        for (int i = 0; i < AllObjects.Singleton.TakingItems.Length; i++)
        {
            if (_currentItem == null)
            {
                AllObjects.Singleton.TakeItButton.SetActive(false);
                if (Vector3.Distance(Character.Singleton.Transform.position, AllObjects.Singleton.TakingItems[i].transform.position) < 2)
                {
                    _currentI = i;
                    _currentItem = AllObjects.Singleton.TakingItems[i];
                }
            }
            else
            {
                if (_currentItem.activeSelf)
                {
                    AllObjects.Singleton.TakeItButton.SetActive(true);
                    if (Vector3.Distance(Character.Singleton.Transform.position, _currentItem.transform.position) > 2)
                    {
                        _currentItem = null;
                    }
                }
                else
                {
                    _currentItem = null;
                }
            }
        }

        if(_takingTimer > 0)
        {
            _takingTimer -= (int)Time.deltaTime;
            AllObjects.Singleton.TakingSlider.value += Time.deltaTime;
        }

        #endregion

        #region Attack

        if (_attackTimer > 0)
        {
            _attackTimer -= Time.deltaTime;
            
        }

        #endregion

        #region Barn'n'Garden Buttons

        if (Vector3.Distance(Character.Singleton.Transform.position, AllObjects.Singleton.Buildes[(int)Builds.barn].transform.position) < 5 && AllObjects.Singleton.Buildes[(int)Builds.barn].activeSelf)
        {
            AllObjects.Singleton.BarnButton.SetActive(true);
        }
        else
        {
            AllObjects.Singleton.BarnButton.SetActive(false);
        }

        if(Vector3.Distance(Character.Singleton.Transform.position, AllObjects.Singleton.Buildes[(int)Builds.garden].transform.position) < 5 && AllObjects.Singleton.Buildes[(int)Builds.garden].activeSelf)
        {
            AllObjects.Singleton.GardenButton.SetActive(true);
        }
        else
        {
            AllObjects.Singleton.GardenButton.SetActive(false);
        }

        if(_barnTimer > 0)
        {
            _barnTimer -= Time.deltaTime;
            AllObjects.Singleton.BarnTimer.fillAmount = _barnTimer / 100;
            AllObjects.Singleton.BarnTimer.GetComponentInChildren<TextMeshProUGUI>().text = $"{_barnTimer / 60} min";
        }

        for (int i = 0; i < AllObjects.Singleton.sv.MakedFoods.Length; i++)
        {
            if(AllObjects.Singleton.sv.MakedFoods[i] > 0)
            {
                if (_gardenTimer > 0)
                {
                    _gardenTimer -= Time.deltaTime;
                    AllObjects.Singleton.GardenTimer[i].fillAmount = _gardenTimer / 100;
                    AllObjects.Singleton.GardenTimer[i].GetComponentInChildren<TextMeshProUGUI>().text = $"{(int)_gardenTimer / 60} min";
                    AllObjects.Singleton.Vegatybles[i].SetActive(true);
                }
                else
                {
                    AllObjects.Singleton.sv.MakedFoods[i]--;
                    AllObjects.Singleton.sv.Foods[i]++;
                    AllObjects.Singleton.SaveUpdate();
                    AllObjects.Singleton.Vegatybles[i].SetActive(false);
                    AllObjects.Singleton.GardenTimer[i].gameObject.SetActive(false);
                }
            }
        }
        #endregion
    }

    #region Take

    public void TakeItem()
    {
        if (!AllObjects.Singleton.CharacterIsBusy)
        {
            if (_currentItem.tag == "Rock")
            {
                if(AllObjects.Singleton.sv.Tasks[(int)Tasks.pickaxe])
                {
                    _takingTimer = Random.Range(1, 2);
                }
                else
                {
                    _takingTimer = Random.Range(4, 5);
                }
            }
            else if (_currentItem.tag == "Tree")
            {
                if (AllObjects.Singleton.sv.Tasks[(int)Tasks.axe])
                {
                    _takingTimer = Random.Range(2, 3);
                }
                else
                {

                    _takingTimer = Random.Range(5, 7);
                }
            }

            StartCoroutine(ItemTaking(_takingTimer));
            StartCoroutine(RespawnItem(_currentI));
        }
    }

    IEnumerator ItemTaking(int takingTimer)
    {
        AllObjects.Singleton.CharacterIsBusy = true;
        AllObjects.Singleton.TakingSlider.value = 0;
        AllObjects.Singleton.TakingSlider.maxValue = takingTimer;
        AllObjects.Singleton.TakingSlider.gameObject.SetActive(true);

        yield return new WaitForSeconds(takingTimer);

        if (_currentItem.tag == "Rock") AllObjects.Singleton.sv.Rock++;
        else if (_currentItem.tag == "Tree") AllObjects.Singleton.sv.Tree++;
        AllObjects.Singleton.SaveUpdate();

        AllObjects.Singleton.CharacterIsBusy = false;
        AllObjects.Singleton.TakingSlider.gameObject.SetActive(false);
        AllObjects.Singleton.TakingItems[_currentI].SetActive(false);
        _currentItem = null;
    }

    IEnumerator RespawnItem(int currentI)
    {
        yield return new WaitForSeconds(Random.Range(60,120));
        AllObjects.Singleton.TakingItems[currentI].SetActive(true);
    }

    #endregion

    #region Attack
    
    public void Attack()
    {
        if (_attackTimer <= 0)
        {
            for (int i = 0; i < AllObjects.Singleton.Animals.Length; i++)
            {
                if (Vector3.Distance(Character.Singleton.Transform.position, AllObjects.Singleton.Animals[i].transform.position) < 2)
                {
                    AllObjects.Singleton.Animals[i].TakeDamage();
                }
            }

            _attackTimer = 1.5f;
        }
    }

    #endregion

    #region Teleport

    public void Teleport()
    {
        StartCoroutine(TeleportWait());
    }

    IEnumerator TeleportWait()
    {
        AllObjects.Singleton.CharacterIsBusy = true;
        Character.Singleton.Transform.position = new Vector3(AllObjects.Singleton.TeleportBuild.transform.position.x, AllObjects.Singleton.TeleportBuildY, AllObjects.Singleton.TeleportBuild.transform.position.z);
        yield return new WaitForSeconds(0.5f);
        AllObjects.Singleton.CharacterIsBusy = false;
    }

    #endregion

    #region Tasks
    
    public void DoTask(int taskNubmer)
    {
        AllObjects.Singleton.sv.Tasks[taskNubmer] = true;
        AllObjects.Singleton.SaveUpdate();
    }

    #endregion

    #region Barn'n'Garden

    public void Barn()
    {
        _barnTimer += _barnTimerValue;
    }

    public void Garden (int vegatybles)
    {
        if (AllObjects.Singleton.sv.Zerns[vegatybles] > 0)
        {
            _gardenTimer += _gardenTimerValue;
            AllObjects.Singleton.sv.Zerns[vegatybles]--;
            AllObjects.Singleton.sv.MakedFoods[vegatybles]++;
            AllObjects.Singleton.SaveUpdate();
        }
    }
    #endregion
}
