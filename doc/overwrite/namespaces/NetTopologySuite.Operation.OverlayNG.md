---
uid: NetTopologySuite.Operation.OverlayNG
summary: *content
---
Contains classes that perform vector overlay to compute boolean set-theoretic spatial functions.
Overlay operations are used in spatial analysis for computing set-theoretic operations (boolean combinations) of input <xref href="NetTopologySuite.Geometries.Geometry">Geometry</xref> s.

The <xref href="NetTopologySuite.Operation.OverlayNG.OverlayNG">OverlayNG</xref> class provides the standard Simple Features
boolean set-theoretic overlay operations.  
These are:
* **Intersection** - all points which lie in both geometries
* **Union** - all points which lie in at least one geometry
* **Difference** - all points which lie in the first geometry but not the second
* **Symmetric Difference** - all points which lie in one geometry but not both

These operations are supported for all combinations of the basic geometry types and their homogeneous collections.

Additional operations include:

* <xref href="NetTopologySuite.Operation.Union.UnaryUnionOp">UnaryUnion</xref> unions collections of geometries in an efficient way
* <xref href="NetTopologySuite.Operation.OverlayNG.CoverageUnion">CoverageUnion</xref> provides enhanced performance for unioning valid polygonal and lineal coverages
* <xref href="NetTopologySuite.Operation.OverlayNG.PrecisionReducer">PrecisionReducer</xref> allows reducing the precision of a geometry in a topologically-valid way

## Semantics
The requirements for overlay input are:
* Input geometries may have different dimension.
* Collections must be homogeneous   
  (all elements must have the same dimension).
* In general, inputs must be valid geometries.
* However, polygonal inputs may contain the following two kinds of "mild" invalid topology:
  * rings which self-touch at discrete points (sometimes called inverted shells and exverted holes).
  * rings which touch along line segments (i.e. topology collapse).

The semantics of overlay output are:
* Results are always valid geometries.  
  In particular, result `MultiPolygon`s are valid.
* Repeated vertices are removed
* Linear results include all nodes (endpoints) present in the input.
  In some cases more nodes will be present.
  (If merged lines are required see <xref href="NetTopologySuite.Operation.Linemerge.LineMerger">LineMerger</xref>.)
* Polygon edges which undergo topology collapse to lines
  (due to rounding or snapping) are included in the result.
  This means that all operations may produce a heterogeneous result.
  Usually this only occurs when using a fixed-precision model,
  but it can happen due to snapping performed to improve robustness.
* The `intersection` operation result includes all components of the
  intersection for geometries which intersect in components of the same and/or
  lower dimension.
* The `difference` operation produces a homogeneous result if no topology
  collapses are present.   
  In this case the result dimension is equal to that of the left-hand operand.
* The `union` and `symmetric difference` operations
  may produce heterogeneous result if the inputs are of mixed dimension.
* Homogeneous results are output as `Multi` geometries.
* Heterogeneous results are output as a `GeometryCollection`
  containing a set of atomic geometries.  
  (This provides backwards compatibility
  with the original NTS overlay implementation.
  However, it loses the information that the polygonal results
  have valid `MultiPolygon` topology.)
* Empty results are atomic `EMPTY` geometries of dimension appropriate
  to the operation.
* As far as possible, results preserve the order and direction of the inputs.
  For instance, a MultiLineString intersection with a Polygon
  will have resultants which are in the same order and have the same direction
  as the input lines (assuming the input lines are disjoint).
  If an input line is split into two or more parts, 
  they are ordered in the direction of occurence along their parent line.

## Features
Functionality
* **Precision Model** - operations are performed using a defined precision model (finite or floating)
* **Robust Computation** - provides fully robust computation when an appropriate noder is used
* **Performance optimizations** - including:
  * Short-circuiting for disjoint input envelopes
  * Reduction of input segment count via clipping / limiting to overlap envelope
  * Optimizations can be disabled if required (e.g. for testing or performance evaluation)
* **Pluggable Noding** - allows using different noders to change characteristics of performance and accuracy
* **Precision Reduction** - in a topologically correct way. Implemented by unioning a single input with an empty geometry
* **[Topology Correction / Conversion]** - handles certain kinds
of polygonal inputs which are invalid
* **Fast Coverage Union** - of valid polygonal and linear coverages

## Pluggable Noding
The noding phase of overlay uses a <xref href="NetTopologySuite.Noding.INoder">INoder</xref> subclass. This is determine automatically based on the precision model of the input. Or it can be provided explicity, which allows changing characteristics of performance and robustness. Examples of relevant noders include:
* <xref href="NetTopologySuite.Noding.MCIndexNoder">MCIndexNoder</xref> - a fast full-precision noder, which however may not produce
a valid noding in some situations. Should be combined with a <xref href="NetTopologySuite.Noding.ValidatingNoder">ValidatingNoder</xref> wrapper to detect
noding failures.
* <xref href="NetTopologySuite.Noding.Snap.SnappingNoder">SnappingNoder</xref> - a robust full-precision noder
* <xref href="NetTopologySuite.Noding.Snapround.SnapRoundingNoder">SnapRoundingNoder</xref> - a noder which enforces a supplied fixed precision model
by snapping vertices and intersections to a grid
* <xref href="NetTopologySuite.Noding.ValidatingNoder">ValidatingNoder</xref> - a wrapper which can be used to verify the noding prior to topology building
* <xref href="NetTopologySuite.Operation.OverlayNG.SegmentExtractingNoder">SegmentExtractingNoder</xref> - requires node-clean input, and provides very fast noding

## Topology Correction / Conversion
As noted above, the overlay process
can handle polygonal inputs which are invalid according to the OGC topology model
in certain limited ways.
These invalid conditions are:
* rings which self-touch at discrete points (sometimes called inverted shells and exverted holes).
* rings which touch along line segments (i.e. topology collapse).

These invalidities are corrected during the overlay process.

Some of these invalidities are considered as valid in other geometry models.
By peforming a self-overlay these inputs can be converted
into the JTS OGC topological model.

## Codebase
* Defines a simple, full-featured topology model, with clear semantics.
  The topology model incorporates handling topology collapse, which is
  essential for snapping and fixed-precision noding.
* Uses a simple topology graph data structure (based on the winged edge pattern).
* Uses a simpler topology graph data structure (based on winged edge pattern).
* Decouples noding and topology-build phases. This makes the code clearer, and makes it possible to allow supplying alternate implementations and semantics for each phase.
* All optimizations are implemented internally, so that clients do not have to add checks such as envelope overlap<./li>

## Algorithm
For non-point inputs the overlay algorithm is:
1. Check for empty input geometries, and return a result appropriate for the specified operation
2. Extract linework and points from input geometries, with topology location information
3. (If optimization enabled) Apply overlap envelope optimizations:
   1. For Intersection, check if the input envelopes are disjoint (using an envelope expansion adjustment to account for the precision grid).
   2. For Intersection and Difference, clip or limit the linework of the input geometries to the overlap envelope.
   3. If the optimized linework is empty, return an empty result of appropriate type.
4. Node the linework. For full robustness snap-rounding noding is used. Other kinds of noder can be used as well (for instance, the full-precision noding algorithm as the original overlay code).
5. Merge noded edges. Coincident edges from the two input geometries are merged, along with their topological labelling. Topology collapses are detected in this step, and are flagged in the labelling so they can be handled appropriately duing result polygon extraction
6. Build a fully-labelled topology graph. This includes:
   1. Create a graph structure on the noded, merged edges
   2. Propagate topology locations around nodes in the graph
   3. Label edges that have incomplete topology locations. These occur when edges from an input geometry are isolated (disjoint from the edges of the other geometry in the graph).
7. If result is empty return an empty geometry of appropriate type
8. Generate the result geometry from the labelled graph:
   1. Build result polygons
      1. Mark edges which should be included in the result areas
      2. Link maximal rings together
      3. Convert maximal rings to minimal (valid) rings
      4. Determine nesting of holes
      5. Construct result polygons
   2. Build result linework
      1. Mark edges to be included in the result lines
      2. Construct node-to-node linework
   3. Build result points
      1. For intersection only, output point occur where the input touch at single points
   4. Collect result elements into the result geometry

## Package Specification
* OpenGIS Simple Features Specification for SQL
