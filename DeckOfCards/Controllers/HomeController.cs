using DeckOfCards.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;


namespace DeckOfCards.Controllers
{
    public class HomeController : Controller
    {
        public static int[] indexes = { 0, 1, 2, 3, 4 };
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        //call the API to get the data for my deck of cards
        public ActionResult Game()
        {
            HttpWebRequest data = WebRequest.CreateHttp("https://deckofcardsapi.com/api/deck/new/shuffle/?deck_count=1");

            HttpWebResponse Response;

            Response = (HttpWebResponse)data.GetResponse();

            StreamReader sr = new StreamReader(Response.GetResponseStream());

            string d = sr.ReadToEnd();
            JObject JsonData = JObject.Parse(d);

            Session["deckID"] = JsonData["deck_id"];

            return RedirectToAction("GetCards");
        }
        //this makes an API call to get a new deck of cards
        public ActionResult GetCards()
        {
            DeckOfCardsEntities ORM = new DeckOfCardsEntities();
            Card tempCard;

            NewCard Deck = new NewCard();

            object ID = Session["deckID"];
            HttpWebRequest data = WebRequest.CreateHttp($"https://deckofcardsapi.com/api/deck/{ID}/draw/?count=52");

            HttpWebResponse Response;
            Response = (HttpWebResponse)data.GetResponse();
            StreamReader sr = new StreamReader(Response.GetResponseStream());

            string d = sr.ReadToEnd();

            JObject JsonData = JObject.Parse(d);
            ViewBag.Remain = JsonData;
            ViewBag.Cards = JsonData["cards"];

            for (int i = 0; i < ViewBag.Cards.Count; i++)
            {
                Deck.AddToDeck(new NewCard(ViewBag.Cards[i]["image"], ViewBag.Cards[i]["value"], ViewBag.Cards[i]["suit"], ViewBag.Cards[i]["code"]));
            }

            //after grabbing a new deck of cards, I then add the data to my DB
            //as a deck of cards, so that I can call on my server instead of the API
            for (int i = 0; i < ViewBag.Cards.Count; i++)
            {
                tempCard = new Card();
                tempCard.CardImage = ViewBag.Cards[i]["image"];
                tempCard.CardValue = ViewBag.Cards[i]["value"];
                tempCard.CardSuit = ViewBag.Cards[i]["suit"];
                tempCard.CardCode = ViewBag.Cards[i]["code"];
                ORM.Card.Add(tempCard);
                ORM.SaveChanges();
            }

            ViewBag.Indexes = indexes;
            Session["Deck"] = Deck;
            ViewBag.Deck = Deck.DeckOfCards;
            ViewBag.Keep = null;
            ViewBag.Count = 5;

            return View();
        }

        public ActionResult DrawCards(string[] data)
        {
            NewCard Deck = (NewCard)Session["Deck"];
            int drawCount = 0;
            List<NewCard> Draw = new List<NewCard>();
            List<object> Keepers = new List<object>();

            if (data != null)
            {
                foreach (var item in data)
                {
                    Keepers.Add(Deck.DeckOfCards[Convert.ToInt32(item)].CardImage);
                }
                Deck.count += data.Length;
                drawCount = data.Length;
            }

            for (int i = 0; i < 5 - drawCount; i++)
            {
                Deck.count++;
                if (Deck.count > 51)
                {
                    Deck.count = 4;
                    return RedirectToAction("Game");
                }
                Draw.Add(Deck.DeckOfCards[Deck.count]);
            }

            ViewBag.Indexes = indexes;
            ViewBag.Keep = Keepers;
            ViewBag.Deck = Draw;
            ViewBag.Count = 5 - drawCount;

            return View("GetCards");
        }
    }
}