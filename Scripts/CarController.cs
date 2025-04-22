using UnityEngine;

public class CarController : MonoBehaviour
{
    public WheelCollider frontLeftWheel, frontRightWheel;  // Přední kola
    public WheelCollider rearLeftWheel, rearRightWheel;    // Zadní kola
    public Transform frontLeftMesh, frontRightMesh;        // Vizuální kola
    public Transform rearLeftMesh, rearRightMesh;

    public Transform steeringWheel;                        // GameObject volantu

    public float motorForce = 1500f;       // Síla motoru
    public float maxSteerAngle = 30f;      // Maximální úhel zatáčení
    public float brakeForce = 3000f;       // Automatická brzda
    public float handbrakeForce = 10000f;  // Ruční brzda (mezerník)

    private float steeringInput;           // Vstup pro zatáčení
    private float throttleInput;           // Vstup pro plyn
    private bool isHandbrakeActive;
    private void Start()
    {
        AdjustFriction(frontLeftWheel);
        AdjustFriction(frontRightWheel);
        AdjustFriction(rearLeftWheel);
        AdjustFriction(rearRightWheel);
    }
    void Update()
    {
        // Získání vstupů
        steeringInput = Input.GetAxis("Horizontal");
        throttleInput = Input.GetAxis("Vertical");
        isHandbrakeActive = Input.GetKey(KeyCode.Space);

        Debug.Log($"Vstupy: Zatáčení = {steeringInput}, Plyn = {throttleInput}, Ruční brzda = {isHandbrakeActive}");

        // Aktualizace vizuálních kol
        UpdateWheelPose(frontLeftWheel, frontLeftMesh);
        UpdateWheelPose(frontRightWheel, frontRightMesh);
        UpdateWheelPose(rearLeftWheel, rearLeftMesh);
        UpdateWheelPose(rearRightWheel, rearRightMesh);

        // Otáčení volantu
        if (steeringWheel != null)
        {
            UpdateSteeringWheel();
        }
    }

    void FixedUpdate()
    {
        // Ovládání kol
        float steerAngle = maxSteerAngle * steeringInput;
        frontLeftWheel.steerAngle = steerAngle;
        frontRightWheel.steerAngle = steerAngle;

        float motorTorque = throttleInput * motorForce;
        frontLeftWheel.motorTorque = motorTorque;
        frontRightWheel.motorTorque = motorTorque;

        // Automatická brzda, když není plyn
        bool noThrottle = Mathf.Approximately(throttleInput, 0f);

        float currentBrakeForce = 0f;

        if (isHandbrakeActive)
        {
            currentBrakeForce = handbrakeForce;
        }
        else if (noThrottle)
        {
            currentBrakeForce = brakeForce;
        }

        // Brzdný moment aplikovaný na všechna kola
        ApplyBrakeForce(currentBrakeForce);
    }

    private void ApplyBrakeForce(float brake)
    {
        frontLeftWheel.brakeTorque = brake;
        frontRightWheel.brakeTorque = brake;
        rearLeftWheel.brakeTorque = brake;
        rearRightWheel.brakeTorque = brake;
    }

    private void UpdateWheelPose(WheelCollider collider, Transform mesh)
    {
        Vector3 position;
        Quaternion rotation;
        collider.GetWorldPose(out position, out rotation);
        mesh.position = position;
        mesh.rotation = rotation;
    }
    void AdjustFriction(WheelCollider wheel)
    {
        var forwardFriction = wheel.forwardFriction;
        forwardFriction.stiffness = 3.5f;
        wheel.forwardFriction = forwardFriction;

        var sidewaysFriction = wheel.sidewaysFriction;
        sidewaysFriction.stiffness = 3.5f;
        wheel.sidewaysFriction = sidewaysFriction;
    }
    private void UpdateSteeringWheel()
    {
        float steerAngle = maxSteerAngle * steeringInput;
        steeringWheel.localRotation = Quaternion.Lerp(
            steeringWheel.localRotation,
            Quaternion.Euler(0, 0, -steerAngle),
            Time.deltaTime * 5f
        );
    }
}
