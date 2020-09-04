using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    static public GameController instance = null;

    public Deck playerDeck = new Deck();
    public Deck enemyDeck = new Deck();

    public Hand playerHand = new Hand();
    public Hand enemyHand = new Hand();

    public Player player = null;
    public Player enemy = null;

    public List<CardData> cards = new List<CardData>();

    public Sprite[] healthNumbers = new Sprite[10];
    public Sprite[] damangeNumbers = new Sprite[10];

    public GameObject cardPrefab = null;
    public Canvas canvas = null;

    public bool isPlayable = false;

    public GameObject effectFromLeftPrefab = null;
    public GameObject effectFromRightPrefab = null;

    public Sprite fireBallImage = null;
    public Sprite iceBallImage = null;
    public Sprite multiFireBallImage = null;
    public Sprite multiIceBallImage = null;
    public Sprite iceAndFireBallImage = null;
    public Sprite destroyBallImage = null;

    public bool playersTurn = true;


    public Image enemySkipTurnImage = null;
    
    public Text turnText = null;
    public Text scoreText = null;

    public int playerScore = 0;
    public int playerKills = 0;


    public Sprite fireDeamon = null;
    public Sprite iceDeamon = null;

    public AudioSource playerDieAudio = null;
    public AudioSource enemyDieAudio = null;

    public void Awake()
    {
        instance = this;

        SetUpEnemy();

        playerDeck.Create();
        enemyDeck.Create();

        StartCoroutine(DealHands());
    }

    public void Quit()
    {
        SceneManager.LoadScene(0);
    }

    
    public void SkipTurn()
    {
        if(playersTurn && isPlayable)
            NextPlayersTurn();
    }

    internal IEnumerator DealHands()
    {
        yield return new WaitForSeconds(1);
        for(int i=0; i<3; i++)
        {
            playerDeck.DealCard(playerHand);
            enemyDeck.DealCard(enemyHand);

            yield return new WaitForSeconds(1);
        }

        isPlayable = true;
    }

    internal bool UseCard(Card card, Player usingOnPlayer, Hand fromHand)
    {

        if(!CardValid(card, usingOnPlayer, fromHand))
            return false;

        isPlayable = false;

        CastCard(card, usingOnPlayer, fromHand);

        player.glowImage.gameObject.SetActive(false);
        enemy.glowImage.gameObject.SetActive(false);

        fromHand.RemoveCard(card);

        return false;
    }

    internal bool CardValid(Card cardBeingPlayed, Player usingOnPlayer, Hand fromHand)
    {
        bool valid = false;

        if(cardBeingPlayed == null)
        {
            return false;
        }

        if (fromHand.isPlayer)
        {
            if (cardBeingPlayed.cardData.cost <= player.mana)
            {
                if (usingOnPlayer.isPlayer && cardBeingPlayed.cardData.isDefenceCard)
                    valid = true;

                if (!usingOnPlayer.isPlayer && !cardBeingPlayed.cardData.isDefenceCard)
                    valid = true;
            }
        }
        else
        {
            if (cardBeingPlayed.cardData.cost <= enemy.mana)
            {
                if (!usingOnPlayer.isPlayer && cardBeingPlayed.cardData.isDefenceCard)
                    valid = true;

                if (usingOnPlayer.isPlayer && !cardBeingPlayed.cardData.isDefenceCard)
                    valid = true;
            }
        }
        return valid;
    }

    internal void CastCard(Card card, Player usingOnPlayer, Hand fromHand)
    {
        if(card.cardData.isMirrorCard)
        {
            usingOnPlayer.SetMirror(true);
            usingOnPlayer.PlayMirrorSound();
            NextPlayersTurn();
            isPlayable = true;

        }
        else
        {
            if(card.cardData.isDefenceCard)
            {
                usingOnPlayer.health += card.cardData.damage;
                usingOnPlayer.PlayHealSound();


                if(usingOnPlayer.health > usingOnPlayer.maxHealth)
                    usingOnPlayer.health = usingOnPlayer.maxHealth;

                UpdateHealths();

                StartCoroutine(CastHealEffect(usingOnPlayer));
            }
            else
            {
                CardAttackEffect(card, usingOnPlayer);
            }

            if (fromHand.isPlayer)
                playerScore += card.cardData.damage;
               
            UpdateScore();
        }

        if(fromHand.isPlayer)
        {
            instance.player.mana -= card.cardData.cost;
            instance.player.UpdateManaBalls();
        }
        else
        {
            instance.enemy.mana -= card.cardData.cost;
            instance.enemy.UpdateManaBalls();
        }
    }

    private IEnumerator CastHealEffect(Player usingOnPlayer)
    {
        yield return new WaitForSeconds(0.5f);
        NextPlayersTurn();
        isPlayable = true;
    }

    internal void CardAttackEffect(Card card, Player usingOnPLayer)
    {
        GameObject effectGo = null;
        if (usingOnPLayer.isPlayer)
            effectGo = Instantiate(effectFromRightPrefab, canvas.gameObject.transform);
        else
            effectGo = Instantiate(effectFromLeftPrefab, canvas.gameObject.transform);

        Effect effect = effectGo.GetComponent<Effect>();

        if(effect)
        {
            effect.targetPlayer = usingOnPLayer;
            effect.sourceCard = card;

            switch(card.cardData.damageType)
            {
                case CardData.DamageType.Fire:
                    if (card.cardData.isMultipleCard)
                        effect.effectImage.sprite = multiFireBallImage;
                    else
                        effect.effectImage.sprite = fireBallImage;
                    effect.PlayFireSound();
                    break;
                case CardData.DamageType.Ice:
                    if (card.cardData.isMultipleCard)
                        effect.effectImage.sprite = multiIceBallImage;
                    else
                        effect.effectImage.sprite = iceBallImage;
                    effect.PlayIceSound();
                    break;
                case CardData.DamageType.Both:
                    effect.effectImage.sprite = iceAndFireBallImage;
                    effect.PlayFireSound();
                    effect.PlayIceSound();
                    break;
                case CardData.DamageType.Destroy:
                    effect.effectImage.sprite = destroyBallImage;
                    effect.PlayDestroySound();
                    break;
            }
        }
    }

    internal void UpdateHealths()
    {
        player.UpdateHealth();
        enemy.UpdateHealth();

        if(player.health <= 0)
        {
            StartCoroutine(GameOver());
        }

        if(enemy.health <= 0)
        {
            playerKills++;
            playerScore += 100;
            UpdateScore();
            StartCoroutine(NewEnemy());
        }
    }

    private IEnumerator NewEnemy()
    {
        enemy.gameObject.SetActive(false);
        enemyHand.ClearHand();
        yield return new WaitForSeconds(0.75f);
        SetUpEnemy();
        enemy.gameObject.SetActive(true);
        StartCoroutine(DealHands());
    }

    private void SetUpEnemy()
    {
        enemy.mana = 0;
        enemy.health = 5;
        enemy.UpdateHealth();
        enemy.isFire = true;
        if (UnityEngine.Random.Range(0, 2) == 1)
            enemy.isFire = false;

        if (enemy.isFire)
            enemy.playerImage.sprite = fireDeamon;
        else
            enemy.playerImage.sprite = iceDeamon;
    }

    private IEnumerator GameOver()
    {
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(2);
    }

    internal void NextPlayersTurn()
    {
        playersTurn = !playersTurn;

        bool enemyIsDead = false;

        if(playersTurn)
        {
            if(player.mana < 5)
                player.mana++;
        }
        else
        {
            if(enemy.health > 0)
            {
                if (enemy.mana < 5)
                    enemy.mana++;
            }
            else
                enemyIsDead = true;
            
        }
        
        if(enemyIsDead)
        {
            playersTurn = !playersTurn;
            if (player.mana < 5)
                player.mana++;
        }
        else
        {
            SetTurnText();
            if (!playersTurn)
                MonstersTurn();
        }

        player.UpdateManaBalls();
        enemy.UpdateManaBalls();
    }

    internal void SetTurnText()
    {
        if(playersTurn)
        {
            turnText.text = "Merlin's Turn";
        }
        else
        {
            turnText.text = "Enemy's Turn";
        }
    }

    private void MonstersTurn()
    {
        Card card = AIChooseCard();
        StartCoroutine(MonsterCastCard(card));
    }

    private Card AIChooseCard()
    {
        List<Card> available = new List<Card>();
        for (int i = 0; i < 3; i++)
        {
            if (CardValid(enemyHand.cards[i], enemy, enemyHand))
                available.Add(enemyHand.cards[i]);
            else if (CardValid(enemyHand.cards[i], player, enemyHand))
                available.Add(enemyHand.cards[i]);
        }

        if(available.Count == 0)
        {
            NextPlayersTurn();
            return null;
        }
        int choice = UnityEngine.Random.Range(0, available.Count);
        return available[choice];
    }

    private IEnumerator MonsterCastCard(Card card)
    {
        yield return new WaitForSeconds(0.5f);

        if(card)
        {
            TurnCard(card);

            yield return new WaitForSeconds(2);

            if (card.cardData.isDefenceCard)
                UseCard(card, enemy, enemyHand);
            else
                UseCard(card, player, enemyHand);

            yield return new WaitForSeconds(1);

            enemyDeck.DealCard(enemyHand);

            yield return new WaitForSeconds(1);
        }
        else
        {
            enemySkipTurnImage.gameObject.SetActive(true);
            yield return new WaitForSeconds(1);
            enemySkipTurnImage.gameObject.SetActive(false);
        }
            
    }

    internal void TurnCard(Card card)
    {
        Animator animator = card.GetComponentInChildren<Animator>();

        if(animator)
        {
            animator.SetTrigger("Flip");
        }
        else
        {
            Debug.LogError("No animator found");
        }
    }

    private void UpdateScore()
    {
        scoreText.text = "Deamons killed: " + playerKills.ToString() + 
                         " Score: " + playerScore.ToString();
    }

    internal void PlayPlayerDieSound()
    {
        playerDieAudio.Play();
    }

    internal void PlayEnemyDieSound()
    {
        enemyDieAudio.Play();
    }
}
