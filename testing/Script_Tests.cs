//This file is a base that executes other test scripts

exec("./Script_TestBase.cs");

function VBL_SaveLoadTest(%file)
{
	%file2 = "config/ToolBrick/tests/saveload.bls";
	%vbl = newVBL();
	%vbl.loadBLSFile(%file);
	%vbl.createBricks();
	
	%vbl.shiftBricks(%vbl.getSizeX() SPC "0 0");
	%vbl.exportBLSFile(%file2);
	%vbl2 = newVBL();
	%vbl2.loadBLSFile(%file2);
	
	%vbl.createBricks();
	%vbl2.createBricks();
	
	%vbl.delete();
	%vbl2.delete();
}

function VBL_LoadPlaceRotatePlace(%file)
{
	%vbl = newVBL();
	%vbl.loadBLSFile(%file);
	%vbl.createBricks();
	
	%vbl.shiftBricks("0 0" SPC %vbl.getSizeZ());
	%vbl.rotateBricksCW(1);
	%vbl.createBricks();
	%vbl.delete();
}