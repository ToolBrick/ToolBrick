function fxDTSBrick::getBrickPosition(%obj)
{
	return worldToBrick(%obj.getPosition());
}

function fxDTSBrick::getBrickBox(%obj)
{
	%box = %obj.getWorldBox();
	return worldToBrick(getWords(%box, 0, 2)) SPC worldToBrick(getWords(%box, 3, 5));
}

function worldToBrick(%pos)
{
	%pos = VectorAdd(%pos, "0.25 0.25 0.1");
	return getWord(%pos, 0) / 0.5 SPC getWord(%pos, 1) / 0.5 SPC getWord(%pos, 2) / 0.2;
}

function brickToWorld(%pos)
{
	return VectorSub(getWord(%pos, 0) * 0.5 SPC getWord(%pos, 1) * 0.5 SPC getWord(%pos, 2) * 0.2, "0.25 0.25 0.1");
}