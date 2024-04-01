using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public GameObject collectableItemEffect;
    public GameObject enemyDieEffect;

    void Start()
    {
        if (instance == null)
            instance = this;
    }
}
