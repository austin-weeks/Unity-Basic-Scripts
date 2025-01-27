using UnityEngine;

public class CoasterCar : MonoBehaviour
{
    public CoasterTrack track; // Reference to the spline
    public const float baseSpeed = 5f;       // Movement speed along the spline
    private float progress = 0f;   // Progress along the spline (0 to 1)
    private float curSpeed = baseSpeed;

    void Awake()
    {
        if (track == null)
        {
            Debug.LogError("Coaster Car is missing a track!");
        }
    }

    void Update()
    {
        if (track.TrackSpline == null) return;
        // Update progress based on speed and time
        progress += curSpeed * Time.deltaTime / track.TrackSpline.CalculateLength();

        // Wrap progress to stay within the bounds of the spline
        progress = Mathf.Repeat(progress, 1f);

        // Evaluate position on the spline
        transform.position = track.TrackSpline.EvaluatePosition(progress);

        // Evaluate slope of spline for rotating capsule 
        Vector3 splineTangent = track.TrackSpline.EvaluateTangent(progress);
        splineTangent.Normalize();
        transform.forward = splineTangent;

        float momentumFactor = 10f;
        curSpeed = baseSpeed + (momentumFactor * -splineTangent.y);
        if (curSpeed < 0.5f)
        {
            curSpeed = baseSpeed / 2f;
        }
    }
}
