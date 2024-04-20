using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public GameObject collectableItemEffect;
    public GameObject enemyDieEffect;
    public Image[] imageLifeCherrys;
    public TextMeshProUGUI playerScoreText;
    public int maxPlayerLife = 3;
    public int playerLife;
    public int playerScore;

    void Start()
    {
        if (instance == null)
            instance = this;

        playerLife = maxPlayerLife;
    }

    public void DecrementPlayerLife(int life = 1)
    {
        playerLife -= life;

        if (playerLife < 1)
        {
            print("Game Over!");

            imageLifeCherrys[0].color = Color.gray;
            return;
        }

        imageLifeCherrys[playerLife].color = Color.gray;
    }

    public void IncrementPlayerLife()
    {
        if (playerLife < maxPlayerLife)
        {
            playerLife++;
        }
        imageLifeCherrys[playerLife - 1].color = Color.white;
    }

    public void IncrementPlayerScore()
    {
        playerScore++;
        playerScoreText.text = playerScore.ToString("D3");
    }
}
