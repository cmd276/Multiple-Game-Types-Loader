// BoostStdLib.cs by Com. Sentinal [M.I.B.]
// HercBoost Standard Library version 2.1
//-------------------------------------------------------------------------
// Features enhanced control of mid-air flight.
// Full 360 degree range of motion for forward flight.
// Included keymap also has enhanced controls, featuring
// a Forward boost button which can be held down and released.
//-------------------------------------------------------------------------

// Edited for inclusion into Scripting library by Wilzuun.
function boost::vehicle::onAdd(%vehicleId)
{
   %vehicleId.boost = false;
   %vehicleId.hover = false;
   %vehicleId.forward = false;
   %vehicleId.cancel = true;
}

function remoteBoostSwitch(%player)
{
   %vehicleId = playerManager::playerNumToVehicleId(%player);
   %vehicleId.cancel = false;
   %vehicleId.forward = false;
   %vehicleId.boost = true;
   boost(%vehicleId);
}

function remoteCancelswitch(%player)
{
   %vehicleId = playerManager::playerNumToVehicleId(%player);
   %vehicleId.boost = false;
   %vehicleId.hover = false;
   %vehicleId.forward = false;
   %vehicleId.cancel = true;
}

function remoteHoverSwitch(%player)
{
   %vehicleId = playerManager::playerNumToVehicleId(%player);
   %vehicleId.cancel = false;
   %vehicleId.boost = false;
   %vehicleId.forward = false;
   %vehicleId.hover = true;
   hover(%vehicleId);
}

function remoteForwardSwitch(%player)
{
   %vehicleId = playerManager::playerNumToVehicleId(%player);
   %vehicleId.boost = false;
   %vehicleId.hover = false;
   %vehicleId.cancel = false;
   %vehicleId.forward = true;
   forward(%vehicleId);
}

function boost(%vehicleId)
{
   if(%vehicleId.cancel == true) return;
   else if(%vehicleId.boost == true)
   {
      %x = getposition(%vehicleId, x);
      %y = getposition(%vehicleId, y);
      %z = getposition(%vehicleId, z) + 5;
      setPosition(%vehicleId, %x, %y, %z);
      schedule("boost(" @ %vehicleId @ ");", 0.1);
   }
}

function hover(%vehicleId)
{
   if(%vehicleId.cancel == true) return;
   else if(%vehicleId.hover == true)
   {
      %x = getposition(%vehicleId, x);
      %y = getposition(%vehicleId, y);
      %z = getposition(%vehicleId, z) + 0.2;
      setPosition(%vehicleId, %x, %y, %z);
      schedule("hover(" @ %vehicleId @ ");", 0.1);
   }
}

function forward(%vehicleId)
{
   if(%vehicleId.cancel == true) return;
   else if(%vehicleId.forward == true)
   {
      %hypotenuse = 10;
      %rot = getPosition(%vehicleId, rot);
      %angle = RadToDeg(%rot);
      //Not sure exactly why I need to add 90 degrees, but it works
      %xOffset = cos(%angle + 90) * %hypotenuse;
      %yOffset = sin(%angle + 90) * %hypotenuse;
      %x = getposition(%vehicleId, x) + %xOffset;
      %y = getposition(%vehicleId, y) + %yOffset;
      %z = getposition(%vehicleId, z) + 0.2;
      setPosition(%vehicleId, %x, %y, %z);
      schedule("forward(" @ %vehicleId @ ");", 0.25);
   }
}
