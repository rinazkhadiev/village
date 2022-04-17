using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class AllObjects : MonoBehaviour
{
    [Header("Другие скрипты")]
    public Save sv = new Save();
    public JoystickController JoyController;
   
    [Header("Персонаж")]
    [NonSerialized] public bool CharacterIsBusy;
   
    [Header("Окружающий мир")]
    public GameObject[] TakingItems;
    public Animal[] Animals;

    [Header("Инвентарь")]
    public TextMeshProUGUI RockText;
    public TextMeshProUGUI TreeText;

    [Header("Постройки")]
    public GameObject[] Buildes;

    [NonSerialized] public GameObject TeleportBuild;
    [NonSerialized] public float TeleportBuildY;

    [Header("UI")]
    public GameObject TakeItButton;
    public Slider TakingSlider;
    public TextMeshProUGUI HpText;
    public GameObject TeleportPanel;

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

        if (PlayerPrefs.HasKey("Save"))
        {
            sv = JsonUtility.FromJson<Save>(PlayerPrefs.GetString("Save"));
            SaveUpdate();
        }
    }

    public void SaveUpdate()
    {
        RockText.text = sv.Rock.ToString();
        TreeText.text = sv.Tree.ToString();

        for (int i = 0; i < Buildes.Length; i++)
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
    }
}

[Serializable] public class Save
{
    public int Rock;
    public int Tree;
    public bool[] BuildsActives;
    public bool[] Tasks;
}
