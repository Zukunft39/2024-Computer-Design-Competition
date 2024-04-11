using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hook : MonoBehaviour
{
    Rope rope;
    private void Start()
    {
        rope = transform.parent.GetComponent<Rope>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((collision.CompareTag("Chicken") && rope.CheckLength()) || (collision.CompareTag("Rabbit") && !rope.CheckLength()))
        {
            if (collision.CompareTag("Chicken")) MainAudioManager.AudioManagerInstance.PlaySFXScene("Chicken");
            else if (collision.CompareTag("Rabbit")) MainAudioManager.AudioManagerInstance.PlaySFXScene("Rabbit");
            Move move = collision.GetComponent<Move>();
#if UNITY_EDITOR
            if (move == null) Debug.LogError("" + collision.name + "have no Move Script");
#endif
            move.enabled = false;

            collision.transform.parent = transform;
            transform.parent.GetComponent<Rope>().GetState = State.Shorten;
            for (int i = 0; i < transform.childCount; i++) transform.GetChild(i).GetComponent<Collider2D>().enabled = false;
            transform.GetComponent<Collider2D>().enabled = false;
        }
    }
}
