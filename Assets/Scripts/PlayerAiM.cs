using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 玩家工具逻辑脚本
/// </summary>
public class PlayerAiM : MonoBehaviour
{
    public Transform cameraTrans;
    public GameObject mark;//用来标记敌人的标记
    public float minDistance;
    public float maxDistance;//最近最远距离
    private Player player;
    private Enemy nearEnemy;//最近的敌人
    private RaycastHit2D hit;//定义一个射线
    private int layValue;
    
    private float distance;//怪物与玩家的距离
    // Start is called before the first frame update
    void Start()
    {
        player = GameManager.Instance.player;
        mark.SetActive(false);
        minDistance = maxDistance = 10;
        layValue = ~(1 << 9);//定义一个layvalue变量，表示敌人所在的层级
    }
    //LayerMask LayerMask = ~(1 << 0);
    // 射线建设是用一个32位的整数来表示的，每一位代表一个层级，1表示该层级被包含在射线检测中，
    //0表示不包含。通过按位取反操作符~，可以将指定的层级排除在射线检测之外。第零层表示为1
    // Update is called once per frame
    void Update()
    {
        // 每帧先把"最近敌人"和"最近距离"重置，再重新扫描
        nearEnemy = null;
        minDistance = maxDistance;

        for (int i = 0; i < player.enemyList.Count; i++)
        {
            // 从玩家向敌人方向发射射线（方向向量归一化）
            hit = Physics2D.Raycast(transform.position, (player.enemyList[i].transform.position - player.transform.position).normalized, 200f, layValue);
            Debug.Log(hit.collider);

            // 射线命中了物体
            if (hit.collider != null)
            {
                // 如果射线先命中的不是敌人（比如墙壁），说明敌人被遮挡，跳过这个敌人
                if (!hit.collider.CompareTag("Enemy"))
                {
                    continue;
                }

                // 命中敌人，计算距离，找最近的敌人
                float distance = Vector3.Distance(player.enemyList[i].transform.position, player.transform.position);
                if (distance < minDistance)//在玩家视野里，找最近的敌人
                {
                    minDistance = distance;
                    nearEnemy = player.enemyList[i];
                }
            }
        }

        //有目标
        if (nearEnemy != null)
        {
            player.isLocking = true;   // 锁定：朝向交给敌人控制

            mark.SetActive(true);
            mark.transform.position = nearEnemy.transform.position;

            //瞄准之后玩家要看向敌人
            Vector3 moveDirection = nearEnemy.transform.position - player.transform.position;
            if (moveDirection != Vector3.zero)
            {
                float angle = Mathf.Atan2(moveDirection.x, moveDirection.y) * Mathf.Rad2Deg;
                player.moveAngle = -angle;
                // 立即旋转，让玩家面朝敌人
                player.transform.rotation = Quaternion.Euler(0, 0, player.moveAngle);
            }
            if(hit.collider.CompareTag("Wall"))
            {
                nearEnemy = null;
                minDistance = maxDistance;
            }

        }
        else
        {
            // 没有目标：解除锁定，朝向交还按键控制
            player.isLocking = false;
            mark.SetActive(false);
        }

        cameraTrans.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, -3);
    }
}