using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private Player player;

    IEnumerator Start()
    {
        yield return null;

        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager.Instance 为空！");
            yield break;
        }

            player = GameManager.Instance.player;

        if (player == null)
        {
            Debug.LogError("player为空");
            yield break;
        }

        if (player.enemyList != null)
        {
            player.enemyList.Add(this);
        }

        Debug.Log(player.transform.position);
    }
}