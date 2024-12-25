using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCarController : MonoBehaviour
{
    [SerializeField,Header("車軸の情報")] List<AxleInfo> _axleInfos = new List<AxleInfo>();
    [SerializeField,Header("ホイールに適用可能な最大トルク")] float _maxMotorTorque;
    [SerializeField,Header("適用可能な最大ハンドル角度")] float _maxSteeringAngle;
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
        _inputActions.Dispose(); // 忘れずに…
    }

    [System.Serializable]
    public class AxleInfo
    {
        public WheelCollider leftWheel;
        public WheelCollider rightWheel;
        public bool motor; //このホイールがエンジンにアタッチされているかどうか
        public bool steering; // このホイールがハンドルの角度を反映しているかどうか
    }
}
