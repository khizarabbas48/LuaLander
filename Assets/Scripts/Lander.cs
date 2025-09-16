using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Lander : MonoBehaviour
{
    private const float Gravity_Normal = 0.7f;

    // Singleton instance
    public static Lander Instance { get; private set; }

    // Events for thruster actions
    public event EventHandler onUpForce;
    public event EventHandler onLeftForce;
    public event EventHandler onRightForce;
    public event EventHandler onBeforeForce;
    public event EventHandler onCoinPickup;
    public event EventHandler onFuelPickup;
    

    // Event for state changes
    public event EventHandler<OnStateChangedEventArgs> OnStateChange;
    public class OnStateChangedEventArgs : EventArgs
    {
        public State state;
    }

    // Event for landing with score details
    public event EventHandler<OnLandedEventArgs> onLanded;
    public class OnLandedEventArgs : EventArgs
    {
        public LandingType landingType;
        public int score;
        public float dotVector;
        public float landingSpeed;
        public float scoreMultiplier;
    }

    // Types of landing outcomes
    public enum LandingType
    {
        Success,
        WrongLandingArea,
        TooSteepLandingAngle,
        TooFastLandingSpeed
    }

    public enum State
    {
        WatingToStart,
        Normal,
        GameOver,
    }

    private Rigidbody2D landerRigidbody2D;
    private float fuelAmout; // Tracke of Fuel Amount
    private float FuelAmountMax = 10f; // Maximum fuel capacity
    private State state; // Current state of the lander

    private void Awake()
    {
        Instance = this; // Set the singleton instance

        fuelAmout = FuelAmountMax; // Initialize fuel amount to maximum capacity
        state = State.WatingToStart;

        landerRigidbody2D = GetComponent<Rigidbody2D>(); // Get the Rigidbody2D component
        landerRigidbody2D.gravityScale = 0f; // Set gravity scale
    }

    private void FixedUpdate()
    {
        // invoke before applying any forces
        onBeforeForce?.Invoke(this, EventArgs.Empty);

        switch (state)
        {
            default:
            case State.WatingToStart:
                if (
                    GameInput.Instance.IsUpActionPressed() ||
                    GameInput.Instance.IsLeftActionPressed() ||
                    GameInput.Instance.IsRightActionPressed() ||
                    GameInput.Instance.GetMovementInputValueVector2() != Vector2.zero)
                {
                    ConsumeFuel();

                    // Enable gravity on inputs are used
                    landerRigidbody2D.gravityScale = Gravity_Normal;
                    state = State.Normal;
                    SetState(State.Normal);
                }
                break;
            case State.Normal:
                // Fuel check
                if (fuelAmout <= 0)
                {
                    //No fuel
                    return;
                }

                if (
                    GameInput.Instance.IsUpActionPressed() ||
                    GameInput.Instance.IsLeftActionPressed() ||
                    GameInput.Instance.IsRightActionPressed() ||
                    GameInput.Instance.GetMovementInputValueVector2() != Vector2.zero)
                {
                    //Pressing Any Input
                    ConsumeFuel();
                }

                float gamePadDeadZone = .4f;
                if (GameInput.Instance.IsUpActionPressed() || GameInput.Instance.GetMovementInputValueVector2().y > gamePadDeadZone)
                {
                    float force = 700f; // Adjust the force value as needed
                    landerRigidbody2D.AddForce(force * transform.up * Time.deltaTime); //Apply force in the direction lander is facing

                    onUpForce?.Invoke(this, EventArgs.Empty); // invoke
                }

                if (GameInput.Instance.IsLeftActionPressed() || GameInput.Instance.GetMovementInputValueVector2().x < -gamePadDeadZone)
                {
                    float leftTurnSpeed = 100f; // Adjust the turn speed as needed
                    landerRigidbody2D.AddTorque(leftTurnSpeed * Time.deltaTime); // Positive torque for counter-clockwise rotation

                    onLeftForce?.Invoke(this, EventArgs.Empty);
                }

                if (GameInput.Instance.IsRightActionPressed() || GameInput.Instance.GetMovementInputValueVector2().x > gamePadDeadZone)
                {
                    float rightTurnSpeed = -100f; // Adjust the turn speed as needed
                    landerRigidbody2D.AddTorque(rightTurnSpeed * Time.deltaTime); // Negative torque for clockwise rotation

                    onRightForce?.Invoke(this, EventArgs.Empty);
                }
                break;
            case State.GameOver:
                // Do nothing
                break;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision2D)
    {
        // Detect the collision if it is on the Landing pad or not
        if (!collision2D.gameObject.TryGetComponent(out LandingPad landingPad))
        {
            Debug.Log("Crashed on the terrain!");

            // Trigger the onLanded event for wrong landing area
            onLanded?.Invoke(this, new OnLandedEventArgs
            {
                landingType = LandingType.WrongLandingArea,
                dotVector = 0f,
                landingSpeed = 0f,
                scoreMultiplier = 0f,
                score = 0
            });
            SetState(State.GameOver);

            return;
        }

        //Check the landing velocity that the lander has when it collides with the landing pad 
        float SoftLandingVelocityMagnitude = 4f;
        float relativeVelocityMagnitude = collision2D.relativeVelocity.magnitude;
        if (relativeVelocityMagnitude > SoftLandingVelocityMagnitude)
        {
            Debug.Log("Landed Too Hard!");

            // Trigger the onLanded event for too fast landing speed
            onLanded?.Invoke(this, new OnLandedEventArgs
            {
                landingType = LandingType.TooFastLandingSpeed,
                dotVector = 0f,
                landingSpeed = relativeVelocityMagnitude,
                scoreMultiplier = 0,
                score = 0
            });
            SetState(State.GameOver);
            return;
        }

        //Dot product to determine alignment with the ground
        float dotVector = Vector2.Dot(Vector2.up, transform.up);
        float minDotVectorForSafeLanding = .90f;
        if (dotVector < minDotVectorForSafeLanding)
        {
            Debug.Log("Landed on too steep Angle!");

            // Trigger the onLanded event for too steep landing angle
            onLanded?.Invoke(this, new OnLandedEventArgs
            {
                landingType = LandingType.TooSteepLandingAngle,
                dotVector = dotVector,
                landingSpeed = relativeVelocityMagnitude,
                scoreMultiplier = 0,
                score = 0
            });
            SetState(State.GameOver);
            return;
        }

        Debug.Log("Landed Successfully!");

        float maxScoreLandingAngle = 100;
        float scoreDotVectorMultiplier = 10f;

        // Calculate the landing angle score based on the dot product
        float landingAngleScore = maxScoreLandingAngle - Mathf.Abs(dotVector - 1f) *
        scoreDotVectorMultiplier * maxScoreLandingAngle;

        float maxScoreLandingSpeed = 100;
        float landingSpeedScore = (SoftLandingVelocityMagnitude - relativeVelocityMagnitude) *
        maxScoreLandingSpeed;

        Debug.Log("Landing Angle " + landingAngleScore);
        Debug.Log("Lading Speed " + landingSpeedScore);

        // Calculate the total score with the landing pad's score multiplier
        int score = Mathf.RoundToInt((landingAngleScore + landingSpeedScore) * landingPad.GetscoreMultiplier());
        Debug.Log("Score: " + score);

        // Trigger the onLanded event for successful landing
        onLanded?.Invoke(this, new OnLandedEventArgs
        {
            landingType = LandingType.Success,
            dotVector = dotVector,
            landingSpeed = relativeVelocityMagnitude,
            scoreMultiplier = landingPad.GetscoreMultiplier(),
            score = score
        });
        SetState(State.GameOver);

    }

    private void OnTriggerEnter2D(Collider2D collision2D)
    {
        // Detect fuel pickup
        if (collision2D.gameObject.TryGetComponent(out FuelPickup fuelPickup))
        {
            float addFuelAmount = 10f;
            fuelAmout += addFuelAmount;

            // Ensure fuel does not exceed maximum capacity
            if (fuelAmout > FuelAmountMax)
            {
                fuelAmout = FuelAmountMax;
            }

            onFuelPickup?.Invoke(this, EventArgs.Empty);
            fuelPickup.DestroySelf();
        }
        // Detect coin pickup
        if (collision2D.gameObject.TryGetComponent(out CoinPickup coinPickup))
        {
            onCoinPickup?.Invoke(this, EventArgs.Empty);
            coinPickup.DestroySelf();
        }
    }

    private void SetState(State state)
    {
        this.state = state;
        OnStateChange?.Invoke(this, new OnStateChangedEventArgs
        {
            state = state
        });
    }

    // Reduce fuel amount when thrusters are used on inputs
    private void ConsumeFuel()
    {
        float fuelConsumptionAmount = 1f; // Fuel consumption rate per second
        fuelAmout -= fuelConsumptionAmount * Time.deltaTime;
    }

    // Getters for fuel and speed
    public float GetFuelAmountNormalized()
    {
        return fuelAmout / FuelAmountMax;
    }

    public float GetFuel()
    {
        return fuelAmout;
    }
    public float GetSpeedX()
    {
        return landerRigidbody2D.linearVelocityX;
    }
    public float GetSpeedY()
    {
        return landerRigidbody2D.linearVelocityY;
    }
}
