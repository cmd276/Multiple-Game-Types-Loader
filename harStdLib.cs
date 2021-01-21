// Ore Harvesting Standard Library
// ][)ull (dull@ss-harvester.com)
// Large portions and original idea by Orogogus
// Other major contributers include Johnrich, Sentinal [M.I.B.], Dolf Kooz,
//  and the rest of the Editing & Scripting Forum (http://community.sierra.com click on "Starsiege" then on "Editing and Scripting")
// Status/Info at http://www.geocities.com/imverydull/Harvester.html or http://www.ss-harvester.com
// 
// ver 1.000 -- 2-23-1
//================================================
// ***Server Host Info***
//
// There are various Logging and anti-team-killing options built in.
//
// $HarvLog -- set to true to log joining players and kicked players to the file "HarvestPlayersLog.cs" 
//          --False disables log, blank defaults to enabled
//
// $BootTKers -- set to true to kick players with $Warnings team kills from the game
//   (meaning define $Warnings to the number of team kills you want them to be kicked out at.)
//
// Team-killers will be killed regardless of if they get booted or not.
//
//================================================
// ***Map Maker Info***
//
// - - - - - - - - - - - - - - - - - - - - - - - - -
// 
// *The .mis file*
//
// Should be (but isn't really) pretty easy to make a map for. 
// Maps need the following:
// -Nav point at the center of each of the team bases
// -Triggers with "OrePatch" as the script class
// -Triggers with "supplyDump" as the script class. ObjectName must be RDump[anything] for Red team, BDump[anything] for Blue team,
//    or anything for both teams.
// -Any triggers/scripts for heal pads, etc required
//
// *Building structures require the following:
//
// -Small Turret (2 blasters): ScriptClass "TurretBuilderL"
// -Large Turret (8 blasters): ScriptClass "TurretBuilderH"
// -Refinery (limit of three): ScriptClass "RefineryBuilder"
// -Base Extension: ScriptClass "BaseExtender"
// -AI Units: 
//    -ScriptClass "AiBuilder"
//    -Object Name of pattern: "[1 letter group] hyphen [4 letter chassis] [anything else or nothing]"
//       -Example: "C-Exec" C for cybrid, choices are (C)ybrid (P)latinumGuard (T)erran (K)night's (R)ebel
//       -Complete list of choices lower in this file.
//
// - - - - - - - - - - - - - - - - - - - - - - - - -
// 
// *The .cs file* 
//
// For full customization, there are a number of possible variables to change.
// For info on customizing look at Harv_Desolate.cs as a template.
//
//================================================
// Edited for inclusion into Scripting library by Wilzuun.
$HarvStdLibVer="StdLib 1.000";
$server::TeamDamage = true;

function InstantMoney(%value)     // Debugging, to be removed before finalization
{
    if(!%value) 
        %value = 100;
    $Ore[*IDSTR_TEAM_RED] = $Ore[*IDSTR_TEAM_RED] + %value;
    $Unallocated[*IDSTR_TEAM_RED] = $Unallocated[*IDSTR_TEAM_RED] + %value;
    $Ore[*IDSTR_TEAM_BLUE] = $Ore[*IDSTR_TEAM_BLUE] + %value;
    $Unallocated[*IDSTR_TEAM_BLUE] = $Unallocated[*IDSTR_TEAM_BLUE] + %value;
}

function Har::setDefaultMissionOptions()
{
    $server::TeamPlay = true;
    $server::AllowDeathmatch = false;
    $server::AllowTeamPlay = true;    
    $server::AllowTeamRed = true;
    $server::AllowTeamBlue = true;
    $server::AllowTeamYellow = false;
    $server::AllowTeamPurple = false;
    $server::disableTeamRed = false;
    $server::disableTeamBlue = false;
    $server::disableTeamYellow = true;
    $server::disableTeamPurple = true;
}

function Har::onMissionStart()
{
    if($BootTKers != true) 
        $Warnings = "";
    if($BootTKers == true && $Warnings == "") 
        $Warnings = 3;
    if(!$NewBaseRadius) 
        $NewBaseRadius = 300;
    $Multiplier[*IDSTR_TEAM_RED] = 1;
    $Ore[*IDSTR_TEAM_RED] = 0;
    $Unallocated[*IDSTR_TEAM_RED] = 0;
    $Multiplier[*IDSTR_TEAM_BLUE] = 1;
    $Ore[*IDSTR_TEAM_BLUE] = 0;
    $Unallocated[*IDSTR_TEAM_BLUE] = 0;
    $Deaths[*IDSTR_TEAM_BLUE] = 0;
    $Deaths[*IDSTR_TEAM_RED] = 0;
    $Kills[*IDSTR_TEAM_BLUE] = 0;
    $Kills[*IDSTR_TEAM_RED] = 0;
    $TargetsLeft[*IDSTR_TEAM_RED] = $Targets[*IDSTR_TEAM_RED];
    $TargetsLeft[*IDSTR_TEAM_BLUE] = $Targets[*IDSTR_TEAM_BLUE];
    // Nuke Script Adapted from Sentinal M.I.B.'s map DM_Nuclear_Fallout (at his suggestion)  :)
    // Nuke Var. Init.
    $Spotter[*IDSTR_TEAM_BLUE] = "Nobody";
    $Spotter[*IDSTR_TEAM_RED] = "Nobody";
    $InitCheck[*IDSTR_TEAM_BLUE] = false;
    $InitCheck[*IDSTR_TEAM_RED] = false;
    $SiloBuilt[*IDSTR_TEAM_BLUE] = false;
    $SiloBuilt[*IDSTR_TEAM_RED] = false;
    $Nuke[*IDSTR_TEAM_BLUE] = "Used";
    $Nuke[*IDSTR_TEAM_RED] = "Used";
    $Target[*IDSTR_TEAM_BLUE] = "Nothing";
    $Target[*IDSTR_TEAM_RED] = "Nothing";
    $TargetSpotted[*IDSTR_TEAM_BLUE] = false;
    $TargetSpotted[*IDSTR_TEAM_RED] = false;
    $DracoLaunched[*IDSTR_TEAM_BLUE] = false;
    $DracoLaunched[*IDSTR_TEAM_RED] = false;
    // End Nuke Var. Init.
    if(!$me::enableMissionEditor) // See rock reson below
    {
        %remove = newObject("remove", SimGroup);
        %removeai = newObject("ai", SimGroup);
        %Red = newObject("Red", SimGroup);
        %Blue = newObject("Blue", SimGroup);
        addToSet("MissionGroup", %remove);
        addToSet(%remove, %removeai);
        addToSet(%removeai, %Red);
        addToSet(%removeai, %Blue);
    }
    setHarvestVariables();
    if($HarvLog == "") 
        $HarvLog = true;
    if($Frosty == true)
        $Planet = "Frosty";
    if($Planet == "Europa")
        $UseThisDTS = "xeuroparock02";
    else if($Planet == "Mars")
        $UseThisDTS = "xMarsrock1";
    else if($Planet == "Snow")
        $UseThisDTS = "xSnowRock1";
    else if($Planet == "Mercury")
        $UseThisDTS = "xmercuryrock01";
    else if($Planet == "Moon")
        $UseThisDTS = "xmoonrock01";
    else if($Planet == "Pluto")
        $UseThisDTS = "xplutorock03";
    else if($Planet == "Titan")
        $UseThisDTS = "xtitanrock02";
    else if($Planet == "Venus")
        $UseThisDTS = "xvenusrock02";
    else if($Planet == "Frosty")
        $UseThisDTS = "xSnowMan";
    else
        $UseThisDTS = "xTree2"; // If undefined planet, use trees... at least it wont crash
    if(!$me::enableMissionEditor) 
    { 
        spawnOrePatches(); 
    }  //So I can edit the map without rocks getting created/moved and then saved
    setHarvAiVar(); // Set up the static Arrays
    for(%i=1; %i <= $BlueMarkers; %i++)
    {
        $BlueBaseX[%i] = getPosition($BlueMarker[%i], x);
        $BlueBaseY[%i] = getPosition($BlueMarker[%i], y);
        if(!$BlueRadius[%i]) 
            $BlueRadius[%i] = 700;
    }
    for(%i=1; %i <= $RedMarkers; %i++)
    {
        $RedBaseX[%i] = getPosition($RedMarker[%i], x);
        $RedBaseY[%i] = getPosition($RedMarker[%i], y);
        if(!$RedRadius[%i]) 
            $RedRadius[%i] = 700;
    }
    HarvOnMissionStart();
}

function Har::player::onAdd(%player)
{
    say(%player, 35, "<F4>Send Harvester comments to dull@ss-harvester.com");
    say(%player, 35, "<F4>Info found at www.SS-Harvester.com");
    say(%player, 35, "<F5>READ THE RULES! This is not a normal game, and important things are explained there!");
    %player.teamKills = 0;
    if(strAlign(1, left, getname(%player))=="/")
    {
        say(0,0, getname(%player)@" joined the game with a slash.");
        say(%player,0,"Slash Detector in Place.");
    }
    if($HarvLog == true)
    {
        %name = getName(%player);
        %IP = getConnection(%player);
        %time = getTime();
        %date = getDate();
        fileWrite("HarvestPlayersLog.cs", append, %name @ " " @ %IP @ " -- " @ %time @ " " @%date@" joined the game.");
    }
    HarvPlayerOnAdd(%player);
}

function Har::player::onRemove(%player)
{
    if(strAlign(1, left, getname(%player))=="/")
    {
        say(0,0,"<f4>"@getname(%player)@" left the game with a slash.");
    }
    if($HarvLog == true)
    {
        %name = getName(%player);
        %IP = getConnection(%player);
        %time = getTime();
        %date = getDate();
        fileWrite("HarvestPlayersLog.cs", append, %player @ " " @ %name @ " " @ %IP @ " -- " @ %time @ " " @%date@" left the game.");
    }
    HarvPlayerOnRemove(%player);
}

function Har::onMissionLoad()         // ==========Rules to be updated (and spell checked) once more scripting is done==========
{
    if($HarvLog == true)
    {
        %time = getTime();
        %date = getDate();
        fileWrite("HarvestPlayersLog.cs", append, "Starting Mission: " @ $missionName @" -- " @ %time @ " " @%date@".");
    }
    cdAudioCycle("Purge", "Newtech", "Cyberntx"); 
    %rules = "<F2>GAMETYPE:\n" @ 
        "<F0>Harvester \n\n" @
        "<tIDMULT_TDM_MAPNAME>"    @ 
        $missionName               @
        "\n\n<F2>RULES/OBJECTIVES:\n"      @
        "<F0>1) Destroy Enemy targets, Red has "@$Targets[*IDSTR_TEAM_RED]@" and Blue has "@$Targets[*IDSTR_TEAM_BLUE]@"\n\n"  @  // May change objectives...
        "<F0>2) To assist in destroying enemies set up turrets, AI gaurds, etc.\n\n"  @
        "<F0>3) Buy such assitacnces by harvesting ore and returning it to your team's supply dump(s)\n\n"  @
        "<F2>Brief Explination:\n" @
        "<F0>Targets are buildings in the enemy base named Red or Blue Target. Harvest ore by shuting down in ore patches." @
        " An ore patch is a group of rocks in close proximity. Take ore back to your team's supply dump to make available " @
        "for use. You can then create things such as turrets, AI gaurds, base expansions, etc. by scanning the " @
        "corresponding pylons. After playing for a while you will (hopefully) be able to understand how it works. " @
        "\nGoodluck, and spend wisely. \n\n   Download available at <F3>www.SS-Harvester.com\n\n" @
        "<F2>VERSION and DATE:\n" @
        "<F0>" @ $MapVersion @ " \n\n" @  
        "<F4>Harvester by dull@ss-harvester.com\n";
    if($MapBy != "") 
        %rules = %rules @ $MapBy;
    if($BootTKers == true)
    { 
        %rules = %rules @ "\n\n<F0>Team-killers will be kicked from the game after " @ $Warnings @ " offences. Every team-kill will result in death.\n"; 
    }
    if($AditionalRules) 
        %rules = %rules @ $AdditionalRules;
    %rules = %rules @ "<IDMULT_STD_TELEPORTER>";
    setGameInfo(%rules); 
    HarvOnMissionLoad();
}

function Har::onMissionEnd()
{
    deleteObject("MissionGroup\\remove");
    flushConsoleScheduler();
    notifyMissionEnd();
}

function Har::vehicle::onAdd(%vehicle)
{
    %player = playerManager::vehicleIdToPlayerNum(%vehicle);
    %vehicle.oreCarried = 0;
    %vehicle.oreCapacity = getVehicleCapacity(%vehicle);
    %vehicle.harvestRate = 2;
    %vehicle.inPatch = 0;
    %vehicle.readyToMine = 0;
    %vehicle.inDump = 0;
    %vehicle.readyToDump = 0;
    %vehicle.dumpRate = 4;
    %vehicle.cost = checkValue(%vehicle);
    %vehicle.guards = 0;
    setHUDTimer(0, -1, "0", 3, %player);
    %team = getTeam(%vehicle);
    if($Nuke[%team] == "Building")
    {
        %time = getCurrentTime();
        %elapsed = %time - ($StartNukeTime[%team] - 180);
        setHUDTimer(%elapsed, -1, "Building Nuke...", 3, %player);
    }
    %vehicleName = getVehicleName(%vehicle);
    if(%player != 0 && %player.lastVehicle != %vehicleName)
    {
        say(%player, 35, "You are dropping in a " @ getVehicleName(%vehicle) @ ". Your maximum ore capacity is " @ %vehicle.oreCapacity @ " units of ore.");
        %player.lastVehicle = %vehicleName;
    }
    %team = getTeam(%vehicle);
    HarvVehicleOnAdd(%vehicle);
} 

function Har::vehicle::onDestroyed(%destroyed, %destroyer)
{
    %time = getTime();
    %team = getTeam(%destroyed);
    if(%destroyed==$Draco[%team])
    {
        schedule("$DracoLaunched[\""@%team@"\"] = false;",3);
        schedule("deleteObject($Draco[\""@%team@"\"]);",3);
    }
    if(%time != $LastNukeTime)
    { 
        %message = getFancyDeathMessage(getHUDName(%destroyed), getHUDName(%destroyer)); 
    }
    else
    {
        %name = getHUDName(%destroyed);
        %message = %name@" got caught in the nuclear blast!";
    }
    if(%message != "")
    {
        say( 0, 0, %message);
    }
    %destroyed.dead = "true";
    %marker = getVehicleNavMarkerId(%destroyed);
    %marker.dead = "true";
    %team = getTeam(%destroyer);
    %deadTeam = getTeam(%destroyed);
    %player = playerManager::vehicleIdToPlayerNum(%destroyer);
    %deadPlayer = playerManager::VehicleIdToPlayerNum(%destroyed);
    if(%deadPlayer == 0)
    {
        %destroyed.time = 0;
        schedule("deleteObject(" @ %destroyed @ ");", 10);
        if(getVehicleNavMarkerId(%destroyed.guarding))
            shuffleGuards(%destroyed.guarding, %destroyed.whichGuard);
    }
    else if(%destroyed.Guards > 0)
    {
        for(%i=1; %i<=%destroyed.Guards; %i++)
        {
            %time = getTime();
            %j = %destroyed.Guard[%i];
            %j.guarding = "home";
            %j.ingPoint = true;
            %j.time = %time;
            guardUpdater(%destroyed.Guard[%i], %time, "home");
        }
    }
    $Deaths[%deadTeam]++;
    if(%team != %deadTeam)
    {
        $Kills[%team]++;
    }
    else // Anti-team kill stuff
    {
        if(%destroyed != %destroyer)
        {
            resetLastHitBy(%destroyer);
            if(%player != 0)
            {
                %player.teamKills++;
                TKer(%player);
            }
        }
    }
    // Put ore (s)he was carrying back in the ore field
    %trigger = %destroyed.inPatch;
    if(%trigger != 0)
    {
        %howMuch = %vehicle.cost / 1000; 
        %howmuch = round(%howMuch);
        if(%destroyed.oreCarried > 0)
            %howMuch = %howMuch + %destroyed.oreCarried;
        %trigger.oreRemaining = %trigger.oreRemaining + %howMuch;
    }
    else
    { 
        scrap(%destroyed); 
    }
    HarvVehicleOnDestroyed(%destroyed, %destroyer);
}

function Har::vehicle::onScan(%scanned, %scanner)
{
    %player = playerManager::vehicleIdToPlayerNum(%scanner);
    %edPlayer = playerManager::vehicleIdToPlayerNum(%scanned);
    %team = getTeam(%player);
    %edTeam = getTeam(%scanned);
    %marker = getVehicleNavMarkerId(%scanner);
    if(%team != %edTeam) //================ Was the other team scanned? Then attack...
    {
        if(%scanner.Guards > 0)
        {
            for(%i=1; %i<=%scanner.guards; %i++)
            {
                %temp = %scanner.Guard[%i];
                order(%temp, attack, %scanned);
                %temp.guarding = %scanner;
                %temp.ingPoint = false;
                %time = getTime();
                %scanned.time = %time;
                schedule("guardUpdater("@%vehicle.Guard[%i]@",  \"" @ %time @ "\", " @ %scanner @ " );", 10);
            }
        }
    }
    else if(%edPlayer == 0)   //=============== AI Instructions
    {
        if(%scanned.guarding == %scanner)
        {
            %scanned.guarding = %marker; 
            %scanned.ingPoint = true;
            shuffleGuards(%scanner, %scanned.whichGuard);
            %scanned.whichGuard = "";
            say(%player, 35, "<F4>Ai guarding your marker. Scan to order it to guard your vehicle.");
            %time = getTime();
            %scanned.time = %time;
            %x = getPosition(%marker, x);
            %y = getPosition(%marker, y);
            %z = getTerrainHeight(%x, %y);
            GuardUpdater(%scanned, %time, %x, %y, %z);
            return;
        }
        else
        {
            if(getVehicleNavMarkerId(%scanner.guarding))
            {
                %old = %scanned.guarding;
                shuffleGuards(%old, %scanned.whichGuard);
            }
            order(%scanned, guard, %scanner); echo(%scanned, " GV ", %scanner);
            %scanned.guarding = %scanner;
            %scanned.ingPoint = false;
            %i = %scanner.guards + 1;
            %scanner.Guard[%i] = %scanned;  // Lots of cross-referencing
            %scanned.whichGuard = %i;
            %scanner.guards++;
            say(%player, 35, "<F4>Ai guarding your vehicle. Scan to order it to guard your marker.");
            %time = getTime();
            %scanned.time = %time;
            schedule("guardUpdater(" @ %scanned @ ",  \"" @ %time @ "\", " @ %scanner @ ");", 10);
            return;
        }
    }
}

function structure::onScan(%building, %vehicle)
{
    %team = getTeam(%vehicle);
    %edTeam = getTeam(%building);
    %player = playerManager::vehicleIdToPlayerNum(%vehicle);
    if(%team != %edTeam) //================ Was the other team scanned? Then attack...
    {
        if(%scanner.Guards > 0)
        {
            for(%i=1; %i<=%scanner.guards; %i++)
            {
                %temp = %scanner.Guard[%i];
                order(%temp, attack, %building);
                %temp.guarding = %scanner;
                %temp.ingPoint = false;
                %time = getTime();
                %scanned.time = %time;
                schedule("guardUpdater("@%vehicle.Guard[%i]@",  \"" @ %time @ "\", " @ %scanner @ " );", 10);
            }
        }
        return;
    }
}

function nukeBuilder::structure::onScan(%silo, %veh)
{
    %team = getTeam(%veh);
    %edTeam = getTeam(%silo);
    %player = playerManager::vehicleIdToPlayerNum(%veh);
    if(%team != %edTeam)
    {
        say(%player, %player, "<F5>This is not your team's nuclear silo.");
        return;
    }
    if($Unallocated[%team] < 200)
    {
        say(%player, 35, "<F5>Not enough ore.");
        return;
    }
    if($Nuke[%team] == "Used")
    {
        $Nuke[%team] = "Building";
        teamTransmissions("<F5>Production of "@%team@"'s nuke will now begin.", %team);
        $Unallocated[%team] = $Unallocated[%team] - 200;
        schedule("buildNuke(\""@%team@"\");",3);
    }
    else if($Nuke[%team]=="Building")
    {
        say(%player, %player, "<F5>"@%team@"'s nuke is already being built.");
    }
    else if($Nuke[%team]=="Ready")
    {
        say(%player, %player, "<F5>"@%team@" already has a nuke built & ready to use.");
    }
}

function buildNuke(%team)
{
    teamTransmissions("<F5>The nuke will be ready in 3 minutes...", %team);
    $Ore[%team] = $Ore[%team] - 200;
    $StartNukeTime[%team] = getCurrentTime();
    teamHUDTimer(180, -1, "Building Nuke", 3, %team);
    schedule("nukeReady(\""@%team@"\");",180);
}

function teamHUDTimer(%time, %dir, %head, %which, %team)
{
    %count = playerManager::getPlayerCount();
    for(%i = 0; %i < %count; %i++)
    {
        %player = playerManager::getPlayerNum(%i);
        if(getTeam(%player)==%team)
        {
            setHUDTimer(%time, %dir, %head, %which, %player);
        }
    }
}

function cancelTeamHUDTimer(%which, %team)
{
    %count = playerManager::getPlayerCount();
    for(%i = 0; %i < %count; %i++)
    {
        %player = playerManager::getPlayerNum(%i);
        if(getTeam(%player)==%team)
        {
            setHUDTimer(-1, 0, "", %which, %player);
        }
    }
}

function nukeReady(%team)
{
    if($CancelNuke[%team] == true)
    {
        $CancelNuke[%team] = false;
        return;
    }
    $Nuke[%team] = "Ready";
    if(%team == *IDSTR_TEAM_BLUE) 
        %otherTeam = *IDSTR_TEAM_RED;
    if(%team == *IDSTR_TEAM_RED) 
        %otherTeam = *IDSTR_TEAM_BLUE;
    teamTransmissions("<F5>"@%team@"'s nuke is ready to be used.", %team);
    teamTransmissions("<F2>TAC-COM: Warning! Satellites have detected that "@%team@" has produced a nuclear weapon.", %otherTeam, "lock_warn.wav", 3);
    schedule("teamTransmissions(\"<F5>Use LTADS to spot for the Draco bomber air strike.\", \""@%team@"\");",3);
}

function Har::vehicle::onSpot(%spotter, %target)
   {
    %player = playerManager::vehicleIdToPlayerNum(%spotter);
    %team = getTeam(%spotter);
    %edTeam = getTeam(%target);
    if(%team == *IDSTR_TEAM_RED) 
        %otherTeam = *IDSTR_TEAM_BLUE;
    if(%team == *IDSTR_TEAM_BLUE) 
        %otherTeam = *IDSTR_TEAM_RED;
    if(%edTeam != %team)
    {
        if($Nuke[%team] == "Ready")
        {
            if(%target != "")
            { 
                if($Spotter[%team] == "Nobody" || $Spotter[%team] == %spotter)
                {
                    $Spotter[%team] = %spotter;
                    $Target[%team] = %target;
                    $TargetSpotted[%team] = true;
                    teamTransmissions("<F5>TAC-COM: Roger that " @ getHudName(%spotter) @ ", the Draco bomber has your position and is on its way." ,%team);
                    if($DracoLaunched[%team] == false)
                    {
                        launchDraco(%spotter, %target, %team);
                    }
                    else
                    {
                        order($Draco[%team], clear);
                        order($Draco[%team], guard, $Waypoints[%team]);
                        %time = 5;
                        %cycle = 1;
                        schedule("updatePosition(" @ %target @ ", " @ %time @ ", " @ %cycle @ ", " @ %team @ ");", 5);
                    }
                }
                else if($Spotter[%team] != %spotter)
                {
                    teamTransmissions("<F5>DRACO: Negative " @ getHudName(%spotter) @ ", I have already aquired " @ getHudName($Spotter[%team]) @ "'s target." , %team);
                }
            }
            else if(%target=="")
            {
                if($Spotter[%team] == %spotter)
                {
                    $Spotter[%team] = "Nobody";
                    $TargetSpotted[%team] = false;
                    teamTransmissions( "<F5>DRACO: Tac-Com, I've lost the target. I need someone to spot it for me.", %team);
                    order($Draco[%team], guard, $Waypoint2[%otherTeam]);
                    if($InitCheck[%team] == false)
                    {
                        $InitCheck[%team] = true;
                        schedule("checkSpotDraco(\""@%team@"\");",15);
                    }
                }
            }
        }
        else if(($Nuke[%team] == "Building")&&(%target!=""))
        {
            say(%player, %player, "<F5>Your team's nuke is not ready yet.");
        }
        else if(($redNuke=="Used")&&(%target!=""))
        {
            say(%player, %player, "<F5>Your team does not have a nuke built yet.");
        }
    }
    else if(%edTeam == %team)
    {
        say(%player, %player, "<F5>You cannot spot your own team!");
    }
}

function updatePosition(%target, %time, %cycle, %team)
{
    %x = getPosition(%target, x);
    %y = getPosition(%target, y);
    %z = getPosition(%target, z) + 200;
    if(%cycle == 1)
    {
        %cycle = 2;
    }
    else if(%cycle == 2)
    {
        %time = %time + 5;
        if(%time <= 10)
        {
            %cycle = 1;
        }
    }
    if(($TargetSpotted[%team] == true)&&($DracoLaunched[%team] == true)&&($Nuke[%team] == "Ready"))
    {
        setPosition($Waypoint[%team], %x, %y, %z);
        order($Draco[%team], guard, $Waypoints[%team]);
        schedule("updatePosition(" @ %target @ ", " @ %time @ ", " @ %cycle @ ", " @ %team @ ");", %time);
        echo("Updating Target Coordinates: " @ %team @ ", " @ %target @ ", " @ %time);
    }
}

function launchDraco(%spotter, %target, %team)
{
    $DracoLaunched[%team] = true;
    %x = getPosition(%target, x);
    %y = getPosition(%target, y);
    %z = getPosition(%target, z) + 350;
    $Draco[%team] = newObject("Draco", flyer, 131);
    addToSet("Missiongroup/remove", $Draco[%team]);
    setTeam($Draco[%team], %team);
    setPosition($Draco[%team], $DracoX[%team], $DracoY[%team], $DracoZ[%team]);
    //setShapeVisibility($Waypoint[%team], false);
    setPosition($Waypoint[%team], %x, %y, %z);
    order($Draco[%team], speed, high);
    order($Draco[%team], guard, $Waypoints[%team]);
    %time = 5;
    %cycle = 1;
    schedule("updatePosition(" @ %target @ ", " @ %time @ ", " @ %cycle @ ", " @ %team @ ");", 5);
    echo("Updating Target Coordinates: " @ %team @ ", " @ %target @ ", " @ %time);
}

function Draco::vehicle::onArrived(%this, %where)
{
    %team = getTeam(%this);
    if(%where==$Waypoint[%team])
    {  
        say("Everybody", 1,"<F5>DRACO: Payload Released.");
        dropBomb(%team);
    }
    else if(%where==$Waypoint2[%team])
    {  
        schedule("$DracoLaunched[\""@%team@"\"] = false;",3);
        schedule("deleteObject("@%this@");",3);
    }
}

function dropBomb(%team)
{
    $Nuke[%team] = "Used";
    %x = getPosition($Waypoint[%team], x);
    %y = getPosition($Waypoint[%team], y);
    %z = getPosition($Waypoint[%team], z);
    %bomb = $Bomb[%team];
    setPosition(%bomb, %x, %y, %z);
    schedule("fall("@%bomb@" , 40);",0.1);
}

function fall(%bomb, %frame)
{
    if(%frame<1)
    {
        %team = getTeam(%bomb);
        %target = $Target[%team];
        nukeTarget(%target, %bomb);
    }
    else
    {
        %frame--;
        %x = getPosition(%bomb, x);
        %y = getPosition(%bomb, y);
        %z = getPosition(%bomb, z) - 5;
        setPosition(%bomb, %x, %y, %z, 0, -90);
        schedule("fall(" @ %bomb @ ", " @ %frame @ ");",0.1);
    }
}

function nukeTarget(%target, %bomb, %quiet)
{
    %team = getTeam(%bomb);
    %x = getPosition(%target, x);
    %y = getPosition(%target, y);
    %z = getTerrainHeight(%x, %y);
    %random = randomInt(0,2);
    setPosition($Explosion[%team], %x, %y, %z);
    setPosition($Shockwave[%team], %x, %y, (%z + 5));
    playAnimSequence($Explosion[%team], 0, true);
    playAnimSequence($Shockwave[%team], 0, true);
    %time = getTime();
    $LastNukeTime = %time;
    damageArea(%bomb, 0, 0, 0, 350, 1000000);
    if(getVehicleName(%target) == false)
        damageObject(%target, 50000);
    if(%quiet != true)
    {
        say("Everybody", 1, "<F5>"@%team@"'s nuke has detonated!", "sfx_fog.wav");
        if(%random==0)
        {
            schedule("say(\"Everybody\", 1, \"<F5>DRACO: Yeah! Boom-baby-boom! hahah.\", \"M6_Saxon_yeahboom.WAV\");",3);
        }
        else if(%random==1)
        {
            schedule("say(\"Everybody\", 1, \"<F5>DRACO: Target eliminated.\", \"M9_Delta6_targetel.WAV\");",3);
        }
        else if(%random==2)
        {
            schedule("say(\"Everybody\", 1, \"<F5>DRACO: Target destroyed.\", \"M9_Delta6_targetd.WAV\");",3);
        }
    }
    $Spotter[%team] = "Nobody";
    $TargetSpotted[%team] = false;
    schedule("setPosition($Explosion[\""@%team@"\"] , 5000, 5000, -1000);",1.067);
    schedule("setPosition($Shockwave[\""@%team@"\"], 5000, 5000, -1000);",1.667);
    schedule("playAnimSequence($Explosion[\""@%team@"\"], 0, false);",5);
    schedule("playAnimSequence($Shockwave[\""@%team@"\"], 0, false);",5);
    setPosition(%bomb, 5000, 5000, -1000);
}

function checkSpotDraco(%team)
{
    if($TargetSpotted[%team] == false)
    {
        schedule("$DracoLaunched[\""@%team@"\"] = false;",3);
        schedule("deleteObject($Draco[\""@%team@"\"]);",3);
        schedule("$Draco[\""@%team@"\"] = \"\";",3);
    }
    $InitCheck[%team] = false;
}
//MODIFIED BY DUN_STARSCAPER
//
function teamTransmissions(%text, %team, %wav, %plays)
//{
//%plays--;
//%count = playerManager::getPlayerCount();
//
//for(%i = 0; %i < %count; %i++)
// {
// %player = playerManager::getPlayerNum(%i);
// if(getTeam(%player)==%team)
//  {
//  say(%player, %player, %text, %wav);
//  for(%a = 1; %a<=%plays; %a++)
//   {say(%player,%player,%wav);}
//  }
// }
//}

function teamTransmissions(%text, %team, %wav, %plays)
{
    //More efficient to let the engine do the work of finding players -- dun
    if(%team==*IDSTR_TEAM_RED) 
        %teamID = IDSTR_TEAM_RED;
    if(%team==*IDSTR_TEAM_BLUE) 
        %teamID = IDSTR_TEAM_BLUE;
    else 
        %teamID = 0;
    say(%teamID, %teamID, %text, %wav);
    for(%a = 1; %a<%plays; %a++)
        say(%teamID,%teamID,%wav);
}

function structure::onDestroyed(%building, %vehicle)
{
    schedule("deleteObject(" @ %building @ ");", 30);
    %time = getTime();
    if($isType[%building] == "scrap")
    {
        Scrap::structure::onDestroyed(%building, %vehicle);
        return;
    }
    if($isType[%building] == "ore")
    {
        rock::structure::onDestroyed(%building, %vehicle);
        return;
    }
    if($isType[%building] == "refinery")
    {
        %deadTeam = getTeam(%building);
        $Multiplier[%deadteam]--;
        %name = getName(%vehicle);
        say(0, 35, %name @ " destroyed a refinery belonging to " @ %deadteam @ ".");
        %team = getTeam(%vehicle);
        %player = playerManager::vehicleIdToPlayerNum(%vehicle);
        if(%team == %deadTeam)
        {
            %player.teamKills++;
            TKer(%player);
        }
        return;
    }
    if($isType[%building] == "silo")
    {
        %team = getTeam(%building);
        $SiloBuilt[%team] = false;
        setShapeVisibility($Silo[%team], false);
        %name = getName(%vehicle);
        %time = getTime();
        if(%time != $LastNukeTime)
        {
            say(0, 35, %name @ " destroyed " @ %team @"'s nuclear silo.");
            %deadTeam = getTeam(%building);
            %team = getTeam(%vehicle);
            %player = playerManager::vehicleIdToPlayerNum(%vehicle);
            if(%team == %deadTeam)
            {
                %player.teamKills++;
                TKer(%player);
            }
            if($Nuke[%deadTeam] == "Ready") 
                destroyNuke(%deadTeam, %building);
        }
        else
        {
            say(0,0,%team@"'s silo got destroyed in the nuclear blast!");
        }
        if($Nuke[%deadTeam] == "Building") 
            cancelNuke(%deadTeam);
        return;
    }
    if(getHUDName(%building) == "false") 
        return;
    if(%time != $LastNukeTime)
    { 
        say(0,0, getFancyDeathMessage(getHUDName(%building), getHUDName(%vehicle)) ); 
    }
    else
    {
        %name = getHUDName(%building);
        say(0,0, %name@" got caught in the nuclear blast!");
    }
}

function Har::vehicle::onMessage(%a, %b, %c, %d, %e, %f, %g, %h)
{
    if(%b == "TargetDestroyed")
    {
        if($isType[%c] == "Turret")
        {
            if(getTeam(%c) != getTeam(%a))
            {
                if(%c.sent != true)
                {
                    %c.sent = true;
                    turret::onDestroyed(%c, %a);
                    HarvTurretOnDestroyed(%c, %a);
                }
            }
        }
    }
    HarvVehicleOnMessage(%a, %b, %c, %d, %e, %f, %g, %h);
}

function turret::onDestroyed(%building, %vehicle)
{
    %time = getTime();
    if(%time != $LastNukeTime)
    { 
        %message = getFancyDeathMessage(getHUDName(%building), getHUDName(%vehicle)); 
    }
    else
    {
        %name = getHUDName(%building);
        %message = %name@" got caught in the nuclear blast!";
    }
    if(%message != "")
    {
        say( 0, 0, %message);
    }
    %team = getTeam(%vehicle);
    %deadTeam = getTeam(%building);
    $Kills[%team]++;
    $Deaths[%deadTeam]++;
}

function target::structure::onDestroyed(%building, %vehicle)
{
    %time = getTime();
    %team = getTeam(%vehicle);
    %deadTeam = getTeam(%building);
    $TargetsLeft[%deadTeam]--;
    if(%time != $LastNukeTime)
    {
        %message = getFancyDeathMessage(getHUDName(%building), getHUDName(%vehicle));
        if(%message != "") 
            say(0,0,%message);
        say(0,0,getHUDName(%vehicle)@" destroyed a "@%deadTeam@" target building! "@$TargetsLeft[%deadTeam]@" targets remaining!");
    }
    else
    {
        %name = getHUDName(%building);
        %message = %name@" got caught in the nuclear blast!";
        say( 0, 0, %message);
    }
    if(%team == %deadTeam)
    {
        %player.teamKills++;
        TKer(%player);
    } 
    if(%team != %deadTeam)
    {
        $Kills[%team]++;
    }
    $Deaths[%deadTeam]++;
    if($TargetsLeft[%deadTeam] <= 0)
    {
        if(%deadTeam == *IDSTR_TEAM_RED)
        {
            %winner = *IDSTR_TEAM_BLUE;
            fadeEvent(0, Out, 2, 0, 0.4784, 0.8471);
            schedule("fadeEvent(0, In, 2, 0, 0.4784, 0.8471);", 2);
        }
        if(%deadTeam == *IDSTR_TEAM_BLUE) 
        {
            %winner = *IDSTR_TEAM_RED;
            fadeEvent(0, Out, 2, 1, 0.3725, 0.2471);
            schedule("fadeEvent(0, In, 2, 1, 0.3725, 0.2471);", 2);
        }
        say(0,0,"All of "@%deadTeam@"'s targets have been destroyed! "@%winner@" wins!");
        schedule("MissionEndConditionMet();", 3);
    }
}

function cancelNuke(%team)
{
    $Nuke[%team] = "Used";
    $CancelNuke[%team] = true;
    %time = getCurrentTime();
    %elapsed = %time - $StartNukeTime[%team];
    $StartNukeTime[%team] = "";
    cancelTeamHUDTimer(3, %team);
    %oreLeft = 200 - ((10/9) * %elapsed);
    %oreLeft = round(%oreLeft);
    $Ore[%team] = $Ore[%team] + %oreLeft;
    $Unallocated[%team] = $Unallocated[%team] + %oreLeft;
}

function destroyNuke(%team, %building)
{
    $Nuke[%team] = "Used";
    if($DracoLaunched[%team] == true) 
        return;
    %rand = randomInt(0, 3);
    if(%rand > 0)
    {
        say(0,0,%team@"'s silo got destroyed in the nuclear blast!");
        return;
    }
    %x = getPosition(%building, x);
    %y = getPosition(%building, y);
    %z = getPosition(%building, z);
    setPosition($Bomb[%team], %x, %y, %z, 0, 90);
    nukeTarget(%building, $Bomb[%team], true);
    say(0,0,"<F5>"@%team@"'s nuke detonated when their silo was destroyed!", "sfx_fog.wav");
}
//======================  Anti-team-killer 
function, and in the Dull style: too many options. See top for list.

function Tker(%player)
{
    if(%player == 0) 
        return;
    if(%player.teamKills < $Warnings || $Warnings == "")
    {
        %vehicle = playerManager::PlayerNumToVehicleId(%player);
        damageObject(%vehicle, 10000000);
        say(%player, %player, "", "cy_rules_violated.wav");
        if($BookTKers == true)
            messageBox(%player, "You have killed a friendly. Continued team-killing will result in being removed from the game. Offences: " @ %player.teamKills );
        else
            messageBox(%player, "You have killed a friendly, don't do it again.");
        return;
    }
    %name = getName(%player);
    if($HarvLog == true)
    {
        %IP = getConnection(%player);
        %time = getTime();
        %date = getDate();
        fileWrite("HarvestPlayersLog.cs", append, %player @ " " @ %name @ " " @ %IP @ " -- " @ %time @ " " @%date@" KICKED for team-killing with "@%player.teamKills@" offences.");
    }
    if($BootTKers != true) 
        return;
    say(%player, 0, "rules_violated.wav");
    messageBox(%player, "You have killed team-mates too often. Come back when you have a better attitude.");
    say(0, 35, "Kicking " @ %name @ " for too many team-kills.");
    kick(%player);
}
//=============== Scrap metal (from dead vehicles) harvesting

function scrap(%vehicle)
{
    %x = getPosition(%vehicle, x);
    %y = getPosition(%vehicle, y);
    %zt = getTerrainHeight(%x, %y);
    %debries = newObject("Scrap", StaticShape, "DEBRIS_LRG_1.DTS");
    %rand1 = randomInt(0, 360);
    %rand2 = randomInt(0, 360);
    setPosition(%debries, %x, %y, %zt, %rand1, %rand2);
    setVehicleRadarVisible(%debries, true);   // Test
    addToSet("MissionGroup\\remove", %debries);
    %howMuch = %vehicle.cost / 1000; 
    %howmuch = round(%howMuch);
    if(%vehicle.oreCarried > 0)
    { 
        %howMuch = %howMuch + %vehicle.oreCarried; 
    }
    %debries.ore = %howMuch;
    $isType[%debries] = "scrap";
    echo(%debries, ", ", %debries.ore, " <= salvage at ", %x, " ", %y, " ", %zt);
}

function round(%value)
{
    %negative = false;
    %inverse = %value * -1;
    if(%inverse > %value)
    {
        %value = %inverse;
        %negative = true;
    }
    %count = 0;
    for(%i = %value; %i > 0; %i--)
    {
        %count++; 
    }
    if(%i < -0.5)
    {
        %count--;
    }
    if(%negative == true)
    {
        %count = %count * -1;
    }
    return %count;
}

function Scrap::structure::onAttacked(%debries, %vehicle)
{
    damageObject(%debries, -10000);
    %dist = getDistance(%debries, %vehicle);
    %player = playerManager::vehicleIdToPlayerNum(%vehicle);
    if(%dist > 300)
    {
        if(%player != 0)
        {
            say(%player, 35, "<F4>Too far away to salvage. Must be within 300 meters."); 
        }
        return;
    }
    %roomLeft = %vehicle.oreCapacity - %vehicle.oreCarried;
    if(%debries.ore >= %roomLeft && %roomLeft > 0)
    {
        %vehicle.oreCarried = %vehicle.oreCapacity;
        %debries.ore = %debries.ore - %roomLeft;
        if(%player != 0)
            say(%player, 35, "<F4>Salvaging " @ %roomLeft @ " units of ore. Ore capacity reached.");
    }
    else if(%roomLeft > %deries.ore)
    {
        %vehicle.oreCarried = %vehicle.oreCarried + %debries.ore;
        if(%player != 0)
            say(%player, 35, "<F4>Salvaging " @ %debries.ore @ " units of ore. " @ %vehicle.oreCarried @ " units of ore in hold.");
        %debries.ore = 0;
    }
    else if(%roomLeft <= 0)
    {
        if(%player != 0)
            say(%player, 35, "Maximum ore capacity has already been reached. Return to your team's supply dump to unload it.");
    }
    if(%debries.ore <= 0)
    {
        schedule("deleteObject(" @ %debries @ ");", 0);
        if(%player != 0)
            say(%player, 35, "<F4>All ore salvaged.");
    }
    else
    {
        if(%player != 0)
            say(%player, 35, "<F4>" @ %debries.ore @ " units of ore remaining.");
    }
}

function Scrap::structure::onDestroyed(%scrap, %vehicle)
{
    if(%scrap.ore <= O) 
    { 
        echo(%scrap, ": dead empty scrap"); return; 
    }
    %x = getPosition(%scrap, x);
    %y = getPosition(%scrap, y);
    %z = getPosition(%scrap, z);
    %debries = newObject("Scrap", StaticShape, "DEBRIS_LRG_1.DTS");
    %rand1 = randomInt(0, 360);
    %rand2 = randomInt(0, 360);
    setPosition(%debries, %x, %y, %z, %rand1, %rand2);
    setVehicleRadarVisible(%debries, true);
    addToSet("MissionGroup\\remove", %debries);
    %debries.ore = %scrap.ore;
    $isType[%debries] = "scrap";
    echo(%debries @ " replacing " @ %scrap @ ", " @ %debries.ore @ " <= salvage at " @ %x @ " " @ %y @ " " @ %z);
}

function structure::onAttacked(%attacked, %attacker)
{
    if($isType[%attacked] == "scrap")
        Scrap::structure::onAttacked(%attacked, %attacker);
}
//===============  Ore patch creation (Orogogus', edited for multiple zones for patches to spawn in)

function spawnOrePatches()
{
    setUpOreArea();
    for(%i=1; %i <= $orePatches; %i++)
    {
        %rand = randomInt(1, 100);    // Choose one of the available zones (% by area)
        %which = $Position[%rand];
        %x = randomInt($Xlow[%which], $Xhigh[%which]);
        %y = randomInt($Ylow[%which], $Yhigh[%which]);
        %z = getTerrainHeight(%x, %y);
        echo("Spawn Patch "@ %i @" -- " @ %x @" "@ %y @" ("@%rand@": Zone "@%which@")");
        setPosition($orePatch[%i], %x, %y, %z);
        $orePatch[%i].number = %i;
        for(%j=1; %j <= $orePatch[%i].maxRocks; %j++)
        {
            %dist = $orePatch[%i].size / 2;
            %x1 = %x - %dist;
            %x2 = %x + %dist;
            %y1 = %y - %dist;
            %y2 = %y + %dist;
            %rx = randomInt(%x1, %x2);
            %ry = randomInt(%y1, %y2);
            %rz = getTerrainHeight(%rx, %ry);
            %rRotZ = randomInt( 0, 360);
            if($Planet != "Frosty")
            %rRotX = randomInt( 0, 360);
            %newRock = newObject($UseThisDTS, StaticShape, $UseThisDTS @ ".dts");
            addToSet("MissionGroup\\remove", %newrock);
            setPosition( %newRock, %rx, %ry, %rz, %rRotZ, %rRotX);
            echo("Rock "@ %j @ ": " @ %newRock @" "@ %rx @" "@ %ry);
            $orePatch[%i].rock[%j] = %newRock;
            $isType[%newRock] = "ore";
            $wasRock[%newRock] = %j;
            $wasPatch[%newRock] = %i;
        }
        $orePatch[%i].oreRemaining = $orePatch[%i].maxOre;
        $orePatch[%i].rocksRemaining = $orePatch[%i].maxRocks;
        $orePatch[%i].orePerRock = $orePatch[%i].maxOre / $orePatch[%i].maxRocks;
    }
}

function updateRocks(%trigger)
{
    %rockCount = %trigger.oreRemaining / %trigger.orePerRock;
    if((%trigger.rocksRemaining - %rockCount) >= 1)
    {
        deleteObject(%trigger.rock[%trigger.rocksRemaining]);
        %trigger.rocksRemaining--;
    }
}

function setUpOreArea()
{
    echo("===Calculating Patch Spawn Frequency Rates===");
    %howmany = $OreZones;
    %total = 0;
    for(%i=1; %i<=%howmany; %i++)
    {
        if($Xlow[%i] > $Xhigh[%i])
        {
            echo("Reversing X ", %i, "'s high and low");
            %temp = $Xlow[%i];
            $Xlow[%i] = $Xhigh[%i];
            $Xhigh[%i] = %temp;
        }
        if($Ylow[%i] > $Yhigh[%i])
        {
            echo("Reversing Y ", %i, "'s high and low");
            %temp = $Ylow[%i];
            $Ylow[%i] = $Yhigh[%i];
            $Yhigh[%i] = %temp;
        }
        %x = $Xhigh[%i] - $Xlow[%i];
        %y = $Yhigh[%i] - $Ylow[%i];
        %area[%i] = %x * %y;
        %total = %total + %area[%i];
        echo(%i, ": A:", %area[%i], " C:", %x, " ", %y, " T:", %total);
    }
    %start = 1;
    for(%i=1; %i<=%howmany; %i++)
    {
        %freq = %area[%i] / %total;
        %freq = %freq * 100;
        %freq = round(%freq);
        %stop = %start + %freq - 1;
        for(%j=%start; %j<=%stop; %j++)
        { 
            $Position[%j] = %i;
        }
        echo(%i, ": ", %freq, "% -- ", %start, "-", %stop);
        %start = %stop + 1;
    }
    echo("============Calculations Complete============");
}

function rock::structure::onDestroyed(%rock, %vehicle)
{
    %i = $wasPatch[%rock];
    %j = $wasRock[%rock];
    %rx = getPosition(%rock, x);
    %ry = getPosition(%rock, y);
    //%rz = getPosition(%rock, z);
    %rrotz = getPosition(%rock, rot);
    // %dist = $orePatch[%i].size / 2;
    //  %x1 = %x - %dist;
    //  %x2 = %x + %dist;
    //  %y1 = %y - %dist;
    //  %y2 = %y + %dist;
    //  %rx = randomInt(%x1, %x2);
    //  %ry = randomInt(%y1, %y2);
    %rz = getTerrainHeight(%rx, %ry);
    //  %rRotZ = randomInt( 0, 360);
    if($Planet != "Frosty")
        %rRotX = randomInt( 0, 360);
    %newRock = newObject($UseThisDTS, StaticShape, $UseThisDTS @ ".dts");
    addToSet("MissionGroup\\remove", %newrock);
    setPosition( %newRock, %rx, %ry, %rz, %rRotZ, %rRotX);
    echo("Rock replacement "@ %j @ ": " @ %newRock @" "@ %rx @" "@ %ry);
    $orePatch[%i].rock[%j] = %newRock;
    $wasPatch[%newRock] = %i;
    $wasRock[%newRock] = %j;
    $isType[%newRock] = "ore";
}
//=============== Respawn OrePatches (Copy/Edit of my edit of Orogogus' SpawnPatches)

function respawnOrePatch(%i)
{
    for(%j=1; %j <= $orePatch[%i].maxrocks; %j++)
    {
        deleteObject($orePatch[%i].rock[%j]);   // Should already be gone, but...
    }
    %rand = randomInt(1, 100);
    %which = $Position[%rand];
    %x = randomInt($Xlow[%which], $Xhigh[%which]);
    %y = randomInt($Ylow[%which], $Yhigh[%which]);
    %z = getTerrainHeight(%x, %y);
    echo("Re-Spawn Patch "@ %i @" -- " @ %x @" "@ %y@" ("@%rand@": Zone "@%which@")");
    setPosition($orePatch[%i], %x, %y, %z);
    for(%j=1; %j <= $orePatch[%i].maxRocks; %j++)
    {
        %dist = $orePatch[%i].size / 2;
        %x1 = %x - %dist;
        %x2 = %x + %dist;
        %y1 = %y - %dist;
        %y2 = %y + %dist;
        %rx = randomInt(%x1, %x2);
        %ry = randomInt(%y1, %y2);
        %rz = getTerrainHeight(%rx, %ry);
        %rRotZ = randomInt( 0, 360);
        %rRotX = randomInt( 0, 360);
        %newRock = newObject($UseThisDTS, StaticShape, $UseThisDTS @ ".dts");
        addToSet("MissionGroup\\remove", %newrock);
        setPosition( %newRock, %rx, %ry, %rz, %rRotZ, %rRotX);
        echo("Rock "@ %newRock @" "@ %rx @" "@ %ry);
        $orePatch[%i].rock[%j+1] = %newRock;
        $isType[%newRock] = "ore";
        $wasRock[%newRock] = %j;
        $wasPatch[%newRock] = %i;
    }
    $orePatch[%i].oreRemaining = $orePatch[%i].maxOre;
    $orePatch[%i].rocksRemaining = $orePatch[%i].maxRocks;
}
//===============  Ore mining 
functions (Orogogus', slightly edited)

function orePatch::trigger::onEnter(%trigger, %vehicle)
{
    %player = playerManager::vehicleIdToPlayerNum(%vehicle);
    echo(%player @ " enter patch " @ %trigger.number);
    if(%player != 0)
        say(%player, 35, "<F4>" @ %trigger.oreRemaining @ " units of ore in this patch.  Shutdown to mine.");
    if(%vehicle.oreCarried >= %vehicle.oreCapacity)
    {
        if(%player != 0)
            say(%player, 35, "<F4>Already carrying a full load of ore");
        return;
    }
    %vehicle.inPatch = %trigger;
    schedule( "harvestOre( " @ %trigger @ ", " @ %vehicle @ ");", 1);
}

function orePatch::trigger::onLeave(%trigger, %vehicle)
{
    %vehicle.inPatch = 0;
}

function harvestOre(%trigger, %vehicle)
{
    %player = playerManager::vehicleIdToPlayerNum(%vehicle);
    if(%vehicle.inPatch == 0) 
        return;
    if(isShutDown(%vehicle) == false) 
    {
        if(%vehicle.readyToMine == 1)
        {
            %vehicle.readyToMine = 0;
            if(%player != 0)
                say(%player, 35, "<F4>" @ %vehicle.oreCarried @ " units of ore in hold.");
            schedule("harvestOre(" @ %trigger @ ", " @ %vehicle @ ");", 1);
        }
        else if(%vehicle.readyToMine == 0)
        {
            schedule("harvestOre(" @ %trigger @ ", " @ %vehicle @ ");", 1);
        }
        return;
    }
    %vehicle.readyToMine = 1;
    %trigger.oreRemaining = %trigger.oreRemaining - %vehicle.harvestRate;
    %vehicle.oreCarried = %vehicle.oreCarried + %vehicle.harvestRate;
    if(%vehicle.oreCarried >= %vehicle.oreCapacity)
    {
        if(%player != 0)
            say(%player, 35, "<F4>Full load!");
        %remainder = %vehicle.oreCarried - %vehicle.oreCapacity;
        %vehicle.oreCarried = %vehicle.oreCapacity;
        %trigger.oreRemaining = %trigger.oreRemaining + %remainder;
        updateRocks(%trigger);
        return;
    }
    if(%trigger.oreRemaining <= 0)
    {
        if(%player != 0)
            say(%player, 35, "<F4>Ore patch exhausted");
        updateRocks(%trigger);
        setPosition(%trigger, 10000, 10000, 10000);
        schedule("respawnOrePatch("@%trigger.number@");", 45);
        return;
    }
    schedule("harvestOre(" @ %trigger @ ", " @ %vehicle @ ");", 1);
}
//=============== Ore dumping 
functions (Copy of Orogogus' Harvest Ore, modifyed to do the reverse, sort-of)

function supplyDump::trigger::onEnter(%trigger, %vehicle)
{
    %player = playerManager::vehicleIdToPlayerNum(%vehicle);
    %trigName = getObjectName(%trigger);
    %trigName = strAlign(5, left, %trigName);
    %team = getTeam(%vehicle);
    %CorrectTeam = 0;
    if(%team == *IDSTR_TEAM_RED && %trigName == "RDump") 
    { 
        %CorrectTeam = 1; 
    }
    if(%team == *IDSTR_TEAM_BLUE && %trigName == "BDump") 
    { 
        %CorrectTeam = 1; 
    }
    if(%trigName != "RDump" && %trigName != "BDump") 
    {
        %CorrectTeam = 1; 
    }
    if(%CorrectTeam != 1)
    {
        if(%player != 0)
            say(%player, 35, "<F4>This isn't your team's supply dump. \nShutdown to steal ore from the other team.");
        %vehicle.inDump = 2;
        %player.readyToDump = 0;
        schedule( "stealOre(" @ %vehicle @ ", " @ %player @ ");", 0);
        return;
    }
    if(%vehicle.oreCarried <= 0)
    {
        if(%player != 0)
            say(%player, 35, "<F4>No Ore to dump. Your Team has " @ $Unallocated[%team] @ " units of ore.");
        return;
    }
    if(%player != 0)
        say(%player, 35, "<F4>Your team has " @ $Unallocated[%team] @ " units of ore. Shutdown to Dump.");
    %vehicle.inDump = 1;
    schedule( "dumpOre( " @ %trigger @ ", " @ %vehicle @ ", \"" @ %team @ "\");", 1);
}

function stealOre(%vehicle, %player)  // Ore theft: misdeminor, up to $1,500 & 31 days in prison.
{
    %team = getTeam(%vehicle);
    if(%vehicle.inDump != 2)
    { 
        return; 
    }
    if(isShutDown(%vehicle) == false)
    {
        schedule( "stealOre(" @ %vehicle @ ", " @ %player @ ");", 1);
        return;
    }
    if(%team == *IDSTR_TEAM_BLUE) 
        %unTeam = *IDSTR_TEAM_RED;
    if(%team == *IDSTR_TEAM_RED) 
        %unTeam = *IDSTR_TEAM_BLUE;
    if($Unallocated[%unTeam] <= 0)
    {
        if(%player)
            say(%player, 35, "<F4>" @ %unTeam @ " has no more ore to steal. Carrying " @ %vehicle.oreCarried @ " units of ore.");
        %remainder = 0 - $Unallocated[%unTeam];
        $Unallocated[%unTeam] = 0;
        $Ore[%unTeam] = 0;
        %vehicle.oreCarried = %vehicle.oreCarried - %remainder;
        %vehicle.inDump = 0;
        return;
    }
    $Ore[%unTeam] = $Ore[%unTeam] - %vehicle.dumpRate;
    $Unallocated[%unTeam] = $Unallocated[%unTeam] - %vehicle.dumpRate;
    %vehicle.oreCarried = %vehicle.oreCarried + %vehicle.dumpRate;
    if(%vehicle.oreCarried >= %vehicle.oreCapacity)
    {
        %remainder = %vehicle.oreCarried - %vehicle.oreCapacity;
        %vehicle.oreCarried = %vehicle.oreCapacity;
        $Ore[%unTeam] = $Ore[%unTeam] + %remainder;
        $Unallocated[%unTeam] = $Unallocated[%unTeam] + %remainder;
        %vehicle.inDump = 0;
        if(%player)
            say(%player, 35, "<F4>Max capacity reached. Carrying " @ %vehicle.oreCarried @ " units of ore.");
        return;
    }
    schedule( "stealOre(" @ %vehicle @ ", " @ %player @ ");", 1);
}

function supplyDump::trigger::onLeave(%trigger, %vehicle)
{
    %vehicle.inDump = 0;
}

function dumpOre(%trigger, %vehicle, %team)
{
    %player = playerManager::vehicleIdToPlayerNum(%vehicle);
    if(%vehicle.inDump == 0) 
        return;
    if(isShutDown(%vehicle) == false) 
    {
        if(%vehicle.readyToDump == 1)
        {
        %vehicle.readyToDump = 0;
        if(%player != 0)
            say(%player, 35, "<F4>" @ %vehicle.oreCarried @ " units of ore in hold.");
        schedule("dumpOre(" @ %trigger @ ", " @ %vehicle @ ", \"" @ %team @ "\");", 1);
        }
        else if(%vehicle.readyToDump == 0)
        {
            schedule("dumpOre(" @ %trigger @ ", " @ %vehicle @ ", \"" @ %team @"\");", 1);
        }
        return;
    }
    %vehicle.readyToDump = 1;
    %howmuch = %vehicle.dumpRate * $Multiplier[%team];
    $Ore[%team] = $Ore[%team] + %howmuch;
    $Unallocated[%team] = $Unallocated[%team] + %howmuch;
    %vehicle.oreCarried = %vehicle.oreCarried - %vehicle.dumpRate;
    if(%vehicle.oreCarried <= 0)
    {
        %remainder = %vehicle.oreCarried * -1;
        %vehicle.oreCarried = 0;
        %howmuchover = %remainder * $Multiplier[%team];
        $Ore[%team] = $Ore[%team] - %howmuchover;
        $Unallocated[%team] = $Unallocated[%team] - %howmuchover;
        if(%player != 0)
            say(%player, 0, "Dumping complete. Your team has " @ $Unallocated[%team] @ " units of ore.");
        return;
    }
    schedule("dumpOre(" @ %trigger @ ", " @ %vehicle @ ", \"" @ %team @ "\");", 1);
}
//=============== Building 
functions (Finally, something I actually did)

function Cancel::structure::onScan(%scanned, %scanner)
{
    %player = playerManager::VehicleIdToPlayerNum(%scanner);
    if(%scanner.cancel != true)
    {
        say(%player, 35, "<F4>Next Build Order to finish will be canceled. Scan again to un-cancel.");
        %scanner.cancel = true;
    }
    else
    {
        say(%player, 35, "<F4>Cancel canceled. (Next build will <F6>NOT<F4> be canceled.)");
        %scanner.cancel = false;
    }
}

function TurretBuilderL::structure::onScan(%scanned, %scanner)
{
    %player = playerManager::vehicleIdToPlayerNum(%scanner);
    %team = getTeam(%player);
    %teamScanned = getTeam(%scanned);
    if(%team != %teamScanned)
    {
        say(%player, 35, "<F4>This is not your team's builder.");
        return;
    }
    if($Unallocated[%team] < 20)
    {
        say(%player, 35, "<F4>Not enough ore.");
        return;
    }
    say(%player, 35, "<F4>Light Turret to be created at your Nav Point in 10 seconds.  Must be within the vacinity of one of your bases. Scan central pylon to cancel.");
    schedule("StuffBuilder(\""@%team@"\", "@%scanner@", \"Light Turret\", 20);", 10);
    $Unallocated[%team] = $Unallocated[%team] - 20;
}

function TurretBuilderH::structure::onScan(%scanned, %scanner)
{
    %player = playerManager::vehicleIdToPlayerNum(%scanner);
    %team = getTeam(%player);
    %teamScanned = getTeam(%scanned);
    if(%team != %teamScanned)
    {
        say(%player, 35, "<F4>This is not your team's builder.");
        return;
    }
    if($Unallocated[%team] < 30)
    {
        say(%player, 35, "<F4>Not enough ore.");
        return;
    }
    say(%player, 35, "<F4>Heavy Turret to be created at your Nav Point in 10 seconds.  Must be within the vacinity of one of your bases. Scan central pylon to cancel.");
    schedule("StuffBuilder(\""@%team@"\", "@%scanner@", \"Heavy Turret\", 30);", 10);
    $Unallocated[%team] = $Unallocated[%team] - 30;
}

function RefineryBuilder::structure::onScan(%scanned, %scanner)
{
    %player = playerManager::vehicleIdToPlayerNum(%scanner);
    %team = getTeam(%player);
    %teamScanned = getTeam(%scanned);
    if(%team != %teamScanned)
    {
        say(%player, 35, "<F4>This is not your team's builder.");
        return;
    }
    if($Multiplier[%team] >= 4)
    {
        say(%player, 35, "<F4>Maximum number of refineries already reached.");
        return;
    } 
    if($Unallocated[%team] < 40)
    {
        say(%player, 35, "<F4>Not enough ore.");
        return;
    }
    say(%player, 35, "<F4>Refinery to be created at your Nav Point in 10 seconds.  Must be within the vacinity of one of your bases. Scan central pylon to cancel.");
    schedule("StuffBuilder(\""@%team@"\", "@%scanner@", \"Refinery\", 40);", 10);
    $Unallocated[%team] = $Unallocated[%team] - 40;
}

function expansionBuilder::structure::onScan(%scanned, %scanner)
{
    %player=playerManager::vehicleIdToPlayerNum(%scanner);
    %team=getTeam(%player);
    %teamScanned=getTeam(%scanned);
    if(%team != %teamScanned)
    {
        say(%player, 35, "<F4>This is not your team's builder.");
        return;
    }
    if($Unallocated[%team] < 80)
    {
        say(%player, 35, "<F4>Not enough ore.");
        return;
    }
    say(%player, 35, "<F4>Base expansion to be created at your Nav Point in 10 seconds.  Must be within the vacinity of one of your bases. Scan central pylon to cancel.");
    schedule("baseExpander(\""@%team@"\", "@%scanner@");", 10);
    $Unallocated[%team] = $Unallocated[%team] - 80;
}

function baseExpander(%team, %vehicle)
{
    %player = playerManager::VehicleIdToPlayerNum(%vehicle);
    if($ore[%team] < 80)
    {
        say(%player, 35, "<F4>Not enough ore to complete base expansion.");
        $Unallocated[%team] = $Unallocated[%team] + 80;
        return;
    }
    %marker = getVehicleNavMarkerId(%vehicle);
    %x = getPosition(%marker, x);
    %y = getPosition(%marker, y);
    %z = getPosition(%marker, z);
    if(%x == 0 && %y == 0 && %z == 0)
    {
        say(%player, 35, "<F4>You must set a nav marker first. That is where the base expansion will be centered.");
        $Unallocated[%team] = $Unallocated[%team] + %cost;
        return;
    }
    if(%team==*IDSTR_TEAM_BLUE)
    {
        for(%i=1; %i<=$BlueMarkers; %i++)
        {
            %dist = getDistance(%marker, $BlueMarker[%i]);
            if(%dist <= $BlueRadius[%i]) 
            { 
                %inside = true; 
            }
        }
    }
    else if(%team==*IDSTR_TEAM_RED)
    {
        for(%i=1; %i<=$RedMarkers; %i++)
        {
            %dist = getDistance(%marker, $RedMarker[%i]);
            if(%dist <= $RedRadius[%i]) 
            { 
                %inside = true; 
            }
        }
    }
    if(%inside != true)
    {
        say(%player, 35, "<F4>Nav too far from base. Expansion canceled.");
        $Unallocated[%team] =  $Unallocated[%team] + 80;
        return;
    }
    if(%team == *IDSTR_TEAM_BLUE)
    {
        %new = $BlueMarkers + 1;
        %newMarker = newObject(navigationMarker, ESNavMarker);
        $BlueMarker[%new] = %newMarker;
        $BlueBaseX[%new] = %x;
        $BlueBaseY[%new] = %y;
        $BlueMarkers = %new;
        $BlueRadius[%new] = $NewBaseRadius;
        setPosition(%newMarker, %x, %y, %z);
        setTeam(%newMarker, *IDSTR_TEAM_BLUE);
        // setNavMarker(%newMarker, True, 2);
        $Ore[%team] = $Ore[%team] - 80;
    }
    else if(%team==*IDSTR_TEAM_RED)
    {
        %new = $RedMarkers + 1;
        %newMarker=newObject(navigationMarker, ESNavMarker);
        $RedMarker[%new] = %newMarker;
        $RedBaseX[%new]= %x;
        $RedBaseY[%new]= %y;
        $RedMarkers = %new;
        $RedRadius[%new] = $NewBaseRadius;
        setPosition(%newMarker, %x, %y, %z);
        setTeam(%newMarker, *IDSTR_TEAM_RED);
        // setNavMarker(%newMarker, True, 3);
        $Ore[%team] = $Ore[%team] - 80;
    }
    teamTransmissions("<F4>Base Extended. You can now build things farther away (think \"turrets closer to ore patches\").", %team);
    addToSet("MissionGroup\\remove", %newMarker); 
}

function SiloBuilder::structure::onScan(%scanned, %scanner)
{
    %player = playerManager::vehicleIdToPlayerNum(%scanner);
    %team = getTeam(%player);
    %teamScanned = getTeam(%scanned);
    if(%team != %teamScanned)
    {
        say(%player, 35, "<F4>This is not your team's builder.");
        return;
    }
    if($SiloBuilt[%team] == true)
    {
        say(%player, 35, "<F4>Your team already has a silo.");
        return;
    } 
    if($Unallocated[%team] < 100)
    {
        say(%player, 35, "<F4>Not enough ore.");
        return;
    }
    say(%player, 35, "<F4>Nuclear Silo to be created at your Base in 10 seconds.  Scan central pylon to cancel. You may have only one nuclear silo.");
    schedule("BuildSilo(\""@%team@"\", "@%scanner@");", 10);
    $Unallocated[%team] = $Unallocated[%team] - 100;
}

function BuildSilo(%team, %vehicle)
{
    %player = playerManager::vehicleIdToPlayerNum(%vehicle);
    if(%vehicle.cancel == true)
    {
        say(%player, 35, "<F4>Nuclear Silo canceled. Scan central pylon again to cancel additional build orders.");
        %vehicle.cancel = false;
        return;
    }
    if($Ore[%team] < 100)
    {
        say(%player, 35, "<F4>Not Enough Ore To Complete Build Order.");
        $Unallocated[%team] =  $Unallocated[%team] + 100;
        return;
    }
    if($SiloBuilt[%team] == true)
    {
        say(%player, 35, "<F4>Your team already has a silo.");
        $Unallocated[%team] = $Unallocated[%team] + 100;
        return;
    } 
    setShapeVisibility($Silo[%team], true);
    %building = newObject("Silo", StaticShape, $SiloDTS[%team] );
    setTeam(%building, %team);
    if($SiloZRot[%team] == "") 
        $SiloZRot[%team] = 0;
    if($SiloXRot[%team] == "") 
        $SiloXRot[%team] = 0;
    setPosition(%building, $SiloX[%team], $SiloY[%team], $SiloZ[%team], $SiloZRot[%team], $SiloXRot[%team]);
    $isType[%building] = "silo";
    $SiloBuilt[%team] = true;
    $Ore[%team] = $Ore[%team] - 100;
    teamTransmissions("<F4>Build Complete: Nuclear Silo", %team);
}

function StuffBuilder(%team, %vehicle, %type, %cost)
{
    %player = playerManager::vehicleIdToPlayerNum(%vehicle);
    if(%vehicle.cancel == true)
    {
        say(%player, 35, "<F4>"@%type@" canceled. Scan central pylon again to cancel additional build orders.");
        %vehicle.cancel = false;
        return;
    }
    if($Ore[%team] < %cost)
    {
        say(%player, 35, "<F4>Not Enough Ore To Complete Build Order.");
        $Unallocated[%team] =  $Unallocated[%team] + %cost;
        return;
    }
    %marker=getVehicleNavMarkerId(%vehicle);
    %x=getPosition(%marker, x);
    %y=getPosition(%marker, y);
    %z=getPosition(%marker, z);
    if(%x == 0 && %y == 0 && %z == 0)
    {
        say(%player, 35, "<F4>You must set a nav marker first. That is where the building will be placed.");
        $Unallocated[%team] = $Unallocated[%team] + %cost;
        return;
    }
    %zt = getTerrainHeight(%x, %y);
    if(%z > %zt) 
    { 
        %z = %zt; 
    }
    %inside=false;
    if(%team==*IDSTR_TEAM_BLUE)
    {
        for(%i=1; %i<=$BlueMarkers; %i++)
        {
            %dist = getDistance(%marker, $BlueMarker[%i]);
            if(%dist <= $BlueRadius[%i]) 
            { 
                %inside = true; 
            }
        }
    }
    else if(%team==*IDSTR_TEAM_RED)
    {
        for(%i=1; %i<=$RedMarkers; %i++)
        {
            %dist = getDistance(%marker, $RedMarker[%i]);
            if(%dist <= $RedRadius[%i]) 
            { 
                %inside = true; 
            }
        }
    }
    if(%inside!=true)
    {
        say(%player, 35, "<F4>Nav too far from base. "@%type@" canceled.");
        $Unallocated[%team] = $Unallocated[%team] + %cost;
        return;
    }
    $Ore[%team] = $Ore[%team] - %cost;
    if(%type == "Light Turret")
    {
        %building = newObject(Turret_H_M_LS, Turret, 7);
        $isType[%building] = "turret";
    }
    else if(%type == "Heavy Turret")
    {
        %building = newObject(Turret_H_M_MS, Turret, 17); 
        $isType[%building] = "turret";
    }
    else if(%type == "Refinery")
    {
        if($Multiplier[%team] >= 4)
        {
            say(%player, 35, "<F4>Maximum number of refineries already reached.");
            $ore[%team] = $ore[%team] + %cost;
            $Unallocated[%team] = $Unallocated[%team] + %cost;
            return;
        } 
        %building = newObject("Refinery", StaticShape, "hMoonSilos.dts" );
        $isType[%building] = "refinery";
        $Multiplier[%team]++;
    }
    %randRot = randomInt(0, 4);
    %randRot = %randRot * 90; // Keep buildings orderly by making each face N/S/E/W but random which face is which (make sense?)
    setPosition(%building, %x, %y, %z, %randRot);
    setTeam(%building, %team);
    teamTransmissions("<F4>Build Completed: " @ %type, %team);
    addToSet("MissionGroup\\remove", %building); 
}
//=================================================================================
//===============AI STUFF!!========================================================
//=================================================================================

function AiBuilder::structure::onScan(%scanned, %scanner)
{
    %which = getObjectName(%scanned);
    %type = strAlign(6, left, %which);
    %player = playerManager::vehicleIdToPlayerNum(%scanner);
    %team = getTeam(%player);
    %scannedTeam = getTeam(%scanned);
    if(%team != %scannedTeam)
    {
        say(%player, 35, "<F4>This is not your team's builder.");
        return;
    }
    %cost = $HarvAiCost[%type];
    if($Unallocated[%team] < %cost)
    {
        say(%player, 35, "<F4>Not enough ore.");
        return;
    }
    say(%player, 0, "<F4>" @ %type @ " to be built at your nav in 10 seconds. Scan it to have it guard your nav. Must be within the vacinity of one of your bases.");
    schedule("AIbuilder(\"" @ %team @ "\", " @ %scanner @ ", \"" @ %type @ "\", " @ %cost @ ");", 10);
    $Unallocated[%team] = $Unallocated[%team] - %cost;
}

function AIbuilder(%team, %vehicle, %type, %cost)
{
    %player = playerManager::VehicleIdToPlayerNum(%vehicle);
    if($Ore[%team] < %cost)
    {
        say(%player, 35, "<F4>Not enough ore to complete AI construction.");
        $Unallocated[%team] = $Unallocated[%team] + %cost;
        return;
    }
    %marker = getVehicleNavMarkerId(%vehicle);
    %x = getPosition(%marker, x);
    %y = getPosition(%marker, y);
    %z = getPosition(%marker, z);
    if(%x == 0 && %y == 0 && %z == 0)
    {
        say(%player, 35, "<F4>You must set a nav marker first. That is where the AI will be built.");
        $Unallocated[%team] = $Unallocated[%team] + %cost;
        return;
    }
    %zt = getTerrainHeight(%x, %y);
    if(%z < %zt) 
    { 
        %z = %zt; 
    }
    if(%team==*IDSTR_TEAM_BLUE)
    {
        for(%i=1; %i<=$BlueMarkers; %i++)
        {
            %dist = getDistance(%marker, $BlueMarker[%i]);
            if(%dist <= $BlueRadius[%i]) 
            {
                %inside = true; 
            }
        }
    }
    else if(%team==*IDSTR_TEAM_RED)
    {
        for(%i=1; %i<=$RedMarkers; %i++)
        {
            %dist = getDistance(%marker, $RedMarker[%i]);
            if(%dist <= $RedRadius[%i]) 
            {
                %inside = true;
            }
        }
    }
    if(%inside!=true)
    {
        say(%player, 35, "<F4>Nav too far from base. AI canceled.");
        $Unallocated[%team] = $Unallocated[%team] + %cost;
        return;
    }
    %nameIt = %team@" "@%type;
    %newAi = newObject(%nameIt, $HarvAiType[%type], $HarvAiId[%type]);
    setTeam(%newAi, %team);
    setStaticShapeShortName(%newAi, %nameIt);
    setPosition(%newAi, %x, %y, %z);
    //setPilotId(%newAi, 35);
    order(%newAi, speed, high);
    order(%newAi, guard, %vehicle);
    $Ore[%team] = $Ore[%team] - %cost;
    %newAi.guarding = %vehicle;
    %newAi.ingPoint = false;
    if(%team == "Red Team")
    {
        addToSet("MissionGroup\\remove\\Ai\\Red", %newAi);
    }
    if(%team == "Blue Team")
    {
        addToSet("MissionGroup\\remove\\Ai\\Blue", %newAi);
    }
    say(%player, 35, "<F4>AI guard built. guarding your vehicle. Scan to order it to guard your nav.");
    %time = getTime();
    %newAi.time = %time;
    schedule("guardUpdater(" @ %newAi @ ", \"" @ %time @ "\", " @ %vehicle @ ");", 10);
    %i = %vehicle.guards + 1;
    %vehicle.Guard[%i] = %newAi;  // Lots of cross-referencing
    %newAi.whichGuard = %i;
    %vehicle.guards++;
}

function guardUpdater(%vehicle, %time, %what, %y, %z)
{
    %name = getObjectName(%vehicle);
    if($echoGuard) echo(%vehicle, ", ", %name, ", ", %vehicle.ingPoint, ", ", %what, " ", %y, " ", %z);
    if(%vehicle.time != %time) 
    {
        return; 
    }
    if(%vehicle.ingPoint == false)
    {
        %team = getTeam(%what);
        %player = playerManager::vehicleIdToPlayerNum(%vehicle.guarding);
    }
    if(%what == "home" || (%team == "" && %vehicle.ingPoint == false))
    {
        %team = getTeam(%vehicle);
        if(%team == *IDSTR_TEAM_BLUE)
        {
            %rand = randomInt(1, $BlueMarkers);
            %what = $BlueMarker[%rand];
        }
        if(%team == *IDSTR_TEAM_RED)
        {
            %rand = randomInt(1, $RedMarkers);
            %what = $RedMarker[%rand];
        }
        %vehicle.ingPoint = true;
    }
    // 9-6-00 Had to re-write all of the checks so that it wouldn't get 6 "xxx" object not found for each of the %what.s when guarding a point
    %patch = %vehicle.inPatch; 
    %dump = %vehicle.inDump;
    if(%dump != 0)
    {
        %trigName = getObjectName(%trigger);
        %trigName = strAlign(5, left, %trigName);
        %team = getTeam(%vehicle);
        %CorrectTeam = 0;
        if(%team == *IDSTR_TEAM_RED && %trigName == "RDump") 
        {
            %CorrectTeam = 1;
        }
        if(%team == *IDSTR_TEAM_BLUE && %trigName == "BDump") 
        { 
            %CorrectTeam = 1;
        }
        if(%trigName != "RDump" && %trigName != "BDump") 
        {
            %CorrectTeam = 1; 
        }
        if((%CorrectTeam == 0 && %vehicle.oreCarried < %vehicle.oreCapacity) || (%correctTeam != 0 && %vehicle.oreCarried > 0))
            order(%vehicle, shutdown, true);
    }
    if(%patch != 0 && %vehicle.oreCarried < %vehicle.oreCapacity)
        order(%vehicle, shutdown, true);
    if(%patch != 0 && %vehicle.oreCarried >= %vehicle.oreCapacity)
        order(%vehicle, shutdown, false);
    if(%dump != 0)
    {
        if((%correctTeam != 0 && %vehicle.oreCarried <= 0) || (%correctTeam == 0 && %vehicle.oreCarried >= %vehicle.oreCapacity))
            order(%vehicle, shutdown, false);
    }
    if(!%y) 
        order(%vehicle, guard, %what);
    else 
    {
        %x = (%what + randomInt(-30, 30));
        %y = (%y + randomInt(-30, 30));
        %z = getTerrainHeight(%x, %y);
        order(%vehicle, guard, %x, %y, %z);
    }
    reloadObject(%vehicle, 100000); // AI units get unlimited ammo 
    %time = getTime();
    %vehicle.time = %time;
    if(!%y) 
        schedule("guardUpdater(" @ %vehicle @ ", \"" @ %time @"\", " @ %what @ ");", 10);
    else 
        schedule("guardUpdater(" @ %vehicle @ ", \"" @ %time @"\", " @ %what @ ", " @ %y @ ", " @ %z @ ");", 10);
}

function shuffleGuards(%vehicle, %which)
{
    %number = %vehicle.Guards;
    for(%i = %which; %i <= %number; %i++)
    {
        %vehicle.Guard[%i] = %vehicle.Guard[%i+1];
    }
    %vehicle.Guards--;
}
//===============Admin 
function===================

function FindPlayers()
{
    %howmany = playerManager::getPlayerCount();
    echo("=======FindPlayers "@%howmany@" total players=======");
    echo("Name++Player Num++Vehicle Num++AI Count++X++Y++Z");
    echo("==AI Type++AI Num++Vehicle Num++waypoint/vehicle++X++Y++Z");
    for(%i = 0; %i < %howmany; %i++)
    {
        %player = playerManager::getPlayerNum(%i);
        %vehicle = playerManager::playerNumToVehicleId(%player);
        %name = getName(%player);
        %guards = %vehicle.guards;
        %x = getPosition(%vehicle, x);
        %y = getPosition(%vehicle, y);
        %z = getPosition(%vehicle, z);
        echo(%name@"++"@%player@"++"@%vehicle@"++"@%guards@"++"@%x@"++"@%y@"++"@%z);
        if(%guards>0)
        {
            for(%j = 1; %j <= %guards; %j++)
            {
                %veh = %vehicle.Guard[%j];
                %type = getObjectName(%veh);
                if(%veh.ingPoint) %wp = "waypoint";
                else %wp = "vehicle";
                %x = getPosition(%veh, x);
                %y = getPosition(%veh, y);
                %z = getPosition(%veh, z);
                echo("=="@%type@"++"@%j@"++"@%veh@"++"@%wp@"++"@%x@"++"@%y@"++"@%z);
            }
        }
    }
    echo("==============End FindPlayers==============");
}

function showOre()
{
    echo("Red: Un-Alocated: "@$Unallocated["Red Team"]@"; Ore: "@$Ore["Red Team"]);
    echo("Blue: Un-Alocated: "@$Unallocated["Blue Team"]@"; Ore: "@$Ore["Blue Team"]);
}
//========================================= Long static array list (Prices suggested by Sentinal, they may get edited for balance)

function setHarvAiVar()
{
    //======================= Vehicle Id Number
    $HarvAiId["C-Seek"] = 20;
    $HarvAiId["T-Talo"] = 4;
    $HarvAiId["K-Talo"] = 13;
    $HarvAiId["C-Goad"] = 21;
    $HarvAiId["R-Aven"] = 31;
    $HarvAiId["R-Eman"] = 30;
    $HarvAiId["T-Pala"] = 6;
    $HarvAiId["K-Pala"] = 15;
    $HarvAiId["T-Mino"] = 2;
    $HarvAiId["K-Mino"] = 11;
    $HarvAiId["T-Disr"] = 8;
    $HarvAiId["K-Disr"] = 17;
    $HarvAiId["R-Drea"] = 32;
    $HarvAiId["C-Bolo"] = 25;
    $HarvAiId["C-Recl"] = 26;
    $HarvAiId["R-Pred"] = 41;
    $HarvAiId["C-Shep"] = 22;
    $HarvAiId["T-Myrm"] = 7;
    $HarvAiId["K-Myrm"] = 16;
    $HarvAiId["T-Basi"] = 5;
    $HarvAiId["K-Basi"] = 14;
    $HarvAiId["C-Adju"] = 23;
    $HarvAiId["P-Adju"] = 55;
    $HarvAiId["T-Gorg"] = 3;
    $HarvAiId["K-Gorg"] = 12;
    $HarvAiId["R-Olym"] = 33;
    $HarvAiId["T-Apoc"] = 1;
    $HarvAiId["K-Apoc"] = 10;
    $HarvAiId["C-Exec"] = 24;
    $HarvAiId["P-Exec"] = 56;
    //========================= Vehicle Cost
    $HarvAiCost["C-Seek"] = 50;
    $HarvAiCost["T-Talo"] = 55;
    $HarvAiCost["K-Talo"] = 60;
    $HarvAiCost["C-Goad"] = 70;
    $HarvAiCost["R-Aven"] = 80;
    $HarvAiCost["R-Eman"] = 100;
    $HarvAiCost["T-Pala"] = 100;
    $HarvAiCost["K-Pala"] = 110;
    $HarvAiCost["T-Mino"] = 110;
    $HarvAiCost["K-Mino"] = 110;
    $HarvAiCost["T-Disr"] = 120;
    $HarvAiCost["K-Disr"] = 120;
    $HarvAiCost["R-Drea"] = 130;
    $HarvAiCost["C-Bolo"] = 135;
    $HarvAiCost["C-Recl"] = 135;
    $HarvAiCost["R-Pred"] = 140;
    $HarvAiCost["C-Shep"] = 150;
    $HarvAiCost["T-Myrm"] = 160;
    $HarvAiCost["K-Myrm"] = 170;
    $HarvAiCost["T-Basi"] = 170;
    $HarvAiCost["K-Basi"] = 180;
    $HarvAiCost["C-Adju"] = 190;
    $HarvAiCost["P-Adju"] = 200;
    $HarvAiCost["T-Gorg"] = 220;
    $HarvAiCost["K-Gorg"] = 230;
    $HarvAiCost["R-Olym"] = 250;
    $HarvAiCost["T-Apoc"] = 260;
    $HarvAiCost["K-Apoc"] = 270;
    $HarvAiCost["C-Exec"] = 290;
    $HarvAiCost["P-Exec"] = 300;
    //=========================== Vehicle Type (Tank/Herc/Flyer)
    $HarvAiType["C-Seek"] = Herc;
    $HarvAiType["T-Talo"] = Herc;
    $HarvAiType["K-Talo"] = Herc;
    $HarvAiType["C-Goad"] = Herc;
    $HarvAiType["R-Aven"] = Tank;
    $HarvAiType["R-Eman"] = Herc;
    $HarvAiType["T-Pala"] = Tank;
    $HarvAiType["K-Pala"] = Tank;
    $HarvAiType["T-Mino"] = Herc;
    $HarvAiType["K-Mino"] = Herc;
    $HarvAiType["T-Disr"] = Tank;
    $HarvAiType["K-Disr"] = Tank;
    $HarvAiType["R-Drea"] = Tank;
    $HarvAiType["C-Bolo"] = Tank;
    $HarvAiType["C-Recl"] = Tank;
    $HarvAiType["R-Pred"] = Tank;
    $HarvAiType["C-Shep"] = Herc;
    $HarvAiType["T-Myrm"] = Tank;
    $HarvAiType["K-Myrm"] = Tank;
    $HarvAiType["T-Basi"] = Herc;
    $HarvAiType["K-Basi"] = Herc;
    $HarvAiType["C-Adju"] = Herc;
    $HarvAiType["P-Adju"] = Herc;
    $HarvAiType["T-Gorg"] = Herc;
    $HarvAiType["K-Gorg"] = Herc;
    $HarvAiType["R-Olym"] = Herc;
    $HarvAiType["T-Apoc"] = Herc;
    $HarvAiType["K-Apoc"] = Herc;
    $HarvAiType["C-Exec"] = Herc;
    $HarvAiType["P-Exec"] = Herc;
}
//============================================\\
//  || From Mike The Goad's scoreBoard.cs ||  \\
//  \/        Slightly Modifyed           \/  \\
//============================================\\

function wilzuun::initScoreBoard()
{
    deleteVariables("$ScoreBoard::PlayerColumn*");
    deleteVariables("$ScoreBoard::TeamColumn*");
    $ScoreBoard::TeamColumnHeader1 = "Ore";
    $ScoreBoard::TeamColumnHeader2 = *IDMULT_SCORE_PLAYERS;
    $ScoreBoard::TeamColumnHeader3 = *IDMULT_SCORE_KILLS;
    $ScoreBoard::TeamColumnHeader4 = *IDMULT_SCORE_DEATHS;
    $ScoreBoard::TeamColumnHeader5 = "Nuke status";
    $ScoreBoard::TeamColumn
function1 = "getAvailableOre";
    $ScoreBoard::TeamColumn
function2 = "getNumberOfPlayersOnTeam";
    $ScoreBoard::TeamColumn
function3 = "getPermTeamKills";
    $ScoreBoard::TeamColumn
function4 = "getPermTeamDeaths";
    $ScoreBoard::TeamColumn
function5 = "getNukeStatus";
    $ScoreBoard::PlayerColumnHeader1 = *IDMULT_SCORE_TEAM;
    $ScoreBoard::PlayerColumnHeader2 = *IDMULT_SCORE_SQUAD;
    $ScoreBoard::PlayerColumnHeader3 = "Ore";
    $ScoreBoard::PlayerColumnHeader4 = *IDMULT_SCORE_KILLS;
    $ScoreBoard::PlayerColumnHeader5 = *IDMULT_SCORE_DEATHS;
    $ScoreBoard::PlayerColumn
function1 = "getTeam";
    $ScoreBoard::PlayerColumn
function2 = "getSquad";
    $ScoreBoard::PlayerColumn
function3 = "getPlayerOre";
    $ScoreBoard::PlayerColumn
function4 = "getKills";
    $ScoreBoard::PlayerColumn
function5 = "getDeaths";
    serverInitScoreBoard();
}

function getNukeStatus(%a)
{
    if(%a == "1" || %a == "8") 
        return "---";
    if(%a == "2") 
        %team = *IDSTR_TEAM_BLUE;
    if(%a == "4") 
        %team = *IDSTR_TEAM_RED;
    if ($SiloBuilt[%team] == false) 
        return "No silo";
    return $Nuke[%team];
}

function getPermTeamKills(%a)
{
    if(%a == "1" || %a == "8") 
        return "---";
    if(%a == "2") 
        return $Kills[*IDSTR_TEAM_BLUE];
    if(%a == "4") 
        return $Kills[*IDSTR_TEAM_RED];
}

function getPermTeamDeaths(%a)
{
    if(%a == "1" || %a == "8") 
        return "---";
    if(%a == "2") 
        return $Deaths[*IDSTR_TEAM_BLUE];
    if(%a == "4") 
        return $Deaths[*IDSTR_TEAM_RED];
}

function getAvailableOre(%a)
{
    if(%a == "1" || %a == "8") 
        return "---";
    if(%a == "2") 
        return $Unallocated[*IDSTR_TEAM_BLUE];
    if(%a == "4") 
        return $Unallocated[*IDSTR_TEAM_RED];
}

function getPlayerOre(%player)
{
    %vehicle = playerManager::playerNumToVehicleId(%player);
    if(%vehicle == "") 
        return 0;
    return %vehicle.oreCarried;
}
//===================================\\
//  || From MultiPlayerStdLib.cs ||  \\
//  \/    Slightly Formatted     \/  \\
//===================================\\


function getNumberOfPlayersOnTeam(%bits)
{
    %name = getTeamNameFromTeamId(%bits);
    %count = 0;
    %playerCount = playerManager::getPlayerCount();
    for (%p = 0; %p < %playerCount; %p++)
    {
        %player = playerManager::getPlayerNum(%p);
        %team = getTeam(%player);
        if(%team == %name)
        {
            %count++;
        }      
    }
    return %count;
}
//=============================\\
//  || Stuff--ore capacity ||  \\
//  \/                     \/  \\
//=============================\\


function getVehicleCapacity(%vehicle)
{
    %vehicleName = getVehicleName(%vehicle);  
    %vehCapacity = $Capacity[%vehicleName];
    if(%vehCapacity == 0) 
        %vehCapacity = 10;
    return %vehCapacity;
}
$Capacity["Knight's Apocalypse"] = 50;
$Capacity["Basilisk"] = 35;
$Capacity["Knight's Basilisk"] = 35;
$Capacity["Emancipator"] = 25;
$Capacity["Gorgon"] = 45;
$Capacity["Knight's Gorgon"] = 45;
$Capacity["Minotaur"] = 30;
$Capacity["Knight's Minotaur"] = 30;
$Capacity["Olympian"] = 50;
$Capacity["Talon"] = 20;
$Capacity["Knight's Talon"] = 20;
$Capacity["Adjudicator"] = 40;
$Capacity["Platinum Guard Adjudicator"] = 40;
$Capacity["Executioner"] = 55;
$Capacity["Platinum Guard Executioner"] = 55;
$Capacity["Goad"] = 20;
$Capacity["Shepherd"] = 30;
$Capacity["Seeker"] = 20;
$Capacity["Metagen Goad"] = 20;
$Capacity["Metagen Seeker"] = 20;
$Capacity["Metagen Shepherd"] = 30;
$Capacity["Avenger"] = 30;
$Capacity["Disrupter"] = 30;
$Capacity["Knight's Disrupter"] = 30;
$Capacity["Dreadlock"] = 30;
$Capacity["Harabec's Predator"] = 30;
$Capacity["Myrmidon"] = 30;
$Capacity["Knight's Myrmidon"] = 30;
$Capacity["Paladin"] = 30;
$Capacity["Knight's Paladin"] = 30;
$Capacity["Bolo"] = 30;
$Capacity["Recluse"] = 30;
//======================\\
//  || Repair Pylon ||  \\
//  \/              \/  \\
//======================\\

function repair::structure::onScan(%pole, %veh)
{
    %player = playerManager::vehicleIdToPlayerNum(%veh);
    %PoleTeam = getTeam(%pole);
    %team = getTeam(%veh);
    if((%team != %PoleTeam) && (%PoleTeam != *IDSTR_TEAM_NUTRAL))
    {
        say(%player, 35, "This is not your team's repair pole.");
        return;
    }
    if($Unallocated[%team] < 45)
    {
        say(%player, 35, "Not enough ore.");
        return;
    }
    $Unallocated[%team] = $Unallocated[%team] - 45;
    $Ore[%team] = $Ore[%team] - 45;
    damageObject(%veh, -1000000);
    damageObject(%veh, -1000000);
    damageObject(%veh, -1000000);
    damageObject(%veh, -1000000);
    damageObject(%veh, -1000000);
    damageObject(%veh, -1000000);
    reloadObject(%veh, 1000000);
    reloadObject(%veh, 1000000);
    reloadObject(%veh, 1000000);
    for(%i=1; %i<=%veh.Guards; %i++)
    {
        %j = %veh.Guard[%i];
        damageObject(%j, -1000000);
        damageObject(%j, -1000000);
        damageObject(%j, -1000000);
        damageObject(%j, -1000000);
        damageObject(%j, -1000000);
        damageObject(%j, -1000000);
        reloadObject(%j, 1000000);
        reloadObject(%j, 1000000);
        reloadObject(%j, 1000000);
    }
    say(%player, 35, "Repairs Complete.");
}
//=======================================================\\
//  ||      Most of SalvageScores.cs by Orogogus     ||  \\
//  \/ Used for Salvageable Ore (from dead vehicles) \/  \\
//=======================================================\\

function checkValue(%vehicle)
{
    %totalCost = getVehValue(%vehicle);
    // iterate through the component list
    %componentCount = getComponentCount(%vehicle);
    %index = 0;
    while(%index < %componentCount) 
    {
        %id = getComponentId(%vehicle, %index);
        %compValue = compIdValue(%id);
        %totalCost = %totalCost + %compValue;
        %index++;
    }
    // iterate through the weapon list
    %weapCount = getWeaponCount(%vehicle);
    %index = 0;
    while(%index < %weapCount) 
    {
        %id = getWeaponId(%vehicle, %index);
        %weapValue = weapIdValue(%id);
        %totalCost = %totalCost + %weapValue;
        %index++;
    }
    return(%totalCost);
}
//============

function weapIdValue(%id) 
{ 
    if ( $weaponST[%id] != "" ) 
    { 
        return($weaponST[%id]);
    }
    return(0);
}

function compIdValue(%id) 
{ 
    if ( $componentST[%id] != "" ) 
    { 
        return($componentST[%id]);
    }
    return(0);
}
//=============

function getVehValue(%vehicleId)
{
    %vehicleName = getVehicleName(%vehicleId);  
    %vehValue = getVehValue1(%vehicleName);
    return %vehValue;
}

function getVehValue1(%vehicle)
{
    if(%vehicle == "Apocalypse") 
        return 2540;
    else if(%vehicle == "Knight's Apocalypse") 
        return 2670;
    else if(%vehicle == "Basilisk") 
        return 2000;
    else if(%vehicle == "Knight's Basilisk") 
        return 2325;
    else if(%vehicle == "Emancipator") 
        return 1730;
    else if(%vehicle == "Gorgon") 
        return 3350;
    else if(%vehicle == "Knight's Gorgon") 
        return 3805;
    else if(%vehicle == "Minotaur") 
        return 1300;
    else if(%vehicle == "Knight's Minotaur") 
        return 1650;
    else if(%vehicle == "Olympian") 
        return 3150;
    else if(%vehicle == "Talon") 
        return 1000;
    else if(%vehicle == "Knight's Talon") 
        return 1250;
    else 
        getVehValue2(%vehicle);
}

function getVehValue2(%vehicle)
{
    if(%vehicle == "Adjudicator") 
        return 2705;
    else if(%vehicle == "Platinum Guard Adjudicator") 
        return 2995;
    else if(%vehicle == "Executioner") 
        return 3600;
    else if(%vehicle == "Platinum Guard Executioner") 
        return 4010;
    else if(%vehicle == "Goad") 
        return 1600;
    else if(%vehicle == "Shepherd") 
        return 1810;
    else if(%vehicle == "Seeker") 
        return 1205;
    else if(%vehicle == "Metagen Goad") 
        return 1600;
    else if(%vehicle == "Metagen Seeker") 
        return 1205;
    else if(%vehicle == "Metagen Shepherd") 
        return 1810;
    else 
        getVehValue3(%vehicle);
}

function getVehValue3(%vehicle)
{
    if(%vehicle == "Avenger") 
        return 1830;
    else if(%vehicle == "Disrupter") 
        return 2700;
    else if(%vehicle == "Knight's Disrupter") 
        return 2880;
    else if(%vehicle == "Dreadlock") 
        return 2215;
    else if(%vehicle == "Harabec's Predator") 
        return 3510;
    else if(%vehicle == "Myrmidon") 
        return 3035;
    else if(%vehicle == "Knight's Myrmidon") 
        return 3215;
    else if(%vehicle == "Paladin") 
        return 2005;
    else if(%vehicle == "Knight's Paladin") 
        return 2300;
    else if(%vehicle == "Bolo") 
        return 1470;
    else if(%vehicle == "Recluse") 
        return 1720;
    else 
        return 5000;
}
//=================

function initScanTable() 
{
    // assign weapon names for known weapons
    $weaponST[3]   = 600; //DIS
    $weaponST[101] = 435; //LAS
    $weaponST[102] = 700; //HLAS
    $weaponST[103] = 570; //CLAS
    $weaponST[104] = 495; //TLAS
    $weaponST[105] = 610; //EMP
    $weaponST[106] = 820; //ELF
    $weaponST[107] = 755; //BLAS
    $weaponST[108] = 865; //HBLAS
    $weaponST[109] = 900; //PBW
    $weaponST[110] = 1025; //PLAS
    $weaponST[111] = 785; //BLINK
    $weaponST[112] = 1135; //QGUN
    $weaponST[113] = 1225; //MFAC
    $weaponST[114] = 600; //NANO
    $weaponST[115] = 660; //NCAN
    $weaponST[116] = 400; //ATC
    $weaponST[117] = 610; //HATC
    $weaponST[118] = 520; //EMC
    $weaponST[119] = 920; //BC
    $weaponST[120] = 1015; //HBC
    $weaponST[121] = 975; //RAIL
    $weaponST[124] = 605; //VIP8
    $weaponST[125] = 700; //VIP12
    $weaponST[126] = 490; //SPR6
    $weaponST[127] = 560; //SPR10
    $weaponST[128] = 630; //SWRM6
    $weaponST[129] = 840; //MIN10
    $weaponST[130] = 800; //SHRK8
    $weaponST[131] = 410; //ARACH4
    $weaponST[132] = 450; //ARACH8
    $weaponST[133] = 490; //ARACH12
    $weaponST[134] = 400; //PRX6
    $weaponST[135] = 425; //PRX8
    $weaponST[136] = 450; //PRX10
    $weaponST[142] = 770; //RAD
    $weaponST[147] = 595; //APHID10
    $weaponST[150] = 625; //SGUN
    // assign component names for known components
    // engines
    $componentST[100] = 506; // LIGHT Engine
    $componentST[101] = 537; // LITE HO Engine
    $componentST[102] = 620; // L AGL Engine
    $componentST[103] = 669; // MEDIUM Engine
    $componentST[104] = 716; // MED HO Engine
    $componentST[105] = 759; // M AGL Engine
    $componentST[106] = 780; // HEAVY Engine
    $componentST[107] = 800; // I HVY Engine
    $componentST[108] = 839; // H CRU Engine
    $componentST[109] = 876; // HVY HO Engine
    $componentST[110] = 930; // H AGL Engine
    $componentST[111] = 980; // AST Engine
    $componentST[112] = 1012; // I AST Engine
    $componentST[113] = 1058; // HT Engine
    $componentST[114] = 1073; // HOT Engine
    $componentST[115] = 1131; // S/H Engine
    $componentST[128] = 506; // ALPHA Engine
    $componentST[129] = 566; // BETA Engine
    $componentST[130] = 593; // GAMMA Engine
    $componentST[131] = 620; // DELTA Engine
    $componentST[132] = 645; // EPSILON Engine
    $componentST[133] = 693; // ZETA Engine
    $componentST[134] = 716; // ETA Engine
    $componentST[135] = 750; // THETA Engine
    $componentST[136] = 763; // IOTA Engine
    $componentST[137] = 792; // KAPPA Engine
    $componentST[138] = 893; // LAMBDA Engine
    $componentST[139] = 912; // MU Engine
    $componentST[140] = 947; // NU Engine
    $componentST[141] = 1012; // XI Engine
    $componentST[142] = 1043; // OMICRON Engine
    $componentST[143] = 1073; // PI Engine
    // reactors
    $componentST[200] = 410; // Micro Reactor
    $componentST[201] = 450; // Small Reactor
    $componentST[202] = 503; // Standard Reactor
    $componentST[203] = 578; // Medium Reactor
    $componentST[204] = 588; // Large Reactor
    $componentST[205] = 845; // Maxim Reactor
    $componentST[225] = 398; // Alpha Reactor
    $componentST[226] = 438; // Beta Reactor
    $componentST[227] = 513; // Gamma Reactor
    $componentST[228] = 588; // Delta Reactor
    $componentST[229] = 575; // Epsilon Reactor
    $componentST[230] = 730; // Zeta Reactor
    // shields
    $componentST[300] = 639; // Standard Shield
    $componentST[301] = 719; // Protector Shield
    $componentST[302] = 843; // Guardian Shield
    $componentST[303] = 1161; // FastCharge Shield
    $componentST[304] = 926; // Centurion Shield
    $componentST[305] = 1163; // Repulsor Shield
    $componentST[306] = 1025; // Titan Shield
    $componentST[307] = 1447; // Medusa Shield
    $componentST[326] = 639; // Alpha Shield
    $componentST[327] = 671; // Beta Shield
    $componentST[328] = 787; // Gamma Shield
    $componentST[329] = 887; // Delta Shield
    $componentST[330] = 1045; // Epsilon Shield
    $componentST[331] = 1305; // Zeta Shield
    $componentST[332] = 1119; // Eta Shield
    $componentST[333] = 1462; // Theta Shield
    // sensors
    $componentST[400] = 131; // Basic Sensor
    $componentST[401] = 143; // Ranger Sensor
    $componentST[408] = 181; // Standard Sensor
    $componentST[409] = 259; // Longbow Sensor
    $componentST[410] = 351; // Infiltrator Sensor
    $componentST[411] = 250; // Crossbow Sensor
    $componentST[412] = 139; // Ultralight Sensor
    $componentST[413] = 214; // Bloodhound Sensor
    $componentST[414] = 192; // Thermal Sensor
    $componentST[426] = 117; // Alpha Sensor
    $componentST[427] = 133; // Beta Sensor
    $componentST[428] = 158; // Gamma Sensor
    $componentST[429] = 224; // Delta Sensor
    $componentST[430] = 251; // Epsilon Sensor
    $componentST[431] = 211; // Zeta Sensor
    $componentST[432] = 114; // Eta Sensor
    $componentST[433] = 214; // Theta Sensor
    $componentST[434] = 108; // Iota Sensor
    // computers
    $componentST[800] = 25; // Basic Computer
    $componentST[801] = 100; // Improved Computer
    $componentST[802] = 150; // Advanced Computer
    $componentST[805] = 75; // Alpha Computer
    $componentST[806] = 150; // Beta Computer
    $componentST[807] = 200; // Gamma Computer
    // other special items
    $componentST[810] = 700; // ECM-G
    $componentST[811] = 900; // ECM-D
    $componentST[812] = 800; // Alpha ECM
    $componentST[813] = 1000; // Beta ECM
    $componentST[820] = 1075; // THERM
    $componentST[830] = 850; // CHAM
    $componentST[831] = 1125; // CUT
    $componentST[840] = 1200; // SMOD
    $componentST[845] = 500; // SCAP
    $componentST[850] = 825; // SAMP
    $componentST[860] = 500; // LTADS
    $componentST[865] = 1000; // BATTERY
    $componentST[870] = 450; // CAP
    $componentST[875] = 400; // FIELD
    $componentST[880] = 675; // ROCKET
    $componentST[885] = 875; // TURBO
    $componentST[890] = 3000; // REPAIR
    $componentST[900] = 425; // LIFE
    $componentST[910] = 1250; // AGRAV
    $componentST[912] = 500; // EHUL
    $componentST[914] = 750; // UAP
    // armor
    $componentST[926] = 773; //CARLAM Armor
    $componentST[927] = 844; //QBM Armor
    $componentST[928] = 1367; //DURAC Armor
    $componentST[929] = 1182; //CERC Armor
    $componentST[930] = 1257; //CRYSTAL Armor
    $componentST[931] = 1480; //QUICK Armor
}
initScanTable();
//===================================\\
//  || From multiplayerStdLib.cs ||  \\
//  \/          Trimmed          \/  \\
//===================================\\

function getFullPath(%object)
{
    if(%object == 0)
    {
        return "";
    }
    %name = getObjectName(%object);
    if(%name == "MissionGroup")
    {
        return %name;
    }
    else
    {
        return (getFullPath(getGroup(%object)) @ "/" @ %name);
    }
}
// To be overwritten if the player makes one -- if not, no reason to show the admin an error (Unknown 
function)

function HarvVehicleOnDestroyed(%destroyed, %destroyer)
{ }

function HarvVehicleOnAdd(%vehicle)
{ }

function HarvOnMissionLoad()
{ }

function HarvPlayerOnAdd(%player)
{ }

function HarvPlayerOnRemove(%player)
{ }

function HarvOnMissionStart()
{ }

function HarvTurretOnDestroyed(%turret, %veh)
{ }

function HarvVehicleOnMessage(%a, %b, %c, %d, %e, %f, %g, %h)
{ }
// Presented by:  +-------------------------+
//                | FLYING DUCK PRODUCTIONS |  
//                |       <^>(o o)<^>       |
//                |           \_/         � |
//                +-------------------------+
// END  