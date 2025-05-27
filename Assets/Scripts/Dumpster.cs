using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class JunkItem
{
    public string name;
    public int scoreValue;
    public string description;
}// Class JunkItem

public class Dumpster : MonoBehaviour
{

    public List<JunkItem> level1Junk = new List<JunkItem>()
{
    new JunkItem { name = "JuiceBox", scoreValue = 1, description = "Half full. Questionably drinkable." },
    new JunkItem { name = "Toilet Paper Roll", scoreValue = 1, description = "Completely empty. Completely useless." },
    new JunkItem { name = "Banana Peel", scoreValue = 2, description = "It’s been stepped on. Twice." },
    new JunkItem { name = "Dumpster Dash Bible Folder", scoreValue = 3, description = "The original design doc. Glows faintly." },
    new JunkItem { name = "Broken Flip-Flop", scoreValue = 1, description = "Only one. Size unknown." },
    new JunkItem { name = "Crushed Soda Can", scoreValue = 2, description = "Still has one sip. Or backwash." },
    new JunkItem { name = "Melted Crayon", scoreValue = 1, description = "Fusion of colors and regret." },
    new JunkItem { name = "Fast Food Receipt", scoreValue = 0, description = "Ink is gone but shame remains." },
    new JunkItem { name = "Bent Plastic Fork", scoreValue = 1, description = "Could stab an enemy... once." },
    new JunkItem { name = "Game Cartridge", scoreValue = 3, description = "No label. Possibly cursed." },
    new JunkItem { name = "Soggy Business Card", scoreValue = 1, description = "'Jeff. Solutions.' What kind of solutions?" },
    new JunkItem { name = "Hairy Lollipop Stick", scoreValue = 1, description = "Don't. Just don't." },
    new JunkItem { name = "Free Hugs Sign", scoreValue = 1, description = "Still sticky. Still hopeful." },
    new JunkItem { name = "Hole-Ridden Sock", scoreValue = 1, description = "Its partner left years ago." }
};

    public JunkItem GetRandomJunk()
    {
        int index = UnityEngine.Random.Range(0, level1Junk.Count);
        return level1Junk[index];
    }

}//Class Dumpster
