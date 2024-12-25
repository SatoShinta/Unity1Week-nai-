using UnityEngine;
using UnityEngine.InputSystem;

public class FirstPlayerMoveController : MonoBehaviour
{
    [SerializeField] Rigidbody _playerRB;
    [SerializeField] PlayerController _playerController;
    [SerializeField] FirstPlayerMoveController _firstMove;
    [SerializeField] InputSystem_Actions _inputActions;
    Vector2 _moveInputValue;
    [SerializeField, Header("���݂̃X�s�[�h")] float _nowSpeed = 0f;
    [SerializeField, Header("�������тɑ�����X�s�[�h")] float _increaseSpeed = 0.5f;
    [SerializeField, Header("�������тɌ���X�s�[�h")] float _decrecaseSpeed = 0.5f;
    [SerializeField, Header("�Ԃɏ�������ǂ����̃t���O")] bool isGetInACar = false;
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
    /// �X�y�[�X�L�[��A�ł��Ă���Ƃ��͏��X�ɃX�s�[�h������
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
    /// �v���C���[�̈ړ����s������
    /// </summary>
    void MovePlayer()
    {
        // ���͕����Ɋ�Â��Ĉړ��������v�Z
        Vector3 moveDirection = new Vector3(_moveInputValue.x, 0f, _moveInputValue.y).normalized;

        // �ړ��������[���łȂ��ꍇ�A�v���C���[�����̕����Ɍ�����
        if (moveDirection != Vector3.zero)
        {
            // �v���C���[�̌������ړ������ɍ��킹��
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            _playerRB.MoveRotation(Quaternion.Slerp(_playerRB.rotation, targetRotation, Time.deltaTime * 2f)); // 5f �͉�]���x
        }

        Vector3 foward = transform.forward * _nowSpeed;
        // �͂������Ĉړ�
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
