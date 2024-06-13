using UnityEngine;

public class PotionImg : MonoBehaviour
{
    public Texture2D imageTexture;
    private float scale = 0.2f; 
    public PotionMakerScript potionMakerScript;
    private int fontSize = 40;
    
    void Start()
    {
        byte[] fileData = System.IO.File.ReadAllBytes("Assets/Potion.png");
        Texture2D tex = new Texture2D(2, 2);
        tex.LoadImage(fileData);
        imageTexture = tex;
    }

    void OnGUI()
    {
        float width = imageTexture.width * scale;
        float height = imageTexture.height * scale;
        GUI.DrawTexture(new Rect(50, 380, width, height), imageTexture);
        
        GUIStyle style = new GUIStyle(GUI.skin.label);
        style.fontSize = fontSize;
            
        GUI.Label(new Rect(110, 300 + height + 10, 400, 50), "" + PotionMakerScript.potionAmount, style); // Изменено здесь
    }
}