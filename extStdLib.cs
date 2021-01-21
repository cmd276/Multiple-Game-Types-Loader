##--------------------------- Header
// FILE:        ExtStdLib.cs
// AUTHORS:     ^TFW^ Wilzuun
// LAST MOD:    Dec 25th, 2019
// VERSION:     1.0r
// NOTES:       Warframe Extermination game-type.
##--------------------------- Version History
//  1.0r
//      - Initial startup.

##--------------------------- Notes
//  Gameplay notes: 
//      Exterminate Missions requires players to kill a limited number of enemies in the area, and then make it to extraction. A counter will be displayed below the minimap showing the number of enemies remaining.

##--------------------------- On-The-Fly Settings
// Feel free to edit this variables in your own scripts as much as you want.
// These are more of a default setup, so one can minimally do something, and see results.
$killLimit = randomInt(50,100);
$spawnLimit = $killLimit * (randomInt(100,200)/100);
$respawnCount = randomInt(1,3);
$spawnDistance = 6000;

##--------------------------- Concrete Settings
// Please don't edit these, unless you understand the issues you may cause.
$kills = 0;

##--------------------------- execute files.
exec("getNPC.cs");

##--------------------------- Functions
function Ext::onMissionStart()
{
   say(0, 0, "Mission starting...");
}

function spawnEnemies()
{
   for(%i = 0; %i < $respawnCount; %i++)
   {
      %new = getNPC();
      %randPlayer = playerManager::getPlayerNum(randomInt(0, playerManager::getPlayerCount()));
      
   }
}

##--------------------------- Imported from DMstdLib.cs
function Ext::vehicle::onDestroyed(%destroyed, %destroyer)
{
   // left over from missionStdLib.cs
   vehicle::onDestroyedLog(%destroyed, %destroyer);
   
   // this is weird but %destroyer isn't necessarily a vehicle
   %message = getFancyDeathMessage(getHUDName(%destroyed), getHUDName(%destroyer));
   if(%message != "")
   {
      say( 0, 0, %message);
   }

   // enforce the rules
   if($server::TeamPlay == true)
   {
      if(
         (getTeam(%destroyed) == getTeam(%destroyer)) &&
         (%destroyed != %destroyer)
      )
      {
         antiTeamKill(%destroyer);
      }
   } 

   %dead = playerManager::vehicleIdToPlayerNum(%destroyed);
   %living = playerManager::vehicleIdToPlayerNum(%destroyer);
   if ((%living != 0) && (%dead == 0))
   {
      $kills++;
      spawnEnemies();
   }
}

function Ext::vehicle::onAdd(%this)
{
   %player = playerManager::vehicleIdToPlayerNum(%this);
   if(%player == 0)  // that's the server
      return;      
   setTeam(%this, *IDSTR_TEAM_RED);
}

function getPlayerScore(%a)
{
   return((getKills(%a) * $killPoints) - (getDeaths(%a) * $deathPoints));
}

function getTeamScore(%a)
{
   return((getTeamKills(%a) * $killPoints) - (getTeamDeaths(%a) * $deathPoints));
}

function Ext::initScoreBoard()
{
   deleteVariables("$ScoreBoard::PlayerColumn*");
   deleteVariables("$ScoreBoard::TeamColumn*");

   if($server::TeamPlay == "True")    
   {
       // Player ScoreBoard column headings
       $ScoreBoard::PlayerColumnHeader1 = *IDMULT_SCORE_TEAM;
       $ScoreBoard::PlayerColumnHeader2 = *IDMULT_SCORE_SQUAD;
       $ScoreBoard::PlayerColumnHeader3 = *IDMULT_SCORE_SCORE;
       $ScoreBoard::PlayerColumnHeader4 = *IDMULT_SCORE_KILLS;
       $ScoreBoard::PlayerColumnHeader5 = *IDMULT_SCORE_DEATHS;

       // Player ScoreBoard column functions
       $ScoreBoard::PlayerColumnFunction1 = "getTeam";
       $ScoreBoard::PlayerColumnFunction2 = "getSquad";
       $ScoreBoard::PlayerColumnFunction3 = "getPlayerScore";
       $ScoreBoard::PlayerColumnFunction4 = "getKills";
       $ScoreBoard::PlayerColumnFunction5 = "getDeaths";
   }
   else
   {
       // Player ScoreBoard column headings
       $ScoreBoard::PlayerColumnHeader1 = *IDMULT_SCORE_SQUAD;
       $ScoreBoard::PlayerColumnHeader2 = *IDMULT_SCORE_SCORE;
       $ScoreBoard::PlayerColumnHeader3 = *IDMULT_SCORE_KILLS;
       $ScoreBoard::PlayerColumnHeader4 = *IDMULT_SCORE_DEATHS;

       // Player ScoreBoard column functions
       $ScoreBoard::PlayerColumnFunction1 = "getSquad";
       $ScoreBoard::PlayerColumnFunction2 = "getPlayerScore";
       $ScoreBoard::PlayerColumnFunction3 = "getKills";
       $ScoreBoard::PlayerColumnFunction4 = "getDeaths";
   }

   // Team ScoreBoard column headings
   $ScoreBoard::TeamColumnHeader1 = *IDMULT_SCORE_SCORE;
   $ScoreBoard::TeamColumnHeader2 = *IDMULT_SCORE_PLAYERS;
   $ScoreBoard::TeamColumnHeader3 = *IDMULT_SCORE_KILLS;
   $ScoreBoard::TeamColumnHeader4 = *IDMULT_SCORE_DEATHS;

   // Team ScoreBoard column functions
   $ScoreBoard::TeamColumnFunction1 = "getTeamScore";
   $ScoreBoard::TeamColumnFunction2 = "getNumberOfPlayersOnTeam";
   $ScoreBoard::TeamColumnFunction3 = "getTeamKills";
   $ScoreBoard::TeamColumnFunction4 = "getTeamDeaths";

   // tell server to process all the scoreboard definitions defined above
   serverInitScoreBoard();
}
