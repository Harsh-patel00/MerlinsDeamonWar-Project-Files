using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOver : MonoBehaviour
{
	public Text deamonsKilledText;
	public Text scoreText;

	private void Start()
	{
		UpdateScore();
	}

	public void GameOverFunction()
	{
		SceneManager.LoadScene(0);
	}

	public void UpdateScore()
	{
		deamonsKilledText.text = "Deamons killed: " + GameController.instance.playerKills;
		scoreText.text = "Score: " + GameController.instance.playerScore;
	}


}
