//The point of this script is to make a system that can easily locate special bricks
//The manipulator/virtualbricklist will be the first to use this but it should be very useful for
//brick gamemodes

function brickFinder::onAdd(%obj)
{
	%obj.brickGroups = new SimSet();
	%obj.finishCommand = "echo(\"set a finish command!\");";
}

function brickFinder::onRemove(%obj)
{
	%obj.cleanup();
	%obj.brickGroups.delete();
}


function addBFFindMethod(%name, %funcName)
{
	if ($BF::FindMethods[%name] $= "" && isFunction(%funcName))
	{
		$BF::FindMethods[%name] = %funcName;
		return 1;
	}
	else
		return -1;
}

function addBFType(%name, %funcName)
{
	if ($BFNumTypeFunctions $= "")
		$BFNumTypeFunctions = 0;
	if ($BF::Types[%name] $= "" && isFunction(%funcName))
	{
		$BF::TypeFunctions[$BFNumTypeFunctions] = %funcName;
		$BF::TypeNames[$BFNumTypeFunctions] = %name;
		$BF::Types[%name] = $BFNumTypeFunctions;
		$BFNumTypeFunctions++;
		return 1;
	}
	else
		return -1;
}
//bf.search(clientgroup.getObject(0).wrenchBrick, "chain", "all");
//This method just sets up, actual searching is in another method so we can easily stretch out the search
function brickFinder::search(%obj, %base, %findMethod, %includeTypes, %excludeTypes, %searchFromExcluded)
{
	%obj.cleanup();
	if (!%base.getClassName() $= "fxDTSBrick")
		return;
	if (%searchFromExcluded $= "")
		%searchFromExcluded = 1;
	%obj.selectBricks = new SimSet();
	%obj.foundBricks = new SimSet();
	%obj.justFoundBricks = new SimSet();
	%obj.searchBricks = new SimSet();
	%numFakes = 0;
	for (%f = 0; %f < getFieldCount(%includeTypes); %f++)
	{
		%field = getField(%includeTypes, %f);
		if (%field $= "")
		{
			%numFakes++;
			continue;
		}
		%idName = getWord(%field, 0);
		%group = new SimSet();
		%group.id = $BF::Types[%idName];
		%evalString = $BF::TypeFunctions[%group.id] @ "(\t";
		for (%a = 1; %a < getWordCount(%field); %a++)
		{
			%evalString = %evalString @ ", \"" @ getWord(%field, %a) @ "\"";
		}
		%evalString = %evalString @ ");";
		%obj.incTypes[%f] = %evalString;
		%obj.brickGroups.add(%group);
		%obj.idGroup[%group.id] = %group;
	}
	%obj.numIncTypes = %f - %numFakes;
	%numFakes = 0;
	echo("exclude ty[es!" SPC %excludeTypes);
	for (%f = 0; %f < getFieldCount(%excludeTypes); %f++)
	{
		%field = getField(%excludeTypes, %f);
		if (%field $= "")
		{
			%numFakes++;
			continue;
		}
		%obj.excTypes[%f] = $BF::TypeFunctions[$BF::Types[%field]];
		%idName = getWord(%field, 0);
		%id = $BF::Types[%idName];
		%evalString = $BF::TypeFunctions[%id] @ "(\t";
		echo("exc done: " @ %obj.excTypes[%f] SPC %evalString);
		for (%a = 1; %a < getWordCount(%field); %a++)
		{
			%evalString = %evalString @ ", \"" @ getWord(%field, %a) @ "\"";
		}
		%evalString = %evalString @ ");";
		%obj.excTypes[%f] = %evalString;
	}
	%obj.numExcTypes = %f - %numFakes;
	%obj.searchFromExcluded = %searchFromExcluded;
	%obj.findMethod = $BF::FindMethods[%findMethod];
	%obj.searchBricks.add(%base);
	%obj.justFoundBricks.add(%base);
	%obj.i = 1;
	
	%obj.continueSearch();
}

function brickFinder::continueSearch(%obj)
{
	%startTime = getSimTime();
	while (%obj.searchBricks.getCount() || %obj.inspectingBricks)
	{
		if (!%obj.inspectingBricks)
		{
			for (%obj.i = %obj.i; %obj.i < %obj.searchBricks.getCount(); %obj.i++)
			{	
				%brick = %obj.searchBricks.getObject(%obj.i);
				call(%obj.findMethod, %brick, %obj.justFoundBricks);
			}
			%obj.i = 0;
			%obj.searchBricks.clear();
			%obj.inspectingBricks = 1;
			%obj.b = 0;
		}
		for (%obj.b = 0; %obj.b < %obj.justFoundBricks.getCount(); %obj.b++)
		{
			%inspectedBricks++;
			%sb = %obj.justFoundBricks.getObject(%obj.b);
			//echo("hi!" SPC %obj.justFoundBricks.getCount() SPC %sb);
			if (%obj.foundBricks.isMember(%sb))
			{
				//echo("OH NO!");
				%obj.justFoundBricks.remove(%sb);
				%obj.b--;
				continue;
			}
			%obj.foundBricks.add(%sb);
				
			%change = false;
			for (%et = 0; %et < %obj.numExcTypes; %et++)
			{
				echo("exc" SPC getField(%obj.excTypes[%et], 0) @ %sb @ getField(%obj.excTypes[%et], 1));
				if (eval(getField(%obj.excTypes[%et], 0) @ %sb @ getField(%obj.excTypes[%et], 1)))
				{
					%obj.justFoundBricks.remove(%sb);
					%obj.b--;
					%change = true;
					break;
				}
			}
			if (%change)
			{
				if (%obj.searchFromExcluded)
					%obj.searchBricks.add(%sb);
				continue;
			}
			for (%it = 0; %it < %obj.numIncTypes; %it++)
			{
				if (eval(getField(%obj.incTypes[%it], 0) @ %sb @ getField(%obj.incTypes[%it], 1)))
				{
					if (!%change)
					{
						%obj.selectBricks.add(%sb);
						if (%obj.onSelectCommand !$= "")
							eval(%obj.onSelectCommand);
						%change = true;
					}
					%obj.brickGroups.getObject(%it).add(%sb);
				}
			}
			%obj.searchBricks.add(%sb);
			if (%inspectedBricks > 50)//%now - %startTime > 50)
			{
				%obj.findSchedule = %obj.schedule(50, "continueSearch");
				return;
			}
		}
		%obj.inspectingBricks = 0;
		%obj.justFoundBricks.clear();
		%now = getSimTime();
		if (%inspectedBricks > 500)//%now - %startTime > 50)
		{
			echo(%now SPC %startTime);
			%obj.findSchedule = %obj.schedule(100, "continueSearch");
			return;
		}
		//check here everytime and take a break if needed
		//prepare for the next search
	}
	echo(%now SPC %startTime);
	eval(%obj.finishCommand);
}

function brickFinder::setFinishCommand(%obj, %command)
{
	%obj.finishCommand = %command;
}

function brickFinder::setOnSelectCommand(%obj, %command)
{
	%obj.onSelectCommand = %command;
}

function brickFinder::cleanup(%obj)
{
	if (isEventPending(%obj.findSchedule))
		cancel(%obj.findSchedule);
	while (%obj.brickGroups.getCount())
	{
		%group = %obj.brickGroups.getObject(0);
		%obj.idGroup[%group.id] = "";
		%group.delete();
	}
	%obj.numIncTypes = 0;
	%obj.numExcTypes = 0;
	%obj.searchFromExcluded = 0;
	%obj.inspectingBricks = 0;
	%obj.i = 0;
	%obj.b = 0;
	if (isObject(%obj.selectBricks))
		%obj.selectBricks.delete();
	if (isObject(%obj.foundBricks))
		%obj.foundBricks.delete();
	if (isObject(%obj.justFoundBricks))
		%obj.justFoundBricks.delete();
}

function testBF(%off, %client)
{
	if (%client $= "") %client = clientgroup.getObject(0);
	if (!isObject(bf))
		new ScriptObject(bf) {class = "brickFinder";};
	bf.setFinishCommand("colorGroup(" @ %off @ ");");
	bf.search(%client.wrenchBrick, "chain", "bfColorBricks 0\tbfColorBricks 1");
}

function colorGroup(%off)
{
	for (%g = 0; %g < bf.brickGroups.getCount(); %g++)
	{
		%group = bf.brickGroups.getObject(%g);
		echo(%group.getCount());
		echo(%group.id);
		for (%i = 0; %i < %group.getCount(); %i++)
		{
			%group.getObject(%i).setColor(%g + %off);
		}
	}
}

function serverCmdTestBF(%client)
{
	testBF(getRandom(20), %client);
}

function ServerCmdGetManipulatorFilters(%client)
{
	for (%i = 0; %i < $BFNumTypeFunctions; %i++)
	{
		commandToClient(%client, 'AddManipulatorFilter', $BF::TypeNames[%i]);
	}
}

function ServerCmdResetManipulator(%client)
{
	%client.manipNumIL = 0;
	%client.manipNumEL = 0;
}

function ServerCmdAddMIF(%client, %typeId, %args)
{
	if (%client.manipNumIL $= "")
		%client.manipNumIL = 0;
		
	%client.manipILs[%client.manipNumIL] = $BF::TypeNames[%typeId] SPC %args;
	%client.manipNumIL++;
}

function ServerCmdAddMEF(%client, %typeId, %args)
{
	if (%client.manipNumEL $= "")
		%client.manipNumEL = 0;
	echo(%typeId SPC $BF::TypeNames[%typeId] SPC %args);
	%client.manipELs[%client.manipNumEL] = $BF::TypeNames[%typeId] SPC %args;
	%client.manipNumEL++;
}