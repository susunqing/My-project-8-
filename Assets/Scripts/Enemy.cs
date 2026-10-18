 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    private Player player;
    // Start is called before the first frame update
    void Start()
    {
        player = GameManager.Instance.player;
        player.enemyList.Add(this);

    }

    // Update is called once per frame
    void Update()
    {

    }
}