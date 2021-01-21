##--------------------------- Header
// FILE:        WilzuunLoader.cs
// AUTHORS:     ^TFW^ Wilzuun
// LAST MOD:    10 Dec 2019
// VERSION:     1.0

##--------------------------- IMPORTANT NOTES
// IF YOU USE DOV, HAVE AN EMPTY .CS FILE!
// DOV WILL NOT WORK BECAUSE OF THE VOLUME OF AREAS IT NEEDS TO TRIGGER AT.

// Specifically, this file will attempt to load all modules that a map maker wants.

// DYNCITY CAN ERASE A LOT OF THINGS! BEWARE!
// The DynCity script(s) are meant to make dynamic cities on an empty map.
// It will not make trigger areas. Like healing or ammo pads. Or map boundaries.

##--------------------------- Install Instructions
// Place this file in the Starsiege Multiplayer directory.

##--------------------------- Version History
//  1.0 - 09 Dec 2019
//      Started ground works.
//  1.1 - 25 July 2020
//      Fixed the DOV loading scripts section
//  14 Jan 2021
//      Reconfigured all libraries of game types for new experimental project.

##--------------------------- Required libraries. 
newObject( wilzunnVol, SimVolume, "WilzuunTools.vol" );
exec("TrigonometryStdLib.cs");
exec("WilzuunStdLib.cs");
exec("pilots.cs");

##--------------------------- Loading function. 
// Please don't edit below this line.
// onMissionPreload executes before everything in your map file, and is a vastly under used function.
// I utilize it for all of my projects.
function onMissionPreload()
{
    
    if($wilzuun::Boost == true)
    {
        exec(BoostStdLib);
    }

    
    for(%count = 1; $gameTypes[%count] != ""; %count++)
    {
        exec($gameTypes[%count] @ "StdLib");
    }
    
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
    player::onAddLog(%player);
}

function player::onRemove(%player) 
{
    wilzuun::player::onRemove(%player);
    player::onRemoveLog(%player);
}

## -------------------------------------------------------------------- Vehicle
function vehicle::onAdd(%vehicleId)
{
    wilzuun::vehicle::onAdd(%vehicleId);
}

function vehicle::onDestroyed(%destroyed, %destroyer)
{
    wilzuun::vehicle::onDestroyed(%destroyed, %destroyer);
    vehicle::onDestroyedLog(%destroyed, %destroyer);
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
function wilzuun::OverRide()
{
    $swarmClone = true;
    $wilzuun::Boost = false;
}

function setDefaultMissionOptions()
{
    wilzuun::setDefaultMissionOptions();
}

// *****************************************************************************
// This section is here for the Multi-Game Type mergin project.
// *****************************************************************************

## -------------------------------------------------------------------- onMission
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
## -------------------------------------------------------------------- Player
function wilzuun::player::onAdd(%player) 
{
    resolveGameFunctions("player::onAdd", %player);
    player::onAddLog(%player);
}

function wilzuun::player::onRemove(%player) 
{
    resolveGameFunctions("player::onRemove", %player);
    player::onRemoveLog(%player);
}

## -------------------------------------------------------------------- Vehicle
function wilzuun::vehicle::onAdd(%vehicleId)
{
    resolveGameFunctions("vehicle::onAdd", %vehicleId);
}

function wilzuun::vehicle::onDestroyed(%destroyed, %destroyer)
{
    resolveGameFunctions("vehicle::onDestroyed", %destroyed, %destroyer);
    vehicle::onDestroyedLog(%destroyed, %destroyer);
}

function wilzuun::vehicle::onAction()
{
    resolveGameFunctions("vehicle::onAction");
}

function wilzuun::vehicle::onAttacked(%attacked, %attacker)
{
    resolveGameFunctions("vehicle::onAttacked", %attacked, %attacker);
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
// *****************************************************************************
// In an attempt to prevent lots of repeated for statements that do the same
// exact thing, here is a function that resolves all the calls.
// *****************************************************************************

function resolveGameFunctions(%function, %var1, %var2)
{
    if(%function == "")
    {
        say("resolveGameFunctions <functionName> [%value1] [%value2]");
    }
    if(%var1 == "")
    {
        dbecho(3, %function @ "();");
        for(%count = 1; $gameTypes[%count] != ""; %count++)
        {
            eval( $gameTypes[%count] @ "::" @ %function @ "();");
        }
    }
    else if(%var2 == "")
    {
        dbecho(3, %function @ "(" @ %var1 @ ");");
        for(%count = 1; $gameTypes[%count] != ""; %count++)
        {
            eval( $gameTypes[%count] @ "::" @ %function @ "(" @ %var1 @ ");");
        }
    }
    else
    {
        dbecho(3, %function @ "(" @ %var1 @ "," @ %var1 @ ");");
        for(%count = 1; $gameTypes[%count] != ""; %count++)
        {
            eval( $gameTypes[%count] @ "::" @ %function @ "(" @ %var1 @ "," @ %var1 @ ");");
        }
    }
}
