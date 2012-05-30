//exec("add-ons/tool_manipulator/script_blockFiller.cs");
function BlockFiller::indexBricks()
{
	$BlockFiller::numBricks = 0;
	for (%i = 0; %i < DataBlockGroup.getCount(); %i++)
	{
		%db = DataBlockGroup.getObject(%i);
		if (%db.getClassName() $= "fxDTSBrickData" && (%db.category $= "Bricks" || %db.category $= "Baseplates" || %db.category $= "Plates"))
		{
			%vol = %db.getVolume();
			if ($BlockFiller::numBricks == 0 || ($BlockFiller::bricks[$BlockFiller::numBricks - 1].getVolume() < %vol))
			{
				$BlockFiller::bricks[$BlockFiller::numBricks] = %db;
				$BlockFiller::numBricks++;
			}
			else
			{
				%placed = false;
				for (%j = $BlockFiller::numBricks; %j > 0; %j--)
				{
					$BlockFiller::bricks[%j + 1] = $BlockFiller::bricks[%j];
					if ($BlockFiller::bricks[%j-1].getVolume() < %vol)
					{
						$BlockFiller::bricks[%j] = %db;
						%placed = true;
					}
				}
				if (!%placed)
				{
					$BlockFiller::bricks[%j + 1] = $BlockFiller::bricks[%j];
					$BlockFiller::bricks[%j] = %db;
				}
				$BlockFiller::numBricks++;
			}
		}
	}
}
//echo(BlockFiller::fillSpace("-2.5 59 0", "1 1 3"));
//-0.5 -130.5 34
//BlockFiller::fillSpace("-0.5 -130.5 34", "1 1 3").createBricks();
//TODO: Make cache only work off size, not size and position
function BlockFiller::fillSpace(%pos, %space)
{
	%px = getWord(%pos, 0);
	%py = getWord(%pos, 1);
	%pz = getWord(%pos, 2);
	if (!isObject($BlockFiller::cache[%space]))
	{
		if (VectorLen(%space))
		{
			%numAdds = 0;
			%sx = getWord(%space, 0);
			%sy = getWord(%space, 1);
			%sz = getWord(%space, 2);
			
			%tried = 0;
			%i = $BlockFiller::numBricks - 1;
			%angleNum = 0;
			while (%tried <  3 && %i >= 0)
			{
				%db = $BlockFiller::bricks[%i];
				%angles = %db.fitsInSpace(%space);
				%angle = getWord(%angles, %angleNum);
				//echo("SPACE!!!!!!!!!!!!!!!" SPC %space SPC %angle SPC %db.getName());
				if (%angle != 0)
				{
					%tried++;
					%add = newVBL();
					if (!%angleNum)
					{
						%x = %db.brickSizeX;
						%y = %db.brickSizeY;
					}
					else
					{
						%x = %db.brickSizeY;
						%y = %db.brickSizeX;
					}
					%z = %db.brickSizeZ;
					%add.addBrick(%db, "0 0 0", %angleNum, 0, 0, "", 0, 0, 1, 1, 1);
					%add.realign("south" SPC %py);
					%add.realign("west" SPC %px);
					%add.realign("down" SPC %pz);
					
					if (%sx - %x > 0) //create new east side
					{
						%maybeAdd = BlockFiller::fillSpace(%px + %x*0.5 SPC %py SPC %pz, %sx - %x SPC %sy SPC %z);
						%add.addVBL(%maybeAdd);
					//	%maybeAdd.delete();
					}
					if (%sy - %y > 0) //create new north side
					{
						%maybeAdd = BlockFiller::fillSpace(%px SPC %py + %y*0.5 SPC %pz, %x SPC %sy - %y SPC %z);
						%add.addVBL(%maybeAdd);
					//	%maybeAdd.delete();
					}
					if (%sz - %z > 0) //create new top side
					{
						%maybeAdd = BlockFiller::fillSpace(%px SPC %py SPC %pz + %z*0.2, %sx SPC %sy SPC %sz - %z);
						%add.addVBL(%maybeAdd);
					//	%maybeAdd.delete();
					}
					
					if (!isObject(%vbl))
						%vbl = %add;
					else if (%vbl.getCount() > %add.getCount())
					{
						%vbl.delete();
						%vbl = %add;
					}
					else
						%add.delete();
				}
				if (!%angleNum)
					%i--;
				%angleNum ^= 1;
			}
		}
		else
			%vbl = newVBL();
		$BlockFiller::cache[%space] = %vbl;
	}
	else
	{
		%vbl = $BlockFiller::cache[%space];
		%vbl.realign("south" SPC %py);
		%vbl.realign("west" SPC %px);
		%vbl.realign("down" SPC %pz);
	}
	return %vbl;
}

function BlockFiller::fill(%pos, %space)
{
	%pos = VectorSub(brickToWorld(%pos), "0.25 0.25 0.1");
	return BlockFiller::fillSpace(%pos, %space);
}

function BlockFiller::fillCorners(%c1, %c2)
{
	%c1x = getWord(%c1, 0);
	%c1y = getWord(%c1, 1);
	%c1z = getWord(%c1, 2);
	
	%c2x = getWord(%c2, 0);
	%c2y = getWord(%c2, 1);
	%c2z = getWord(%c2, 2);
	
	if (%c1x > %c2x)
		%minX = %c2x;
	else
		%minX = %c1x;
	if (%c1y > %c2y)
		%minY = %c2y;
	else
		%minY = %c1y;
	if (%c1z > %c2z)
		%minZ = %c2z;
	else
		%minZ = %c1z;
	echo(%c1x SPC "c'sss" SPC %c2x);
	echo("BlockFiller:" SPC %minX SPC %minY SPC %minZ SPC mAbs(%c1x-%c2x) SPC mAbs(%c1y-%c2y) SPC mAbs(%c1z-%c2z));
	return BlockFiller::fill(%minX SPC %minY SPC %minZ, mAbs(%c1x-%c2x)+1 SPC mAbs(%c1y-%c2y)+1 SPC mAbs(%c1z-%c2z)+1);
}

function fxDTSBrickData::fitsInSpace(%db, %space)
{
	%angles = "";
	%vol = getWord(%space, 0) * getWord(%space, 1) * getWord(%space, 2);
	if (%db.getVolume() <= %vol && %db.brickSizeZ <= getWord(%space, 2))
	{
		if (%db.brickSizeX <= getWord(%space, 0) && %db.brickSizeY <= getWord(%space, 1))
			%angles = %angles @ "1 ";
		else
			%angles = %angles @ "0 ";
		if (%db.brickSizeX <= getWord(%space, 1) && %db.brickSizeY <= getWord(%space, 0))
			%angles = %angles @ "1";
		else
			%angles = %angles @ "0";
	}
	else
		%angles = "0 0";
	return %angles;
}

package BlockFillerPackage
{
	function ServerCmdPlantBrick(%client)
	{
		if (!isObject(%client.vbl))
			%client.vbl = newVBL();
		if (%client.coolBuilding && isObject(%client.player) && isObject(%client.player.tempBrick) && !%client.player.tempBrick.isVblBase)
		{
			if (%client.cbn == "")
				%client.cbn = 0;
			%tb = %client.player.tempBrick;
			%pos = %tb.getPosition();
			%box = %tb.getWorldBox();
			%minX = getWord(%box, 0);
			%minY = getWord(%box, 1);
			%minZ = getWord(%box, 2);
			%maxX = getWord(%box, 3);
			%maxY = getWord(%box, 4);
			%maxZ = getWord(%box, 5);
			
			if (%client.gCBMaxX == "" || %client.gCBMaxX < %maxX)
				%client.gCBMaxX = %maxX;
			if (%client.gCBMaxY == "" || %client.gCBMaxY < %maxY)
				%client.gCBMaxY = %maxY;
			if (%client.gCBMaxZ == "" || %client.gCBMaxZ < %maxZ)
				%client.gCBMaxZ = %maxZ;
				
			if (%client.gCBMinX == "" || %client.gCBMinX > %minX)
				%client.gCBMinX = %minX;
			if (%client.gCBMinY == "" || %client.gCBMinY > %minY)
				%client.gCBMinY = %minY;
			if (%client.gCBMinZ == "" || %client.gCBMinZ > %minZ)
				%client.gCBMinZ = %minZ;
			%client.cbn++;
			if (%client.cbn >= 2)
			{
				%bx = (%client.gCBMaxX - %client.gCBMinX) / 0.5;
				%by = (%client.gCBMaxY - %client.gCBMinY) / 0.5;
				%bz = (%client.gCBMaxZ - %client.gCBMinZ) / 0.2;
				
				%client.vbl.addVBL(BlockFiller::fillSpace(%client.gCBMinX SPC %client.gCBMinY SPC %client.gCBMinZ, %bx SPC %by SPC %bz));
				for (%c = 0; %c < %client.vbl.getCount(); %c++)
					%client.vbl.setColorId(%c, %client.currentColor);
				%client.vbl.createBricks(%client, %client);
				%client.vbl.clearList();
				
				%client.cbn = 0;
				%client.gCBMaxX = "";
				%client.gCBMaxY = "";
				%client.gCBMaxZ = "";
				%client.gCBMinX = "";
				%client.gCBMinY = "";
				%client.gCBMinZ = "";
			}
		}
		else
		{
			Parent::ServerCmdPlantBrick(%client);
		}
	}
	function ServerCmdMessageSent(%client, %message)
	{
		echo(%message);
		Parent::ServerCmdMessageSent(%client, %message);
	}
};

function scaleBuild(%build, %scale)
{
	%vbl = newVBL();
	%minBound = getWords(%build.getWorldBox(), 0, 2);
	for (%i = 0; %i < %build.getCount(); %i++)
	{
		echo(%i SPC "out of" SPC %build.getCount());
		%db = %build.getDatablock(%i);
		%color = %build.getColorId(%i);
		%minOff = VectorScale(%build.getSize(%i), %scale/2);
		%bSize = VectorScale(%build.getBrickSize(%i), %scale);
		%pos = %build.getPosition(%i);
		%dif = VectorSub(%pos, getWords(%buildBox, 0, 2));
		%newDif = VectorScale(%dif, %scale);
		%newPos = VectorAdd(getWords(%buildBox, 0, 2), %scale);
		%oPos = VectorAdd(getWords(%build.getObjectBox(%i), 0, 2), %pos);
		%dif = VectorSub(%oPos, %minBound);
		%sPos = VectorScale(%dif, %scale);
		//echo("name" SPC %db.getName() SPC "objectBox:" SPC getWords(%build.getObjectBox(%i), 0, 2));
		//echo("OPOS:" SPC %oPos SPC "MINBOUND:" SPC %minBound);
		//echo("SPOS:" SPC %sPOS SPC "BSIZE" SPC %bSize);
		//echo(%i SPC "SPOS: " @ %sPos SPC %minBound SPC %oPos SPC %dif);
		
		%startPos = VectorSub(%newPos, %minOff);
		%add = BlockFiller::fillSpace(%sPos, %bSize);
		for (%c = 0; %c < %add.getCount(); %c++)
			%add.setColorId(%c, %color);
		%vbl.addVBL(%add);
	}
	echo(%vbl.getCount());
	return %vbl;
}

function ServerCmdEnlargeSelection(%client, %scale)
{
	if (%client.isAdmin)
	{
		if (isObject(%client.vbl))
		{
			%old = %client.vbl;
			%client.vbl = scaleBuild(%old, %scale);
			%bounds = %client.player.tempbrick.getWorldBox();
			%client.vbl.realign("south" SPC getWord(%bounds, 1));
			%client.vbl.realign("west" SPC getWord(%bounds, 0));
			%client.vbl.realign("down" SPC getWord(%bounds, 2));
			%client.vbl.createBricks(%client);
		}
	}
}
activatePackage(BlockFillerPackage);