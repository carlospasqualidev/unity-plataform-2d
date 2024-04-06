using UnityEngine;

enum CollectableItemType
{
    Health,
    Score,
}

public class CollectableItem : MonoBehaviour
{
    [SerializeField]
    private AudioClip[] collectableItemSFX;

    [SerializeField]
    private CollectableItemType Action;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            ExecuteAction();
            SFXController.instance.PlaySound(collectableItemSFX, 2);
            GameObject effect = Instantiate(
                GameManager.instance.collectableItemEffect,
                transform.position,
                Quaternion.identity
            );
            Destroy(gameObject);
            Destroy(effect, 0.35f);
        }
    }

    private void ExecuteAction()
    {
        switch (Action)
        {
            case CollectableItemType.Health:
                GameManager.instance.IncrementPlayerLife();
                break;
            case CollectableItemType.Score:
                GameManager.instance.IncrementPlayerScore();
                break;
        }
    }
}
