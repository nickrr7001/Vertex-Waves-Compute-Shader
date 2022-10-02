using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipController : MonoBehaviour
{
    public float forwardAcceleration;
    public float maxVelocity;
    public Vector3 currentVelocity;
    public bool ancored;
    public float maxAngularVelocity;
    public float angleChangeSpeed;
    public float angularVelcoity;
    public float angularDeceleration;
    public float xRotation = 0;
    public Vector3 decel;
    public int framesColDetect = 10;
    private int curFrames = 0;
    public Transform[] collisionPoints;
    private CollisionDetector collisionDetector;
    public Waves_compute w;
    private void Start()
    {
        collisionDetector = new CollisionDetector(collisionPoints[0],collisionPoints[1],transform,w);
    }
    private void Update()
    {
        curFrames++;
        if (curFrames == framesColDetect)
        {
            curFrames = 0;

        }
        float velocityToAdd = ancored ? -forwardAcceleration : forwardAcceleration;
        currentVelocity = transform.forward * velocityToAdd * Time.deltaTime;
    /*    currentVelocity = new Vector3(
            Mathf.Clamp(currentVelocity.x, 0f, maxVelocity),
            Mathf.Clamp(currentVelocity.y, 0f, maxVelocity),
            Mathf.Clamp(currentVelocity.z, 0f, maxVelocity)
            ); */
        float angleToAdd = Input.GetAxis("Horizontal") * Time.deltaTime * angleChangeSpeed;
        transform.position += currentVelocity * Time.deltaTime;
        angularVelcoity += angleToAdd;
        angularVelcoity += angularDeceleration * Time.deltaTime * ((angularVelcoity > 0)?-1:1);
        xRotation += (angularVelcoity);
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, xRotation, transform.rotation.eulerAngles.z);
        collisionDetector.collisionOperation();
    }
}
public class CollisionDetector
{
    private Transform collisionPoint0;
    private Transform collisionPoint1;
    private Transform mainTransform;
    private Waves_compute w;
    float lastAngle = 0;
    Vector3 lastRight = Vector3.zero;
    public CollisionDetector (Transform collisionP0, Transform collisionP1,Transform mt, Waves_compute w)
    {
        collisionPoint0 = collisionP0;
        collisionPoint1 = collisionP1;
        mainTransform = mt;
        this.w = w;
    }
    public void collisionOperation()
    {
        float cp0y = w.getSinatX(collisionPoint0.position.x) + w.getCoseatZ(collisionPoint0.position.z);
        float cp1y = w.getSinatX(collisionPoint1.position.x) + w.getCoseatZ(collisionPoint1.position.z);
        Vector3 mtp = mainTransform.position;
        mtp.y = 5 +( (cp0y + cp1y) / 2);
        mainTransform.position = mtp;
    }
}
