using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public enum GameState
{
    Waiting,
    Starting,
    Playing,
    Celebrating
}

public class ClampedCurveAttribute : PropertyAttribute { }

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    public float celebrateDuration = 7f;
    public int startDuration = 5;

    public GameObject waiting;
    public GameObject starting;
    public GameObject playing;
    public GameObject celebrating;
    public GameObject pause;

    public GameObject confetti;
    public GameObject whale;

    public Player red;
    public Player blue;

    private bool paused;
    private float startTime;
    private GameState state = GameState.Waiting;

    private Player winner;

    public static GameObject Whale
    {
        get
        {
            if (!instance) instance = FindObjectOfType<GameManager>();

            return instance.whale;
        }
    }

    public static GameObject Confetti
    {
        get
        {
            if (!instance) instance = FindObjectOfType<GameManager>();

            return instance.confetti;
        }
    }

    public static Player Red
    {
        get
        {
            if (!instance) instance = FindObjectOfType<GameManager>();

            return instance.red;
        }
    }

    public static Player Blue
    {
        get
        {
            if (!instance) instance = FindObjectOfType<GameManager>();

            return instance.blue;
        }
    }

    public static bool Paused
    {
        get
        {
            if (!instance) instance = FindObjectOfType<GameManager>();

            return instance.paused;
        }
        set
        {
            if (!instance) instance = FindObjectOfType<GameManager>();

            instance.paused = value;
        }
    }

    public static GameState State
    {
        get
        {
            if (!instance) instance = FindObjectOfType<GameManager>();

            return instance.state;
        }
        set
        {
            if (!instance) instance = FindObjectOfType<GameManager>();

            instance.state = value;
        }
    }

    public static Player Winner
    {
        get
        {
            if (!instance) instance = FindObjectOfType<GameManager>();

            if (State != GameState.Celebrating) return null;

            return instance.winner;
        }
    }

    public static float StartTime
    {
        get
        {
            if (!instance) instance = FindObjectOfType<GameManager>();

            return instance.startTime;
        }
        set
        {
            if (!instance) instance = FindObjectOfType<GameManager>();

            instance.startTime = value;
        }
    }

    public static int StartDuration
    {
        get
        {
            if (!instance) instance = FindObjectOfType<GameManager>();

            return instance.startDuration;
        }
    }

    public static float CelebrateDuration
    {
        get
        {
            if (!instance) instance = FindObjectOfType<GameManager>();

            return instance.celebrateDuration;
        }
    }

    public static async void Celebrate(Player winner)
    {
        if (!instance) instance = FindObjectOfType<GameManager>();

        //spawn effect at winner
        GameObject go = Instantiate(Confetti, CameraManager.Transform.position, Quaternion.identity);
        Destroy(go, 10f);

        instance.winner = winner;
        State = GameState.Celebrating;
        Time.timeScale = 0.5f;

        //8 seconds later, go back to wait state
        int ms = Mathf.RoundToInt(instance.celebrateDuration * 1000f);
        await Task.Delay(ms);

        Time.timeScale = 1f;
        State = GameState.Waiting;

        //reset players
        ResetPlayers();
    }

    public static void ResetPlayers()
    {
        Player[] allPlayers = FindObjectsOfType<Player>();
        for (int i = 0; i < allPlayers.Length; i++)
        {
            allPlayers[i].Reset();
        }
    }

    private void Awake()
    {
        instance = this;
        State = GameState.Waiting;
    }

    private void OnEnable()
    {
        instance = this;
    }

    private void Update()
    {
        if (State == GameState.Starting)
        {
            StartTime += Time.deltaTime;
            if (StartTime > StartDuration)
            {
                State = GameState.Playing;
            }
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            ShakeAll();
        }

        ProcessWaiting();
        ProcessPause();

        if (waiting) waiting.SetActive(State == GameState.Waiting);
        if (starting) starting.SetActive(State == GameState.Starting);
        if (playing) playing.SetActive(State == GameState.Playing);
        if (celebrating) celebrating.SetActive(State == GameState.Celebrating);

        if (pause) pause.SetActive(Paused);
    }

    private void ProcessWaiting()
    {
        if (State != GameState.Waiting) return;

        bool wantsToPlay = Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space);
        if (!wantsToPlay)
        {
            bool allPressingX = true;
            for (int i = 0; i < JoyconManager.Joycons.Count; i++)
            {
                if (JoyconManager.Joycons[i].Type == Joycon.JoyconType.Left) continue;

                if (!JoyconManager.Joycons[i].X)
                {
                    allPressingX = false;
                    break;
                }
            }

            if (allPressingX && JoyconManager.Joycons.Count != 0)
            {
                wantsToPlay = true;
            }
        }

        if (wantsToPlay)
        {
            StartGame();
        }
    }

    public static void ShakeAll()
    {
        foreach (var joycon in JoyconManager.Joycons)
        {
            joycon.SetRumble(0f, 1f, 100f, 200);
        }
    }

    public void StartGame()
    {
        if (!instance) instance = FindObjectOfType<GameManager>();

        //reset players
        ResetPlayers();
        Paused = false;
        State = GameState.Starting;
        StartTime = 0f;
    }

    private void ProcessPause()
    {
        if (State != GameState.Playing) return;

        bool wantsToPause = Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Escape);
        if (wantsToPause)
        {
            Paused = !Paused;
        }

        Time.timeScale = Paused ? 0.00001f : 1f;
    }
}
