using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manipulation : MonoBehaviour {

    public SteamVR_TrackedObject ob;
    private SteamVR_Controller.Device input;

    private bool holding;
    private bool colliding;
    private GameObject heldObject;
    private GameObject selectedObject;

    // Use this for initialization
    void Start () {
        holding = false;
        colliding = false;
        input = SteamVR_Controller.Input((int)ob.index);
    }
	
	// Update is called once per frame
	void Update () {
        if (input.GetPressDown(Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger)) {
            if (!holding && selectedObject != null) {
                gameObject.AddComponent<FixedJoint>();
                gameObject.GetComponent<FixedJoint>().breakForce = 5000;
                heldObject = selectedObject;
                GetComponent<FixedJoint>().connectedBody = heldObject.GetComponent<Rigidbody>();
                holding = true;
            }
            else if (holding) {
                heldObject.GetComponent<Rigidbody>().velocity = input.velocity *3f;
                Destroy(GetComponent<FixedJoint>());
                holding = false;
                heldObject = null;
            }
        }
        if (input.GetPressDown(Valve.VR.EVRButtonId.k_EButton_Grip)) {
            if (holding && heldObject.tag == "Fire") {
                ParticleSystem p = heldObject.GetComponentInChildren<ParticleSystem>();
                if (p.IsAlive()) {
                    p.Clear();
                    p.Stop();
                }
                else {
                    p.Play();
                }
            }
        }
    }

    void OnTriggerEnter(Collider other) {
        colliding = true;
        if (other.gameObject.tag == "pickup" || other.gameObject.tag == "Fire") {
            selectedObject = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other) {
        colliding = false;
        selectedObject = null;
    }

    private void OnJointBreak(float breakForce) {
        heldObject.GetComponent<Rigidbody>().velocity = input.velocity * 3f;
        GetComponent<FixedJoint>().connectedBody = null;
        heldObject = null;
        holding = false;
    }
}
