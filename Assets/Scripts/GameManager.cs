using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    public Dice dice;
    public Camera mainCam;

    [Serializable]
    public struct RoundScoreUI
    {
        public Text[] scoreTexts;
    }

    [Serializable]
    public struct ScoreTableUI
    {
        public Text player1Text;
        public Text player2Text;
        public RoundScoreUI[] roundScoreText;
    }
    public ScoreTableUI scoreTable;
    public Text deskUI;

    public Button continueBtn;
    public Button playAgainBtn;
    public Button rerollBtn;

    int player1wins = 0; 
    int player2wins = 0;
    int[,] score = new int[2,3];

    int currentPlayerId = 1;
    int currentRound = 1;
    bool diceReady = false;


    private void Start()
    {
        SetupGame();
    }

    private void Update()
    {
        if (diceReady)
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = Camera.main.nearClipPlane + dice.size;
            Vector3 worldPoint = mainCam.ScreenToWorldPoint(mousePos);
            dice.transform.position = worldPoint;

            if (Input.GetMouseButtonDown(0))
            {
                StartCoroutine(RunTrial(currentPlayerId));
            }
        }
    }

    void SetupGame()
    {
        playAgainBtn.gameObject.SetActive(false);
        SetupRound(1);
    }

    void SetupRound(int round)
    {
        currentPlayerId = 1;
        SetupTrial(1);
    }

    void SetupTrial(int playerId)
    {
        if (currentPlayerId == 1) deskUI.text = scoreTable.player1Text.text;
        if (currentPlayerId == 2) deskUI.text = scoreTable.player2Text.text;

        continueBtn.gameObject.SetActive(false);
        dice.ResetDice();
        diceReady = true;
    }

    IEnumerator RunTrial(int playerId)
    {
        diceReady = false;
        yield return StartCoroutine(dice.Throw());

        if(dice.currentValue == -1)
        {
            deskUI.text = "Fail";
            rerollBtn.gameObject.SetActive(true);
        }
        else
        {
            AssignPoints(playerId, dice.currentValue);
            continueBtn.gameObject.SetActive(true);
        }
    }

    // Called from UI
    public void SetupReroll()
    {
        SetupTrial(currentPlayerId);
        rerollBtn.gameObject.SetActive(false);
    }

    // Called from UI
    public void FinishTrial()
    {
        if (currentPlayerId == 1)
        {
            currentPlayerId++;
            SetupTrial(currentPlayerId);
        }
        else if (currentPlayerId == 2)
        {
            FinishRound();
        }
    }

    private void FinishRound()
    {
        AssignWins();

        if (currentPlayerId == 2 && currentRound == 3)
        {
            FinishGame();
        }
        else if (player1wins == 2 || player2wins == 2)
        {
            FinishGame();
        }
        else
        {
            currentRound++;
            SetupRound(currentRound);
        }

    }

    private void AssignWins()
    {
        if (score[0, currentRound - 1] > score[1, currentRound - 1])
        {
            scoreTable.roundScoreText[currentRound - 1].scoreTexts[0].text = "<color=green>" + score[0, currentRound - 1].ToString() + "</color>";
            scoreTable.roundScoreText[currentRound - 1].scoreTexts[1].text = "<color=red>" + score[1, currentRound - 1].ToString() + "</color>";
            player1wins++;
        }
        if (score[0, currentRound - 1] < score[1, currentRound - 1])
        {
            scoreTable.roundScoreText[currentRound - 1].scoreTexts[0].text = "<color=red>" + score[0, currentRound - 1].ToString() + "</color>";
            scoreTable.roundScoreText[currentRound - 1].scoreTexts[1].text = "<color=green>" + score[1, currentRound - 1].ToString() + "</color>";
            player2wins++;
        }
    }

    private void AssignPoints(int playerId, int value)
    {
        score[playerId - 1, currentRound - 1] = value;
        scoreTable.roundScoreText[currentRound - 1].scoreTexts[playerId - 1].text = value.ToString();
        deskUI.text = "<size=400>" + value.ToString() + "</size>";
    }

    public void FinishGame()
    {
        continueBtn.gameObject.SetActive(false);
        dice.gameObject.SetActive(false);
        playAgainBtn.gameObject.SetActive(true);

        deskUI.text = scoreTable.player1Text.text + ":" + "<color=green>" + player1wins.ToString() + "</color>";
        deskUI.text += "\n";
        deskUI.text += scoreTable.player2Text.text + ":" + "<color=green>" + player2wins.ToString() + "</color>";
        deskUI.text += "\n" + "<b>";

        if(player1wins > player2wins)
            deskUI.text += scoreTable.player1Text.text + " won!";
        else if (player1wins < player2wins)
            deskUI.text += scoreTable.player2Text.text + " won!";
        else
            deskUI.text += "DRAW";

        deskUI.text += "</b>";
    }

    // Called from UI
    public void ResetScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
