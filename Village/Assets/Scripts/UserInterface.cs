using UnityEngine;
using System.Collections;

public class UserInterface : MonoBehaviour
{
    private GameObject _currentItem;
    private int _currentI;
    private int _takingTimer;

    private float _attackTimer;

    private void Update()
    {
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

        if(_takingTimer > 0)
        {
            _takingTimer -= (int)Time.deltaTime;
            AllObjects.Singleton.TakingSlider.value += Time.deltaTime;
        }

        if(_attackTimer > 0)
        {
            _attackTimer -= Time.deltaTime;
        }
    }

    #region Build

    public void TakeItem()
    {
        if (!AllObjects.Singleton.CharacterIsBusy)
        {
            if (_currentItem.tag == "Rock")
            {
                _takingTimer = Random.Range(1, 2);
            }
            else if (_currentItem.tag == "Tree")
            {
                _takingTimer = Random.Range(3, 5);
            }

            StartCoroutine(ItemTaking(_takingTimer));
            StartCoroutine(RespawnItem(_currentI));
        }
    }

    IEnumerator ItemTaking(int takingTimer)
    {
        AllObjects.Singleton.CharacterIsBusy = true;
        AllObjects.Singleton.TakingSlider.value = 0;
        AllObjects.Singleton.TakingSlider.maxValue = takingTimer;
        AllObjects.Singleton.TakingSlider.gameObject.SetActive(true);

        yield return new WaitForSeconds(takingTimer);

        if (_currentItem.tag == "Rock") AllObjects.Singleton.sv.Rock++;
        else if (_currentItem.tag == "Tree") AllObjects.Singleton.sv.Tree++;
        PlayerPrefs.SetString("Save", JsonUtility.ToJson(AllObjects.Singleton.sv));
        AllObjects.Singleton.SaveUpdate();

        AllObjects.Singleton.CharacterIsBusy = false;
        AllObjects.Singleton.TakingSlider.gameObject.SetActive(false);
        AllObjects.Singleton.TakingItems[_currentI].SetActive(false);
        _currentItem = null;
    }

    IEnumerator RespawnItem(int currentI)
    {
        yield return new WaitForSeconds(Random.Range(60,120));
        AllObjects.Singleton.TakingItems[currentI].SetActive(true);
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

            _attackTimer = 1.5f;
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

    #region
    
    public void DoTask(int taskNubmer)
    {
        AllObjects.Singleton.sv.Tasks[taskNubmer] = true;
        AllObjects.Singleton.SaveUpdate();
    }

    #endregion
}
