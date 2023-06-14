using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MySynchronizationScript : MonoBehaviour, IPunObservable
{

    Rigidbody rb;
    PhotonView photonView;

    Vector3 networkedPosition;
    Quaternion networkedRotation;

    public bool synchronizeVelocity = true;
    public bool synchronizeAngularVelocity = true;
    public bool isTeleportEnabled = true;
    public float teleportIfDistanceGreaterThan = 1.0f;

    private float distance;
    private float angle;

    private Animator animator;
    private HealthSystem healthSystem;

    // Animation state variables
    private bool isRunning = false;
    private float speed = 0f;
    private float health = 100f;
    private bool lightAttack = false;
    private bool heavyAttack = false;
    private bool takeDamage = false;

    private GameObject battleArenaGameobject;


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        photonView = GetComponent<PhotonView>();
        healthSystem = GetComponent<HealthSystem>();

        networkedPosition = new Vector3();
        networkedRotation = new Quaternion();

        battleArenaGameobject = GameObject.Find("BattleArena");

        animator = GetComponentInChildren<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine)
        {
            // Update animation states based on player input
            bool isRunningNow = animator.GetBool("Moving");
            float speedNow = animator.GetFloat("Velocity");
            bool lightAttackNow = animator.GetBool("Light Attack");
            bool heavyAttackNow = animator.GetBool("Heavy Attack");
            bool takeDamageNow = animator.GetBool("Take Damage");

            if (isRunningNow != isRunning)
            {
                isRunning = isRunningNow;
                photonView.RPC("UpdateRunningState", RpcTarget.All, isRunning);
            }

            if (speedNow != speed)
            {
                speed = speedNow;
                photonView.RPC("UpdateSpeed", RpcTarget.All, speed);
            }

            if (lightAttackNow)
            {
                photonView.RPC("TriggerLightAttack", RpcTarget.All);
            }

            if (heavyAttackNow)
            {
                photonView.RPC("TriggerHeavyAttack", RpcTarget.All);
            }

            if (takeDamageNow)
            {
                //photonView.RPC("TriggerDamage", RpcTarget.All);
            }
        }
    }

    private void FixedUpdate()
    {

        if (!photonView.IsMine)
        {
            rb.position = Vector3.MoveTowards(rb.position, networkedPosition, distance*(1.0f/ PhotonNetwork.SerializationRate));
            rb.rotation = Quaternion.RotateTowards(rb.rotation, networkedRotation, angle*(1.0f/ PhotonNetwork.SerializationRate));

            //rb.position = networkedPosition;
            //rb.rotation = networkedRotation;
        }

    }

    [PunRPC]
    private void UpdateRunningState(bool state)
    {
        isRunning = state;
    }

    [PunRPC]
    private void UpdateSpeed(float value)
    {
        speed = value;
    }

    [PunRPC]
    private void TriggerLightAttack()
    {
        animator.SetTrigger("Light Attack");
    }

    [PunRPC]
    private void TriggerHeavyAttack()
    {
        animator.SetTrigger("Heavy Attack");
    }

    [PunRPC]
    private void TriggerDamage()
    {
        animator.SetTrigger("Take Damage");
    }


    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            //Then, photonView is mine and I am the one who controls this player.
            //should send position, velocity etc. data to the other players
            stream.SendNext(rb.position-battleArenaGameobject.transform.position);
            stream.SendNext(rb.rotation);
            stream.SendNext(isRunning);
            stream.SendNext(speed);
            stream.SendNext(health);
            stream.SendNext(lightAttack);
            stream.SendNext(heavyAttack);
            stream.SendNext(takeDamage);

            if (synchronizeVelocity)
            {
                stream.SendNext(rb.velocity);
            }

            if (synchronizeAngularVelocity)
            {
                stream.SendNext(rb.angularVelocity);
            }
        }
        else
        {
            //Called on my player gameobject that exists in remote player's game

            networkedPosition = (Vector3)stream.ReceiveNext()+battleArenaGameobject.transform.position;
            networkedRotation = (Quaternion)stream.ReceiveNext();

            isRunning = (bool)stream.ReceiveNext();
            speed = (float)stream.ReceiveNext();
            health = (float)stream.ReceiveNext();

            lightAttack = (bool)stream.ReceiveNext();
            heavyAttack = (bool)stream.ReceiveNext();
            takeDamage = (bool)stream.ReceiveNext();

            animator.SetBool("Moving", isRunning);
            animator.SetFloat("Velocity", speed);

            //healthSystem.health = health;

            if (lightAttack)
            {
                animator.SetTrigger("Light Attack");
            }

            if (heavyAttack)
            {
                animator.SetTrigger("Heavy Attack");
            }

            if (takeDamage) {
                animator.SetTrigger("Take Damage");
            }

            // if (isTeleportEnabled)
            // {
            //     if (Vector3.Distance(rb.position, networkedPosition) > teleportIfDistanceGreaterThan)
            //     {
            //         rb.position = networkedPosition;

            //     }
            // }

            if (synchronizeVelocity || synchronizeAngularVelocity)
            {
                float lag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));

                if (synchronizeVelocity)
                {
                    rb.velocity = (Vector3)stream.ReceiveNext();

                    networkedPosition += rb.velocity * lag;

                    distance = Vector3.Distance(rb.position, networkedPosition);
                }

                if (synchronizeAngularVelocity)
                {
                    rb.angularVelocity = (Vector3)stream.ReceiveNext();

                    networkedRotation = Quaternion.Euler(rb.angularVelocity*lag)*networkedRotation;

                    angle = Quaternion.Angle(rb.rotation, networkedRotation);
                }
            }
        }
    }
    
}