using UnityEngine;
using System.Collections;
using System;

[CreateAssetMenu(fileName = "SpriteHolder", menuName = "Sprites/SpriteHolder", order = 1)]
public class SpriteHolder : ScriptableObject
{
    public Sprite[] spritesToLoad;
    public int[] prices;

    public void OnValidate()
    {
        prices = new int[spritesToLoad.Length];
        for(int i = 0; i < spritesToLoad.Length; i++)
        {
            int x = 5 + i;
            prices[i] = x * x + 100;
        }
    }

    public Sprite GetSprite(string name)
    {
        foreach(Sprite s in spritesToLoad)
        {
            if (s.name.Equals(name))
                return s;
        }
        throw new ArgumentException("Sprite not found");
    }

    public int GetSpriteIndex(string name)
    {
        for(int i = 0; i < spritesToLoad.Length; i++)
        {
            if (spritesToLoad[i].name.Equals(name))
                return i;
        }
        throw new ArgumentException("Sprite not found");
    }
}