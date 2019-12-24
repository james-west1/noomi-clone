using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerControl : MonoBehaviour
{

    public static PlayerControl instance;

    public GameObject leftArm, rightArm, torso, leftUpperLeg, rightUpperLeg, leftLowerLeg, rightLowerLeg, leftFoot, rightFoot, head, bar; // body parts + bar
    Rigidbody leftArmBody, rightArmBody, torsoBody, leftUpperLegBody, rightUpperLegBody, leftLowerLegBody, rightLowerLegBody, leftFootBody, rightFootBody, headBody; // rigidbodies
    HingeJoint leftArmHinge, rightArmHinge, leftShoulder, rightShoulder, leftHip, rightHip, leftKnee, rightKnee, leftAnkle, rightAnkle; // joints
    public JointSpring leftShoulderSpring, rightShoulderSpring, leftHipSpring, rightHipSpring, leftKneeSpring, rightKneeSpring, leftAnkleSpring, rightAnkleSpring; // spring component of joints
    public float strength; // "spring" constant, how hard the player tries to hit a body position
    public float damp; // how long it takes for the player to go from one pos to another, having a little bit makes it look more real
    public bool offBar = true; // have the joints been destroyed?
    bool alreadyTucked; // has the character tucked while off the bar yet?

    public bool shouldArch, shouldTuck, shouldLetGo;

    public float regrabCooldown;
    public float currentRegrabCooldown;

    public Transform rightHand, leftHand;

    // Start is called before the first frame update
    void Start()
    {
        //Time.timeScale = 0.1f;

        if (instance) {
            Destroy(this);
        } else {
            instance = this;
        }

        initJoints();
        initRigidbodies();
        initSprings();
    }

    // Update is called once per frame
    void Update()
    {
        if (currentRegrabCooldown > 0) {
            currentRegrabCooldown -= Time.deltaTime;

            if (currentRegrabCooldown < 0) {
                currentRegrabCooldown = 0;
            }
        }

        //Debug.Log(Vector3.Distance(rightHand.position, bar.transform.position));

        if (Vector3.Distance(rightHand.position, bar.transform.position) < 2f && currentRegrabCooldown == 0 && offBar) {

            rightArmBody.AddForce((bar.transform.position - rightArmBody.position) * 100);
            leftArmBody.AddForce((bar.transform.position - leftArmBody.position) * 100);

        }

        if (Vector3.Distance(rightHand.position, bar.transform.position) < 1.1f && currentRegrabCooldown == 0 && offBar) {
            reGrab();
        }

        if (Input.GetKeyDown(KeyCode.UpArrow) || shouldLetGo)
        {
            letGo();
            shouldLetGo = false;
        }
        // set up tuck, arch, let go, reset buttons
        if (Input.GetKey(KeyCode.LeftArrow) || shouldArch) {
            arch();
        } else if (Input.GetKey(KeyCode.Space) || shouldTuck) {
            tuck();
        } else if (Input.GetKeyDown(KeyCode.P)) {

            transform.position = new Vector3(0, 2.7f, 1.2f);

        } else if (alreadyTucked) // if the player has already tucked and is not on the bar, go to landing position
          {
            land();
        } else // otherwise go to default position
          {
            defaultPosition();
        }

        if (Input.GetKeyUp(KeyCode.LeftArrow)) {
            shouldArch = false;
        }
    }

    void initJoints() {
        // initialize all hinge joints
        leftArmHinge = leftArm.GetComponent<HingeJoint>();
        rightArmHinge = rightArm.GetComponent<HingeJoint>();
        // two hinge joints associated with torso
        leftShoulder = torso.GetComponents<HingeJoint>()[0];
        rightShoulder = torso.GetComponents<HingeJoint>()[1];
        leftHip = leftUpperLeg.GetComponent<HingeJoint>();
        rightHip = rightUpperLeg.GetComponent<HingeJoint>();
        leftKnee = leftLowerLeg.GetComponent<HingeJoint>();
        rightKnee = rightLowerLeg.GetComponent<HingeJoint>();
        leftAnkle = leftFoot.GetComponent<HingeJoint>();
        rightAnkle = rightFoot.GetComponent<HingeJoint>();
    }

    void initRigidbodies() {
        // initialize all rigidbodies
        leftArmBody = leftArm.GetComponent<Rigidbody>();
        rightArmBody = rightArm.GetComponent<Rigidbody>();

        leftUpperLegBody = leftUpperLeg.GetComponent<Rigidbody>();
        rightUpperLegBody = rightUpperLeg.GetComponent<Rigidbody>();

        leftLowerLegBody = leftLowerLeg.GetComponent<Rigidbody>();
        rightLowerLegBody = rightLowerLeg.GetComponent<Rigidbody>();

        torsoBody = torso.GetComponent<Rigidbody>();

        headBody = head.GetComponent<Rigidbody>();

        leftFootBody = leftFoot.GetComponent<Rigidbody>();
        rightFootBody = rightFoot.GetComponent<Rigidbody>();

        // set max angular velocity to the max value so it can flip at a normal speed

        torsoBody.maxAngularVelocity = float.MaxValue;

        leftArmBody.maxAngularVelocity = float.MaxValue;
        rightArmBody.maxAngularVelocity = float.MaxValue;

        rightLowerLegBody.maxAngularVelocity = float.MaxValue;
        leftLowerLegBody.maxAngularVelocity = float.MaxValue;

        headBody.maxAngularVelocity = float.MaxValue;

        leftUpperLegBody.maxAngularVelocity = float.MaxValue;
        rightUpperLegBody.maxAngularVelocity = float.MaxValue;

        leftFootBody.maxAngularVelocity = float.MaxValue;
        rightFootBody.maxAngularVelocity = float.MaxValue;
    }

    void initSprings() {
        // initialize springs, then set strength values
        leftShoulderSpring = leftShoulder.spring;
        rightShoulderSpring = rightShoulder.spring;

        leftHipSpring = leftHip.spring;
        rightHipSpring = rightHip.spring;

        leftShoulderSpring.spring = strength;
        rightShoulderSpring.spring = strength;

        leftHipSpring.spring = strength;
        rightHipSpring.spring = strength;

        // knees are stronger so they don't bounce around too much (i think this makes it more realistic?)
        leftKneeSpring = leftKnee.spring;
        rightKneeSpring = rightKnee.spring;

        leftKneeSpring.spring = strength * 4;
        rightKneeSpring.spring = strength * 4;

        leftAnkleSpring = leftAnkle.spring;
        rightAnkleSpring = rightAnkle.spring;

        leftAnkleSpring.spring = strength;
        rightAnkleSpring.spring = strength;

        // set damper values (all the same)

        leftShoulderSpring.damper = damp;
        rightShoulderSpring.damper = damp;
        leftHipSpring.damper = damp;
        rightHipSpring.damper = damp;
        leftKneeSpring.damper = damp;
        rightKneeSpring.damper = damp;
        leftAnkleSpring.damper = damp;
        rightAnkleSpring.damper = damp;
    }

    // body position methods tell the joints what angle they should go to, pretty much that simple (for now)

    void land()
    {
        leftShoulderSpring.targetPosition = 90;
        rightShoulderSpring.targetPosition = 90;
        leftShoulder.spring = leftShoulderSpring;
        rightShoulder.spring = rightShoulderSpring;

        leftHipSpring.targetPosition = 60;
        rightHipSpring.targetPosition = 60;
        leftHip.spring = leftHipSpring;
        rightHip.spring = rightHipSpring;

        leftKneeSpring.targetPosition = -60;
        rightKneeSpring.targetPosition = -60;
        leftKnee.spring = leftKneeSpring;
        rightKnee.spring = rightKneeSpring;

        leftAnkleSpring.targetPosition = 10;
        rightAnkleSpring.targetPosition = 10;
        leftAnkle.spring = leftAnkleSpring;
        rightAnkle.spring = rightAnkleSpring;
    }
    void defaultPosition()
    {
        leftShoulderSpring.targetPosition = 150;
        rightShoulderSpring.targetPosition = 150;
        leftShoulder.spring = leftShoulderSpring;
        rightShoulder.spring = rightShoulderSpring;

        leftHipSpring.targetPosition = 110;
        rightHipSpring.targetPosition = 110;
        leftHip.spring = leftHipSpring;
        rightHip.spring = rightHipSpring;

        leftKneeSpring.targetPosition = 0;
        rightKneeSpring.targetPosition = 0;
        leftKnee.spring = leftKneeSpring;
        rightKnee.spring = rightKneeSpring;

        leftAnkleSpring.targetPosition = 10;
        rightAnkleSpring.targetPosition = 10;
        leftAnkle.spring = leftAnkleSpring;
        rightAnkle.spring = rightAnkleSpring;
    }

    public void arch()
    {
        leftShoulderSpring.targetPosition = -20;
        rightShoulderSpring.targetPosition = -20;
        leftShoulder.spring = leftShoulderSpring;
        rightShoulder.spring = rightShoulderSpring;

        leftHipSpring.targetPosition = -20;
        rightHipSpring.targetPosition = -20;
        leftHip.spring = leftHipSpring;
        rightHip.spring = rightHipSpring;

        leftKneeSpring.targetPosition = -40;
        rightKneeSpring.targetPosition = -40;
        leftKnee.spring = leftKneeSpring;
        rightKnee.spring = rightKneeSpring;

        leftAnkleSpring.targetPosition = -45;
        rightAnkleSpring.targetPosition = -45;
        leftAnkle.spring = leftAnkleSpring;
        rightAnkle.spring = rightAnkleSpring;
    }

    public void tuck()
    {
        leftShoulderSpring.targetPosition = 140;
        rightShoulderSpring.targetPosition = 140;
        leftShoulder.spring = leftShoulderSpring;
        rightShoulder.spring = rightShoulderSpring;

        leftHipSpring.targetPosition = 150;
        rightHipSpring.targetPosition = 150;
        leftHip.spring = leftHipSpring;
        rightHip.spring = rightHipSpring;
        if (offBar) // make hips stronger so the dude can actually pull the tuck, and set alreadyTucked to true so that when you untuck, he knows to hit the land position
        {
            leftHipSpring.spring = strength * 3;
            rightHipSpring.spring = strength * 3;
            alreadyTucked = true;
        }


        leftKneeSpring.targetPosition = -90;
        rightKneeSpring.targetPosition = -90;
        leftKnee.spring = leftKneeSpring;
        rightKnee.spring = rightKneeSpring;

        leftAnkleSpring.targetPosition = 10;
        rightAnkleSpring.targetPosition = 10;
        leftAnkle.spring = leftAnkleSpring;
        rightAnkle.spring = rightAnkleSpring;
    }

    public void letGo() // destroys joints between arms and bar
    {
        Destroy(leftArmHinge);
        Destroy(rightArmHinge);
        offBar = true; // not on the bar
        currentRegrabCooldown = regrabCooldown;
    }

    public void reGrab() {

        if (currentRegrabCooldown == 0) {

            leftHand.transform.position = bar.transform.position;
            rightHand.transform.position = bar.transform.position;

            leftArmHinge = leftArm.AddComponent<HingeJoint>();
            leftArmHinge.connectedBody = bar.GetComponent<Rigidbody>();
            leftArmHinge.axis = bar.gameObject.transform.up;
            leftArmHinge.autoConfigureConnectedAnchor = true;
            leftArmHinge.anchor = new Vector3(0, 1.1f, 0);

            rightArmHinge = rightArm.AddComponent<HingeJoint>();
            rightArmHinge.connectedBody = bar.GetComponent<Rigidbody>();
            rightArmHinge.axis = bar.gameObject.transform.up;
            rightArmHinge.autoConfigureConnectedAnchor = true;
            rightArmHinge.anchor = new Vector3(0, 1.1f, 0);

            offBar = false;
        }
    }
}

