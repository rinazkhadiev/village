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
    public float HungerTimerValue = 10f;

    [Header("���������� ���")]
    public GameObject[] TakingItems;
    public Animal[] Animals;

    [Header("���������")]
    public TextMeshProUGUI RockText;
    public TextMeshProUGUI TreeText;
    public TextMeshProUGUI[] ZernsTexts;
    public TextMeshProUGUI[] FoodsTexts;

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
    public Image BarnTimer;
    public Image[] GardenTimer;

    [Header("Tasks")]
    public TextMeshProUGUI[] TextsofTasks;
    public GameObject DidParent;

    [Header("Audio")]
    public AudioSource StepAudio;
    public AudioClip[] FirstSteps;










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
            ZernsTexts[i].text = $"{sv.MakedFoods[i]}/{sv.Zerns[i]}";
        }

        for (int i = 0; i < sv.Foods.Length; i++)
        {
            FoodsTexts[i].text = sv.Foods[i].ToString();
        }
    }
}

[Serializable] public class Save
{
    public int Rock;
    public int Tree;

    public bool[] BuildsActives;

    public bool[] Tasks;

    public int[] Zerns;
    public int[] MakedFoods;
    public int[] Foods;
}

enum Tasks
{
    crafttable, eda, axe, pickaxe, garden, barn, bronya
}

enum Builds
{
    crafttable, house, garden, barn, axe, pickaxe
}
