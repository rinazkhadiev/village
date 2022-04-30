using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class AllObjects : MonoBehaviour
{
    [Header("������ �������")]
    public Save sv;
    public JoystickController JoyController;
   
    [Header("��������")]
    [NonSerialized] public bool CharacterIsBusy;

    [Header("���������� ���")]
    public GameObject[] TakingItems;
    public Animal[] Animals;
    public GameObject Shoper;

    [Header("���������")]
    public TextMeshProUGUI RockText;
    public TextMeshProUGUI TreeText;

    public TextMeshProUGUI[] ZernsTexts;
    public TextMeshProUGUI[] VegatyblesTexts;

    public TextMeshProUGUI[] AnimalsTexts;
    public TextMeshProUGUI[] MeetsTexts;

    [Header("���������")]
    public GameObject[] Buildes;

    [NonSerialized] public GameObject TeleportBuild;
    [NonSerialized] public float TeleportBuildY;

    public GameObject[] Vegatybles;

    [Header("UI")]
    public GameObject TakeItButton;
    public Slider TakingSlider;

    public Image HpImage;
    public TextMeshProUGUI HpText;
    public Image HungerImage;
    public TextMeshProUGUI HungerText;

    public GameObject TeleportPanel;

    public GameObject BarnButton;
    public GameObject GardenButton;
    public Image[] BarnTimer;
    public Image[] GardenTimer;

    public GameObject ShoperButton;
    public TextMeshProUGUI MoneyText;

    [Header("Tasks")]
    public TextMeshProUGUI[] TextsofTasks;
    public GameObject DidParent;

    [Header("Audio")]
    public AudioSource StepAudio;
    public AudioClip[] FirstSteps;

  

    [Header("Character")]

    public float AttackSpeed;
    public float PlayerSpeed = 2.0f;
    public float JumpHeight = 1.0f;

    public float HungerTimerValue = 10f;

    public int StoneTakeMin;
    public int StoneTakeMax;
    public int TreeTakeMin;
    public int TreeTakeMax;

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










    public static AllObjects Singleton { get; private set; }

    private void Awake()
    {
        Singleton = this;
    }

    private void Start()
    {
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

            PlayerPrefs.SetString("Save", JsonUtility.ToJson(sv));
            SaveUpdate();
        }
    }

    public void SaveUpdate()
    {
        PlayerPrefs.SetString("Save", JsonUtility.ToJson(sv));

        RockText.text = sv.Rock.ToString();
        TreeText.text = sv.Tree.ToString();

        for (int i = 0; i < sv.BuildsActives.Length; i++)
        {
            Buildes[i].SetActive(sv.BuildsActives[i]);
        }

        for (int i = 0; i < sv.Tasks.Length; i++)
        {
            if (sv.Tasks[i])
            {
                TextsofTasks[i].fontStyle = FontStyles.Strikethrough;
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
    }
}

[Serializable] public class Save
{
    public int Rock;
    public int Tree;

    public bool[] BuildsActives;

    public bool[] Tasks;

    [Header("������")]
    public int[] Zerns;
    public int[] MakedVegetybles;
    public int[] Vegetybles;
    public float[] GardenTimer;

    [Header("�����")]
    public int[] Animals;
    public int[] MakedMeets;
    public int[] Meets;
    public float[] BarnTimer;

    public int Money;
}

enum Tasks
{
    crafttable, eda, axe, pickaxe, garden, barn, bronya
}

enum Builds
{
    crafttable, house, garden, barn, axe, pickaxe
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
