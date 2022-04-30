using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class UserInterface : MonoBehaviour
{
    public static UserInterface Singleton { get; private set; }

    private GameObject _currentItem;
    private int _currentI;
    private float _takingTimer;

    private float _attackTimer;

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

        for (int i = 0; i < AllObjects.Singleton.sv.MakedMeets.Length; i++)
        {
            if (AllObjects.Singleton.sv.MakedMeets[i] > 0)
            {
                if (AllObjects.Singleton.sv.BarnTimer[i] > 0)
                {
                    AllObjects.Singleton.sv.BarnTimer[i] -= Time.deltaTime;

                    AllObjects.Singleton.BarnTimer[i].fillAmount = AllObjects.Singleton.sv.BarnTimer[i] / 100;
                    AllObjects.Singleton.BarnTimer[i].gameObject.SetActive(true);
                    AllObjects.Singleton.BarnTimer[i].GetComponentInChildren<TextMeshProUGUI>().text = $"{(int)AllObjects.Singleton.sv.BarnTimer[i] / 60} min";
                }
                else
                {
                    AllObjects.Singleton.sv.MakedMeets[i]--;
                    AllObjects.Singleton.sv.Meets[i]++;
                    AllObjects.Singleton.SaveUpdate();
                    AllObjects.Singleton.BarnTimer[i].gameObject.SetActive(false);
                }
            }
        }

        for (int i = 0; i < AllObjects.Singleton.sv.MakedVegetybles.Length; i++)
        {
            if(AllObjects.Singleton.sv.MakedVegetybles[i] > 0)
            {
                if (AllObjects.Singleton.sv.GardenTimer[i] > 0)
                {
                    AllObjects.Singleton.sv.GardenTimer[i] -= Time.deltaTime;

                    AllObjects.Singleton.GardenTimer[i].fillAmount = AllObjects.Singleton.sv.GardenTimer[i] / 100;
                    AllObjects.Singleton.GardenTimer[i].GetComponentInChildren<TextMeshProUGUI>().text = $"{(int)AllObjects.Singleton.sv.GardenTimer[i] / 60} min";
                    AllObjects.Singleton.GardenTimer[i].gameObject.SetActive(true);

                    AllObjects.Singleton.Vegatybles[i].SetActive(true);
                }
                else
                {
                    AllObjects.Singleton.sv.MakedVegetybles[i]--;
                    AllObjects.Singleton.sv.Vegetybles[i]++;
                    AllObjects.Singleton.SaveUpdate();
                    AllObjects.Singleton.Vegatybles[i].SetActive(false);
                    AllObjects.Singleton.GardenTimer[i].gameObject.SetActive(false);
                }
            }
        }
        #endregion

        #region Shoper

        if(Vector3.Distance(Character.Singleton.Transform.position, AllObjects.Singleton.Shoper.transform.position) < 2)
        {
            AllObjects.Singleton.ShoperButton.SetActive(true);
        }
        else
        {
            AllObjects.Singleton.ShoperButton.SetActive(false);
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
                    _takingTimer = Random.Range(AllObjects.Singleton.StoneTakeMin, AllObjects.Singleton.StoneTakeMax) / 2;
                }
                else
                {
                    _takingTimer = Random.Range(AllObjects.Singleton.StoneTakeMin, AllObjects.Singleton.StoneTakeMax);
                }
            }
            else if (_currentItem.tag == "Tree")
            {
                if (AllObjects.Singleton.sv.Tasks[(int)Tasks.axe])
                {
                    _takingTimer = Random.Range(AllObjects.Singleton.TreeTakeMin, AllObjects.Singleton.TreeTakeMax) / 2;
                }
                else
                {

                    _takingTimer = Random.Range(AllObjects.Singleton.TreeTakeMin, AllObjects.Singleton.TreeTakeMax);
                }
            }

            StartCoroutine(ItemTaking(_takingTimer));
            StartCoroutine(RespawnItem(_currentI));
        }
    }

    IEnumerator ItemTaking(float takingTimer)
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

            _attackTimer = AllObjects.Singleton.AttackSpeed;
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

    public void Barn(int meet)
    {
        if(AllObjects.Singleton.sv.Animals[meet] > 0)
        {
            AllObjects.Singleton.sv.BarnTimer[meet] += AllObjects.Singleton.BarnTimerValue; ;
            AllObjects.Singleton.sv.Animals[meet]--;
            AllObjects.Singleton.sv.MakedMeets[meet]++;
            AllObjects.Singleton.SaveUpdate();
        }
    }

    public void Garden (int vegatybles)
    {
        if (AllObjects.Singleton.sv.Zerns[vegatybles] > 0)
        {
            AllObjects.Singleton.sv.GardenTimer[vegatybles] += AllObjects.Singleton.GardenTimerValue;
            AllObjects.Singleton.sv.Zerns[vegatybles]--;
            AllObjects.Singleton.sv.MakedVegetybles[vegatybles]++;
            AllObjects.Singleton.SaveUpdate();
        }
    }
    #endregion
}
