##--------------------------- Header
// FILE:        TagStdLib.cs
// AUTHORS:     ^TFW^ Wilzuun
// LAST MOD:    11 Aug, 2020
// VERSION:     1.0r
// NOTES:       One shot kills. Lasers only.
##--------------------------- Version History
//  1.0r
//      - Initial startup.

function Tag::vehicle::onAttacked(%destroyed, %destroyer)
{
    %a = playerManager::vehicleIdToPlayerNum(%destroyer);
    %b = playerManager::vehicleIdToPlayerNum(%destroyed);
    if (%a == 0) return;
    if (%b == 0) return;
    if (%a == %b) return;
    schedule("damageObject("@%destroyed@"",99999);",0.5");
}

function Tag::setRules()
{
    %rules = "<jc><f2>Welcome to Tag!<f0>\n<b0,5:table_head8.bmp><b0,5:table_head8.bmp><jl>\n\n<y17>";
    %rules = %rules @ "So you found yourself bored, and you got yourself a friend (I hope) and the two of you decide, the world needs a good game.\n";
    %rules = %rules @ "So you build yourself some rules, no sensors. Only lasers will work for this project. And off we go!\n";
    %rules = %rules @ "One shot kills! Find 'em before they find you!\n";

    setGameInfo(%rules);
    return %rules;
}

//  One shot kills! Find 'em before they find you!