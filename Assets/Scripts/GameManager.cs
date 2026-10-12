using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //单例
    public static GameManager Instance { get; private set; }
    //玩家数据
    [HideInInspector]
    public int unLockLevel;
    public int selectLevel;
    public int money;
    public int gunLevel;
    public Player player;
    //系统设置
    public float volume;
    public float joystickSize;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 加载关卡
    /// </summary>
    private void LoadGame()
    {
        if (PlayerPrefs.HasKey("Money"))
        {
            money = PlayerPrefs.GetInt("Money");
        }
        else
        {
            money = 0;
        }
        if (PlayerPrefs.HasKey("Levels"))
        {
            unLockLevel = PlayerPrefs.GetInt("Levels");
        }
        else
        {
            unLockLevel = 1;
        }
        if (PlayerPrefs.HasKey("Sound"))
        {
            volume = PlayerPrefs.GetFloat("Sound");
        }
        else { volume = 0; }
        if (PlayerPrefs.HasKey("JoyStick"))
        {
            joystickSize = PlayerPrefs.GetFloat("JoyStick");
        }
        else
        {
            joystickSize = 0.3f;
        }
    }
}