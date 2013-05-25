function newRBL()
{
	return new ScriptObject()
	{
		superClass = BrickManipulation;
		deleteBricks = true;
		class = RealBrickList;
	};
}

function RealBrickList::onAdd(%this, %obj)
{
	%obj.brickGroup = new SimSet();
}

function RealBrickList::onRemove(%this, %obj)
{
	if (isObject(%obj.brickGroup))
	{
		if (%obj.deleteBricks)
		{
			while (%obj.brickGroup.getCount())
				%obj.brickGroup.getObject(0).delete();
		}
		%obj.brickGroup.delete();
	}
}

function RealBrickList::getDatablock(%obj, %num)
{
	return %obj.brickGroup.getObject(%num).getDatablock();
}
function RealBrickList::getPosition(%obj, %num)
{
	return %obj.brickGroup.getObject(%num).getPosition();
}
function RealBrickList::getAngleId(%obj, %num)
{
	return %obj.brickGroup.getObject(%num).getAngleId();
}
function RealBrickList::isBP(%obj, %num)
{
	return %obj.brickGroup.getObject(%num).isBP();
}
function RealBrickList::getColorId(%obj, %num)
{
	return %obj.brickGroup.getObject(%num).getColorId();
}
function RealBrickList::getPrint(%obj, %num)
{
	return %obj.brickGroup.getObject(%num).getPrintNameSingle();
}
function RealBrickList::getColorFx(%obj, %num)
{
	return %obj.brickGroup.getObject(%num).getColorFxId();
}
function RealBrickList::getShapeFx(%obj, %num)
{
	return %obj.brickGroup.getObject(%num).getShapeFxId();
}
function RealBrickList::isRaycasting(%obj, %num)
{
	return %obj.brickGroup.getObject(%num).isRaycasting();
}
function RealBrickList::isColliding(%obj, %num)
{
	return %obj.brickGroup.getObject(%num).isColliding();
}
function RealBrickList::isRendering(%obj, %num)
{
	return %obj.brickGroup.getObject(%num).isRendering();
}

function RealBrickList::addBrick(%obj, %brick)
{
	if (isObject(%brick))
	{
		%obj.brickGroup.add(%brick);
		%obj.onAddBasicData(%obj.brickGroup.getCount() - 1);
	}
}

//brick manipulation stuff
function RealBrickList::onAddBasicData(%obj, %num)
{
	if (!isObject(%obj.getDatablock(%num)))
		return;
	//purpose of this function is to update the list's properties so
	//width and length are always precalculated
	//if angleId is 1 or 3, reverse x and y
	//brickSizeX brickSizeY
	%sizeX = %obj.getDatablock(%num).brickSizeX / 4;
	%sizeY = %obj.getDatablock(%num).brickSizeY / 4;
	%sizeZ = %obj.getDatablock(%num).brickSizeZ * 0.2 / 2;
	if (%obj.getAngleId(%num) == 1 || %obj.getAngleId(%num) == 3)
	{
		%ty = %sizeX;
		%sizeX = %sizeY;
		%sizeY = %ty;
	}
	%posX = getWord(%obj.getPosition(%num), 0);
	%posY = getWord(%obj.getPosition(%num), 1);
	%posZ = getWord(%obj.getPosition(%num), 2);
	if (%obj.maxX $= "")
	{
		%obj.minX = %posX;
		%obj.maxY = %posY;
		%obj.maxX = %posX;
		%obj.minY = %posY;
		%obj.maxZ = %posZ;
		%obj.minZ = %posZ;
	}
	if (%obj.maxX < %sizeX + %posX)
		%obj.maxX = %sizeX + %posX;
	if (%obj.minX > -%sizeX + %posX)
		%obj.minX = -%sizeX + %posX;
	
	if (%obj.maxY < %sizeY + %posY)
		%obj.maxY = %sizeY + %posY;
	if (%obj.minY > -%sizeY + %posY)
		%obj.minY = -%sizeY + %posY;
		
	if (%obj.maxZ < %sizeZ + %posZ)
		%obj.maxZ = %sizeZ + %posZ;
	if (%obj.minZ > -%sizeZ + %posZ)
		%obj.minZ = -%sizeZ + %posZ;
}

function RealBrickList::resetSize(%obj)
{
	%obj.maxX = "";
	%obj.maxY = "";
	%obj.maxZ = "";
	%obj.minX = "";
	%obj.minY = "";
	%obj.minZ = "";
}

function RealBrickList::getNorthFace(%obj)
{
	return %obj.maxY;
}

function RealBrickList::getSouthFace(%obj)
{
	return %obj.minY;
}

function RealBrickList::getWestFace(%obj)
{
	return %obj.minX;
}

function RealBrickList::getEastFace(%obj)
{
	return %obj.maxX;
}

function RealBrickList::getBottomFace(%obj)
{
	return %obj.minZ;
}

function RealBrickList::getTopFace(%obj)
{
	return %obj.maxZ;
}

function RealBrickList::getFace(%obj, %dir)
{
	switch (%dir)
	{
		case 0:
			%face = %obj.getNorthFace();
		case 1:
			%face = %obj.getEastFace();
		case 2:
			%face = %obj.getSouthFace();
		case 3:
			%face = %obj.getWestFace();
		case 4:
			%face = %obj.getTopFace();
		case 5:
			%face = %obj.getBottomFace();
	}
	
	return %face;
}

function RealBrickList::setColliding(%obj, %val)
{
	for (%i = 0; %i < %obj.brickGroup.getCount(); %i++)
	{
		%obj.brickGroup.getObject(%i).setColliding(%val);
	}
}
function RealBrickList::setRendering(%obj, %val)
{
	for (%i = 0; %i < %obj.brickGroup.getCount(); %i++)
	{
		%obj.brickGroup.setRendering(%val);
	}
}