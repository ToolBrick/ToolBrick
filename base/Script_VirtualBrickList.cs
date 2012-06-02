//The virtualbricklist will store brick information in multiarrays
//datablock" position angleid unknown color PRINT colorfx shapefx
//function virtualBrickList::addBrick(%datablock, %pos, %angleid, %unknown, %color, %print, %colorfx, %shapefx)
//function virtualBrickList::addBrickObj(%brick)
//making this change so svn should update

//above text is old
//TODO: make water bricks place correctly

package vblPackage
{
function newVBL(%returnBrickSet)
{
	if (%returnBrickSet $= "")
		%returnBrickSet = 0;
	return new ScriptObject()
	{
		//superClass = BrickManipulation;
		class = virtualBrickList;
		returnBrickSet = %returnBrickSet;
		brickOffset = "0 0 0";
	};
}

function inputEvent_GetTargetIndex(%arg1, %arg2, %arg3)
{
	Parent::inputEvent_GetTargetIndex(%arg1, %arg2, %arg3);
}
function virtualBrickList::onAdd(%this, %obj)
{
	%obj.absoluteRotation = 0;
	%obj.vBricks = new SimSet();
	%obj.markers = new SimSet();
}

function virtualBrickList::onRemove(%this, %obj)
{
	%obj.clearList();
	
	%obj.vBricks.delete();
	
	if (isObject(%obj.markers))
	{
		while (%obj.markers.getCount())
			%obj.markers.getObject(0).delete();
		%obj.markers.delete();
	}
}

function addCustSave(%pref)
{
	if ($custSavePrefs[%pref]) return 0;
	if ($numCustSaves $= "") $numCustSaves = 0;
	$custSaves[$numCustSaves, "name"] = %pref;
	$custSavePrefs[%pref] = 1;
	$custSavePrefs[%pref, "id"] = $numCustSaves;
	$numCustSaves++;
}

function virtualBrickList::cs_addReal(%obj, %csName, %vb, %brick)
{
	if (%csName $= "") return;
	if ($custSavePrefs[%csName])
		eval("%obj.cs_addReal_" @ %csName @ "(%vb, %brick);");
}

function virtualBrickList::cs_create(%obj, %csName, %vb, %brick)
{
	if (%csName $= "") return;
	if ($custSavePrefs[%csName])
		eval("%obj.cs_create_" @ %csName @ "(%vb,  %brick);");
}

function virtualBrickList::cs_rotateCW(%obj, %csName, %vb, %times)
{
	if (isFunction("virtualBrickList", "cs_rotateCW_" @ %csName))
		eval("%obj.cs_rotateCW_" @ %csName @ "(%vb, %times);");
}

function virtualBrickList::cs_rotateCCW(%obj, %csName, %vb, %times)
{
	if (isFunction("virtualBrickList", "cs_rotateCCW_" @ %csName))
		eval("%obj.cs_rotateCCW_" @ %csName @ "(%vb, %times);");
}

function virtualBrickList::cs_save(%obj, %csName, %vb, %file)
{
	if (%csName $= "") return;
	if ($custSavePrefs[%csName])
		eval("%obj.cs_save_" @ %csName @ "(%vb,  %file);");
}

function virtualBrickList::cs_load(%obj, %csName, %vb, %addData, %addInfo, %addArgs, %line)
{
	if (%csName $= "") return;
	if ($custSavePrefs[%csName])
		eval("%obj.cs_load_" @ %csName @ "(%vb,  %addData, %addInfo, %addArgs, %line);");
}

function virtualBrickList::getVirtualBrick(%obj, %num)
{
	return %obj.vBricks.getObject(%num);
}

function virtualBrickList::getDatablock(%obj, %num)
{
	return %obj.getVirtualBrick(%num).getDatablock();
}
function virtualBrickList::getPosition(%obj, %num)
{
	return VectorAdd(%obj.getVirtualBrick(%num).getPosition(), %obj.brickOffset);
}
function virtualBrickList::getObjectBox(%obj, %num)
{
	return %obj.getVirtualBrick(%num).getObjectBox();
}

function virtualBrickList::getWorldBox(%obj, %num)
{
	%pos = %obj.getPosition(%num);
	%ob = %obj.getObjectBox(%num);
	return VectorAdd(%pos, getWords(%ob, 0, 2)) SPC VectorAdd(%pos, getWords(%ob, 3, 5));
}
function virtualBrickList::getBrickSize(%obj, %num)
{
	return %obj.getVirtualBrick(%num).getBrickSize();
}
function virtualBrickList::getSize(%obj, %num)
{
	return %obj.getVirtualBrick(%num).getSize();
}
function virtualBrickList::getAngleId(%obj, %num)
{
	return %obj.getVirtualBrick(%num).getAngleId();
}
function virtualBrickList::isBP(%obj, %num)
{
	return %obj.getVirtualBrick(%num).isBP();
}
function virtualBrickList::getColorId(%obj, %num)
{
	return %obj.getVirtualBrick(%num).getColorId();
}
function virtualBrickList::getPrint(%obj, %num)
{
	return %obj.getVirtualBrick(%num).getPrint();
}
function virtualBrickList::getColorFx(%obj, %num)
{
	return %obj.getVirtualBrick(%num).getColorFx();
}
function virtualBrickList::getShapeFx(%obj, %num)
{
	return %obj.getVirtualBrick(%num).getShapeFx();
}
function virtualBrickList::isRaycasting(%obj, %num)
{
	return %obj.getVirtualBrick(%num).isRaycasting();
}
function virtualBrickList::isColliding(%obj, %num)
{
	return %obj.getVirtualBrick(%num).isColliding();
}
function virtualBrickList::isRendering(%obj, %num)
{
	return %obj.getVirtualBrick(%num).isRendering();
}

function virtualBrickList::setDatablock(%obj, %num, %db)
{
	%obj.getVirtualBrick(%num).setDatablock(%db);
}
function virtualBrickList::setPosition(%obj, %num, %pos) //takes into account the offset
{
	%obj.getVirtualBrick(%num).setPosition(VectorSub(%pos, %obj.brickOffset));
}
function virtualBrickList::setAngleId(%obj, %num, %id)
{
	%obj.getVirtualBrick(%num).setAngleId(%id);
}
function virtualBrickList::setBP(%obj, %num, %bp)
{
	%obj.getVirtualBrick(%num).setBP(%bp);
}
function virtualBrickList::setColorId(%obj, %num, %id)
{
	%obj.getVirtualBrick(%num).setColorId(%id);
}
function virtualBrickList::setPrint(%obj, %num, %print)
{
	%obj.getVirtualBrick(%num).setPrint(%print);
}
function virtualBrickList::setColorFx(%obj, %num, %fx)
{
	%obj.getVirtualBrick(%num).setColorFx(%fx);
}
function virtualBrickList::setShapeFx(%obj, %num, %fx)
{
	%obj.getVirtualBrick(%num).setShapeFx(%fx);
}
function virtualBrickList::setRaycasting(%obj, %num, %raycasting)
{
	%obj.getVirtualBrick(%num).setRaycasting(%raycasting);
}
function virtualBrickList::setColliding(%obj, %num, %colliding)
{
	%obj.getVirtualBrick(%num).setColliding(%colliding);
}
function virtualBrickList::setRendering(%obj, %num, %rendering)
{
	%obj.getVirtualBrick(%num).setRendering(%rendering);
}

function virtualBrickList::addBrick(%obj, %datablock, %pos, %angleid, %isBaseplate, %color, %print, %colorfx, %shapefx, %raycasting, %collision, %rendering)
{
	//currently assuming this is the only way to insert a brick into the vbl!
	%idx = %obj.getCount();
	
	//Assignments can be done directly here, might be worth doing that instead of calling all these methods
	%vBrick = new ScriptObject()
	{
		class = "VirtualBrick";
	};
	%obj.vBricks.add(%vBrick);
	
	%obj.setDatablock(%idx, %datablock);
	%obj.setPosition(%idx, %pos);
	%obj.setAngleId(%idx, %angleid);
	%obj.setBP(%idx, %isBaseplate);
	%obj.setColorId(%idx, %color);
	%obj.setPrint(%idx, %print);
	%obj.setColorfx(%idx, %colorfx);
	%obj.setShapefx(%idx, %shapefx);
	%obj.setRaycasting(%idx, %raycasting);
	%obj.setColliding(%idx, %collision);
	%obj.setRendering(%idx, %rendering);
	%obj.onAddBasicData(%idx);
	
	return %obj.getCount() - 1;
}

function virtualBrickList::removeBrick(%obj, %i)
{
	%obj.vBricks.remove(%obj.vBricks.getObject(%i));
}

function virtualBrickList::getSizeX(%obj)
{
	return %obj.maxX - %obj.minX;
}

function virtualBrickList::getSizeY(%obj)
{
	return %obj.maxY - %obj.minY;
}

function virtualBrickList::getSizeZ(%obj)
{
	return %obj.maxZ - %obj.minZ;
}

function virtualBrickList::loadBLSFile(%obj, %fileName)
{
	$pref::brickColors::num = 0;
	//if (isFile(%fileName) && fileExt(%fileName) $= ".bls")
	//{
		%file = new FileObject();
		%file.openForRead(%fileName);
		%curLine = 0;
		%atMarkers = 0;
		while (!%file.isEOF())
		{	
			%line = %file.readLine();
			%lines[%curLine] = %line;
			if (%atMarkers)
			{
				//name position primary secondary
				%obj.addMarker(getField(%line, 0), getField(%line, 1), getField(%line, 2), getField(%line, 3));
				//%obj.markers[getField(%line, 0)] = new ScriptObject()
				//{
				//	class = "vblMarker";
				//	name = getField(%line, 0);
				//	position = getField(%line, 1);
				//	primary = getFIeld(%line, 2);
				//	secondary = getField(%line, 3);
				//	vbl = %obj;
				//};
				//%obj.markers.add(%obj.markers[getField(%line, 0)]);
			}
			else if (getSubStr(%line, 0, 2) !$= "+-" && %atbricks && strstr(%line, "\"") > 0)
			{
				if (getSubStr(%line, 0, 1) $= "\t")
					%atMarkers = true;
				else
				{
					%qspot = strstr(%line, "\"");
					%datablock = getSubStr(%line, 0, %qspot);
					if (!$uinametablecreated)
						createUINameTable();
					%datablock = $uiNameTable[%datablock];
					if (!isObject(%datablock))
						continue;
					%posLine = getSubStr(%line, %qspot + 2, strLen(%line) - %qspot);
					%curBrick = %obj.addBrick
					(
						%datablock,
						getWords(%posLine, 0, 2),
						getWord(%posLine, 3),
						getWord(%posLine, 4) + 1,
						getWord(%posLine, 5) ,
						$printNameTable[getWord(%posLine, 6)],
						getWord(%posLine, 7),
						getWord(%posLine, 8),
						getWord(%posLine, 9),
						getWord(%posLine, 10),
						getWord(%posLine, 11)
					);
				}
			}
			else if (getSubStr(%line, 0, 2) $= "+-" && %atbricks)// && strstr(%line, "\"") > 0)
			{
				%vb = %obj.getVirtualBrick(%curBrick);
				%addType = getWord(%line, 0);
				%addType = getSubStr(%addType, 2, strLen(%addType) - 2);
				%addInStart = strLen(%addType) + 3;
				%addInfo = getSubStr(%line, %addInStart, strLen(%line) - %addInStart);
				if (strstr(%addInfo, "\"") > 0)
				{
					%word = "";
					for (%l = 0; %l < strlen(%addInfo); %l++)
					{
						%let = getSubStr(%addInfo, %l, 1);
						if (%let !$= "\"")
							%word = %word @ %let;
						else
						{
							%addData = %word;
							if (strLen(%addInfo) - (strLen(%addData) + 2) >= 0)
								%addArgs = getSubStr(%addInfo, strLen(%addData) + 2, strLen(%addInfo) - (strLen(%addData) + 2));//
							else
								%addArgs = "";
							//%addArgs = just remove %addData @ "\" " from the list
						}
					}
				}
				//different types..
				//we can replace the bottom lines with %obj.loadCustSave(%curBrick, %addType, %addArgs);
				if (%addType $= "Emitter")
				{
					%addInfo = $uiNameTable_Emitters[%addData];
					if (isObject(%addInfo))
					{
						%vb.bProps["Emitter"] = %addInfo;
						%vb.bProps["Emitter", 0] = getWord(%addArgs, 0);
					}
				}
				else if (%addType $= "Light")
				{
					%addInfo = $uiNameTable_Lights[%addData];
					if (isObject(%addInfo))
						%vb.bProps["Light"] = %addInfo;
				}
				else if (%addType $= "Music")
				{
					%addInfo = $uiNameTable_Music[%addData];
					if (isObject(%addInfo))
					{
						%vb.bProps["Music"] = %addInfo;
						%vb.bProps["Music", 0] = getWord(%addArgs, 0);
					}
				}
				else if (%addType $= "Vehicle")
				{
					%addInfo = $uiNameTable_Vehicle[%addData];
					if (isObject(%addInfo))
					{
						%vb.bProps["Vehicle"] = %addInfo;
						%vb.bProps["Vehicle", 0] = getWord(%addArgs, 0);
					}
				}
				else if (%addType $= "Item")
				{
					%addInfo = $uiNameTable_Items[%addData];
					if (isObject(%addInfo))
					{
						%vb.bProps["Item"] = %addInfo;
						%vb.bProps["Item", 1] = getWord(%addArgs, 0);
						%vb.bProps["Item", 0] = getWord(%addArgs, 1);
						%vb.bProps["Item", 2] = getWord(%addArgs, 2);
					}
				}
				else if ($custSavePrefs[%addType])
					%obj.cs_load(%addType, %vb, %addData, %addInfo, %addArgs, %line);
			}
			else if (%atBricks && %line $= "\tMarkers")
			{
				%atMarkers = true;
			}
			if (getWord(%line, 0) $= "Linecount" && getWordCount(%line) == 2)
				%atbricks = true;
			%curLine++;
		}
		%file.close();
		%file.delete();
	//}
}

function virtualBrickList::exportBLSFile(%obj, %fileName)
{
	%file = new FileObject();
	%file.openForWrite(%fileName);
	%file.writeLine("This is a Blockland save file.  You probably shouldn't modify it cause you'll screw it up.");
	%file.writeLine("1");
	//%file.writeLine("This file has been exported from virtualBrickList.");
	%file.writeLine("");
	//export colors here
	for (%i = 0; %i < 64; %i++)
		%file.writeLine(getColorIDTable(%i));
	%file.writeLine("Linecount" SPC %obj.getCount());
	for (%brickNum = 0; %brickNum < %obj.getCount(); %brickNum++)
	{
		%vb = %obj.getVirtualBrick(%brickNum);
		%datablock = %obj.getDatablock(%brickNum);
		%pos = %obj.getPosition(%brickNum);
		%angleid = %obj.getAngleId(%brickNum);
		%isBaseplate = %obj.isBP(%brickNum);
		%color = %obj.getColorId(%brickNum) + 1;
		%print = %obj.getPrintName(%brickNum);
		%colorfx = %obj.getColorFx(%brickNum);
		%shapefx = %obj.getShapeFx(%brickNum);
		%raycasting = %obj.isRaycasting(%brickNum);
		%collision = %obj.isColliding(%brickNum);
		%rendering = %obj.isRendering(%brickNum);
		//brickUIname" position angleId isBaseplate color print colorfx shapefxraycasting collision rendering
		%file.writeLine(%datablock.uiName @ "\"" SPC %pos SPC %angleid SPC %isBaseplate SPC %color - 1 SPC %print SPC %colorfx SPC %shapefx SPC %raycasting SPC %collision SPC %rendering);
		if (%vb.bProps["Emitter"] !$= "")
			%file.writeLine("+-EMITTER " @ %vb.bProps["Emitter"].uiName @ "\" " @ %vb.bProps["Emitter", 0]);
		if (%vb.bProps["Light"] !$= "")
			%file.writeLine("+-LIGHT " @ %vb.bProps["Light"].uiName @ "\"");
		if (%vb.bProps["Music"] !$= "")
			%file.writeLine("+-MUSIC " @ %vb.bProps["Music"].uiName @ "\" " @ %vb.bProps["Music", 0]);
		if (%vb.bProps["Vehicle"] !$= "")
			%file.writeLine("+-VEHICLE " @ %vb.bProps["Vehicle"].uiName @ "\" " @ %vb.bProps["Vehicle", 0]);
		if (%vb.bProps["Item"] !$= "")
			%file.writeLine("+-ITEM " @ %vb.bProps["Item"].uiName @ "\" " @ %vb.bProps["Item", 1] SPC %vb.bProps["Item", 0] SPC %vb.bProps["Item", 2]);
		for (%i = 0; %i < $numCustSaves; %i++)
		{
			%csName = $custSaves[%i, "name"];
			if (%vb.props[%csName] !$= "")
				%obj.cs_save(%csName, %vb, %file);
		}
	}
	
		//name position primary secondary
	if (%obj.markers.getCount())
	{
		%file.writeLine("\tMarkers");
		for (%i = 0; %i < %obj.markers.getCount(); %i++)
		{
			%marker = %obj.markers.getObject(%i);
			%file.writeLine(%marker.name TAB %marker.position TAB %marker.primary TAB %marker.secondary);
		}
	}
	
	%file.close();
	%file.delete();
}

function virtualBrickList::clearList(%obj)
{
	//Two important notes:
	//First, this could be bad to do if multiple lists are allowed to reference a single VirtualBrick
	//Second, this could take awhile, maybe the onRemove method should pass this to another method
	//that slowly deletes all the objects
	while (%obj.vBricks.getCount())
		%obj.vBricks.getObject(0).delete();
	
	%obj.brickOffset = "0 0 0"; //is this alright to do?
}

function virtualBrickList::createBricks(%obj, %client, %overideClient)
{
	%factory = new ScriptObject()
	{
		class = "BrickFactory";
		returnBrickSet = %obj.returnBrickSet;
	};
	%ret = %factory.createBricks(%obj, %client, %overideClient);
	%factory.delete();
	return true;
}

function virtualBrickList::createBricksForBlid(%obj, %blid)
{
	%factory = new ScriptObject()
	{
		class = "BrickFactory";
		returnBrickSet = %obj.returnBrickSet;
	};
	%ret = %factory.createBricks(%obj, %blid);
	%factory.delete();
	return true;
}

function virtualBrickList::createBricksNoOwner(%obj)
{
	%factory = new ScriptObject()
	{
		class = "BrickFactory";
		returnBrickSet = %obj.returnBrickSet;
	};
	%ret = %factory.createBricksNoOwner(%obj);
	%factory.delete();
	return true;
}

function BrickFactory::createBricks(%obj, %vbl, %client, %overideClient)
{
	if (%client $= "")
		%client = 0;
	if (%obj.returnBrickSet)
		%set = newRBL();
		
	for (%i = 0; %i < %vbl.getCount(); %i++)
	{
		%b = %vbl.createBrick(%i, %client, %overideClient);
		if (isObject(%b))
			%obj.onCreateBrick(%b);
		if (%obj.returnBrickSet)
			%set.addBrick(%b);
	}
	if (%obj.returnBrickSet)
		return %set;
	
	return true;
}

function BrickFactory::createBricksNoOwner(%obj, %vbl)
{
	if (%obj.returnBrickSet)
		%set = newRBL();
	for (%i = 0; %i < %vbl.getCount(); %i++)
	{
		%b = %vbl.createBrickNoOwner(%i);
		if (isObject(%b))
			%obj.onCreateBrick(%b);
		if (%obj.returnBrickSet)
			%set.addBrick(%b);
	}
	if (%obj.returnBrickSet)
		return %set;
	
	return true;
}

function BrickFactory::createBricksForBlid(%obj, %vbl, %blid)
{
	%client = findClientByBL_ID(%blid);
	
	%brickGroupName = "BrickGroup_" @ %blid;
	if (!isObject(%brickGroupName))
	{
		new SimGroup(%brickGroupName);
		%brickGroup = %brickGroupName.getId();
		%brickGroup.bl_id = %blid;
		%brickGroup.name = "BL_ID:" SPC %blid;
		mainBrickGroup.add(%brickGroup);
	}
	
	if (%obj.returnBrickSet)
		%set = newRBL();
		
	if (%client)
		%obj.createBricks(%vbl, %client);
	else
	{
		for (%i = 0; %i < %vbl.getCount(); %i++)
		{
			%b = %vbl.createBasicBrick(%i);
			
			if (!isObject(%b))
				continue;
				
			%b.stackBL_ID = %bl_id;
			
			%brickGroupName.add(%b);

			%b = %vbl.standardPlantBrick(%i, %b);
			
			if (!isObject(%b))
				continue;
			%obj.onCreateBrick(%b);
			if (%obj.returnBrickSet)
				%set.addBrick(%b);
		}
	}
	
	if (%obj.returnBrickSet)
		return %set;
	
	return true;
}

function BrickFactory::onCreateBrick(%obj, %brick)
{
	//stub to be overridden
}

function virtualBrickList::asyncCreateBricks(%obj, %client, %overideClient, %callback, %pass, %set)
{
	if (%client $= "")
		%client = 0;
	if (!%pass)
		%pass = 0;
	%times = 0;
	if (%obj.returnBrickSet && !isObject(%set))
		%set = newRBL();
	while (%pass < %obj.getCount() && %times < 4)
	{
		%b = %obj.createBrick(%pass, %client, %overideClient);
		if (isObject(%set))
			%set.addBrick(%b);
		%times++;
		%pass++;
	}
	if (%pass < %obj.getCount())
	{
		%obj.asyncCreate = %obj.schedule(33, "asyncCreateBricks", %client, %overideClient, %callback, %pass, %set);
	}
	else
	{
		call(%callback, %obj, %set);
	}
}

function virtualBrickList::createGhostBrick(%obj, %i)
{
	%db = %obj.getDatablock(%i);
	if (!isObject(%db))
	{
		return;
	}
	%pos = %obj.getPosition(%i);
	%angId = %obj.getAngleId(%i);
	%isBasePlate = %obj.isBP(%i);
	%colorId = %obj.getColorId(%i);
	%printId = %obj.getPrint(%i);
	%colorFX = %obj.getColorFx(%i);
	%shapeFX = %obj.getShapeFx(%i);
	%raycasting = %obj.isRaycasting(%i);
	%collision = %obj.isColliding(%i);
	%rendering = %obj.isRendering(%i);
	//get all the properties so badspot's code can work
	%trans = %pos;
	switch(%angId)
	{
		case 0:
		%trans = %trans SPC " 1 0 0 0";
		case 1:
		%trans = %trans SPC " 0 0 1" SPC $piOver2;
		case 2:
		%trans = %trans SPC " 0 0 1" SPC $pi;
		case 3:
		%trans = %trans SPC " 0 0 -1" SPC $piOver2;
	}
	
	%b = new fxDTSBrick()
	{
		dataBlock = %db;
		angleID = %angId;
		isBasePlate = %isBasePlate;
		colorId = %colorId;
		printId = %printId;
		colorFXID = %colorFX;
		shapeFXID = %shapeFX;
		isPlanted = true;
	};
	
	if (isObject(%b))
	{
		%b.setRaycasting(%raycasting);
		%b.setColliding(%collision);
		%b.setRendering(%rendering);
		
		%b.setTrusted(1);
		%b.setTransform(%trans);
	}
	

	
	return %b;
}

function virtualBrickList::applyCustomSaves(%obj, %i, %b)
{
	%vb = %obj.getVirtualBrick(%i);
	for (%cs = 0; %cs < $numCustSaves; %cs++)
	{
		%csName = $custSaves[%cs, "name"];
		%obj.cs_create(%csName, %vb, %b);
	}
}

function virtualBrickList::applyPlantedProperties(%obj, %i, %b)
{
	%vb = %obj.getVirtualBrick(%i);
	if (isObject(%vb.bProps["Emitter"]))
	{
		%b.setEmitter(%vb.bProps["Emitter"]);
		%b.setEmitterDirection(%vb.bProps["Emitter", 0]);
	}
	if (isObject(%vb.bProps["Light"]))
		%b.setLight(%vb.bProps["Light"]);
	if (isObject(%vb.bProps["Music"]))
		%b.setSound(%vb.bProps["Music"]);
	if (isObject(%vb.bProps["Vehicle"]))
	{
		%b.setVehicle(%vb.bProps["Vehicle"]);
		if (%vb.bProps["Vehicle", 0] == 1)
			%b.spawnVehicle();
	}
	if (isObject(%vb.bProps["Item"]))
	{
		%b.setItem(%vb.bProps["Item"]);
		%b.setItemDirection(%vb.bProps["Item", 0]);
		%b.setItemPosition(%vb.bProps["Item", 1]);
		%b.setItemRespawnTime(%vb.bProps["Item", 2]);
	}
}

function virtualBrickList::createBasicBrick(%obj, %i)
{
	%b = %obj.createGhostBrick(%i);
	%obj.applyCustomSaves(%i, %b);
	return %b;
}

function fxDTSBrick::vblPlant(%b)
{
	$Server_LoadFileObj = 1;
	%err = %b.plant();
	$Server_LoadFileObj = "";
	
	return %err;
}

function virtualBrickList::standardPlantBrick(%obj, %i, %b)
{
	%err = %b.vblPlant();
	//plant() returns an integer:
	//0 = plant successful
	//1 = blocked by brick
	//2 = no attachment points
	//3 = blocked by something else
	//4 = ground not level (baseplates only)
	//5 = burried
	if(%err == 1 || %err == 3 || %err == 5)
	{
		//error("ERROR: loadBricks() - Brick could not be placed!");
		%failureCount++;
		%b.delete();
		%b = 0;
	}
	else
	{
		%obj.applyPlantedProperties(%i, %b);
		%obj.onCreateBrick(%b);
	}
	return %b;
}

function virtualBrickList::createBrick(%obj, %i, %client, %overideClient)
{
	%b = %obj.createBasicBrick(%i);
	
	if (!isObject(%b))
		return 0;
	
	%brickGroup = "";
	if (isObject(%client))
	{
		%client.brickGroup.add(%b);
		%b.stackBL_ID = %client.bl_id;
	}
	else if ($Server::Lan)
	{
		BrickGroup_999999.add(%b);
		if (isObject(BrickGroup_999999.client)) %b.client = BrickGroup_999999.client;
		%b.stackBL_ID = BrickGroup_999999.bl_id;
	}
	else
	{
		ClientGroup.getObject(0).brickGroup.add(%b);
		%b.client = ClientGroup.getObject(0);
		%b.stackBL_ID = ClientGroup.getObject(0).bl_id;
	}

	%b = %obj.standardPlantBrick(%i, %b);
	
	return %b;
}

//custom save prefs will still give it an owner if available
function virtualBrickList::createBrickNoOwner(%obj, %i)
{
	%b = %obj.createBasicBrick(%i);
	
	if (!isObject(%b))
		return 0;
	%b = %obj.standardPlantBrick(%i, %b);
	
	return %b;
}

function virtualBrickList::onCreateBrick(%obj, %b)
{
	//override this
}

function virtualBrickList::loadBricks(%obj)
{
	for (%i = 0; %i < mainBrickGroup.getCount(); %i++)
	{
		%bg = mainBrickGroup.getObject(%i);
		for (%bn = 0; %bn < %bg.getCount(); %bn++)
		{
			%b = %bg.getObject(%bn);
			%obj.addRealBrick(%b);
		}
	}
}

function virtualBrickList::loadBLIDBricks(%obj, %id)
{
	%group = "BrickGroup_" @ %id;
	if (isObject(%group))
	{
		for (%i = 0; %i < %group.getCount(); %i++)
		{
			%obj.addRealBrick(%group.getObject(%i));
		}
	}
}

function virtualBrickList::addVBL(%obj, %vbl)
{
	for (%i = 0; %i < %vbl.getCount(); %i++)
	{
		%obj.addBrick(%vbl.getDatablock(%i), 
		%vbl.getPosition(%i), 
		%vbl.getAngleId(%i), 
		%vbl.isBP(%i), 
		%vbl.getColorId(%i), 
		%vbl.getPrint(%i), 
		%vbl.getColorFx(%i), 
		%vbl.getShapeFx(%i), 
		%vbl.isRaycasting(%i), 
		%vbl.isColliding(%i), 
		%vbl.isRendering(%i));
	}
}

function virtualBrickList::addSet(%obj, %set)
{
	for (%i = 0; %i < %set.getCount(); %i++)
	{
		%b = %set.getObject(%i);
		%obj.addRealBrick(%set.getObject(%i));
	}
}

function virtualBrickList::addRealBrick(%obj, %b)
{
			//time to add the bricks! %obj, %datablock, %pos, %angleid, %isBaseplate, %color, %print, %colorfx, %shapefx
			%num = %obj.addBrick(%b.getDataBlock(), %b.getPosition(), %b.getAngleId(), %b.isBaseplate(), %b.getColorId(), %b.getPrintId(), %b.getColorFxId(), %b.getShapeFxId(), %b.isRaycasting(), %b.isColliding(), %b.isRendering());
			%vb = %obj.getVirtualBrick(%num);
			//time for the special stuff
			if (isObject(%b.emitter))
			{
				%vb.props["Emitter"] = %b.emitter.emitter.getName();
				%vb.props["Emitter", 0] = %b.emitterDirection;
			}
			else
			{
				%vb.props["Emitter"] = "";
				%vb.props["Emitter", 0] = "";
			}
			if (isObject(%b.light))
				%vb.props["Light"] = %b.light.getDataBlock().getName();
			else
			{
				%vb.props["Light"] = "";
			}

			if (isObject(%b.item))
			{
				%vb.props["Item"] = %b.item.getDataBlock().getName();
				%vb.props["Item", 0] = %b.itemDirection;
				%vb.props["Item", 1] = %b.itemPosition;
				%vb.props["Item", 2] = %b.itemRespawnTime;
			}
			else
			{
				%vb.props["Item"] = "";
				%vb.props["Item", 0] = "";
				%vb.props["Item", 1] = "";
				%vb.props["Item", 2] = "";
			}
			if (isObject(%b.vehicleDatablock))
			{
				%vb.props["Vehicle"] = %b.vehicleDatablock;
				if (isObject(%b.vehicle)) %vb.props["Vehicle", 0] = 1;
				else %vb.props["Vehicle", 0] = 0;
			}
			else
			{
				%vb.props["Vehicle"] = "";
				%vb.props["Vehicle", 0] = 0;
			}
			for (%i = 0; %i < $numCustSaves; %i++)
			{
				%csName = $custSaves[%i, "name"];
				%obj.cs_addReal(%csName, %vb, %b);
			}
}

function virtualBrickList::addRealBuild(%obj, %brick, %incStr, %excStr)
{
	if (%incStr $= "")
		%incStr = "all";
	%obj.bf = new ScriptObject()
	{
		class = "BrickFinder";
	};
	
	%obj.bf.setOnSelectCommand(%obj @ ".onFoundRealBrick(%sb);");
	%obj.bf.setFinishCommand(%obj @ ".onFinishAddingBuild(" @ %obj.bf @ ");");
	
	%obj.bf.search(%brick, "chain", "all", "", 1);
}

function virtualBrickList::onFoundRealBrick(%obj, %sb)
{
	%obj.addRealBrick(%sb);
}

function virtualBrickList::onFinishAddingBuild(%obj, %bf)
{
	%obj.bf.delete();
}

function virtualBrickList::importBuild(%obj, %base, %getDown, %flash)
{
	if (!%base.getClassName() $= "fxDTSBrick")
		return;
	%selectBricks = new SimSet();
	%selectBricks.add(%base);
	%foundBricks = new SimSet();
	%foundBricks.add(%base);
	%justFoundBricks = new SimSet();
	%searchBricks = new SimSet();
	for (%i = 0; %i < %base.getNumUpBricks(); %i++)
	{
		%brick = %base.getUpBrick(%i);
		%justFoundBricks.add(%brick);
	}
	if (%getDown)
	{
		for (%i = 0; %i < %base.getDownUpBricks(); %i++)
		{
			%brick = %base.getDownBrick(%i);
			%justFoundBricks.add(%brick);
		}
	}
	//just found bricks and found bricks are seperate because if we put in more restrictions, we might need them
	while (%justFoundBricks.getCount())
	{
		for (%i = 0; %i < %justFoundBricks.getCount(); %i++)
		{
			%jb = %justFoundBricks.getObject(%i);
			if (!%foundBricks.isMember(%jb) && !%jb.noImport) //also other checks here if wanted
			{
				%searchBricks.add(%jb);
				%selectBricks.add(%jb);
			}
			%foundBricks.add(%jb);
		}
		%justFoundBricks.clear();
		for (%i = 0; %i < %searchBricks.getCount(); %i++)
		{
			%sb = %searchBricks.getObject(%i);
			for (%u = 0; %u < %sb.getNumUpBricks(); %u++)
				if (!%foundBricks.isMember(%sb.getUpBrick(%u))) %justFoundBricks.add(%sb.getUpBrick(%u));
			for (%b = 0; %b < %sb.getNumDownBricks(); %b++)
				if (!%foundBricks.isMember(%sb.getDownBrick(%b))) %justFoundBricks.add(%sb.getDownBrick(%b));
		}
		%searchBricks.clear();
	}
	%foundBricks.delete();
	%justFoundBricks.delete();
	%searchBricks.delete();
	for (%i = 0; %i < %selectBricks.getCount(); %i++)
	{
		%brick = %selectBricks.getObject(%i);
		%obj.addRealBrick(%brick);
	}
	%obj.copyNum = 0; //This is so copying stuff works sort of
	if (%flash)
		highlightBricks(%selectBricks);
	else
		%selectBricks.delete();
}

function highlightBricks(%bgroup, %stop)
{
	if (isObject(%bgroup))
	{
		for (%i = 0; %i < %bgroup.getCount(); %i++)
		{
			%brick = %bgroup.getObject(%i);
			if (!(%stop || %brick.highlighted))
			{
				%brick.origColor = %brick.getColorId();
				%brick.setColor(3);
				%brick.highlighted = 1;
			}
			else if (%stop && %brick.highlighted)
			{
				%brick.setColor(%brick.origColor);
				%brick.highlighted = 0;
			}
		}
		if (!%stop)
			schedule(1000, 0, "highlightBricks", %bgroup, 1);
	}
}

function virtualBrickList::shiftBricks(%obj, %dis)
{
	%obj.brickOffset = VectorAdd(%obj.brickOffset, %dis);
	%x = getWord(%dis, 0);
	%y = getWord(%dis, 1);
	%z = getWord(%dis, 2);
	
	%obj.maxX += %x;
	%obj.minX += %x;
	%obj.maxY += %y;
	%obj.minY += %y;
	%obj.maxZ += %z;
	%obj.minZ += %z;
	
	for (%i = 0; %i < %obj.markers.getCount(); %i++)
		%obj.markers.getObject(%i).shift(%dis);
}



function virtualBrickList::realign(%obj, %posStr)
{
	%dirs["north"] = 0;		%dirs[0] = 0;
	%dirs["east"] = 1;		%dirs[1] = 1;
	%dirs["south"] = 2;		%dirs[2] = 2;
	%dirs["west"] = 3;		%dirs[3] = 3;
	%dirs["up"] = 4;		%dirs[4] = 4;
	%dirs["down"] = 5;		%dirs[5] = 5;
	%dirs["centerX"] = 6;	%dirs[6] = 6;
	%dirs["centerY"] = 7;	%dirs[7] = 7;
	%dirs["centerZ"] = 8;	%dirs[8] = 8;
	%xOff = 0;
	%yOff = 0;
	%zOff = 0;
	for (%i = 0; %i < getFieldCount(%posStr); %i++)
	{
		%field = getField(%posStr, %i);
		if (getWordCount(%field) != 2)
			continue;
		%dir = getWord(%field, 0);
		%pos = getWord(%field, 1);
		
		//maybe I shouldn't use += ...
		switch (%dirs[%dir])
		{
			case 0:
				%yOff += %pos - %obj.maxY;
			case 1:
				%xOff += %pos - %obj.maxX;
			case 2:
				%yOff += %pos - %obj.minY;
			case 3:
				%xOff += %pos - %obj.minX;
			case 4:
				%zOff += %pos - %obj.maxZ;
			case 5:
				%zOff += %pos - %obj.minZ;
			case 6:
				%xOff += %pos - getWord(%obj.getCenter(), 0);
			case 7:
				%yOff += %pos - getWord(%obj.getCenter(), 1);
			case 8:
				%zOff += %pos - getWord(%obj.getCenter(), 2);
		}
	}
	if (%xOff != 0 || %yOff != 0 || %zOff != 0)
		%obj.shiftBricks(%xOff SPC %yOff SPC %zOff);
}

function virtualBrickList::alignNorthOf(%obj, %other)
{
	%obj.realign(0 SPC %other.getFace(2));
}
function virtualBrickList::alignEastOf(%obj, %other)
{
	%obj.realign(1 SPC %other.getFace(3));
}
function virtualBrickList::alignSouthOf(%obj, %other)
{
	%obj.realign(2 SPC %other.getFace(0));
}
function virtualBrickList::alignWestOf(%obj, %other)
{
	%obj.realign(3 SPC %other.getFace(1));
}
function virtualBrickList::alignTopOf(%obj, %other)
{
	%obj.realign(4 SPC %other.getFace(5));
	%obj.realign(5 SPC %other.getFace(4));
}
function virtualBrickList::alignBottomOf(%obj, %other)
{
	%obj.realign(5 SPC %other.getFace(4));
}

function virtualBrickList::recenter(%obj, %pos)
{
	%cen = %obj.getCenter();
	%dis = VectorSub(%pos, %cen);
	//%x = mFloor(getWord(%dis, 0) + 0.5);
	//%y = mFloor(getWord(%dis, 1) + 0.5);
	//%z = mFloor(getWord(%dis, 2) + 0.5);
	%obj.shiftBricks(%dis);
	
}

function virtualBrickList::getCenter(%obj)
{
	%centX = ((%obj.maxX - %obj.minX) / 2) + %obj.minX;
	%centY = ((%obj.maxY - %obj.minY) / 2) + %obj.minY;
	%centZ = ((%obj.maxZ - %obj.minZ) / 2) + %obj.minZ;
	return %centX SPC %centY SPC %centZ;
}

function virtualBrickList::getWorldBox(%obj)
{
	return %obj.minX SPC %obj.minY SPC %obj.minZ SPC %obj.maxX SPC %obj.maxY SPC %obj.maxZ;
}

function virtualBrickList::rotateBricksCW(%obj, %times)
{
	if (%times $= "") %times = 1;
	%times %= 4;
	if (!%times) return;
	
	//markers must be rotated before the bricks, because the center changes!
	for (%i = 0; %i < %obj.markers.getCount(); %i++)
	{
		%obj.markers.getObject(%i).rotateCW(%times);
	}	

	%cpos = %obj.getCenter();
	%cx = getWord(%cpos, 0);
	%cy = getWord(%cpos, 1);
	%cz = getWord(%cpos, 2);
	%obj.resetSize();
	for (%i = 0; %i < %obj.getCount(); %i++)
	{
		%pos = %obj.getPosition(%i);
		%x = getWord(%pos, 0);
		%y = getWord(%pos, 1);
		%z = getWord(%pos, 2);
		%ux = %x - %cx;
		%uy = %y - %cy;
		
		//rotation variable is so the setter method isn't continuously called
		%rot = %obj.getAngleId(%i);
		
		for (%d = 0; %d < %times; %d++)
		{
			%tx = %ux;
			%ux = %uy;
			%uy = %tx;
			%uy = -%uy;
			%rot++;
		}
		while (%rot > 3)
			%rot -= 4;
		
		%obj.setAngleId(%i, %rot);
		
		%obj.setPosition(%i, %ux + %cx SPC %uy + %cy SPC %z);
		//now give custom save properties a chance to change
		%vb = %obj.getVirtualBrick(%i);
		for (%c = 0; %c < $numCustSaves; %c++)
		{
			%csName = $custSaves[%c, "name"];
			if (%vb.props[%csName] !$= "")
				%obj.cs_rotateCW(%csName, %vb, %times);
		}
		%obj.onAddBasicData(%i);
	}
}

function virtualBrickList::rotateBricksCCW(%obj, %times)
{
	if (%times $= "") %times = 1;
	%times %= 4;
	if (!%times) return;
	
	//markers must be rotated before the bricks, because the center changes!
	for (%i = 0; %i < %obj.markers.getCount(); %i++)
	{
		%obj.markers.getObject(%i).rotateCCW(%times);
	}
	
	%cpos = %obj.getCenter();
	%cx = getWord(%cpos, 0);
	%cy = getWord(%cpos, 1);
	%cz = getWord(%cpos, 2);
	%obj.resetSize();
	for (%i = 0; %i < %obj.getCount(); %i++)
	{
		%pos = %obj.getPosition(%i);
		%x = getWord(%pos, 0);
		%y = getWord(%pos, 1);
		%z = getWord(%pos, 2);
		%ux = %x - %cx;
		%uy = %y - %cy;
		
		//rotation variable is so the setter method isn't continuously called
		%rot = %obj.getAngleId(%i);
		
		for (%d = 0; %d < %times; %d++)
		{
			%tx = %ux;
			%ux = %uy;
			%uy = %tx;
			%ux = -%ux;
			%rot--;
		}
		while (%rot < 0)
			%rot += 4;
			
		%obj.setAngleId(%i, %rot);
		
		%obj.setPosition(%i, %ux + %cx SPC %uy + %cy SPC %z);
		//now give custom save properties a chance to change
		%vb = %obj.getVirtualBrick(%i);
		for (%c = 0; %c < $numCustSaves; %c++)
		{
			%csName = $custSaves[%c, "name"];
			if (%vb.props[%csName] !$= "")
				%obj.cs_rotateCCW(%csName, %vb, %times);
		}
		%obj.onAddBasicData(%i);
	}
}

function virtualBrickList::setAbsDirection(%obj, %direction)
{
	while (%direction > 3)
		%direction -= 4;
	while (%direction < 0)
		%direction += 4;
	if (%direction < %obj.absoluteRotation)
		%direction += 4;
	%dif = %direction - %obj.absoluteRotation;
	if (%dif > 0)
		%obj.rotateBricksCW(%dif);
	//else if (%dif < 0)
	//	%obj.rotateBricksCWW(%dif);
	while (%direction > 3)
		%direction -= 4;
	while (%direction < 0)
		%direction += 4;
	%obj.absoluteRotation = %direction;
}

function findClientByBlId(%id)
{
	for (%i = 0; %i < ClientGroup.getCount(); %i++)
	{
		%client = ClientGroup.getObject(%i);
		if (%client.bl_id == %id)
			return %client;
	}
	return 0;
}

//stuff that should go in the brick manipulation class
function virtualBrickList::onAddBasicData(%obj, %num)
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

function virtualBrickList::resetSize(%obj)
{
	%obj.maxX = "";
	%obj.maxY = "";
	%obj.maxZ = "";
	%obj.minX = "";
	%obj.minY = "";
	%obj.minZ = "";
}

function virtualBrickList::getOffset(%obj)
{
	return %obj.brickOffset;
}

function virtualBrickList::getCount(%obj)
{
	return %obj.vBricks.getCount();
}

function virtualBrickList::getNorthFace(%obj)
{
	return %obj.maxY;
}

function virtualBrickList::getSouthFace(%obj)
{
	return %obj.minY;
}

function virtualBrickList::getWestFace(%obj)
{
	return %obj.minX;
}

function virtualBrickList::getEastFace(%obj)
{
	return %obj.maxX;
}

function virtualBrickList::getBottomFace(%obj)
{
	return %obj.minZ;
}

function virtualBrickList::getTopFace(%obj)
{
	return %obj.maxZ;
}

function virtualBrickList::getFace(%obj, %dir)
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

//markers
function virtualBrickList::addMarker(%obj, %name, %point, %pDir, %sDir)
{
	if (isObject(%obj.markers[%name]))
	{
		error("ERROR: virtualBrickList::addMarker - marker name already used");
	}
	else if (%pDir < 0 || %pDir > 5)
	{
		error("ERROR: virtualBrickList::addMarker - primary direction is incorrect");
	}
	else
	{
		if (%sDir == "")
		{
			if (%pDir < 4)
				%sDir = 4;
			else
				%sDir = 0;
		}
		if ((%sDir < 4 && %pDir < 4) || (%sDir > 3 && %pDir > 3))
		{
			error("ERROR: virtualBrickList::addMarker - secondary direction is incorrect");
		}
		else
		{
			%mark = new ScriptObject()
			{
				class = "vblMarker";
				name = %name;
				position = %point;
				primary = %pDir;
				secondary = %sDir;
				vbl = %obj;
				name = %name;
			};
			%obj.markers[%name] = %mark;
			%obj.markers.add(%mark);
			return %mark;
		}
	}
}

function virtualBrickList::removeMarker(%obj, %name)
{
	if (isObject(%obj.markers[%name]))
	{
		%obj.markers.remove(%obj.markers[%name]);
		%obj.markers[%name].delete();
		%obj.markers[%name] = "";
	}
}

function virtualBrickList::getMarkerPrimary(%obj, %name)
{
	return %obj.markers[%name].primary;
}

function virtualBrickList::getMarkerSecondary(%obj, %name)
{
	return %obj.markers[%name].secondary;
}

function virtualBrickList::getMarkerPosition(%obj, %name)
{
	return %obj.markers[%name].position;
}

function vblMarker::shift(%obj, %dis)
{
	%obj.position = VectorAdd(%obj.position, %dis);
}

function vblMarker::rotateCW(%obj, %times)
{
	if (%times < 0)
	{
		%obj.rotateCCW(-%times);
	}
	else
	{
		//update position
		%cpos = %obj.vbl.getCenter();
		%cx = getWord(%cpos, 0);
		%cy = getWord(%cpos, 1);
		%cz = getWord(%cpos, 2);
		
		%x = getWord(%obj.position, 0);
		%y = getWord(%obj.position, 1);
		%z = getWord(%obj.position, 2);
		%ux = %x - %cx;
		%uy = %y - %cy;
		for (%d = 0; %d < %times; %d++)
		{
			%tx = %ux;
			%ux = %uy;
			%uy = %tx;
			%uy = -%uy;
		}
		%obj.position = %ux + %cx SPC %uy + %cy SPC %z;
		
		//update rotation
		if (%obj.primary < 4)
		{
			%obj.primary += %times;
			%obj.primary %= 4;
		}
		else
		{
			%obj.secondary += %times;
			%obj.secondary %= 4;
		}
	}
}

//align the markers against eachother
function vblMarker::alignWith(%obj, %marker)
{
	if (%obj.primary < 4 && %marker.primary < 4)
	{
		//only have to align the primary
		%rotNeeded = %marker.primary + 2;
		%rotNeeded %= 4;
		if (%rotNeeded < %obj.primary)
			%rotNeeded += 4;
		%rot = %rotNeeded - %obj.primary;
		%obj.vbl.rotateBricksCW(%rot);
		%obj.vbl.shiftbricks(VectorSub(%marker.position, %obj.position));
	}
	else if (%obj.primary > 3 && %marker.primary > 3 && (%obj.primary != %marker.primary))
	{
		//only have to align the primary
		%rotNeeded = %marker.secondary + 2;
		%rotNeeded %= 4;
		if (%rotNeeded < %obj.secondary)
			%rotNeeded += 4;
		%rot = %rotNeeded - %obj.secondary;
		%obj.vbl.rotateBricksCW(%rot);
		%obj.vbl.shiftbricks(VectorSub(%marker.position, %obj.position));
	}
	else
	{
		error("ERROR: vblMarker::alignWith - directions don't match up");
	}
}

//align the markers on top of eachother
function vblMarker::alignOnto(%obj, %marker)
{
	if (%obj.primary < 4 && %marker.primary < 4)
	{
		//only have to align the primary
		%rotNeeded %= 4;
		if (%rotNeeded < %obj.primary)
			%rotNeeded += 4;
		%rot = %rotNeeded - %obj.primary;
		%obj.vbl.rotateBricksCW(%rot);
		%obj.vbl.shiftbricks(VectorSub(%marker.position, %obj.position));
	}
	else if (%obj.primary > 3 && %marker.primary > 3 && (%obj.primary != %marker.primary))
	{
		//only have to align the primary
		%rotNeeded %= 4;
		if (%rotNeeded < %obj.secondary)
			%rotNeeded += 4;
		%rot = %rotNeeded - %obj.secondary;
		%obj.vbl.rotateBricksCW(%rot);
		%obj.vbl.shiftbricks(VectorSub(%marker.position, %obj.position));
	}
	else
	{
		error("ERROR: vblMarker::alignWith - directions don't match up");
	}
}

function vblMarker::rotateCCW(%obj, %times)
{
	if (%times < 0)
	{
		%obj.rotateCW(-%times);
	}
	else
	{
		%times %= 4;
		if (%times > 0)
			%obj.rotateCW(4 - %times);
	}
}

};

//The following function was blatantly modified from the duplicator
function VirtualBrickList::getPrintName(%obj, %num)
{
	if(%obj.getDataBlock(%num).subCategory $= "Prints")
	{
		%texture = getPrintTexture(%obj.getPrint(%num, 5));
		%path = filePath(%texture);
		%underscorePos = strPos(%path, "_");
		%name = getSubStr(%path, %underscorePos + 1, strPos(%path, "_", 14) - 14) @ "/" @ fileBase(%texture);
		if($printNameTable[%name] !$= "")
			return %name;
	}
	
	return "";
}

activatePackage(vblPackage);
