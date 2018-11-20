using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Klondike
{
    public partial class KlondikeGame : Form
    {
        private List<List<CardGUI>> Tables = new List<List<CardGUI>>();
        private List<CardGUI> cardGUITables = new List<CardGUI>();

        private List<CardGUI> Stock = new List<CardGUI>();
        private List<CardGUI> Unused = new List<CardGUI>();

        private List<CardGUI> Hearts = new List<CardGUI>();
        private List<CardGUI> Diamonds = new List<CardGUI>();
        private List<CardGUI> Clubs = new List<CardGUI>();
        private List<CardGUI> Spades = new List<CardGUI>();

        private const int cardShift = 20;

        public KlondikeGame()
        {
            InitializeComponent();

            cardGUITables.Add(cardGUITable1);
            cardGUITables.Add(cardGUITable2);
            cardGUITables.Add(cardGUITable3);
            cardGUITables.Add(cardGUITable4);
            cardGUITables.Add(cardGUITable5);
            cardGUITables.Add(cardGUITable6);
            cardGUITables.Add(cardGUITable7);

            for (int i = 0; i < 7; i++)
            {
                cardGUITables[i].columnNumber = i;
            }

            AttachDragEventsToCardGUI(cardGUIHearts, false);
            AttachDragEventsToCardGUI(cardGUIDiamonds, false);
            AttachDragEventsToCardGUI(cardGUIClubs, false);
            AttachDragEventsToCardGUI(cardGUISpades, false);
            AttachDragEventsToCardGUI(cardGUIUnused, false);
            AttachDragEventsToCardGUI(cardGUIStock);

            for (int i = 0; i < cardGUITables.Count; i++)
            {
                AttachDragEventsToCardGUI(cardGUITables[i], false);
            }
        }

        private void Klondike_Load(object sender, EventArgs e)
        {
            StartGame();
            DrawTables();
            DrawStock();
        }

        private void FinishGame()
        {
            int fullStackCardCount = 13;
            if (Hearts.Count != fullStackCardCount)
            {
                return;
            }
            if (Diamonds.Count != fullStackCardCount)
            {
                return;
            }
            if (Clubs.Count != fullStackCardCount)
            {
                return;
            }
            if (Spades.Count != fullStackCardCount)
            {
                return;
            }
            MessageBox.Show("YOU WIN!");
            Application.Exit();
        }

        private void StartGame()
        {
            Deck deck = new Deck();
            deck.Shuffle();

            for (int i = 0; i < 7; i++)
            {
                List<CardGUI> cardGUIList = new List<CardGUI>();
                Tables.Add(cardGUIList);
            }

            for (int i = 0; i < 7; i++)
            {
                for (int j = 0; j <= i; j++)
                {
                    Card card = deck.Pop();
                    CardGUI cardGUI = new CardGUI(card.Type, CardPlacement.Table);
                    cardGUI.columnNumber = i;
                    AttachDragEventsToCardGUI(cardGUI);
                    Tables[i].Add(cardGUI);
                }
            }

            for (int i = 0; i < 7; i++)
            {
                OpenCard(Tables[i].Last());
            }
            
            for (int i = 0; i < 24; i++)
            {
                Card card = deck.Pop();
                CardGUI cardGUI = new CardGUI(card.Type, CardPlacement.Stock);
                AttachDragEventsToCardGUI(cardGUI);
                Stock.Add(cardGUI);
            }
        }

        private void DrawTables()
        {
            for (int i = 0; i < Tables.Count; i++)
            {
                for (int j = 0; j < Tables[i].Count; j++)
                {
                    LocateCard(Tables[i][j], cardGUITables[i], j);
                }
            }
        }

        private void DrawStock()
        {
            for (int i = 0; i < 24; i++)
            {
                LocateCard(Stock[i], cardGUIStock);
            }
        }

        private void AttachDragEventsToCardGUI(CardGUI cardGUI, bool withMouseEvents = true)
        {
            cardGUI.AllowDrop = true;
            cardGUI.DragEnter += new DragEventHandler(cardGUI_DragEnter);
            cardGUI.DragDrop += new DragEventHandler(cardGUI_DragDrop);
            if (withMouseEvents)
            {
                cardGUI.MouseDown += new MouseEventHandler(cardGUI_MouseDown);
            }
        }

        private void LocateCard(CardGUI sourceCard, CardGUI fixedCard, int multiplier = 0)
        {
            Controls.Add(sourceCard);
            sourceCard.Location = new Point(fixedCard.Location.X,
                fixedCard.Location.Y + multiplier * cardShift);
            sourceCard.Size = fixedCard.Size;
            sourceCard.SizeMode = PictureBoxSizeMode.StretchImage;
            sourceCard.BringToFront();
        }

        private void OpenCard(CardGUI cardGUI)
        {
            cardGUI.card.OpenCard();
            cardGUI.Image = (Bitmap)Properties.Resources.ResourceManager.GetObject(cardGUI.card.Type.ToString());
        }

        private void CloseCard(CardGUI cardGUI)
        {
            cardGUI.card.CloseCard();
            cardGUI.Image = Properties.Resources.Back;
        }

        private void cardGUI_DragDrop(object sender, DragEventArgs e)
        {
            // TODOOO SFOUUERE

            CardGUI destinationCard = (CardGUI)sender;
            CardGUI sourceCard = (CardGUI)e.Data.GetData(typeof(CardGUI));
            
            switch (sourceCard.card.Placement)
            {
                case CardPlacement.Stack:
                    //
                    if (destinationCard.Tag != null)
                    {
                        if (destinationCard.Tag.ToString() == "Table" && sourceCard.card.Rank == CardRank.King)
                        {
                            MoveCardsFromStackToTable(sourceCard, destinationCard);
                        }
                    }
                    else
                    {
                        switch (destinationCard.card.Placement)
                        {
                            case CardPlacement.Stack:
                                break;
                            case CardPlacement.Stock:
                                break;
                            case CardPlacement.Table:
                                if (destinationCard.card.Type != Tables[destinationCard.columnNumber].Last().card.Type)
                                {
                                    return;
                                }
                                if (sourceCard.card.PutField(destinationCard.card))
                                {
                                    MoveCardsFromStackToTable(sourceCard, destinationCard);
                                }
                                break;
                            case CardPlacement.Unused:
                                break;
                        }
                    }
                    break;
                case CardPlacement.Stock:
                    break;
                case CardPlacement.Table:
                    // Empty
                    if (destinationCard.Tag != null)
                    {
                        if (destinationCard.Tag.ToString() == "Table" && sourceCard.card.Rank == CardRank.King)
                        {
                            MoveCardsFromTableToTable(sourceCard, destinationCard, true);
                        }
                        if (destinationCard.Tag.ToString() == "Stack" && sourceCard.card.Rank == CardRank.Ace)
                        {
                            MoveCardFromTableToStack(sourceCard);
                        }
                    }
                    else
                    {
                        switch (destinationCard.card.Placement)
                        {
                            case CardPlacement.Stack:
                                if (sourceCard.card.Type != Tables[sourceCard.columnNumber].Last().card.Type)
                                {
                                    return;
                                }
                                if (sourceCard.card.PutStack(destinationCard.card))
                                {
                                    MoveCardFromTableToStack(sourceCard);
                                    FinishGame();
                                }
                                break;
                            case CardPlacement.Stock:
                                break;
                            case CardPlacement.Table:
                                if (destinationCard.card.Type != Tables[destinationCard.columnNumber].Last().card.Type)
                                {
                                    return;
                                }
                                if (sourceCard.card.PutField(destinationCard.card))
                                {
                                    MoveCardsFromTableToTable(sourceCard, destinationCard);
                                }
                                break;
                            case CardPlacement.Unused:
                                break;
                        }
                    }
                    break;
                case CardPlacement.Unused:
                    // Empty
                    if (destinationCard.Tag != null)
                    {
                        if (destinationCard.Tag.ToString() == "Table" && sourceCard.card.Rank == CardRank.King)
                        {
                            MoveCardFromUnusedToTable(destinationCard.columnNumber, true);
                        }
                        if (destinationCard.Tag.ToString() == "Stack" && sourceCard.card.Rank == CardRank.Ace)
                        {
                            MoveCardFromUnusedToStack(sourceCard);
                        }
                    }
                    else
                    {
                        switch (destinationCard.card.Placement)
                        {
                            case CardPlacement.Stack:
                                if (sourceCard.card.PutStack(destinationCard.card))
                                {
                                    MoveCardFromUnusedToStack(sourceCard);
                                    FinishGame();
                                }
                                break;
                            case CardPlacement.Stock:
                                break;
                            case CardPlacement.Table:
                                if (destinationCard.card.Type != Tables[destinationCard.columnNumber].Last().card.Type)
                                {
                                    return;
                                }
                                if (sourceCard.card.PutField(destinationCard.card))
                                {
                                    MoveCardFromUnusedToTable(destinationCard.columnNumber);
                                }
                                break;
                            case CardPlacement.Unused:
                                break;
                        }
                    }
                    break;
            }
        }

        private void MoveCardsFromStackToTable(CardGUI sourceCard, CardGUI destinationCard)
        {
            CardGUI cardG;
            switch (sourceCard.card.Suite)
            {
                case CardSuite.Hearts:
                    cardG = Shift(Hearts, Tables[destinationCard.columnNumber], true);
                    LocateCard(cardG, cardGUITables[destinationCard.columnNumber], Tables[destinationCard.columnNumber].Count - 1);
                    break;
                case CardSuite.Diamonds:
                    cardG = Shift(Diamonds, Tables[destinationCard.columnNumber], true);
                    LocateCard(cardG, cardGUITables[destinationCard.columnNumber], Tables[destinationCard.columnNumber].Count - 1);
                    break;
                case CardSuite.Clubs:
                    cardG = Shift(Clubs, Tables[destinationCard.columnNumber], true);
                    LocateCard(cardG, cardGUITables[destinationCard.columnNumber], Tables[destinationCard.columnNumber].Count - 1);
                    break;
                case CardSuite.Spades:
                    cardG = Shift(Spades, Tables[destinationCard.columnNumber], true);
                    LocateCard(cardG, cardGUITables[destinationCard.columnNumber], Tables[destinationCard.columnNumber].Count - 1);
                    break;
                default:
                    cardG = new CardGUI();
                    break;
            }
            cardG.card.Placement = CardPlacement.Table;
            cardG.columnNumber = destinationCard.columnNumber;
        }

        private void cardGUI_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetData(typeof(CardGUI)) != null)
                e.Effect = DragDropEffects.Move;
            else
                e.Effect = DragDropEffects.None;
        }
        private void cardGUI_MouseDown(object sender, MouseEventArgs e)
        {
            CardGUI cardG = ((CardGUI)sender);

            // Stock
            if (cardG.Tag != null)
            {
                MoveCardsFromUnusedToStock();
                return;
            }

            switch (cardG.card.Placement)
            {
                case CardPlacement.Stock:
                    MoveCardFromStockToUnused();
                    break;
                default:
                    if (cardG.card.isOpened())
                    {
                        cardG.DoDragDrop(cardG, DragDropEffects.Move);
                    }
                    break;
            }
        }

        private void MoveCardFromTableToStack(CardGUI sourceCard)
        {
            CardGUI cardG;
            switch (sourceCard.card.Suite)
            {
                case CardSuite.Hearts:
                    cardG = Shift(Tables[sourceCard.columnNumber], Hearts, true);
                    LocateCard(cardG, cardGUIHearts);
                    break;
                case CardSuite.Diamonds:
                    cardG = Shift(Tables[sourceCard.columnNumber], Diamonds, true);
                    LocateCard(cardG, cardGUIDiamonds);
                    break;
                case CardSuite.Clubs:
                    cardG = Shift(Tables[sourceCard.columnNumber], Clubs, true);
                    LocateCard(cardG, cardGUIClubs);
                    break;
                case CardSuite.Spades:
                    cardG = Shift(Tables[sourceCard.columnNumber], Spades, true);
                    LocateCard(cardG, cardGUISpades);
                    break;
                default:
                    cardG = new CardGUI();
                    break;
            }
            cardG.card.Placement = CardPlacement.Stack;

            if (Tables[cardG.columnNumber].Count != 0)
            {
                OpenCard(Tables[cardG.columnNumber].Last());
            }

            cardG.columnNumber = -1;
        }

        private void MoveCardsFromTableToTable(CardGUI cardFrom, CardGUI cardTo, bool destinationIsEmpty = false)
        {
            int i = Tables[cardFrom.columnNumber].Count - 1;
            for (; i >= 0; i--)
            {
                if (Tables[cardFrom.columnNumber][i].card.Type == cardFrom.card.Type)
                {
                    break;
                }
            }

            int oldColumnNumber = cardFrom.columnNumber;
            if (destinationIsEmpty)
            {
                CardGUI cardG = Shift(Tables[oldColumnNumber], cardTo.columnNumber, i);
                LocateCard(cardG, cardGUITables[cardTo.columnNumber]);
                cardG.columnNumber = cardTo.columnNumber;
            }

            while (i < Tables[oldColumnNumber].Count)
            {
                CardGUI cardG = Shift(Tables[oldColumnNumber], Tables[cardTo.columnNumber], true, i);
                LocateCard(cardG, cardGUITables[cardTo.columnNumber], Tables[cardTo.columnNumber].Count - 1);
                cardG.columnNumber = cardTo.columnNumber;
            }

            if (Tables[oldColumnNumber].Count != 0)
            {
                OpenCard(Tables[oldColumnNumber].Last());
            }
        }

        private void MoveCardsFromUnusedToStock()
        {
            while (Unused.Count != 0)
            {
                CardGUI cardG = Shift(Unused, Stock, false);
                cardG.card.Placement = CardPlacement.Stock;
                LocateCard(cardG, cardGUIStock);
            }
        }

        private void MoveCardFromStockToUnused()
        {
            CardGUI cardG = Shift(Stock, Unused, true);
            cardG.card.Placement = CardPlacement.Unused;
            LocateCard(cardG, cardGUIUnused);
        }

        private void MoveCardFromUnusedToStack(CardGUI sourceCard)
        {
            CardGUI cardG;
            switch (sourceCard.card.Suite)
            {
                case CardSuite.Hearts:
                    cardG = Shift(Unused, Hearts, true);
                    LocateCard(cardG, cardGUIHearts);
                    break;
                case CardSuite.Diamonds:
                    cardG = Shift(Unused, Diamonds, true);
                    LocateCard(cardG, cardGUIDiamonds);
                    break;
                case CardSuite.Clubs:
                    cardG = Shift(Unused, Clubs, true);
                    LocateCard(cardG, cardGUIClubs);
                    break;
                case CardSuite.Spades:
                    cardG = Shift(Unused, Spades, true);
                    LocateCard(cardG, cardGUISpades);
                    break;
                default:
                    cardG = new CardGUI();
                    break;
            }
            cardG.card.Placement = CardPlacement.Stack;
            cardG.columnNumber = -1;
        }

        private void MoveCardFromUnusedToTable(int columnNumber, bool destinationIsEmpty = false)
        {
            CardGUI cardG;
            if (destinationIsEmpty)
            {
                cardG = Shift(Unused, columnNumber, Unused.Count - 1);
            }
            else
            {
                cardG = Shift(Unused, Tables[columnNumber], true);
            }
            cardG.card.Placement = CardPlacement.Table;
            cardG.columnNumber = columnNumber;
            LocateCard(cardG, cardGUITables[columnNumber], Tables[columnNumber].Count - 1);
        }

        private CardGUI Shift(List<CardGUI> From, int columnNumber, int IndexFirstReplacedCard)
        {
            CardGUI cardG = From[IndexFirstReplacedCard];
            cardG.columnNumber = columnNumber;
            From.Remove(From[IndexFirstReplacedCard]);
            OpenCard(cardG);
            Tables[columnNumber].Add(cardG);
            return cardG;
        }
        
        private CardGUI Shift(List<CardGUI> From, List<CardGUI> To, bool OpenOrClose)
        {
            CardGUI cardG = From.Last();
            From.Remove(From.Last());
            if (OpenOrClose)
            {
                OpenCard(cardG);
            }
            else
            {
                CloseCard(cardG);
            }
            To.Add(cardG);
            return cardG;
        }

        private CardGUI Shift(List<CardGUI> From, List<CardGUI> To, bool OpenOrClose, int IndexFirstReplacedCard)
        {
            CardGUI cardG = From[IndexFirstReplacedCard];
            cardG.columnNumber = To.Last().columnNumber;
            From.Remove(From[IndexFirstReplacedCard]);
            if (OpenOrClose)
            {
                OpenCard(cardG);
            }
            else
            {
                CloseCard(cardG);
            }
            To.Add(cardG);
            return cardG;
        }
    }
}
