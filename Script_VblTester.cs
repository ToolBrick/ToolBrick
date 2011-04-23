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
		messageClient(%client, '', %client.testVbl.numBricks);
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
			echo("dasd");
			%obj.schedule(100, "delete");
		}
	}
};

activatePackage(VBLTester);