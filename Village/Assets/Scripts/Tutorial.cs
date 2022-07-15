using UnityEngine;

public class Tutorial : MonoBehaviour
{
    public static Tutorial Singleton { get; private set; }

    [Header("Steps")]
    public bool Stone;
    public bool Tree;
    public bool Hunting;
    public bool Food;

    public int PanelIndex;
    [SerializeField] private GameObject[] _panels;

    private float _timer;

    private bool _joystick;

    private void Start()
    {
        Singleton = this;
        Analytics.Singleton.OnEvent("5. Game is start");
    } 

    public void DoStep(ref bool Step, int panelIndex)
    {
        if (!AllObjects.Singleton.sv.Tutorial)
        {
            if (!Step && PanelIndex == panelIndex)
            {
                Step = true;
                SkipPanel();
            }
        }
    }

    private void Update()
    {
        if (!AllObjects.Singleton.sv.Tutorial)
        {
            _timer += Time.deltaTime;

            if (_timer >= 5)
            {
                SkipPanel();
            }
            else
            {
                for (int i = 0; i < _panels.Length; i++)
                {
                    _panels[i].SetActive(false);
                }

                _panels[PanelIndex].SetActive(true);
            }

            if (_joystick)
            {
                AllObjects.Singleton.JoyStickBG.SetActive(true);
            }
            else
            {
                AllObjects.Singleton.JoyStickBG.SetActive(false);
            }
        }
    }

    public void SkipPanel()
    {
        switch (PanelIndex)
        {
            case (int)Steps.Welcome:
                _panels[PanelIndex].SetActive(false);
                PanelIndex++;
                _timer = 0;
                Analytics.Singleton.OnEvent("6. Tutorial_Welcome");
                break;

            case (int)Steps.UI:
                _panels[PanelIndex].SetActive(false);
                PanelIndex++;
                _timer = 0;
                Analytics.Singleton.OnEvent("7. Tutorial_UI");
                _joystick = true;
                break;

            case (int)Steps.Interface:
                _panels[PanelIndex].SetActive(false);
                PanelIndex++;
                _timer = 0;
                Analytics.Singleton.OnEvent("8. Tutorial_Interface");
                AllObjects.Singleton.ArrowsToRocks.SetActive(true);
                break;

            case (int)Steps.Stone:
                if (Stone)
                {
                    _panels[PanelIndex].SetActive(false);
                    PanelIndex++;
                    _timer = 0;
                    Analytics.Singleton.OnEvent("9. Tutorial_Stone");
                    AllObjects.Singleton.ArrowsToRocks.SetActive(false);
                    AllObjects.Singleton.ArrowsToTrees.SetActive(true);
                }
                break;

            case (int)Steps.Tree:
                if (Tree)
                {
                    _panels[PanelIndex].SetActive(false);
                    PanelIndex++;
                    _timer = 0;
                    Analytics.Singleton.OnEvent("10. Tutorial_Tree");
                    AllObjects.Singleton.ArrowsToTrees.SetActive(false);
                    AllObjects.Singleton.InventoryAnimator.Play("Anim");
                    AllObjects.Singleton.InventoryArrow.SetActive(true);
                    AllObjects.Singleton.CraftTableArrow.SetActive(true);
                }
                break;

            case (int)Steps.CraftTable:
                if (AllObjects.Singleton.sv.BuildsActives[(int)Builds.crafttable])
                {
                    _panels[PanelIndex].SetActive(false);
                    PanelIndex++;
                    _timer = 0;
                    Analytics.Singleton.OnEvent("11. Tutorial_CraftTable");
                    AllObjects.Singleton.InventoryAnimator.Play("Non");
                    AllObjects.Singleton.InventoryArrow.SetActive(false);
                    AllObjects.Singleton.CraftTableArrow.SetActive(false);
                }
                break;

            case (int)Steps.Hunting_ready:
                AllObjects.Singleton.Animals[0].transform.position = AllObjects.Singleton.BearTutorialTransform.position;
                    _panels[PanelIndex].SetActive(false);
                    PanelIndex++;
                    _timer = 0;
                Analytics.Singleton.OnEvent("12. Tutorial_HuntingReady?");
                break;

            case (int)Steps.Hunting:
                if (Hunting)
                {
                    _panels[PanelIndex].SetActive(false);
                    PanelIndex++;
                    _timer = 0;
                    Analytics.Singleton.OnEvent("13. Tutorial_Hunting");
                    AllObjects.Singleton.InventoryAnimator.Play("Anim");
                    AllObjects.Singleton.InventoryArrow.SetActive(true);
                    AllObjects.Singleton.FoodArrow.SetActive(true);
                }
                break;

            case (int)Steps.Food:
                if (Food)
                {
                    _panels[PanelIndex].SetActive(false);
                    PanelIndex++;
                    _timer = 0;
                    Analytics.Singleton.OnEvent("14. Tutorial_Food");
                    AllObjects.Singleton.InventoryAnimator.Play("Non");
                    AllObjects.Singleton.InventoryArrow.SetActive(false);
                    AllObjects.Singleton.FoodArrow.SetActive(false);
                }
                break;

            case (int)Steps.End:
                AllObjects.Singleton.sv.Tutorial = true;
                AllObjects.Singleton.SaveUpdate();
                _panels[PanelIndex].SetActive(false);
                Analytics.Singleton.OnEvent("15. Tutorial_End");
                UserInterface.Singleton.XpPlus(5);
                break;

            default:
                _panels[PanelIndex].SetActive(false);
                PanelIndex++;
                _timer = 0;
                break;
        }
    }
}
enum Steps
{
    Welcome, UI, Interface, Stone, Tree, CraftTable, Hunting_ready, Hunting, Food, End
}

