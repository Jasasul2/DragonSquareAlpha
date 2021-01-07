using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System; 

public class NameGenerator : MonoBehaviour
{
    public enum MonsterTypes
    {
        AngryBall,
        PhasingBall
    }

    public static string[] normalNames = {
        "Thomas",
        "Felix",
        "James",
        "John",
        "Richard",
        "Gaben",
        "Peter",
        "Randall",
        "Daniel",
        "Zori",
        "Anakin",
        "Thompson",
        "Eduard",
        "Indigo",
        "Janette",
        "Juan",
        "Pedro",
        "Obelix",
        "Kurvafix",
        "Adam",
        "David"
    };

    public static string[] angryBallNames = {
        "Hokulk",
        "Xomath",
        "Xaguk",
        "Opoguk",
        "Brag",
        "Suhgan",
        "Zurgug",
        "Grikug",
        "Quimghig",
        "Zugorim",
        "Garakh",
        "Ghorza",
        "Rolfish",
        "Burzob",
        "Dura",
        "Gonk",
        "Bula",
        "Ghob",
        "Ulumpha",
        "Ushat",
        "Uzul",
        "Swamp",
        "Svax",
        "Jagga"
    };

    public static string[] nameSuffixes = {
        " The Angry Ball",
        " The Phasing Ball"

    };

    public static Dictionary<MonsterTypes, string[]> monsterNameDict = new Dictionary<MonsterTypes, string[]> { { MonsterTypes.AngryBall, angryBallNames }, { MonsterTypes.PhasingBall, angryBallNames } };
    public static Dictionary<MonsterTypes, string> monsterNameSuffixes = new Dictionary<MonsterTypes, string> { { MonsterTypes.AngryBall, nameSuffixes[0] }, { MonsterTypes.PhasingBall, nameSuffixes[1] } };

    public static string GetEnemyName(MonsterTypes monsterType)
    {
        int nameAlgorithm = UnityEngine.Random.Range(0, 3);
        string result = "";

        string name1 = normalNames[UnityEngine.Random.Range(0, normalNames.Length)];
        string name2 = monsterNameDict[monsterType][UnityEngine.Random.Range(0, angryBallNames.Length)];
        char[] charArr1;
        char[] charArr2;

        if (nameAlgorithm == 0) //just the name 
        { 
            result = name2;
            if (UnityEngine.Random.Range(0, 2) == 1)
            { 
                name2 = Reverse(name2).ToString();
                name2 = char.ToUpper(name2[0]) + name2.Substring(1);
            }
        }

        else 
        {
            int type = UnityEngine.Random.Range(0, 4);
            charArr1 = name1.ToCharArray();
            charArr2 = name2.ToCharArray();
            if (type == 1 || type == 3)
            {
                charArr1 = Reverse(name1);
            }
            if(type == 2 || type == 3)
            {
                charArr2 = Reverse(name2);
            }
            if (UnityEngine.Random.Range(0, 2) == 0)
            {
                result = Combine(charArr1, charArr2, UnityEngine.Random.Range(1.5f, 3.25f));
            }
            else
                result = Combine(charArr2, charArr1, UnityEngine.Random.Range(1.5f, 3.25f));
        }
        string suffix = monsterNameSuffixes[monsterType];
        result += suffix;
        return result;
    }

    private static string Combine(char[] array1, char[] array2, float div = 2)
    {
        string combination = "";
        int shorter = array1.Length;
        if (array2.Length < shorter)
            shorter = array2.Length;
        for (int i = 0; i < Mathf.Ceil(shorter / div); i++)
        {
            if (i != 0)
            { 
                combination += array2[i].ToString().ToLower();
            }
            else
                combination += array2[i].ToString().ToUpper();
            combination += array1[i].ToString().ToLower();
        }
        return combination; 
    }

    public static char[] Reverse(string s)
    {
        char[] charArray = s.ToLower().ToCharArray();
        Array.Reverse(charArray);
        return charArray;
    }
}
