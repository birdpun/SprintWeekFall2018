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

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    public GameObject waiting;
    public GameObject starting;
    public GameObject playing;
    public GameObject celebrating;
    public GameObject pause;

    public GameObject whale;

    public Player red;
    public Player blue;

    private bool paused;

    [SerializeField]
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

    public static int CountdownSecond
    {
        get; set;
    }

    public static async void Celebrate(Player winner)
    {
        if (!instance) instance = FindObjectOfType<GameManager>();

        instance.winner = winner;
        State = GameState.Celebrating;
        Time.timeScale = 0.4f;

        //8 seconds later, go back to wait state
        await Task.Delay(8000);

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
            Paused = false;
            State = GameState.Starting;

            //reset players
            ResetPlayers();

            StartCountdown(5);
        }
    }

    public static void ShakeAll()
    {
        foreach (var joycon in JoyconManager.Joycons)
        {
            joycon.SetRumble(0f, 1f, 100f, 200);
        }
    }

    private async void StartCountdown(int seconds)
    {
        CountdownSecond = seconds;
        for (int i = 0; i < seconds; i++)
        {
            ShakeAll();
            await Task.Delay(1000);
            CountdownSecond--;
        }

        CountdownSecond = 0;
        State = GameState.Playing;
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
