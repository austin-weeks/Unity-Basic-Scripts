using UnityEngine;
using UnityEngine.Splines;

public class CoasterTrack : MonoBehaviour
{
    public SplineContainer TrackSpline { get; private set; }
    [SerializeField] bool loopingTrack = true;
    [Header("Track Smoothing Mode")]
    [SerializeField] TangentMode tangentMode = TangentMode.AutoSmooth;

    void Awake()
    {
        TrackSpline = GetComponent<SplineContainer>();
        if (TrackSpline == null)
        {
            TrackSpline = gameObject.AddComponent<SplineContainer>();
        }
        CombineSplines();
    }

    void CombineSplines()
    {
        TrackSpline.Spline.Clear();
        var childContainers = GetComponentsInChildren<SplineContainer>();
        if (childContainers.Length == 0)
        {
            Debug.LogError("There are no spline containers in the track's children!", this);
            return;
        }

        foreach (var sourceContainer in childContainers)
        {
            if (sourceContainer == TrackSpline) continue;
            foreach (var knot in sourceContainer.Spline)
            {
                var worldPos = sourceContainer.track.TransformPoint(knot.Position);
                var localPos = transform.InverseTransformPoint(worldPos);
                TrackSpline.Spline.Add(localPos, tangentMode);
            }
        }

        TrackSpline.Spline.Closed = loopingTrack;
    }
}