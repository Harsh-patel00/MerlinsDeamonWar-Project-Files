using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Card", menuName = "CardGame/Card")]
public class CardData : ScriptableObject
{
    public enum DamageType
    {
        Fire,
        Ice,
        Both,
        Destroy
    }

    public string cardTitle;
    public string description;
    public int cost;
    public int damage;
    public DamageType damageType;
    public Sprite cardImage;
    public Sprite frameImage;
    public int cardsInDeck;
    public bool isDefenceCard;
    public bool isMirrorCard;
    public bool isMultipleCard;
    public bool isDestroyCard;
}
