using UnityEngine;
using System.Collections;

[RequireComponent (typeof(BoxCollider))]
public class Pickup : MonoBehaviour {

	void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player"))
            gameObject.SetActive(false);
    }

    void Update()
    {
        transform.Rotate(Vector3.up * 80 * Time.deltaTime);
    }
}
