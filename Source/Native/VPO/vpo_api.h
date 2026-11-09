//------------------------------------------------------------------------
//  Visplane Overflow Library
//------------------------------------------------------------------------
//
//  Copyright (C) 1993-1996 Id Software, Inc.
//  Copyright (C) 2005      Simon Howard
//  Copyright (C) 2012-2014 Andrew Apted
//
//  This program is free software; you can redistribute it and/or
//  modify it under the terms of the GNU General Public License
//  as published by the Free Software Foundation; either version 2
//  of the License, or (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//------------------------------------------------------------------------

#ifndef __VPO_API_H__
#define __VPO_API_H__

#ifdef __cplusplus
extern "C" {
#endif

typedef void* VPOContext;

VPOContext VPO_NewContext();
void VPO_DeleteContext(VPOContext ctx);

// return error message when something fails
// (this will be a static buffer, so is not guaranteed to remain valid
//  after any other API call)
const char *VPO_GetError(VPOContext ctx);

// try to load a wad file
// returns 0 on success, negative value on error
int VPO_LoadWAD(VPOContext ctx, const char *wad_filename);

// free all data associated with the wad file
// can be safely called without any loaded wad file
void VPO_FreeWAD(VPOContext ctx);

// retrieve the map names in the wad, one at a time
// index starts at 0
// returns NULL when index is past the end of the list
// NOTE: return pointer is a static buffer, and is NOT guaranteed to
//       remain valid once this function is called again
const char * VPO_GetMapName(VPOContext ctx, unsigned int index, bool *is_hexen = NULL);

// try to open a map from the current wad file
// returns 0 on success, negative value on error
int VPO_OpenMap(VPOContext ctx, const char *map_name, bool *is_hexen = NULL);

// free all data associated with a map
// can be safely called without any opened map
void VPO_CloseMap(VPOContext ctx);

// retrieve the linedefs in the current map, one at a time
// index starts at 0
// returns number of sides (0 to 2), or -1 for invalid index
int VPO_GetLinedef(VPOContext ctx, unsigned int index, int *x1, int *y1, int *x2, int *y2);

// retrieve a seg for the current map
// index starts at 0
// side value will be 0 for front/right, 1 for back/left
// returns a value >= 0 if valid, otherwise -1
int VPO_GetSeg(VPOContext ctx, unsigned int index, int *linedef, int *side, int *x1, int *y1, int *x2, int *y2);

// retrieve the bounding box of the map
void VPO_GetBBox(VPOContext ctx, int *x1, int *y1, int *x2, int *y2);

// open or close all sectors which seem to be doors
// dir must be > 0 to open them, or -1 to close them
void VPO_OpenDoorSectors(VPOContext ctx, int dir);

// test a spot and angle, returning the number of visplanes
// dz is the height above the floor (or offset from ceiling if < 0)
// angle is in degrees (0 to 360), 0 is east, 90 is north
// returns RESULT_OK on success, or a negative value spot was in the
// void or an error occurred -- see the RESULT_* values below
//
// the num_xxx parameters point to variables which get _updated_ by
// this call (i.e. new value is maximum of old value + checked value).
// hence you need to set those variables to zero before the first
// call at a particular (X Y) location.
//
// RESULT_OVERFLOW means that an internal limit overflowed (which are
// four times or more the actual DOOM limits).

#define RESULT_OK         0
#define RESULT_BAD_Z     -1
#define RESULT_IN_VOID   -2
#define RESULT_OVERFLOW  -3

int VPO_TestSpot(VPOContext ctx,
                 int x, int y, int dz, int angle,
                 int *num_visplanes,
                 int *num_drawsegs,
                 int *num_openings,
                 int *num_solidsegs);

#ifdef __cplusplus
}
#endif

#endif  /* __VPO_API_H__ */

