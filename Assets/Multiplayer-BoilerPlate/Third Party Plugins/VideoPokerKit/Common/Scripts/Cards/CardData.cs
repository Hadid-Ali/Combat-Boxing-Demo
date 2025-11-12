using System;
using UnityEngine;
using System.Collections;

//--------------------------------------------

// card suits
public enum CardType
{
	TYPE_HEARTS,
	TYPE_SPADES,
	TYPE_DIAMONDS,
	TYPE_CLUBS,
	TYPES_NO
}

//--------------------------------------------

// card values
public enum Cardvalue
{
	value_2 = 0,
	value_3 = 1,
	value_4 = 2,
	value_5 = 3,
	value_6 = 4,
	value_7 = 5,
	value_8 = 6,
	value_9 = 7,
	value_10 = 8,
	value_J = 9,
	value_Q = 10,
	value_K = 11,
	value_A = 12,
	valueS_A = -1, //Only for straight flush tiebreaker evaluation
	valueS_NO = 14,
}

//--------------------------------------------

[System.Serializable]
public class CardData 
{
	// card suit and value (set in the Inspector)
	public CardType type = CardType.TYPE_HEARTS;
	public Cardvalue value = Cardvalue.value_2;
	
	// if was dealt or not from the deck (used only for library cards)
	private bool m_dealt = false;

	public bool dealt
	{
		get => m_dealt;
		set => m_dealt = value;
	}

	public CardData()
	{
		
	}
	public CardData(CardType type, Cardvalue value)
	{
		this.type = type;
		this.value = value;
	}

	//----------------------------------

	// used to make a copy of another card
	public void CopyInfoFrom(CardData other)
	{
		type = other.type;
		value = other.value;
	}


	
        public static string RankToString(int rank)
        {
            return ((Cardvalue)rank).ToString();
        }
        
        public int GetRank()
        {
            return (int)value;
        }
    
    
        #region Overrides
    
        public override string ToString()
        {
            return $"{value} of {type}";
        }
        
        public static bool operator <(CardData a, CardData b)
        {
            return a.value < b.value;
        }
        
        public static bool operator >(CardData a, CardData b)
        {
            return a.value > b.value;
        }
        
        public static bool operator <=(CardData a, CardData b)
        {
            return a.value <= b.value;
        }
        
        public static bool operator >=(CardData a, CardData b)
        {
            return a.value >= b.value;
        }
    
        protected bool Equals(CardData other)
        {
            return type == other.type && value == other.value;
        }
    
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
    
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
    
            if (obj.GetType() != GetType())
            {
                return false;
            }
    
            return Equals((CardData)obj);
        }
    
        public override int GetHashCode()
        {
            return HashCode.Combine((int)type, (int)value);
        }
    
    
    
        #endregion
	
}

