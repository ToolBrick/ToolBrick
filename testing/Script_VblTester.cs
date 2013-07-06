function ServerCmdTestVbl(%client)
{
	if (%client.isAdmin)
	{
		if (isObject(%client.testVbl))
			%client.testVbl.delete();
		%client.testVbl = newVBL();
	}
}

function ServerCmdAddWrenchbricks(%client)
{
	if (%client.isAdmin)
	{
		%brick = %client.wrenchbrick;
		%bf = newBrickFinder("vblTester");
		%bf.vbl = %client.testVbl;
		%bf.search(%client.wrenchbrick, "chain", "all \t", "", 1);
	}
}

function ServerCmdTestVblCount(%client)
{
	if (%client.isAdmin)
	{
		messageClient(%client, '', %client.testVbl.getCount());
	}
}

function ServerCmdTestVblSave(%client, %name)
{
	if (%client.isAdmin)
	{
		%client.testVbl.exportBLSFile("saves/testVbl/" @ %name @ ".bls");
	}
}

function ServerCmdTestVblLoad(%client, %name)
{
	if (%client.isAdmin)
	{
		%client.testVbl.loadBLSFile("saves/testVbl/" @ %name @ ".bls");
	}
}

function ServerCmdTestVblAddMarker(%client, %name, %px, %py, %pz, %pd, %sd)
{
	if (%client.isAdmin)
	{
		%client.testVbl.addMarker(%name, %px SPC %py SPC %pz, %pd, %sd);
	}
}

function ServerCmdTestVblMCount(%client)
{
	if (%client.isAdmin)
	{
		messageClient(%client, '', %client.testVbl.markers.getCount());
	}
}

function ServerCmdTestVblMInfo(%client, %num)
{
	if (%client.isAdmin)
	{
		%marker = %client.testVbl.markers.getObject(%num);
		messageClient(%client, '', %marker.name @ "|" @ %marker.position @ "|" @ %marker.primary @ "|" @ %marker.secondary);
	}
}

function ServerCmdTestVblId(%client)
{
	messageClient(%client, '', %client.testVbl);
}

function VirtualBrickList::preview(%obj)
{
	if (!isObject(%obj.previewBricks))
		%obj.previewBricks = new SimSet();
	for (%i = 0; %i < %obj.getCount(); %i++)
	{
		%obj.previewBricks.add(%obj.previewBrick(%i));
	}
}

function VirtualBrickList::previewBrick(%obj, %i, %client, %overideClient)
{
	%db = %obj.getDatablock(%i);
	if (!isObject(%db))
	{
		error("data block does not exist!!!" SPC %db);
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
	};
	if (!isObject(%b))
	{
		error("Brick not created!" SPC %db);
		return;
	}
	if(isObject(DoorSO) && DoorSO.getIDFromDatablockBrick(%db) > -1)
	{
		%brick.noDefaultDoorEvents = 1;
	}
	%b.setRaycasting(%raycasting);
	%b.setColliding(%collision);
	%b.setRendering(%rendering);
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
	
	%b.setTrusted(1);
	%b.setTransform(%trans);
	
	return %b;
	//add code to handle the emitters and other +- stuff
}


function VirtualBrickList::deletePreview(%obj)
{
	if (isObject(%obj.previewBricks))
		while (%obj.previewBricks.getCount())
			%obj.previewBricks.getObject(0).delete();
}

package VBLTester
{
	function brickFinder::onSelect(%obj, %brick)
	{
		if (%obj.type $= "vblTester")
		{
			%obj.vbl.addRealBrick(%brick);
		}
	}
	
	function brickFinder::onFinish(%obj, %list)
	{
		if (%obj.type $= "vblTester")
		{
			%obj.schedule(100, "delete");
		}
	}
	
	function VirtualBrickList::onRemove(%this, %obj)
	{
		%obj.deletePreview();
		if (isObject(%obj.previewBricks))
			%obj.previewBricks.delete();
		Parent::onRemove(%this, %obj);
	}
};

activatePackage(VBLTester);