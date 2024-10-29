using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    private Animator _animator;
    private PlayerController _playerController;

    private static readonly int IsWalking = Animator.StringToHash("isWalking");
    private static readonly int Jump = Animator.StringToHash("Jump");

     void Awake()
    {
        _animator = GetComponent<Animator>();
        _playerController = GetComponent<PlayerController>();
    }

    private void Start()
    {
        if (_playerController != null)
        {
            _playerController.OnMoveEvent += OnMove;
            _playerController.OnJumpEvent += OnJump;
        }
    }

    private void OnMove(Vector2 movementInput)
    {
        bool isWalking = movementInput.sqrMagnitude > 0.01f;
        _animator.SetBool(IsWalking, isWalking);
    }

    private void OnJump()
    {
        _animator.SetTrigger(Jump);
    }
}
