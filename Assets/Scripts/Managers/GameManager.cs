using UnityEngine;

[RequireComponent(typeof(InputManager))]
public class GameManager : MonoBehaviour
{
    public static InputManager InputManager;
    public static AudioManager AudioManager;
    
    void Awake()
    {
        InputManager = GetComponent<InputManager>();
        AudioManager = GetComponent<AudioManager>();
        //DontDestroyOnLoad(gameObject);
    }
}