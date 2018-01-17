using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class GameVersion
{
    public enum T
    {
        Integrated,
        NotIntegrated
    }

    private static readonly Regex sWhitespace = new Regex(@"\s+");

    public static string RemoveWhitespace(string input) {
        return sWhitespace.Replace(input, "");
    }

    private static string Normalize(string name)
    {
        return RemoveWhitespace(name.ToLower());
    }

    public static bool ValidID(string name)
    {
        return Map.ContainsKey(Normalize(name));
    }

    public static T GetVersion(string name)
    {
        name = Normalize(name);
        if (Map.ContainsKey(name))
        {
            switch (Map[name].Trim())
            {
                case "N": return T.NotIntegrated;
                case "I": return T.Integrated;
                default:
                    Debug.Log("Bad version: " + Map[name].Trim());
                    return T.Integrated;
            }
        } else
        {
            Debug.Log("Bad name: " + name);
            return T.Integrated;
        }


    }

    #region Map
    private static Dictionary<string, string> Map = new Dictionary<string, string>()
    {
        // Class Name  Class Size  Grade
        // Class A 18  2nd
        // Class B 16  2nd
        // Class C 16  2nd
        // Class D 19  2nd
        // Class E 21  3rd
        // Class F 20  3rd
        // Class G 20  3rd

        // Class total: 18	2nd Grade
        // CLASS A

        #region Class A
        {"redpig", "N"},
        {"yellowfrog", "N"},
        {"pinkfish", "  N"},
        {"orangebear", "N"},
        {"goldbunny", " N"},
        {"orangefrog", "N"},
        {"yellowgoat", "N"},
        {"redfish", "   N"},
        {"golddog", "   N"},
        {"whitebird", " I"},
        {"purplegoat", "I"},
        {"bluelion", "  I"},
        {"greenbird", " I"},
        {"blackhorse", "I"},
        {"purplebear", "I"},
        {"blackfrog", " I"},
        {"greentiger", "I"},
        {"whitelion", " I"},
        {"silverfish", "N"},
        {"silvergoat", "I"},
        #endregion /* Class A */

        // Class total: 16  2nd Grade
        // CLASS B
        #region Class B
        {"redcow", "N"},
        {"yellowdog", " N"},
        {"pinktiger", " N"},
        {"goldcat", "   N"},
        {"orangegoat", "N"},
        {"pinkdog", "   N"},
        {"redcat", "N"},
        {"orangefish", "N"},
        {"whitehorse", "I"},
        {"blackbird", " I"},
        {"purplebunny", "   I"},
        {"bluecow", "   I"},
        {"greenlion", " I"},
        {"greenpig", "  I"},
        {"purplefish", "I"},
        {"whitegoat", " I"},
        {"silverbear", "N"},
        {"silvertiger", "   I"},
        #endregion Class B

        // Class total: 16 2nd Grade
        // CLASS C
        #region Class C
        {"redbunny", "  N"},
        {"yellowfish", "N"},
        {"orangehorse", "   N"},
        {"goldpig", "   N"},
        {"goldbear", "  N"},
        {"pinklion", "  N"},
        {"redtiger", "  N"},
        {"yellowbunny", "   N"},
        {"purplehorse", "   I"},
        {"whitebear", " I"},
        {"blackgoat", " I"},
        {"purplefrog", "I"},
        {"bluebird", "  I"},
        {"greenhorse", "I"},
        {"bluegoat", "  I"},
        {"blackbear", " I"},
        {"silverlion", "N"},
        {"silverbird", "I"},
        #endregion Class C


        // Class total: 19 2nd Grade
        // CLASS D
        #region Class D
        {"redhorse", "  N"},
        {"yellowbird", "N"},
        {"orangebird", "N"},
        {"goldcow", "   N"},
        {"redbear", "   N"},
        {"yellowcow", " N"},
        {"pinkgoat", "  N"},
        {"goldfish", "  N"},
        {"orangecat", " N"},
        {"purplecow", " I"},
        {"blackbunny", "I"},
        {"whitefrog", " I"},
        {"bluetiger", " I"},
        {"bluecat", "   I"},
        {"greendog", "  I"},
        {"purplecat", " I"},
        {"blacklion", " I"},
        {"whitefish", " I"},
        {"purpletiger", "   I"},
        {"silverbunny", "   N"},
        {"silerfrog", " I"},
        #endregion /* Class D */

        // Class total: 21 3rd Grade
        // CLASS E
        #region Class E
        {"orangedog", " N"},
        {"yellowbear", "N"},
        {"redgoat", "   N"},
        {"yellowcat", " N"},
        {"goldbird", "  N"},
        {"goldtiger", " N"},
        {"pinkfrog", "  N"},
        {"redfrog", "   N"},
        {"pinkcow", "   N"},
        {"orangetiger", "   N"},
        {"orangepig", " N"},
        {"blackcat", "  I"},
        {"whitetiger", "I"},
        {"purpledog", " I"},
        {"bluehorse", " I"},
        {"greenfrog", " I"},
        {"greengoat", " I"},
        {"blackpig", "  I"},
        {"whitecat", "  I"},
        {"purplepig", " I"},
        {"bluebear", "  I"},
        {"silvercow", " N"},
        {"silverhorse", "   I"},
        #endregion /* Class E */


        // Class total: 20 3rd Grade
        // CLASS F
        #region Class F
        {"reddog", "N"},
        {"yellowpig", " N"},
        {"goldgoat", "  N"},
        {"pinkbear", "  N"},
        {"yellowhorse", "   N"},
        {"redlion", "   N"},
        {"yellowtiger", "   N"},
        {"pinkbird", "  N"},
        {"goldfrog", "  N"},
        {"orangebunny", "   N"},
        {"blackdog", "  I"},
        {"whitebunny", "I"},
        {"blacktiger", "I"},
        {"purplelion", "I"},
        {"greenfish", " I"},
        {"bluefrog", "  I"},
        {"whitepig", "  I"},
        {"bluebunny", " I"},
        {"greencow", "  I"},
        {"blackcow", "  I"},
        {"silverpig", " N"},
        {"silvercat", " I"},
        #endregion /* Class F */

        // Class total: 20 3rd Grade
        // CLASS G
        #region Class G
        {"redbird", "   N"},
        {"yellowlion", "N"},
        {"pinkbunny", " N"},
        {"goldlion", "  N"},
        {"pinkcat", "   N"},
        {"pinkhorse", " N"},
        {"goldhorse", " N"},
        {"pinkpig", "   N"},
        {"orangecow", " N"},
        {"orangelion", "N"},
        {"blackfish", " I"},
        {"whitedog", "  I"},
        {"purplebird", "I"},
        {"bluefish", "  I"},
        {"greenbear", " I"},
        {"whitecow", "  I"},
        {"bluedog", "   I"},
        {"bluepig", "   I"},
        {"greencat", "  I"},
        {"greenbunny", "I"},
        {"silvermoose", "   N"},
        {"silversnake", "   I"},
        #endregion /* Class G */
    };
    #endregion /* Map */

}
