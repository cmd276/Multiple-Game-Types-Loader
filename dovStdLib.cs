##--------------------------- Header
//  Filename: dovStdLib.cs
//  AUTHOR: ^TFW^ Wilzuun
//  CO-AUTHOR: Drake
//  NOTES: Dark OVerrun functionality.

##--------------------------- History
//
//    History:
//    1.0:
//      Copy and pasted desired effects from C&D map sets.
//      Tested, and works as desired. But code looks ugly.
//      Requested that [eC] Drake™ take a look and give opinions.
//
//    1.0r2
//      Revision and complete rewrite by [eC] Drake™.
//
//    2.0
//      Added rewards for killing players.
//      Added a few of the long standing player "rewards"
//
//    2.1
//      Fixed crash issues with improper vehicle drop attempts.
//      Added random chance artillery drops.
//      Moved a few items around, due to how the game loads data.
//
//    3.0
//      Fixed issue with vehicles spawning in on top of players when using clone
//      mode. Added in Clone Mode.
//      Updated history. Refactored death code. 
//      
//    3.1
//      Changed how overriding settings works.
//      Moved all vehicle configuration settings to wilzuunStdLib.cs

##--------------------------- Plans
//
//    Road Map / Plans:
//
//    Get basic map up and running. -- Done.
//    Get AI to drop near players. -- Done;
//    Get AI to attack the player.  -- Done;
//    Get AI to attack player after respawn  -- Done;
//    Add more AI on AI death.  -- Done;
//
//    Add Tank / Herc Only Modes.
//    Add Tank & Herc Mode.
//
//    Harsh Progression. Things get harder as time goes on.
//    -> Survive Long enough, and get Artillery against you.
//    -> Survive long enough, reinforcements grow larger.  -- Done;
//    Reward Progression. Killing players grants boons.
//    -> AI no longer hunt you for a brief time period.  -- Done;

##--------------------------- Swarm Variables
//  Swarm Variables. This directly affects the number of AI on the map.

//  $fixedSwarm : boolean :
//    If true, Each AI is replaced with a fixed amount of AI.
if ($fixedSwarm == "") $fixedSwarm = true;

//  $swarmStatic : int
//    The value of how many AI to spawn on AI spawn.
//    (if $fixedSwarm == true)
if ($swarmStatic == "") $swarmStatic = 2;

//  $swarmDynanic : int
//    (if $fixedSwarm == false)
//    playerKills / 10 + 1
//    On an AI death, will calculate how many AI to spawn. The value used here
//      will be used as a base value. If you use 15, the first 14 AI deaths will
//      spawn one AI, 15-29 will spawn 2 AI, 30-44 will spawn 3 AI, and so on.
if ($swarmDynanic == "") $swarmDynanic = 2;

//  $swarmClone : boolean
//      If this is true, will make all swarm AI spawned, clones of the player that "owns" them.
if ($swarmClone == "") $swarmClone = false;

##--------------------------- Rewards System
//
//  Rewards System. For those that want to see others fall.
//  This sytem is for PvP play rewards.

//  $pvpReward : boolean
//    true : Turns on giving players incentive to kill other players.
//    false : Players get nothing for killing another player.
if ($pvpReward == "") $pvpReward = true;

//  $pvpReload : boolean
//    true : reload a player killer.
if ($pvpReload == "") $pvpReload = true;

//  $pvpHeal : boolean
//    true : heal the killer
if ($pvpHeal == "") $pvpHeal = true;

//  $pvpStopAI : boolean
//    true : Player is made a member of the AI team. AI stop attacking them.
if ($pvpStopAi == "") $pvpStopAi = true;

//  $pvpTimeAi : int
//    if $pvpStopAI == true
//    The amount of time that the AI doesn't attack a player killer, and how long
//      player killer is on the AI team. in seconds!
if ($pvpTimeAi == "") $pvpTimeAi = 60;

##--------------------------- Success System
//
//  Success System. Most games should reward a successful player. This isnt like
//    that in the least. We want our players to die. Miserably. With a challenge.
//    Question is how hard do we want to get?

//  $pveReward : boolean
//    true : They get rewards!
if ($pveReward == "") $pveReward = true;

//  $pveArtillery : boolean
//    true : Spawn artillery
if ($pveArtillery == "") $pveArtillery = true;

//  $pveArtilleryChance : int
//    1 - 100%, 2 - 50%, 3 - 33%, 4 - 25%, 5 - 20%, 10 - 10% ,
//    100 - 1%, 50 - 2%, 33 - 3%, 25 - 4%, 20 - 5%
if ($pveArtilleryChance == "") $pveArtilleryChance = 5;

//  $pveArtWhen : int
//    At how many kills will we start spawning artillery units against the player.
if ($pveArtWhen == "") $pveArtWhen = 10;

//  $pveReload : boolean
//    true : Stop reloading the player after so much time.
if ($pveReload == "") $pveReload = true;

//  $pveReloadWhen : int
//    When do we start neglecting the reload on kill?
if ($pveReloadWhen == "") $pveReloadWhen = 20;

//  $pveHeal : boolean
//    true : Stop healing the player after so much time.
if ($pveHeal == "") $pveHeal = true;

//  $pveHealWhen : int
//    When do we start neglecting the heal on kill?
if ($pveHealWhen == "") $pveHealWhen = 20;

##--------------------------- Stealth Detection
//
//  Stealth Detection. Alert players when some one leaves / joins with a name
//    like the following:
//    '// ^TFW^ DarkFlare'
//    true is on, false is off.
if ($stealthDetect == "") $stealthDetect = true;

##--------------------------- Spawn Protection
//
//  Spawn Safety Switch
//  $spawnSafetySwitch : bool
//      Enables preventing AI from instantly attacking players.
if ($spawnSafetySwitch == "") $spawnSafetySwitch = true;

//  Spawn Protection. Puts players on Neutral team for X Seconds.
//  $spawnSafetyBase : int
if ($spawnSafetyBase == "") $spawnSafetyBase = 5;

// Spawn Increment. Used as a buffer for more AI.
// floor( player.AICount / spawnSafetyMod) + spawnSafetyBase;
// Example Mod = 5, player.AI = 23;
// A player would get 23/5 = 4.6, Rounded down is 4. 4 + 5 is 9.
// A player would have 9 seconds of being on Neutral Team.

//  $spawnSafetyMod : int
if ($spawnSafetyMod == "") $spawnSafetyMod = 5;

##-----------------------------------------------------------------------------
##-----------------------------------------------------------------------------
//      Dont edit below this section. Its poorly documented, and editing it may
//      cause issues with the game running properly.
##-----------------------------------------------------------------------------
##-----------------------------------------------------------------------------

function getAiCount (%player)
{
    return %player.aiCount;
}

function getMax (%player)
{
    return %player.maxAi;
}

function getSpawn (%player)
{
    return %player.sinceSpawn;
}

function wilzuun::initScoreBoard()
{
   deleteVariables("$ScoreBoard::PlayerColumn*");
   deleteVariables("$ScoreBoard::TeamColumn*");
       // Player ScoreBoard column headings
       $ScoreBoard::PlayerColumnHeader1 = "AI Count";
       $ScoreBoard::PlayerColumnHeader2 = *IDMULT_SCORE_KILLS;
       $ScoreBoard::PlayerColumnHeader3 = *IDMULT_SCORE_DEATHS;
       $ScoreBoard::PlayerColumnHeader4 = "Kills since Spawn";
       $ScoreBoard::PlayerColumnHeader5 = "Max Kills";

       // Player ScoreBoard column functions
       $ScoreBoard::PlayerColumnFunction1 = "getAiCount";
       $ScoreBoard::PlayerColumnFunction2 = "getKills";
       $ScoreBoard::PlayerColumnFunction3 = "getDeaths";
       $ScoreBoard::PlayerColumnFunction4 = "getSpawn";
       $ScoreBoard::PlayerColumnFunction5 = "getMax";
       
   // tell server to process all the scoreboard definitions defined above

   serverInitScoreBoard();
}

function dov::onMissionEnd() // Mission::onEnd
{
    // echo("## ------------ function onMissionEnd()");
    %players = playerManager::getPlayerCount();
    for(%i = 0; %i < %players; %i++)
    {
        %player = playerManager::getPlayerNum(%i);
        deleteObject(%player.group);
    }
}

function dov::onMissionLoad()
{
    // echo("## ------------ function onMissionLoad()");
    %players = playerManager::getPlayerCount();
    for(%i = 0; %i < %players; %i++)
    {
        %player = playerManager::getPlayerNum(%i);
        %player.sinceSpawn = 0;
        %player.aiCount = 0;
        %player.joinAi = 0;
        %player.maxAi = 0;
        %player.group = newObject("group" @ %player, simGroup);
        addToSet("MissionGroup", %player.group);
    }
}

function GetKilled()
{
    // echo("## ------------ function function GetKilled()");
    %playerCount = playerManager::getPlayerCount();
    for(%p = 0; %p <%playerCount; %p++)
    {
        %player = playerManager::getPlayerNum(%p);
        %vehicle = playerManager::playerNumToVehicleId(%player);
        if (%player.joinAi == 1)
        {
            %player.joinAi--;
            setTeam(%vehicle, *IDSTR_TEAM_RED);
            say(%player, 0, "Your grace period is over with... The swarm is after you again");
            schedule("order(" @ %player.group @ ", attack, " @ %vehicle @ ");", waitTime(%player));
        }
        else if (%player.joinAi > 1)
        {
            %player.joinAi--;
        }
    }
    schedule("getKilled();", 1);
}

function dov::onMissionStart()
{
    // echo("## ------------ function onMissionStart()");
    ##  Yellow Team are bots. Make sure they hate red.
    setHostile(*IDSTR_TEAM_YELLOW, *IDSTR_TEAM_RED);
    ## Players should be red. Make sure every one hates them.
    ## Allow them to attack any one.
    setHostile(*IDSTR_TEAM_RED, *IDSTR_TEAM_RED, *IDSTR_TEAM_YELLOW, *IDSTR_TEAM_BLUE, *IDSTR_TEAM_PURPLE);
    setHostile(*IDSTR_TEAM_BLUE, *IDSTR_TEAM_RED);
    setHostile(*IDSTR_TEAM_PURPLE, *IDSTR_TEAM_RED);

    schedule("getKilled();", 1);
}

function dov::player::onAdd(%player)
{
    // echo("## ------------ function player::onAdd()");

    ## Stealth Setting Detection.
    if(($stealthDetect) && (strAlign(1, left, getName(%player)) == "/"))
        say(Everyone, 0, "<F4>" @ getName(%player) @ "<F4> joined the game.");

    ## Set player information.
    ## Max amount of AAI killed before own death.
    %player.maxAi = 0;
    ## Kills since we spawned.
    %player.sinceSpawn = 0;
    ## We have this many AI belonging to this player on the field at this time.
    %player.aiCount = 0;
    ## This player killed another player, and are free to roam.
    %player.joinAi = 0;

    ## Group name: group2049, etc.
    %player.group = newObject("group" @ %player, simGroup);
    addToSet("MissionGroup", %player.group);
}

function dov::player::onRemove(%player)
{
    // echo("## ------------ function player::onRemove()");
    deleteObject(%player.group);
}

function setDefaultMissionOptions()
{
    // echo("## ------------ function setDefaultMissionOptions()");
    $server::TeamPlay = false;
    $server::AllowDeathmatch = true;
    $server::AllowTeamPlay = false;
}

function dov::setRules()
{
    // echo("## ------------ function setRules()");
    %rules = "<jc><f2>Welcome to Dark Overrun!<f0>\n<b0,5:table_head8.bmp><b0,5:table_head8.bmp><jl>\n\n<y17>";
    %rules = %rules @ "The Swarmie Bois are mad at you for betraying them. They've rallied their buddies and are coming after you like the mafia.\n";

    if ($fixedSwarm == true) 
    {
        %rules = %rules @ "The Swarm grows by " @ $swarmStatic @ " every time you kill 1 enemy.\n";
    }
    else 
    {
        %rules = %rules @ "The Swarm grows by (Your Kill Count / " @ $swarmDynanic @ ") every time you kill 1 enemy.\n";
    }
    if ($swarmClone == true)
    {
        %rules = %rules @ "All enemy vehicles are a clone of your vehicle. The pilot rivals that of Bek Storm.\n";
    }
    else
    {
        %rules = %rules @ "All enemies drive random vehicles.\n";
    }
    %rules = %rules @ "\n\n<jc><f2>Player Rewards<f0>\n<b0,5:table_head8.bmp><b0,5:table_head8.bmp><jl>\n\n<y17>";
    if ($pvpReward == true)
    {
        if ($pvpReload == true)
        {
            %rules = %rules @ "There is a reload after every player kill\n";
        }
        if ($pvpHeal == true)
        {
            %rules = %rules @ "There is a heal after every player kill\n";
        }
        if ($pvpStopAi == true)
        {
            %rules = %rules @ "You join the hunters if you kill another player. (You join the hunters for " @ $pvpTimeAi @ " seconds)\n";
        }
    }
    if ($pveReward == true)
    {
        if ($pveReload == true)
        {
            %rules = %rules @ "There is a reload after every AI kill, for the first " @ $pveReloadWhen @ " kills\n";
        }
        if ($pveHeal == true)
        {
            %rules = %rules @ "There is a heal after every AI kil, for the first " @ $pveHealWhen @ " kills\n";
        }
    }

    %rules = %rules @ "\n\n<jc><f2>Player Rewards<f0>\n<b0,5:table_head8.bmp><b0,5:table_head8.bmp><jl>\n\n<y17>These are currently broken. Sorry!";

    setGameInfo(%rules);
    return %rules;
}


function dov::vehicle::onAdd(%vehicleId)
{
    // echo("## ------------ function vehicle::onAdd()");
    %player = playerManager::vehicleIdToPlayerNum(%vehicleId);
    ## Anti-AI check.
    if (%player == 0) return;
    
    %timeToWait = waitTime(%player);


    if (%player.aiCount <= 0)
    {
        for(%i=0; %i < swarmVolume(%player); %i++)
            spawnSwarm(%player);

        schedule("order(" @ %player.group @ ", attack, " @ %vehicleId @ ");", 3);
    }
    else
    {
        schedule("order(" @ %player.group @ ", attack, " @ %vehicleId @ ");", %timeToWait);
    }

    schedule("setTeam(" @ %vehicleId @ ",*IDSTR_TEAM_RED);",%timeToWait);
    setTeam(%vehicleId, *IDSTR_TEAM_NUETRAL);
}

function dov::vehicle::onDestroyed(%destroyed, %destroyer)
{
    // echo("## ------------ function vehicle::onDestroyed()");
    %message = getFancyDeathMessage(getHUDName(%destroyed), getHUDName(%destroyer));
    if(%message != "")
        say( 0, 0, %message);
    vehicle::onDestroyedLog(%destroyed, %destroyer);

    %dead = playerManager::vehicleIdToPlayerNum(%destroyed);
    %living = playerManager::vehicleIdToPlayerNum(%destroyer);

    ## if an AI Dies, schedule its deletion.
    // AI killed AI... oops?
    if ((%living == 0) && (%dead == 0))
    {
        %destroyed.owner.aiCount--;
        schedule("deleteObject("@ %destroyed @ ");",1);
    }
    // AI kills Player.
    else if ((%living == 0) && (%dead != 0))
    {
        playerDied(%dead);
    }
    // Player Kills Player
    else if ((%living != 0) && (%dead != 0) && (%dead != %living))
    {
        playerLived(%living,true);
        playerDied(%dead);
    }

    // Player Kills AI
    else if ((%living != 0) && (%dead == 0))
    {
        if (getTeam(%destroyed) == getTeam(%destroyer))
        {
            %living.joinAi = 1;
        }

        %destroyed.owner.aiCount--;
        schedule("deleteObject("@ %destroyed @ ");",1);
    
        %countToSpawn = swarmVolume(%living);
        for(%i = 0; %i < %countToSpawn; %i++)
        {
            // echo("Spawning things");
            spawnSwarm(%living);
        }
        playerLived(%living);
    }
  
    // We dont know any more... Or any thing...
    else 
    {
        // uh oh...
        echo("UNKNOWN DEATH SET");
    }
}

function playerLived(%player, %pvp) 
{
    // echo("## ------------ function playerLived()");

    %destroyer = playerManager::playerNumtoVehicleId(%player);
    // Do we reload them still?
    if (($pveReward) && ($pveReload) && ($pveReloadWhen >= %player.sinceSpawn))
        reloadObject(%destroyer, 99999);
    
    // Do we still heal them?
    if (($pveReward) && ($pveHeal) && ($pveHealWhen >= %player.sinceSpawn))
        healObject(%destroyer,99999);

    if (%pvp)
        if (($pvpReward)&&($pvpStopAi)) // how about joining the swarm, instead of being eaten?
        {
            %player.joinAi = %player.joinAi + $pvpTimeAi; // Extend, or give time.
            setTeam(%destroyer, *IDSTR_TEAM_YELLOW);
            schedule("order(" @ %player.group @ ", guard, " @ %destroyer @ ");", waitTime(%player));
            say(%player, 0, "The swarm is willing to overlook your rogue likeness for a period...");
        }
    %player.sinceSpawn++;
}

function playerDied(%player)
{
    // echo("## ------------ function playerDied()");
    if (%player.sinceSpawn > %player.maxAi)
    {
        %player.maxAi = %player.sinceSpawn;
        say(%player, 0, "New Personal Best! " @ %player.sinceSpawn @ " kills before death");
        // storeScore(%player, %player.sinceSpawn);
    }
    %player.sinceSpawn = 0;
}

function spawnSwarm(%player)
{
    echo("## ------------ function spawnSwarm()");
    ## If AI, quit.
    if (%player == 0) return;
    ## Get the players vehicle ID for later use.
    %vehicleId = playerManager::playerNumToVehicleId(%player);
    echo("----------------------------------------------------"@%vehicleId);

    %x1 = getPosition(%vehicleId, x) - 3000;
    %x2 = getPosition(%vehicleId, x) + 3000;
    %y1 = getPosition(%vehicleId, y) - 3000;
    %y2 = getPosition(%vehicleId, y) + 3000;
        
    if ($swarmClone == true)
    {
        echo("Clone Swarm Detected.");
        storeObject(%vehicleId, %vehicleId @ "_test.veh");
        %newSpawn = loadObject("Swarm Member", %vehicleId @ "_test.veh");
    }
    else
    {
        %newSpawn = loadObject("Swarm Member", wilzuun::loadVehicle());
    }

    %newSpawn.owner = %player;
    setPilotId(%newSpawn, 32);
    randomTransport(%newSpawn, %x1, %y1, %x2, %y2);
    setTeam(%newSpawn, *IDSTR_TEAM_YELLOW);
    addToSet("MissionGroup\\group" @ %player, %newSpawn);
    order(%newSpawn, speed, high);
    schedule("order(" @ %player.group @ ", attack, " @ %vehicleId @ ");", waitTime(%player));
    
    %player.aiCount++;
    // messageBox(0,"Swarm Drop");
}

function swarmVolume(%player)
{
    // echo("## ------------ function swarmVolume()");
    if ($fixedSwarm == true)
        return $swarmStatic;
    else if ($fixedSwarm == false)
        return floor(%player.sinceSpawn / $swarmDynanic) + 1;
}

function isItOn (%v)
{
    if (%v == true)
        return "on";
    else
        return "off";
}

function waitTime (%player) 
{
    if ($spawnSafetySwitch) 
    {
        return floor($spawnSafetyBase + (player.aiCount / $spawnSafetyMod));
    }
    else return 0.5;
}
