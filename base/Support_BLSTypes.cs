//useful event functions:
//outputEvent_GetNumParametersFromIdx(%targetClass, %outputIdx)
//outputEvent_GetOutputName
//outputEvent_GetOutputEventIdx(%targetclass, %outputevent)

//inputEvent_GetTargetName
//inputEvent_GetTargetClass("fxDTSBrick", %inputIDx, %targetIDx)
//inputEvent_GetTargetIndex("fxDTSBrick", %inputIDx, %target)
//inputEvent_GetInputEventIdx("fxDTSBrick", number, number) returns 

addCustSave("OWNER");

function virtualBrickList::cs_addReal_OWNER(%obj, %vb, %b)
{
	if (%b.getGroup().bl_id !$= "")
		%vb.props["OWNER"] = %b.getGroup().bl_id;
	else
		%vb.props["OWNER"] = "";
}

function virtualBrickList::cs_create_OWNER(%obj, %vb, %b)
{
	if (%vb.props["OWNER"] $= "")
		return;
	%brickGroupName = "BrickGroup_" @ %vb.props["OWNER"];
	if (!isObject(%brickGroupName))
	{
		new SimGroup(%brickGroupName);
		%brickGroup = %brickGroupName.getId();
		%brickGroup.bl_id = %vb.props["OWNER"];
		%brickGroup.name = "BL_ID:" SPC %vb.props["OWNER"];
		mainBrickGroup.add(%brickGroup);
	}
	%brickGroup = %brickGroupName.getId();
	%brickGroup.add(%b);	
	if (isObject(%brickGroup.client)) %b.client = %brickGroup.client;
	%b.stackBL_ID = %brickGroup.bl_id;
}

function virtualBrickList::cs_save_OWNER(%obj, %vb, %file)
{
	if (%vb.props["OWNER"] !$= "")
		%file.writeLine("+-OWNER " @ %vb.props["OWNER"]);
}

function virtualBrickList::cs_load_OWNER(%obj, %vb, %addData, %addInfo, %addArgs)
{
	%vb.props["OWNER"] = %addInfo;
}

addCustSave("NTOBJECTNAME");
function virtualBrickList::cs_addReal_NTOBJECTNAME(%obj, %vb, %brick)
{
	if (strLen(%brick.getName()) > 0) %vb.props["NTOBJECTNAME"] = %brick.getName();
	else %vb.props["NTOBJECTNAME"] = "";
}

function virtualBrickList::cs_create_NTOBJECTNAME(%obj, %vb, %brick)
{
	if (strLen(%vb.props["NTOBJECTNAME"]) > 0)
	{
		%brick.setNTObjectName(%vb.props["NTOBJECTNAME"]);
	}
}

function virtualBrickList::cs_save_NTOBJECTNAME(%obj, %vb, %file)
{
	if (strLen(%vb.props["NTOBJECTNAME"]) > 0)
		%file.writeLine("+-NTOBJECTNAME" SPC %vb.props["NTOBJECTNAME"]);
}

function virtualBrickList::cs_load_NTOBJECTNAME(%obj, %vb, %addData, %addInfo, %addArgs, %line)
{
	%vb.props["NTOBJECTNAME"] = %addInfo;
}

addCustSave("EVENT");
function virtualBrickList::cs_addReal_EVENT(%obj, %vb, %brick)
{
	if (%brick.numEvents)
	{
		%vb.props["EVENT"] = %brick.numEvents;
		for (%i = 0; %i < %brick.numEvents; %i++)
		{
			%vb.props["EVENT", "Delay", %i] = %brick.eventDelay[%i];
			%vb.props["EVENT", "Enabled", %i] = %brick.eventEnabled[%i];
			%vb.props["EVENT", "Input", %i] = %brick.eventInput[%i];
			%vb.props["EVENT", "InputIdx", %i] = %brick.eventInputIdx[%i];
			%vb.props["EVENT", "NT", %i] = %brick.eventNT[%i];
			%vb.props["EVENT", "Output", %i] = %brick.eventOutput[%i];
			%vb.props["EVENT", "OutputAppendClient", %i] = %brick.eventOutputAppendClient[%i];
			%vb.props["EVENT", "OutputIdx", %i] = %brick.eventOutputIdx[%i];
			for (%op = 1; %brick.eventOutputParameter[%i, %op] !$= ""; %op++)
				%vb.props["EVENT", "OutputParameter", %i, %op] = %brick.eventOutputParameter[%i, %op];
			%vb.props["EVENT", "Target", %i] = %brick.eventTarget[%i];
			%vb.props["EVENT", "TargetIdx", %i] = %brick.eventTargetIdx[%i];
		}
	}
	else %vb.props["EVENT"] = 0;
}

function virtualBrickList::cs_create_EVENT(%obj, %vb, %brick)
{
	for (%i = 0; %i < %vb.props["EVENT"]; %i++)
	{
		//I hate dealing with targets, lets get this over with first
		//well I was going to only save the target and not the id but then I remembered
		//this SO is also for easy access to brick properties, that wouldn't do!
		///%target = %vb.props["EVENT", "Target", %i];
		%targetIndex = %obj;
		%brick.eventDelay[%i] = %vb.props["EVENT", "Delay", %i];
		%brick.eventEnabled[%i] = %vb.props["EVENT", "Enabled", %i];
		%brick.eventInput[%i] = %vb.props["EVENT", "Input", %i];
		%brick.eventInputIdx[%i] = %vb.props["EVENT", "InputIdx", %i];
		//%brick.eventInputIdx[%i] = inputEvent_GetInputEventIdx(%brick.eventInput);
		%brick.eventNT[%i] = %vb.props["EVENT", "NT", %i];
		%brick.eventOutput[%i] = %vb.props["EVENT", "Output", %i];
		%brick.eventOutputAppendClient[%i] = %vb.props["EVENT", "OutputAppendClient", %i];
		%brick.eventOutputIdx[%i] = %vb.props["EVENT", "OutputIdx", %i];
		//%brick.eventOutputIdx = outputEvent_GetOutputEventIdx(%brick.eventOutput);
		for (%op = 1; %vb.props["EVENT", "OutputParameter", %i, %op] !$= ""; %op++)
			%brick.eventOutputParameter[%i, %op] = %vb.props["EVENT", "OutputParameter", %i, %op];
		%brick.eventTarget[%i] = %vb.props["EVENT", "Target", %i];
		%brick.eventTargetIdx[%i] = %vb.props["EVENT", "TargetIdx", %i];
		%brick.numEvents = %vb.props["EVENT"];
	}
}

function virtualBrickList::cs_rotateCW_Event(%obj, %vb, %times)
{
	%times %= 4;
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
	for (%i = 0; %i < %vb.props["EVENT"]; %i++)
	{
		%outputClass = %vb.props["EVENT", "TargetIdx", %i] == -1 ? "fxDtsBrick" : inputEvent_GetTargetClass("fxDtsBrick", %vb.props["EVENT", "InputIdx", %i], %vb.props["EVENT", "TargetIdx", %i]);
		%relayNum = %relays[%vb.props["EVENT", "Output", %i]];
		if (%relayNum)
		{
			%relayNum += %times;
			%relayNum %= 4;
			%vb.props["EVENT", "Output", %i] = %relayNames[%relayNum];
			%vb.props["EVENT", "OutputIdx", %i] = outputEvent_GetOutputEventIdx("fxDTSBrick", %relayNames[%relayNum]); //relays are fxdtsbrick stuff
		}
		%paras = $OutputEvent_parameterList[%outputClass, %vb.props["EVENT", "OutputIdx", %i]];
		%paraCount = getFieldCount(%paras);
		
		for (%f = 0; %f < %paraCount; %f++)
		{
			%para = getField(%paras, %f);
			%type = firstWord(%para);
			if (%type $= "vector")
				%vb.props["EVENT", "OutputParameter", %i, %f + 1] = rotateVector(%vb.props["EVENT", "OutputParameter", %i, %f + 1], %times);
		}
	}
}

function rotateVector(%vec, %amt)
{
	if(%amt == 0)
	{
		return %vec;
	}
	else if(%amt == 1)
	{
		return getWord(%vec, 1) SPC -getWord(%vec, 0) SPC getWord(%vec, 2);
	}
	else if(%amt == 2)
	{
		return -getWord(%vec, 0) SPC -getWord(%vec, 1) SPC getWord(%vec, 2);
	}
	else if(%amt == 3)
	{
		return -getWord(%vec, 1) SPC getWord(%vec, 0) SPC getWord(%vec, 2);
	}
}
	
function virtualBrickList::cs_rotateCCW_Event(%obj, %vb, %times)
{
	%cw = (4 - (%times % 4)) % 4;
	%obj.cs_rotateCW_Event(%vb, %cw);
}

function virtualBrickList::cs_save_EVENT(%obj, %vb, %file)
{
	for (%i = 0; %i < %vb.props["EVENT"]; %i++)
	{
		%targets = $Input["Event", "TargetListfxDTSBrick",  %vb.props["EVENT", "TargetIdx", %i]];
		%target = getWord(getField(%targets, %vb.props["EVENT", "TargetIdx", %i]), 1);
		%paraList = $Output["Event", "parameterList" @ %target, %vb.props["EVENT", "OutputIdx", %i]];
		%outputParameters = "";
		for (%op = 1; %vb.props["EVENT", "OutputParameter", %i, %op] !$= ""; %op++)
		{
			if (%vb.props["EVENT", "TargetIdx", %i] == -1)
				%outputClass = "fxDTSBrick";
			else
				%outputClass = inputEvent_GetTargetClass("fxDtsBrick", %vb.props["EVENT", "InputIdx", %i], %vb.props["EVENT", "TargetIdx", %i]);
			
			%param = %vb.props["EVENT", "OutputParameter", %i, %op];
			
			if (isObject(%param) && getWord(getField($OutputEvent_parameterList[%outputClass, %vb.props["EVENT", "OutputIdx", %i]], %op - 1), 0) $= "dataBlock")
				%param = %param.getName();
			
			%outputParameters = %outputParameters @ %param @ "\t";
		}
		%file.writeLine("+-EVENT" TAB
		%i TAB
		%vb.props["EVENT", "Enabled", %i] TAB
		%vb.props["EVENT", "Input", %i] TAB
		%vb.props["EVENT", "Delay", %i] TAB
		%vb.props["EVENT", "Target", %i] TAB
		%vb.props["EVENT", "NT", %i] TAB
		%vb.props["EVENT", "Output", %i] TAB
		%outputParameters);
	}
}

function virtualBrickList::cs_load_EVENT(%obj, %vb, %addData, %addInfo, %addArgs, %line)
{
	//unnamed brick loading:
	//+-EVENT^0^1^onActivate^0^Player^^AddVelocity^0 0 50	^^
	
	//another brick:
	//+-EVENT^0^1^onPlayerTouch^0^Client^^CenterPrint^hello derp^2^^
	
	//another, same as above but delay 34:
	//+-EVENT^0^1^onPlayerTouch^34^Client^^CenterPrint^hello derp^2^^
	
	//should probably use getField(%line, 1) to determine this but whatever
	%vb.props["EVENT"]++;
	%i = %vb.props["EVENT"] - 1;
	
	//$InputEvent_TargetList[class, eventId] - list of ReadableName -> ClassName for that event, separated by a space (this isn't needed I think)
	//$InputEvent_Name[class, eventId]			 - name of input event
	//$InputEvent_Count[class]							 - number of events registered for class
	//input class seems to always be fxDtsBrick
	
	//$OutputEvent_Count[class]							 			- output event count for output class
	//$OutputEvent_AppendClient[class, eventId] 	- append client (bool) for given event
	//$OutputEvent_Name[class, eventId]						- name of output event id
	//$OutputEvent_parameterList[class, eventId]	- list of parameters; each field is a different arg, format [type limit]; 
																								//more on this list on bl forums (space guy's topic), this isn't needed either
																								//we can use this to autodetect when to rotate (vector data type)
	
	%vb.props["EVENT", "Enabled", %i] = getField(%line, 2); //yes
	%vb.props["EVENT", "Delay", %i] = getField(%line, 4); //yes
	%vb.props["EVENT", "Input", %i] = getField(%line, 3); //yes
		
	%vb.props["EVENT", "InputIdx", %i] = inputEvent_GetInputEventIdx(%vb.props["EVENT", "Input", %i]);

	%vb.props["EVENT", "Target", %i] = getField(%line, 5); //should be field 5
	%vb.props["EVENT", "TargetIdx", %i] = inputEvent_GetTargetIndex("fxDtsBrick", %vb.props["EVENT", "InputIdx", %i], %vb.props["EVENT", "Target", %i]);

	%vb.props["EVENT", "NT", %i] = getField(%line, 6); //its possible this should be stripping the underscore
	
	%vb.props["EVENT", "Output", %i] = getField(%line, 7); //should be field 7

	if (%vb.props["EVENT", "TargetIdx", %i] == -1)
		%outputClass = "fxDTSBrick";
	else
		%outputClass = inputEvent_GetTargetClass("fxDtsBrick", %vb.props["EVENT", "InputIdx", %i], %vb.props["EVENT", "TargetIdx", %i]);
	%vb.props["EVENT", "OutputIdx", %i] = outputEvent_GetOutputEventIdx(%outputClass, %vb.props["EVENT", "Output", %i]);
	
	//look up in a table: $OutputEvent_AppendClient[class, outputIdx];
	%vb.props["EVENT", "OutputAppendClient", %i] = $OutputEvent_AppendClient[%outputClass, %vb.props["EVENT", "OutputIdx", %i]];
	
	//this works
	for (%op = 8; %op < getFieldCount(%line); %op++) //starts in field 8
	{
		%param = getField(%line, %op);
		
		if (isObject(%param) && getWord(getField($OutputEvent_parameterList[%outputClass, %vb.props["EVENT", "OutputIdx", %i]], %op - 1), 0) $= "dataBlock")
			%param = %param.getId();
				
		%vb.props["EVENT", "OutputParameter", %i, %op - 7] = getField(%line, %op);
	}
}

addCustSave("noimport");

function virtualBrickList::cs_addReal_noimport(%obj, %vb, %brick)
{
	if (%brick.noImport) %vb.props["noimport"] = 1;
	else %vb.props["noimport"] = 0;
}

function virtualBrickList::cs_create_noimport(%obj, %vb, %brick)
{
	if (%vb.props["noimport"]) %brick.noImport = 1;
}

function virtualBrickList::cs_save_noimport(%obj, %vb, %file)
{
	if (%vb.props["noimport"])
		%file.writeLine("+-NOIMPORT " @ 1 @ "\"");
}

function virtualBrickList::cs_load_noimport(%obj, %vb, %addData, %addInfo, %addArgs)
{
	%vb.props["noimport"] = 1;
}