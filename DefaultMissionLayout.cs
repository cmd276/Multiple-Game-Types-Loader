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

function setRules ()
{
    // If you wish to add onto the rules, or prepend rules to the game, this is 
    // the optimal way to do it.
    %rules = wilzuun::setRules();
    %rules = "This is a line before the rules.\n" @ %rules;
    setGameInfo(%rules); 
}
setRules();

function GameTypeOverride ()
{
    // intentionally left blank.
}

