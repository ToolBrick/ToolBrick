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

function BlockManager::onAdd(%this, %obj)
{
	//position should be set to the -x and -y 'ist brick spot in the area
	//width should be the x
	//length should be the y
	//actually width and length will be extent
	//dimension = "x y" or "x z" or "y z"
	//containerRayCast(%start, %end, mask, exempt0, exempt1, exempt2)
	
	//three types of spaces
	//unknown, filled, taken (could more be added?)
	%obj.numSpaces = 0;
	
	//hackish, but I want to test this script out
	%obj.width = getWord(%obj.extent, 0);
	%obj.length = getWord(%obj.extent, 1);
}

//indexes the area the block manager handles
function BlockManager::indexSpaces(%obj)
{
	//this function will take any 'unknown' spaces and index them
	%d1 = $dimensions[getWord(%obj.dimensions, 0)];
	%d1Id = $dimensionIds[getWord(%obj.dimensions, 0)];
	%d2 = $dimensions[getWord(%obj.dimensions, 1)];
	%d2Id = $dimensionIds[getWord(%obj.dimensions, 1)];
	
	//thinking in terms of x and y is easier, so use those as variable names (but could really be using any dimension)
	for (%x = 0; %x < %obj.width; %x++) //loop through each row
	{
		%y = 0;
		%y = %obj.nextOpenSpace(%x SPC %y, 1);
		while (%y != -1) //go through the row until an unindexed spot is found
		{
			echo(%x SPC %y);
			%length = 0;
			%width = 0;
			%curRow = 0;
			//at this point we know we need to make a new block, find out what type and set up for the block creation loop
			//
			//we perform a raycast to find the type (and if it's an open space, then its length)
			
			//find the length we need to do the raycast
			%endY = %obj.nextFilledSpace(%x SPC %y, 1);
			if (%endY == -1)
				%endY = %obj.length;
				
			%maxLength = %endY - %y;
			//%ray = containerRayCast(%obj.getWorldPosition(%x SPC %y), %obj.getWorldPosition(%x SPC %endY), $TypeMasks::FxBrickAlwaysObjectType);
			
			//now interpret results to find what type of space to make, possibly its length
			%brick = %obj.checkBrick(%x SPC %y);
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
				//%curBrick = getWord(%ray, 0);
				//%hitLoc = %obj.getGridPosition(getWord(%curBrick.getWorldBox(), %d2Id) + 0.5);
			}
			while (%width <= 0 || %length <= 0)
			{
				//echo("Type:" @ %type SPC "CurRow:" @ %curRow SPC "CurLength:" @ %curLength);
				//echo("Width:" @ %width SPC "Length:" @ %length SPC "MaxLength:" @ %maxLength);
				if (%curRow > 22)
					return;
				//do something different depending on if we are making an open or filled space
				if (%type == $spaces["filled"])
				{
					echo("start: " @ %curRow);
					//always start at a spot where we need to check further down the y dimension
					//but if we don't know any bricks on this row yet, then we need to do a box check
					if (%curLength < 1)
					{
						echo("special check!" SPC %x + %curRow SPC %y);
						%brick = %obj.checkBrick(%x + %curRow SPC %y);
					}
					else
					{
						//do a raycast on the y dimension
						echo("curlength:" @ %curLength SPC %length);
						%rayStart = %obj.getWorldPosition(%x + %curRow SPC %y + %curLength - 1); //start inside the last brick
						%rayEnd = %obj.getWorldPosition(%x + %curRow SPC %y + %curLength); //end in the suspected spot
						//%ray = containerRayCast(%rayStart, %rayEnd, $TypeMasks::FxBrickAlwaysObjectType, %brick);
						//%brick = getWord(%ray, 0);
						%brick = %obj.checkBrick(%x + %curRow SPC %y + %curLength);
					}
					if (isObject(%brick))
					{
						//if we found something, update our current length
						%brickBox = %brick.getWorldBox();
						%curLength += (getWord(%brickBox, %d2Id + 3) - getWord(%brickBox, %d2Id))*2;
						echo("curlength became:" @ %curLength);
						if (%curLength >= %maxLength)
						{
							echo("greater then max length" SPC %maxLength);
							%curLength = %maxLength;
							if (%length <= 0)
								%length = %curLength;
						}
						if (%length > 0 && %curLength >= %length)
						{
							echo("increment row!");
							//time to move to the next row
							%curLength = 0;
							%curRow++;
						}
					}
					else
					{
					echo("no brick :(");
						//if we didn't find anything, our curLength is the max length for this row
						//check it with the current length value or make it the length if necessary
						if (%length <= 0)
						{
							//the length will be this, now move to the next row
							%length = %curLength;
							%curRow++;
							%curLength = 0;
							//now need to check for the first brick on the next row
						}
						else
						{
							//this row is not large enough to be included in this rectangle, we have the info we need to form this space
							%width = %curRow;
						}
					}
					echo("end: " @ %curRow);
				}
				else
				{
					//at this point we just need to check this row for bricks
					//always use a raycast
					if (%length <= 0)
						%rayLength = %maxLength - 1;
					else
						%rayLength = %length - 1;
					%rayStart = %obj.getWorldPosition(%x + %curRow SPC %y); //start inside the last brick
					%rayEnd = %obj.getWorldPosition(%x + %curRow SPC %y + %rayLength); //end in the suspected spot
					//echo("RayStart:" @ %rayStart SPC "rayEnd:" @ %rayEnd);
					%ray = containerRayCast(%rayStart, %rayEnd, $TypeMasks::FxBrickAlwaysObjectType, %brick);
					%brick = getWord(%ray, 0);
					if (isObject(%brick))
					{
						echo("found this brick: " @ %brick);
						if (%curRow <= 0)
						{
							//if this is the first row, then we now have the length of this space
							%brickBox = %brick.getWorldBox();
							%radius = $dimensionScale[getWord(%obj.dimensions, 1)]/2;
							echo("DEBUG: curLength: " SPC %d2Id);
							echo("DEBUG2: curLength:" @getWord(%obj.getWorldPosition(%x + %curRow SPC %y), %d2Id));
							%curLength = ((getWord(%brickBox, %d2Id) + %radius) - getWord(%obj.getWorldPosition(%x + %curRow SPC %y), %d2Id))*2; //first brick stud - start of row, convert to brick units
							%length = %curLength;
							
							//next line
							%curRow++;
							if (isObject(%obj.checkBrick(%x + %curRow SPC %y)))
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
						
						if (isObject(%obj.checkBrick(%x + %curRow SPC %y)))
							%width = %curRow;
					}
				}
				
				//I guess this check can go here >.>
				if (%x + %curRow > %obj.width)
					%width = %curRow;
			}
			echo("width:" @ %width SPC "length:" @ %length);
			//we are now able to make a space!
			%obj.addSpace(%type, %x SPC %y, %width SPC %length);
			
			//set up for the next loop around
			%y = %obj.nextOpenSpace(%x SPC %y, 1);
		}
	}
}

function BlockManager::addSpace(%obj, %type, %pos, %ext)
{
	echo("add space :D " SPC %type SPC %pos SPC %ext);
	%obj.spaces[%obj.numSpaces, "type"] = %type;
	%obj.spaces[%obj.numSpaces, "position"] = %pos;
	%obj.spaces[%obj.numSpaces, "extent"] = %ext;
	%obj.numSpaces++;
}

//returns the next indexed space that is filled
//if the cell at %pos is filled, that will be returned
function BlockManager::nextFilledSpace(%obj, %pos, %dx)
{
	%dy = 1 ^ %dx;
	%next = getWord(%obj.extent, %dx) + 1;
	
	%x = getWord(%pos, %dx);
	%y = getWord(%pos, %dy);
		
	for (%i = 0; %i < %obj.numSpaces; %i++)
	{
		%spaceMinX = getWord(%obj.spaces[%i, "position"], %dx);
		%spaceMinY = getWord(%obj.spaces[%i, "position"], %dy);
		%spaceMaxX = %spaceMinX + getWord(%obj.spaces[%i, "extent"], %dx) - 1;
		%spaceMaxY = %spaceMinY + getWord(%obj.spaces[%i, "extent"], %dy) - 1;
		
		if (%y >= %spaceMinY && %y <= %spaceMaxY && %spaceMinX < %next && %spaceMinX >= %x)
			%next = %spaceMinX;
	}
	if (%next > getWord(%obj.extent, %dx))
		%next = -1;
	
	return %next;
}

function BlockManager::nextOpenSpace(%obj, %pos, %dx)
{
	//echo("find openspace: " @ %pos SPC %dx);
	%dy = 1 ^ %dx;
	
	%x = getWord(%pos, %dx);
	%y = getWord(%pos, %dy);
	
	%next = %x;
		
	for (%i = 0; %i < %obj.numSpaces; %i++)
	{
		%spaceMinX = getWord(%obj.spaces[%i, "position"], %dx);
		%spaceMinY = getWord(%obj.spaces[%i, "position"], %dy);
		%spaceMaxX = %spaceMinX + getWord(%obj.spaces[%i, "extent"], %dx) - 1;
		%spaceMaxY = %spaceMinY + getWord(%obj.spaces[%i, "extent"], %dy) - 1;
		
		if (%y >= %spaceMinY && %y <= %spaceMaxY && %next >= %spaceMinX && %next <= %spaceMaxX)
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

function BlockManager::checkBrick(%obj, %pos)
{
	initContainerBoxSearch(%obj.getWorldPosition(%pos), "0.1 0.1 0.1", $TypeMasks::FxBrickAlwaysObjectType);
	return containerSearchNext();
}

function BlockManager::getWorldPosition(%obj, %pos)
{
	%dsx = $dimensionScale[getWord(%obj.dimensions, 0)];
	%dsy = $dimensionScale[getWord(%obj.dimensions, 1)];
	%dvx = $dimensions[getWord(%obj.dimensions, 0)];
	%dvy = $dimensions[getWord(%obj.dimensions, 1)];
	
	%x = VectorScale(%dvx, %dsx * getWord(%pos, 0));
	%y = VectorScale(%dvy, %dsy * getWord(%pos, 1));
	//echo("getWorldPosition:" @ %dsy @"|"@ %dvy @"|"@ %y @"|"@ %pos);
	return VectorAdd(%obj.position, VectorAdd(%x, %y));
}

function BlockManager::plantSpaces(%obj, %pos)
{
	%vbl = newVBL(1);
	for (%i = 0; %i < %obj.numSpaces; %i++)
	{
		%numX = getWord(%obj.spaces[%i, "extent"], 0);
		%numY = getWord(%obj.spaces[%i, "extent"], 1);
		%offX = getWord(%obj.spaces[%i, "position"], 0);
		%offY = getWord(%obj.spaces[%i, "position"], 1);
		%color = %i;//98832 130802
		%shape = %obj.spaces[%i, "type"];
		%offset = VectorScale("0 0 0.2", %obj.spaces[%i, "type"]);
		while (%color >= 31)
			%color -= 31;
		for (%x = 0; %x < %numX; %x++)
		{
			for (%y = 0; %y < %numY; %y++)
			{
				 %vbl.addBrick(brick1x1fData, vectorAdd(vectorAdd(%obj.getWorldPosition(%x + %offX SPC %y + %offY), %pos), %offset), 0, 0, %color, %shape, %obj.spaces[%i, "type"] * 3, 0, 1, 1, 1);
			}
		}
	}
	%ret = %vbl.createBricks();
	%vbl.delete();
	return %ret;
}

function debugBlock()
{
	if (isObject(tt))
		tt.delete();
	if (isObject($blockmanDelete))
		$blockmanDelete.delete();
	exec("add-ons/tool_manipulator/script_blockManager.cs");
	new ScriptObject(tt) {class = "BlockManager"; position = getWords(_foo.getPosition(), 0, 2); extent = "20 20"; dimensions = "x y";};
	tt.indexSpaces();
	$blockmanDelete = tt.plantSpaces("0 0 5");
}
//new scriptObject(tt) {class = "blockmanager"; position = $popo; extent = "74 106";dimensions = "x y";};

//debug
//exec("add-ons/tool_manipulator/script_blockManager.cs");
//new ScriptObject(tt) {class = "BlockManager"; position = getWords(_foo.getPosition(), 0, 1); extent = "20 20"; dimensions = "x y";};