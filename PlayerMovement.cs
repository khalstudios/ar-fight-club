using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] public FixedJoystick _joystick;

    // [SerializeField] private AnimatorController _animatorController;

    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _rotateSpeed;
    [SerializeField] private Animator animator;

    private Rigidbody _rigidbody;
    PhotonView photonView;
    private Vector3 _moveVector;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _joystick = GetComponentInChildren<FixedJoystick>();
        // Setup Animator, add AnimationEvents script.
		animator = GetComponentInChildren<Animator>();
        photonView = GetComponent<PhotonView>();
    }

    private void Update()
    {
        if (photonView.IsMine)
        {
            Move();
        }
    }

    private void Move()
    {
        _moveVector = Vector3.zero;
        _moveVector.x = _joystick.Horizontal * _moveSpeed * Time.deltaTime;
        _moveVector.z = _joystick.Vertical * _moveSpeed * Time.deltaTime;

        if(_joystick.Direction.magnitude > 0f)
        {
            Vector3 direction = Vector3.RotateTowards(transform.forward, _moveVector, _rotateSpeed * Time.deltaTime, 0.0f);
            transform.rotation = Quaternion.LookRotation(direction);

            animator.SetBool("Moving", true);    
            animator.SetFloat("Velocity", _joystick.Direction.magnitude);
            
        }

        else if(_joystick.Horizontal == 0 && _joystick.Vertical == 0)
        {
            animator.SetBool("Moving", false);
            animator.SetFloat("Velocity", 0.0f);
        }

        //_rigidbody.MovePosition(_rigidbody.position + _moveVector);
        _rigidbody.MovePosition(_rigidbody.position + _moveVector * _joystick.Direction.magnitude);
    }

    

    public void LightAttack() {
        animator.SetTrigger("Light Attack");
    }

    public void HeavyAttack() {
        animator.SetTrigger("Heavy Attack");
    }
}