using UnityEngine;

[RequireComponent(typeof(AudioManager)), RequireComponent(typeof(InputManager))]
public class GameManager : MonoBehaviour
{
    public static AudioManager AudioManager;
    public static InputManager InputManager;
    
    void Awake()
    {
        Debug.Log("Managers ONLINE!");

        InputManager = GetComponent<InputManager>();
        AudioManager = GetComponent<AudioManager>();
        //DontDestroyOnLoad(gameObject);
    }
}