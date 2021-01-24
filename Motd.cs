//MIB IP Logger/Banner version 1.1
//Designed by Gen. Raven [M.I.B.] and Com. Sentinal [M.I.B.]
//----------------------------------------------------------------------
//Install this program by placing this file (motd.cs) into your
//Starsiege main directory, replacing the old version of motd.cs.
//This program logs every player that enters your server into
//IP_LOGGER.txt, which is created in your Starsiege main directory
//after you've started your server once. The program logs the date,
//the time, the player's name, his squad, and his IP address.
//You can also ban certain IPs by examining your log, then adding
//the player to the ban list (instructions below). This program does
//NOT use the player::onAdd function. This means that you retain full
//scripting capabilities on your server.
//----------------------------------------------------------------------

$motd = "Hello there, and welcome to Wilzuun's server!\n"@
"Rules: Behave yourself; you're an adult, respect the other players.";

$banCount = 0; //Number of banned IPs (CHANGE THIS NUMBER EACH TIME YOU ADD AN IP)

//Banned IPs and the reason for banning the IP
//The last number of the IP address must be changed to 0:0 for the ban to work.
//For example, 216.183.38.43 would be changed to 216.183.38.0:0

// EXAMPLES OF THE REQUIRED FORMAT:
// $banIp1 = "IP:216.183.38.0:0";
// $banReason1 = "Banned for using foul language";
// $banIp2 = "IP:34.123.37.0:0";
// $banReason2 = "Banned for server crashing";

$banIp1 = "";        // Enter offender's IP address here
$banReason1 = "";    // Enter reason for banning here

//IP logger
%count = playermanager::getPlayerCount();
for(%i=0;%i<%count;%i++)
{
   if((%i+1)==%count)
   {
##--------------------------- Start Edited.
        // We're just going to let the player know what games are running on this server.
        for(%c = 1; $gameTypes[%c] != ""; %c++)
        {
            say(%player,0, $gameTypesNames[$gameTypes[%c]] @ " is currently running.");
        }
##--------------------------- End Edited.
      %player = playermanager::getPlayerNum(%i);
      if(getConnection(%player)=="LOOPBACK")
      {
         fileWrite("IP_LOGGER.txt", append, getDate(), " ", getTime(), " -----------------SERVER STARTED----------------");
      }
      echo(getConnection(%player));
      fileWrite("IP_LOGGER.txt", append, getDate(), " ", getTime(), " - ", getName(%player)," - ", getSquad(%player), " - ", getConnection(%player));
   }   
}

for(%i=1;%i<=$banCount;%i++)
{
   if(isEqualIp(getConnection(%player),$banIP[%i]))
   {
      fileWrite("IP_LOGGER.txt", append, "----> " @ getName(%player) @ " was kicked by AutoBan. Reason: " @ $banReason[%i]);
      say(0,900,"<f5>"@getName(%player)@" was kicked by AutoBan\nIP Address: "@getconnection(%player)@"\nReason: "@$banReason[%i],"alarm2.wav");
      kick(%player, "AUTO BAN: You are on this server's permanent ban list. The reason specified in the ban list is: \""@$banReason[%i]@"\".");
   }
}
