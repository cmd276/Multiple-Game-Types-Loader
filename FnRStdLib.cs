// FILENAME:    FnRStdLib.cs
//
// AUTHORS:         Orogogus
// EMAIL:        h2liu@iname.com
// WEBSITE:      http://home.san.rr.com/orogogus
// LAST UPDATED: 11:26 PM 2/6/00
//
//------------------------------------------------------------------------------

// Edited for inclusion into Scripting library by Wilzuun. Find n Retrieve


function FnR::onMissionLoad()
{
  $gameOver = 0;

  for(%i=1; %i<($totalFlags + 1); %i++)
  {
    $flag[%i] = getObjectId("MissionGroup\\Flags\\flag" @ %i);
    $flag[%i].owner = 0;
    $magnet[%i] = getObjectId("MissionGroup\\Magnets\\flag" @ %i @ "magnet");
    $flagTrigger[%i] = getObjectId("MissionGroup\\FlagTriggers\\flag" @ %i @ "Trigger");

    $flag[%i].magnet = $magnet[%i];
    $flag[%i].trigger = $flagTrigger[%i];
    $magnet[%i].flag = $flag[%i];
    $flagTrigger[%i].flag = $flag[%i];
  }

  $blueFlags = 0;
  $purpleFlags = 0;
  $redFlags = 0;
  $yellowFlags = 0;


  for(%i=1; %i<($blueStands + 1); %i++)
  {
    if($server::AllowTeamBlue == false) return;
    $blueStand[%i] = getObjectId("MissionGroup\\Stands\\BlueStand" @ %i);
    $blueStand[%i].flag = 0;
    $blueStand[%i].team = *IDSTR_TEAM_BLUE;
  }

  for(%i=1; %i<($purpleStands + 1); %i++)
  {
    if($server::AllowTeamPurple == false) return;
    $purpleStand[%i] = getObjectId("MissionGroup\\Stands\\PurpleStand" @ %i);
    $purpleStand[%i].flag = 0;
    $purpleStand[%i].team = *IDSTR_TEAM_PURPLE;
  }

  for(%i=1; %i<($redStands + 1); %i++)
  {
    if($server::AllowTeamRed == false) return;
    $redStand[%i] = getObjectId("MissionGroup\\Stands\\RedStand" @ %i);
    $redStand[%i].flag = 0;
    $redStand[%i].team = *IDSTR_TEAM_RED;
  }

  for(%i=1; %i<($yellowStands + 1); %i++)
  {
    if($server::AllowTeamYellow == false) return;
    $yellowStand[%i] = getObjectId("MissionGroup\\Stands\\YellowStand" @ %i);
    $yellowStand[%i].flag = 0;
    $yellowStand[%i].team = *IDSTR_TEAM_YELLOW;
  }
}


//======================= player functions


function FnR::vehicle::onAdd(%vehicle)
{
    %player = playerManager::vehicleIdToPlayerNum(%vehicle);
    %player.name = getHUDName(%vehicle);

    %vehicle.flag = 0;
  %vehicle.team = getTeam(%vehicle);
}


function FnR::vehicle::OnDestroyed( %victimVeh, %destroyerVeh )
{

  %player = playerManager::vehicleIdToPlayerNum(%victimVeh);

  if(%victimVeh.flag != 0)
  {
    setVehicleSpecialIdentity(%victimVeh, false);

    %placeX = getPosition(%victimVeh, x);
    %placeY = getPosition(%victimVeh, y);
    %placeZ = getTerrainHeight(%placeX, %placeY);

    %flag = %victimVeh.flag;

    setPosition(%flag, %placeX, %placeY, %placeZ);
    setPosition(%flag.trigger, %placeX, %placeY, %placeZ);

    %victimVeh.flag = 0;
    %flag.owner = 0;
 
    say("everyone", 0, %player.name @ " lost a flag!");
  }
}


//======================== flag triggers

function FlagTrigger::trigger::onenter(%this, %vehicleId)
{
  if($gameOver == 1) return;

  if ((%vehicleId.team == %this.team) && (%vehicleId.flag == 0)) return;
  if ((%vehicleId.team == %this.team) && (%this.flag != 0)) return;
  if ((%vehicleId.team != %this.team) && (%this.flag == 0)) return;

  %flag = %this.flag;
  %magnet = %flag.magnet;

  if((%vehicleId.team == *IDSTR_TEAM_BLUE) && (%flag.owner == 1)) return;
  if((%vehicleId.team == *IDSTR_TEAM_PURPLE) && (%flag.owner == 2)) return;
  if((%vehicleId.team == *IDSTR_TEAM_RED) && (%flag.owner == 3)) return;
  if((%vehicleId.team == *IDSTR_TEAM_YELLOW) && (%flag.owner == 4)) return;


  if(%flag.owner != 0) return;

  %player = playerManager::vehicleIdToPlayerNum(%vehicleId);

  if(%vehicleId.flag != 0) 
  {
    say(%player, 0, "Can only hold one flag at a time!");
    return;
  }

  warp(%flag, %magnet);            // send the flag away
  warp(%flag.trigger, %magnet);    // and its little trigger, Toto, too

  setVehicleSpecialIdentity(%vehicleid, on);
  %flag.owner = %vehicleId;
  %vehicleId.flag = %flag;

  say("everybody", 0, %player.name @ " has grabbed a flag for the " @ %vehicleId.team @ "!");
}


//=================  flag stands

function FlagStand::trigger::OnEnter(%this, %vehicleId)
{
  if($gameOver == 1) return;

  %player = playerManager::vehicleIdToPlayerNum(%vehicleId);

  if ((%vehicleId.team == %this.team) && (%vehicleId.flag != 0) && (%this.flag == 0))
  {
    %flag = %vehicleId.flag;
    %this.flag = %flag;
    warp(%flag, %this);
    warp(%flag.trigger, %this);
    %vehicleId.flag = 0;
    setVehicleSpecialIdentity(%vehicleid, false);
    %flag.owner = 1;
    say("everyone", 0, "The " @ %this.team @ " has claimed a flag!");
    addFlag(%this.team);
  }

  if ((%vehicleId.team != %this.team) && (%this.flag != 0))
  {
    %flag = %this.flag;

    warp(%flag, %flag.magnet);
    warp(%flag.trigger, %flag.magnet);
    %vehicleId.flag = %flag;
    %flag.owner = %vehicleId;

    say("everybody", 0, %player.name @ " (" @ %vehicleId.team @ ") has stolen a flag from the " @ %this.team @ "!");
    %this.flag = 0;
    subFlags(%this.team);
  }
}


//==========================  victory conditions and such

function addFlag(%color)
{
  if(%color == *IDSTR_TEAM_BLUE)
  {
    $blueFlags++;
    if($blueFlags >= $blueToWin)
    {
      say("everyone", 0, "The Blue Team has won the game!");
      schedule("missionEndConditionMet();", 10);
      $gameOver = 1;
    }
  }

  if(%color == *IDSTR_TEAM_PURPLE)
  {
    $purpleFlags++;
    if($purpleFlags >= $purpleToWin)
    {
      say("everyone", 0, "The Purple Team has won the game!");
      schedule("missionEndConditionMet();", 10);
      $gameOver = 1;
    }
  }

  if(%color == *IDSTR_TEAM_RED)
  {
    $redFlags++;
    if($redFlags >= $totalFlags)
    {
      say("everyone", 0, "The Red Team has won the game!");
      schedule("missionEndConditionMet();", 10);
      $gameOver = 1;
    }
  }

  if(%color == *IDSTR_TEAM_YELLOW)
  {
    $yellowFlags++;
    if($yellowFlags >= $totalFlags)
    {
      say("everyone", 0, "The Yellow Team has won the game!");
      schedule("missionEndConditionMet();", 10);
      $gameOver = 1;
    }
  }
}