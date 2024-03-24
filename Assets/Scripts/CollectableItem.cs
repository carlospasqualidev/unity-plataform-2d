using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableItem : MonoBehaviour
{
    private CircleCollider2D coll;

    [SerializeField]
    private AudioClip[] collectableItemSFX;

    // Start is called before the first frame update
    void Start()
    {
        coll = GetComponent<CircleCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
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
}
