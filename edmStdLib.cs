## -------------------------------------------------------------------- Header
// FILE:        edm_StdLib.cs
// AUTHORS:     ^TFW^ Wilzuun, dun_Starscaper, unnamed Original Author.
// LAST MOD:    
// VERSION:     1.0r
// NOTES:       Elimination Death Match.
## -------------------------------------------------------------------- IMPORTANT NOTES
// Major edits by ^TFW^ Wilzuun
## -------------------------------------------------------------------- Settings
$timeToStart = 40;
$gameState = 0;
$allowAdds = 1;

//Modification by dun_Starscaper
//Fixed HUD timer so that late spawners get the true time to elimination
//In the original it was always $timeToStart, even if the waiting period was nearly over
$bootTime = 0;
## -------------------------------------------------------------------- onMission
function edm::onMissionStart()
{
  schedule("startGame();", $timeToStart);
  say("everyone", 0, "Round will start in " @ $timeToStart @ " seconds");
  setHUDTimerAll($timeToStart, -1, "Time to start", 1);
  $bootTime = getCurrentTime();
}

## -------------------------------------------------------------------- inMission
// For loading while mission is already in session.
function edm::inMissionStart()
{
    $allowAdds = 1;
    // because the onMissionStart does so much for the game setup... Lets run it.
    edm::onMissionStart();

    // cycle through each player, and treat them as if they just joined, or just spawend.
    %players = playerManager::getPlayerCount();
    for(%i = 0; %i < %players; %i++)
    {
        // get this players ID.
        %player = playerManager::getPlayerNum(%i);
        // get this players vehicle ID.
        %vehicle = playerManager::playerNumtoVehicleId(%player);
        // check to see if they're spawned.
        if(%player.hasVehicle == true)
        {
            // lets act like they jsut spawned.
            edm::vehicle::onAdd(%vehicle);
        }
    }

    // set mission name to reflect new game type.
    $missionName = "EDM - Dynamic Games";
    
    // Announce new game type.
    say(0,0,"ELIMINATION DEATH MATCH now in effect!");
}

// For unlaoding the play type while a mission is running.
function edm::inMissionEnd()
{
    
}
## -------------------------------------------------------------------- Player
function edm::player::onAdd(%player)
{
  %player.startTime = getCurrentTime();
  %player.name = getName(%player);
  %nowDate = getDate();
  %nowTime = getTime();
  %player.IP = getConnection(%player);
  %outputString =  %nowDate @ ", " @ %nowTime @ " -- " @ %player.name @ " joined the game (" @ %player.IP @ ")";
  fileWrite("multiplayer\\serverlog.txt", append, %outputString);
  %player.alive = 0;
}

function edm::player::OnRemove(%player)
{
  %player.name = getName(%player);
  %nowDate = getDate();
  %nowTime = getTime();
  %timeEnd = getCurrentTime();
  %timeIn = timeDifference( %timeEnd, %player.startTime);
  %outputString =  %nowDate @ ", " @ %nowTime @ " -- " @ %player.name @ " left the game (" @ %player.IP @ ") -- " @ %timeIn;

  fileWrite("multiplayer\\serverlog.txt", append, %outputString);
}
## -------------------------------------------------------------------- Vehicle
function edm::vehicle::onDestroyed( %victimVeh, %destroyerVeh )
{
  %player = playerManager::vehicleIdToPlayerNum(%victimVeh);
  if($gameState == 2) return;

  %player.alive = 0;

  countTeams();
}

function edm::vehicle::onAdd(%vehicleId)
{
  %player = playerManager::vehicleIdToPlayerNum(%vehicleId);

  if($allowAdds == 0)
  {
    say(%player, 0, "Sorry, round in progress.  Please wait until next round to join in");
    schedule("damageObject(" @ %vehicleId @ ", 50000);", 2);
  }
  else setHudTimer($timeToStart - (getCurrentTime() - $bootTime), -1, "Time to start", 1, %player);

  %player.alive = 1;
}

## -------------------------------------------------------------------- Server
function edm::setRules()
{
    %rules = "<jc><f2>Welcome to Elimination Death Match!<f0>\n<b0,5:table_head8.bmp><b0,5:table_head8.bmp><jl>\n\n<y17>";
    %rules = %rules @ "You get one life, make it count!\n";
    %rules = %rules @ "Last player remaining, wins.\n";

    setGameInfo(%rules);
    return %rules;
}

## -------------------------------------------------------------------- Custom Fn
function countTeams()
{
  if($gameState != 1) return;

  %teamCount = 0;
  %yellow = 0;
  %blue = 0;
  %red = 0;
  %purple = 0;

  %count = playerManager::getPlayerCount();
  for(%i = 0; %i < %count; %i++)
  {
    %player = playerManager::getPlayerNum(%i);
    if(%player.alive == 1)
    {
      if(getTeam( %player ) == *IDSTR_TEAM_YELLOW)
      {
        if(%yellow == 0)
        {
          %yellow = 1;
          %teamCount = %teamCount + 1;
//        say(0, 0, "Yellow is alive");
        }
      } 
      if(getTeam( %player ) == *IDSTR_TEAM_BLUE)
      {
        if(%blue == 0)
        {
          %blue = 1;
          %teamCount = %teamCount + 2;
//        say(0, 0, "Blue is alive");
        }
      } 
      if(getTeam( %player ) == *IDSTR_TEAM_RED)
      {
        if(%red == 0)
        {
          %red = 1;
          %teamCount = %teamCount + 4;
//        say(0, 0, "Red is alive");
        }
      } 
      if(getTeam( %player ) == *IDSTR_TEAM_PURPLE)
      {
        if(%purple == 0)
        {
          %purple = 1;
          %teamCount = %teamCount + 8;
//        say(0, 0, "Purple is alive");
        }
      } 
    }
  }

  if(%teamCount == 1)
  {
    messageBox(0, "Yellow Team wins the round!");    
    endMission();
  }
  else if(%teamCount == 2)
  {
    messageBox(0, "Blue Team wins the round!");    
    endMission();
  }
  else if(%teamCount == 4)
  {
    messageBox(0, "Red Team wins the round!");    
    endMission();
  }
  else if(%teamCount == 8)
  {
    messageBox(0, "Purple Team wins the round!");    
    endMission();
  }

  else if(%teamCount == 0)
  {
    messageBox(0, "Everyone's dead, game is a draw");    
    endMission();
  }
}

function startGame()
{
  $allowAdds = 0;
  $gameState = 1;
  say("everyone", 0, "The round has now started, no more spawns until the next round.");
  countTeams();
}

function endMission()
{
  $gameState = 2;
  schedule("missionEndConditionMet();", 10);
}

function setHudTimerAll(%time, %increment, %string, %channel)
{
  %count = playerManager::getPlayerCount();
  for(%i = 0; %i < %count; %i++)
  {
    %player = playerManager::getPlayerNum(%i);
    setHudTimer(%time, %increment, %string, %channel, %player);
  }
}
