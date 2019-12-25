using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarController : MonoBehaviour {

    //public CapsuleCollider[] colliders;

    public CapsuleCollider triggerCollider;

    // Start is called before the first frame update
    void Start() {
       // colliders = GetComponents<CapsuleCollider>();
    }

    // Update is called once per frame
    void Update() {

    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Hand") && PlayerControl.instance.offBar) {
            other.GetComponent<Rigidbody>().MovePosition(new Vector3(this.transform.position.x, this.transform.position.y, other.gameObject.transform.position.z) + other.gameObject.transform.up); //triggerCollider.ClosestPoint(other.transform.position));

            Time.timeScale = 0.1f;

            PlayerControl.instance.reGrab();

            Debug.Log("moved position");
        } else {
            Debug.Log("Not hands");
        }
    }
}
