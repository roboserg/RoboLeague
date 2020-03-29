using UnityEngine;

public class BallTrail : MonoBehaviour
{
    public float trailTime = 0.7f;
    
    float _speedMagnitude = 0, _speedMagnitudeKmH = 0;
    Rigidbody _rb;
    TrailRenderer _trail;
    Gradient _gradient;
    
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _trail = GetComponent<TrailRenderer>();
        _trail.time = trailTime;
        
        _gradient = new Gradient();
        _gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(Color.white, 0.0f), new GradientColorKey(Color.white, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(1f, 0.0f), new GradientAlphaKey(0, 1.0f) }
        );
    }
    
    private void FixedUpdate()
    {
        _speedMagnitude = _rb.velocity.magnitude;
        _speedMagnitudeKmH = _speedMagnitude * 3.6f;

        var currentAlpha = CubeController.Scale(40, 100, 0, 1, _speedMagnitudeKmH);
        currentAlpha = Mathf.Clamp(currentAlpha, 0, 1);
        _gradient.alphaKeys = new GradientAlphaKey[]{ new GradientAlphaKey(currentAlpha, 0), new GradientAlphaKey(0, 1) }; 
        _trail.colorGradient = _gradient;

        var maxTrailWidth = 0.6f;
        var minTrailWidth = 0.3f;
        var trailWidth = CubeController.Scale(40, 80, minTrailWidth, maxTrailWidth, _speedMagnitudeKmH);
        _trail.startWidth = Mathf.Clamp(trailWidth, minTrailWidth, maxTrailWidth);
    }
}