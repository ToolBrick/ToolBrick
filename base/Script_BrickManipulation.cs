//The idea of brick manipulation was that it would be a common interface between VirtualBrickList and RealBrickList
//This probably won't work or is pointless, but should be considered a little more before being completely removed

function BrickManipulation::onAdd(%this, %obj)
{
	
}

function BrickManipulation::onAddBasicData(%obj, %num)
{
	if (!isObject(%obj.getDatablock(%obj)))
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
		%obj.maxX = %posX;
		%obj.minX = %posX;
		%obj.maxY = %posY;
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

function BrickManipulation::resetSize(%obj)
{
	%obj.maxX = "";
	%obj.maxY = "";
	%obj.maxZ = "";
	%obj.minX = "";
	%obj.minY = "";
	%obj.minZ = "";
}