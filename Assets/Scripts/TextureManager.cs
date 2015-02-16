using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TextureManager : MonoBehaviour {

    private static int TEXTURE_SIZE = 64;
    private static Color32 DEFAULT_COLOUR = new Color32(0, 0, 0, 255);

    private static Dictionary<int, Texture2D> cachedTextures;

	// Use this for initialization
	void Start () {
        cachedTextures = new Dictionary<int, Texture2D>();

        Color32 a = new Color32(255, 255, 255, 255);
        Color32 b = new Color32(255, 255, 255, 255);
        Color32 c = new Color32(255, 255, 255, 255);
	}

    public static Texture2D GetTexture(Color32 first)
    {
        return GetTexture(first, DEFAULT_COLOUR, DEFAULT_COLOUR);
    }

    public static Texture2D GetTexture(Color32 first, Color32 second)
    {
        return GetTexture(first, second, DEFAULT_COLOUR);
    }

    public static Texture2D GetTexture(Color32 first, Color32 second, Color32 third)
    {
        int key = GetHashCode(first, second, third);
        
        Texture2D cached = cachedTextures[key];
        if (cached != null)
        {
            return cached;
        }

        Color32[] colours = {
                                first,
                                second,
                                third,
                                DEFAULT_COLOUR
                            };


        // texture not cached, generate it!
        Texture2D t = new Texture2D(TEXTURE_SIZE, TEXTURE_SIZE);
        t.SetPixels32(colours, TEXTURE_SIZE / 2);

        return t;
    }

    private static int GetHashCode(Color32 first, Color32 second, Color32 third)
    {
        return (first.ToString() + second.ToString() + third.ToString()).GetHashCode();
    }
	
}
