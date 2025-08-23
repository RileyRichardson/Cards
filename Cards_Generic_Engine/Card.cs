using System.Diagnostics;
using System.IO;

namespace Cards_Generic_Engine {
	internal class Card {
		protected PictureBox cardImage;
		protected Button Overlay;
		public bool FaceUp;
		public string card;
		protected ContextMenuStrip menu;
		protected bool Locked = false;
		public bool SystemLock = false;
		public static event EventHandler? NewCard;
		public static event EventHandler? CardPlaced;
		public event EventHandler? CardMoved;
		public static event EventHandler? CardNulled;
		protected Form BaseForm;
		public bool Voided = false;
		public string identifier;
		private static int cardNum = 0;
		public string deck_folder;
		public int deck_index;

		public Card(Form form, string card_, Point p) {
			if(card_ != "") {
				DirectoryInfo dir = new(card_);
				deck_folder = dir.Name;
			} else {
				deck_folder = "";
			}
				cardNum++;
			identifier = cardNum.ToString();
			BaseForm = form;
			card = card_;
			FaceUp = true;
			cardImage = new PictureBox {
				Location = p,
				Size = new Size(form.Width / 12, form.Height / 5),
				SizeMode = PictureBoxSizeMode.StretchImage,
				BackColor = Color.Transparent
			};
			cardImage.Tag = "card";
			if (card_ == "") {
				cardImage.Image = Properties.Resources.EmptyStack;
			} else {
				cardImage.ImageLocation = card_;
			}
			Overlay = new() {
				Location = new(0, 0),
				Size = cardImage.Size,
			};
			Overlay.FlatAppearance.BorderSize = 0;
			Overlay.FlatAppearance.MouseDownBackColor = Color.Transparent;
			Overlay.FlatAppearance.MouseOverBackColor = Color.Transparent;
			Overlay.FlatStyle = FlatStyle.Flat;
			cardImage.Controls.Add(Overlay);
			Overlay.MouseDown += MouseDown;
			Overlay.MouseMove += MouseMove;
			Overlay.MouseUp += MouseUp;
			menu = new();
			menu.Items.Add("Lock");
			menu.Items.Add("Flip");
			menu.Items[0].Click += Lock;
			menu.Items[1].Click += Flip;
			Overlay.ContextMenuStrip = menu;
			form.Controls.Add(cardImage);
		}
		public string? GetTag() {
			return cardImage.Tag?.ToString();
		}
		public void Delete() {
			BaseForm.Controls.Remove(cardImage);
		}
		public void Nullify() {
			Delete();
			CardNulled?.Invoke(this, EventArgs.Empty);
		}
		public void Lock() {
			if (Locked) {
				Locked = false;
				menu.Items[0].Text = "Lock";
			} else {
				Locked = true;
				menu.Items[0].Text = "Unlock";
			}
		}
		private void Lock(object? sender, EventArgs e) {
			Lock();
		}
		public void BringToTop() {
			cardImage.BringToFront();
			Overlay.Select();
		}
		public void ChangeCard(string CardFile) {
			card = CardFile;
			if (FaceUp) cardImage.ImageLocation = card;
			else {
				cardImage.ImageLocation = null;
				cardImage.Image = Properties.Resources.CardBack;
			}

		}
		public virtual void Flip() {
			if (FaceUp) {
				cardImage.Image = Properties.Resources.CardBack;
				cardImage.ImageLocation = null;
			} else {
				cardImage.ImageLocation = card;
			}
			FaceUp = !FaceUp;
		}
		public void Flip(object? sender, EventArgs e) {
			Flip();
		}
		public virtual string GetCard() {
			return card;
		}
		private Point MouseDownPoint;
		public virtual void MouseDown(object? sender, MouseEventArgs e) {
			if (e.Button == System.Windows.Forms.MouseButtons.Left && !Locked && !SystemLock) {
				MouseDownPoint = e.Location;
			}
		}
		public virtual void MouseMove(object? sender, MouseEventArgs e) {
			if (e.Button == System.Windows.Forms.MouseButtons.Left && !Locked && !SystemLock) {
				cardImage.Left = e.X + cardImage.Left - MouseDownPoint.X;
				cardImage.Top = e.Y + cardImage.Top - MouseDownPoint.Y;
				CardMoved?.Invoke(this, EventArgs.Empty);

			}
		}
		public Point GetLocation() {
			return cardImage.Location;
		}
		public void Move(Point p) {
			cardImage.Location = p;
		}
		public virtual void MouseUp(object? sender, MouseEventArgs e) {
			if (e.Button == System.Windows.Forms.MouseButtons.Left && !Locked) {
				CardPlaced?.Invoke(this, EventArgs.Empty);
			}
		}
		protected void InvokeNewCard(Card card) {
			NewCard?.Invoke(card, EventArgs.Empty);
		}
	}
	class Stack : Card {
		private List<Card> Cards;
		ToolStripMenuItem LayoutMenu;
		public Stack(Form form, Point p) : base(form, "", p) {
			Cards = [];
			cardImage.SendToBack();
			CardPlaced += SnapCard;
			cardImage.Tag = "stack";
			menu.Items.Add("Change Card Layout");
			LayoutMenu = (ToolStripMenuItem)menu.Items[^1];
			LayoutMenu.DropDownItems.Add("Left To Right");
			LayoutMenu.DropDownItems.Add("Top To Bottom");
			LayoutMenu.DropDownItems[0].Click += SetLeftToRight;
			LayoutMenu.DropDownItems[1].Click += SetTopToBottom;
		}
		private int leftToRight = 0;
		private int topToBottom = 1;
		private int stack = -1;
		public void SetLeftToRight(object? sender,EventArgs e) {
			LayoutMenu.DropDownItems[leftToRight].Click -= SetLeftToRight;
			if (stack == -1) {
				LayoutMenu.DropDownItems[leftToRight].Text = "Stack";
				LayoutMenu.DropDownItems[leftToRight].Click += SetStack;
				stack = leftToRight;
			} else if ( topToBottom == -1) {
				LayoutMenu.DropDownItems[leftToRight].Text = "Top To Bottom";
				LayoutMenu.DropDownItems[leftToRight].Click += SetTopToBottom;
				topToBottom = leftToRight;
			}
			leftToRight = -1;
		}
		public void SetTopToBottom(object? sender, EventArgs e) {
			LayoutMenu.DropDownItems[topToBottom].Click -= SetTopToBottom;
			if (stack == -1) {
				LayoutMenu.DropDownItems[topToBottom].Text = "Stack";
				LayoutMenu.DropDownItems[topToBottom].Click += SetStack;
				stack = topToBottom;
			} else if (leftToRight == -1) {
				LayoutMenu.DropDownItems[topToBottom].Text = "Left To Right";
				LayoutMenu.DropDownItems[topToBottom].Click += SetLeftToRight;
				leftToRight = topToBottom;
			}
			topToBottom = -1;
		}
		public void SetStack(object? sender, EventArgs e) {
			LayoutMenu.DropDownItems[stack].Click -= SetStack;
			if (leftToRight == -1) {
				LayoutMenu.DropDownItems[stack].Text = "Left To Right";
				LayoutMenu.DropDownItems[stack].Click += SetLeftToRight;
				leftToRight = stack;
			} else if (topToBottom == -1) {
				LayoutMenu.DropDownItems[stack].Text = "Top To Bottom";
				LayoutMenu.DropDownItems[stack].Click += SetTopToBottom;
				topToBottom = stack;
			}
			stack = -1;
		}
		private int scale = 3;
		public void AddCard(Card card) {
			
			if(stack == -1) {
				card.Move(cardImage.Location);
			}else if(leftToRight == -1) {
				Point p = cardImage.Location;
				p.X += Cards.Count*cardImage.Width/scale;
				card.Move(p);
			} else {

			}
			card.BringToTop();
			BringToTop();
			Cards.Insert(0, card);
		}
		public void AddCard(string deck, int[] order) {
			DirectoryInfo dirInfo = new(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/Cards/Decks/" + deck);
			FileInfo[] fileInfo = dirInfo.GetFiles();
			Array.Sort(fileInfo, (f1, f2) => f1.Name.CompareTo(f2.Name));
			foreach(int i in order){
				AddCard(new(BaseForm, fileInfo[i].FullName,GetLocation()));
			}
		}
		public override string GetCard() {
			if (Cards.Count == 0) {
				return "";
			}
			string card = Cards[0].card;
			Cards.RemoveAt(0);
			return card;
		}
		public void RemoveCards(string deck_id) {
			for (int i = 0; i < Cards.Count; i++) {
				string[] split = Cards[i].identifier.Split("-");
				if (split.Length == 1) continue;
				if (split[1] == deck_id) {
					Cards.RemoveAt(i);
					i--;
				}
			}
		}
		public int[] GetOrder() {
			int[] ret = new int[Cards.Count];
			for(int i = 0; i < Cards.Count; i++) {
				ret[i] = Cards[i].deck_index;
			}
			return ret;
		}
		public override void Flip() {

		}

		public void SnapCard(object? sender, EventArgs e) {
			if (sender == null) return;
			Card c = (Card)sender;
			if (c.GetTag() != "card") return;
			Point p = c.GetLocation();
			int right = p.X + cardImage.Width;
			int bottom = p.Y + cardImage.Height;
			if (((p.X > cardImage.Left && p.X < cardImage.Right) || (right > cardImage.Left && right < cardImage.Right)) &&
				((p.Y > cardImage.Top && p.Y < cardImage.Bottom) || (bottom > cardImage.Top && bottom < cardImage.Bottom)) ||
				p == GetLocation()) {
				AddCard(c);
				if (c.FaceUp ^ FaceUp) Flip();

			}
		}
		public override void MouseDown(object? sender, MouseEventArgs e) {
			if (e.Button != MouseButtons.Left) return;
			if (Locked&&Cards.Count>0) {
				
			} else {
				if (Locked) return;
				base.MouseDown(sender, e);
			}

		}

	}
	class Deck : Card {
		public class CardIndexPair {
			public CardIndexPair(string c, int i) {
				cardFile = c;
				index = i;
			}
			public string cardFile;
			public int index;
		}
		List<CardIndexPair> Cards;
		public static event EventHandler Recall;
		public Deck(Form form, Point p,string deck_dir = "Default_Cards") : base(form, "", p) {
			deck_folder = deck_dir;
			Cards = [];
			menu.Items.Add("Shuffle Deck");
			menu.Items[1].Text = "Reveal Top Card";
			menu.Items[^1].Click += Shuffle;
			menu.Items.Add("Recall");
			menu.Items[^1].Click += RecallDeck;
			menu.Items.Add("Flip Deck");
			menu.Items[^1].Click += FlipDeck;
			Flip();
			LoadDeck(deck_folder);
			Shuffle();
			cardImage.Tag = "deck";
		}
		private void FlipDeck(object? sender,EventArgs e) {

		}
		public int[] GetOrder() {
			int[] ret = new int[Cards.Count];
			for (int i = 0; i<Cards.Count;i++) {
				ret[i] = Cards[i].index;
			}
			return ret;
		}

		public void Reorder(int[] order) {
			LoadDeck(deck_folder);
			List<CardIndexPair> oldDeck = Cards;
			Cards = [];
			foreach(int i in order) {
				Cards.Add(oldDeck[i]);
			}
		}
		public override string GetCard() {
			if (Cards.Count == 0) {
				return "";
			}
			string card = Cards[0].cardFile;
			Cards.RemoveAt(0);
			ChangeCard(Cards[0].cardFile);
			if (FaceUp) {
				Flip();
			}
			return card;
		}
		public void LoadDeck(string deckdir) {
			DirectoryInfo dirInfo = new(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/Cards/Decks/"+deckdir);
			deck_folder = deckdir;
			FileInfo[] fileInfo = dirInfo.GetFiles();
			Array.Sort(fileInfo,(f1,f2)=>f1.Name.CompareTo(f2.Name));
			Cards = [];
			int j = 0;
			foreach (FileInfo i in fileInfo) {
				Cards.Add(new(i.FullName, j));
				j++;
			}
			ChangeCard(Cards[0].cardFile);
		}
		private void Shuffle(object? sender, EventArgs e) {
			Shuffle();
		}
		public void Shuffle() {
			List<CardIndexPair> oldDeck = [.. Cards];
			Cards = [];
			Random random = new();
			while (oldDeck.Count > 0) {
				int randomNum = random.Next(0, oldDeck.Count - 1);
				Cards.Add(oldDeck[randomNum]);
				oldDeck.RemoveAt(randomNum);
			}
			ChangeCard(Cards[0].cardFile);

		}
		public void RecallDeck(object? sender, EventArgs e) {
			Recall?.Invoke(this, EventArgs.Empty);
			Cards = [];
			LoadDeck(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/Cards/Decks/Default_Cards");
			Shuffle();
		}
		public override void MouseDown(object? sender, MouseEventArgs e) {
			if (e.Button != MouseButtons.Left) return;
			if (Locked) {
				bool face = FaceUp;
				int index = Cards[0].index;
				Card newCard = new(BaseForm, GetCard(), new(cardImage.Left + e.X - BaseForm.Width / 24, cardImage.Top + e.Y - BaseForm.Height / 10));
				newCard.deck_index = index;
				if (!face) newCard.Flip();
				newCard.identifier += "-" + identifier;
				newCard.BringToTop();
				newCard.MouseDown(sender, e);
				InvokeNewCard(newCard);
			} else {
				base.MouseDown(sender, e);
			}

		}

	}

}
