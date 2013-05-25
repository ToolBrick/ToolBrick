function bfFindChainBricks(%searchBrick, %justFoundBricks)
{
	if (isObject(%searchBrick))
	{
		for (%i = 0; %i < %searchBrick.getNumUpBricks(); %i++)
		{
			%brick = %searchBrick.getUpBrick(%i);
			%justFoundBricks.add(%brick);
		}
		for (%i = 0; %i < %searchBrick.getNumDownBricks(); %i++)
		{
			%brick = %searchBrick.getDownBrick(%i);
			%justFoundBricks.add(%brick);
		}
	}
}

addBFFindMethod("chain", "bfFindChainBricks");


function bfAllBricks(%brick)
{
	return 1;
}

addBFType("all", "bfAllBricks");


function bfSpecialBricks(%brick, %type)
{
	if (%brick.getDatablock().category $= %type)
		return 1;
	return 0;
}

addBFType("special", "bfSpecialBricks");

function bfNotSpecialBricks(%brick, %type)
{
	if (%brick.getDatablock().category !$= %type)
		return 1;
	return 0;
}

addBFType("notSpecial", "bfNotSpecialBricks");


function bfColorBricks(%brick, %color)
{
	if (%brick.getColorId() == %color)
		return 1;
	return 0;
}

addBFType("color", "bfColorBricks");

function bfNotColorBricks(%brick, %color)
{
	if (%brick.getColorId() != %color)
		return 1;
	return 0;
}

addBFType("notColor", "bfNotColorBricks");


function bfHasInputEvent(%brick, %eventName)
{
	for (%i = 0; %i < %brick.numEvents; %i++)
	{
		if (%brick.eventInput[%i] $= %eventName)
			return 1;
	}
	return 0;
}

addBFType("inputEvent", "bfHasInputEvent");

function bfNotHasInputEvent(%brick, %eventName)
{
	for (%i = 0; %i < %brick.numEvents; %i++)
	{
		if (%brick.eventInput[%i] $= %eventName)
			return 0;
	}
	return 1;
}

addBFType("notInputEvent", "bfNotHasInputEvent");


function bfHasOutputEvent(%brick, %eventName)
{
	for (%i = 0; %i < %brick.numEvents; %i++)
	{
		if (%brick.eventOutput[%i] $= %eventName)
			return 1;
	}
	return 0;
}

addBFType("outputEvent", "bfHasOutputEvent");

function bfNotHasOutputEvent(%brick, %eventName)
{
	for (%i = 0; %i < %brick.numEvents; %i++)
	{
		if (%brick.eventOutput[%i] $= %eventName)
			return 0;
	}
	return 1;
}

addBFType("notOutputEvent", "bfNotHasOutputEvent");


function bfHasName(%brick, %name)
{
	%brickName = %brick.getName();
	if (%brickName !$= "" && getSubStr(%brickName, 0, 1) $= "_" && getSubStr(%brickName, 1, strLen(%brickName) - 1))
		return 1;
	return 0;
}

addBFType("name", "bfHasName");

function bfNotHasName(%brick, %name)
{
	%brickName = %brick.getName();
	if (%brickName !$= "" && getSubStr(%brickName, 0, 1) $= "_" && getSubStr(%brickName, 1, strLen(%brickName) - 1))
		return 0;
	return 1;
}

addBFType("notName", "bfNotHasName");


function bfHasUIName(%brick, %uiName)
{
	%datablock = $uiNameTable[%uiName];
	if (!isObject(%datablock))
		return 0;
	if (%datablock == %brick.getId().getDatablock())
		return 1;
	return 0;
}

addBFType("uiName", "bfHasUIName");