##--------------------------- Header
// FILE:        tkStdLib.cs
// AUTHORS:     ^TFW^ Wilzuun
// LAST MOD:    29 Sep, 2020
// VERSION:     1.0r
// NOTES:       Targeting Game. Target some one and die. Targeting Kills
//              This file was created, tested, and modified in a day.
##--------------------------- Version History
//  1.0r
//      - Initial startup.
//  2.0
//      - Organized code into categories.
//      - Added inMissionStart and inMissionEnd functions.

## -------------------------------------------------------------------- inMission
// This game type actually doesn't require any special activity to setup, or exit
// from. So these are here solely for if the game type gets updated.
function tk::inMissionStart() 
{
    // set the allowed items.
    tk::setAllowedItems();
    // set the rules.
    tk::setrules();
    // set mission name to reflect new game type.
    $missionName = "TK - Dynamic Games";
    // announce new game type.
    say(0,0,"TARGETING KILLS now in effect!");
}
function tk::inMissionEnd() {}


## -------------------------------------------------------------------- Vehicle
function tk::vehicle::onTargeted(%targeted, %targeter)
{
    if($gameTypes[1] == "tdm")
    {
        if (getTeam(%targeted) != getTeam(%targeter))
        {
            damageObject(%targeter, 9876543210);
        }
    }
    else
    {
        damageObject(%targeter, 9876543210);
    }
}

function tk::vehicle::onScan(%d, %r)
{
    if (getTeam(%targeted) == getTeam(%targeter))
    {
        healObject(%d, 9876543210);
    }
}

## -------------------------------------------------------------------- Server
function tk::setRules()
{
    %rules = "<jc><f2>Welcome to Targeting Kills!<f0>\n<b0,5:table_head8.bmp><b0,5:table_head8.bmp><jl>\n\n<y17>";
    %rules = %rules @ "If you target an enemy, you will die.\n";
    %rules = %rules @ "Player with most kills, wins!\n";

    setGameInfo(%rules);
    return %rules;
}

function tk::setAllowedItems()
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
