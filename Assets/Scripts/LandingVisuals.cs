using UnityEngine;

public class LandingVisuals : MonoBehaviour
{
    // Particle systems for thrusters
    [SerializeField] private ParticleSystem LeftThrusterParticleSystem;
    [SerializeField] private ParticleSystem MiddleThrusterParticleSystem;
    [SerializeField] private ParticleSystem RightThrusterParticleSystem;
    [SerializeField] private GameObject landerExplosionVFX;
    private Lander lander;

    private void Awake()
    {
        lander = GetComponent<Lander>(); // get the lander component
        // Events listners 
        //
        lander.onUpForce += Lander_onUpForce;
        lander.onLeftForce += Lander_onLeftForce;
        lander.onRightForce += Lander_onRightForce;
        lander.onBeforeForce += Lander_onBeforeForce;
        
        // Disable all thruster particle systems at the start
        SetEnabledThrusterParticleSystem(LeftThrusterParticleSystem, false);
        SetEnabledThrusterParticleSystem(MiddleThrusterParticleSystem, false);
        SetEnabledThrusterParticleSystem(RightThrusterParticleSystem, false);
    }

    private void Start()
    {
        lander.onLanded += Lander_onLanded;
    }
    private void Lander_onLanded(object sender, Lander.OnLandedEventArgs e)
    {
        switch (e.landingType)
        {
            case Lander.LandingType.WrongLandingArea:
            case Lander.LandingType.TooSteepLandingAngle:
            case Lander.LandingType.TooFastLandingSpeed:
                // spawn crash particle effect
                Instantiate(landerExplosionVFX, transform.position, Quaternion.identity);
                gameObject.SetActive(false); // Disable the lander object
                break;
        }
    }

    // Event handlers for lander events
    // Disable all thruster particle systems before applying any forces
    private void Lander_onBeforeForce(object sender, System.EventArgs e)
    {
        SetEnabledThrusterParticleSystem(LeftThrusterParticleSystem, false);
        SetEnabledThrusterParticleSystem(MiddleThrusterParticleSystem, false);
        SetEnabledThrusterParticleSystem(RightThrusterParticleSystem, false);
    }

    // Enable left thruster particle system and disable others
    private void Lander_onLeftForce(object sender, System.EventArgs e)
    {
        SetEnabledThrusterParticleSystem(RightThrusterParticleSystem, true);
    }

    // Enable right thruster particle system and disable others
    private void Lander_onRightForce(object sender, System.EventArgs e)
    {
        SetEnabledThrusterParticleSystem(LeftThrusterParticleSystem, true);
    }

    // Enable all thruster particle systems
    private void Lander_onUpForce(object sender, System.EventArgs e)
    {
        SetEnabledThrusterParticleSystem(LeftThrusterParticleSystem, true);
        SetEnabledThrusterParticleSystem(MiddleThrusterParticleSystem, true);
        SetEnabledThrusterParticleSystem(RightThrusterParticleSystem, true);
    }

    // Helper method to enable or disable a thruster particle system
    private void SetEnabledThrusterParticleSystem(ParticleSystem particleSystem, bool enabled)
    {
        ParticleSystem.EmissionModule emissionModule = particleSystem.emission; // get the emission module
        emissionModule.enabled = enabled; // enable or disable the emission module
    }

}
