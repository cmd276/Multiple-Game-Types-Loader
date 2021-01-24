##--------------------------- Header
//  Filename: domStdLib.cs
//  AUTHOR: ^TFW^ Wilzuun
//  CO-AUTHOR: Drake
//  NOTES: Domination Game Library.

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
//
//    3.2
//      Merged settings back into a single file. This was to enable the random
//      game generator to work a bit smoother. Edited order of scripts, so that
//      its more organized.
//      Added `inMissionStart()` and `inMissionEnd()` functions to allow for the
//      new game randomizer to be able to load and unload different game types.
//      These functions will attempt to properly clean up each of their 
//      respective game types and then load a new game type. Removed unused
//      functions from the library. 

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
//    Add Tank / Herc Only Modes. -- Via settings. Done.
//    Add Tank & Herc Mode. -- Via settings. Done.
//
//    Harsh Progression. Things get harder as time goes on.
//    -> Survive Long enough, and get Artillery against you.
//    -> Survive long enough, reinforcements grow larger.  -- Done;
//    Reward Progression. Killing players grants boons.
//    -> AI no longer hunt you for a brief time period.  -- Done;

## -------------------------------------------------------------------- Settings
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


## ------------------------------------------------------------------- onMission
// Deletes all the AI to all players on missionEnd.
function dom::onMissionEnd() // Mission::onEnd
{
    %players = playerManager::getPlayerCount();
    for(%i = 0; %i < %players; %i++)
    {
        %player = playerManager::getPlayerNum(%i);
        deleteObject(%player.group);
    }
    deleteFunctions("dom::GetKilled()");
}

// A little setup of the game on the start of a new game.
function dom::onMissionLoad()
{
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

// Make sure the right teams hate each other.
function dom::onMissionStart()
{
    ##  Yellow Team are bots. Make sure they hate red.
    setHostile(*IDSTR_TEAM_YELLOW, *IDSTR_TEAM_RED);
    ## Players should be red. Make sure every one hates them.
    ## Allow them to attack any one.
    setHostile(*IDSTR_TEAM_RED, *IDSTR_TEAM_RED, *IDSTR_TEAM_YELLOW, *IDSTR_TEAM_BLUE, *IDSTR_TEAM_PURPLE);
    setHostile(*IDSTR_TEAM_BLUE, *IDSTR_TEAM_RED);
    setHostile(*IDSTR_TEAM_PURPLE, *IDSTR_TEAM_RED);

    schedule("dom::getKilled();", 1);
}

## ------------------------------------------------------------------- inMission

// For loading while mission is already in session.
function dom::inMissionStart()
{
    // load default options.
    dom::setDefaultMissionOptions();
    // because the onMissionStart does so much for the game setup... Lets run it.
    dom::onMissionLoad();
    dom::onMissionStart();
    // get number of players.
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
            dom::vehicle::onAdd(%vehicle);
        }
    }
    
    // set allowed items.
    dom::setAllowedItems();
    
    // set mission name to reflect new game type.
    $missionName = "DOM - Dynamic Games";
    
    // Announce new game type.
    say(0,0,"DOMINATION now in effect!");
}

// For unlaoding the play type while a mission is running.
function dom::inMissionEnd()
{
    // since its well rounded out clean up, run it.
    dom::onMissionEnd();
    // we really want this function gone...
    deleteFunctions("dom::GetKilled()");
}
## ------------------------------------------------------------------- Player
// whenever a player joins, make sure they get setup properly.
function dom::player::onAdd(%player)
{
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

// Delete a player's AI group. Don't need those AI wandering around waiting for
// some poor, unwitting player to find them.
function dom::player::onRemove(%player)
{
    deleteObject(%player.group);
}

## ------------------------------------------------------------------- Vehicle

// When a player spawns into a game, make sure they have 'friends' to visit.
// Or if they already have friends, make sure those friends know where to find
// the player.
function dom::vehicle::onAdd(%vehicleId)
{
    %player = playerManager::vehicleIdToPlayerNum(%vehicleId);
    ## Anti-AI check.
    if (%player == 0) return;
    
    %timeToWait = dom::waitTime(%player);

    if (%player.aiCount <= 0)
    {
        for(%i=0; %i < dom::swarmVolume(%player); %i++)
            dom::spawnSwarm(%player);

        schedule("order(" @ %player.group @ ", attack, " @ %vehicleId @ ");", 3);
    }
    else
    {
        schedule("order(" @ %player.group @ ", attack, " @ %vehicleId @ ");", %timeToWait);
    }

    schedule("setTeam(" @ %vehicleId @ ",*IDSTR_TEAM_RED);",%timeToWait);
    setTeam(%vehicleId, *IDSTR_TEAM_NUETRAL);
}

// some one died... Who, how, why, when, where?
// We just wanna know how, and who. Was it AI vs AI? or player vs AI?
// if a player, give that player more friends!
function dom::vehicle::onDestroyed(%destroyed, %destroyer)
{
    %dead = playerManager::vehicleIdToPlayerNum(%destroyed);
    %living = playerManager::vehicleIdToPlayerNum(%destroyer);

    ## if an AI Dies, schedule its deletion.
    // AI killed AI... oops?
    if ((%living == 0) && (%dead == 0))
    {
        %destroyed.owner.aiCount--;
        schedule("deleteObject("@ %destroyed @ ");",1);
    }
    // AI kills Player. Sorry player!
    else if ((%living == 0) && (%dead != 0))
    {
        dom::playerDied(%dead);
    }
    // Player Kills Player, reward them!
    // Players don't get anything for suicide.
    else if ((%living != 0) && (%dead != 0) && (%dead != %living))
    {
        dom::playerLived(%living,true);
        dom::playerDied(%dead);
    }

    // Player Kills AI. Player needs more friends, clearly.
    else if ((%living != 0) && (%dead == 0))
    {
        // give 1 second to joining AI team if both %living and %dead are on the
        // same team. Clearly betrayal.
        if (getTeam(%destroyed) == getTeam(%destroyer))
        {
            %living.joinAi = 1;
        }
        // decrease the owner's AI count.
        %destroyed.owner.aiCount--;
        // schedule a delete. Instantly deleting causes a crash? wtf dynamix?
        schedule("deleteObject("@ %destroyed @ ");",1);
        
        // how many AI are we to spawn?
        %countToSpawn = dom::swarmVolume(%living);
        // SPAWN NEW FRIENDS
        for(%i = 0; %i < %countToSpawn; %i++)
        {
            dom::spawnSwarm(%living);
        }
        // run some repeative shit.
        dom::playerLived(%living);
    }
  
    // We dont know any more... Or any thing...
    else 
    {
        // uh oh...
        echo("UNKNOWN DEATH SET");
    }
}

// The player killed something... Do we give rewards?
function dom::playerLived(%player, %pvp) 
{
    %destroyer = playerManager::playerNumtoVehicleId(%player);
    // Do we reload them still?
    if (($pveReward) && ($pveReload) && ($pveReloadWhen >= %player.sinceSpawn))
        reloadObject(%destroyer, 99999);
    
    // Do we still heal them?
    if (($pveReward) && ($pveHeal) && ($pveHealWhen >= %player.sinceSpawn))
        healObject(%destroyer,99999);

    // are we awarding pvp behavior?
    if (%pvp)
        if (($pvpReward)&&($pvpStopAi)) // how about joining the swarm, instead of being eaten?
        {
            %player.joinAi = %player.joinAi + $pvpTimeAi; // Extend, or give time.
            setTeam(%destroyer, *IDSTR_TEAM_YELLOW);
            schedule("order(" @ %player.group @ ", guard, " @ %destroyer @ ");", dom::waitTime(%player));
            say(%player, 0, "The swarm is willing to overlook your rogue likeness for a period...");
        }
    %player.sinceSpawn++;
}

// oh no, the player died... Do some record keeping.
function dom::playerDied(%player)
{
    if (%player.sinceSpawn > %player.maxAi)
    {
        %player.maxAi = %player.sinceSpawn;
        say(%player, 0, "New Personal Best! " @ %player.sinceSpawn @ " kills before death");
    }
    %player.sinceSpawn = 0;
}

// Spawn more friends for a player.
function dom::spawnSwarm(%player)
{
    ## If AI, quit.
    if (%player == 0) return;
    ## Get the players vehicle ID for later use.
    %vehicleId = playerManager::playerNumToVehicleId(%player);

    // get a random location within 3km of player.
    %x1 = getPosition(%vehicleId, x) - 3000;
    %x2 = getPosition(%vehicleId, x) + 3000;
    %y1 = getPosition(%vehicleId, y) - 3000;
    %y2 = getPosition(%vehicleId, y) + 3000;
    
    // Are we cloning the player?
    if ($swarmClone == true)
    {
        storeObject(%vehicleId, %vehicleId @ "_test.veh");
        %newSpawn = loadObject("Swarm Member", %vehicleId @ "_test.veh");
    }
    // we aren't cloning, get random vehicle.
    else
    {
        %newSpawn = loadObject("Swarm Member", wilzuun::loadVehicle());
    }
    
    // give the new AI an owner.
    %newSpawn.owner = %player;
    // give them a pilot.
    setPilotId(%newSpawn, 32);
    // teleport them randomly.
    randomTransport(%newSpawn, %x1, %y1, %x2, %y2);
    // set their team.
    setTeam(%newSpawn, *IDSTR_TEAM_YELLOW);
    // add them to the right group.
    addToSet("MissionGroup\\group" @ %player, %newSpawn);
    // order them full speed to attack.
    order(%newSpawn, speed, high);
    // schedule an attack... i don't know why this is here?
    schedule("order(" @ %player.group @ ", attack, " @ %vehicleId @ ");", dom::waitTime(%player));
    // increase the player's active AI count.
    %player.aiCount++;
}

// how many friends are we spawning?
function dom::swarmVolume(%player)
{
    if ($fixedSwarm == true)
        return $swarmStatic;
    else if ($fixedSwarm == false)
        return floor(%player.sinceSpawn / $swarmDynanic) + 1;
}

## ------------------------------------------------------------------- Server
// Scoreboard asking for amount of AI on field for a player.
function dom::getAiCount (%player)
{
    return %player.aiCount;
}

// Score board asking how many AI a player killed before dying themselves.
function dom::getMax (%player)
{
    return %player.maxAi;
}

// Scoreboard asking for the number AI a player killed since they spawned.
function dom::getSpawn (%player)
{
    return %player.sinceSpawn;
}

// scoreboard for this game type.
function dom::initScoreBoard()
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
       $ScoreBoard::PlayerColumnFunction1 = "dom::getAiCount";
       $ScoreBoard::PlayerColumnFunction2 = "getKills";
       $ScoreBoard::PlayerColumnFunction3 = "getDeaths";
       $ScoreBoard::PlayerColumnFunction4 = "dom::getSpawn";
       $ScoreBoard::PlayerColumnFunction5 = "dom::getMax";
       
   // tell server to process all the scoreboard definitions defined above

   serverInitScoreBoard();
}

// Set somethings server wide, like we're not doing teams.
function dom::setDefaultMissionOptions()
{
    $server::TeamPlay = false;
    $server::AllowDeathmatch = true;
    $server::AllowTeamPlay = false;
}
// set the rules tab info.
function dom::setRules()
{
    %rules = "<jc><f2>Welcome to Domination!<f0>\n<b0,5:table_head8.bmp><b0,5:table_head8.bmp><jl>\n\n<y17>";
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

function dom::setAllowedItems()
{
    //Vehicles
    allowVehicle( all, true);   // first turn them all off

    // Terran Vehicles
    allowVehicle(   1, true );  // Terran Apocalypse                
    allowVehicle(   2, true );  // Terran Minotaur                  
    allowVehicle(   3, true );  // Terran Gorgon                    
    allowVehicle(   4, true );  // Terran Talon                     
    allowVehicle(   5, true );  // Terran Basilisk                  
    allowVehicle(   6, true );  // Paladin Tank                     
    allowVehicle(   7, true );  // Myrmidon Tank                    
    allowVehicle(   8, true );  // Disruptor Tank                   
    allowVehicle( 133, true );  // Nike Artillery                   
    allowVehicle( 134, true );  // Supressor Tank                   
    allowVehicle(   9, true );  // Banshee Flyer                    
    allowVehicle(  18, true );  // Cargo ship                       
    allowVehicle(  19, true );  // Escape ship                      
    allowVehicle( 130, true );  // Drop ship                        
    allowVehicle( 131, true );  // Draco Bomber                     
    allowVehicle( 132, true );  // Conveyor                         

    // Knight Vehicles
    allowVehicle(  10, true );  // Knight's Apocalypse
    allowVehicle(  11, true );  // Knight's Minotaur
    allowVehicle(  12, true );  // Knight's Gorgon  
    allowVehicle(  13, true );  // Knight's Talon   
    allowVehicle(  14, true );  // Knight's Basilisk
    allowVehicle(  15, true );  // Knight's Paladin 
    allowVehicle(  16, true );  // Knight's Myrmidon
    allowVehicle(  17, true );  // Knight's Disruptor
    allowVehicle( 110, true );  // Knight's Banshee 
    allowVehicle( 111, true );  // Knight's Drop Ship

    // Cybrid Vehicles
    allowVehicle(  20, true );  // Cybrid Seeker       
    allowVehicle(  21, true );  // Cybrid Goad         
    allowVehicle(  22, true );  // Cybrid Shepherd     
    allowVehicle(  23, true );  // Cybrid Adjudicator  
    allowVehicle(  24, true );  // Cybrid Executioner  
    allowVehicle(  25, true );  // Bolo Tank           
    allowVehicle(  26, true );  // Recluse Tank        
    allowVehicle(  27, true ); // Platinum Adjudicator (SP version, not selectable)
    allowVehicle(  28, true ); // Platinum Executioner (SP version, not selectable)
    allowVehicle(  55, true );  // Platinum Adjudicator 2
    allowVehicle(  56, true );  // Platinum Executioner 2
    allowVehicle(  90, true );  // Cybrid Artillery    
    allowVehicle(  91, true );  // Cybrid Advocate     
    allowVehicle(  92, true );  // Cybrid Drop Ship    
    allowVehicle(  93, true );  // Cybrid Consul Bomber

    // Metagen Vehicles
    allowVehicle(  35, true );  // Metagen Seeker   
    allowVehicle(  36, true );  // Metagen Goad     
    allowVehicle(  37, true );  // Metagen Shepherd 
    allowVehicle(  38, true );  // Metagen Adjudicator
    allowVehicle(  39, true );  // Metagen Executioner

    // Rebel Vehicles
    allowVehicle(  30, true );  // Rebel Emancipator
    allowVehicle(  31, true );  // Avenger Tank   
    allowVehicle(  32, true );  // Dreadnought Tank
    allowVehicle(  33, true );  // Rebel Olympian 
    allowVehicle(  72, true );  // Rebel Thumper  
    allowVehicle( 137, true );  // Rebel Artillery
    allowVehicle( 138, true );  // Rebel bike
    allowVehicle( 150, true );  // SUAV Bus

    // Special Vehicles
    allowVehicle(  40, true );  // Harabec's Apocalypse   
    allowVehicle(  43, true );  // Harabec's Apocalypse, cin
    allowVehicle(  41, true );  // Harabec's Predator     
    allowVehicle(  45, true );  // Harabec's Super Predator
    allowVehicle(  42, true );  // Caanan's Basilisk      
    allowVehicle(  44, true );  // Caanan's Basilisk, cin 
    allowVehicle(  29, true );  // Prometheus             

    // Pirate Vehicles
    allowVehicle(  50, true );  // Pirate's Apocalypse
    allowVehicle(  51, true );  // Pirate's Dreadlock
    allowVehicle(  52, true );  // Pirate's Emancipator

    // Drone Vehicles
    allowVehicle(  60, true );  // Terran Empty Cargo         
    allowVehicle(  61, true );  // Terran Ammo Cargo          
    allowVehicle(  62, true );  // Terran Big Ammo Cargo      
    allowVehicle(  63, true );  // Terran Big Personnel Cargo 
    allowVehicle(  64, true );  // Terran Fuel Cargo          
    allowVehicle(  65, true );  // Terran Minotaur Cargo      
    allowVehicle(  71, true );  // Terran Utility Truck       
    allowVehicle( 135, true );  // Terran Sovereign           
    allowVehicle( 136, true );  // Terran Surveyor            
    allowVehicle(  73, true );  // Terran Starefield          
    allowVehicle(  66, true );  // Rebel Empty Cargo          
    allowVehicle(  67, true );  // Rebel Ammo Cargo           
    allowVehicle(  68, true );  // Rebel Big Cargo Transport  
    allowVehicle(  69, true );  // Rebel Bix Box Cargo Transport
    allowVehicle(  70, true );  // Rebel Box Cargo Transport  
    allowVehicle(  94, true );  // Cybrid Omnicrawler         
    allowVehicle(  95, true );  // Cybrid Protector           
    allowVehicle(  96, true );  // Cybrid Jamma               

    //weapons
    allowWeapon(  all,  true );
    allowWeapon(  101,  true );      //Laser
    allowWeapon(  102,  true );      //Heavy Laser
    allowWeapon(  103,  true );      //Comp Laser
    allowWeapon(  104,  true );      //Twin Laser
    allowWeapon(  105,  true );      //Emp
    allowWeapon(  106,  true );      //ELF
    allowWeapon(  107,  true );      //Blaster
    allowWeapon(  108,  true );      //Heavy Blaster
    allowWeapon(  109,  true );      //PBW
    allowWeapon(  110,  true );      //Plasma
    allowWeapon(  111,  true );      //Blink Gun
    allowWeapon(  112,  true );      //Qgun
    allowWeapon(  113,  true );      //MFAC
    allowWeapon(  114,  true );      //Nano Infuser
    allowWeapon(  115,  true );      //Nanite Cannon
    allowWeapon(  116,  true );      //Autocannon
    allowWeapon(  117,  true );      //Hvy Autocannon
    allowWeapon(  118,  true );      //EMC Autocannon
    allowWeapon(  119,  true );      //Blast Cannon
    allowWeapon(  120,  true );      //Hvy Blast Can
    allowWeapon(  121,  true );      //Rail Gun
    allowWeapon(  124,  true );      //Pit Viper 8
    allowWeapon(  125,  true );      //Pit Viper 12
    allowWeapon(  126,  true );      //Sparrow 6
    allowWeapon(  127,  true );      //Sparrow 10
    allowWeapon(  128,  true );      //SWARM 6
    allowWeapon(  129,  true );      //Minion
    allowWeapon(  130,  true );      //Shrike 8
    allowWeapon(  147,  true );      //Aphid
    allowWeapon(  131,  true );      //Arachnitron 4
    allowWeapon(  132,  true );      //Arachnitron 8
    allowWeapon(  133,  true );      //Arachnitron 12
    allowWeapon(  134,  true );      //Proximity 6
    allowWeapon(  135,  true );      //Proximity 10
    allowWeapon(  136,  true );      //Proximity 15
    allowWeapon(  142,  true );      //Radiation Gun
    allowWeapon(    3,  true );      //Disrupter
    allowWeapon(  150,  true );      //SMART Gun

    //Components
    //Reactors
    allowComponent(  200,  true );      //Human Micro Reactor
    allowComponent(  201,  true );      //Small Human Reactor 2 -- small
    allowComponent(  202,  true );      //Medium Human Reactor 1 Standard
    allowComponent(  203,  true );      //Medium Human Reactor 2 medium
    allowComponent(  204,  true );      //Large Human Reactor 1 -- large
    allowComponent(  205,  true );      //Large Human Reactor 2-- Maxim
    allowComponent(  225,  true );      //Small Cybrid Reactor 1 -- Alpha
    allowComponent(  226,  true );      //Small Cybrid Reactor 2-- Beta
    allowComponent(  227,  true );      //Medium Cybrid Reactor 1 -- Gamma
    allowComponent(  228,  true );      //Medium Cybrid Reactor 2--delta
    allowComponent(  229,  true );      //Large Cybrid Reactor 1--epsilon
    allowComponent(  230,  true );      //Large Cybrid Reactor 2--theta
    //Shields
    allowComponent(  300,  true );      //Human Standard Shield
    allowComponent(  301,  true );      //Human Protector Shield
    allowComponent(  302,  true );      //Human Guardian Shield
    allowComponent(  303,  true );      //Human FastCharge Shield
    allowComponent(  304,  true );      //Human Centurian Shield
    allowComponent(  305,  true );      //Human Repulsor Shield
    allowComponent(  306,  true );      //Human Titan Shield
    allowComponent(  307,  true );      //Human Medusa Shield
    allowComponent(  326,  true );      //Cybrid Alpha Shield
    allowComponent(  327,  true );      //Cybrid Beta Shield
    allowComponent(  328,  true );      //Cybrid Gamma Shield
    allowComponent(  329,  true );      //Cybrid Delta Shield
    allowComponent(  330,  true );      //Cybrid Epsilon Shield
    allowComponent(  331,  true );      //Cybrid Zeta Shield
    allowComponent(  332,  true );      //Cybrid Eta Shield
    allowComponent(  333,  true );      //Cybrid Theta Shield

    //Sensors
    allowComponent(  400,  true );      //Basic Human Sensor
    allowComponent(  401,  true );      //Long Range Sensor -- Ranger
    allowComponent(  408,  true );      //Standard Human Sensor
    allowComponent(  409,  true );      //Human Longbow Sensor
    allowComponent(  410,  true );      //Human Infiltrator Sensor
    allowComponent(  411,  true );      //Human Crossbow Sensor
    allowComponent(  412,  true );      //Human Ultralight Sensor
    allowComponent(  413,  true );      //Human Hound Dog Sensor
    allowComponent(  414,  true );      //Thermal Sensor
    allowComponent(  426,  true );      //Basic Cybrid Sensor (Alpha)
    allowComponent(  427,  true );      //Long Range Cybrid Sensor (Beta)
    allowComponent(  428,  true );      //Standard Cybrid Sensor (Gamma)
    allowComponent(  429,  true );      //Cybrid Longbow Sensor (Delta)
    allowComponent(  430,  true );      //Cybrid Infiltrator Sensor (Epsilon)
    allowComponent(  431,  true );      //Cybrid Crossbow Sensor (Zeta)
    allowComponent(  432,  true );      //Cybrid Ultralight Sensor (Eta)
    allowComponent(  433,  true );      //Cybrid Hound Dog Sensor (Theta)
    allowComponent(  434,  true );      //Motion Detector (Iota)

    //Engines
    allowComponent(  100,  true );      //Human Light Vehicle Engine
    allowComponent(  101,  true );      //Human High Output Light Engine
    allowComponent(  102,  true );      //Human Agile Light Engine
    allowComponent(  103,  true );      //Human Standard Medium Engine
    allowComponent(  104,  true );      //Human High Output Medium Engine
    allowComponent(  105,  true );      //Human Medium Agility Engine
    allowComponent(  106,  true );      //Human Standard Heavy Engine
    allowComponent(  107,  true );      //Human Improved Heavy Engine
    allowComponent(  108,  true );      //Human Heavy Cruise Engine
    allowComponent(  109,  true );      //Human High Output Heavy Engine
    allowComponent(  110,  true );      //Human Agile Heavy Engine
    allowComponent(  111,  true );      //Human Standard Assault Engine
    allowComponent(  112,  true );      //Human Improved Assault Engine
    allowComponent(  113,  true );      //Human heavy turbine engine
    allowComponent(  114,  true );      //High Output Turbine (HOT)
    allowComponent(  115,  true );      //Human super heavy engine
    allowComponent(  128,  true );      //Cybrid Alpha Light Vehicle Engine
    allowComponent(  129,  true );      //Cybrid Beta Light Agility Engine
    allowComponent(  130,  true );      //Cybrid Gamma Standard Medium Engine
    allowComponent(  131,  true );      //Cybrid Delta Medium Cruise Engine
    allowComponent(  132,  true );      //Cybrid Epsilon Improved Medium Engine
    allowComponent(  133,  true );      //Cybrid Zeta Medium High Output Engine
    allowComponent(  134,  true );      //Cybrid Eta Medium Agility Engine
    allowComponent(  135,  true );      //Cybrid Theta Standard Heavy Engine
    allowComponent(  136,  true );      //Cybrid Iota Heavy High Output Engine
    allowComponent(  137,  true );      //Cybrid Kappa Heavy Agility Engine
    allowComponent(  138,  true );      //Cybrid Lamda Standard Assault Engine
    allowComponent(  139,  true );      //Cybrid Mu Improved Assault Engine
    allowComponent(  140,  true );      //Cybrid Nu High Output Assault Engine
    allowComponent(  141,  true );      //Cybrid Xi Heavy Assault Engine
    allowComponent(  142,  true );      //Cybrid Omicron Heavy Assault Turbine
    allowComponent(  143,  true );      //Cybrid Pi Super Heavy Turbine

    //Armor
    allowComponent(  926,  true );      //Carbon Fiber (CARLAM)
    allowComponent(  927,  true );      //Quad Bonded Metaplas (QBM)
    allowComponent(  928,  true );      //DURAC (Depleteted Uranium)
    allowComponent(  929,  true );      //Ceramic
    allowComponent(  930,  true );      //Crystaluminum
    allowComponent(  931,  true );      //Quicksilver

    //Internal Components
    allowComponent(  800,  true );      //Human Basic Computer
    allowComponent(  801,  true );      //Human Improved Computer
    allowComponent(  802,  true );      //Human Advanced Computer
    allowComponent(  805,  true );      //Cybrid Basic Systems Control
    allowComponent(  806,  true );      //Cybrid Enhanced Systems Control
    allowComponent(  807,  true );      //Cybrid Advanced Systems Control
    allowComponent(  810,  true );      //Guardian ECM
    allowComponent(  811,  true );      //Doppleganger ECM
    allowComponent(  812,  true );      //Cybrid Alpha ECM
    allowComponent(  813,  true );      //Cybrid Beta ECM
    allowComponent(  820,  true );      //Thermal Diffuser
    allowComponent(  830,  true );      //Chameleon
    allowComponent(  831,  true );      //Cuttlefish cloak
    allowComponent(  840,  true );      //Shield Modulator
    allowComponent(  845,  true );      //Shield Capacitor
    allowComponent(  850,  true );      //Shield Amplifier (increases shield constant)
    allowComponent(  860,  true );      //Laser Targeting Module
    allowComponent(  865,  true );      //Extra Battery
    allowComponent(  870,  true );      //Reactor Capacitor
    allowComponent(  875,  true );      //Field Stabilizer
    allowComponent(  880,  true );      //Rocket Booster
    allowComponent(  885,  true );      //Turbine Boost
    allowComponent(  890,  true );      //NanoRepair
    allowComponent(  900,  true );      //Angel Life Support
    allowComponent(  910,  true );      //Agrav Generator
    allowComponent(  912,  true );      //ElectroHull
    allowComponent(  914,  true );      //UAP
}

## -------------------------------------------------------------------- Custom Fn

// how long do we wait before letting the friends (AI) know where to find the player?
function dom::waitTime (%player) 
{
    if ($spawnSafetySwitch) 
    {
        return floor($spawnSafetyBase + (player.aiCount / $spawnSafetyMod));
    }
    else return 0.5;
}

// The timer function for testing if a player should still be on yellow team.
function dom::GetKilled()
{
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
            schedule("order(" @ %player.group @ ", attack, " @ %vehicle @ ");", dom::waitTime(%player));
        }
        else if (%player.joinAi > 1)
        {
            %player.joinAi--;
        }
    }
    schedule("dom::getKilled();", 1);
}