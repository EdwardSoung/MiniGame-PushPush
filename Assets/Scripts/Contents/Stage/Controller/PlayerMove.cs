using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] private Transform _playerObj;
    [SerializeField] private CharacterController _characterController;

    private Vector3 _moveDir;

    public void Jump()
    {
        if (_characterController.isGrounded == true)
        {
            _moveDir.y = Constants.JUMP_FORCE;
        }
    }

    public void Move(float horizontal, float vertical, float speed)
    {
        var dir = (_playerObj.transform.forward * vertical) + (_playerObj.transform.right * horizontal);
        _moveDir.x = dir.x;
        _moveDir.z = dir.z;
        if (_characterController.isGrounded == false)
        {
            _moveDir.y += Physics.gravity.y * Time.deltaTime;
        }
        _characterController.Move(_moveDir * speed * Time.deltaTime);
    }

    public void Rotate(float rotate)
    {
        _playerObj.transform.Rotate(Vector3.up * Constants.CAM_TURN_SPEED * Time.deltaTime * rotate);
    }
}
