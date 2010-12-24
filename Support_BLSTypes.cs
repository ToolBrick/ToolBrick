//useful event functions:
//outputEvent_GetNumParametersFromIdx(%targetClass, %outputIdx)
//outputEvent_GetOutputName
//outputEvent_GetOutputEventIdx(%targetclass, %outputevent)

//inputEvent_GetTargetName
//inputEvent_GetTargetClass("fxDTSBrick", %inputIDx, %targetIDx)
//inputEvent_GetTargetIndex("fxDTSBrick", %inputIDx, %target)
//inputEvent_GetInputEventIdx("fxDTSBrick", number, number) returns 

addCustSave("EVENT");
function virtualBrickList::cs_addReal_EVENT(%obj, %num, %brick)
{
	if (%brick.numEvents)
	{
		%obj.virBricks[%num, "EVENT"] = %brick.numEvents;
		for (%i = 0; %i < %brick.numEvents; %i++)
		{
			%obj.virBricks[%num, "EVENT", "Delay", %i] = %brick.eventDelay[%i];
			%obj.virBricks[%num, "EVENT", "Enabled", %i] = %brick.eventEnabled[%i];
			%obj.virBricks[%num, "EVENT", "Input", %i] = %brick.eventInput[%i];
			%obj.virBricks[%num, "EVENT", "InputIdx", %i] = %brick.eventInputIdx[%i];
			%obj.virBricks[%num, "EVENT", "NT", %i] = %brick.eventNT[%i];
			%obj.virBricks[%num, "EVENT", "Output", %i] = %brick.eventOutput[%i];
			%obj.virBricks[%num, "EVENT", "OutputAppendClient", %i] = %brick.eventOutputAppendClient[%i];
			%obj.virBricks[%num, "EVENT", "OutputIdx", %i] = %brick.eventOutputIdx[%i];
			for (%op = 1; %brick.eventOutputParameter[%i, %op] !$= ""; %op++)
				%obj.virBricks[%num, "EVENT", "OutputParameter", %i, %op] = %brick.eventOutputParameter[%i, %op];
			%obj.virBricks[%num, "EVENT", "Target", %i] = %brick.eventTarget[%i];
			%obj.virBricks[%num, "EVENT", "TargetIdx", %i] = %brick.eventTargetIdx[%i];
		}
	}
	else %obj.virBricks[%num, "EVENT"] = 0;
}

function virtualBrickList::cs_create_EVENT(%obj, %num, %brick)
{
	for (%i = 0; %i < %obj.virBricks[%num, "EVENT"]; %i++)
	{
		//I hate dealing with targets, lets get this over with first
		//well I was going to only save the target and not the id but then I remembered
		//this SO is also for easy access to brick properties, that wouldn't do!
		///%target = %obj.virBricks[%num, "EVENT", "Target", %i];
		%targetIndex = %obj;
		%brick.eventDelay[%i] = %obj.virBricks[%num, "EVENT", "Delay", %i];
		%brick.eventEnabled[%i] = %obj.virBricks[%num, "EVENT", "Enabled", %i];
		%brick.eventInput[%i] = %obj.virBricks[%num, "EVENT", "Input", %i];
		%brick.eventInputIdx[%i] = %obj.virBricks[%num, "EVENT", "InputIdx", %i];
		//%brick.eventInputIdx[%i] = inputEvent_GetInputEventIdx(%brick.eventInput);
		%brick.eventNT[%i] = %obj.virBricks[%num, "EVENT", "NT", %i] @ "_" @ %obj.copyNum;
		%brick.eventOutput[%i] = %obj.virBricks[%num, "EVENT", "Output", %i];
		%brick.eventOutputAppendClient[%i] = %obj.virBricks[%num, "EVENT", "OutputAppendClient", %i];
		%brick.eventOutputIdx[%i] = %obj.virBricks[%num, "EVENT", "OutputIdx", %i];
		//%brick.eventOutputIdx = outputEvent_GetOutputEventIdx(%brick.eventOutput);
		for (%op = 1; %obj.virBricks[%num, "EVENT", "OutputParameter", %i, %op] !$= ""; %op++)
			%brick.eventOutputParameter[%i, %op] = %obj.virBricks[%num, "EVENT", "OutputParameter", %i, %op];
		%brick.eventTarget[%i] = %obj.virBricks[%num, "EVENT", "Target", %i];
		%brick.eventTargetIdx[%i] = %obj.virBricks[%num, "EVENT", "TargetIdx", %i];
		%brick.numEvents = %obj.virBricks[%num, "EVENT"];
	}
}

function virtualBrickList::cs_rotateCW_Event(%obj, %num, %times)
{
	echo("Event rotation");
	//is it a good idea to declare constants inside a function called multiple times?
	//this will probably be switched to some globals
	%relays["fireRelayNorth"] = 1;
	%relays["fireRelayEast"] = 2;
	%relays["fireRelaySouth"] = 3;
	%relays["fireRelayWest"] = 4;
	%relayNames[1] = "fireRelayNorth";
	%relayNames[2] = "fireRelayEast";
	%relayNames[3] = "fireRelaySouth";
	%relayNames[4] = "fireRelayWest";
	for (%i = 0; %i < %obj.virBricks[%num, "EVENT"]; %i++)
	{
		%relayNum = %relays[%obj.virBricks[%num, "EVENT", "Output", %i]];
		if (%relayNum)
		{
			%relayNum += %times;
			while (%relayNum > 4) %relayNum -= 4;
			%obj.virBricks[%num, "EVENT", "Output", %i] = %relayNames[%relayNum];
			%obj.virBricks[%num, "EVENT", "OutputIdx", %i] = outputEvent_GetOutputEventIdx("fxDTSBrick", %relayNames[%relayNum]); //relays are fxdtsbrick stuff
		}
		%paras = $OutputEvent_parameterList[inputEvent_getTargetClass("fxDTSBrick"), %obj.virBricks[%num, "EVENT", "OutputIdx", %i]];
		%paraCount = %obj.virBricks[%num, "EVENT", "OutputIdx", %i];//can't we just do getFieldCount()? using this because it's here
		outputEvent_GetNumParametersFromIdx(%targetClass, %obj.virBricks); //now check the event's output parameters for the vector type It'd be nice if there was a more efficient way than search for every brick every rotation
		for (%t = 0; %t < %times; %t++)
		{
			
		}
	}
}

function virtualBrickList::cs_rotateCCW_Event(%obj, %num, %times)
{
	echo("Event rotation 2");
}

function virtualBrickList::cs_save_EVENT(%obj, %num, %file)
{
	for (%i = 0; %i < %obj.virBricks[%num, "EVENT"]; %i++)
	{
		%targets = $Input["Event", "TargetListfxDTSBrick",  %obj.virBricks[%num, "EVENT", "TargetIdx", %i]];
		%target = getWord(getField(%targets, %obj.virBricks[%num, "EVENT", "TargetIdx", %i]), 1);
		%paraList = $Output["Event", "parameterList" @ %target, %obj.virBricks[%num, "EVENT", "OutputIdx", %i]];
		%outputParameters = "";
		for (%op = 1; %obj.virBricks[%num, "EVENT", "OutputParameter", %i, %op] !$= ""; %op++)
			%outputParameters = %outputParameters @ %obj.virBricks[%num, "EVENT", "OutputParameter", %i, %op] @ "\t";
		%file.writeLine("+-EVENT" TAB
		%obj.virBricks[%num, "EVENT", "Delay", %i] @ "\t" @
		%obj.virBricks[%num, "EVENT", "Enabled", %i] @ "\t" @
		%obj.virBricks[%num, "EVENT", "Input", %i] @ "\t" @
		%obj.virBricks[%num, "EVENT", "InputIdx", %i] @ "\t" @
		%obj.virBricks[%num, "EVENT", "NT", %i] @ "\t" @
		%obj.virBricks[%num, "EVENT", "Output", %i] @ "\t" @
		%obj.virBricks[%num, "EVENT", "OutputAppendClient", %i] @ "\t" @
		%obj.virBricks[%num, "EVENT", "OutputIdx", %i] @ "\t" @
		%obj.virBricks[%num, "EVENT", "Target", %i] @ "\t" @
		%obj.virBricks[%num, "EVENT", "TargetIdx", %i] @ "\t" @
		%outputParameters);
	}
}

function virtualBrickList::cs_load_EVENT(%obj, %num, %addData, %addInfo, %addArgs, %line)
{
echo("called" SPC %num);
	%obj.virBricks[%num, "EVENT"]++;
	%i = %obj.virBricks[%num, "EVENT"] - 1;
	%obj.virBricks[%num, "EVENT", "Delay", %i] = getField(%line, 1);
	%obj.virBricks[%num, "EVENT", "Enabled", %i] = getField(%line, 2);
	%obj.virBricks[%num, "EVENT", "Input", %i] = getField(%line, 3);
	%obj.virBricks[%num, "EVENT", "InputIdx", %i] = getField(%line, 4);
	%obj.virBricks[%num, "EVENT", "NT", %i] = getField(%line, 5);
	%obj.virBricks[%num, "EVENT", "Output", %i] = getField(%line, 6);
	%obj.virBricks[%num, "EVENT", "OutputAppendClient", %i] = getField(%line, 7);
	%obj.virBricks[%num, "EVENT", "OutputIdx", %i] = getField(%line, 8);
	%obj.virBricks[%num, "EVENT", "Target", %i] = getField(%line, 9);
	%obj.virBricks[%num, "EVENT", "TargetIdx", %i] = getField(%line, 10);
	for (%op = 11; %op < getFieldCount(%line); %op++)
		%obj.virBricks[%num, "EVENT", "OutputParameter", %i, %op - 10] = getField(%line, %op);
}




addCustSave("NTOBJECTNAME");
function virtualBrickList::cs_addReal_NTOBJECTNAME(%obj, %num, %brick)
{
	if (strLen(%brick.getName()) > 0) %obj.virBricks[%num, "NTOBJECTNAME"] = %brick.getName();
	else %obj.virBricks[%num, "NTOBJECTNAME"] = "";
}

function virtualBrickList::cs_create_NTOBJECTNAME(%obj, %num, %brick)
{
	if (strLen(%obj.virBricks[%num, "NTOBJECTNAME"]) > 0)
	{
		%brick.setNTObjectName(%obj.virBricks[%num, "NTOBJECTNAME"] @ "_" @ %obj.copyNum);
	}
}

function virtualBrickList::cs_save_NTOBJECTNAME(%obj, %num, %file)
{
	if (strLen(%obj.virBricks[%num, "NTOBJECTNAME"]) > 0)
		%file.writeLine("+-NTOBJECTNAME" SPC %obj.virBricks[%num, "NTOBJECTNAME"]);
}

function virtualBrickList::cs_load_NTOBJECTNAME(%obj, %num, %addData, %addInfo, %addArgs, %line)
{
	%obj.virBricks[%num, "NTOBJECTNAME"] = %addInfo;
}




addCustSave("OWNER");

function virtualBrickList::cs_addReal_OWNER(%obj, %num, %b)
{
	if (%b.getGroup().bl_id !$= "")
		%obj.virBricks[%num, "OWNER"] = %b.getGroup().bl_id;
	else
		%obj.virBricks[%num, "OWNER"] = "";
}

function virtualBrickList::cs_create_OWNER(%obj, %num, %b)
{
	if (%obj.virBricks[%num, "OWNER"] $= "")
		return;
	%brickGroupName = "BrickGroup_" @ %obj.virBricks[%num, "OWNER"];
	%brickGroup = %brickGroupName.getId();
	if (!isObject(%brickGroup))
	{
		new SimGroup(%brickGroupName);
		%brickGroup = %brickGroupName.getId();
		%brickGroup.bl_id = %obj.virBricks[%num, "OWNER"];
		%brickGroup.name = "BL_ID:" SPC %obj.virBricks[%num, "OWNER"];
		//%idClient = findClientByBlId(%obj.virBricks[%i, "OWNER"]); //wait, if the group isn't created, the client must not be here!
		//if (isObject(%idClient))
		//{
		//	%brickGroup.client = %idClient;
		//	%brickGroup.name = %idClient.name;
		//}
		mainBrickGroup.add(%brickGroup);
	}
	%brickGroup.add(%b);	
	if (isObject(%brickGroup.client)) %b.client = %brickGroup.client;
	%b.stackBL_ID = %brickGroup.bl_id;
}

function virtualBrickList::cs_save_OWNER(%obj, %num, %file)
{
	if (%obj.virBricks[%num, "OWNER"] !$= "")
		%file.writeLine("+-OWNER " @ %obj.virBricks[%num, "OWNER"]);
}

function virtualBrickList::cs_load_OWNER(%obj, %num, %addData, %addInfo, %addArgs)
{
	echo("\"" @ %addData @ "\"");
	echo(%addInfo);
	%obj.virBricks[%num, "OWNER"] = %addInfo;
}






addCustSave("noimport");

function virtualBrickList::cs_addReal_noimport(%obj, %num, %brick)
{
	if (%brick.noImport) %obj.virBricks[%num, "noimport"] = 1;
	else %obj.virBricks[%num, "noimport"] = 0;
}

function virtualBrickList::cs_create_noimport(%obj, %num, %brick)
{
	if (%obj.virBricks[%num, "noimport"]) %brick.noImport = 1;
}

function virtualBrickList::cs_save_noimport(%obj, %num, %file)
{
	if (%obj.virBricks[%num, "noimport"])
		%file.writeLine("+-NOIMPORT " @ 1 @ "\"");
}

function virtualBrickList::cs_load_noimport(%obj, %num, %addData, %addInfo, %addArgs)
{
	%obj.virBricks[%num, "noimport"] = 1;
}