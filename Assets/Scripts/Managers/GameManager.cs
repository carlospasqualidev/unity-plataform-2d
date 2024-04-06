using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public GameObject collectableItemEffect;
    public GameObject enemyDieEffect;
    public Image[] imageLifeCherrys;
    public int maxPlayerLife = 3;
    public int playerLife;
    public int playerScore;

    void Start()
    {
        if (instance == null)
            instance = this;

        playerLife = maxPlayerLife;
    }

    public void DecrementPlayerLife()
    {
        if (playerLife < 1)
        {
            print("Game Over!");
            return;
        }
        playerLife--;

        imageLifeCherrys[playerLife].color = Color.gray;
        print("Player Life: " + playerLife);
    }

    public void IncrementPlayerLife()
    {
        if (playerLife < maxPlayerLife)
        {
            playerLife++;
        }
        imageLifeCherrys[playerLife - 1].color = Color.white;
        print("Player Life: " + playerLife);
    }

    public void IncrementPlayerScore()
    {
        playerScore++;
    }
}
