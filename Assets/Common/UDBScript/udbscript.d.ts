declare namespace UDB {
	namespace GameConfiguration {
		/**
		 * Engine name, like `doom`, `boom`, `zdoom` etc. Used for the namespace in UDMF maps. Read-only.
		 */
		let engineName: string;
		/**
		 * If the game configuration supports local sidedef texture offsets (distinct offsets for upper, middle, and lower sidedef parts).
		 */
		let hasLocalSidedefTextureOffsets: boolean;
	}
	namespace Angle2D {
		/**
		 * Converts a Doom angle (where 0° is east) to a real world angle (where 0° is north).
		 * @param doomangle Doom angle in degrees
		 * @returns Doom angle in degrees
		 */
		function doomToReal(doomangle: number): number;
		/**
		 * Converts a Doom angle (where 0° is east) to a real world angle (where 0° is north) in radians.
		 * @param doomangle Doom angle in degrees
		 * @returns Doom angle in radians
		 */
		function doomToRealRad(doomangle: number): number;
		/**
		 * Converts a real world angle (where 0° is north) to a Doom angle (where 0° is east).
		 * @param realangle Real world angle in degrees
		 * @returns Doom angle in degrees
		 */
		function realToDoom(realangle: number): number;
		/**
		 * Converts a real world angle (where 0° is north) to a Doom angle (where 0° is east) in radians.
		 * @param realangle Real world angle in radians
		 * @returns Doom angle in degrees
		 */
		function realToDoomRad(realangle: number): number;
		/**
		 * Converts radians to degrees.
		 * @param rad Angle in radians
		 * @returns Angle in degrees
		 */
		function radToDeg(rad: number): number;
		/**
		 * Converts degrees to radians.
		 * @param deg Angle in degrees
		 * @returns Angle in radians
		 */
		function degToRad(deg: number): number;
		/**
		 * Normalizes an angle in degrees so that it is bigger or equal to 0° and smaller than 360°.
		 * @param angle Angle in degrees
		 * @returns Normalized angle in degrees
		 */
		function normalized(angle: number): number;
		/**
		 * Normalizes an angle in radians so that it is bigger or equal to 0 and smaller than 2 Pi.
		 * @param angle Angle in radians
		 * @returns Normalized angle in radians
		 */
		function normalizedRad(angle: number): number;
		/**
		 * Returns the angle between three positions.
		 * @param p1 First position
		 * @param p2 Second position
		 * @param p3 Third position
		 * @returns Angle in degrees
		 */
		function getAngle(p1: any, p2: any, p3: any): number;
		/**
		 * Returns the angle between three positions in radians.
		 * @param p1 First position
		 * @param p2 Second position
		 * @param p3 Third position
		 * @returns Angle in radians
		 */
		function getAngleRad(p1: any, p2: any, p3: any): number;
	}
	class BlockEntry {
		/**
		 * Gets all `Linedef`s in the blockmap entry.
		 * @returns `Array` of `Linedef`s
		 */
		getLinedefs(): Linedef[];
		/**
		 * Gets all `Thing`s in the blockmap entry.
		 * @returns `Array` of `Thing`s
		 */
		getThings(): Thing[];
		/**
		 * Gets all `Sector`s in the blockmap entry.
		 * @returns `Array` of `Sector`s
		 */
		getSectors(): Sector[];
		/**
		 * Gets all `Vertex` in the blockmap entry.
		 * @returns `Array` of `Vertex`
		 */
		getVertices(): Vertex[];
	}
	class BlockMapQueryResult {
		/**
		 * Gets all `Linedef`s in the blockmap query result.
		 * @returns `Array` of `Linedef`s
		 */
		getLinedefs(): Linedef[];
		/**
		 * Gets all `Thing`s in the blockmap query result.
		 * @returns `Array` of `Thing`s
		 */
		getThings(): Thing[];
		/**
		 * Gets all `Sector`s in the blockmap query result.
		 * @returns `Array` of `Sector`s
		 */
		getSectors(): Sector[];
		/**
		 * Gets all `Vertex` in the blockmap query result.
		 * @returns `Array` of `Vertex`
		 */
		getVertices(): Vertex[];
	}
	class BlockMap {
		/**
		 * Creates a blockmap that includes linedefs, things, sectors, and vertices.
		 */
		constructor();
		/**
		 * Creates a blockmap that only includes certain map element types.
		 * @param lines If linedefs should be added or not
		 * @param things If thigs should be added or not
		 * @param sectors If sectors should be added or not
		 * @param vertices If vertices should be added or not
		 */
		constructor(lines: boolean, things: boolean, sectors: boolean, vertices: boolean);
		/**
		 * Gets the `BlockEntry` at a point. The given point can be a `Vector2D` or an `Array` of two numbers.
		 * @param pos The point to get the `BlockEntry` of
		 * @returns The `BlockEntry` on the given point
		 */
		getBlockAt(pos: any): BlockEntry;
		/**
		 * Gets a `BlockMapQueryResult` for the blockmap along a line between two points. The given points can be `Vector2D`s or an `Array`s of two numbers.
		 * @param v1 The first point
		 * @param v2 The second point
		 * @returns The `BlockMapQueryResult` for the line between the two points
		 */
		getLineBlocks(v1: any, v2: any): BlockMapQueryResult;
		/**
		 * Gets a `BlockMapQueryResult` for the blockmap in a rectangle.
		 * @param x X position of the top-left corner of the rectangle
		 * @param y Y position of the top-left corner of the rectangle
		 * @param width Width of the rectangle
		 * @param height Height of the rectangle
		 * @returns None
		 */
		getRectangleBlocks(x: number, y: number, width: number, height: number): BlockMapQueryResult;
	}
	namespace Data {
		/**
		 * Returns an `Array` of all texture names.
		 * @returns `Array` of all texture names
		 */
		function getTextureNames(): string[];
		/**
		 * Checks if a texture with the given name exists.
		 * @param name Texture name to check
		 * @returns `true` if the texture exists, `false` if it doesn't
		 */
		function textureExists(name: string): boolean;
		/**
		 * Returns an `ImageInfo` object for the given texture name.
		 * @param name Texture name to get the info for
		 * @returns `ImageInfo` object containing information about the texture
		 */
		function getTextureInfo(name: string): ImageInfo;
		/**
		 * Returns an `Array`of all flat names.
		 * @returns `Array` of all flat names
		 */
		function getFlatNames(): string[];
		/**
		 * Checks if a flat with the given name exists.
		 * @param name Flat name to check
		 * @returns `true` if the flat exists, `false` if it doesn't
		 */
		function flatExists(name: string): boolean;
		/**
		 * Returns an `ImageInfo` object for the given flat name.
		 * @param name Flat name to get the info for
		 * @returns `ImageInfo` object containing information about the flat
		 */
		function getFlatInfo(name: string): ImageInfo;
	}
	class ImageInfo {
		/**
		 * Name of the image.
		 */
		name: string;
		/**
		 * Width of the image.
		 */
		width: number;
		/**
		 * Height of the image.
		 */
		height: number;
		/**
		 * Scale of the image as `Vector2D`.
		 */
		scale: Vector2D;
		/**
		 * If the image is a flat (`true`) or not (`false`).
		 */
		isFlat: boolean;
	}
	class Line2D {
		/**
		 * Creates a new `Line2D` from two points.
		 * @param v1 First point
		 * @param v2 Second point
		 */
		constructor(v1: any, v2: any);
		/**
		 * Returns the coordinates on the line, where `u` is the position between the first and second point, `u = 0.0` being on the first point, `u = 1.0` being on the second point, and `u = 0.5` being in the middle between the points.
		 * @param u Position on the line, between 0.0 and 1.0
		 * @returns Position on the line as `Vector2D`
		 */
		getCoordinatesAt(u: number): Vector2D;
		/**
		 * Returns the length of the `Line2D`.
		 * @returns Length of the `Line2D`
		 */
		getLength(): number;
		/**
		 * Returns the angle of the `Line2D` in radians.
		 * @returns Angle of `Line2D` in radians
		 */
		getAngleRad(): number;
		/**
		 * Return the angle of the `Line2D` in degrees.
		 * @returns Angle of the `Line2D` in degrees
		 */
		getAngle(): number;
		/**
		 * Returns the perpendicular of this line as `Vector2D`.
		 * @returns Perpendicular of this line as `Vector2D`
		 */
		getPerpendicular(): Vector2D;
		/**
		 * Checks if the given `Line2D` intersects this line. If `bounded` is set to `true` (default) the finite length of the lines is used, otherwise the infinite length of the lines is used.
		 * @param ray `Line2D` to check against
		 * @param bounded `true` (default) to use finite length of lines, `false` to use infinite length of lines
		 * @returns `true` if lines intersect, `false` if they do not intersect
		 */
		isIntersecting(ray: Line2D, bounded: boolean): boolean;
		/**
		 * Checks if the given line intersects this line. If `bounded` is set to `true` (default) the finite length of the lines is used, otherwise the infinite length of the lines is used.
		 * @param a1 First point of the line to check against
		 * @param a2 Second point of the line to check against
		 * @param bounded `true` (default) to use finite length of lines, `false` to use infinite length of lines
		 * @returns `true` if the lines intersect, `false` if they do not
		 */
		isIntersecting(a1: any, a2: any, bounded: boolean): boolean;
		/**
		 * Returns the intersection point of of the given line defined by its start and end points with this line as `Vector2D`. If the lines do not intersect the `x` and `y` properties of the `Vector2D` are `NaN`. If `bounded` is set to `true` (default) the finite length of the lines is used, otherwise the infinite length of the lines is used.
		 * @param a1 First point of first line
		 * @param a2 Second point of first line
		 * @param bounded `true` (default) to use finite length of lines, `false` to use infinite length of lines
		 * @returns The intersection point as `Vector2D`
		 */
		getIntersectionPoint(a1: any, a2: any, bounded: boolean): Vector2D;
		/**
		 * Returns the intersection point of of the given line with this line as `Vector2D`. If the lines do not intersect the `x` and `y` properties of the `Vector2D` are `NaN`. If `bounded` is set to `true` (default) the finite length of the lines is used, otherwise the infinite length of the lines is used.
		 * @param ray Other `Line2D` to get the intersection point from
		 * @param bounded `true` (default) to use finite length of lines, `false` to use infinite length of lines
		 * @returns The intersection point as `Vector2D`
		 */
		getIntersectionPoint(ray: Line2D, bounded: boolean): Vector2D;
		/**
		 * Returns which the of the line defined by its start and end point a given point is on.
		 * @param p Point to check
		 * @returns `< 0` if `p` is on the front (right) side, `> 0` if `p` is on the back (left) side, `== 0` if `p` in on the line
		 */
		getSideOfLine(p: any): number;
		/**
		 * `Vector2D` position of start of the line.
		 */
		v1: Vector2D;
		/**
		 * `Vector2D` position of end of the line.
		 */
		v2: Vector2D;
	}
	namespace Line2D {
		/**
		 * Checks if two lines intersect. If `bounded` is set to `true` (default) the finite length of the lines is used, otherwise the infinite length of the lines is used.
		 * @param line1 First `Line2D`
		 * @param line2 Second `Line2D`
		 * @param bounded `true` to use finite length of lines, `false` to use infinite length of lines
		 * @returns `true` if the lines intersect, `false` if they do not
		 */
		function areIntersecting(line1: Line2D, line2: Line2D, bounded: boolean): boolean;
		/**
		 * Checks if two lines defined by their start and end points intersect. If `bounded` is set to `true` (default) the finite length of the lines is used, otherwise the infinite length of the lines is used.
		 * @param a1 First point of first line
		 * @param a2 Second point of first line
		 * @param b1 First point of second line
		 * @param b2 Second point of second line
		 * @param bounded `true` (default) to use finite length of lines, `false` to use infinite length of lines
		 * @returns `true` if the lines intersect, `false` if they do not
		 */
		function areIntersecting(a1: any, a2: any, b1: any, b2: any, bounded: boolean): boolean;
		/**
		 * Returns the intersection point of two lines as `Vector2D`. If the lines do not intersect the `x` and `y` properties of the `Vector2D` are `NaN`. If `bounded` is set to `true` (default) the finite length of the lines is used, otherwise the infinite length of the lines is used.
		 * @param a1 First point of first line
		 * @param a2 Second point of first line
		 * @param b1 First point of second line
		 * @param b2 Second point of second line
		 * @param bounded `true` (default) to use finite length of lines, `false` to use infinite length of lines
		 * @returns The intersection point as `Vector2D`
		 */
		function getIntersectionPoint(a1: any, a2: any, b1: any, b2: any, bounded: boolean): Vector2D;
		/**
		 * Returns which the of the line defined by its start and end point a given point is on.
		 * @param v1 First point of the line
		 * @param v2 Second point of the line
		 * @param p Point to check
		 * @returns `< 0` if `p` is on the front (right) side, `> 0` if `p` is on the back (left) side, `== 0` if `p` in on the line
		 */
		function getSideOfLine(v1: any, v2: any, p: any): number;
		/**
		 * Returns the shortest distance from point `p` to the line defined by its start and end points. If `bounded` is set to `true` (default) the finite length of the lines is used, otherwise the infinite length of the lines is used.
		 * @param v1 First point of the line
		 * @param v2 Second point of the line
		 * @param p Point to get the distance to
		 * @param bounded `true` (default) to use finite length of lines, `false` to use infinite length of lines
		 * @returns The shortest distance to the line
		 */
		function getDistanceToLine(v1: any, v2: any, p: any, bounded: boolean): number;
		/**
		 * Returns the shortest square distance from point `p` to the line defined by its start and end points. If `bounded` is set to `true` (default) the finite length of the lines is used, otherwise the infinite length of the lines is used.
		 * @param v1 First point of the line
		 * @param v2 Second point of the line
		 * @param p Point to get the distance to
		 * @param bounded `true` (default) to use finite length of lines, `false` to use infinite length of lines
		 * @returns The shortest square distance to the line
		 */
		function getDistanceToLineSq(v1: any, v2: any, p: any, bounded: boolean): number;
		/**
		 * Returns the offset coordinate on the line nearest to the given point. `0.0` being on the first point, `1.0` being on the second point, and `u = 0.5` being in the middle between the points.
		 * @param v1 First point of the line
		 * @param v2 Second point of the line
		 * @param p Point to get the nearest offset coordinate from
		 * @returns The offset value relative to the first point of the line.
		 */
		function getNearestOnLine(v1: any, v2: any, p: any): number;
		/**
		 * Returns the coordinate on a line defined by its start and end points as `Vector2D`.
		 * @param v1 First point of the line
		 * @param v2 Second point of the line
		 * @param u Offset coordinate relative to the first point of the line
		 * @returns Point on the line as `Vector2D`
		 */
		function getCoordinatesAt(v1: any, v2: any, u: number): Vector2D;
	}
	class Linedef {
		/**
		 * Copies the properties of this `Linedef` to another `Linedef`.
		 * @param other The `Linedef` to copy the properties to
		 */
		copyPropertiesTo(other: Linedef): void;
		/**
		 * Clears all flags.
		 */
		clearFlags(): void;
		/**
		 * Flips the `Linedef`'s vertex attachments.
		 */
		flipVertices(): void;
		/**
		 * Flips the `Linedef`'s `Sidedef`s.
		 */
		flipSidedefs(): void;
		/**
		 * Flips the `Linedef`'s vertex attachments and `Sidedef`s. This is a shortcut to using both `flipVertices()` and `flipSidedefs()`.
		 */
		flip(): void;
		/**
		 * Gets a `Vector2D` for testing on one side. The `Vector2D` is on the front when `true` is passed, otherwise on the back.
		 * @param front `true` for front, `false` for back
		 * @returns `Vector2D` that's either on the front of back of the Linedef
		 */
		getSidePoint(front: boolean): Vector2D;
		/**
		 * Gets a `Vector2D` that's in the center of the `Linedef`.
		 * @returns `Vector2D` in the center of the `Linedef`
		 */
		getCenterPoint(): Vector2D;
		/**
		 * Automatically sets the blocking and two-sided flags based on the existing `Sidedef`s.
		 */
		applySidedFlags(): void;
		/**
		 * Get a `Vector2D` that's *on* the line, closest to `pos`. `pos` can either be a `Vector2D`, or an array of numbers.
		 * @param pos Point to check against
		 * @returns `Vector2D` that's on the linedef
		 */
		nearestOnLine(pos: any): Vector2D;
		/**
		 * Gets the shortest "safe" squared distance from `pos` to the line. If `bounded` is `true` that means that the not the whole line's length will be used, but `lengthInv` less at the start and end.
		 * @param pos Point to check against
		 * @param bounded `true` if only the finite length of the line should be used, `false` if the infinite length of the line should be used
		 * @returns Squared distance to the line
		 */
		safeDistanceToSq(pos: any, bounded: boolean): number;
		/**
		 * Gets the shortest "safe" distance from `pos` to the line. If `bounded` is `true` that means that the not the whole line's length will be used, but `lengthInv` less at the start and end.
		 * @param pos Point to check against
		 * @param bounded `true` if only the finite length of the line should be used, `false` if the infinite length of the line should be used
		 * @returns Distance to the line
		 */
		safeDistanceTo(pos: any, bounded: boolean): number;
		/**
		 * Gets the shortest squared distance from `pos` to the line.
		 * @param pos Point to check against
		 * @param bounded `true` if only the finite length of the line should be used, `false` if the infinite length of the line should be used
		 * @returns Squared distance to the line
		 */
		distanceToSq(pos: any, bounded: boolean): number;
		/**
		 * Gets the shortest distance from `pos` to the line.
		 * @param pos Point to check against
		 * @param bounded `true` if only the finite length of the line should be used, `false` if the infinite length of the line should be used
		 * @returns Distance to the line
		 */
		distanceTo(pos: any, bounded: boolean): number;
		/**
		 * Tests which side of the `Linedef` `pos` is on. Returns < 0 for front (right) side, > for back (left) side, and 0 if `pos` is on the line.
		 * @param pos Point to check against
		 * @returns < 0 for front (right) side, > for back (left) side, and 0 if `pos` is on the line
		 */
		sideOfLine(pos: any): number;
		/**
		 * Splits the `Linedef` at the given position. This can either be a `Vector2D`, an array of numbers, or an existing `Vertex`. The result will be two lines, from the start `Vertex` of the `Linedef` to `pos`, and from `pos` to the end `Vertex` of the `Linedef`.
		 * @param pos `Vertex` to split by
		 * @returns The newly created `Linedef`
		 */
		split(pos: any): Linedef;
		/**
		 * Deletes the `Linedef`. Note that this will result in unclosed `Sector`s unless it has the same `Sector`s on both sides.
		 */
		delete(): void;
		/**
		 * Returns an `Array` of the `Linedef`'s tags. UDMF only. Supported game configurations only.
		 * @returns `Array` of tags
		 */
		getTags(): number[];
		/**
		 * Adds a tag to the `Linedef`. UDMF only. Supported game configurations only.
		 * @param tag Tag to add
		 * @returns `true` when the tag was added, `false` when the tag already exists
		 */
		addTag(tag: number): boolean;
		/**
		 * Removes a tag from the `Linedef`. UDMF only. Supported game configurations only.
		 * @param tag Tag to remove
		 * @returns `true` when the tag was removed successfully, `false` when the tag did not exist
		 */
		removeTag(tag: number): boolean;
		/**
		 * The linedef's index. Read-only.
		 */
		index: number;
		/**
		 * The linedef's start `Vertex`.
		 */
		start: Vertex;
		/**
		 * The linedef's end `Vertex`.
		 */
		end: Vertex;
		/**
		 * The `Linedef`'s front `Sidedef`. Is `null` when there is no front (should not happen).
		 */
		front: Sidedef;
		/**
		 * The `Linedef`'s back `Sidedef`. Is `null` when there is no back.
		 */
		back: Sidedef;
		/**
		 * The `Line2D` from the `start` to the `end` `Vertex`.
		 */
		line: Line2D;
		/**
		 * If the `Linedef` is selected or not.
		 */
		selected: boolean;
		/**
		 * If the `Linedef` is marked or not. It is used to mark map elements that were created or changed (for example after drawing new geometry).
		 */
		marked: boolean;
		/**
		 * The activation flag. Hexen format only.
		 */
		activate: number;
		/**
		 * `Linedef` flags. It's an object with the flags as properties. In Doom format and Hexen format they are identified by numbers, in UDMF by their name.
		 */
		flags: any;
		/**
		 * `Array` of arguments of the `Linedef`. Number of arguments depends on game config (usually 5). Hexen format and UDMF only.
		 */
		args: number[];
		/**
		 * `Linedef` action.
		 */
		action: number;
		/**
		 * `Linedef` tag. UDMF only.
		 */
		tag: number;
		/**
		 * The `Linedef`'s squared length. Read-only.
		 */
		lengthSq: number;
		/**
		 * The `Linedef`'s length. Read-only.
		 */
		length: number;
		/**
		 * 1.0 / length. Read-only.
		 */
		lengthInv: number;
		/**
		 * The `Linedef`'s angle in degree. Read-only.
		 */
		angle: number;
		/**
		 * The `Linedef`'s angle in radians. Read-only.
		 */
		angleRad: number;
		/**
		 * UDMF fields. It's an object with the fields as properties.
		 */
		fields: any;
	}
	namespace Map {
		/**
		 * Returns the given point snapped to the current grid.
		 * @param pos Point that should be snapped to the grid
		 * @returns Snapped position as `Vector2D`
		 */
		function snappedToGrid(pos: any): Vector2D;
		/**
		 * Returns an `Array` of all `Thing`s in the map.
		 * @returns `Array` of `Thing`s
		 */
		function getThings(): Thing[];
		/**
		 * Returns an `Array` of all `Sector`s in the map.
		 * @returns `Array` of `Sector`s
		 */
		function getSectors(): Sector[];
		/**
		 * Returns an `Array` of all `Sidedef`s in the map.
		 * @returns `Array` of `Sidedef`s
		 */
		function getSidedefs(): Sidedef[];
		/**
		 * Returns an `Array` of all `Linedef`s in the map.
		 * @returns `Array` of `Linedef`s
		 */
		function getLinedefs(): Linedef[];
		/**
		 * Returns an `Array` of all `Vertex` in the map.
		 * @returns `Array` of `Vertex`
		 */
		function getVertices(): Vertex[];
		/**
		 * Stitches marked geometry with non-marked geometry.
		 * @param mergemode Mode to merge by as `MergeGeometryMode`
		 * @returns `true` if successful, `false` if failed
		 */
		function stitchGeometry(mergemode: MergeGeometryMode): boolean;
		/**
		 * Snaps all vertices and things to the map format accuracy. Call this to ensure the vertices and things are at valid coordinates.
		 * @param usepreciseposition `true` if decimal places defined by the map format should be used, `false` if no decimal places should be used
		 */
		function snapAllToAccuracy(usepreciseposition: boolean): void;
		/**
		 * Gets a new tag.
		 * @param usedtags `Array` of tags to skip
		 * @returns The new tag
		 */
		function getNewTag(usedtags: number[]): number;
		/**
		 * Gets multiple new tags.
		 * @param count Number of tags to get
		 * @returns `Array` of the new tags
		 */
		function getMultipleNewTags(count: number): number[];
		/**
		 * Gets the `Linedef` that's nearest to the specified position.
		 * @param pos Position to check against
		 * @param maxrange Maximum range (optional)
		 * @returns Nearest `Linedef`
		 */
		function nearestLinedef(pos: any, maxrange: number): Linedef;
		/**
		 * Gets the `Thing` that's nearest to the specified position.
		 * @param pos Position to check against
		 * @param maxrange Maximum range (optional)
		 * @returns Nearest `Linedef`
		 */
		function nearestThing(pos: any, maxrange: number): Thing;
		/**
		 * Gets the `Vertex` that's nearest to the specified position.
		 * @param pos Position to check against
		 * @param maxrange Maximum range (optional)
		 * @returns Nearest `Vertex`
		 */
		function nearestVertex(pos: any, maxrange: number): Vertex;
		/**
		 * Gets the `Sidedef` that's nearest to the specified position.
		 * @param pos Position to check against
		 * @returns Nearest `Sidedef`
		 */
		function nearestSidedef(pos: any): Sidedef;
		/**
		 * Draws lines. Data has to be an `Array` of `Array` of numbers, `Vector2D`s, `Vector3D`s, or objects with x and y properties. Note that the first and last element have to be at the same positions to make a complete drawing.
		 * @param data `Array` of positions
		 * @returns `true` if drawing was successful, `false` if it wasn't
		 */
		function drawLines(data: any): boolean;
		/**
		 * Sets the `marked` property of all map elements. Can be passed `true` to mark all map elements.
		 * @param mark `false` to set the `marked` property to `false` (default), `true` to set the `marked` property to `true`
		 */
		function clearAllMarks(mark: boolean): void;
		/**
		 * Sets the `marked` property of all vertices. Can be passed `true` to mark all vertices.
		 * @param mark `false` to set the `marked` property to `false` (default), `true` to set the `marked` property to `true`
		 */
		function clearMarkedVertices(mark: boolean): void;
		/**
		 * Sets the `marked` property of all `Thing`s. Can be passed `true` to mark all `Thing`s.
		 * @param mark `false` to set the `marked` property to `false` (default), `true` to set the `marked` property to `true`
		 */
		function clearMarkedThings(mark: boolean): void;
		/**
		 * Sets the `marked` property of all `Linedef`s. Can be passed `true` to mark all `Linedef`s.
		 * @param mark `false` to set the `marked` property to `false` (default), `true` to set the `marked` property to `true`
		 */
		function clearMarkeLinedefs(mark: boolean): void;
		/**
		 * Sets the `marked` property of all `Sidedef`s. Can be passed `true` to mark all `Sidedef`s.
		 * @param mark `false` to set the `marked` property to `false` (default), `true` to set the `marked` property to `true`
		 */
		function clearMarkeSidedefs(mark: boolean): void;
		/**
		 * Sets the `marked` property of all `Sector`s. Can be passed `true` to mark all `Sector`s.
		 * @param mark `false` to set the `marked` property to `false` (default), `true` to set the `marked` property to `true`
		 */
		function clearMarkeSectors(mark: boolean): void;
		/**
		 * Inverts all marks of all map elements.
		 */
		function invertAllMarks(): void;
		/**
		 * Inverts the `marked` property of all vertices.
		 */
		function invertMarkedVertices(): void;
		/**
		 * Inverts the `marked` property of all `Thing`s.
		 */
		function invertMarkedThings(): void;
		/**
		 * Inverts the `marked` property of all `Linedef`s.
		 */
		function invertMarkedLinedefs(): void;
		/**
		 * Inverts the `marked` property of all `Sidedef`s.
		 */
		function invertMarkedSidedefs(): void;
		/**
		 * Inverts the `marked` property of all `Sector`s.
		 */
		function invertMarkedSectors(): void;
		/**
		 * Gets all marked (default) or unmarked vertices.
		 * @param mark `true` to get all marked vertices (default), `false` to get all unmarked vertices
		 * @returns None
		 */
		function getMarkedVertices(mark: boolean): Vertex[];
		/**
		 * Gets all marked (default) or unmarked `Thing`s.
		 * @param mark `true` to get all marked `Thing`s (default), `false` to get all unmarked `Thing`s
		 * @returns None
		 */
		function getMarkedThings(mark: boolean): Thing[];
		/**
		 * Gets all marked (default) or unmarked `Linedef`s.
		 * @param mark `true` to get all marked `Linedef`s (default), `false` to get all unmarked `Linedef`s
		 * @returns None
		 */
		function getMarkedLinedefs(mark: boolean): Linedef[];
		/**
		 * Gets all marked (default) or unmarked `Sidedef`s.
		 * @param mark `true` to get all marked `Sidedef`s (default), `false` to get all unmarked `Sidedef`s
		 * @returns None
		 */
		function getMarkedSidedefs(mark: boolean): Sidedef[];
		/**
		 * Gets all marked (default) or unmarked `Sector`s.
		 * @param mark `true` to get all marked `Sector`s (default), `false` to get all unmarked `Sector`s
		 * @returns None
		 */
		function getMarkedSectors(mark: boolean): Sector[];
		/**
		 * Marks (default) or unmarks all selected vertices.
		 * @param mark `true` to mark all selected vertices (default), `false` to unmark
		 */
		function markSelectedVertices(mark: boolean): void;
		/**
		 * Marks (default) or unmarks all selected `Linedef`s.
		 * @param mark `true` to mark all selected `Linedef`s (default), `false` to unmark
		 */
		function markSelectedLinedefs(mark: boolean): void;
		/**
		 * Marks (default) or unmarks all selected `Sector`s.
		 * @param mark `true` to mark all selected `Sector`s (default), `false` to unmark
		 */
		function markSelectedSectors(mark: boolean): void;
		/**
		 * Marks (default) or unmarks all selected `Thing`s.
		 * @param mark `true` to mark all selected `Thing`s (default), `false` to unmark
		 */
		function markSelectedThings(mark: boolean): void;
		/**
		 * Gets all selected (default) or unselected vertices.
		 * @param selected `true` to get all selected vertices, `false` to get all unselected ones
		 * @returns `Array` of `Vertex`
		 */
		function getSelectedVertices(selected: boolean): Vertex[];
		/**
		 * Get the currently highlighted `Vertex`.
		 * @returns The currently highlighted `Vertex` or `null` if no `Vertex` is highlighted
		 */
		function getHighlightedVertex(): Vertex;
		/**
		 * Gets the currently selected `Vertex`s *or*, if no `Vertex`s are selected, a currently highlighted `Vertex`.
		 * @returns `Array` of `Vertex`
		 */
		function getSelectedOrHighlightedVertices(): Vertex[];
		/**
		 * Gets all selected (default) or unselected `Thing`s.
		 * @param selected `true` to get all selected `Thing`s, `false` to get all unselected ones
		 * @returns `Array` of `Thing`s
		 */
		function getSelectedThings(selected: boolean): Thing[];
		/**
		 * Get the currently highlighted `Thing`.
		 * @returns The currently highlighted `Thing` or `null` if no `Thing` is highlighted
		 */
		function getHighlightedThing(): Thing;
		/**
		 * Gets the currently selected `Thing`s *or*, if no `Thing`s are selected, a currently highlighted `Thing`.
		 * @returns `Array` of `Thing`s
		 */
		function getSelectedOrHighlightedThings(): Thing[];
		/**
		 * Gets all selected (default) or unselected `Sector`s.
		 * @param selected `true` to get all selected `Sector`s, `false` to get all unselected ones
		 * @returns `Array` of `Sector`s
		 */
		function getSelectedSectors(selected: boolean): Sector[];
		/**
		 * Get the currently highlighted `Sector`.
		 * @returns The currently highlighted `Sector` or `null` if no `Sector` is highlighted
		 */
		function getHighlightedSector(): Sector;
		/**
		 * Gets the currently selected `Sector`s *or*, if no `Sector`s are selected, a currently highlighted `Sector`.
		 * @returns `Array` of `Sector`s
		 */
		function getSelectedOrHighlightedSectors(): Sector[];
		/**
		 * Gets all selected (default) or unselected `Linedef`s.
		 * @param selected `true` to get all selected `Linedef`s, `false` to get all unselected ones
		 * @returns `Array` of `Linedef`s
		 */
		function getSelectedLinedefs(selected: boolean): Linedef[];
		/**
		 * Get the currently highlighted `Linedef`.
		 * @returns The currently highlighted `Linedef` or `null` if no `Linedef` is highlighted
		 */
		function getHighlightedLinedef(): Linedef;
		/**
		 * Gets the currently selected `Linedef`s *or*, if no `Linede`f`s are selected, a currently highlighted `Linedef`.
		 * @returns `Array` of `Linedef`s
		 */
		function getSelectedOrHighlightedLinedefs(): Linedef[];
		/**
		 * Gets all `Sidedef`s from the selected `Linedef`s.
		 * @param selected `true` to get all `Sidedef`s of all selected `Linedef`s, `false` to get all `Sidedef`s of all unselected `Linedef`s
		 * @returns `Array` of `Sidedef`
		 */
		function getSidedefsFromSelectedLinedefs(selected: boolean): Sidedef[];
		/**
		 * Gets the `Sidedef`s of the currently selected `Linedef`s *or*, if no `Linedef`s are selected, the `Sidedef`s of the currently highlighted `Linedef`.
		 * @returns `Array` of `Sidedef`s
		 */
		function getSidedefsFromSelectedOrHighlightedLinedefs(): Sidedef[];
		/**
		 * Clears all selected map elements.
		 */
		function clearAllSelected(): void;
		/**
		 * Clears all selected vertices.
		 */
		function clearSelectedVertices(): void;
		/**
		 * Clears all selected `Thing`s.
		 */
		function clearSelectedThings(): void;
		/**
		 * Clears all selected `Sector`s.
		 */
		function clearSelectedSectors(): void;
		/**
		 * Creates a new `Vertex` at the given position. The position can be a `Vector2D` or an `Array` of two numbers.
		 * @param pos Position where the `Vertex` should be created at
		 * @returns The created `Vertex`
		 */
		function createVertex(pos: any): Vertex;
		/**
		 * Creates a new `Thing` at the given position. The position can be a `Vector2D`, `Vector3D`, or an `Array` of two numbers or three numbers (note that the z position only works for game configurations that support vertical pos. A thing type can be supplied optionally.
		 * @param pos Position where the `Thing` should be created at
		 * @param type Thing type (optional)
		 * @returns The new `Thing`
		 */
		function createThing(pos: any, type: number): Thing;
		/**
		 * Joins `Sector`s, keeping lines shared by the `Sector`s. All `Sector`s will be joined with the first `Sector` in the array.
		 * @param sectors `Array` of `Sector`s
		 */
		function joinSectors(sectors: Sector[]): void;
		/**
		 * Merges `Sector`s, deleting lines shared by the `Sector`s. All `Sector`s will be merged into the first `Sector` in the array.
		 * @param sectors `Array` of `Sector`s
		 */
		function mergeSectors(sectors: Sector[]): void;
		/**
		 * `true` if the map is in Doom format, `false` if it isn't. Read-only.
		 */
		let isDoom: boolean;
		/**
		 * `true` if the map is in Hexen format, `false` if it isn't. Read-only.
		 */
		let isHexen: boolean;
		/**
		 * `true` if the map is in UDMF, `false` if it isn't. Read-only.
		 */
		let isUDMF: boolean;
		/**
		 * The map coordinates of the mouse position as a `Vector2D`. Read-only.
		 */
		let mousePosition: Vector2D;
		/**
		 * `VisualCamera` object with information about the position of the camera in visual mode. Read-only.
		 */
		let camera: VisualCamera;
	}
	namespace Map {
		/**
		 * How geometry should be merged when geometry is stitched.
		 */
		enum MergeGeometryMode {
			/**
			 * Merge vertices only
			 */
			CLASSIC,
			/**
			 * Merge vertices and lines
			 */
			MERGE,
			/**
			 * Merge vertices and lines, replacing sector geometry
			 */
			REPLACE,
		}

	}
	class Plane {
		/**
		 * Creates a new `Plane` from a normal and an offset. The normal vector has to be `Vector3D`, `Array`s of 3 numbers, or an object with x, y, and z properties.
		 * @param normal Normal vector of the plane
		 * @param offset Distance of the plane from the origin
		 */
		constructor(normal: any, offset: number);
		/**
		 * Creates a new `Plane` from 3 points. The points have to be `Vector3D`, `Array`s of 3 numbers, or an object with x, y, and z properties.
		 * @param p1 First point
		 * @param p2 Second point
		 * @param p3 Thrid point
		 * @param up `true` if plane is pointing up, `false` if pointing down
		 */
		constructor(p1: any, p2: any, p3: any, up: boolean);
		/**
		 * Checks if the line between `from` and `to` intersects the plane.
		 * @param from `Vector3D` of the start of the line
		 * @param to `Vector3D` of the end of the line
		 * @returns None
		 */
		getIntersection(from: any, to: any): object[];
		/**
		 * Computes the distance between the `Plane` and a point. The given point can be a `Vector3D` or an `Array` of three numbers. A result greater than 0 means the point is on the front of the plane, less than 0 means the point is behind the plane.
		 * @param p Point to compute the distnace to
		 * @returns Distance between the `Plane` and the point as `number`
		 */
		distance(p: any): number;
		/**
		 * Returns the point that's closest to the given point on the `Plane`. The given point can be a `Vector3D` or an `Array` of three numbers.
		 * @param p Point to get the closest position from
		 * @returns Point as `Vector3D` on the plane closest to the given point
		 */
		closestOnPlane(p: any): Vector3D;
		/**
		 * Returns the position on the z axis of the plane for the given point. The given point can be a `Vector2D` or an `Array` of two numbers.
		 * @param p Point to get the z position from
		 * @returns None
		 */
		getZ(p: any): number;
		/**
		 * The plane's normal vector.
		 */
		normal: Vector3D;
		/**
		 * The distance of the plane along the normal vector.
		 */
		offset: number;
		/**
		 * The `a` value of the plane equation. This is the `x` value of the normal vector.
		 */
		a: number;
		/**
		 * The `b` value of the plane equation. This is the `y` value of the normal vector.
		 */
		b: number;
		/**
		 * The `c` value of the plane equation. This is the `z` value of the normal vector.
		 */
		c: number;
		/**
		 * The `d` value of the plane equation. This is the same as the `offset` value.
		 */
		d: number;
	}
	class Sector {
		/**
		 * Returns an `Array` of all `Sidedef`s of the `Sector`.
		 * @returns `Array` of the `Sector`'s `Sidedef`s
		 */
		getSidedefs(): Sidedef[];
		/**
		 * Clears all flags.
		 */
		clearFlags(): void;
		/**
		 * Copies the properties from this `Sector` to another.
		 * @param s the `Sector` to copy the properties to
		 */
		copyPropertiesTo(s: Sector): void;
		/**
		 * Checks if the given point is in this `Sector` or not. The given point can be a `Vector2D` or an `Array` of two numbers.
		 * @param p Point to test
		 * @returns `true` if the point is in the `Sector`, `false` if it isn't
		 */
		intersect(p: any): boolean;
		/**
		 * Joins this `Sector` with another `Sector`. Lines shared between the sectors will not be removed.
		 * @param other Sector to join with
		 */
		join(other: Sector): void;
		/**
		 * Deletes the `Sector` and its `Sidedef`s.
		 */
		delete(): void;
		/**
		 * Gets an array of `Vector2D` arrays, representing the vertices of the triangulated sector. Note that for sectors with islands some triangles may not always have their points on existing vertices.
		 * @returns Array of `Vector2D` arrays
		 */
		getTriangles(): Vector2D[][];
		/**
		 * Gets the floor's slope vector.
		 * @returns The floor's slope normal as a `Vector3D`
		 */
		getFloorSlope(): Vector3D;
		/**
		 * Sets the floor's slope vector. The vector has to be normalized.
		 * @param normal The new slope vector as `Vector3D`
		 */
		setFloorSlope(normal: any): void;
		/**
		 * Gets the ceiling's slope vector.
		 * @returns The ceiling's slope normal as a `Vector3D`
		 */
		getCeilingSlope(): Vector3D;
		/**
		 * Sets the ceiling's slope vector. The vector has to be normalized.
		 * @param normal The new slope vector as `Vector3D`
		 */
		setCeilingSlope(normal: any): void;
		/**
		 * Returns an `Array` of `Vector2D` of label positions for the `Sector`. This are the positions where for example selection number or tags are shown.
		 * @returns `Array` of `Vector2D` of all label positions
		 */
		getLabelPositions(): Vector2D[];
		/**
		 * Returns an `Array` of the `Sector`'s tags. UDMF only. Supported game configurations only.
		 * @returns `Array` of tags
		 */
		getTags(): number[];
		/**
		 * Adds a tag to the `Sector`. UDMF only. Supported game configurations only.
		 * @param tag Tag to add
		 * @returns `true` when the tag was added, `false` when the tag already exists
		 */
		addTag(tag: number): boolean;
		/**
		 * Removes a tag from the `Sector`. UDMF only. Supported game configurations only.
		 * @param tag Tag to remove
		 * @returns `true` when the tag was removed successfully, `false` when the tag did not exist
		 */
		removeTag(tag: number): boolean;
		/**
		 * The `Sector`'s index. Read-only.
		 */
		index: number;
		/**
		 * Floor height of the `Sector`.
		 */
		floorHeight: number;
		/**
		 * Ceiling height of the `Sector`.
		 */
		ceilingHeight: number;
		/**
		 * Floor texture of the `Sector`.
		 */
		floorTexture: string;
		/**
		 * Ceiling texture of the `Sector`.
		 */
		ceilingTexture: string;
		/**
		 * If the `Sector` is selected or not.
		 */
		selected: boolean;
		/**
		 * If the `Sector`'s floor is selected or not. Will always return `true` in classic modes if the `Sector` is selected. Read-only.
		 */
		floorSelected: boolean;
		/**
		 * If the `Sector`'s floor is highlighted or not. Will always return `true` in classic modes if the `Sector` is highlighted. Read-only.
		 */
		floorHighlighted: boolean;
		/**
		 * If the `Sector`'s ceiling is selected or not. Will always return `true` in classic modes if the `Sector` is selected. Read-only.
		 */
		ceilingSelected: boolean;
		/**
		 * If the `Sector`'s ceiling is highlighted or not. Will always return `true` in classic modes if the `Sector` is highlighted. Read-only.
		 */
		ceilingHighlighted: boolean;
		/**
		 * If the `Sector` is marked or not. It is used to mark map elements that were created or changed (for example after drawing new geometry).
		 */
		marked: boolean;
		/**
		 * `Sector` flags. It's an object with the flags as properties. Only available in UDMF.
		 */
		flags: any;
		/**
		 * The `Sector`'s special type.
		 */
		special: number;
		/**
		 * The `Sector`'s tag.
		 */
		tag: number;
		/**
		 * The `Sector`'s brightness.
		 */
		brightness: number;
		/**
		 * The floor's slope offset.
		 */
		floorSlopeOffset: number;
		/**
		 * The ceiling's slope offset.
		 */
		ceilingSlopeOffset: number;
		/**
		 * UDMF fields. It's an object with the fields as properties.
		 */
		fields: any;
	}
	class Sidedef {
		/**
		 * The `Sidedef`'s index. Read-only.
		 */
		index: number;
		/**
		 * `true` if this `Sidedef` is the front of its `Linedef`, otherwise `false`. Read-only.
		 */
		isFront: boolean;
		/**
		 * The `Sector` the `Sidedef` belongs to. Read-only.
		 */
		sector: Sector;
		/**
		 * The `Linedef` the `Sidedef` belongs to. Read-only.
		 */
		line: Linedef;
		/**
		 * The `Sidedef` on the other side of this `Sidedef`'s `Linedef`. Returns `null` if there is no other. Read-only.
		 */
		other: Sidedef;
		/**
		 * The `Sidedef`'s angle in degrees. Read-only.
		 */
		angle: number;
		/**
		 * The `Sidedef`'s angle in radians. Read-only.
		 */
		angleRad: number;
		/**
		 * The x offset of the `Sidedef`'s textures.
		 */
		offsetX: number;
		/**
		 * The y offset of the `Sidedef`'s textures.
		 */
		offsetY: number;
		/**
		 * `Sidedef` flags. It's an object with the flags as properties. Only available in UDMF.
		 */
		flags: any;
		/**
		 * The `Sidedef`'s upper texture.
		 */
		upperTexture: string;
		/**
		 * The `Sidedef`'s middle texture.
		 */
		middleTexture: string;
		/**
		 * The `Sidedef`'s lower texture.
		 */
		lowerTexture: string;
		/**
		 * If the `Sidedef`'s upper part is selected or not. Will always return `true` in classic modes if the parent `Linedef` is selected.
		 */
		upperSelected: boolean;
		/**
		 * If the `Sidedef`'s upper part is highlighted or not. Will always return `true` in classic modes if the parent `Linedef` is selected.
		 */
		upperHighlighted: boolean;
		/**
		 * If the `Sidedef`'s middle part is selected or not. Will always return `true` in classic modes if the parent `Linedef` is selected.
		 */
		middleSelected: boolean;
		/**
		 * If the `Sidedef`'s middle part is highlighted or not. Will always return `true` in classic modes if the parent `Linedef` is selected.
		 */
		middleHighlighted: boolean;
		/**
		 * If the `Sidedef`'s lower part is selected or not. Will always return `true` in classic modes if the parent `Linedef` is selected.
		 */
		lowerSelected: boolean;
		/**
		 * If the `Sidedef`'s lower part is highlighted or not. Will always return `true` in classic modes if the parent `Linedef` is selected.
		 */
		lowerHighlighted: boolean;
		/**
		 * UDMF fields. It's an object with the fields as properties.
		 */
		fields: any;
	}
	class Thing {
		/**
		 * Copies the properties from this `Thing` to another.
		 * @param t The `Thing` to copy the properties to
		 */
		copyPropertiesTo(t: Thing): void;
		/**
		 * Clears all flags.
		 */
		clearFlags(): void;
		/**
		 * Snaps the `Thing`'s position to the grid.
		 */
		snapToGrid(): void;
		/**
		 * Snaps the `Thing`'s position to the map format's accuracy.
		 */
		snapToAccuracy(): void;
		/**
		 * Gets the squared distance between this `Thing` and the given point.
		 * @param pos Point to calculate the squared distance to.
		 * @returns Distance to `pos`
		 */
		distanceToSq(pos: any): number;
		/**
		 * Gets the distance between this `Thing` and the given point. The point can be either a `Vector2D` or an array of numbers.
		 * @param pos Point to calculate the distance to.
		 * @returns Distance to `pos`
		 */
		distanceTo(pos: any): number;
		/**
		 * Deletes the `Thing`.
		 */
		delete(): void;
		/**
		 * Determines and returns the `Sector` the `Thing` is in.
		 * @returns The `Sector` the `Thing` is in
		 */
		getSector(): Sector;
		/**
		 * Index of the `Thing`. Read-only.
		 */
		index: number;
		/**
		 * Type of the `Thing`.
		 */
		type: number;
		/**
		 * Angle of the `Thing` in degrees, see https://doomwiki.org/wiki/Angle.
		 */
		angle: number;
		/**
		 * Angle of the `Thing` in radians.
		 */
		angleRad: number;
		/**
		 * `Array` of arguments of the `Thing`. Number of arguments depends on game config (usually 5). Hexen format and UDMF only.
		 */
		args: number[];
		/**
		 * `Thing` action. Hexen and UDMF only.
		 */
		action: number;
		/**
		 * `Thing` tag. UDMF only.
		 */
		tag: number;
		/**
		 * If the `Thing` is selected or not.
		 */
		selected: boolean;
		/**
		 * If the `Thing` is marked or not. It is used to mark map elements that were created or changed (for example after drawing new geometry).
		 */
		marked: boolean;
		/**
		 * `Thing` flags. It's an object with the flags as properties. In Doom format and Hexen format they are identified by numbers, in UDMF by their name.
		 */
		flags: any;
		/**
		 * Position of the `Thing`. It's an object with `x`, `y`, and `z` properties. The latter is only relevant in Hexen format and UDMF.
		 */
		position: any;
		/**
		 * Pitch of the `Thing` in degrees. Only valid for supporting game configurations.
		 */
		pitch: number;
		/**
		 * Roll of the `Thing` in degrees. Only valid for supporting game configurations.
		 */
		roll: number;
		/**
		 * UDMF fields. It's an object with the fields as properties.
		 */
		fields: any;
	}
	/**
		 * Set the progress of the script in percent. Value can be between 0 and 100. Also shows the script running dialog.
		 * @param value Number between 0 and 100
		 */
		function setProgress(value: number): void;
	/**
		 * Adds a line to the script log. Also shows the script running dialog.
		 * @param text Line to add to the script log
		 */
		function log(text: any): void;
	/**
		 * Shows a message box with an "OK" button.
		 * @param message Message to show
		 */
		function showMessage(message: any): void;
	/**
		 * Shows a message box with an "Yes" and "No" button.
		 * @param message Message to show
		 * @returns true if "Yes" was clicked, false if "No" was clicked
		 */
		function showMessageYesNo(message: any): boolean;
	/**
		 * Exist the script prematurely without undoing its changes.
		 * @param s Text to show in the status bar (optional)
		 */
		function exit(s: string): void;
	/**
		 * Exist the script prematurely with undoing its changes.
		 * @param s Text to show in the status bar (optional)
		 */
		function die(s: string): void;
	class Vector2D {
		/**
		 * Creates a new `Vector2D` from x and y coordinates
		 * @param x The x coordinate
		 * @param y The y coordinate
		 */
		constructor(x: number, y: number);
		/**
		 * Creates a new `Vector2D` from a point.
		 * @param v The vector to create the `Vector2D` from
		 */
		constructor(v: any);
		/**
		 * Returns the perpendicular to the `Vector2D`.
		 * @returns The perpendicular as `Vector2D`
		 */
		getPerpendicular(): Vector2D;
		/**
		 * Returns a `Vector2D` with the sign of all components.
		 * @returns A `Vector2D` with the sign of all components
		 */
		getSign(): Vector2D;
		/**
		 * Returns the angle of the `Vector2D` in radians.
		 * @returns The angle of the `Vector2D` in radians
		 */
		getAngleRad(): number;
		/**
		 * Returns the angle of the `Vector2D` in degree.
		 * @returns The angle of the `Vector2D` in degree
		 */
		getAngle(): number;
		/**
		 * Returns the length of the `Vector2D`.
		 * @returns The length of the `Vector2D`
		 */
		getLength(): number;
		/**
		 * Returns the square length of the `Vector2D`.
		 * @returns The square length of the `Vector2D`
		 */
		getLengthSq(): number;
		/**
		 * Returns the normal of the `Vector2D`.
		 * @returns The normal as `Vector2D`
		 */
		getNormal(): Vector2D;
		/**
		 * Returns the transformed vector as `Vector2D`.
		 * @param offsetx X offset
		 * @param offsety Y offset
		 * @param scalex X scale
		 * @param scaley Y scale
		 * @returns The transformed vector as `Vector2D`
		 */
		getTransformed(offsetx: number, offsety: number, scalex: number, scaley: number): Vector2D;
		/**
		 * Returns the inverse transformed vector as `Vector2D`.
		 * @param invoffsetx X offset
		 * @param invoffsety Y offset
		 * @param invscalex X scale
		 * @param invscaley Y scale
		 * @returns The inverse transformed vector as `Vector2D`
		 */
		getInverseTransformed(invoffsetx: number, invoffsety: number, invscalex: number, invscaley: number): Vector2D;
		/**
		 * Returns the rotated vector as `Vector2D`.
		 * @param theta Angle in degree to rotate by
		 * @returns The rotated `Vector2D`
		 */
		getRotated(theta: number): Vector2D;
		/**
		 * Returns the rotated vector as `Vector2D`.
		 * @param theta Angle in radians to rotate by
		 * @returns The rotated `Vector2D`
		 */
		getRotatedRad(theta: number): Vector2D;
		/**
		 * Checks if the `Vector2D` is finite or not.
		 * @returns `true` if `Vector2D` is finite, otherwise `false`
		 */
		isFinite(): boolean;
		/**
		 * The `x` value of the vector.
		 */
		x: number;
		/**
		 * The `y` value of the vector.
		 */
		y: number;
	}
	namespace Vector2D {
		/**
		 * Returns the dot product of two `Vector2D`s.
		 * @param a First `Vector2D`
		 * @param b Second `Vector2D`
		 * @returns The dot product of the two vectors
		 */
		function dotProduct(a: Vector2D, b: Vector2D): number;
		/**
		 * Returns the cross product of two `Vector2D`s.
		 * @param a First `Vector2D`
		 * @param b Second `Vector2D`
		 * @returns Cross product of the two vectors as `Vector2D`
		 */
		function crossProduct(a: any, b: any): Vector2D;
		/**
		 * Reflects a `Vector2D` over a mirror `Vector2D`.
		 * @param v `Vector2D` to reflect
		 * @param m Mirror `Vector2D`
		 * @returns The reflected vector as `Vector2D`
		 */
		function reflect(v: any, m: any): Vector2D;
		/**
		 * Returns a reversed `Vector2D`.
		 * @param v `Vector2D` to reverse
		 * @returns The reversed vector as `Vector2D`
		 */
		function reversed(v: any): Vector2D;
		/**
		 * Creates a `Vector2D` from an angle in radians,
		 * @param angle Angle in radians
		 * @returns Vector as `Vector2D`
		 */
		function fromAngleRad(angle: number): Vector2D;
		/**
		 * Creates a `Vector2D` from an angle in degrees,
		 * @param angle Angle in degrees
		 * @returns Vector as `Vector2D`
		 */
		function fromAngle(angle: number): Vector2D;
		/**
		 * Returns the angle between two `Vector2D`s in radians
		 * @param a First `Vector2D`
		 * @param b Second `Vector2D`
		 * @returns Angle in radians
		 */
		function getAngleRad(a: any, b: any): number;
		/**
		 * Returns the angle between two `Vector2D`s in degrees.
		 * @param a First `Vector2D`
		 * @param b Second `Vector2D`
		 * @returns Angle in degrees
		 */
		function getAngle(a: any, b: any): number;
		/**
		 * Returns the square distance between two `Vector2D`s.
		 * @param a First `Vector2D`
		 * @param b Second `Vector2D`
		 * @returns The squared distance
		 */
		function getDistanceSq(a: any, b: any): number;
		/**
		 * Returns the distance between two `Vector2D`s.
		 * @param a First `Vector2D`
		 * @param b Second `Vector2D`
		 * @returns The distance
		 */
		function getDistance(a: any, b: any): number;
	}
	class Vector3D {
		/**
		 * Creates a new `Vector3D` from x and y coordinates
		 * @param x The x coordinate
		 * @param y The y coordinate
		 * @param z The z coordinate
		 */
		constructor(x: number, y: number, z: number);
		/**
		 * Creates a new `Vector3D` from a point.
		 * @param v The vector to create the `Vector3D` from
		 */
		constructor(v: any);
		/**
		 * Returns the x/y angle of the `Vector3D` in radians.
		 * @returns The x/y angle of the `Vector3D` in radians
		 */
		getAngleXYRad(): number;
		/**
		 * Returns the angle of the `Vector3D` in degrees.
		 * @returns The angle of the `Vector3D` in degrees
		 */
		getAngleXY(): number;
		/**
		 * Returns the z angle of the `Vector3D` in radians.
		 * @returns The z angle of the `Vector3D` in radians
		 */
		getAngleZRad(): number;
		/**
		 * Returns the z angle of the `Vector3D` in degrees.
		 * @returns The z angle of the `Vector3D` in degrees
		 */
		getAngleZ(): number;
		/**
		 * Returns the length of the `Vector3D`.
		 * @returns The length of the `Vector3D`
		 */
		getLength(): number;
		/**
		 * Returns the square length of the `Vector3D`.
		 * @returns The square length of the `Vector3D`
		 */
		getLengthSq(): number;
		/**
		 * Returns the normal of the `Vector3D`.
		 * @returns The normal as `Vector3D`
		 */
		getNormal(): Vector3D;
		/**
		 * Return the scaled `Vector3D`.
		 * @param scale Scale, where 1.0 is unscaled
		 * @returns The scaled `Vector3D`
		 */
		getScaled(scale: number): Vector3D;
		/**
		 * Checks if the `Vector3D` is normalized or not.
		 * @returns `true` if `Vector3D` is normalized, otherwise `false`
		 */
		isNormalized(): boolean;
		/**
		 * Checks if the `Vector3D` is finite or not.
		 * @returns `true` if `Vector3D` is finite, otherwise `false`
		 */
		isFinite(): boolean;
		/**
		 * The `x` value of the vector.
		 */
		x: number;
		/**
		 * The `y` value of the vector.
		 */
		y: number;
		/**
		 * The `z` value of the vector.
		 */
		z: number;
	}
	namespace Vector3D {
		/**
		 * Returns the dot product of two `Vector3D`s.
		 * @param a First `Vector3D`
		 * @param b Second `Vector3D`
		 * @returns The dot product of the two vectors
		 */
		function dotProduct(a: Vector3D, b: Vector3D): number;
		/**
		 * Returns the cross product of two `Vector3D`s.
		 * @param a First `Vector3D`
		 * @param b Second `Vector3D`
		 * @returns Cross product of the two vectors as `Vector3D`
		 */
		function crossProduct(a: any, b: any): Vector3D;
		/**
		 * Reflects a `Vector3D` over a mirror `Vector3D`.
		 * @param v `Vector3D` to reflect
		 * @param m Mirror `Vector3D`
		 * @returns The reflected vector as `Vector3D`
		 */
		function reflect(v: any, m: any): Vector3D;
		/**
		 * Returns a reversed `Vector3D`.
		 * @param v `Vector3D` to reverse
		 * @returns The reversed vector as `Vector3D`
		 */
		function reversed(v: any): Vector3D;
		/**
		 * Creates a `Vector3D` from an angle in radians
		 * @param angle Angle on the x/y axes in radians
		 * @returns Vector as `Vector3D`
		 */
		function fromAngleXYRad(angle: number): Vector3D;
		/**
		 * Creates a `Vector3D` from an angle in radians,
		 * @param angle Angle on the x/y axes in degrees
		 * @returns Vector as `Vector3D`
		 */
		function fromAngleXY(angle: number): Vector3D;
		/**
		 * Creates a `Vector3D` from two angles in radians
		 * @param anglexy Angle on the x/y axes in radians
		 * @param anglez Angle on the z axis in radians
		 * @returns Vector as `Vector3D`
		 */
		function fromAngleXYZRad(anglexy: number, anglez: number): Vector3D;
		/**
		 * Creates a `Vector3D` from two angles in degrees
		 * @param anglexy Angle on the x/y axes in radians
		 * @param anglez Angle on the z axis in radians
		 * @returns Vector as `Vector3D`
		 */
		function fromAngleXYZ(anglexy: number, anglez: number): Vector3D;
	}
	class Vertex {
		/**
		 * Gets all `Linedefs` that are connected to this `Vertex`.
		 * @returns Array of linedefs
		 */
		getLinedefs(): Linedef[];
		/**
		 * Copies the properties from this `Vertex` to another.
		 * @param v the vertex to copy the properties to
		 */
		copyPropertiesTo(v: Vertex): void;
		/**
		 * Gets the squared distance between this `Vertex` and the given point.
		 * @param pos Point to calculate the squared distance to.
		 * @returns Squared distance to `pos`
		 */
		distanceToSq(pos: any): number;
		/**
		 * Gets the distance between this `Vertex` and the given point.
		 * @param pos Point to calculate the distance to.
		 * @returns Distance to `pos`
		 */
		distanceTo(pos: any): number;
		/**
		 * Returns the `Linedef` that is connected to this `Vertex` that is closest to the given point.
		 * @param pos Point to get the nearest `Linedef` connected to this `Vertex` from
		 * @returns None
		 */
		nearestLinedef(pos: any): Linedef;
		/**
		 * Snaps the `Vertex`'s position to the map format's accuracy.
		 */
		snapToAccuracy(): void;
		/**
		 * Snaps the `Vertex`'s position to the grid.
		 */
		snapToGrid(): void;
		/**
		 * Joins this `Vertex` with another `Vertex`, deleting this `Vertex` and keeping the other.
		 * @param other `Vertex` to join with
		 */
		join(other: Vertex): void;
		/**
		 * Deletes the `Vertex`. Note that this can result in unclosed sectors.
		 */
		delete(): void;
		/**
		 * The vertex index. Read-only.
		 */
		index: number;
		/**
		 * Position of the `Vertex`. It's an object with `x` and `y` properties.
		 */
		position: any;
		/**
		 * If the `Vertex` is selected or not.
		 */
		selected: boolean;
		/**
		 * If the `Vertex` is marked or not. It is used to mark map elements that were created or changed (for example after drawing new geometry).
		 */
		marked: boolean;
		/**
		 * The ceiling z position of the `Vertex`. Only available in UDMF. Only available for supported game configurations.
		 */
		ceilingZ: number;
		/**
		 * The floor z position of the `Vertex`. Only available in UDMF. Only available for supported game configurations.
		 */
		floorZ: number;
		/**
		 * UDMF fields. It's an object with the fields as properties.
		 */
		fields: any;
	}
	class VisualCamera {
		/**
		 * Position of the camera as `Vector3D`. Read-only.
		 */
		position: Vector3D;
		/**
		 * Angle of the camera on the X/Y axes. Read-only.
		 */
		angleXY: number;
		/**
		 * Angle of the camera on the Z axis. Read-only.
		 */
		angleZ: number;
	}
	class QueryOptions {
		/**
		 * Initializes a new `QueryOptions` object.
		 */
		constructor();
		/**
		 * Adds a parameter to query
		 * @param name Name of the variable that the queried value is stored in
		 * @param description Textual description of the parameter
		 * @param type UniversalType value of the parameter
		 * @param defaultvalue Default value of the parameter
		 */
		addOption(name: string, description: string, type: number, defaultvalue: any): void;
		/**
		 * Adds a parameter to query
		 * @param name Name of the variable that the queried value is stored in
		 * @param description Textual description of the parameter
		 * @param type UniversalType value of the parameter
		 * @param defaultvalue Default value of the parameter
		 * @param enumvalues *missing*
		 */
		addOption(name: string, description: string, type: number, defaultvalue: any, enumvalues: any): void;
		/**
		 * Removes all parameters
		 */
		clear(): void;
		/**
		 * Queries all parameters. Options a window where the user can enter values for the options added through `addOption()`.
		 * @returns True if OK was pressed, otherwise false
		 */
		query(): boolean;
		/**
		 * Object containing all the added options as properties.
		 */
		options: any;
	}
}
