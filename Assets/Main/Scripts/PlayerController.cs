using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField, Header("playerのリジットボディ")] Rigidbody _playerRB;
    [SerializeField, Header("playerの移動速度")] float _moveSpeed = 1.0f;
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
        _playerRB.AddForce(new Vector3(_moveInputValue.x, 0f, _moveInputValue.y) * _moveSpeed, ForceMode.Force);
    }

    private void OnDestroy()
    {
        _inputActions.Dispose(); // 忘れずに…
    }
}
