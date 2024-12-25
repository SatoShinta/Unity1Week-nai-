using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField, Header("playerのリジットボディ")] Rigidbody _playerRB;
    [SerializeField, Header("playerの移動速度")] float _moveSpeed = 1.0f;
    [SerializeField,Header("ブレーキの利き加減")] float _blakePawer = 1.0f;
    [SerializeField] InputSystem_Actions _inputActions;
    Vector2 _moveInputValue;

    void Start()
    {
        _inputActions = new InputSystem_Actions();

        _inputActions.Player.Move.started += OnMove;
        _inputActions.Player.Move.performed += OnMove;
        _inputActions.Player.Move.canceled += OnMove;

        _inputActions.Enable(); // 忘れずに…
    }

    /// <summary>
    /// 入力をVector2に変換して受け取るメソッド
    /// </summary>
    /// <param name="context"></param>
    private void OnMove(InputAction.CallbackContext context)
    {
        _moveInputValue = context.ReadValue<Vector2>();
    }


    private void FixedUpdate()
    {
        Vector3 playerVector = new Vector3(_moveInputValue.x, 0f, _moveInputValue.y).normalized;
        _playerRB.AddForce(playerVector * _moveSpeed, ForceMode.Force);

        if(Input.GetKey(KeyCode.S))
        {
            Vector3 brakePower = _playerRB.linearVelocity.normalized * _blakePawer;
            _playerRB.AddForce(brakePower, ForceMode.Force);
        }

        if(playerVector.sqrMagnitude > 0.1f)
        {
            transform.forward = Vector3.Slerp(transform.forward, playerVector, Time.fixedDeltaTime * 1f);
        }
    }

    private void OnDestroy()
    {
        _inputActions.Dispose(); // 忘れずに…
    }
}
