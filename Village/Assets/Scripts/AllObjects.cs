using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.Rendering;

public class AllObjects : MonoBehaviour
{
    [Header("Другие скрипты")]
    public Save sv;
    public JoystickController JoyController;
    public DayTimeManager DayTimeMan;

    [Header("Пост-процессинг")]
    public Volume GlobalVolume;

    [Header("Персонаж")]
    [NonSerialized] public bool CharacterIsBusy;
    [NonSerialized] public bool CharacterIsAttack;

    [NonSerialized] public string WhichAnimation;

    public GameObject Axe;
    public GameObject PickAxe;

    [Header("Окружающий мир")]
    public GameObject[] TakingItems;
    public Animal[] Animals;
    public GameObject Shoper;
    public Transform BearTutorialTransform;

    public GameObject ArrowsToRocks;
    public GameObject ArrowsToTrees;
    public Animator InventoryAnimator;
    public GameObject InventoryArrow;
    public GameObject FoodArrow;
    public GameObject CraftTableArrow;

    [Header("Инвентарь")]
    public Text RockText;
    public Text TreeText;




    public Text[] ZernsTexts;

    public Text[] VegatyblesTexts;
    public Text[] VegatyblesTextsInInventory;

    public Text GardenVolumeText;




    public Text[] AnimalsTexts;
    public Text[] MeetsTexts;

    [Header("Постройки")]
    public GameObject[] Buildes;

    [NonSerialized] public GameObject TeleportBuild;
    [NonSerialized] public float TeleportBuildY;

    public GameObject[] Vegatybles_0;
    public GameObject[] Vegatybles_1;

    [Header("UI")]
    public GameObject TakeItButton;
    public Slider TakingSlider;

    public Image HpImage;
    public Text HpText;
    public GameObject HpIsFull;

    public Image HungerImage;
    public Text HungerText;
    public Image FatigueImage;
    public Text FatigueText;

    public GameObject TeleportPanel;

    public GameObject BarnButton;
    public GameObject GardenButton;
    public Image[] BarnTimer;
    public Image[] GardenTimer;

    public GameObject ShoperButton;
    public Text MoneyText;
    public GameObject HaveNotMoneyText;
    public GameObject HaveNotResoursesText;

    public Text TimeText;
    public Text DayText;
    public GameObject SleepButton;

    public Slider AnimalHPSlider;

    public GameObject PoorPanel;
    public GameObject InventoryButton;

    public GameObject JoyStickBG;

    public Image TeleportImage;
    public Text TeleportText;
    public GameObject TeleportFail;

    public Slider XpSlider;
    public Animation XpAnim;
    public Text XpLvlText;
    public Text XpText;

    public GameObject HaveNotSeeds;
    public GameObject HaveNotGardenVolume;
    public Text PlusSeeds;

    [Header("Bridge")]

    public Text BridgePartText;
    public GameObject BridgeBuildButton;
    public GameObject BridgeMessage;
    public GameObject NeedABridgeText;
    public GameObject NeedABridgeCollider;

    [Header("Wife")]

    public GameObject WifeButton;
    public GameObject WifeFirstText;
    public GameObject WifeSecondText;
    public Transform WifeHomePosition;
    public GameObject WifeTower;
    public GameObject Wife;
    public bool WifeWithCharacter;

    public GameObject LovePanel;
    public Slider LoveSlider;
    public Text LoveSliderText;

    public Button LoveButton;
    public Text LoveTimerText;
    public int LoveTimerMax;

    public Text LoveProgressText;
    public GameObject LoveProgressButton;

    [Header("Son")]

    public GameObject Son;
    public GameObject SonWithCharacterButton;
    [NonSerialized] public bool SonWithCharacter;

    [Header("Tree")]
    public GameObject Krystal;
    public GameObject TreeFindedText;
    public GameObject Tree;
    public GameObject TreeButton;
    public bool TreeInBag;

    [Header("Tasks")]
    public Text[] TextsofTasks;
    public GameObject DidParent;
    public Text CurrentTaskText;
    public GameObject TaskGUIGameObject;
    public GameObject TasksPanel;

    [Header("Audio")]
    public AudioSource StepAudio;
    public AudioClip[] FirstSteps;
    public AudioSource MainSound;
    public AudioSource SonRaiseSound;
    public AudioSource TeleportSound;

    public AudioSource XpSound;
    public AudioSource XpLvlUpSound;

    public AudioClip FoodIsReadySound;

    public AudioClip AttackClip;
    public AudioClip[] JumpingClip;

    [Header("Character")]

    [SerializeField] private GameObject[] Armour;

    public float AttackSpeed = 1f;
    public float PlayerSpeed = 2.0f;
    public float PlayerDamage = 1f;

    public float HungerTimerValue = 10f;
    public float FatigueTimerValue = 15f;

    public float StoneTakeMin;
    public float StoneTakeMax;
    public float TreeTakeMin;
    public float TreeTakeMax;

    [Header("Animals")]

    public float AnimalSpeed;
    public float AnimalAttackSpeed;
    public int AnimalHpMin;
    public int AnimalHpMax;
    public int AnimalDistance;
    public int AnimalMinDamage;
    public int AnimalMaxDamage;

    [Header("Barn and Garden")]

    public float BarnTimerValue;
    public float GardenTimerValue;

    [Header("DaySettings")]

    public float HourSecond = 60;










    public static AllObjects Singleton { get; private set; }

    private void Awake()
    {
        Singleton = this;

        if (PlayerPrefs.HasKey("Save"))
        {
            sv = JsonUtility.FromJson<Save>(PlayerPrefs.GetString("Save"));
            SaveUpdate();
        }
        else
        {
            sv.MakedVegetybles = new int[sv.Zerns.Length];
            sv.Vegetybles = new int[sv.Zerns.Length];
            sv.GardenTimer = new float[sv.Zerns.Length];

            sv.MakedMeets = new int[sv.Animals.Length];
            sv.Meets = new int[sv.Animals.Length];
            sv.BarnTimer = new float[sv.Animals.Length];

            sv.BrigdeParts = 1;
            sv.BridgePartGameObjects = new GameObject[3];

            PlayerPrefs.SetString("Save", JsonUtility.ToJson(sv));
            SaveUpdate();
        }

        if (PlayerPrefs.HasKey("Graphics") && PlayerPrefs.GetInt("Graphics") == 0)
        {
            GlobalVolume.gameObject.SetActive(false);
        }
    }

    public void Start()
    {
        TasksPanel.SetActive(false);
        if (!sv.TasksIsEnd)
        {
            CurrentTaskText.text = TextsofTasks[sv.CurrentTask].text;
        }
        else
        {
            TaskGUIGameObject.SetActive(false);
        }

    }

    public void SaveUpdate()
    {
        PlayerPrefs.SetString("Save", JsonUtility.ToJson(sv));

        RockText.text = sv.Rock.ToString();
        TreeText.text = sv.Tree.ToString();

        BridgePartText.text = $"{sv.BrigdeParts}/4";

        for (int i = 0; i < sv.BuildsActives.Length; i++)
        {
            Buildes[i].SetActive(sv.BuildsActives[i]);
        }

        for (int i = 0; i < sv.BridgePartGameObjects.Length; i++)
        {
            if (sv.BridgePartGameObjects[i] != null)
            {
                sv.BridgePartGameObjects[i].SetActive(false);
            }
        }

        if (sv.BuildsActives[(int)Builds.bridge])
        {
            BridgeMessage.SetActive(false);
            NeedABridgeCollider.SetActive(false);
        }

        if (sv.Tasks[(int)Tasks.axe])
        {
            Axe.SetActive(true);
        }

        if (sv.Tasks[(int)Tasks.pickaxe])
        {
            PickAxe.SetActive(true);
        }

        for (int i = 0; i < sv.Tasks.Length; i++)
        {
            if (sv.Tasks[i])
            {
                TextsofTasks[i].gameObject.transform.parent = DidParent.transform;
            }
        }

        for (int i = 0; i < sv.Zerns.Length; i++)
        {
            ZernsTexts[i].text = $"{sv.MakedVegetybles[i]}/{sv.Zerns[i]}";
        }

        for (int i = 0; i < sv.Vegetybles.Length; i++)
        {
            VegatyblesTexts[i].text = sv.Vegetybles[i].ToString();
            VegatyblesTextsInInventory[i].text = sv.Vegetybles[i].ToString();
        }

        for (int i = 0; i < sv.Animals.Length; i++)
        {
            AnimalsTexts[i].text = $"{sv.MakedMeets[i]}/{sv.Animals[i]}";
        }

        for (int i = 0; i < sv.Meets.Length; i++)
        {
            MeetsTexts[i].text = sv.Meets[i].ToString();
        }

        MoneyText.text = $"{sv.Money}$";

        if (sv.TreeIsPlaced)
        {
            Tree.transform.position = sv.TreeTransform;
            Tree.SetActive(true);
            DayTimeMan.DayTextChange();
        }

        if (sv.BuildsActives[(int)Builds.armour])
        {
            for (int i = 0; i < Armour.Length; i++)
            {
                Armour[i].SetActive(true);
            }
        }

        if (!sv.TasksIsEnd)
        {
            CurrentTaskText.text = TextsofTasks[sv.CurrentTask].text;
        }
        else
        {
            TaskGUIGameObject.SetActive(false);
        }

        if (sv.WifeLove <= 5)
        {
            LoveSlider.value = sv.WifeLove;
            LoveSliderText.text = $"{sv.WifeLove}/5";
        }
        else
        {
            Son.SetActive(true);
        }

        XpSlider.maxValue = sv.XpLvl * 2;

        if(sv.XpLvl == 0)
        {
            XpSlider.maxValue = 1;
        }

        if (sv.Xp > XpSlider.maxValue)
        {
            sv.XpLvl++;
            sv.Xp = 0;
            XpSlider.maxValue = sv.XpLvl * 2;
            XpLvlUpSound.PlayOneShot(XpLvlUpSound.clip);
        }

        XpSlider.value = sv.Xp;
        XpText.text = $"{sv.Xp}/{XpSlider.maxValue}";
        XpLvlText.text = $"{sv.XpLvl}";

    }
}

[Serializable] public class Save
{
    public int Rock;
    public int Tree;

    public int BrigdeParts;
    public GameObject[] BridgePartGameObjects;

    public bool[] BuildsActives;

    public bool[] Tasks;

    [Header("Огород")]
    public int[] Zerns;
    public int[] MakedVegetybles;
    public int[] Vegetybles;
    public float[] GardenTimer;

    [Header("Сарай")]
    public int[] Animals;
    public int[] MakedMeets;
    public int[] Meets;
    public float[] BarnTimer;

    public int Money;

    public bool WifeIsFree;

    public bool TreeIsPlaced;
    public Vector3 TreeTransform;

    public bool Tutorial;

    public int CurrentTask;
    public bool TasksIsEnd;

    public int WifeLove;

    public int Xp;
    public int XpLvl;
}

enum Tasks
{
    crafttable, eda, axe, pickaxe, garden, barn, bronya, bridge, main_tree, main_wife, main_home, main_son
}

enum Builds
{
    crafttable, house, garden, barn, axe, pickaxe, bridge, armour
}

enum Zerns
{
    pomidors, kapusta
}

enum Meets
{
    cow, sheep
}

enum ShopItems
{
    Rock, Tree, Zerns, Vegetybles, Animals, Meets
}
