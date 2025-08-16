namespace Cards_Generic_Engine {
	public partial class Form1 : Form {
		private Network network;
		private Board board;
		protected override CreateParams CreateParams {
			get {
				CreateParams cp = base.CreateParams;
				cp.ExStyle |= 0x02000000;  // Turn on WS_EX_COMPOSITED
				return cp;
			}
		}
		public Form1() {
			InitializeComponent();
			DoubleBuffered = true;
			this.FormClosed += OnFormClose;

		}
		public void OnFormClose(object? sender, EventArgs e) {
			network.EndNetworkThreads();
		}
		private void ShowHostMenu(object sender, EventArgs e) {
			JoinButton.Visible = false;
			HostButton.Visible = false;
			JoinButton.Enabled = false;
			HostButton.Enabled = false;
			CodeLabel.Visible = true;
			network = new();
			board = new(this);
			network.UpdateReceived += board.UpdateBoard;
			board.UpdatedBoardState += network.PostUpdate;
			network.StartServer();
			network.ConnectWithCode(network.GetNetworkCode());
			CodeLabel.Text = network.GetNetworkCode();
		}
		private void ShowJoinMenu(object sender, EventArgs e) {
			JoinButton.Visible = false;
			HostButton.Visible = false;
			JoinButton.Enabled = false;
			HostButton.Enabled = false;
			EnterCodeBox.Visible = true;
			EnterCodeBox.Enabled = true;
			EnterCodeButton.Visible = true;
			EnterCodeButton.Enabled = true;
			EnterCodeLabel.Visible = true;
			network = new();
		}

		private void EnterCodeButton_Click(object sender, EventArgs e) {
			bool connected = network.ConnectWithCode(EnterCodeBox.Text);
			if (connected) {
				
				EnterCodeLabel.Visible = false;
				EnterCodeBox.Visible = false;
				EnterCodeBox.Enabled = false;
				EnterCodeButton.Visible = false;
				EnterCodeButton.Enabled = false;
				CodeLabel.Text = EnterCodeBox.Text;
				board = new(this);
				network.UpdateReceived += board.UpdateBoard;
				board.UpdatedBoardState += network.PostUpdate;
				board.SetIdentifier(connected.ToString());
			} else {
				EnterCodeLabel.Text = "Invalid Code";
			}
		}
		private Size oldSize;
		private void Form1_Load(object sender, EventArgs e) => oldSize = base.Size;

		private void CopyCode(object? sender, EventArgs e) {
			Clipboard.SetText(CodeLabel.Text);
		}

		protected override void OnResize(System.EventArgs e) {
			base.OnResize(e);

			foreach (Control cnt in this.Controls)
				ResizeAll(cnt, base.Size);

			oldSize = base.Size;
		}
		private void ResizeAll(Control control, Size newSize) {
			int width = newSize.Width - oldSize.Width;
			control.Left += (control.Left * width) / oldSize.Width;
			control.Width += (control.Width * width) / oldSize.Width;

			int height = newSize.Height - oldSize.Height;
			control.Top += (control.Top * height) / oldSize.Height;
			control.Height += (control.Height * height) / oldSize.Height;
		}
	}
}
