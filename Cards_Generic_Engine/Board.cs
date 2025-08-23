using System.Diagnostics;

namespace Cards_Generic_Engine {
	internal class Board {
		private readonly List<Card> Cards;
		private readonly Form BaseForm;
		private Deck? createdDeck;
		private Stack? createdStack;
		private readonly Button NewDeck;
		private readonly Button NewStack;
		public event EventHandler UpdatedBoardState;
		private string identefier;
		public Board(Form form) {
			identefier = "0";
			BaseForm = form;
			Cards = [];
			NewDeck = new() {
				Size = new Size(form.Width / 28, form.Height / 15),
				Location = new Point(form.Width - 32 - (form.Width / 28), 16),
				BackgroundImage = Properties.Resources.DeckIcon,
				BackgroundImageLayout = ImageLayout.Stretch
			};
			NewDeck.MouseDown += NewDeckMouseDown;
			NewStack = new() {
				Size = new Size(form.Width / 28, form.Height / 15),
				Location = new Point(NewDeck.Left - 32 - (form.Width / 28), 16),
				BackgroundImage = Properties.Resources.StackIcon,
				BackgroundImageLayout = ImageLayout.Stretch
			};
			NewStack.MouseDown += NewStack_Button;
			form.Controls.Add(NewStack);
			form.Controls.Add(NewDeck);
			Card.NewCard += OnNewCard;
			Card.CardNulled += RemoveCard;
			Deck.Recall += OnDeckRecalled;
		}
		public void SetIdentifier(string id) {
			identefier = id;
			foreach (Card card in Cards) {
				if (card.identifier[1] == '0') {
					card.identifier.Remove(1, 1);
					card.identifier.Insert(1, id);
				}
			}
		}
		private void NewDeckMouseDown(object? sender, MouseEventArgs e) {
			if (e.Button == System.Windows.Forms.MouseButtons.Left) {
				createdDeck = new Deck(BaseForm, new Point(NewDeck.Left + e.X - (BaseForm.Width / 24), 16));
				AddCard(createdDeck);
				createdDeck.BringToTop();
				createdDeck.MouseDown(sender, e);
				OnNewCard(createdDeck,EventArgs.Empty);
			}
		}
		private void NewStack_Button(object? sender, MouseEventArgs e) {
			if (e.Button == System.Windows.Forms.MouseButtons.Left) {
				createdStack = new Stack(BaseForm, new Point(NewDeck.Left - 16 - (BaseForm.Width / 24), 16));
				AddCard(createdStack);
				createdStack.BringToTop();
				createdStack.MouseDown(sender, e);
				OnNewCard(createdDeck,EventArgs.Empty);	
			}
		}
		public string GetBoard() {

			return "";
		}
		public string AddCard(Card card) {
			string? tag = card.GetTag();
			if (tag == "card") {
				if (card.identifier[0] != 'c') card.identifier = "c" + identefier + "." + card.identifier;
			} else if (tag == "deck") {
				if (card.identifier[0] != 'd') card.identifier = "d" + identefier + "." + card.identifier;
			} else if (tag == "stack") {
				if (card.identifier[0] != 's') card.identifier = "s" + identefier + "." + card.identifier;
			}
			card.CardMoved += MoveCard;
			Cards.Add(card);
			return card.identifier;
		}
		public void OnNewCard(object? sender, EventArgs e) {
			if (sender == null) return;
			string id = AddCard((Card)sender);
			//N<card_id>,<x>,<y>,<deck_file>,<card_list>
			Point p = ((Card)sender).GetLocation();
			string msg = "N" + id + "." + p.X + "." + p.Y;
			if (id[0] == 'c') {
				msg += "." + ((Card)sender).deck_index;
			} else if (id[0] == 'd') {
				int[] indexs = ((Deck)sender).GetOrder();
				foreach (int i in indexs) {
					msg += "." + i.ToString();
				}
			} else if (id[0] == 's') {
				int[] indexs = ((Stack)sender).GetOrder();
				foreach (int i in indexs) {
					msg += "." + i.ToString();
				}
			}
			Debug.WriteLine(msg);
			UpdatedBoardState?.Invoke(msg, EventArgs.Empty);

		}
		public void RecallDeck(string deck_id) {
			for (int i = 0; i < Cards.Count; i++) {
				string[] split = Cards[i].identifier.Split(".");
				if (Cards[i].GetTag() == "stack") {
					((Stack)Cards[i]).RemoveCards(deck_id);
					continue;
				}
				if (split.Length == 1) continue;
				if (split[1] == deck_id) {
					Cards[i].Delete();
					Cards.RemoveAt(i);
					i--;
				}
			}
		}
		public void OnDeckRecalled(object? sender, EventArgs e) {
			if (sender == null) return;
			RecallDeck(((Deck)sender).identifier);
			//R<deck_id>
			string msg = "R" + ((Deck)sender).identifier;
			UpdatedBoardState?.Invoke(msg, EventArgs.Empty);
		}
		public void RemoveCard(string card_id) {
			for (int i = 0; i < Cards.Count; i++) {
				if (Cards[i].identifier == card_id) {
					Cards.RemoveAt(i);
					return;
				}
			}
		}
		public void RemoveCard(object? sender, EventArgs e) {
			if (sender == null) return;
			//D<card_id>
			string msg = "D" + ((Card)sender).identifier;
			RemoveCard(((Card)sender).identifier);
			UpdatedBoardState?.Invoke(msg, EventArgs.Empty);
		}
		public void MoveCard(string card_id, Point p) {
			foreach (Card card in Cards) {
				if (card.identifier == card_id) {
					card.Move(p);
					return;
				}
			}
		}
		public void MoveCard(object? sender, EventArgs e) {
			if (sender == null) return;
			//M<card_id>,<x>,<y>
			Point p = ((Card)sender).GetLocation();
			string msg = "M" + ((Card)sender).identifier + "," + p.X + "," + p.Y;

			UpdatedBoardState?.Invoke(msg, EventArgs.Empty);
		}
		public void UpdateBoard(object? sender, EventArgs e) {
			Debug.WriteLine("update");
			if (sender == null) return;
			char Code = ((string)sender)[0];
			string[] msg = ((string)sender).Substring(1).Split(',');
			switch (Code) {
				//Move
				case 'M':

					break;
				//Flip
				case 'F':

					break;
				//Lock
				case 'L':

					break;
				//Delete
				case 'D':

					break;
				//Pull
				case 'P':

					break;
				//New
				//N<card_id>,<x>,<y>,<deck_file>,<card_list>
				case 'N':
					Point p = new(int.Parse(msg[1]), int.Parse(msg[2]));
					DirectoryInfo dirInfo = new(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/Cards/Decks/" + msg[3]);
					FileInfo[] fileInfo = dirInfo.GetFiles();
					Array.Sort(fileInfo, (f1, f2) => f1.Name.CompareTo(f2.Name));
					if (msg[0][0] == 'c') {
						Card c = new(BaseForm, fileInfo[int.Parse(msg[4])].FullName, p);
						c.deck_index = int.Parse(msg[4]);
						c.identifier = msg[0];
						AddCard(c);
					} else if (msg[0][0] == 'd') {
						Deck c = new(BaseForm, p, msg[3]);
						int[] order = new int[msg.Length - 4];
						for (int i = 4; i < msg.Length; i++) {
							order[i - 4] = int.Parse(msg[i]);
						}
						c.Reorder(order);
						c.identifier = msg[0];
						AddCard(c);
					} else if (msg[0][0] == 's') {
						Stack c = new(BaseForm,p);
						int[] order = new int[msg.Length - 4];
						for (int i = 4; i < msg.Length; i++) {
							order[i - 4] = int.Parse(msg[i]);
						}
					}
					break;
				//Shuffle
				case 'S':

					break;
				//Recall
				case 'R':

					break;
				case 'I':
					identefier = msg[0];
					break;
				default:
					return;
			}
		}
	}
}
