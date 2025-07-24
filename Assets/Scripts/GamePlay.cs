using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

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

    private int computerMinGuess;
    private int computerMaxGuess;
    private List<int> computerGuesses;

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
            gameState.text = "<sprite=15> Please enter a valid number";
            return;
        }
        if (guess < minNumber || guess > maxNumber)
        {
            gameState.text = $"<sprite=15> Please enter a number between {minNumber} - {maxNumber}";
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
            gameLog.text += $"<sprite=1> {playerName} got it right\n";
            Endgame();
        }
        else if (currentAttemps >= maxAttemps)
        {
            //loser
            gameLog.text += $"<sprite=10> GameOver! The Correct number was {targetNumber}\n";
            Endgame();
        }
        else
        {
            //Wrong guess - give hint
            string hint = guess < targetNumber ? "Too Low" : "Too High";
            gameLog.text += $"<sprite=\"symbols-01\" index=24 >{hint}\n";

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
        if (computerGuesses.Count > 0)
        {
            int lastGuess = computerGuesses[computerGuesses.Count - 1];
            if (targetISHigher)
            {
                computerMinGuess = lastGuess + 1;
            }
            else
            {
                computerMaxGuess = lastGuess - 1;
            }
        }

        // AI use binary Search strategy
        int computerGuess  = (computerMinGuess + computerMaxGuess) / 2;

        computerGuesses.Add(computerGuess);


        //int computerGuess = Random.Range(minNumber, maxNumber + 1);
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

        computerMinGuess = minNumber;
        computerMaxGuess = maxNumber;
        computerGuesses = new List<int>();
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
