using UnityEngine;
using UnityEngine.InputSystem;

public class SimpleGravityUI : MonoBehaviour
{
    private GravityField gravityField;
    private int projectileCount = 0;
    
    // New Input System
    private Keyboard keyboard;
    
    void Start()
    {
        gravityField = FindObjectOfType<GravityField>();
        keyboard = Keyboard.current;
    }
    
    void Update()
    {
        if (keyboard == null) return;
        
        projectileCount = GameObject.FindGameObjectsWithTag("Projectile").Length;
        
        // Space - 일시정지
        if (keyboard.spaceKey.wasPressedThisFrame)
        {
            Time.timeScale = Time.timeScale > 0 ? 0 : 1;
        }
        
        // Q/E - 중력 조절
        if (gravityField != null)
        {
            if (keyboard.qKey.isPressed)
            {
                gravityField.SetGravityStrength(gravityField.GetGravityStrength() - 50f * Time.deltaTime);
            }
            if (keyboard.eKey.isPressed)
            {
                gravityField.SetGravityStrength(gravityField.GetGravityStrength() + 50f * Time.deltaTime);
            }
        }
        
        // 1-4 숫자키 - 시간 배속
        if (keyboard.digit1Key.wasPressedThisFrame) Time.timeScale = 0.5f;
        if (keyboard.digit2Key.wasPressedThisFrame) Time.timeScale = 1f;
        if (keyboard.digit3Key.wasPressedThisFrame) Time.timeScale = 2f;
        if (keyboard.digit4Key.wasPressedThisFrame) Time.timeScale = 4f;
    }
    
    void OnGUI()
    {
        GUIStyle boxStyle = new GUIStyle(GUI.skin.box);
        boxStyle.fontSize = 12;
        boxStyle.normal.textColor = Color.white;
        boxStyle.normal.background = MakeTexture(1, 1, new Color(0.2f, 0.2f, 0.2f, 0.8f));
        
        GUILayout.BeginArea(new Rect(10, 10, 250, 150));
        GUILayout.Box("== 중력장 컨트롤 ==", boxStyle);
        
        if (gravityField != null)
        {
            GUILayout.Label($"중력 강도: {gravityField.GetGravityStrength():F0}");
            float newStrength = GUILayout.HorizontalSlider(
                gravityField.GetGravityStrength(), 10f, 200f);
            gravityField.SetGravityStrength(newStrength);
            
            GUILayout.Label($"중력 반경: {gravityField.GetGravityRadius():F1}");
            float newRadius = GUILayout.HorizontalSlider(
                gravityField.GetGravityRadius(), 5f, 30f);
            gravityField.SetGravityRadius(newRadius);
        }
        
        GUILayout.Space(10);
        GUILayout.Label($"발사체: {projectileCount}개");
        GUILayout.Label($"시간 배속: {Time.timeScale:F1}x");
        
        GUILayout.EndArea();
        
        // 조작법
        GUILayout.BeginArea(new Rect(10, 170, 250, 140));
        GUILayout.Box("== 조작법 ==", boxStyle);
        GUILayout.Label("마우스 드래그: 발사");
        GUILayout.Label("R: 모든 발사체 제거");
        GUILayout.Label("Space: 일시정지");
        GUILayout.Label("Q/E: 중력 강도 조절");
        GUILayout.Label("1-4: 시간 배속 조절");
        GUILayout.Label("ESC: 발사 취소");
        GUILayout.EndArea();
    }
    
    private Texture2D MakeTexture(int width, int height, Color color)
    {
        Color[] pixels = new Color[width * height];
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = color;
        }
        
        Texture2D texture = new Texture2D(width, height);
        texture.SetPixels(pixels);
        texture.Apply();
        
        return texture;
    }
}