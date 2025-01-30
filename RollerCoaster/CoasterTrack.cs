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
            var sourceTransform = sourceContainer.transform;
            for (int i = 0; i < sourceContainer.Spline.Count; i++)
            {
                var knot = sourceContainer.Spline[i];
                var worldPos = sourceTransform.TransformPoint(knot.Position);
                var localPos = transform.InverseTransformPoint(worldPos);
                TrackSpline.Spline.Add(localPos, tangentMode);
            }
        }

        TrackSpline.Spline.Closed = loopingTrack;
    }
}
