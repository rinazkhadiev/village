using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    [SerializeField] private string _item;
    [SerializeField] private int _index;
    [SerializeField] private int _price;

    [SerializeField] private Text _priceText;

    private void Start()
    {
        _priceText.text = $"{_price}$";
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
    }
}
