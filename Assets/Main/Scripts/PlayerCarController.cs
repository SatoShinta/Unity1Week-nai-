using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCarController : MonoBehaviour
{
    [SerializeField, Header("player�̃��W�b�g�{�f�B")] Rigidbody plRigitbody;
    [SerializeField, Header("player��Animator������")] Animator plAnim = null;
    [SerializeField, Header("�Ԏ��̏��")] List<AxleInfo> _axleInfos = new List<AxleInfo>();
    [SerializeField, Header("�z�C�[���ɓK�p�\�ȍő�g���N")] float _maxMotorTorque;
    [SerializeField, Header("�K�p�\�ȍő�n���h���p�x")] float _maxSteeringAngle;
    [SerializeField] InputSystem_Actions _inputActions;
    [SerializeField, Header("���݂̃X�s�[�h")] float _nowSpeed = 0f;
    [SerializeField, Header("�������тɑ�����X�s�[�h")] float _increaseSpeed = 0.5f;
    [SerializeField, Header("���X�Ɍ���X�s�[�h")] float _decrecaseSpeed = 0.5f;
    [SerializeField, Header("�o�ߎ���")] float _elapsedtime = 0f;
    float _speed;

    Vector2 _moveInputValue;

    void Start()
    {
        _inputActions = new InputSystem_Actions();
        _inputActions.Player.Move.started += OnMove;
        _inputActions.Player.Move.performed += OnMove;
        _inputActions.Player.Move.canceled += OnMove;
        _inputActions.Enable(); // �Y�ꂸ�Ɂc
    }

    private void FixedUpdate()
    {
        SpeedControl();
        float motor = _nowSpeed;
        float steering = _maxSteeringAngle * _moveInputValue.x;

        foreach (var axleInfo in _axleInfos)
        {
            if (axleInfo.steering)
            {
                axleInfo.leftWheel.steerAngle = steering;
                axleInfo.rightWheel.steerAngle = steering;
            }
            if (axleInfo.motor)
            {
                axleInfo.leftWheel.motorTorque = motor;
                axleInfo.rightWheel.motorTorque = motor;
            }
            if (!this.gameObject.CompareTag("Player"))
            {
                ApplyLocalPositionToVisuals(axleInfo.rightWheel);
                ApplyLocalPositionToVisuals(axleInfo.leftWheel);
            }

        }

        if (plAnim)
        {
            // �A�j���[�V�����p�ɐ��l�̒���
            plAnim?.SetFloat("Blend", _nowSpeed);
        }

        _speed = plRigitbody.linearVelocity.magnitude;
        Debug.Log(_speed);
    }

    /// <summary>
    /// ���͂�Vector2�ɕϊ����Ď󂯎�郁�\�b�h
    /// </summary>
    /// <param name="context"></param>
    private void OnMove(InputAction.CallbackContext context)
    {
        _moveInputValue = context.ReadValue<Vector2>();
    }

    /// <summary>
    /// �^�C���̉�]���s�����\�b�h
    /// </summary>
    /// <param name="collider"></param>
    public void ApplyLocalPositionToVisuals(WheelCollider collider)
    {
        Transform visualWheel = collider.transform.GetChild(0);
        Vector3 position;
        Quaternion rotation;
        collider.GetWorldPose(out position, out rotation);
        visualWheel.transform.position = position;
        visualWheel.transform.rotation = rotation;
    }

    /// <summary>
    /// �X�y�[�X�L�[��A�ł��Ă���Ƃ��͏��X�ɃX�s�[�h������
    /// �����Ă��Ȃ��Ƃ��͏��X�ɃX�s�[�h���������Ă���
    /// </summary>

    void SpeedControl()
    {
        _elapsedtime += Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            // ���x������݂���
            if (_nowSpeed < _maxMotorTorque)
            {
                _nowSpeed += _increaseSpeed;
                _elapsedtime = 0f;
            }
        }
        else
        {
            // ��莞�ԃX�y�[�X�L�[�������Ă��Ȃ��Ə��X�ɃX�s�[�h�������Ă�������
            if (_elapsedtime >= 1.5f && _nowSpeed > 0f)
            {
                _nowSpeed -= _decrecaseSpeed;
            }
            else if (_nowSpeed <= 0)
            {
                _nowSpeed = 0;
                _elapsedtime = 0f;
            }
        }

    }




    private void OnDestroy()
    {
        _inputActions.Dispose(); // �Y�ꂸ�Ɂc
    }

    [System.Serializable]
    public class AxleInfo
    {
        public WheelCollider leftWheel;
        public WheelCollider rightWheel;
        public bool motor; //���̃z�C�[�����G���W���ɃA�^�b�`����Ă��邩�ǂ���
        public bool steering; // ���̃z�C�[�����n���h���̊p�x�𔽉f���Ă��邩�ǂ���
    }
}
