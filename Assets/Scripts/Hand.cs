﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

[Serializable]
public class Hand
{
    public Card[] cards = new Card[3];
    public Transform[] positions = new Transform[3];
    public string[] animNames = new string[3];
    public bool isPlayer;

    internal void RemoveCard(Card card)
    {
        for(int i=0; i< cards.Length; i++)
        {
            if(cards[i] == card)
            {
                GameObject.Destroy(cards[i].gameObject);
                cards[i] = null;
                if (isPlayer)
                    GameController.instance.playerDeck.DealCard(this);
                else
                    GameController.instance.enemyDeck.DealCard(this);
                break;

            }
        }
    }

    internal void ClearHand()
    {
        for(int i=0; i<3; i++)
        {
            GameObject.Destroy(cards[i].gameObject);
            cards[i] = null;
        }
    }
}
