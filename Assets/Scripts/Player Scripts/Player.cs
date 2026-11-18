using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    //资源
    public GameObject blood; //血迹
    public GameObject bloodParticle;//掉血特效
    public GameObject bonus;//奖励特效

    //数据
    public int speed;
    public int maxHp;
    public bool isMoving;//是否移动
    public bool isLocking;//是否锁定了敌人（锁定后朝向由敌人决定，不随移动键改变）
    public float moveAngle;//移动的角度
    public float Hp;//当前血量
    public int regentHpSpeed;//恢复血量的速度（脱战）
    public int delayRegen;//收到攻击后回复血量的冷却时间
    public float delayTimer;//可回复血量的计时器
    private Vector2 dir = Vector2.zero;

    //引用
    private Rigidbody2D playerRig;
    private AudioSource audioSource;
    public List<Enemy> enemyList;
    //组件
    private Rigidbody2D playerRigidbody;
    private void Awake()
    {//Debug.Log("Player OnEnable");

        //数据初始化
        Hp = maxHp = 100;
        //Hp = 0;
        speed = 2;
        regentHpSpeed = delayRegen = 1;
        audioSource = GetComponent<AudioSource>();
        playerRig = GetComponent<Rigidbody2D>();
        enemyList = new List<Enemy>();
        GameManager.Instance.player = this;
        playerRigidbody= GetComponent<Rigidbody2D>();

    }
    //测试
    private void OnEnable()
    {//Debug.Log("Player OnEnable");
     //    GameManager.Instance.player = this;
     //    //数据初始化
     //    Hp = maxHp = 100;
     //    //Hp = 0;
     //    speed = 2;
     //    regentHpSpeed = delayRegen = 1;
     //    audioSource = GetComponent<AudioSource>();
     //    playerRig = GetComponent<Rigidbody2D>();
     //    enemyList = new List<Enemy>();





    }
    private void Start()
    {
       

        //GameManager.Instance.player = this;

        


    }
    private void PlayerMove()
    {
        dir = Vector2.zero;
        // —— 移动部分：永远响应按键，只施加力，不管朝向 ——
        if (Keyboard.current.wKey.isPressed)
        {
            dir = Vector2.up;
            //Debug.Log("Player is moving");
        }
        if (Keyboard.current.aKey.isPressed)
        {
            dir = Vector2.left;
        }
        if (Keyboard.current.sKey.isPressed)
        {
            dir = Vector2.down;
        }
        if (Keyboard.current.dKey.isPressed)
        {
            dir = Vector2.right;
        }
        playerRigidbody.velocity = dir * speed;
        // —— 朝向部分：只有"没有锁定敌人"时，才按移动键设置朝向 ——
        if (!isLocking)
        {
            if (Keyboard.current.wKey.isPressed) moveAngle = 0;
            if (Keyboard.current.aKey.isPressed) moveAngle = 90;
            if (Keyboard.current.sKey.isPressed) moveAngle = -180;
            if (Keyboard.current.dKey.isPressed) moveAngle = -90;

            transform.rotation = Quaternion.Euler(0, 0, moveAngle);
        }
    }
    void Update()
    {
        PlayerMove();

        RegentHP();

    }
    /// <summary>
    /// 血量回复HP
    /// </summary>
    private void RegentHP()
    {
        if (delayTimer <= 0 && Hp < maxHp)
        {
            Hp += regentHpSpeed * Time.deltaTime;
            if (Hp > maxHp)
            {
                Hp = maxHp;
            }
        }
        else if (delayTimer > 0)
        {
            delayTimer -= Time.deltaTime;

        }

    }
}