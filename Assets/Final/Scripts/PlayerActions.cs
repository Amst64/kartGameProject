using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerActions : MonoBehaviour
{
    float inputX;
    float inputY;
    bool inputReload;
    bool inputQuit;

    [SerializeField]
    Transform gfx;

    [SerializeField]
    Transform sphere;

    [SerializeField]
    Rigidbody rbSphere;

    float acceleration;

    float currentSpeed;

    float speed;

    float speedInspector;

    [SerializeField]
    float speedRotation;

    [SerializeField]
    Transform tire1;

    [SerializeField]
    Transform tire2;

    [SerializeField]
    Transform tire3;

    [SerializeField]
    Transform tire4;

    [SerializeField]
    Transform leftTireRotator;

    [SerializeField]
    Transform rightTireRotator;

    [SerializeField]
    Transform tireLeftRotationDirection;

    [SerializeField]
    Transform tireRightRotationDirection;

    float rbSphereDrag = 0.5f;

    [SerializeField]
    ParticleSystem smokeParticle;

    [SerializeField]
    ParticleSystem smokeParticle2;

    [SerializeField]
    ParticleSystem turboParticle;

    [SerializeField]
    ParticleSystem turboParticle2;

    Vector3 velocity;
    Vector3 lastVelocity;

    Vector3 gravity = new Vector3(0, -9.81f, 0);

    float timeWhenSideSlip;

    private void Start()
    {
        rbSphere.drag = rbSphereDrag;
        acceleration = 15;
        lastVelocity = rbSphere.velocity;
    }
    // Update is called once per frame
    void Update()
    {
        int layerMask = LayerMask.GetMask("boost");
        
        speedInspector = rbSphere.velocity.magnitude; // pour tourner plus ou moins vite en fonction de la vitesse, ligne avec la grande flèche (vers 200)

        Vector3 testVelocity = transform.InverseTransformDirection(rbSphere.velocity);

        inputX = Input.GetAxisRaw("Horizontal");
        inputY = Input.GetAxisRaw("Vertical");
        inputReload = Input.GetButtonDown("Jump");
        inputQuit = Input.GetButtonDown("Cancel");

        float invert = -inputX;

        if(inputY != 0)
        {
            rbSphereDrag = 0.5f;
            speed = acceleration * inputY;
        }
        else
        {
            rbSphereDrag = 2;
        }
        rbSphere.drag = rbSphereDrag;

        if(inputY < 0)
        {
            inputX = invert;
        }

        if (inputReload)
        {
            SceneManager.LoadScene("Scene_Canyon");
        }

        if (inputQuit)
        {
            SceneManager.LoadScene("Scene_Menus");

        }

        if (velocity.x < 0.5f || velocity.x > -0.5f)
        {
            timeWhenSideSlip += Time.deltaTime;
        }



        bool smokeCond = (velocity.x >= 0.5f || velocity.x <= -0.5f);

        if (smokeCond)
        {
            timeWhenSideSlip = 0;
            if (!smokeParticle.isPlaying)
            {
                //active smoke particle
                smokeParticle.Play();
                smokeParticle2.Play();
            }
        }
        else if(!smokeCond && timeWhenSideSlip >= 0.65f)
        {
            if (smokeParticle.isPlaying)
            {
                smokeParticle.Stop();
                smokeParticle2.Stop();
                //smokeParticle.Clear();
            }
        }

        gfx.position = sphere.position;

        currentSpeed = Mathf.SmoothStep(currentSpeed, speed, Time.deltaTime * 12f);
        speed = 0;
        RaycastHit hit;

        //pour boost lorsqu'on est sur une plaque de boost
        if (Physics.Raycast(gfx.position, Vector3.down, out hit)){
            if(hit.transform.gameObject.layer == LayerMask.NameToLayer("boost"))
            {
                acceleration = 80;
                if (!turboParticle.isPlaying)
                {
                    turboParticle.Play();
                    turboParticle2.Play();
                }
                
            }
            else
            {
                acceleration = 15;
                if (turboParticle.isPlaying)
                {
                    turboParticle.Stop();
                    turboParticle2.Stop();
                }
                
            }
        }
        
        
    }

    private void FixedUpdate()
    {
        Physics.gravity = gravity;
        velocity = (rbSphere.velocity - lastVelocity)*3; //permet de savoir quand la voiture "glisse" sur le côté
        lastVelocity = rbSphere.velocity;

        Quaternion currOrientation = gfx.rotation;
        rbSphere.AddForce(gfx.forward * currentSpeed,ForceMode.Acceleration);

        RaycastHit hit;
        
        bool raycast = Physics.Raycast(gfx.position, Vector3.down, out hit, 4.0f);

        //modification de la gravité lorsque la voiture est en l'air, plus réaliste
        if (raycast)
        {
            gravity = new Vector3(0, -9.81f, 0);
        }
        else
        {
            gravity.y -= 0.2f;
        }
        //

        Quaternion alignUpWithTerrainNormalQuaternion = Quaternion.FromToRotation(gfx.transform.up, hit.normal); //< TROU 5 > // the quaternion that aims at aligning the UP vector of the tire with the terrain normal (newNormal)
        Quaternion newOrientation = Quaternion.Slerp(currOrientation, alignUpWithTerrainNormalQuaternion * currOrientation, Time.fixedDeltaTime * 4f);

        newOrientation = Quaternion.AngleAxis(Time.deltaTime * speedInspector * inputX * 6.428f, transform.up) * newOrientation; // <<<<<<<<<<<<<<<<<<<<<<<<<--------------------------------------------------------------------



        gfx.rotation = newOrientation ;                                                                        // 6.428 car la vitesse max est 14(on regarde le speedInspector)
                                                                                                               // et 90 la vitesse de rotation max, 90/14 = 5.714f 
                                                                                                               // pour que le joueur ne tourne pas à une vitesse constante
                                                                                                               // depend de la vitesse de la voiture
        //La suite pour la rotation des pneus
        tire1.rotation = Quaternion.AngleAxis(currentSpeed, tire1.right) * tire1.rotation;
        tire2.rotation = Quaternion.AngleAxis(currentSpeed, tire2.right) * tire2.rotation;
        tire3.rotation = Quaternion.AngleAxis(currentSpeed, tire3.right) * tire3.rotation;
        tire4.rotation = Quaternion.AngleAxis(currentSpeed, tire4.right) * tire4.rotation;

        if (inputX < 0 && inputY == 0)
        {
            leftTireRotator.rotation = Quaternion.FromToRotation(leftTireRotator.forward, tireLeftRotationDirection.forward) * leftTireRotator.rotation;
            rightTireRotator.rotation = Quaternion.FromToRotation(rightTireRotator.forward, tireLeftRotationDirection.forward) * rightTireRotator.rotation;
        }
        else if (inputX > 0 && inputY == 0)
        {
            leftTireRotator.rotation = Quaternion.FromToRotation(leftTireRotator.forward, tireRightRotationDirection.forward) * leftTireRotator.rotation;
            rightTireRotator.rotation = Quaternion.FromToRotation(rightTireRotator.forward, tireRightRotationDirection.forward) * rightTireRotator.rotation;
        }
        else if (inputX < 0 && inputY > 0)
        {
            leftTireRotator.rotation = Quaternion.FromToRotation(leftTireRotator.forward, tireLeftRotationDirection.forward) * leftTireRotator.rotation;
            rightTireRotator.rotation = Quaternion.FromToRotation(rightTireRotator.forward, tireLeftRotationDirection.forward) * rightTireRotator.rotation;
        }
        else if (inputX > 0 && inputY > 0)
        {
            leftTireRotator.rotation = Quaternion.FromToRotation(leftTireRotator.forward, tireRightRotationDirection.forward) * leftTireRotator.rotation;
            rightTireRotator.rotation = Quaternion.FromToRotation(rightTireRotator.forward, tireRightRotationDirection.forward) * rightTireRotator.rotation;
        }
        else if (inputX < 0 && inputY < 0)
        {
            leftTireRotator.rotation = Quaternion.FromToRotation(leftTireRotator.forward, tireRightRotationDirection.forward) * leftTireRotator.rotation;
            rightTireRotator.rotation = Quaternion.FromToRotation(rightTireRotator.forward, tireRightRotationDirection.forward) * rightTireRotator.rotation;
        }
        else if (inputX > 0 && inputY < 0)
        {
            leftTireRotator.rotation = Quaternion.FromToRotation(leftTireRotator.forward, tireLeftRotationDirection.forward) * leftTireRotator.rotation;
            rightTireRotator.rotation = Quaternion.FromToRotation(rightTireRotator.forward, tireLeftRotationDirection.forward) * rightTireRotator.rotation;
        }
        else
        {
            leftTireRotator.rotation = Quaternion.FromToRotation(leftTireRotator.forward, gfx.forward) * leftTireRotator.rotation;
            rightTireRotator.rotation = Quaternion.FromToRotation(rightTireRotator.forward, gfx.forward) * rightTireRotator.rotation;
        }
    }

}
