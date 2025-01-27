# Coaster Track System

The `CoasterTrack` class allows multiple splines to be combined at runtime, making it easier to create large roller coaster tracks with model assets.

## Usage

The `CoasterTrack` class should be attached to a parent object for each coaster in the game.

For each coaster, track pieces should be placed as child objects of the parent coaster object. The track pieces should be in order.

Each track piece should have its own `SplineContainer` component that contains a spline matching its track shape.

The `CoasterTrack` class will automatically combine any splines within its children at runtime. As such, the `CoasterCar` class uses the `CoasterTrack.TrackSpline` property, which is a `SplineContainer` that contains the combined splines.