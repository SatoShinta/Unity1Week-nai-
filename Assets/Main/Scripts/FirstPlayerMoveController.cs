using UnityEngine;
using UnityEngine.InputSystem;

public class FirstPlayerMoveController : MonoBehaviour
{
    [SerializeField] Rigidbody _playerRB;
    [SerializeField] PlayerController _playerController;
    [SerializeField] FirstPlayerMoveController _firstMove;
    [SerializeField] InputSystem_Actions _inputActions;
    Vector2 _moveInputValue;
    [SerializeField, Header("現在のスピード")] float _nowSpeed = 0f;
    [SerializeField, Header("押すたびに増えるスピード")] float _increaseSpeed = 0.5f;
    [SerializeField, Header("押すたびに減るスピード")] float _decrecaseSpeed = 0.5f;
    [SerializeField, Header("車に乗ったかどうかのフラグ")] bool isGetInACar = false;
    [SerializeField] float elapsedtime = 0f;
    private void Start()
    {
        _playerController.enabled = false;
        _inputActions = new InputSystem_Actions();

        _inputActions.Player.Move.started += OnMove;
        _inputActions.Player.Move.performed += OnMove;
        _inputActions.Player.Move.canceled += OnMove;

        _inputActions.Enable();

    }


    private void Update()
    {
        if (isGetInACar)
        {
            _playerController.enabled = true;
            _firstMove.enabled = false;
        }
    }

    private void FixedUpdate()
    {
        SpeedUp();
        MovePlayer();
    }

    /// <summary>
    /// スペースキーを連打しているときは徐々にスピードが増す
    /// </summary>
    void SpeedUp()
    {
        elapsedtime += Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            _nowSpeed += _increaseSpeed;
            elapsedtime = 0f;
        }
        else
        {
            if (elapsedtime >= 2f && _nowSpeed > 0f)
            {
                _nowSpeed -= _decrecaseSpeed;
            }
            else if (_nowSpeed <= 0)
            {
                _nowSpeed = 0;
                elapsedtime = 0f;
            }
        }

    }

    /// <summary>
    /// プレイヤーの移動を行う処理
    /// </summary>
    void MovePlayer()
    {
        // 入力方向に基づいて移動方向を計算
        Vector3 moveDirection = new Vector3(_moveInputValue.x, 0f, _moveInputValue.y).normalized;

        // 移動方向がゼロでない場合、プレイヤーをその方向に向ける
        if (moveDirection != Vector3.zero)
        {
            // プレイヤーの向きを移動方向に合わせる
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            _playerRB.MoveRotation(Quaternion.Slerp(_playerRB.rotation, targetRotation, Time.deltaTime * 2f)); // 5f は回転速度
        }

        Vector3 foward = transform.forward * _nowSpeed;
        // 力を加えて移動
        _playerRB.AddForce(foward, ForceMode.Force);
    }


    private void OnMove(InputAction.CallbackContext context)
    {
        _moveInputValue = context.ReadValue<Vector2>();
    }

    private void OnDestroy()
    {
        _inputActions.Dispose();
    }
}
