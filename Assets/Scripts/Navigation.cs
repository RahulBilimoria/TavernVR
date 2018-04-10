using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Navigation : MonoBehaviour {

    public SteamVR_TrackedObject ob;
    private SteamVR_Controller.Device input;

    public ParticleSystem part;

    private Vector3 point;

    private bool nav;
    private bool move;

    // Use this for initialization
    void Start () {
        nav = false;
        move = false;
        point = Vector3.zero;
        input = SteamVR_Controller.Input((int)ob.index);
    }

    // Update is called once per frame
    void Update() {

        if (input.GetPressDown(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad)) {
            nav = true;
            part.Play();
        }
        else {
            GetComponent<LineRenderer>().SetPosition(0, transform.position);
            GetComponent<LineRenderer>().SetPosition(1, transform.position);
        }

        if (input.GetPressUp(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad)) {
            nav = false;
            if (!move) {
                part.Clear();
                part.Stop();
            }
        }

        if (nav && !move) {
            GetComponent<LineRenderer>().SetPosition(0, transform.position);
            GetComponent<LineRenderer>().SetPosition(1, transform.position + transform.forward * 10f);
            RaycastHit collide;
            bool hit = Physics.Raycast(gameObject.transform.position, gameObject.transform.forward, out collide, 1000);
            part.transform.position = new Vector3(10, 10, 10);
            if (hit) {
                if (collide.collider.tag.Equals("floor")) {
                    part.transform.position = collide.point;
                    GetComponent<LineRenderer>().SetPosition(1, collide.point);
                    if (input.GetPressDown(Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger)) {
                        move = true;
                        point = collide.point;
                        SteamVR_Fade.View(Color.black, 3f);
                        Invoke("movePlayer", 5f);
                    }
                }
            }
            else {
                
            }
        }
    }

    void movePlayer() {
        part.Clear();
        part.Stop();
        Vector3 up = new Vector3(0, 1, 0);
        Vector3 head = gameObject.transform.parent.GetChild(2).localPosition;
        head.y = -gameObject.transform.parent.position.y;
        gameObject.transform.parent.position = point - head;
        point = Vector3.zero;
        SteamVR_Fade.View(new Color(0, 0, 0, 0), 3f);
        move = false;
    }
}
