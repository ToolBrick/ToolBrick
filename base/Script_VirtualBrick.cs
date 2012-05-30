function VirtualBrick::onAdd(%this, %obj)
{
	//set basic brick properties to default values or not?
	//when loading large groups of bricks into a vbl
	//that would cause around 10x more steps to be done per brick
	//it isn't hard to imagine a 10k set of bricks being loaded
	//that woul cause 100,000 more steps to be performed
	
	//but if it simplifies code maybe, it's alright?
}

function virtualBrickList::getDatablock(%obj)
{
	return %obj.datablock;
}
function virtualBrickList::getPosition(%obj)
{
	return %obj.position;
}
function virtualBrickList::getObjectBox(%obj)
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
function virtualBrickList::getWorldBox(%obj)
{
	%pos = %obj.getPosition();
	%ob = %obj.getObjectBox();
	return VectorAdd(%pos, getWords(%ob, 0, 2)) SPC VectorAdd(%pos, getWords(%ob, 3, 5));
}
function virtualBrickList::getBrickSize(%obj)
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
function virtualBrickList::getSize(%obj)
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
function virtualBrickList::getAngleId(%obj)
{
	return %obj.angleId;
}
function virtualBrickList::isBP(%obj)
{
	return %obj.isBaseplate;
}
function virtualBrickList::getColorId(%obj)
{
	return %obj.colorId;
}
function virtualBrickList::getPrint(%obj)
{
	return %obj.printId;
}
function virtualBrickList::getColorFx(%obj)
{
	return %obj.colorFx;
}
function virtualBrickList::getShapeFx(%obj)
{
	return %obj.shapeFx;
}
function virtualBrickList::isRaycasting(%obj)
{
	return %obj.isRaycasting;
}
function virtualBrickList::isColliding(%obj)
{
	return %obj.isColliding;
}
function virtualBrickList::isRendering(%obj)
{
	return %obj.isRendering;
}

function virtualBrickList::setDatablock(%obj, %db)
{
	%obj.datablock = %db;
}
function virtualBrickList::setPosition(%obj, %pos) //takes into account the offset
{
	%obj.position = %pos;
}
function virtualBrickList::setAngleId(%obj, %id)
{
	%obj.angleId = %id;
}
function virtualBrickList::setBP(%obj, %bp)
{
	%obj.isBaseplate = %bp;
}
function virtualBrickList::setColorId(%obj, %id)
{
	%obj.colorId = %id;
}
function virtualBrickList::setPrint(%obj, %print)
{
	%obj.printId = %print;
}
function virtualBrickList::setColorFx(%obj, %fx)
{
	%obj.colorFx = %fx;
}
function virtualBrickList::setShapeFx(%obj, %fx)
{
	%obj.shapeFx = %fx;
}
function virtualBrickList::setRaycasting(%obj, %raycasting)
{
	%obj.isRaycasting = %raycasting;
}
function virtualBrickList::setColliding(%obj, %colliding)
{
	%obj.isColliding = %colliding;
}
function virtualBrickList::setRendering(%obj, %rendering)
{
	%obj.isRendering = %rendering;
}