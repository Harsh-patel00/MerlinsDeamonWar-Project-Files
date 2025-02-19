﻿using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Deck
{
    public List<CardData> cardDatas = new List<CardData>();

    public void Create()
    {
        List<CardData> cardDataInOrder = new List<CardData>();
        foreach(CardData cardData in GameController.instance.cards)
        {
            for(int i=0; i<cardData.cardsInDeck; i++)
            {
                cardDataInOrder.Add(cardData);
            }
        }

        while(cardDataInOrder.Count > 0)
        {
            int randomIndex = UnityEngine.Random.Range(0, cardDataInOrder.Count);
            cardDatas.Add(cardDataInOrder[randomIndex]);
            cardDataInOrder.RemoveAt(randomIndex);
        }
    }

    private CardData RandomCard()
    {
        CardData result = null;

        if (cardDatas.Count == 0)
            Create();

        result = cardDatas[0];
        cardDatas.RemoveAt(0);

        return result;
    }

    private Card CreateNewCard(Vector3 position, string animName)
    {
        GameObject newCard = GameObject.Instantiate(GameController.instance.cardPrefab,
                                                    GameController.instance.canvas.gameObject.transform);

        newCard.transform.position = position;

        Card card = newCard.GetComponent<Card>();

        if(card)
        {
            card.cardData = RandomCard();
            card.Initialize();

            Animator animator = newCard.GetComponentInChildren<Animator>();
            if(animator)
            {
                animator.CrossFade(animName, 0);
            }
            else
            {
                Debug.LogError("No animator found!");
            }

            return card;
        }
        else
        {
            Debug.LogError("Card component not found!");
            return null;
        }

    }

    internal void DealCard(Hand hand)
    {
        for(int h=0; h<3; h++)
        {
            if(hand.cards[h] == null)
            {
                if(hand.isPlayer)
                {
                    GameController.instance.player.PlayDealSound();
                }
                else
                {
                    GameController.instance.enemy.PlayDealSound();
                }
                hand.cards[h] = CreateNewCard(hand.positions[h].position, hand.animNames[h]);
                return;
            }
        }
    }


}
