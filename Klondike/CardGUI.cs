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
    public partial class CardGUI : PictureBox
    {
        private Card _card;
        private int _columnNumber = -1;
        
        public Card card
        {
            get
            {
                return _card;
            }
        }

        public int columnNumber
        {
            get; set;
        }

        public CardGUI()
        {
            InitializeComponent();
            BorderStyle = BorderStyle.FixedSingle;
            AllowDrop = true;
        }

        public CardGUI(CardType type, CardPlacement cardPlacement)
        {
            InitializeComponent();
            
            _card = new Card((int)type);
            _card.Placement = cardPlacement;

            // FOR DEBUG
            //object o = Properties.Resources.ResourceManager.GetObject(_card.Type.ToString());
            object o = Properties.Resources.Back;
            Image = (Bitmap)o;
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
        }
    }
}
