using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCarController : MonoBehaviour
{
    [SerializeField,Header("�Ԏ��̏��")] List<AxleInfo> _axleInfos = new List<AxleInfo>();
    [SerializeField,Header("�z�C�[���ɓK�p�\�ȍő�g���N")] float _maxMotorTorque;
    [SerializeField,Header("�K�p�\�ȍő�n���h���p�x")] float _maxSteeringAngle;
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
        float motor = _maxMotorTorque * _moveInputValue.y;
        float steering = _maxSteeringAngle * _moveInputValue.x;

        foreach(var axleInfo in _axleInfos)
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
