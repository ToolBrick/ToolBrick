function VirtualBrickList::vbBaseMatches(%obj, %b, %oVBL, %o)
{
	%dEq = %obj.getDatablock(%b) == %oVBL.getDatablock(%o);
	%pEq = %obj.getPosition(%b) $= %oVBL.getPosition(%o);
	%obEq = %obj.getObjectBox(%b) $= %oVBL.getObjectBox(%o);
	%wbEq = %obj.getWorldBox(%b) $= %oVBL.getWorldBox(%o);
	%sEq = %obj.getSize(%b) $= %oVBL.getSize(%o);
	%aEq = %obj.getAngleId(%b) == %oVBL.getAngleId(%o);
	%bpEq = %obj.isBaseplate(%b) == %oVBL.isBasePlate(%o);
	%cEq = %obj.getColorId(%b) == %oVBL.getColorId(%o);
	%prEq = %obj.getPrint(%b) $= %oVBL.getPrint(%o);
	%cfEq = %obj.getColorFx(%b) == %oVBL.getColorFx(%o);
	%sfEq = %obj.getShapeFx(%b) == %oVBL.getShapeFx(%o);
	%rEq = %obj.isRaycasting(%b) == %oVBL.isRaycasting(%o);
	%colEq = %obj.isColliding(%b) == %oVBL.isColliding(%o);
	%renEq = %obj.isRendering(%b) == %oVBL.isRendering(%o);
	
	return %dEq && %pEq && %obEq && %wbEq && %sEq && %aEq && %bpEq && %cEq && %prEq && %cfEq && %sfEq && %rEq && %colEq && %renEq;
}

function VirtualBrickList::vblMatchesOther(%obj, %oVBL)
{
	
}