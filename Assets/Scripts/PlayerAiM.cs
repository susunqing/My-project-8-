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

    // Start is called before the first frame update
    void Start()
    {
        if (GameManager.Instance != null)
        {
            player = GameManager.Instance.player;
        }
        if (mark != null)
        {
            mark.SetActive(false);
        }
        minDistance = maxDistance = 10;
    }

    // Update is called once per frame
    void Update()
    {
        // 尝试重新获取 player，防止 Start 执行时 GameManager 还未准备好
        if (player == null)
        {
            if (GameManager.Instance != null)
            {
                player = GameManager.Instance.player;
            }
            return; // player 为空时不继续执行
        }

        if (player.enemyList == null) return;

        for (int i = 0; i < player.enemyList.Count; i++)
        {
            if (player.enemyList[i] == null) continue; // 跳过空的敌人

            hit = Physics2D.Raycast(player.transform.position,
                player.enemyList[i].transform.position - player.transform.position);

            if (hit.collider != null)
            {
                if (mark != null)
                {
                    mark.SetActive(true);
                    mark.transform.position = hit.collider.transform.position;
                }
            }
        }
    }
}