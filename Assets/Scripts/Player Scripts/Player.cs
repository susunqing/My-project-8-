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
    public float moveAngle;//移动的角度
    public float Hp;//当前血量
    public int regentHpSpeed;//恢复血量的速度（脱战）
    public int delayRegen;//收到攻击后回复血量的冷却时间
    public float delayTimer;//可回复血量的计时器

    //引用
    private Rigidbody2D playerRig;
    private AudioSource audioSource;
    public List<Enemy> enemyList;
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
        Debug.Log("Player Start执行");

        GameManager.Instance.player = this;

        Debug.Log("注册完成");


    }
    private void PlayerMove()
    {
        if (Keyboard.current.wKey.isPressed)
        {
            playerRig.AddForce(new Vector2 (0, speed)*Time.deltaTime*60);
            moveAngle = 0;
        }
        if (Keyboard.current.aKey.isPressed)
        {
            playerRig.AddForce(new Vector2(-speed, 0) * Time.deltaTime * 60);
            moveAngle = 90;
        }
        if (Keyboard.current.sKey.isPressed)
        {
            playerRig.AddForce(new Vector2(0, -speed) * Time.deltaTime * 60);
            moveAngle = -180;
        }
        if (Keyboard.current.dKey.isPressed)
        {
            playerRig.AddForce(new Vector2(speed, 0) * Time.deltaTime * 60);
            moveAngle = -90;
        }
        transform.rotation = Quaternion.Euler(0, 0, moveAngle);
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
        if (delayTimer<=0 && Hp < maxHp)
        {
            Hp += regentHpSpeed * Time.deltaTime;
            if (Hp > maxHp)
            {
                Hp = maxHp;
            }
        }
        else if (delayTimer > 0){
           delayTimer-=Time.deltaTime;

        }

    }
}
