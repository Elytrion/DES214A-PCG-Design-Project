using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using UnityEngine.UI;
using TMPro;

public class CheatCodeManager : MonoBehaviour
{
    // avoid wasd
    public Player PlayerScript;
    public PickupText PickupTextScript;

    public string TeleportCheatCode = "tp";
    public string GodmodeCheatCode = "nohit";
    public string MoneyCheatCode = "mmm";
    public string AllGunsCheatCode = "idkfa";
    public string InfiniteDmg = "boom";

    public GameObject[] Guns;

    private const int MaxInputLength = 24;
    private StringBuilder _currentInput;

    private bool _trackInput = false;
    private bool hasInfiniteDmg = false;
    private bool isGodmodeOn = false;

    public GameObject ConsoleDisplay;
    public TextMeshProUGUI ConsoleText;

    private void Start()
    {
        _currentInput = new StringBuilder(MaxInputLength);
        ConsoleDisplay.SetActive(false);
    }

    private void Update()
    {
        if (isGodmodeOn && !PlayerScript.NoDamage)
        {
            PlayerScript.NoDamage = true;
        }


        // Check if the tab key is pressed
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ResetInput();
            return;
        }

        if (Input.GetKeyDown(KeyCode.F3))
        {
            PlayerScript.NoDamage = !PlayerScript.NoDamage;
            if (PlayerScript.NoDamage)
            {
                PickupTextScript.DisplayPickup("Godmode enabled");
            }
            else
            {
                PickupTextScript.DisplayPickup("Godmode disabled");
            }
            PlayerScript.Health = PlayerScript.MaxHealth;
            ResetInput();
            return;
        }

        if (Input.GetKeyDown(KeyCode.Slash) && !_trackInput)
        {
            ConsoleDisplay.SetActive(true);
            ConsoleText.text = "";
            _trackInput = true;
        }

        if (!_trackInput)
            return;
        
        CheckForInput();


        if (Input.GetKeyDown(KeyCode.Return))
        {
            CheckForCheatCode();
        }
    }

    private void CheckForInput()
    {
        // Check for a-z keys
        for (int i = (int)KeyCode.A; i <= (int)KeyCode.Z; i++)
        {
            if (Input.GetKeyDown((KeyCode)i))
            {
                AddInput(((KeyCode)i).ToString());
            }
        }

        // backspace check
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            if (_currentInput.Length > 0)
            {
                _currentInput.Remove(_currentInput.Length - 1, 1);
                ConsoleText.text = _currentInput.ToString();
            }
            if (_currentInput.Length == 0)
            {
                ResetInput();
            }
        }
    }

    private void AddInput(string input)
    {
        // Don't allow more than MaxInputLength characters
        if (_currentInput.Length >= MaxInputLength)
        {
            return;
        }

        // Add the input to the current input string
        _currentInput.Append(input.ToLower()); // Make it lowercase to be case insensitive
        ConsoleText.text = _currentInput.ToString();
    }

    private void ResetInput()
    {
        ConsoleDisplay.SetActive(false);
        _currentInput.Clear();
        _trackInput = false;
    }

    private void CheckForCheatCode()
    {
        if (_currentInput.ToString() == TeleportCheatCode)
        {
            PlayerScript.transform.position = new Vector3(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y, 0f);
            PickupTextScript.DisplayPickup("TELEPORTED!", 1.0f);
            ResetInput();
            return;
        }

        if (_currentInput.ToString() == GodmodeCheatCode)
        {
            PlayerScript.NoDamage = !PlayerScript.NoDamage;
            isGodmodeOn = !isGodmodeOn;
            if (PlayerScript.NoDamage)
            {
                PickupTextScript.DisplayPickup("Godmode enabled");
            }
            else
            {
                PickupTextScript.DisplayPickup("Godmode disabled");
            }
            PlayerScript.Health = PlayerScript.MaxHealth;
            ResetInput();
            return;
        }

        if (_currentInput.ToString() == MoneyCheatCode)
        {
            PlayerScript.GetComponent<Wallet>().AddCoins(100);
            PickupTextScript.DisplayPickup("Money +100");
            ResetInput();
            return;
        }

        if (_currentInput.ToString() == AllGunsCheatCode)
        {
            foreach (GameObject gun in Guns)
            {
                Vector3 offset = new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), 0);
                Instantiate(gun, PlayerScript.transform.position + offset, Quaternion.identity);
            }
            PickupTextScript.DisplayPickup("Give all weapons");
            ResetInput();
            return;
        }

        if (_currentInput.ToString() == InfiniteDmg)
        {
            hasInfiniteDmg = !hasInfiniteDmg;
            InstancedData playerData = PlayerScript.GetComponent<InstancedData>();
            if (hasInfiniteDmg)
            {
                if (playerData != null)
                {
                    playerData.SetID<int>("playerDamageOG", PlayerScript.playerDamage);
                }
                PlayerScript.playerDamage = 99;
                PickupTextScript.DisplayPickup("Infinite damage");
            }
            else
            {
                int ogplayerdmg = 1;
                if (playerData != null)
                {
                    ogplayerdmg = playerData.GetID<int>("playerDamageOG");
                }
                PlayerScript.playerDamage = ogplayerdmg;
                PickupTextScript.DisplayPickup("Damage reset");
            }
            ResetInput();
            return;

        }
    }
}
