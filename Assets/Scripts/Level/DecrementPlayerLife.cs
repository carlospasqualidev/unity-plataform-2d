using UnityEngine;

public class DecrementPlayerLife : MonoBehaviour
{
    [SerializeField]
    private int lifeForDecrement = 1000000;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.instance.DecrementPlayerLife(lifeForDecrement);
        }
    }
}
