using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Effect : MonoBehaviour
{
	public Player targetPlayer = null;
	public Card sourceCard = null;
	public Image effectImage = null;

	public AudioSource iceAudio = null;
	public AudioSource fireAudio = null;
	public AudioSource destroyAudio = null;


	public void EndTrigger()
	{
		bool bounce = false;
		if(targetPlayer.HasMirror())
		{
			bounce = true;
			targetPlayer.SetMirror(false);
			targetPlayer.PlaySmashSound();


			if(targetPlayer.isPlayer)
			{
				GameController.instance.CardAttackEffect(sourceCard, GameController.instance.enemy);
			}
			else
			{
				GameController.instance.CardAttackEffect(sourceCard, GameController.instance.player);
			}
		}
		else
		{
			int damage = sourceCard.cardData.damage;
			if(!targetPlayer.isPlayer)
			{
				if (sourceCard.cardData.damageType == CardData.DamageType.Fire && targetPlayer.isFire)
					damage /= 2;
				if (sourceCard.cardData.damageType == CardData.DamageType.Ice && !targetPlayer.isFire)
					damage /= 2;
				if (sourceCard.cardData.damageType == CardData.DamageType.Destroy)
					damage = sourceCard.cardData.damage;
			}
			targetPlayer.health -= damage;

			targetPlayer.PlayHitAnim();

			GameController.instance.UpdateHealths();

			if(targetPlayer.health <= 0)
			{
				targetPlayer.health = 0;
				if(targetPlayer.isPlayer)
					GameController.instance.PlayPlayerDieSound();
				else
					GameController.instance.PlayEnemyDieSound();

			}

			if(!bounce)
				GameController.instance.NextPlayersTurn();

			GameController.instance.isPlayable = true;
		}
		Destroy(gameObject);
	}

	internal void PlayIceSound()
	{
		iceAudio.Play();
	}
	internal void PlayFireSound()
	{
		fireAudio.Play();
	}
	internal void PlayDestroySound()
	{
		destroyAudio.Play();
	}

}
