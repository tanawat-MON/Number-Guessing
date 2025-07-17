using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class GamePlay : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI attempsLeft;
    public TextMeshProUGUI currentPlayer;
    public TextMeshProUGUI gameState;
    public TextMeshProUGUI gameLog;

    [Header("Input")]
    public TMP_InputField guessInputField;
    public Button submitButton;
    public Button newgameButton;

    [Header("Game Setting")]
    public int minNumber = 1;
    public int maxNumber = 100;
    public int maxAttemps = 12;

    private int targetNumber;
    private int currentAttemps;
    private bool isPlayerTurn;
    private bool gameActive;

    void InitializeUI()
    {
        submitButton.onClick.AddListener(SubmitGuess);
        newgameButton.onClick.AddListener(StartNewGame);
        guessInputField.onSubmit.AddListener(delegate { SubmitGuess(); });
    }

    void SubmitGuess()
    {
        if (!gameActive || !isPlayerTurn) return;

        string input = guessInputField.text.Trim();
        if (string.IsNullOrEmpty(input)) 
           return;

        int guess;
        if (!int.TryParse(input, out guess))
        {
            gameState.text = "Please enter a valid number";
            return;
        }
        if (guess < minNumber || guess > maxNumber)
        {
            gameState.text = $"Please enter a number between {minNumber} - {maxNumber}";
            return; 
        }
        ProcessGuess(guess, true);
        guessInputField.text = "";
    }

    void ProcessGuess(int guess, bool isPlayerTurn)
    {
        currentAttemps++;
        string playerName = isPlayerTurn ? "Player" : "Computer";

        gameLog.text += $"{playerName} guess: {guess}\n";

        if (guess == targetNumber)
        {
            //Win
            gameLog.text += $"{playerName} got it right\n";
            Endgame();
        }
        else if (currentAttemps >= maxAttemps)
        {
            //loser
            gameLog.text += $"GameOver! The Correct number was {targetNumber}\n";
            Endgame();
        }
        else
        {
            //Wrong guess - give hint
            string hint = guess < targetNumber ? "Too Low" : "Too High";
            gameLog.text += $"{hint}\n";

            //Switch players
            isPlayerTurn = !isPlayerTurn;
            currentPlayer.text = isPlayerTurn ? "Plyer" : "computer";
            attempsLeft.text = $"Attemps Letf: {maxAttemps - currentAttemps}";

            if (!isPlayerTurn) 
            {
                guessInputField.interactable = false;
                submitButton.interactable = false;
                StartCoroutine(ComputerTurn(guess < targetNumber));
            }
            else
            {
                guessInputField.interactable= true;
                submitButton.interactable= true;
                guessInputField.text = " ";
                guessInputField.Select();
                guessInputField.ActivateInputField();
            }
        }
    }

    IEnumerator ComputerTurn(bool targetISHigher)
    {
        yield return new WaitForSeconds(2f); //wait to simulate thinking
        if (!gameActive) yield break;
        int computerGuess = Random.Range(minNumber, maxNumber + 1);
        ProcessGuess(computerGuess, false);
    }
  
    void Endgame()
    {
        gameActive = false;
        guessInputField.interactable = false;
        submitButton.interactable = false;
        currentPlayer.text = "";
        gameState.text = "Game Over - Click New Game to state again";
        Canvas.ForceUpdateCanvases();
    }
    
    void StartNewGame()
    {
        targetNumber = Random.Range(minNumber, maxNumber + 1);
        currentAttemps = 0;
        isPlayerTurn = true;
        gameActive = true;

        currentPlayer.text = "Player Turn";
        attempsLeft.text = $"Attemps Left: {maxAttemps}";
        gameLog.text = "=== Game Log ===\n";
        gameState.text = "new game started! Player goes first";

        guessInputField.interactable = true;
        submitButton.interactable = true;   
        guessInputField.text = "";
        guessInputField.Select();
        guessInputField.ActivateInputField();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InitializeUI();
        StartNewGame();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
