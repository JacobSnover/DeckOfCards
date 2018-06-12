using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeckOfCards.Models
{
    public class NewCard
    {
        private object cardImage;
        private object cardValue;
        private object cardSuit;
        private object cardCode;
        public int count = 4;
        private int index = 0;
        private NewCard[] deckOfCards;

        public object CardImage { get => cardImage; set => cardImage = value; }
        public object CardValue { get => cardValue; set => cardValue = value; }
        public object CardSuit { get => cardSuit; set => cardSuit = value; }
        public object CardCode { get => cardCode; set => cardCode = value; }
        public NewCard[] DeckOfCards { get => deckOfCards; set => deckOfCards = value; }

        public NewCard(object image, object val, object suit, object code)
        {
            CardImage = image;
            CardValue = val;
            CardSuit = suit;
            CardCode = code;
        }

        public NewCard()
        {
            deckOfCards = new NewCard[52];
        }

        public void AddToDeck(NewCard data)
        {
            DeckOfCards[index] = data;
            index++;
        }
    }
}