namespace task0
{
    partial class Form1
    {
        public System.ComponentModel.IContainer components = null;
        public TextBox textBox1;
        public TextBox totalPeopleTextBox;
        public Button buttonAdd;
        public Button buttonGenerate;
        public Button buttonDelete;
        public TextBox textBox2;
        public TextBox textBox3;
        public ListView listView1;
        public Button buttonStop;
        public Button buttonCancel;
        public TextBox statusTextBox;
        public TextBox textBoxSearchName;
        public TextBox textBoxSearchContact;
        public RadioButton radioSqlHandler;
        public RadioButton radioEfHandler;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        public void InitializeComponent()
        {
            textBox1 = new TextBox();
            buttonAdd = new Button();
            buttonGenerate = new Button();
            buttonDelete = new Button();
            textBox2 = new TextBox();
            textBox3 = new TextBox();
            listView1 = new ListView();
            buttonStop = new Button();
            buttonCancel = new Button();
            statusTextBox = new TextBox();
            textBoxSearchName = new TextBox();
            textBoxSearchContact = new TextBox();
            radioSqlHandler = new RadioButton();
            radioEfHandler = new RadioButton();
            SuspendLayout();

            listView1.Columns.Add("Name", 150);
            listView1.Columns.Add("Phone", 150);
            listView1.Columns.Add("Email", 200);
            listView1.FullRowSelect = true;
            listView1.Location = new Point(30, 80);
            listView1.Name = "listView1";
            listView1.Size = new Size(600, 200);
            listView1.UseCompatibleStateImageBehavior = false;
            listView1.View = View.Details;
            listView1.SelectedIndexChanged += listView1_SelectedIndexChanged;

            textBoxSearchName.Location = new Point(30, 300);
            textBoxSearchName.Size = new Size(200, 23);
            textBoxSearchName.PlaceholderText = "Search by name...";
            textBoxSearchName.TextChanged += textBoxSearchName_TextChanged;

            textBoxSearchContact.Location = new Point(250, 300);
            textBoxSearchContact.Size = new Size(200, 23);
            textBoxSearchContact.PlaceholderText = "Search by phone or email...";
            textBoxSearchContact.TextChanged += textBoxSearchContact_TextChanged;

            textBox1.Location = new Point(30, 37);
            textBox1.Name = "textBox1";
            textBox1.PlaceholderText = "Name";
            textBox1.Size = new Size(100, 23);

            textBox2.Location = new Point(136, 37);
            textBox2.Name = "textBox2";
            textBox2.PlaceholderText = "Phone";
            textBox2.Size = new Size(100, 23);

            textBox3.Location = new Point(242, 37);
            textBox3.Name = "textBox3";
            textBox3.PlaceholderText = "Email";
            textBox3.Size = new Size(100, 23);

            buttonAdd.Location = new Point(700, 37);
            buttonAdd.Name = "buttonAdd";
            buttonAdd.Size = new Size(75, 23);
            buttonAdd.Text = "Add";
            buttonAdd.Click += buttonAdd_Click;

            buttonGenerate.Location = new Point(700, 100);
            buttonGenerate.Text = "Generate";
            buttonGenerate.Click += buttonGenerate_Click;

            buttonDelete.Location = new Point(700, 70);
            buttonDelete.Text = "Delete";
            buttonDelete.Click += buttonDelete_Click;

            buttonStop.Location = new Point(700, 130);
            buttonStop.Name = "buttonStop";
            buttonStop.Text = "Stop";
            buttonStop.Click += buttonStop_Click;

            buttonCancel.Location = new Point(700, 160);
            buttonCancel.Name = "buttonCancel";
            buttonCancel.Size = new Size(75, 23);
            buttonCancel.Text = "Cancel";
            buttonCancel.Visible = false;
            buttonCancel.Click += buttonCancel_Click;

            totalPeopleTextBox = new TextBox();
            totalPeopleTextBox.Location = new Point(30, 330);
            totalPeopleTextBox.Name = "totalPeopleTextBox";
            totalPeopleTextBox.Size = new Size(200, 23);
            totalPeopleTextBox.ReadOnly = true;
            totalPeopleTextBox.TextAlign = HorizontalAlignment.Center;
            totalPeopleTextBox.Text = "0 total people";
            totalPeopleTextBox.TabStop = false;
            totalPeopleTextBox.BackColor = this.BackColor;

            statusTextBox = new TextBox();
            statusTextBox.Location = new Point(30, 360);
            statusTextBox.Name = "statusTextBox";
            statusTextBox.Size = new Size(200, 23);
            statusTextBox.ReadOnly = true;
            statusTextBox.TextAlign = HorizontalAlignment.Center;
            statusTextBox.Text = " ";
            statusTextBox.TabStop = false;
            statusTextBox.BackColor = this.BackColor;
            statusTextBox.BorderStyle = BorderStyle.None;

            radioSqlHandler = new RadioButton();
            radioSqlHandler.Location = new Point(700, 200);
            radioSqlHandler.Name = "radioSqlHandler";
            radioSqlHandler.Size = new Size(120, 20);
            radioSqlHandler.Text = "Use SQL";
            radioSqlHandler.Checked = true;
            radioSqlHandler.CheckedChanged += new EventHandler(RadioHandler_CheckedChanged);
            Controls.Add(radioSqlHandler);

            radioEfHandler = new RadioButton();
            radioEfHandler.Location = new Point(700, 230);
            radioEfHandler.Name = "radioEfHandler";
            radioEfHandler.Size = new Size(150, 20);
            radioEfHandler.Text = "Use EF";
            radioEfHandler.CheckedChanged += new EventHandler(RadioHandler_CheckedChanged);
            Controls.Add(radioEfHandler);

            Controls.Add(totalPeopleTextBox);
            Controls.Add(listView1);
            Controls.Add(buttonAdd);
            Controls.Add(buttonGenerate);
            Controls.Add(buttonDelete);
            Controls.Add(buttonStop);
            Controls.Add(textBox1);
            Controls.Add(textBox2);
            Controls.Add(textBox3);
            Controls.Add(buttonCancel);
            Controls.Add(statusTextBox);
            Controls.Add(textBoxSearchName);
            Controls.Add(textBoxSearchContact);
            Controls.Add(radioSqlHandler);
            Controls.Add(radioEfHandler);

            ClientSize = new Size(800, 450);
            Text = "People Manager";
            ResumeLayout(false);
            PerformLayout();
        }
    }
}
