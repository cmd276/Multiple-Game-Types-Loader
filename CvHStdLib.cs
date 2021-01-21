// CvH Standard Library - By Temujin, based on code by Gen Raven [MIB]
// Modified by ^TFW^Wilzuun for conformity into the Starsiege Script Library.
//------------------------------------------------------------------------------

function CvH::player::onAdd(%this)
{
   say(%this, 0, "Welcome to Cybrid Vs. Human Team DeathMatch!  This map is availlable on Temujin's website: www6.50megs.com/temujin.  Cybrids are red, Humans blue.  Tech mixing is NOT allowed.  This is hardcore CvH!!!");
}

//From this line to the next line is the script needed for Cybrid Vs. Human

function CvH::setDefaultMissionItems()
{
   allowVehicle("all", true);
   allowComponent("all", true);
   allowWeapon("all", true);
}

function CvH::vehicle::onAdd(%this)
{
   // Cybrid vs Humans ... Cybrids must be blue, Humans must be red
   if(getVehicleTechBase(%this) == "H" && getTeam(%this) != *IDSTR_TEAM_BLUE)
   {
      setTeam(%this, *IDSTR_TEAM_BLUE);
      say(PlayerManager::vehicleIdToPlayerNum(%this), 1234, "Humans must be on the BLUE team!");
      redrop(%this);
   }
   else if(getVehicleTechBase(%this) == "C" && getTeam(%this) != *IDSTR_TEAM_RED)
   {
      setTeam(%this, *IDSTR_TEAM_RED);
      say(PlayerManager::vehicleIdToPlayerNum(%this), 1234, "Cybrids must be on the RED team!");
      redrop(%this);
   }
}

function CvH::setRules()
{
      %rules = "<F2>GAME\\\\BATTLE TYPE: \n<F0>CvH Death Match.\n\n"   @        
               $missionName             @
               "\n\n<F2>RULES\\\\OBJECTIVES:\n" @
               "<F0>"@
           "1) CYBRIDS\\\\THE NEXT are assigned//allocated to RED team.\n" @
               "2) HUMANS\\\\ANIMALS are assigned//allocated to BLUE team.\n"@
               "3) Pilots\\\\Players who change vehicles\\\\warforms from one species to another\n"@
           "   will be forced to switch//alter teams.\n"@
           "4) All weapons, vehicles, padkilling, vulturing, etc. are LEGAL//ALLOWED.\n"@
           "5) Tech Mixing is off!  This is true Cybrid tech versus Human tech.\n"@
               "<tIDMULT_TDM_SCORING_1>"  @
               "<tIDMULT_TDM_SCORING_2>"  @
               $killPoints                @
               "<tIDMULT_TDM_SCORING_3>"  @
               "<tIDMULT_TDM_SCORING_4>"  @
               $deathPoints               @
               "<tIDMULT_TDM_SCORING_5>"  @
               "<tIDMULT_TDM_SCORING_6>"  @
               "<F2>SERVER RULES/INFORMATION:<F0>  This map was made by Temujin.  "      @ 
               "This and other maps, skins, and training materials can be downloaded from Tem's website:\n" @
            "   http://www6.50megs.com/temujin (yes, that really is www6!)\n\n";

   setGameInfo(%rules);      
}

// setup the rules - this has to be called after the definition of setRules

$server::AllowMixedTech =                            FALSE;



