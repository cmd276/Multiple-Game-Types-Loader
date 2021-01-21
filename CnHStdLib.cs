// Capture And Hold

// CnHStdLib.cs
// v1.003
// Last updated 6/3/00
// Download from http://home.san.rr.com/orogogus/scripts.html#CandH
//
//
// 6/5/00
// - Added a $server::PointLimit value at the beginning of the script, so that non-dedicated servers
//   wouldn't register a win after the first point was scored
//
// 6/3/00  v1.003
// - Allowed games to be won by reaching a team point limit (500 by default), see the Server_CH.cs file
// - Commented out individual player scores.  They're still counted, but not reported unless the server admin
//   uncomments the appropriate lines below.
//
// 5/28/00  v1.002
// - Fixed a minor bug which would activate all the turrets whenever any of the pads were activated,
//   and which would prevent turrets from ever activating if they weren't in //MissionGroup//Bases.
//
// 5/26/00 
// - Updated the script to allow different pads to have different points values
// - Added the zen globals to make sure the pads work as zen pads
// - Added a %padTrigger.zen variable so that trigger pads don't have to act as zen pads
// - Added default values for $killPoints, $deathPoints and $basePoints (points players get for base seizures)
//
// http://home.san.rr.com/orogogus/scripts.html#CandH
//
// Instructions for mapmakers
//
// In the .cs file for your map, you must include a line to run this file, i.e. 
//
// exec("CnHStdLib.cs");
//
// and also you must specify the paths of the various pad triggers.  These must
// have a script class of Capture, and the path specification should follow
// this format:
//
//  function onMissionStart()
//  {
//    %capture1 = getObjectId("MissionGroup\\Bases\\OmegaTrigger");
//    %capture1.timeToCapture = 5;   // How long you have to sit on the pad to convert
//    %capture1.secondsToHold = 60;     // How long you have to hold the pad to score
//    %capture1.name = "Omega Base"; // Name of the pad
//    %capture1.points = 10;         // How many points you get from the pad every .secondsToHold sec
//    %capture1.zen = 1;             // Whether or not the pad acts as a zen pad
//    %capture1.spoils = 5;          // How many associated objects there are for the pad
//    %capture1.spoil[0] = "MissionGroup\\Bases\\T1";
//    %capture1.spoil[1] = "MissionGroup\\Bases\\T2";
//    %capture1.spoil[2] = "MissionGroup\\Bases\\T3";
//    %capture1.spoil[3] = "MissionGroup\\Bases\\T4";
//    %capture1.spoil[4] = "MissionGroup\\Bases\\NavZenOmega";
//    ====  The following should all be set to 0, except for .owner, if you want
//    ====  to let the pad to belong to a team when the game starts
//    %capture1.timeCounted = 0;     // How long the pad has already been sat on
//    %capture1.lastVehicle = 0;     // Last vehicle to shut down on a pad
//    %capture1.owner = 0;           // Current owner of the pad
//    %capture1.heldFor = 0;         // How long the pad has been held for
//  }
// 
//
// - Orogogus

// Edited for inclusion into Scripting library by Wilzuun.
function cnh::onMissionLoad()
{
  if($server::PointLimit < 1) 
    $server::PointLimit = 9999;
  $gameOver = 0;

  $blueBonus = 0;
  $purpleBonus = 0;
  $redBonus = 0;
  $yellowBonus = 0;

  $blueKills = 0;
  $purpleKills = 0;
  $redKills = 0;
  $yellowKills = 0;

  $blueDeaths = 0;
  $purpleDeaths = 0;
  $redDeaths = 0;
  $yellowDeaths = 0;

  $killPoints   = 3;
  $deathPoints  = 2;
  $basePoints   = 5;

  $ZenWaitTime = 40;
  $ZenHealRate = 400; 
  $ZenAmmoRate = 8;   
}

// Capture Pad Functionality
//------------------------------------------------------------------------------


function Capture::trigger::onEnter(%this, %object)
{
  if(%this.zen == 1) Zen::onEnter(%this, %object, *IDMULT_CHAT_ALLPAD, true, true);  
  schedule( "captureWork(" @ %this @ ", " @ %object @ ");", 1);
  %object.inTrigger = 1;
}


function Capture::trigger::onLeave(%this, %object)
{
  %object.inTrigger = 0;
}


function captureWork(%this, %object)
{
  if(%object.inTrigger == 0) return;
  if(isShutDown(%object))
  {
    if(%this.zen == 1) Zen::work(%this, %object, $ZenHealRate, $ZenAmmoRate, $zenWaitTime, true);
    if(%object != %this.lastVehicle)
    {
      %this.lastVehicle = %object;
      %this.timeCounted = 1;
    }
    else
    {
      %this.timeCounted++;
    }

    if(%this.timeCounted >= %this.timeToCapture)
    {
      giveSpoils(%this, %object);
    }
  }

  schedule( "captureWork(" @ %this @ ", " @ %object @ ");", 1);
}


// Spoils Code
// -----------------------------------------------------------------------------------

function giveSpoils(%capturedPad, %vehicleId)
{
  %player = playerManager::vehicleIdToPlayerNum(%vehicleId);
  %color = %player.color; 

  %team = getTeam(%vehicleId);
  for(%i = 0; %i < %capturedPad.spoils; %i++)
  {
    setTeam(%capturedPad.spoil[%i], %team);
    order(%capturedPad.spoil[%i], Shutdown, false);  
  }

  if(%color == %capturedPad.owner) return;
  %capturedPad.owner = %color; 
  %capturedPad.heldFor = 0;
  schedule("captureScore(" @ %color @ ", " @ %capturedPad @ ", " @ %vehicleId @ ");", 1);

  say("everybody", 0, %player.name @ " seizes " @ %capturedPad.name @ " for " @ %team @ "!");
  %player.seizures++;
}


function captureScore(%origColor, %capturedPad, %vehicleId)
{
  if(%origColor != %capturedPad.owner) return;
  %capturedPad.heldFor++;

  if(%capturedPad.heldFor == %capturedPad.secondsToHold)
  {
    if(%origColor == "BLUE")
    {
      %capturedPad.heldFor = 0;
      $blueBonus = $blueBonus + %capturedPad.points; 
      say("everybody", 0, "Blue Team receives " @ %capturedPad.points @ " points for holding " @ %capturedPad.name @ "!");
    }
    if(%origColor == "PURPLE")
    {
      %capturedPad.heldFor = 0;
      $purpleBonus = $purpleBonus + %capturedPad.points; 
      say("everybody", 0, "Purple Team receives " @ %capturedPad.points @ " points for holding " @ %capturedPad.name @ "!");
    }
    if(%origColor == "RED")
    {
      %capturedPad.heldFor = 0;
      $redBonus = $redBonus + %capturedPad.points;
      say("everybody", 0, "Red Team receives " @ %capturedPad.points @ " points for holding " @ %capturedPad.name @ "!");
    }
    if(%origColor == "YELLOW")
    {
      %capturedPad.heldFor = 0;
      $yellowBonus = $yellowBonus + %capturedPad.points; 
      say("everybody", 0, "Yellow Team receives " @ %capturedPad.points @ " points for holding " @ %capturedPad.name @ "!");
    }
  }

  if(getTeamScore(1) >= $server::PointLimit) missionWin(1);
  if(getTeamScore(2) >= $server::PointLimit) missionWin(2);
  if(getTeamScore(4) >= $server::PointLimit) missionWin(4);
  if(getTeamScore(8) >= $server::PointLimit) missionWin(8);

  schedule("captureScore(" @ %origcolor @ ", " @ %capturedPad @ ", " @ %vehicleId @ ");", 1);
}


//
//--------------------------------------
// OnAdd and OnDestroyed functions

function cnh::player::onAdd(%player)
{
  %player.seizures = 0;
//say(%player, 0, "Bugfix testing for Capture and Hold.  Please report problems on the Editing and Scripting forum");
}   


function cnh::vehicle::onAdd(%vehicleId)
{
    if($wilzuun::Boost == true)
    {
        boost::vehicle::onAdd(%vehicleId);
    }

  %player = playerManager::vehicleIdToPlayerNum(%vehicleId);
  %player.name = getHUDName(%vehicleId);

  %color = getTeam(%vehicleId);
  if(%color == *IDSTR_TEAM_BLUE) %player.color = "BLUE";
  if(%color == *IDSTR_TEAM_PURPLE) %player.color = "PURPLE";
  if(%color == *IDSTR_TEAM_RED) %player.color = "RED";
  if(%color == *IDSTR_TEAM_YELLOW) %player.color = "YELLOW";
}


function cnh::vehicle::OnDestroyed( %victim, %destroyer )
{
  %victimNum = playerManager::vehicleIdToPlayerNum( %victim );
  %destroyerNum = playerManager::vehicleIdToPlayerNum( %destroyer );

  %message = getFancyDeathMessage(getHUDName(%victim), getHUDName(%destroyer));
  if(%message != "") say( 0, 0, %message);

  if(getHUDName(%destroyer) == "Blue Turret") $blueKills++;
  if(getHUDName(%destroyer) == "Purple Turret") $purpleKills++;
  if(getHUDName(%destroyer) == "Red Turret") $redKills++;
  if(getHUDName(%destroyer) == "Yellow Turret") $yellowKills++;

  if(%destroyerNum == 0) return;
  if(%destroyerNum.color == "BLUE") $blueKills++; // $scores[getTeam(%destoyerNum)]++;
  if(%destroyerNum.color == "PURPLE") $purpleKills++;
  if(%destroyerNum.color == "RED") $redKills++;
  if(%destroyerNum.color == "YELLOW") $yellowKills++;

  if(%victimNum.color == "BLUE") $blueDeaths++;  // $scores[getTeam(%victimNum)]++;
  if(%victimNum.color == "PURPLE") $purpleDeaths++;
  if(%victimNum.color == "RED") $redDeaths++;
  if(%victimNum.color == "YELLOW") $yellowDeaths++;

  if(getTeamScore(1) >= $server::PointLimit) missionWin(1);
  if(getTeamScore(2) >= $server::PointLimit) missionWin(2);
  if(getTeamScore(4) >= $server::PointLimit) missionWin(4);
  if(getTeamScore(8) >= $server::PointLimit) missionWin(8);
}



//------------------------------------------------------------------------------
// scoreboard 
//

function getPlayerScore(%a)
{
   return((getKills(%a) * $killPoints) - (getDeaths(%a) * $deathPoints) + (%a.seizures * $basePoints));
}


function getSeizures(%a)
{
  return(%a.seizures);
}


function getTeamScore(%team)
{
  if(%team == "1") 
  {
    %points = $yellowBonus + ($yellowKills * $killPoints) - ($yellowDeaths * $deathPoints);
    return(%points);
  }
  if(%team == "2") 
  {
    %points = $blueBonus + ($blueKills * $killPoints) - ($blueDeaths * $deathPoints);
    return(%points);
  }
  if(%team == "4") 
  {
    %points = $redBonus + ($redKills * $killPoints) - ($redDeaths * $deathPoints);
    return(%points);
  }
  if(%team == "8") 
  {
    %points = $purpleBonus + ($purpleKills * $killPoints) - ($purpleDeaths * $deathPoints);
    return(%points);
  }
  return 0;
} 


function getPermTeamDeaths(%a)
{
  if(%a == "1"){return($yellowDeaths);}
  if(%a == "2"){return($blueDeaths);}
  if(%a == "4"){return($redDeaths);}
  if(%a == "8"){return($purpleDeaths);}
}


function getPermTeamKills(%a)
{
  if(%a == "1"){return($yellowKills);}
  if(%a == "2"){return($blueKills);}
  if(%a == "4"){return($redKills);}
  if(%a == "8"){return($purpleKills);}
}


function cnh::initScoreBoard()
{
   deleteVariables("$ScoreBoard::PlayerColumn*");
   deleteVariables("$ScoreBoard::TeamColumn*");

   // Player ScoreBoard column headings
   $ScoreBoard::PlayerColumnHeader1 = *IDMULT_SCORE_TEAM;
   $ScoreBoard::PlayerColumnHeader2 = *IDMULT_SCORE_SQUAD;
//   $ScoreBoard::PlayerColumnHeader3 = *IDMULT_SCORE_SCORE;
//   $ScoreBoard::PlayerColumnHeader4 = *IDMULT_SCORE_KILLS;
//   $ScoreBoard::PlayerColumnHeader5 = *IDMULT_SCORE_DEATHS;
//   $ScoreBoard::PlayerColumnHeader6 = "Base captures";

   // Player ScoreBoard column functions
   $ScoreBoard::PlayerColumnFunction1 = "getTeam";
   $ScoreBoard::PlayerColumnFunction2 = "getSquad";
//   $ScoreBoard::PlayerColumnFunction3 = "getPlayerScore";
//   $ScoreBoard::PlayerColumnFunction4 = "getKills";
//   $ScoreBoard::PlayerColumnFunction5 = "getDeaths";
//   $ScoreBoard::PlayerColumnFunction6 = "getSeizures";

   // Team ScoreBoard column headings
   $ScoreBoard::TeamColumnHeader1 = *IDMULT_SCORE_SCORE;
   $ScoreBoard::TeamColumnHeader2 = *IDMULT_SCORE_PLAYERS;
   $ScoreBoard::TeamColumnHeader3 = *IDMULT_SCORE_KILLS;
   $ScoreBoard::TeamColumnHeader4 = *IDMULT_SCORE_DEATHS;

   // Team ScoreBoard column functions
   $ScoreBoard::TeamColumnFunction1 = "getTeamScore";
   $ScoreBoard::TeamColumnFunction2 = "getNumberOfPlayersOnTeam";
   $ScoreBoard::TeamColumnFunction3 = "getPermTeamKills";
   $ScoreBoard::TeamColumnFunction4 = "getPermTeamDeaths";

   // tell server to process all the scoreboard definitions defined above
   serverInitScoreBoard();
}


function missionWin(%team)
{
  if($gameOver == 1) return;

  $gameOver = 1;

  if(%team == "1") 
  {
    messageBox(0, "Yellow team wins with " @ getTeamScore(%team) @ " points!");
    schedule("missionEndConditionMet();", 10);
  }
  if(%team == "2") 
  {
    messageBox(0, "Blue team wins with " @ getTeamScore(%team) @ " points!");
    schedule("missionEndConditionMet();", 10);
  }
  if(%team == "4") 
  {
    messageBox(0, "Red team wins with " @ getTeamScore(%team) @ " points!");
    schedule("missionEndConditionMet();", 10);
  }
  if(%team == "8") 
  {
    messageBox(0, "Purple team wins with " @ getTeamScore(%team) @ " points!");
    schedule("missionEndConditionMet();", 10);
  }
  return 0;
}



//===================

function cnh::setRules()
{
      %rules = "Capture and Hold: "       @        
               $missionName               @
               "\n\nServer ops: download Capture and Hold from http://home.san.rr.com/orogogus/scripts.html#CandH" @
               "\n\n<F2>RULES/OBJECTIVES:\n" @
               "<F0>1) Locate and destroy enemy vehicles.\n\n" @
               "2) Capture and defend the bases.\n\n" @
               "<tIDMULT_TDM_SCORING_1>"  @
               "<tIDMULT_TDM_SCORING_2>"  @
               $killPoints                @
               "<tIDMULT_TDM_SCORING_3>"  @
               "<tIDMULT_TDM_SCORING_4>"  @
               $deathPoints               @
               "<tIDMULT_TDM_SCORING_5>"  @
               "<F0>Capturing a base is worth <F3>" @ $basePoints @ "<F0> points to the player who seizes it.\n\n" @
               "<F0>Holding a base is (usually) worth 10 points to your team every 60 seconds.\n\n" @
               "<F0>Your team will also score points for kills made by any of your base's turrets.\n\n" @
               "<tIDMULT_TDM_SCORING_6>"  @
               "<tIDMULT_STD_ITEMS>"      @
               "<tIDMULT_STD_ZEN_1>"      @
               $ZenWaitTime               @
               "<F0> seconds. You have to power down on the pad for (usually) 5 seconds to capture the bases. \n ";


  setGameInfo(%rules);  
  return %rules;    
}
