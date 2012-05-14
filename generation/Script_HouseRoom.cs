


function HouseRoom::onAdd(%this, %obj)
{
	%obj.numCorners = 0;
	%obj.height = 18;
	%obj.color[0] = 3; //primary
	%obj.color[1] = 2; //secondary
}
function HouseRoom::createBasicWall(%obj)
{
	//the basic wall has a primary color in the middle, and is outlined by the secondary color
	%wallVBL = newVBL();
	for (%i = 0; %i < %obj.numCorners; %i++)
	{
		%c = %obj.corners[%i];
		%n = %i + 1;
		if (%n >= %obj.numCorners)
			%n = 0;
		%nc = %obj.corners[%n];
		%offset = VectorNormalize(VectorSub(%c, %nc));
		%end = VectorAdd(%nc, %offset);
		echo(%c SPC "::" SPC %nc);
		//bottom trim
		%add = BlockFiller::fillCorners(%c, VectorAdd(%end, "0 0" SPC 2));
		for (%col = 0; %col < %add.numBricks; %col++)
			%add.setColorId(%col, %obj.color[1]);
		%wallVBL.addVBL(%add);
		//middle
		echo("middle:" SPC VectorAdd(%c, "0 0" SPC 3) SPC VectorAdd(%end, "0 0" SPC %obj.height-4));
		%add = BlockFiller::fillCorners(VectorAdd(%c, "0 0" SPC 3), VectorAdd(%end, "0 0" SPC %obj.height-4));
		for (%col = 0; %col < %add.numBricks; %col++)
			%add.setColorId(%col, %obj.color[0]);
		%wallVBL.addVBL(%add);
		//top trim
		%add = BlockFiller::fillCorners(VectorAdd(%c, "0 0" SPC %obj.height-3), VectorAdd(%end, "0 0" SPC %obj.height-1));
		for (%col = 0; %col < %add.numBricks; %col++)
			%add.setColorId(%col, %obj.color[1]);
		%wallVBL.addVBL(%add);
	}
	return %wallVBL;
}

function HouseRoom::addCorner(%obj, %corner)
{
	%obj.corners[%obj.numCorners] = %corner;
	%obj.numCorners++;
}

function HouseRoom::getWall(%obj, %wall)
{
	//wall 0 is from corners 0-1, 1 is from corners 1-2 etc
	if (%wall < 0)
		%wall--;
	%wall %= %obj.numCorners;
	return getCorner(%wall) SPC getCorner(%wall+1);
}

function HouseRoom::getCorner(%obj, %c)
{
	if (%c < 0)
		%c--;
	return %obj.corners[%c % %obj.numCorners];
}

function ServerCmdNewRoom(%client, %height)
{
	%client.curRoom = new ScriptObject()
	{
		class = "HouseRoom";
	};
}
function ServerCmdAddCorner(%client)
{
	%client.curRoom.addCorner(%client.player.tempbrick.getBrickPosition());
}

function ServerCmdCreateRoom(%client)
{
	%client.curRoom.createBasicWall().createBricks(%client);
}

function HouseRoom::createPatternWall()
{
	
}

//room walls are used for handling collisions 
function RoomWall::onAdd(%this, %obj)
{
	%obj.numIntersects = 0;
	%obj.direction = 0;
	%obj.position = "0 0 0";
	%obj.extent = 0;
	%obj.nextWall = 0;
}