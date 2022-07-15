using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Shop : MonoBehaviour
{
    [SerializeField] private string _item;
    [SerializeField] private int _index;
    [SerializeField] private int _price;

    [SerializeField] private Text _priceText;
    [SerializeField] private Text _howMuchText;

    private void Start()
    {
        _priceText.text = $"{_price}$";
    }

    private void Update()
    {
        switch (_item)
        {
            case "Rock":
                _howMuchText.text = AllObjects.Singleton.sv.Rock.ToString();
                break;
            case "Tree":
                _howMuchText.text = AllObjects.Singleton.sv.Tree.ToString();
                break;
            case "Zerns":
                _howMuchText.text = AllObjects.Singleton.sv.Zerns[_index].ToString();
                break;
            case "Vegetybles":
                _howMuchText.text = AllObjects.Singleton.sv.Vegetybles[_index].ToString();
                break;
            case "Animals":
                _howMuchText.text = AllObjects.Singleton.sv.Animals[_index].ToString();
                break;
            case "Meets":
                _howMuchText.text = AllObjects.Singleton.sv.Meets[_index].ToString();
                break;
        }
    }

    public void Buy()
    {
        if (AllObjects.Singleton.sv.Money >= _price)
        {
            switch (_item)
            {
                case "Rock":
                    AllObjects.Singleton.sv.Rock++;
                    break;
                case "Tree":
                    AllObjects.Singleton.sv.Tree++;
                    break;
                case "Zerns":
                    AllObjects.Singleton.sv.Zerns[_index]++;
                    break;
                case "Vegetybles":
                    AllObjects.Singleton.sv.Vegetybles[_index]++;
                    break;
                case "Animals":
                    AllObjects.Singleton.sv.Animals[_index]++;
                    break;
                case "Meets":
                    AllObjects.Singleton.sv.Meets[_index]++;
                    break;

            }

            AllObjects.Singleton.sv.Money -= _price;
            AllObjects.Singleton.SaveUpdate();
        }
        else
        {
            StartCoroutine(HaveNotMoney());
        }
    }

    public void Sell()
    {
        switch (_item)
        {
            case "Rock":
                if(AllObjects.Singleton.sv.Rock >= 1)
                {
                    AllObjects.Singleton.sv.Rock--;
                    AllObjects.Singleton.sv.Money += _price;
                }
                else
                {
                    StartCoroutine(HaveNotResources());
                }
                break;
            case "Tree":
                if (AllObjects.Singleton.sv.Tree >= 1)
                {
                    AllObjects.Singleton.sv.Tree--;
                    AllObjects.Singleton.sv.Money += _price;
                }
                else
                {
                    StartCoroutine(HaveNotResources());
                }
                break;
            case "Zerns":
                if (AllObjects.Singleton.sv.Zerns[_index] >= 1)
                {
                    AllObjects.Singleton.sv.Zerns[_index]--;
                    AllObjects.Singleton.sv.Money += _price;
                }
                else
                {
                    StartCoroutine(HaveNotResources());
                }
                break;
            case "Vegetybles":
                if (AllObjects.Singleton.sv.Vegetybles[_index] >= 1)
                {
                    AllObjects.Singleton.sv.Vegetybles[_index]--;
                    AllObjects.Singleton.sv.Money += _price;
                }
                else
                {
                    StartCoroutine(HaveNotResources());
                }
                break;
            case "Animals":
                if (AllObjects.Singleton.sv.Animals[_index] >= 1)
                {
                    AllObjects.Singleton.sv.Animals[_index]--;
                    AllObjects.Singleton.sv.Money += _price;
                }
                else
                {
                    StartCoroutine(HaveNotResources());
                }
                break;
            case "Meets":
                if (AllObjects.Singleton.sv.Meets[_index] >= 1)
                {
                    AllObjects.Singleton.sv.Meets[_index]--;
                    AllObjects.Singleton.sv.Money += _price;
                }
                else
                {
                    StartCoroutine(HaveNotResources());
                }
                break;
        }

        AllObjects.Singleton.SaveUpdate();
    }

    IEnumerator HaveNotMoney()
    {
        AllObjects.Singleton.HaveNotMoneyText.SetActive(true);
        AllObjects.Singleton.MoneyText.color = Color.red;
        yield return new WaitForSeconds(0.5f);
        AllObjects.Singleton.MoneyText.color = Color.green;
        yield return new WaitForSeconds(0.5f);
        AllObjects.Singleton.MoneyText.color = Color.red;
        yield return new WaitForSeconds(0.5f);
        AllObjects.Singleton.MoneyText.color = Color.green;
        yield return new WaitForSeconds(2);
        AllObjects.Singleton.HaveNotMoneyText.SetActive(false);
    }

    IEnumerator HaveNotResources()
    {
        AllObjects.Singleton.HaveNotResoursesText.SetActive(true);
        _howMuchText.color = Color.red;
        yield return new WaitForSeconds(0.5f);
        _howMuchText.color = Color.white;
        yield return new WaitForSeconds(0.5f);
        _howMuchText.color = Color.red;
        yield return new WaitForSeconds(0.5f); 
        _howMuchText.color = Color.white;
        yield return new WaitForSeconds(2);
        AllObjects.Singleton.HaveNotResoursesText.SetActive(false);
    }
}
