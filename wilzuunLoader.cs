## -------------------------------------------------------------------- Header
// FILE:        WilzuunLoader.cs
// AUTHORS:     ^TFW^ Wilzuun
// LAST MOD:    10 Dec 2019
// VERSION:     1.0

## -------------------------------------------------------------------- IMPORTANT NOTES
// IF YOU USE DOV, HAVE AN EMPTY .CS FILE!
// DOV WILL NOT WORK BECAUSE OF THE VOLUME OF AREAS IT NEEDS TO TRIGGER AT.

// Specifically, this file will attempt to load all modules that a map maker wants.

// DYNCITY CAN ERASE A LOT OF THINGS! BEWARE!
// The DynCity script(s) are meant to make dynamic cities on an empty map.
// It will not make trigger areas. Like healing or ammo pads. Or map boundaries.

## -------------------------------------------------------------------- Install Instructions
// Place this file in the Starsiege Multiplayer directory.

## -------------------------------------------------------------------- Version History
//  1.0 - 09 Dec 2019
//      Started ground works.
//  1.1 - 25 July 2020
//      Fixed the DOV loading scripts section
//  14 Jan 2021
//      Reconfigured all libraries of game types for new experimental project.

## -------------------------------------------------------------------- Required libraries.
newObject( wilzunnVol, SimVolume, "WilzuunTools.vol" );
exec("TrigonometryStdLib.cs");
exec("WilzuunStdLib.cs");
exec("pilots.cs");

## -------------------------------------------------------------------- Setup Functions.
// Feel free to add more game type libraries here.
function setupGameTypes() {

    // How many games are we running?
    for(%count = 1; $gameTypes[%count] != ""; %count++)
    { }
    $gameTypesRunning = %count;

    // How many game types are we running? What are they?
    %games = 0;
    // First line is the short hand of the game's name. Or the filename that is
    // part of the (*)StdLib.cs The second line is the full length name of the
    // game type. Example: First like will equal `CTF`, while second line will
    // be equal to `Capture The Flag`
    $gameTypesAvailable[%games++] = "cnh"; // Capture and Hold
    $gameTypesNames["cnh"] = "Capture and Hold";
    $gameTypesAvailable[%games++] = "ctf"; // Capture the flag
    $gameTypesNames["ctf"] = "Capture the flag";
    $gameTypesAvailable[%games++] = "cvh"; // Cybrid vs Human
    $gameTypesNames["cvh"] = "Cybrid vs Human";
    $gameTypesAvailable[%games++] = "dm";  // Death Match
    $gameTypesNames["dm"] = "(Team) Death Match";
    $gameTypesAvailable[%games++] = "dom"; // Domination
    $gameTypesNames["dom"] = "Domination";
    $gameTypesAvailable[%games++] = "edm"; // Elimination Death Match
    $gameTypesNames["edm"] = "Elimination Death Match";
    $gameTypesAvailable[%games++] = "fnr"; // Find & Retrieve.
    $gameTypesNames["fnr"] = "Find & Retrieve";
    $gameTypesAvailable[%games++] = "har"; // Harvester
    $gameTypesNames["har"] = "Harvester";
    $gameTypesAvailable[%games++] = "tag"; // Tag
    $gameTypesNames["Tag"] = "Tag";
    $gameTypesAvailable[%games++] = "tk"; // Targeting Kills.
    $gameTypesNames["tk"] = "Targeting Kills";
    // $gameTypesAvailable[%games++] = ""; // Left empty
    // $gameTypesNames[""] = "";

    // make sure this is the last line in the function.
    $gameTypesAvailable = %games;
}

## -------------------------------------------------------------------- Loading function.
// Please don't edit below this line.
// onMissionPreload executes before everything in your map file, and is a vastly under used function.
// I utilize it for all of my projects.
function onMissionPreload()
{
    setupGameTypes();
    if($wilzuun::Boost == true)
    {
        exec(BoostStdLib);
    }

    if ($changeTheGame == true)
    {
        // Load all game type files now. Hope to God none of them conflict with
        // each other.
        for(%count = 1; $gameTypesAvailable[%count] != ""; %count++)
        {
            exec($gameTypesAvailable[%count] @ "StdLib");
        }

        // End the old game type.
        //%rand1 = randomInt(1,$gameTypesRunning);
        //eval($gameTypes[%rand1] @ "::onMissionEnd();");

        // Start the new game type.
        //%rand2 = (1,$gameTypesAvailable);
        //$gameTypes[%rand1] = $gameTypesAvailable[%rand2];
        //eval($gameTypes[%rand1] @ "::onMissionStart();");

        return;
    }

    for(%count = 1; $gameTypes[%count] != ""; %count++)
    {
        exec($gameTypes[%count] @ "StdLib");
    }
}

function changeTheGame(%newGameType)
{
    $server::AllowMixedTech = TRUE;
    if ($changeTheGame == true)
    {
        // The game is changing. Do we already have a game running?
        if($gameTypes[1] != "")
        {
            eval($gameTypes[1] @ "::inMissionEnd();");
        }

        // Did we want a specific game type? no? so random one!
        if (%newGameType == "")
        {
            %rand = (1,$gameTypesAvailable);
            $gameTypes[1] = $gameTypesAvailable[%rand];
            eval($gameTypes[1] @ "::initScoreBoard();");
            eval($gameTypes[1] @ "::setRules();");
            eval($gameTypes[1] @ "::inMissionStart();");
        }
        // yes, set it, run it.
        else
        {
            $gameTypes[1] = %newGameType];
            eval($gameTypes[1] @ "::initScoreBoard();");
            eval($gameTypes[1] @ "::setRules();");
            eval($gameTypes[1] @ "::inMissionStart();");
        }
        %rand = randomInt(300, 900);
        echo("New game type in " @ %rand @ "s.");
        schedule("changeTheGame();", %rand);
    }
}

// *****************************************************************************
// In an attempt to prevent lots of repeated for statements that do the same
// exact thing, here is a function that resolves all the calls.
//
// Don't edit this function unless you understand what you might be breaking.
// *****************************************************************************

function resolveGameFunctions(%function, %var1, %var2)
{
    if(%function == "")
    {
        return echo("resolveGameFunctions <functionName> [%value1] [%value2]");
    }
    %return = "";
    if(%var1 == "")
    {
        dbecho(3, %function @ "();");
        for(%count = 1; $gameTypes[%count] != ""; %count++)
        {
            %return = %return @ eval( $gameTypes[%count] @ "::" @ %function @ "();");
        }
    }
    else if(%var2 == "")
    {
        dbecho(3, %function @ "(" @ %var1 @ ");");
        for(%count = 1; $gameTypes[%count] != ""; %count++)
        {
            %return = %return @ eval( $gameTypes[%count] @ "::" @ %function @ "(" @ %var1 @ ");");
        }
    }
    else
    {
        dbecho(3, %function @ "(" @ %var1 @ "," @ %var1 @ ");");
        for(%count = 1; $gameTypes[%count] != ""; %count++)
        {
            %return = %return @ eval( $gameTypes[%count] @ "::" @ %function @ "(" @ %var1 @ "," @ %var1 @ ");");
        }
    }
    return %return;
}

// *****************************************************************************
// Load default functions. Users who write their own will have to include the
// the commands in each of these. Shouldn't be too hard to do for any decent
// mapper with their projects.
//
// Considering the fact that most triggers exist with an explicit <ScriptClass>
// name, they have been left out for what ever game type library that wishes to
// make use of triggers. Although not thoroughly required for any library, they
// are not listed in this library.
// *****************************************************************************

## -------------------------------------------------------------------- onMission
function onMissionLoad()
{
    wilzuun::onMissionLoad();
}

function onMissionStart()
{
    wilzuun::onMissionStart();
}

function onMissionEnd()
{
    wilzuun::onMissionEnd();
}
## -------------------------------------------------------------------- Player
function player::onAdd(%player)
{
    wilzuun::player::onAdd(%player);
}

function player::onRemove(%player)
{
    wilzuun::player::onRemove(%player);
}

## -------------------------------------------------------------------- Vehicle
function vehicle::onAdd(%vehicleId)
{
    wilzuun::vehicle::onAdd(%vehicleId);
}

function vehicle::onDestroyed(%destroyed, %destroyer)
{
    wilzuun::vehicle::onDestroyed(%destroyed, %destroyer);
}

function vehicle::onAction()
{
    wilzuun::vehicle::onAction();
}

function vehicle::onAttacked(%attacked, %attacker)
{
    wilzuun::vehicle::onAttacked(%attacked, %attacker);
}

function vehicle::onMessage(%targeted, %targeter)
{
    wilzuun::vehicle::onMessage(%targeted, %targeter);
}

function vehicle::onScan(%scanned, %scanner)
{
    wilzuun::vehicle::onScan(%scanned, %scanner);
}

function vehicle::onTargeted(%targeted, %targeter)
{
    wilzuun::vehicle::onTargeted(%targeted, %targeter);
}

function vehicle::salvage(%vehicle)
{
    wilzuun::vehicle::salvage(%vehicle);
}

function vehicle::onArrived(%this, %where)
{
    wilzuun::vehicle::onArrived(%this, %where);
}

## -------------------------------------------------------------------- Server
function initScoreBoard()
{
    wilzuun::initScoreBoard();
}

function setDefaultMissionOptions()
{
    wilzuun::setDefaultMissionOptions();
}

// *****************************************************************************
// This section is here for the Multi-Game Type mergin project.
// *****************************************************************************

## ------------------------------------------------------------------- onMission
function wilzuun::onMissionLoad()
{
    resolveGameFunctions("onMissionLoad");
}

function wilzuun::onMissionStart()
{
    resolveGameFunctions("onMissionStart");
}

function wilzuun::onMissionEnd()
{
    resolveGameFunctions("onMissionEnd");
}
## ---------------------------------------------------------------------- Player
function wilzuun::player::onAdd(%player)
{
    resolveGameFunctions("player::onAdd", %player);
    player::onAddLog(%player);
}

function wilzuun::player::onRemove(%player)
{
    resolveGameFunctions("player::onRemove", %player);
    player::onRemoveLog(%player);

    // pulled from mutliplayerStdLib.cs
    %vehicle = playerManager::playerNumToVehicleId(%this);
    if(%vehicle != 0)
    {
        if(%vehicle.pad != "")
        {
            // reset the special for the pad this guy was on
            %vehicle.pad.special = "";
        }
    }
}

## --------------------------------------------------------------------- Vehicle
function wilzuun::vehicle::onAdd(%vehicleId)
{
    resolveGameFunctions("vehicle::onAdd", %vehicleId);
    %player = playerManager::vehicleIdToPlayerNum(%vehicleId);
    %player.hasVehicle = true;
}

function wilzuun::vehicle::onDestroyed(%destroyed, %destroyer)
{
    resolveGameFunctions("vehicle::onDestroyed", %destroyed, %destroyer);
    // This will be needed for a few things.
    %dead = playerManager::vehicleIdToPlayerNum(%destroyed);
    %dead.hasVehicle = false;
    // announce player death messages.
    %message = getFancyDeathMessage(getHUDName(%destroyed), getHUDName(%destroyer));
    if(%message != "")
        say( 0, 0, %message);

    // log death.
    vehicle::onDestroyedLog(%destroyed, %destroyer);

    // are we playing teams? should this person be booted?
    // pulled from mutliplayerStdLib.cs
    if($server::TeamPlay == true)
    {
        if((getTeam(%destroyed) == getTeam(%destroyer)) &&
            (%destroyed != %destroyer))
        {
            antiTeamKill(%destroyer);
        }
    }
}

function wilzuun::vehicle::onAction()
{
    resolveGameFunctions("vehicle::onAction");
}

function wilzuun::vehicle::onAttacked(%attacked, %attacker)
{
    resolveGameFunctions("vehicle::onAttacked", %attacked, %attacker);
    // pulled from mutliplayerStdLib.cs
    if( $enableTeamHittingPenalty == True )
    {
        if($server::TeamPlay == true)
        {
            if( %this != %attacker )
            {
                if(getTeam(%this) == getTeam(%attacker))
                {
                    if(dataRetrieve(%attacker, "onProbation") != True)
                    {
                        dataStore(%attacker, "onProbation", True);
                        schedule("dataStore(" @ %attacker @ ", \"onProbation\", False);", $probeFrequency);
                        dataStore(%attacker, "totalOffenses", (dataRetrieve(%attacker, "totalOffenses") + $teamHitPenalty));

                        if( dataRetrieve(%attacker, "hasCountdownSpawned") != True)
                        {
                            countdownOffenses(%attacker);
                            dataStore(%attacker, "hasCountdownSpawned", True);
                        }

                        if( dataRetrieve(%attacker, "totalOffenses") >= $allowableOffenses )
                        {
                            violate(%attacker);
                            %player = playerManager::vehicleIdToPlayerNum(%attacker);

                            if(%player != 0)
                            {
                                say(%player, 1234, *IDMULT_SPANKED, "rules_violated.wav");
                            }
                        }
                    }
                }
            }
        }
    }
}

function wilzuun::vehicle::onMessage(%targeted, %targeter)
{
    resolveGameFunctions("vehicle::onMessage", %targeted, %targeter);
}

function wilzuun::vehicle::onScan(%scanned, %scanner)
{
    resolveGameFunctions("vehicle::onScan", %scanned, %scanner);
}

function wilzuun::vehicle::onTargeted(%targeted, %targeter)
{
    resolveGameFunctions("vehicle::onTargeted", %targeted, %targeter);
}

function wilzuun::vehicle::salvage(%vehicle)
{
    resolveGameFunctions("vehicle::salvage", %vehicleId);
}

function wilzuun::vehicle::onArrived(%this, %where)
{
    resolveGameFunctions("vehicle::onArrived", %this, %where);
}

## -------------------------------------------------------------------- Server
function wilzuun::initScoreBoard()
{
    resolveGameFunctions("initScoreBoard");
}

function wilzuun::setDefaultMissionOptions()
{
    resolveGameFunctions("setDefaultMissionOptions");
}

function wilzuun::setRules()
{
    %rules = resolveGameFunctions("setRules");
}