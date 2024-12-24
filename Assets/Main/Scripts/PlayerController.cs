using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField, Header("player�̃��W�b�g�{�f�B")] Rigidbody _playerRB;
    [SerializeField, Header("player�̈ړ����x")] float _moveSpeed = 1.0f;
    [SerializeField] InputSystem_Actions _inputActions;
    Vector2 _moveInputValue;

    void Start()
    {
        _inputActions = new InputSystem_Actions();

        _inputActions.Player.Move.started += OnMove;
        _inputActions.Player.Move.performed += OnMove;
        _inputActions.Player.Move.canceled += OnMove;

        _inputActions.Enable(); // �Y�ꂸ�Ɂc
    }

    /// <summary>
    /// ���͂�Vector2�ɕϊ����Ď󂯎�郁�\�b�h
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
        _inputActions.Dispose(); // �Y�ꂸ�Ɂc
    }
}
