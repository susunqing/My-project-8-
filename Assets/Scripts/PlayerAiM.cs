using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 玩家工具逻辑脚本
/// </summary>
public class PlayerAiM : MonoBehaviour
{
    public Transform CameraTrans;
    public GameObject mark;//用来标记敌人的标记
    public float minDistance;
    public float maxDistance;//最近最远距离
    private Player player;
    private Enemy neraEnemy;//最近的敌人
    private RaycastHit2D hit;//定义一个射线
    private int layValue;
    // Start is called before the first frame update
    void Start()
    {
        player = GameManager.Instance.player;
        mark.SetActive(false);
        minDistance = maxDistance = 0;
        layValue = 1 << 10;//定义一个layvalue变量，表示敌人所在的层级
    }

    // Update is called once per frame
    void Update()
    {
       
        for (int i = 0; i < player.enemyList.Count; i++)
        {
            hit = Physics2D.Raycast(transform.position,
                player.enemyList[i].transform.position - player.transform.position,200f,layValue);
            Debug.Log(hit.collider);
            if (hit.collider != null)     
            {
                mark.SetActive(true);
                mark.transform.position = hit.collider.transform.position;     
            }


        }
    }
}