function VirtualBrick::onAdd(%this, %obj)
{
	//set basic brick properties to default values or not?
	//when loading large groups of bricks into a vbl
	//that would cause around 10x more steps to be done per brick
	//it isn't hard to imagine a 10k set of bricks being loaded
	//that woul cause 100,000 more steps to be performed
	
	//but if it simplifies code maybe, it's alright?
}

function VirtualBrick::getDatablock(%obj)
{
	return %obj.db;
}
function VirtualBrick::getPosition(%obj)
{
	return %obj.position;
}
function VirtualBrick::getObjectBox(%obj)
{
	%db = %obj.getDatablock();
	%angle = %obj.getAngleId();
	%angle %= 2;
	if (!%angle)
	{
		%x = %db.brickSizeX/4;
		%y = %db.brickSizeY/4;
	}
	else
	{
		%x = %db.brickSizeY/4;
		%y = %db.brickSizeX/4;
	}
	%z = %db.brickSizeZ/10;
	
	return -%x SPC -%y SPC -%z SPC %x SPC %y SPC %z;
}
function VirtualBrick::getWorldBox(%obj)
{
	%pos = %obj.getPosition();
	%ob = %obj.getObjectBox();
	return VectorAdd(%pos, getWords(%ob, 0, 2)) SPC VectorAdd(%pos, getWords(%ob, 3, 5));
}
function VirtualBrick::getBrickSize(%obj)
{
	%db = %obj.getDatablock();
	%angle = %obj.getAngleId();
	%angle %= 2;
	if (!%angle)
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
	return %x SPC %y SPC %z;
}
function VirtualBrick::getSize(%obj)
{
	%db = %obj.getDatablock();
	%angle = %obj.getAngleId();
	%angle %= 2;
	if (!%angle)
	{
		%x = %db.brickSizeX/2;
		%y = %db.brickSizeY/2;
	}
	else
	{
		%x = %db.brickSizeY/2;
		%y = %db.brickSizeX/2;
	}
	%z = %db.brickSizeZ/5;
	return %x SPC %y SPC %z;
}
function VirtualBrick::getAngleId(%obj)
{
	return %obj.angleId;
}
function fxDTSBrick::isBP(%obj) //Can't make VirtualBrick::isBasePlate, so we need to make a method that works with fxDTSBrick
{
	return %obj.isBasePlate();
}
function VirtualBrick::isBP(%obj)
{
	return %obj.isBaseplate;
}
function VirtualBrick::getColorId(%obj)
{
	return %obj.colorId;
}
function VirtualBrick::getPrint(%obj)
{
	return %obj.printId;
}
function VirtualBrick::getPrintId(%obj)
{
	return %obj.printId;
}
function VirtualBrick::getColorFx(%obj)
{
	return %obj.colorFx;
}
function VirtualBrick::getColorFxId(%obj)
{
	return %obj.colorFx;
}
function VirtualBrick::getShapeFx(%obj)
{
	return %obj.shapeFx;
}
function VirtualBrick::getShapeFxId(%obj)
{
	return %obj.shapeFx;
}
function VirtualBrick::isRaycasting(%obj)
{
	return %obj.isRaycasting;
}
function VirtualBrick::isColliding(%obj)
{
	return %obj.isColliding;
}
function VirtualBrick::isRendering(%obj)
{
	return %obj.isRendering;
}

function VirtualBrick::setDatablock(%obj, %db)
{
	%obj.db = %db;
}
function VirtualBrick::setPosition(%obj, %pos) //takes into account the offset
{
	%obj.position = %pos;
}
function VirtualBrick::setAngleId(%obj, %id)
{
	%obj.angleId = %id;
}
function VirtualBrick::setBP(%obj, %bp)
{
	%obj.isBaseplate = %bp;
}
function VirtualBrick::setColorId(%obj, %id)
{
	%obj.colorId = %id;
}
function VirtualBrick::setPrint(%obj, %print)
{
	%obj.printId = %print;
}
function VirtualBrick::setColorFx(%obj, %fx)
{
	%obj.colorFx = %fx;
}
function VirtualBrick::setShapeFx(%obj, %fx)
{
	%obj.shapeFx = %fx;
}
function VirtualBrick::setRaycasting(%obj, %raycasting)
{
	%obj.isRaycasting = %raycasting;
}
function VirtualBrick::setColliding(%obj, %colliding)
{
	%obj.isColliding = %colliding;
}
function VirtualBrick::setRendering(%obj, %rendering)
{
	%obj.isRendering = %rendering;
}