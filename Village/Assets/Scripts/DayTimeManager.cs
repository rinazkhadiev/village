using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class DayTimeManager : MonoBehaviour
{
    private float _second;
    private int _hour;

    private float _day;

    private bool _isNight;

    private ColorAdjustments _colorAdjustments;
    private VolumeParameter<float> _vp = new VolumeParameter<float>();

    private void Start()
    {
        if (PlayerPrefs.HasKey("Time"))
        {
            _hour = PlayerPrefs.GetInt("Time");
        }
        else
        {
            _hour = 6;
        }

        if (PlayerPrefs.HasKey("Day"))
        {
            _day = PlayerPrefs.GetFloat("Day");
        }
        else
        {
            _day = 1;
        }

        DayTextChange();
        AllObjects.Singleton.TimeText.text = $"{_hour}:00";
        AllObjects.Singleton.GlobalVolume.profile.TryGet<ColorAdjustments>(out _colorAdjustments);


    }

    private void Update()
    {
        _second += Time.deltaTime;

        if (_second >= AllObjects.Singleton.HourSecond)
        {
            _hour++;
            if (_hour >= 24)
            {
                _hour = 0;
                NewDay();
            }
            PlayerPrefs.SetInt("Time", _hour);
            AllObjects.Singleton.TimeText.text = $"{_hour}:00";

            _second = 0;
        }



        if (_hour <= 5)
        {
            _vp.value = _hour - 5;
            _isNight = true;
        }
        else if (_hour >= 6 && _hour <= 18)
        {
            _vp.value = 0.75f;
            _isNight = false;
        }
        else if (_hour >= 19)
        {
            _vp.value = 19 - _hour;
            _isNight = true;
        }

        _colorAdjustments.postExposure.SetValue(_vp);

        if (Vector3.Distance(Character.Singleton.Transform.position, AllObjects.Singleton.Buildes[(int)Builds.house].transform.position) < 10 && AllObjects.Singleton.Buildes[(int)Builds.house].activeSelf)
        {
            AllObjects.Singleton.SleepButton.SetActive(true);
        }
        else
        {
            AllObjects.Singleton.SleepButton.SetActive(false);
        }
    }

    private void ToSleep()
    {
        if (_isNight)
        {
            Character.Singleton.Fatigue = 100;
            Character.Singleton.FatigueChange(0);

            _hour = 6;
            PlayerPrefs.SetInt("Time", _hour);
            AllObjects.Singleton.TimeText.text = $"{_hour}:00";
            _second = 0;

            NewDay();
        }
    }

    private void NewDay()
    {
        _day++;
        PlayerPrefs.SetFloat("Day", _day);
        DayTextChange();
    }

    public void DayTextChange()
    {
        if (PlayerPrefs.HasKey("Language"))
        {
            if (PlayerPrefs.GetString("Language") == "ru")
            {
                AllObjects.Singleton.DayText.text = $"Δενό {_day}";
            }
            else if (PlayerPrefs.GetString("Language") == "en")
            {
                AllObjects.Singleton.DayText.text = $"Day {_day}";
            }
        }
        else
        {
            AllObjects.Singleton.DayText.text = $"Δενό {_day}";
        }

        if(_day <= 100)
        {
            AllObjects.Singleton.Tree.transform.localScale = new Vector3(_day / 10, _day / 10, _day / 10);
        }
        else
        {
            AllObjects.Singleton.Tree.transform.localScale = new Vector3(10, 10, 10);
        }
    }
}
