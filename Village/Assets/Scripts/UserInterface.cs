using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UserInterface : MonoBehaviour
{
    public static UserInterface Singleton { get; private set; }

    private GameObject _currentItem;
    private int _currentI;
    private float _takingTimer;

    private float _attackTimer;

    private Animal _findedAnimal;

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

        if (_takingTimer > 0)
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
        else
        {
            AllObjects.Singleton.CharacterIsAttack = false;
        }

        #endregion

        #region AnimalHP

        if (_findedAnimal == null)
        {
            for (int i = 0; i < AllObjects.Singleton.Animals.Length; i++)
            {
                if (Vector3.Distance(Character.Singleton.Transform.position, AllObjects.Singleton.Animals[i].transform.position) < AllObjects.Singleton.AnimalDistance * 1.5f && AllObjects.Singleton.Animals[i].Hp > 0)
                {
                    _findedAnimal = AllObjects.Singleton.Animals[i];
                }
            }
        }
        else
        {
            if (Vector3.Distance(Character.Singleton.Transform.position, _findedAnimal.transform.position) < AllObjects.Singleton.AnimalDistance * 1.5f && _findedAnimal.Hp > 0)
            {
                AllObjects.Singleton.AnimalHPSlider.gameObject.SetActive(true);

                AllObjects.Singleton.AnimalHPSlider.maxValue = _findedAnimal.MaxHP;
                AllObjects.Singleton.AnimalHPSlider.value = _findedAnimal.Hp;

                AllObjects.Singleton.AnimalHPSlider.GetComponentInChildren<Text>().text = $"{_findedAnimal.Hp} / {_findedAnimal.MaxHP}";

            }
            else
            {
                AllObjects.Singleton.AnimalHPSlider.gameObject.SetActive(false);
                _findedAnimal = null;
            }
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

        if (Vector3.Distance(Character.Singleton.Transform.position, AllObjects.Singleton.Buildes[(int)Builds.garden].transform.position) < 5 && AllObjects.Singleton.Buildes[(int)Builds.garden].activeSelf)
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
                    AllObjects.Singleton.BarnTimer[i].GetComponentInChildren<Text>().text = $"{(int)AllObjects.Singleton.sv.BarnTimer[i] / 60} min";
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
            if (AllObjects.Singleton.sv.MakedVegetybles[i] > 0)
            {
                if (AllObjects.Singleton.sv.GardenTimer[i] > 0)
                {
                    AllObjects.Singleton.sv.GardenTimer[i] -= Time.deltaTime;

                    AllObjects.Singleton.GardenTimer[i].fillAmount = AllObjects.Singleton.sv.GardenTimer[i] / 100;
                    AllObjects.Singleton.GardenTimer[i].GetComponentInChildren<Text>().text = $"{(int)AllObjects.Singleton.sv.GardenTimer[i] / 60} min";
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

        if (Vector3.Distance(Character.Singleton.Transform.position, AllObjects.Singleton.Shoper.transform.position) < 2)
        {
            AllObjects.Singleton.ShoperButton.SetActive(true);
        }
        else
        {
            AllObjects.Singleton.ShoperButton.SetActive(false);
        }

        #endregion

        #region Wife

        if (!AllObjects.Singleton.sv.WifeIsFree)
        {
            if (Vector3.Distance(AllObjects.Singleton.WifeTower.transform.position, Character.Singleton.Transform.position) < 10 && !AllObjects.Singleton.WifeWithCharacter)
            {
                AllObjects.Singleton.WifeButton.SetActive(true);
            }
            else
            {
                AllObjects.Singleton.WifeButton.SetActive(false);
            }

            if (AllObjects.Singleton.WifeWithCharacter)
            {
                if(Vector3.Distance(Character.Singleton.Transform.position, AllObjects.Singleton.Buildes[(int)Builds.crafttable].transform.position) < 10)
                {
                    AllObjects.Singleton.sv.WifeIsFree = true;
                    AllObjects.Singleton.SaveUpdate();
                    StartCoroutine(SetActiveForTime(5, AllObjects.Singleton.WifeSecondText));
                    AllObjects.Singleton.Wife.GetComponent<ObserverNPC>().WifeIsFree();
                }
            }
        }

        #endregion

        #region Tree

        if (!AllObjects.Singleton.sv.TreeIsPlaced)
        {
            if (AllObjects.Singleton.TreeInBag)
            {
                AllObjects.Singleton.TreeButton.SetActive(true);
            }
            else
            {
                if (Vector3.Distance(Character.Singleton.Transform.position, AllObjects.Singleton.Krystal.transform.position) < 5)
                {
                    AllObjects.Singleton.TreeButton.SetActive(true);
                }
                else
                {
                    AllObjects.Singleton.TreeButton.SetActive(false);
                }
            }
        }

        #endregion

        #region CraftTable

        if(Vector3.Distance(Character.Singleton.Transform.position, AllObjects.Singleton.Buildes[(int)Builds.crafttable].transform.position) < 3 && AllObjects.Singleton.Buildes[(int)Builds.crafttable].activeSelf)
        {
            AllObjects.Singleton.InventoryButton.SetActive(true);
        }
        else
        {
            AllObjects.Singleton.InventoryButton.SetActive(false);
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
                if (AllObjects.Singleton.sv.Tasks[(int)Tasks.pickaxe])
                {
                    _takingTimer = Random.Range(AllObjects.Singleton.StoneTakeMin, AllObjects.Singleton.StoneTakeMax) / 2;
                    AllObjects.Singleton.WhichAnimation = "Tree";
                }
                else
                {
                    _takingTimer = Random.Range(AllObjects.Singleton.StoneTakeMin, AllObjects.Singleton.StoneTakeMax);
                    AllObjects.Singleton.WhichAnimation = "Rock";
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

                AllObjects.Singleton.WhichAnimation = "Tree";
            }
            else if (_currentItem.tag == "BridgePart")
            {
                _takingTimer = 3f;

                AllObjects.Singleton.WhichAnimation = "Rock";
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

        if (_currentItem.tag == "Rock")
        {
            AllObjects.Singleton.sv.Rock++;

            Tutorial.Singleton.DoStep(ref Tutorial.Singleton.Stone, (int)Steps.Stone);
        }
        else if (_currentItem.tag == "Tree")
        {
            AllObjects.Singleton.sv.Tree++;

            Tutorial.Singleton.DoStep(ref Tutorial.Singleton.Tree, (int)Steps.Tree);
        }
        else if (_currentItem.tag == "BridgePart") AllObjects.Singleton.sv.BrigdeParts++;

        for (int i = 0; i < AllObjects.Singleton.sv.BridgePartGameObjects.Length; i++)
        {
            if (AllObjects.Singleton.sv.BridgePartGameObjects[i] == null)
            {
                AllObjects.Singleton.sv.BridgePartGameObjects[i] = _currentItem.gameObject;
                break;
            }
        }
        AllObjects.Singleton.SaveUpdate();

        AllObjects.Singleton.CharacterIsBusy = false;
        AllObjects.Singleton.TakingSlider.gameObject.SetActive(false);
        AllObjects.Singleton.TakingItems[_currentI].SetActive(false);
        _currentItem = null;
    }

    IEnumerator RespawnItem(int currentI)
    {
        yield return new WaitForSeconds(Random.Range(60, 120));
        if (AllObjects.Singleton.TakingItems[currentI].tag != "BridgePart")
        {
            AllObjects.Singleton.TakingItems[currentI].SetActive(true);
        }
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

            _attackTimer = Character.Singleton.AttackSpeed;
            AllObjects.Singleton.CharacterIsAttack = true;
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
        if (AllObjects.Singleton.sv.Animals[meet] > 0)
        {
            AllObjects.Singleton.sv.BarnTimer[meet] += AllObjects.Singleton.BarnTimerValue;
            AllObjects.Singleton.sv.Animals[meet]--;
            AllObjects.Singleton.sv.MakedMeets[meet]++;
            AllObjects.Singleton.SaveUpdate();
        }
    }

    public void Garden(int vegatybles)
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

    #region Wife

    public void Wife()
    {
        if(!AllObjects.Singleton.sv.WifeIsFree)
        {
            if (!AllObjects.Singleton.WifeWithCharacter)
            {
                AllObjects.Singleton.Wife.transform.position = Character.Singleton.Transform.position;
                AllObjects.Singleton.Wife.SetActive(true);
                AllObjects.Singleton.WifeWithCharacter = true;
                StartCoroutine(SetActiveForTime(5, AllObjects.Singleton.WifeFirstText));
            }
        }
    }

    #endregion

    #region Tree

    public void PlaceTree()
    {
        if (!AllObjects.Singleton.TreeInBag)
        {
            AllObjects.Singleton.TreeInBag = true;
            StartCoroutine(SetActiveForTime(5, AllObjects.Singleton.TreeFindedText));
        }
        else
        {
            AllObjects.Singleton.sv.TreeTransform = new Vector3(Character.Singleton.Transform.position.x, Character.Singleton.Transform.position.y, Character.Singleton.Transform.position.z - 1.25f);
            AllObjects.Singleton.sv.TreeIsPlaced = true;
            DoTask((int)Tasks.main_tree);
            AllObjects.Singleton.SaveUpdate();
            AllObjects.Singleton.TreeInBag = false;
            AllObjects.Singleton.TreeButton.SetActive(false);
        }
    }

    #endregion

    #region Eat

    public void EatMeet(int meet)
    {
        if (AllObjects.Singleton.sv.Meets[meet] > 0)
        {
            if (Character.Singleton.Hp < Character.Singleton.HpMax || Character.Singleton.Hunger < 100)
            {
                AllObjects.Singleton.sv.Meets[meet]--;
                Character.Singleton.HealthChange(50);
		Character.Singleton.HungerChange(50);
                AllObjects.Singleton.SaveUpdate();
            }
            else
            {
                StartCoroutine(SetActiveForTime(3, AllObjects.Singleton.HpIsFull));
            }


            Tutorial.Singleton.DoStep(ref Tutorial.Singleton.Food, (int)Steps.Food);
        }
    }

    public void EatVegetyble(int vegatybles)
    {
        if (AllObjects.Singleton.sv.Vegetybles[vegatybles] > 0)
        {
            if (Character.Singleton.Hp < Character.Singleton.HpMax || Character.Singleton.Hunger < 100)
            {
                AllObjects.Singleton.sv.Vegetybles[vegatybles]--;
                Character.Singleton.HealthChange(25);
		Character.Singleton.HungerChange(25);
                AllObjects.Singleton.SaveUpdate();
            }
            else
            {
                StartCoroutine(SetActiveForTime(3, AllObjects.Singleton.HpIsFull));
            }
        }
    }

    #endregion

    public IEnumerator SetActiveForTime(int time, GameObject setactiveGameObject)
    {
        setactiveGameObject.SetActive(true);
        yield return new WaitForSeconds(time);
        setactiveGameObject.SetActive(false);
    }
}
