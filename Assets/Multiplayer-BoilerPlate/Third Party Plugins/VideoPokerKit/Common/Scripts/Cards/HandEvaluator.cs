using System;
using UnityEngine;
using System.Collections;

public class HandEvaluator 
{
	//*************
	// NOTE
	// This working class is used for evaluating a poker hand (5 cards)
	// and detect the best possible win (with the greatest outcome)
	// The methods and members are static so that they can be called directly
	// without the need to use an instance object
	//*************


	static CardData [] workCards;
	static int [] cardsValueCount = new int[ (int)Cardvalue.valueS_NO ];
	static int [] cardsTypeCount = new int[ (int)CardType.TYPES_NO ];

	// all combinations of sorted cards for STRAIGHT 
	// (for this type of win, it is more simpler to do it like this instead of using an algorithm)
	static Cardvalue [][] straightSets = new Cardvalue[][] {	
		new Cardvalue[] {Cardvalue.value_2, Cardvalue.value_3, Cardvalue.value_4, Cardvalue.value_5, Cardvalue.value_A},
		new Cardvalue[] {Cardvalue.value_2, Cardvalue.value_3, Cardvalue.value_4, Cardvalue.value_5, Cardvalue.valueS_A},
		new Cardvalue[] {Cardvalue.value_2, Cardvalue.value_3, Cardvalue.value_4, Cardvalue.value_5, Cardvalue.value_6},
		new Cardvalue[] {Cardvalue.value_3, Cardvalue.value_4, Cardvalue.value_5, Cardvalue.value_6, Cardvalue.value_7},
		new Cardvalue[] {Cardvalue.value_4, Cardvalue.value_5, Cardvalue.value_6, Cardvalue.value_7, Cardvalue.value_8},
		new Cardvalue[] {Cardvalue.value_5, Cardvalue.value_6, Cardvalue.value_7, Cardvalue.value_8, Cardvalue.value_9},
		new Cardvalue[] {Cardvalue.value_6, Cardvalue.value_7, Cardvalue.value_8, Cardvalue.value_9, Cardvalue.value_10},
		new Cardvalue[] {Cardvalue.value_7, Cardvalue.value_8, Cardvalue.value_9, Cardvalue.value_10,Cardvalue.value_J},
		new Cardvalue[] {Cardvalue.value_8, Cardvalue.value_9, Cardvalue.value_10,Cardvalue.value_J, Cardvalue.value_Q},
		new Cardvalue[] {Cardvalue.value_9, Cardvalue.value_10,Cardvalue.value_J, Cardvalue.value_Q, Cardvalue.value_K},
		new Cardvalue[] {Cardvalue.value_10,Cardvalue.value_J, Cardvalue.value_Q, Cardvalue.value_K, Cardvalue.value_A}};
	
	//--------------------------------------------------------
	// internal class used to compare 2 cards
	public class CardComparer : IComparer  
	{
		int IComparer.Compare( System.Object x, System.Object y )
		{
			try
			{
				return( ((CardData)x).value >= ((CardData)y).value ? 1 : -1 );
			}
			catch (Exception e)
			{
				Debug.LogError($"Comparing {(CardData)x} with {(CardData)y}");
				return 1;
			}
		}		
	}

	//--------------------------------------------------------

	public static void Evaluate(CardData[] cards, out HandTypes handType)
	{
		handType = HandTypes.HighCard;
		// define all win flags needed in video poker
		bool bOnePair = false;
		bool bTwoPair = false;
		bool bThree = false;
		bool bFive = false;
		bool bStraight = false;
		bool bFlush = false;
		bool bFullHouse = false;
		bool bFour = false;
		bool bStraightFlush = false;
		bool bRoyalFlush = false;

		// remember first pair value in case we need it later
		// (for 2 pair wins)
		int firstPairValue = 0;

		// sort cards based of value so that we end up with a sorted array
		System.Array.Sort(cards, new CardComparer());

		// count cards by value
		for (int i = 0; i < cardsValueCount.Length; i++)
		{
			cardsValueCount[i] = 0; // first reset all counters
		}

		for (int i = 0; i < cards.Length; i++)
		{
			cardsValueCount[(int)cards[i].value]++; // count
		}

		// count types by type
		for (int i = 0; i < cardsTypeCount.Length; i++)
		{
			cardsTypeCount[i] = 0;
		}

		for (int i = 0; i < cards.Length; i++)
		{
			cardsTypeCount[(int)cards[i].type]++;
		}

		for (int i = 0; i < cardsValueCount.Length; i++)
		{
			// check one and two pairs
			if (cardsValueCount[i] == 2)
			{
				if (!bOnePair)
				{
					bOnePair = true;
					firstPairValue = i;
					Debug.Log("One pair");
					handType = HandTypes.Pair;

					// show win only if starting from JACKS
					if (i >= (int)Cardvalue.value_J)
					{
						Debug.Log("Jacks Or Better");
						// if (autoHold)
						// 	HoldCardsByValue(cards, (CardValue)i);
					}
				}
				else
				{
					bTwoPair = true;
					handType = HandTypes.TwoPair;
					Debug.Log("Two pair");
				}
			}

			// check three of a kind
			if (cardsValueCount[i] == 3)
			{
				bThree = true;
				handType = HandTypes.ThreeOfAKind;
				Debug.Log("Three of a kind");
			}

			// check for of a kind
			if (cardsValueCount[i] == 4)
			{
				bFour = true;
				Debug.Log("Four of a kind");
				handType = HandTypes.FourOfAKind;
			}
			//Check five of a kind
			if (cardsValueCount[i] == 5)
			{
				bFive = true;
				Debug.Log("Five of a kind");
				handType = HandTypes.FiveOfAKind;
				return;
			}
		}

		// check flush
		for (int i = 0; i < cardsTypeCount.Length; i++)
			if (cardsTypeCount[i] == 5)
			{
				bFlush = true;
				handType = HandTypes.Flush;
				Debug.Log("Flush");
			}

		// check straight
		for (int i = 0; i < straightSets.Length; i++)
		{
			int match = 0;
			for (int j = 0; j < cards.Length; j++)
				if (cards[j].value == straightSets[i][j])
					match++;

			if (match == 5)
			{
				bStraight = true;
				Debug.Log("Straight");
				handType = HandTypes.Straight;
				break;
			}
		}

		// check full house
		if (bOnePair && bThree)
		{
			bFullHouse = true;
			Debug.Log("Full house");
			handType = HandTypes.FullHouse;
		}

		// check straight flush
		if (bStraight && bFlush)
		{
			bStraightFlush = true;
			Debug.Log("Straight flush");
			handType = HandTypes.StraightFlush;
		}
		
		//Check Royal-Flush
		{
			Cardvalue[] RoyalFlushSequence = new Cardvalue[] { Cardvalue.value_10, Cardvalue.value_J, Cardvalue.value_Q, Cardvalue.value_K, Cardvalue.value_A };
			int sameValue = RoyalFlushSequence.Length;

			for (int i = 0; i < RoyalFlushSequence.Length; i++)
			{
				if (cards[i].value == RoyalFlushSequence[i] &&
				    cards[i].type ==
				    cards[0].type) //If value is in specific order and all types are same as the first card type(meaning all are sme)
				{
					sameValue--;
				}
			}

			if (sameValue == 0)
			{
				bRoyalFlush = true;
				handType = HandTypes.RoyalFlush;
			}
		}
	}
}
