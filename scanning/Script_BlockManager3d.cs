//This system takes an area and scans it for bricks
//The areas with bricks, are then separated into rectangular cubes
//as are the free space
//it is hoped that this will simplify pathfinding, or some other related thing

$dimensions["xy"] = 0;
$dimensions["xz"] = 1;
$dimensions["xz"] = 2;
$dimensions["x"] = "1 0 0";
$dimensions["y"] = "0 1 0";
$dimensions["z"] = "0 0 1";
$dimensionIds["x"] = 0;
$dimensionIds["y"] = 1;
$dimensionIds["z"] = 2;
$dimensionScale["x"] = 0.5;
$dimensionScale["y"] = 0.5;
$dimensionScale["z"] = 0.2;
$spaces["open"] = 0;
$spaces["filled"] = 1;

function BlockManager3d::onAdd(%this, %obj)
{
	//position should be set to the -x and -y 'ist brick spot in the area
	//width should be the x
	//length should be the y
	//actually width and length will be extent
	//dimension = "x y" or "x z" or "y z"
	//containerRayCast(%start, %end, mask, exempt0, exempt1, exempt2)
	
	//three types of spaces
	//unknown, filled, taken (could more be added?)
	%obj.numOpenSpaces = 0;
	%obj.numFilledSpaces = 0;
	
	//hackish, but I want to test this script out
	%obj.width = getWord(%obj.extent, 0);
	%obj.length = getWord(%obj.extent, 1);
	%obj.height = getWord(%obj.extent, 2);
	
	%obj.x = 0;
	%obj.y = 0;
	%obj.z = 0;
	%obj.heightX = 0;
	%obj.heightY = 0;
	
	%obj.paused = false;
	
	%obj.spaceWidth = 0;
	%obj.spaceLength = 0;
	%obj.spaceHeight = 0;
	%obj.spaceType = -1;
	
	%obj.maxWidth = 0;
	%obj.maxLength = 0;
	
	%obj.curBrick = 0;
	%obj.curWidth = 0;
	%obj.curLength = 0;
	%obj.curHeight = 1;
}

//indexes the area the block manager handles
function BlockManager3d::indexSpaces(%obj)
{
	//this function will take any 'unknown' spaces and index them
	%d1 = $dimensions[getWord(%obj.dimensions, 0)];
	%d1Id = $dimensionIds[getWord(%obj.dimensions, 0)];
	%d1Scale = $dimensionScale[getWord(%obj.dimensions, 0)];
	%d2 = $dimensions[getWord(%obj.dimensions, 1)];
	%d2Id = $dimensionIds[getWord(%obj.dimensions, 1)];
	%d2Scale = $dimensionScale[getWord(%obj.dimensions, 1)];
	%d3 = $dimensions[getWord(%obj.dimensions, 2)];
	%d3Id = $dimensionIds[getWord(%obj.dimensions, 2)];
	%d3Scale = $dimensionScale[getWord(%obj.dimensions, 2)];
	
	//thinking in terms of x and y is easier, so use those as variable names (but could really be using any dimension)
	for (%z = 0; %z < %obj.height; %z++)
	{
		for (%x = 0; %x < %obj.width; %x++) //loop through each row
		{
			%y = 0;
			%y = %obj.nextOpenSpace(%x SPC %y SPC %z, 1);
			while (%y != -1) //go through the row until an unindexed spot is found
			{
				%length = 0;
				%width = 0;
				%height = 1; //oh dear, help me
				%curRow = 0;
				//at this point we know we need to make a new block, find out what type and set up for the block creation loop
				//
				//we perform a raycast to find the type (and if it's an open space, then its length)
				
				//find the length we need to do the raycast
				%endY = %obj.nextFilledSpace(%x SPC %y SPC %z, 1);
				%endX = %obj.nextFilledSpace(%x SPC %y SPC %z, 0);
				if (%endY == -1)
					%endY = %obj.length;
				if (%endX == -1)
					%endX = %obj.width;
					
				%maxLength = %endY - %y;
				%maxWidth = %endX - %x;
				//%ray = containerRayCast(%obj.getWorldPosition(%x SPC %y), %obj.getWorldPosition(%x SPC %endY), $TypeMasks::FxBrickAlwaysObjectType);
				
				//now interpret results to find what type of space to make, possibly its length
				%brick = %obj.checkBrick(%x SPC %y SPC %z);
				if (!isObject(%brick))
				{
					//found nothing, open space
					%type = $spaces["open"];
				}
				else
				{
					%brickBox = %brick.getWorldBox();
					//found something, filled space
					%type = $spaces["filled"];
					%curRow = 0;
					%curLength = (getWord(%brickBox, %d2Id + 3) - getWord(%brickBox, %d2Id))*2;
					if (%curLength > %maxLength)
						%curLength = %maxLength;
					//%curBrick = getWord(%ray, 0);
					//%hitLoc = %obj.getGridPosition(getWord(%curBrick.getWorldBox(), %d2Id) + 0.5);
				}
				//%times = 0;
				while (%width < 1 || %length < 1)
				{
					%times++;
					if (%times > 7000)
					{
						return;
					}
					//do something different depending on if we are making an open or filled space
					if (%type == $spaces["filled"])
					{
						//always start at a spot where we need to check further down the y dimension
						//but if we don't know any bricks on this row yet, then we need to do a box check
						if (%curLength < 1)
						{
							%brick = %obj.checkBrick(%x + %curRow SPC %y SPC %z);
						}
						else
						{
							//do a raycast on the y dimension
							//%rayStart = %obj.getWorldPosition(%x + %curRow SPC %y + %curLength - 1 SPC %z); //start inside the last brick
							//%rayEnd = %obj.getWorldPosition(%x + %curRow SPC %y + %curLength SPC %z); //end in the suspected spot
							//%ray = containerRayCast(%rayStart, %rayEnd, $TypeMasks::FxBrickAlwaysObjectType, %brick);
							//%brick = getWord(%ray, 0);
							%brick = %obj.checkBrick(%x + %curRow SPC %y + %curLength SPC %z);
						}
						if (isObject(%brick))
						{
							//if we found something, update our current length
							%brickBox = %brick.getWorldBox();
							%curLength += (getWord(%brickBox, %d2Id + 3) - getWord(%brickBox, %d2Id))*2;
							if (%curLength >= %maxLength)
							{
								%curLength = %maxLength;
								if (%length <= 0)
									%length = %curLength;
							}
							if (%length > 0 && %curLength >= %length)
							{
								//time to move to the next row
								%curLength = 0;
								%curRow++;
								if (%curRow >= %maxWidth)
									%width = %curRow;
							}
						}
						else
						{
							//if we didn't find anything, our curLength is the max length for this row
							//check it with the current length value or make it the length if necessary
							if (%length <= 0)
							{
								//the length will be this, now move to the next row
								%length = %curLength;
								%curRow++;
								if (%curRow >= %maxWidth)
									%width = %curRow;
								%curLength = 0;
								//now need to check for the first brick on the next row
							}
							else
							{
								//this row is not large enough to be included in this rectangle, we have the info we need to form this space
								%width = %curRow;
							}
						}
					}
					else
					{
						//at this point we just need to check this row for bricks
						//always use a raycast
						if (%length <= 0)
							%rayLength = %maxLength - 1;
						else
							%rayLength = %length - 1;
						%rayStart = %obj.getWorldPosition(%x + %curRow SPC %y SPC %z); //start inside the last brick
						%rayEnd = %obj.getWorldPosition(%x + %curRow SPC %y + %rayLength SPC %z); //end in the suspected spot
						
						%ray = containerRayCast(%rayStart, %rayEnd, $TypeMasks::FxBrickAlwaysObjectType, %brick);
						%brick = getWord(%ray, 0);
						if (isObject(%brick))
						{
							if (%curRow <= 0)
							{
								//if this is the first row, then we now have the length of this space
								%brickBox = %brick.getWorldBox();
								%radius = $dimensionScale[getWord(%obj.dimensions, 1)]/2;
								%curLength = ((getWord(%brickBox, %d2Id) + %radius) - getWord(%obj.getWorldPosition(%x + %curRow SPC %y SPC %z), %d2Id))*2; //first brick stud - start of row, convert to brick units
								%length = %curLength;
								
								//next line
								%curRow++;
								if (isObject(%obj.checkBrick(%x + %curRow SPC %y SPC %z)))
									%width = %curRow;
							}
							else
							{
								//this row is too small, we now know the width
								%width = %curRow;
							}
							//otherwise, this row has bricks in it 
						}
						else
						{
							if (%length <= 0)
								%length = %maxLength;
							
							%curRow++;
							
							if (isObject(%obj.checkBrick(%x + %curRow SPC %y SPC %z)))
								%width = %curRow;
						}
					}
					
					//I guess this check can go here >.>
					if (%x + %curRow > %obj.width)
						%width = %curRow;
				}
				//now we need to find the height
				%brick = "";
				%height = 0;
				if (%type == $spaces["filled"])
				{
					%brick = %obj;
					while (isObject(%brick) && %z + %height <= %obj.height)
					{
						%height++;
						for (%scanX = 0; (%scanX < %width && isObject(%brick)); %scanX++)
						{
							for (%scanY = 0; (%scanY < %length && isObject(%brick)); %scanY++)
							{
								%brick = %obj.checkBrick(%x + %scanX SPC %y + %scanY SPC %z + %height);
							}
						}
					}
				}
				else
				{
					//open spaces are easy to check, we can just do a big check
					//if any bricks are found, we don't increment the layer
					while (!isObject(%brick) && %z + %height < %obj.height)
					{
						%height++;
						%scanCenter = VectorAdd(%obj.getWorldPosition(%x + %width - 1 SPC %y + %length - 1 SPC %z + %height), %obj.getWorldPosition(%x SPC %y SPC %z + %height));
						%scanCenter = VectorScale(%scanCenter, 0.5);
						initContainerBoxSearch(%scanCenter, %width*%d1Scale-%d1Scale SPC %length*%d2Scale-%d2Scale SPC %d3Scale/2, $TypeMasks::FxBrickAlwaysObjectType); //this is hard coded to only work with x y z
						%brick = containerSearchNext();
					}
				}
				//we are now able to make a space!
				%obj.addSpace(%type, %x SPC %y SPC %z, %width SPC %length SPC %height);
				
				//set up for the next loop around
				%y = %obj.nextOpenSpace(%x SPC %y SPC %z, 1);
			}
		}
	}
}

//basically, only reset variables once done using them, that way it's easy to pick this back up
function BlockManager3d::asyncIndexSpaces(%obj, %callback)
{
	//constants
	%dx = $dimensions[getWord(%obj.dimensions, 0)];
	%dxId = $dimensionIds[getWord(%obj.dimensions, 0)];
	%dxScale = $dimensionScale[getWord(%obj.dimensions, 0)];
	%dy = $dimensions[getWord(%obj.dimensions, 1)];
	%dyId = $dimensionIds[getWord(%obj.dimensions, 1)];
	%dyScale = $dimensionScale[getWord(%obj.dimensions, 1)];
	%dz = $dimensions[getWord(%obj.dimensions, 2)];
	%dzId = $dimensionIds[getWord(%obj.dimensions, 2)];
	%dzScale = $dimensionScale[getWord(%obj.dimensions, 2)];
	%width = getWord(%obj.extent, 0);
	%length = getWord(%obj.extent, 1);
	%height = getWord(%obj.extent, 2);
	
	%obj.paused = false;
	%MAXTIME = 20;
	
	while (!%obj.paused && %obj.z < %height)
	{
		while (!%obj.paused && %obj.x < %width)
		{
			while (!%obj.paused && (%obj.y = %obj.nextOpenSpace(%obj.x SPC %obj.y SPC %obj.z, %dyId)) != -1)
			{
				if (%obj.spaceType == -1) //only setup if we need to
				{
					//at this point we now have a space to map
					//find its type
					%obj.maxWidth = %obj.nextFilledSpace(%obj.x SPC %obj.y SPC %obj.z, %dxId);
					%obj.maxLength = %obj.nextFilledSpace(%obj.x SPC %obj.y SPC %obj.z, %dyId);
					%obj.maxHeight = %height - %obj.z;
					
					if (%obj.maxWidth < 1)
						%obj.maxWidth = getWord(%obj.extent, %dxId);
					if (%obj.maxLength < 1)
						%obj.maxLength = getWord(%obj.extent, %dyId);
					%obj.maxWidth = %obj.maxWidth - %obj.x;
					%obj.maxLength = %obj.maxLength - %obj.y;
					%obj.curBrick = %obj.checkBrick(%obj.x SPC %obj.y SPC %obj.z);
					if (isObject(%obj.curBrick))
					{
						//is a filled space
						%obj.spaceType = $spaces["filled"];
						%obj.curLength = %obj.getBrickMax(%obj.curBrick, %dyId) - %obj.y; //update the current length
					}
					else
					{
						//is an open space
						%obj.spaceType = $spaces["open"];
					}
				}
					
				//now find the width and length
				while (!%obj.paused && (%obj.spaceWidth < 1 || %obj.spaceLength < 1))
				{
					//different method depending on the space types
					if (%obj.spaceType == $spaces["filled"])
					{
						//check the next brick, or special case of a new row, check the first spot
						if (%obj.curLength > 0)
						{
							%obj.curBrick = %obj.checkBrick(%obj.x + %obj.curWidth SPC %obj.y + %obj.curLength SPC %obj.z);
						}
						else
						{
							%obj.curBrick = %obj.checkBrick(%obj.x + %obj.curWidth SPC %obj.y + %obj.curLength SPC %obj.z);
						}
						if (%obj.spaceLength < 1) //we still need to find the length, should also be on first row
						{
							if (!isObject(%obj.curBrick))
							{
								//alright, the curlength is now the length
								%obj.spaceLength = %obj.curLength;
								%obj.curLength = 0;
								%obj.curWidth++;
							}
							else
							{
								//just increment the current length
								%obj.curLength = %obj.getBrickMax(%obj.curBrick, %dyId) - %obj.y;
							}
						}
						else//we already have the length, now we're just waiting until we reach a row that doesn't fill our needs, or the maxrow
						{
							if (!isObject(%obj.curBrick))
							{
								//easy, the current width is the correct one
								%obj.spaceWidth = %obj.curWidth;
								%obj.curLength = 0;
								%obj.curWidth = 0;
							}
							else
							{
								//increment current length, if it reaches the space's length, then next row
								%obj.curLength = %obj.getBrickMax(%obj.curBrick, %dyId) - %obj.y;
								if (%obj.curLength >= %obj.spaceLength)
								{
									%obj.curWidth++;
									%obj.curLength = 0;
								}
							}
						}
					}
					else
					{
						//should be easy to check open spaces
						if (%obj.spaceLength < 1)
						{
							//ALWAYS come out of here with a length or we could freeze
							%obj.curBrick = %obj.checkBrick(%obj.x SPC %obj.y SPC %obj.z, %obj.x SPC %obj.y + %obj.maxLength-1 SPC %obj.z);
							if (isObject(%obj.curBrick))
							{
								%obj.spaceLength = %obj.getBrickMin(%obj.curBrick, %dyId) - %obj.y;
							}
							else
							{
								%obj.spaceLength = %obj.maxLength;
							}
							%obj.curWidth++;
							
							if (isObject(%obj.checkBrick(%obj.x + %obj.curWidth SPC %obj.y SPC %obj.z)))
								%obj.spaceWidth = %obj.curWidth;
						}
						else
						{
							%obj.curBrick = %obj.checkBrick(%obj.x + %obj.curWidth SPC %obj.y SPC %obj.z, %obj.x + %obj.curWidth SPC %obj.y + %obj.spaceLength-1 SPC %obj.z);
							if (isObject(%obj.curBrick))
							{
								%obj.spaceWidth = %obj.curWidth;
							}
							else
							{
								%obj.curWidth++;
								if (isObject(%obj.checkBrick(%obj.x + %obj.curWidth SPC %obj.y SPC %obj.z)))
									%obj.spaceWidth = %obj.curWidth;
							}
						}
					}
					
					//width check here
					if (%obj.curWidth >= (%obj.maxWidth))
						%obj.spaceWidth = %obj.maxWidth;
						
					%times++;
					if (%times > %MAXTIME)
						%obj.paused = true;
				}
				//now find the height of this space
				while (!%obj.paused && %obj.spaceHeight < 1)
				{
					if (%obj.spaceType == $spaces["filled"])
					{
						while (!%obj.paused && %obj.spaceHeight < 1 && %obj.curHeight <= %obj.maxHeight)
						{
							while (!%obj.paused && %obj.spaceHeight < 1 && %obj.heightX < %obj.spaceWidth)
							{
								while (!%obj.paused && %obj.spaceHeight < 1 && %obj.heightY < %obj.spaceLength)
								{
									if (!isObject(%obj.checkBrick(%obj.x + %obj.heightX SPC %obj.y + %obj.heightY SPC %obj.z + %obj.curHeight)))
									{
										%obj.spaceHeight = %obj.curHeight;
									}
									%times++;
									if (%times > %MAXTIME)
										%obj.paused = true;
									if (!%obj.paused)
										%obj.heightY++;
								}
								if (!%obj.paused)
								{
									%obj.heightX++;
									%obj.heightY = 0;
								}
							}
							if (!%obj.paused)
							{
								%obj.curHeight++;
								%obj.heightX = 0;
								%obj.heightY = 0;
							}
						}
						if (%obj.curHeight > %obj.maxHeight)
						{
							%obj.spaceHeight = %obj.maxHeight;
						}
					}
					else
					{
						while (!%obj.paused && %obj.spaceHeight < 1 && %obj.curHeight < %obj.maxHeight)
						{
							%scanCenter = VectorAdd(%obj.getWorldPosition(%obj.x + %obj.spaceWidth - 1 SPC %obj.y + %obj.spaceLength - 1 SPC %obj.z + %obj.curHeight), %obj.getWorldPosition(%obj.x SPC %obj.y SPC %obj.z + %obj.curHeight));
							%scanCenter = VectorScale(%scanCenter, 0.5);
							
							initContainerBoxSearch(%scanCenter, %obj.spaceWidth*%dxScale-%dxScale SPC %obj.spaceLength*%dyScale-%dyScale SPC %dzScale/2, $TypeMasks::FxBrickAlwaysObjectType); //this is hard coded to only work with x y z
							
							%obj.curBrick = containerSearchNext();
							%times++;
							
							if (%times > %MAXTIME)
								%obj.paused = true;
							if (!isObject(%obj.curBrick))
								%obj.curHeight++;
							else
								%obj.spaceHeight = %obj.curHeight;
						}
						if (!%obj.paused && %obj.spaceHeight < 1)
							%obj.spaceHeight = %obj.curHeight;
					}
				}
				
				if (!%obj.paused)
				{
					//reset stuff
					%obj.curWidth = 0;
					%obj.curLength = 0;
					%obj.addSpace(%obj.spaceType, %obj.x SPC %obj.y SPC %obj.z, %obj.spaceWidth SPC %obj.spaceLength SPC %obj.spaceHeight);
					%obj.spaceWidth = 0;
					%obj.spaceLength = 0;
					%obj.spaceHeight = 0;
					%obj.spaceType = -1;
					%obj.curHeight = 1;
					%obj.heightX = 0;
					%obj.heightY = 0;
				}

			}
			if (!%obj.paused)
			{
				%obj.x++;
				%obj.y = 0;
			}
		}
		if (!%obj.paused)
		{
			%obj.z++;
			%obj.x = 0;
			%obj.y = 0;
		}
	}
	if (%obj.paused)
	{
		%obj.asyncSched = %obj.schedule(100, "asyncIndexSpaces", %callback);
	}
	else
	{
		call(%callback, "onFinish");
	}
}

function BlockManager3d::getBrickMin(%obj, %brick, %dim)
{
	//extent is in brick units, but position is not
	%pos = worldToBrick(VectorSub(%obj.position, "0.25 0.25 0.1"));
	%begs = VectorSub(getWords(%brick.getBrickBox(), 0, 2), %pos);
	return getWord(VectorAdd(%begs, "0.5 0.5 0.2"), %dim);
}


function BlockManager3d::getBrickMax(%obj, %brick, %dim)
{
	//extent is in brick units, but position is not
	%pos = worldToBrick(VectorSub(%obj.position, "0.25 0.25 0.1"));
	%ends = VectorSub(getWords(%brick.getBrickBox(), 3, 5), %pos);
	return getWord(VectorAdd(%ends, "0.5 0.5 0.2"), %dim);
}

function BlockManager3d::addSpace(%obj, %type, %pos, %ext)
{
	if (%type == $spaces["filled"])
	{
		%obj.filledSpaces[%obj.numFilledSpaces, "position"] = %pos;
		%obj.filledSpaces[%obj.numFilledSpaces, "extent"] = %ext;
		%obj.numFilledSpaces++;
	}
	else
	{
		%obj.openSpaces[%obj.numOpenSpaces, "position"] = %pos;
		%obj.openSpaces[%obj.numOpenSpaces, "extent"] = %ext;
		%obj.numOpenSpaces++;
	}
}

function BlockManager3d::getSpaceType(%obj, %i)
{
	if (%i < %obj.numFilledSpaces)
		%type = $spaces["filled"];
	else
		%type = $spaces["open"];
	
	return %type;
}

function BlockManager3d::getSpacePosition(%obj, %i)
{
	if (%i < %obj.numFilledSpaces)
		%pos = %obj.filledSpaces[%i, "position"];
	else
		%pos = %obj.openSpaces[%i - %obj.numFilledSpaces, "position"];
	
	return %pos;
}

function BlockManager3d::getSpaceExtent(%obj, %i)
{
	if (%i < %obj.numFilledSpaces)
		%ext = %obj.filledSpaces[%i, "extent"];
	else
		%ext = %obj.openSpaces[%i - %obj.numFilledSpaces, "extent"];
	
	return %ext;
}

function BlockManager3d::getNumSpaces(%obj)
{
	return %obj.numFilledSpaces + %obj.numOpenSpaces;
}

//returns the next indexed space that is filled
//if the cell at %pos is filled, that will be returned
function BlockManager3d::nextFilledSpace(%obj, %pos, %dx)
{
	%dy = 1 ^ %dx;
	%next = getWord(%obj.extent, %dx) + 1;
	%dz = 2; //hacky...
	
	%x = getWord(%pos, %dx);
	%y = getWord(%pos, %dy);
	%z = getWord(%pos, %dz);
		
	for (%i = 0; %i < %obj.getNumSpaces(); %i++)
	{
		%spaceMinX = getWord(%obj.getSpacePosition(%i), %dx);
		%spaceMinY = getWord(%obj.getSpacePosition(%i), %dy);
		%spaceMinZ = getWord(%obj.getSpacePosition(%i), %dz);
		%spaceMaxX = %spaceMinX + getWord(%obj.getSpaceExtent(%i), %dx) - 1;
		%spaceMaxY = %spaceMinY + getWord(%obj.getSpaceExtent(%i), %dy) - 1;
		%spaceMaxZ = %spaceMinZ + getWord(%obj.getSpaceExtent(%i), %dz) - 1;
		
		if (%y >= %spaceMinY && %y <= %spaceMaxY && %spaceMinX < %next && %spaceMinX >= %x && %z >= %spaceMinZ && %z <= %spaceMaxZ)
			%next = %spaceMinX;
	}
	if (%next > getWord(%obj.extent, %dx))
		%next = -1;
	
	return %next;
}

function BlockManager3d::nextOpenSpace(%obj, %pos, %dx)
{
	%dy = 1 ^ %dx;
	%dz = 2; //change this
	
	%x = getWord(%pos, %dx);
	%y = getWord(%pos, %dy);
	%z = getWord(%pos, %dz);
	
	%next = %x;
		
	for (%i = 0; %i < %obj.getNumSpaces(); %i++)
	{
		%spaceMinX = getWord(%obj.getSpacePosition(%i), %dx);
		%spaceMinY = getWord(%obj.getSpacePosition(%i), %dy);
		%spaceMinZ = getWord(%obj.getSpacePosition(%i), %dz);
		%spaceMaxX = %spaceMinX + getWord(%obj.getSpaceExtent(%i), %dx) - 1;
		%spaceMaxY = %spaceMinY + getWord(%obj.getSpaceExtent(%i), %dy) - 1;
		%spaceMaxZ = %spaceMinZ + getWord(%obj.getSpaceExtent(%i), %dz) - 1;
		
		if (%y >= %spaceMinY && %y <= %spaceMaxY && %next >= %spaceMinX && %next <= %spaceMaxX && %z >= %spaceMinZ && %z <= %spaceMaxZ)
		{
			%next = %spaceMaxX + 1;
			//this stinks because we have to search again, but because of how the spaces are sorted, we must
			%i = -1;
		}
	}
	if (%next >= getWord(%obj.extent, %dx))
		%next = -1;
	
	return %next;
}

function BlockManager3d::checkBrick(%obj, %pos, %end)
{
	if (getWordCount(%end) < 3 || %pos $= %end)
	{
		initContainerBoxSearch(%obj.getWorldPosition(%pos), "0.1 0.1 0.1", $TypeMasks::FxBrickAlwaysObjectType);
		%brick = containerSearchNext();
	}
	else
	{
		%rayStart = VectorAdd(%obj.position, VectorAdd(brickToWorld(%pos), "0.25 0.25 0.1"));
		%rayEnd = VectorAdd(%obj.position, VectorAdd(brickToWorld(%end), "0.25 0.25 0.1"));
		%ray = containerRayCast(%rayStart, %rayEnd, $TypeMasks::FxBrickAlwaysObjectType);
		%brick = getWord(%ray, 0);
	}
	return %brick;
}

function BlockManager3d::getWorldPosition(%obj, %pos)
{
	%dsx = $dimensionScale[getWord(%obj.dimensions, 0)];
	%dsy = $dimensionScale[getWord(%obj.dimensions, 1)];
	%dsz = $dimensionScale[getWord(%obj.dimensions, 2)];
	%dvx = $dimensions[getWord(%obj.dimensions, 0)];
	%dvy = $dimensions[getWord(%obj.dimensions, 1)];
	%dvz = $dimensions[getWord(%obj.dimensions, 2)];
	
	%x = VectorScale(%dvx, %dsx * getWord(%pos, 0));
	%y = VectorScale(%dvy, %dsy * getWord(%pos, 1));
	%z = VectorScale(%dvz, %dsz * getWord(%pos, 2));
	
	return VectorAdd(%obj.position, VectorAdd(VectorAdd(%x, %y), %z));
}

function BlockManager3d::plantSpaces(%obj, %pos)
{
	%vbl = newVBL(1);
	for (%i = 0; %i < %obj.getNumSpaces(); %i++)
	{
		%numX = getWord(%obj.getSpaceExtent(%i), 0);
		%numY = getWord(%obj.getSpaceExtent(%i), 1);
		%numZ = getWord(%obj.getSpaceExtent(%i), 2);
		%offX = getWord(%obj.getSpacePosition(%i), 0);
		%offY = getWord(%obj.getSpacePosition(%i), 1);
		%offZ = getWord(%obj.getSpacePosition(%i), 2);
		//color should be 27-35
		//%color = %i;//98832 130802
		%shape = %obj.getSpaceType(%i);
		%offset = "0 0 0";//VectorScale("0 0 0.2", %obj.getSpaceType(%i));
		//while (%color >= 7)
		//	%color -= 7;
		//%color += 27;

		%color = 27 + (%i - mFloor(%i/8)*8);
		for (%x = 0; %x < %numX; %x++)
		{
			for (%y = 0; %y < %numY; %y++)
			{
				for (%z = 0; %z < %numZ; %z++)
				{
					if (!%x || !%y || !%z || %x == %numX - 1 || %y == %numY - 1 || %z == %numZ - 1)
						%vbl.addBrick(brick1x1fData, vectorAdd(vectorAdd(%obj.getWorldPosition(%x + %offX SPC %y + %offY SPC %z + %offZ), %pos), %offset), 0, 0, %color, %shape, %obj.getSpaceType(%i) * 3, 0, 1, %obj.getSpaceType(%i), 1);
				}
			}
		}
	}
	%ret = %vbl.asyncCreateBricks(clientgroup.getObject(0), clientgroup.getObject(0), "onFinishCreateBM");
	//%vbl.delete();
	return %ret;
}

function BlockManager3d::addToVBL(%obj, %vbl, %pos, %callback, %i, %x, %y, %z)
{
	if (!%i)
		%i = 0;
	if (!%x)
		%x = 0;
	if (!%y)
		%y = 0;
	if (!%z)
		%z = 0;
	%times = 0;
	%MAXTIME = 1;
	
	while (%times < %MAXTIME && %i < %obj.getNumSpaces())
	{
		%numX = getWord(%obj.getSpaceExtent(%i), 0);
		%numY = getWord(%obj.getSpaceExtent(%i), 1);
		%numZ = getWord(%obj.getSpaceExtent(%i), 2);
		%offX = getWord(%obj.getSpacePosition(%i), 0);
		%offY = getWord(%obj.getSpacePosition(%i), 1);
		%offZ = getWord(%obj.getSpacePosition(%i), 2);
		//color should be 27-35
		//%color = %i;//98832 130802
		%shape = %obj.getSpaceType(%i);
		%offset = "0 0 0";//VectorScale("0 0 0.2", %obj.getSpaceType(%i));
		%color = 27 + (%i - mFloor(%i/8)*8);
		while (%times < %MAXTIME && %x < %numX)
		{
			while (%times < %MAXTIME && %y < %numY)
			{
				while (%times < %MAXTIME && %z < %numZ)
				{
					if (!%x || !%y || !%z || %x == %numX - 1 || %y == %numY - 1 || %z == %numZ - 1)
						%vbl.addBrick(brick1x1fData, vectorAdd(vectorAdd(%obj.getWorldPosition(%x + %offX SPC %y + %offY SPC %z + %offZ), %pos), %offset), 0, 0, %color, %shape, %obj.getSpaceType(%i) * 3, 0, 1, %obj.getSpaceType(%i), 1);
					%z++;
					%times++;
				}
				if (%times < %MAXTIME)
				{
					%y++;
					%z = 0;
				}
			}
			if (%times < %MAXTIME)
			{
				%x++;
				%y = 0;
				%z = 0;
			}
		}
		if (%times < %MAXTIME)
		{
			%i++;
			%x = 0;
			%y = 0;
			%z = 0;
		}
	}
	if (%times < %MAXTIME)
	{
		call(%callback, "onAddToVBL");
	}
	else
	{
		%obj.addToVBL = %obj.schedule(33, "addToVBL", %vbl, %pos, %callback, %i, %x, %y, %z);
	}
}

function onFinishCreateBM(%vbl, %set)
{
	%vbl.delete();
	$blockmanDelete = %set;
}

function debugBlock()
{
	if (isObject(tt))
		tt.delete();
	if (isObject(t3d))
		t3d.delete();
	if (isObject($blockmanDelete))
		$blockmanDelete.delete();
	exec("add-ons/tool_manipulator/script_BlockManager3d.cs");
	//new scriptObject(t3d) {class = "BlockManager3d"; position = "263.25 -351.75 6.7"; extent = "74 106 195";dimensions = "x y z";};
	new ScriptObject(t3d) {class = "BlockManager3d"; position = VectorAdd(_foo.getPosition(), "0 0 -0.2"); extent = "18 18 32"; dimensions = "x y z";};
	t3d.asyncIndexSpaces("debugBlock2");
}

function debugBlock2(%callback)
{
	if (%callback $= "onAddToVBL")
		t3dVBL.asyncCreateBricks(clientgroup.getObject(0), clientgroup.getObject(0), "onFinishCreateBM");
	else
	{
		newVBL(1).setName(t3dVBL);
		t3d.addToVBL(t3dVBL, "1000 0 0", "debugBlock2");
	}
}

function BlockManager3d::pointInSpace(%obj, %i, %pos)
{
	
}

//function BlockManager3d::findSpace(%obj, %pos)
//{
//	//check to make sure the position is even in the mapped area
//	for (%i = 0; %i < %obj.getNumSpaces(); %i++)
//	{
//		if (
//	}
//}

function BlockManager3d::findPath(%obj, %start, %end)
{
	
}

//function BlockManager3d::save
//new scriptObject(castleBM) {class = "BlockManager3d"; position = "263.25 -351.75 6.7"; extent = "74 106 195";dimensions = "x y z";};
//8 20 1
//debug
//exec("add-ons/tool_manipulator/script_BlockManager3d.cs");
//new ScriptObject(tt) {class = "BlockManager3d"; position = getWords(_foo.getPosition(), 0, 1); extent = "20 20"; dimensions = "x y";};