using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class UserInterface : MonoBehaviour
{
    public static UserInterface Singleton { get; private set; }

    private GameObject _currentItem;
    private int _currentI;
    private float _takingTimer;

    private float _attackTimer;

    public Animal FindedAnimal { get; private set; }
    private Camera _camera;
    private Button _takingButton;
    private bool _takingBusy;

    private float _loveTimer;

    private int _loveProgress;
    private int _loveProgressMax;

    private float _tpTimer;
    private Transform _tpObject;
    private bool _tpReady;
    public bool TeleportKeep { get; set; }
    public bool TeleportAnim { get; private set; }

    private int _gardenVolume;
    private int _gardenVolumeMax;
    private int _randomSeeds;

    private void Start()
    {
        Singleton = this;
 	    _takingButton =  AllObjects.Singleton.TakeItButton.GetComponent<Button>();
        _camera = Camera.main;

        _gardenVolumeMax = AllObjects.Singleton.Vegatybles_0.Length;
    }

    private void Update()
    {
        #region TakeButton

        if (_takingTimer > 0)
        {
            _takingTimer -= Time.deltaTime;
            AllObjects.Singleton.TakingSlider.value += Time.deltaTime;
            _takingButton.interactable = false;
            _takingBusy = true;
        }
        else
        {
	        _takingButton.interactable = true;
            if (_takingBusy)
            {
                AllObjects.Singleton.CharacterIsBusy = false;
                _takingBusy = false;
            }
            AllObjects.Singleton.TakingSlider.gameObject.SetActive(false);
	 

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

        if (FindedAnimal == null)
        {
            for (int i = 0; i < AllObjects.Singleton.Animals.Length; i++)
            {
                if (Vector3.Distance(Character.Singleton.Transform.position, AllObjects.Singleton.Animals[i].transform.position) < AllObjects.Singleton.AnimalDistance * 1.5f && AllObjects.Singleton.Animals[i].Hp > 0)
                {
                    FindedAnimal = AllObjects.Singleton.Animals[i];
                }
            }

            if (_camera.fieldOfView < 60)
            {
                _camera.fieldOfView += Time.deltaTime * 5;
            }
        }
        else
        {
            if (Vector3.Distance(Character.Singleton.Transform.position, FindedAnimal.transform.position) < AllObjects.Singleton.AnimalDistance * 1.5f && FindedAnimal.Hp > 0)
            {
                AllObjects.Singleton.AnimalHPSlider.gameObject.SetActive(true);
                AllObjects.Singleton.XpSlider.gameObject.SetActive(false);

                AllObjects.Singleton.AnimalHPSlider.maxValue = FindedAnimal.MaxHP;
                AllObjects.Singleton.AnimalHPSlider.value = FindedAnimal.Hp;

                AllObjects.Singleton.AnimalHPSlider.GetComponentInChildren<Text>().text = $"{FindedAnimal.Hp} / {FindedAnimal.MaxHP}";

            }
            else
            {
                AllObjects.Singleton.AnimalHPSlider.gameObject.SetActive(false);
                AllObjects.Singleton.XpSlider.gameObject.SetActive(true);
                FindedAnimal = null;
            }

            if (_camera.fieldOfView > 45)
            {
                _camera.fieldOfView -= Time.deltaTime * 5;
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

        if (Vector3.Distance(Character.Singleton.Transform.position, AllObjects.Singleton.Buildes[(int)Builds.garden].transform.position) < 10 && AllObjects.Singleton.Buildes[(int)Builds.garden].activeSelf)
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
                    AllObjects.Singleton.BarnTimer[i].GetComponentInChildren<Text>().text = $"{(int)AllObjects.Singleton.sv.BarnTimer[i]}";
                }
                else
                {
                    AllObjects.Singleton.sv.MakedMeets[i]--;
                    AllObjects.Singleton.sv.Meets[i]++;
                    AllObjects.Singleton.SaveUpdate();
                    AllObjects.Singleton.BarnTimer[i].gameObject.SetActive(false);
                    AllObjects.Singleton.MainSound.PlayOneShot(AllObjects.Singleton.FoodIsReadySound);
                }
            }
        }

        for (int i = 0; i < AllObjects.Singleton.sv.MakedVegetybles.Length; i++)
        {

            _gardenVolume = AllObjects.Singleton.sv.MakedVegetybles.Sum();
            AllObjects.Singleton.GardenVolumeText.text = $"{_gardenVolume}/{_gardenVolumeMax}";

            if (AllObjects.Singleton.sv.MakedVegetybles[i] > 0)
            {
                if (AllObjects.Singleton.sv.GardenTimer[i] > 0)
                {
                    AllObjects.Singleton.sv.GardenTimer[i] -= Time.deltaTime;

                    for (int j = 0; j < AllObjects.Singleton.sv.MakedVegetybles[0]; j++)
                    {
                        AllObjects.Singleton.Vegatybles_0[j].SetActive(true);
                    }

                    for (int j = 0; j < AllObjects.Singleton.sv.MakedVegetybles[1]; j++)
                    {
                        AllObjects.Singleton.Vegatybles_1[j].SetActive(true);
                    }

                    AllObjects.Singleton.GardenTimer[i].fillAmount = AllObjects.Singleton.sv.GardenTimer[i] / 100;
                    AllObjects.Singleton.GardenTimer[i].GetComponentInChildren<Text>().text = $"{(int)AllObjects.Singleton.sv.GardenTimer[i]}";
                    AllObjects.Singleton.GardenTimer[i].gameObject.SetActive(true);

                }
                else
                {
                    AllObjects.Singleton.sv.MakedVegetybles[i]--;
                    AllObjects.Singleton.sv.Vegetybles[i]++;

                    if (AllObjects.Singleton.sv.MakedVegetybles[0] <= 0)
                    {
                        for (int j = 0; j < AllObjects.Singleton.Vegatybles_0.Length; j++)
                        {
                            AllObjects.Singleton.Vegatybles_0[j].SetActive(false);
                            AllObjects.Singleton.MainSound.PlayOneShot(AllObjects.Singleton.FoodIsReadySound);
                        }
                    }

                    if (AllObjects.Singleton.sv.MakedVegetybles[1] <= 0)
                    {
                        for (int j = 0; j < AllObjects.Singleton.Vegatybles_1.Length; j++)
                        {
                            AllObjects.Singleton.Vegatybles_1[j].SetActive(false);
                            AllObjects.Singleton.MainSound.PlayOneShot(AllObjects.Singleton.FoodIsReadySound);
                        }
                    }

                    AllObjects.Singleton.SaveUpdate();
                    AllObjects.Singleton.GardenTimer[i].gameObject.SetActive(false);
                    AllObjects.Singleton.InventoryAnimator.Play("Anim");
                }
            }
        }
        #endregion

        #region Shoper

        if (Vector3.Distance(Character.Singleton.Transform.position, AllObjects.Singleton.Shoper.transform.position) < 4)
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
                    StartCoroutine(SetActiveForTime(5, AllObjects.Singleton.WifeSecondText));
                    AllObjects.Singleton.Wife.GetComponent<ObserverNPC>().WifeIsFree();
                    DoTask((int)Tasks.main_wife);
                    XpPlus(5);
                    AllObjects.Singleton.SaveUpdate();
                }
            }
        }
        else
        {
            if (Vector3.Distance(AllObjects.Singleton.Wife.transform.position, Character.Singleton.Transform.position) < 4 && AllObjects.Singleton.sv.WifeLove <= 5)
            {
                AllObjects.Singleton.LoveButton.gameObject.SetActive(true);
                _loveTimer -= Time.deltaTime;
                if (_loveTimer <= 0)
                {
                    AllObjects.Singleton.LoveButton.interactable = true;
                    AllObjects.Singleton.LoveTimerText.gameObject.SetActive(false);
                }
                else
                {
                    AllObjects.Singleton.LoveButton.interactable = false;
                    AllObjects.Singleton.LoveTimerText.gameObject.SetActive(true);
                    AllObjects.Singleton.LoveTimerText.text = $"{(int)_loveTimer}";
                }
            }
            else
            {
                AllObjects.Singleton.LoveButton.gameObject.SetActive(false);
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

        #region Son

        if(Vector3.Distance(Character.Singleton.Transform.position, AllObjects.Singleton.Son.transform.position) < 3 && AllObjects.Singleton.Son.activeSelf)
        {
            AllObjects.Singleton.SonWithCharacterButton.SetActive(true);
        }
        else
        {
            AllObjects.Singleton.SonWithCharacterButton.SetActive(false);
        }

        #endregion

        #region Teleport Panel

        if (TeleportKeep)
        { 
            _tpTimer = 0;
            _tpReady = false;
            AllObjects.Singleton.TeleportImage.gameObject.SetActive(false);
            AllObjects.Singleton.TeleportText.gameObject.SetActive(false);
            AllObjects.Singleton.CharacterIsBusy = false;

            TeleportKeep = false;
            TeleportAnim = false;
        }

        if (_tpTimer > 0)
        {
            _tpTimer -= Time.deltaTime;

            AllObjects.Singleton.CharacterIsBusy = true;

            AllObjects.Singleton.TeleportImage.gameObject.SetActive(true);
            AllObjects.Singleton.TeleportText.gameObject.SetActive(true);
            AllObjects.Singleton.TeleportImage.fillAmount = _tpTimer / 5;
            AllObjects.Singleton.TeleportText.text = $"{(int)_tpTimer}";
        }
        else
        {
            if (_tpReady)
            {
                StartCoroutine(TeleportTo(_tpObject));
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
        else if (_currentItem.tag == "BridgePart")
        {
            AllObjects.Singleton.sv.BrigdeParts++;

            for (int i = 0; i < AllObjects.Singleton.sv.BridgePartGameObjects.Length; i++)
            {
                if (AllObjects.Singleton.sv.BridgePartGameObjects[i] == null)
                {
                    AllObjects.Singleton.sv.BridgePartGameObjects[i] = _currentItem.gameObject;
                    break;
                }
            }
        }

        XpPlus(1);

        AllObjects.Singleton.TakingItems[_currentI].SetActive(false);
        _currentItem = null;

        AllObjects.Singleton.SaveUpdate();
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
                if (Vector3.Distance(Character.Singleton.Transform.position, AllObjects.Singleton.Animals[i].transform.position) < 3)
                {
                    AllObjects.Singleton.Animals[i].TakeDamage();
                }
            }

            AllObjects.Singleton.MainSound.PlayOneShot(AllObjects.Singleton.AttackClip);

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
        yield return new WaitForSeconds(0.5f);
        Character.Singleton.Transform.position = new Vector3(AllObjects.Singleton.TeleportBuild.transform.position.x, AllObjects.Singleton.TeleportBuildY, AllObjects.Singleton.TeleportBuild.transform.position.z);
        yield return new WaitForSeconds(0.5f);
        AllObjects.Singleton.CharacterIsBusy = false;
    }

    #endregion

    #region Tasks

    public void DoTask(int taskNubmer)
    {
        AllObjects.Singleton.sv.Tasks[taskNubmer] = true;

        if (!AllObjects.Singleton.sv.TasksIsEnd)
        {
            while (AllObjects.Singleton.sv.Tasks[AllObjects.Singleton.sv.CurrentTask])
            {
                AllObjects.Singleton.sv.CurrentTask++;
                StartCoroutine(TaskDidVFX());

                if (AllObjects.Singleton.sv.CurrentTask > AllObjects.Singleton.sv.Tasks.Length - 1)
                {
                    AllObjects.Singleton.sv.TasksIsEnd = true;
                    break;
                }
            }
        }
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
            if (_gardenVolume < _gardenVolumeMax)
            {
                AllObjects.Singleton.sv.GardenTimer[vegatybles] += AllObjects.Singleton.GardenTimerValue;
                AllObjects.Singleton.sv.Zerns[vegatybles]--;
                AllObjects.Singleton.sv.MakedVegetybles[vegatybles]++;

                XpPlus(1);

                AllObjects.Singleton.SaveUpdate();
            }
            else
            {
                StartCoroutine(SetActiveForTime(3, AllObjects.Singleton.HaveNotGardenVolume));
            }
        }
        else
        {
            StartCoroutine(SetActiveForTime(3, AllObjects.Singleton.HaveNotSeeds));
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
            XpPlus(5);
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

                _randomSeeds = Random.Range(0, 3);
                if (_randomSeeds == 0)
                {
                    AllObjects.Singleton.sv.Zerns[vegatybles]++;
                    AllObjects.Singleton.PlusSeeds.text = "+ 1";
                }
                else if (_randomSeeds == 1)
                {
                    AllObjects.Singleton.sv.Zerns[vegatybles] += 2;
                    AllObjects.Singleton.PlusSeeds.text = "+ 2";
                }
                else
                {
                    AllObjects.Singleton.PlusSeeds.text = "+ 0";
                }

                StartCoroutine(SetActiveForTime(2, AllObjects.Singleton.PlusSeeds.gameObject));

                AllObjects.Singleton.SaveUpdate();
            }
            else
            {
                StartCoroutine(SetActiveForTime(3, AllObjects.Singleton.HpIsFull));
            }
        }
    }

    #endregion

    #region Love

    public void Love()
    {
        AllObjects.Singleton.LovePanel.gameObject.SetActive(true);
        _loveProgressMax = Random.Range(15, 30);
        AllObjects.Singleton.LoveProgressText.text = $"{_loveProgress}/{_loveProgressMax}";
        AllObjects.Singleton.MainSound.Pause();
    }

    public void LoveProgress()
    {
        _loveProgress++;

        if (_loveProgress < _loveProgressMax)
        {
            AllObjects.Singleton.LoveProgressText.text = $"{_loveProgress}/{_loveProgressMax}";
        }
        else
        {
            AllObjects.Singleton.MainSound.UnPause();
            AllObjects.Singleton.LovePanel.gameObject.SetActive(false);

            _loveProgress = 0;

            _loveTimer = AllObjects.Singleton.LoveTimerMax;
            AllObjects.Singleton.sv.WifeLove++;

            XpPlus(1);

            if (AllObjects.Singleton.sv.WifeLove == 6)
            {
                XpPlus(4);
                DoTask((int)Tasks.main_son);
                AllObjects.Singleton.SonRaiseSound.PlayOneShot(AllObjects.Singleton.SonRaiseSound.clip);
            }

            AllObjects.Singleton.SaveUpdate();
        }
    }

    #endregion

    #region Son

    public void SonWithCharater(bool with)
    {
        if (with)
        {
            AllObjects.Singleton.SonWithCharacter = true;
        }
        else
        {
            AllObjects.Singleton.SonWithCharacter = false;
        }
    }

    #endregion

    #region Teleport Panel

    public void Teleport(Transform tpObject)
    {
        if (tpObject.gameObject.activeSelf)
        {
            _tpReady = true;
            _tpTimer = 5;
            _tpObject = tpObject;
            TeleportAnim = true;
            AllObjects.Singleton.TeleportSound.PlayOneShot(AllObjects.Singleton.TeleportSound.clip);
        }
        else
        {
            StartCoroutine(SetActiveForTime(5, AllObjects.Singleton.TeleportFail));
        }
    }

    IEnumerator TeleportTo(Transform transform)
    {
        AllObjects.Singleton.TeleportImage.gameObject.SetActive(false);
        AllObjects.Singleton.TeleportText.gameObject.SetActive(false);
        _tpReady = false;

        AllObjects.Singleton.CharacterIsBusy = true;
        Character.Singleton.Transform.position = new Vector3(transform.position.x, 2, transform.position.z);
        yield return new WaitForSeconds(0.5f);
        TeleportAnim = false;
        AllObjects.Singleton.CharacterIsBusy = false;

    }

    #endregion

    #region Xp

    public void XpPlus(int xp)
    {
        AllObjects.Singleton.XpSound.PlayOneShot(AllObjects.Singleton.XpSound.clip);
        AllObjects.Singleton.XpAnim.Play();

        AllObjects.Singleton.sv.Xp += xp;
        AllObjects.Singleton.SaveUpdate();
    }

    #endregion

    public IEnumerator SetActiveForTime(int time, GameObject setactiveGameObject)
    {
        setactiveGameObject.SetActive(true);
        yield return new WaitForSeconds(time);
        setactiveGameObject.SetActive(false);
    }

    private IEnumerator TaskDidVFX()
    {
        AllObjects.Singleton.CurrentTaskText.color = Color.red;
        yield return new WaitForSeconds(0.5f);
        AllObjects.Singleton.CurrentTaskText.color = Color.white;
        yield return new WaitForSeconds(0.5f);
        AllObjects.Singleton.CurrentTaskText.color = Color.red;
        yield return new WaitForSeconds(0.5f);
        AllObjects.Singleton.CurrentTaskText.color = Color.white;
    }
}
