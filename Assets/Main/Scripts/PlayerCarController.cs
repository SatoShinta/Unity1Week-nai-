using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCarController : MonoBehaviour
{
    [SerializeField, Header("playerのリジットボディ")] Rigidbody plRigitbody;
    [SerializeField, Header("playerのAnimatorを入れる")] Animator plAnim = null;
    [SerializeField, Header("車軸の情報")] List<AxleInfo> _axleInfos = new List<AxleInfo>();
    [SerializeField, Header("ホイールに適用可能な最大トルク")] float _maxMotorTorque;
    [SerializeField, Header("適用可能な最大ハンドル角度")] float _maxSteeringAngle;
    [SerializeField] InputSystem_Actions _inputActions;
    [SerializeField, Header("現在のスピード")] float _nowSpeed = 0f;
    [SerializeField, Header("押すたびに増えるスピード")] float _increaseSpeed = 0.5f;
    [SerializeField, Header("徐々に減るスピード")] float _decrecaseSpeed = 0.5f;
    [SerializeField, Header("経過時間")] float _elapsedtime = 0f;
    float _speed;

    Vector2 _moveInputValue;

    void Start()
    {
        _inputActions = new InputSystem_Actions();
        _inputActions.Player.Move.started += OnMove;
        _inputActions.Player.Move.performed += OnMove;
        _inputActions.Player.Move.canceled += OnMove;
        _inputActions.Enable(); // 忘れずに…
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
            // アニメーション用に数値の調整
            plAnim?.SetFloat("Blend", _nowSpeed);
        }

        _speed = plRigitbody.linearVelocity.magnitude;
        Debug.Log(_speed);
    }

    /// <summary>
    /// 入力をVector2に変換して受け取るメソッド
    /// </summary>
    /// <param name="context"></param>
    private void OnMove(InputAction.CallbackContext context)
    {
        _moveInputValue = context.ReadValue<Vector2>();
    }

    /// <summary>
    /// タイヤの回転を行うメソッド
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
    /// スペースキーを連打しているときは徐々にスピードが増す
    /// 押していないときは徐々にスピードが減少していく
    /// </summary>

    void SpeedControl()
    {
        _elapsedtime += Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            // 速度制限を設けた
            if (_nowSpeed < _maxMotorTorque)
            {
                _nowSpeed += _increaseSpeed;
                _elapsedtime = 0f;
            }
        }
        else
        {
            // 一定時間スペースキーを押していないと徐々にスピードが落ちていく処理
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
