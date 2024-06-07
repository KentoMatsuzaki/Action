using UnityEngine;
using UnityEngine.InputSystem;
using Unity.TinyCharacterController.Control;

/// <summary>プレイヤー制御クラス</summary>
public class PlayerController : MonoBehaviour
{
    /// <summary>プレイヤーのアニメーター</summary>
    Animator _animator;

    /// <summary>プレイヤーの移動制御</summary>
    MoveControl _moveControl;

    /// <summary>通常時の速度</summary>
    public float _normalSpeed = 1.2f;

    /// <summary>スプリント時の速度</summary>
    public float _sprintSpeed = 7.5f;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _moveControl = GetComponent<MoveControl>();
    }
    private void Update()
    {
        // キャラクターのスピードを更新
        _animator.SetFloat("Speed", _moveControl.CurrentSpeed);
    }

    /// <summary>PlayerInputから呼ばれるイベント</summary>
    /// <param name="context">入力時に呼び出されるコールバック関数の状態</param>
    public void OnMove(InputAction.CallbackContext context)
    {
        // Moveアクションが押された場合
        if (context.performed)
        {
            _moveControl.Move(context.ReadValue<Vector2>());
        }
        // Moveアクションがリリースされた場合
        else if (context.canceled)
        {
            _moveControl.Move(Vector2.zero);
        }
    }

    /// <summary>PlayerInputから呼ばれるイベント</summary>
    /// <param name="context">入力時に呼び出されるコールバック関数の状態</param>
    public void OnSprint(InputAction.CallbackContext context)
    {
        // スプリントアクションが押された場合
        if (context.performed)
        {
            _moveControl.MoveSpeed = _sprintSpeed;
            Debug.Log("Sprint is Pressed.");
        }
        // スプリントアクションがリリースされた場合
        else if (context.canceled)
        {
            _moveControl.MoveSpeed = _normalSpeed;
            Debug.Log("Sprint is Released.");
        }
    }
}
