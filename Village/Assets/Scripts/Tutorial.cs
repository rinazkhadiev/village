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

    private void Start()
    {
        Singleton = this;
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
        }
    }

    public void SkipPanel()
    {
        switch (PanelIndex)
        {
            case (int)Steps.Stone:
                if (Stone)
                {
                    _panels[PanelIndex].SetActive(false);
                    PanelIndex++;
                    _timer = 0;
                }
                break;

            case (int)Steps.Tree:
                if (Tree)
                {
                    _panels[PanelIndex].SetActive(false);
                    PanelIndex++;
                    _timer = 0;
                }
                break;

            case (int)Steps.CraftTable:
                if (AllObjects.Singleton.sv.BuildsActives[(int)Builds.crafttable])
                {
                    _panels[PanelIndex].SetActive(false);
                    PanelIndex++;
                    _timer = 0;
                }
                break;

            case (int)Steps.Hunting_ready:
                AllObjects.Singleton.Animals[0].transform.position = AllObjects.Singleton.BearTutorialTransform.position;
                    _panels[PanelIndex].SetActive(false);
                    PanelIndex++;
                    _timer = 0;
                break;

            case (int)Steps.Hunting:
                if (Hunting)
                {
                    _panels[PanelIndex].SetActive(false);
                    PanelIndex++;
                    _timer = 0;
                }
                break;

            case (int)Steps.Food:
                if (Food)
                {
                    _panels[PanelIndex].SetActive(false);
                    PanelIndex++;
                    _timer = 0;
                }
                break;

            case (int)Steps.End:
                AllObjects.Singleton.sv.Tutorial = true;
                AllObjects.Singleton.SaveUpdate();
                _panels[PanelIndex].SetActive(false);
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

