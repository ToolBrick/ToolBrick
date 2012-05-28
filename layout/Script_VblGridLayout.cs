//VblGridLayout provides an easy to use structure for arranging multiple VBLs in a grid

function VblGridLayout::onAdd(%this, %obj)
{
	%obj.tiles = new SimSet();
	//%obj.rows
	//%obj.cols
}

function VblGridLayout::onRemove(%this, %obj)
{
	while (%obj.getCount())
		%obj.getObject(0).delete();
}

function VblGridLayout::addTile(%obj, %vbl)
{
	%obj.tiles.add(%vbl);
}

function VblGridLayout::setTile(%obj, %x, %y, %id)
{
	%obj.tiles[%x, %y] = %id;
}

function VblGridLayout::export(%obj, %file)
{
	echo("the file is " @ %file);
	%name = fileBase(%file);
	%path = filePath(%file);
	%f = new FileObject();
	
	%f.openForWrite(%file);
	
	%f.writeLine(%obj.rows SPC %obj.cols);
	
	for (%y = 0; %y < %obj.rows; %y++)
	{
		%line = "";
		for (%x = 0; %x < %obj.cols; %x++)
			%line = %line @ %obj.grid[%x, %y] @ " ";
		%f.writeLine(%line);
	}
	
	%f.writeLine(%obj.tiles.getCount());
	
	for (%i = 0; %i < %obj.tiles.getCount(); %i++)
	{
		%tileName = %path @ "/" @ %name @ "_" @ %i @ ".bls";
		%obj.tiles.getObject(%i).exportBLSFile(%tileName);
		%f.writeLine(%tileName);
	}
	
	%f.close();
	%f.delete();
}

function VblGridLayout::import(%obj, %file)
{
	%f = new FileObject();
	%f.openForRead(%file);
	
	%line = %f.readLine();
	echo(%line);
	%obj.rows = getWord(%line, 0);
	%obj.cols = getWord(%line, 1);
	
	for (%y = 0; %y < %obj.rows; %y++)
	{
		%line = %f.readLine();
		for (%x = 0; %x < %obj.cols; %x++)
			%obj.grid[%x, %y] = getWord(%line, %x);
	}
	
	%numTiles = %f.readLine();
	
	for (%i = 0; %i < %numTiles; %i++)
	{
		%vbl = newVbl(1);
		%tilePath = 
		%vbl.loadBLSFile(%f.readLine());
		%obj.tiles.add(%vbl);
	}
	
	%f.close();
	%f.delete();
}

function ServerCmdnewVblGrid(%client, %name)
{
	echo("do this");
	%client.vblGrid = new ScriptObject()
	{
		class = "VblGridLayout";
		client = %client;
	};
	
	%client.vblGrid.import("config/server/grid/" @ %name @ ".vgl");
}

function ServerCmdSaveVblGrid(%client, %name)
{
	if (isObject(%client.vblGrid))
	{
		%client.vblGrid.export("config/server/grid/" @ %name @ ".vgl");
	}
}

function ServerCmdaddGridTile(%client)
{
	if (isObject(%client.vblGrid))
	{
	//	%client.
	}
}

datablock ItemData(GridToolItem : wandItem)
{
	uiName = "Grid Tool";
	doColorShift = true;
	colorShiftColor = "1 0 1 1";
	image = GridToolImage;
};

datablock ShapeBaseImageData(GridToolImage : wandImage)
{
	item = GridToolItem;
	doColorShift = true;
	colorShiftColor = GridToolItem.colorShiftColor;
};

function GridToolImage::onFire(%this, %obj, %slot)
{
	%types = ($TypeMasks::FxBrickAlwaysObjectType);
	%col = containerRaycast(%obj.getEyePoint(), VectorAdd(VectorScale(%obj.getEyeVector(), 8), %obj.getEyePoint()), %types);
	%col = getWord(%col, 0);
	
	if(isObject(%col))
		GridToolCollision(%obj, %col);
}

function ServerCmdGridTool(%client)
{
	if (isObject(%client.player))
	{
		%client.player.updateArm(GridToolImage);
		%client.player.mountImage(GridToolImage, 0);
	}
}

function GridToolCollision(%obj, %col)
{
	%vbl = newVBL();
	%vbl.isVblGrid = true;
	%vbl.client = %obj.client;
	
	if (isObject(%obj.client.vblGrid) && %col.getClassName() $= "fxDTSBrick")
		%vbl.addRealBuild(%col);
}

package VblGridPackage
{
	function virtualBrickList::onFinishAddingBuild(%obj, %bf)
	{
		Parent::onFinishAddingBuild(%bf);
		//echo(%obj.client);
		if (%obj.isVblGrid)
			%obj.client.vblGrid.addTile(%obj);
		commandToClient(%obj.client, 'centerPrint', "\c3Added a tile.", 3);
	}
};

activatePackage(VblGridPackage);

function VblGridLayout::generateBuild(%obj, %pos)
{
	//the position is the southwest bottom corner
	//get size from first tile
	%xSize = %obj.tiles.getObject(0).getSizeX();
	%ySize = %obj.tiles.getObject(0).getSizeY();
	
	for (%x = 0; %x < %obj.cols; %x++)
	{
		for (%y = 0; %y < %obj.rows; %y++)
		{
			%tileCorner = %x * %xSize + getWord(%pos, 0) SPC %y * %ySize + getWord(%pos, 1) SPC getWord(%pos, 2);
			%tileNum = %obj.grid[%x, %y];
			if (%tileNum != 0)
			{
				%vbl = %obj.tiles.getObject(%tileNum);
				%vbl.realign("west" SPC getWord(%tileCorner, 0) TAB "south" SPC getWord(%tileCorner, 1) TAB "down" SPC getWord(%tileCorner, 2));
				%vbl.createBricks();
			}
		}
	}
}

function ServerCmdgenerateGrid(%client, %x, %y, %z)
{
	if (isObject(%client.vblGrid))
		%client.vblGrid.generateBuild(%x SPC %y SPC %z);
}

function countGrid(%path)
{
	%file = new FileObject();
	%file.openForRead(%path);
	%lineCount = 0;
	%col = 0;
	%count = 0;
	while (!%file.isEOF())
	{
		%lineCount++;
		%line = %file.readLine();
		%col = getWordCount(%line);
		for (%w = 0; %w < %col; %w++)
		{
			%word = getWord(%line, %w);
			if (%word != 0)
				%count++;
		}
	}
	echo("rows: " @ %lineCount @ " cols: " @ %col @ " tiles: " @ %count);
	%file.close();
	%file.delete();
}

//56 -31 0.1
//59.5 -31 0.1
//56 -27 0.1
function VblGridLayout::scanTiles()
{
	//%start = 
}
