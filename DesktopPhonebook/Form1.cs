using PhonebookServices;
using WebPhonebook.Interfaces;
using WebPhonebook.Models;

namespace DesktopPhonebook
{
    public partial class Form1 : Form
    {
        private List<Person> peopleList = new List<Person>();
        private Person selectedPerson = null;
        private CancellationTokenSource cancellationTokenSource = null;
        private int namesGenerated = 0;
        private DateTime? startTime = null;
        private IDatabaseHandler dbHandler;
        private readonly SqlDatabaseHandler _sqlDatabaseHandler;
        private readonly EfDatabaseHandler _efDatabaseHandler;
        private readonly NameLoader _nameLoader;

        public Form1(SqlDatabaseHandler sqlDatabaseHandler, EfDatabaseHandler efDatabaseHandler, NameLoader nameLoader)
        {
            InitializeComponent();
            dbHandler = sqlDatabaseHandler;

            _efDatabaseHandler = efDatabaseHandler;
            _sqlDatabaseHandler = sqlDatabaseHandler;
            _nameLoader = nameLoader;

            nameLoader.LoadNamesFromFiles();

            sqlDatabaseHandler.InitializeDatabase();
            LoadPeople();
        }

        private void LoadPeople(string filterByName = "", string filterByContact = "")
        {
            peopleList = dbHandler.LoadPeople(filterByName, filterByContact)

            .Take(10000)
            .ToList();

            if (InvokeRequired)
            {
                Invoke(() =>
                {
                    UpdateTotalPeopleCount();
                    UpdateListView();
                });
            }
            else
            {
                UpdateTotalPeopleCount();
                UpdateListView();
            }
        }

        private void UpdateTotalPeopleCount()
        {
            int totalCount = peopleList.Count;
            string formattedCount = string.Format("{0} loaded people", totalCount);
            totalPeopleTextBox.Text = formattedCount;
        }

        private void UpdateListView()
        {
            listView1.BeginUpdate();
            listView1.Items.Clear();

            foreach (var person in peopleList)
            {
                var item = new ListViewItem(person.Name);
                item.SubItems.Add(person.PhoneNumber);
                item.SubItems.Add(person.Email);
                item.Tag = person.Id;
                listView1.Items.Add(item);
            }

            listView1.EndUpdate();
        }

        private void textBoxSearchName_TextChanged(object sender, EventArgs e)
        {
            LoadPeople(textBoxSearchName.Text, textBoxSearchContact.Text);
        }

        private void textBoxSearchContact_TextChanged(object sender, EventArgs e)
        {
            LoadPeople(textBoxSearchName.Text, textBoxSearchContact.Text);
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            var name = textBox1.Text.Trim();
            var phone = textBox2.Text.Trim();
            var email = textBox3.Text.Trim();

            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(phone) || string.IsNullOrEmpty(email))
            {
                MessageBox.Show("All fields must be filled.");
                return;
            }

            var existingPerson = peopleList.FirstOrDefault(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            if (existingPerson != null && selectedPerson == null)
            {
                foreach (ListViewItem item in listView1.Items)
                {
                    if (item.Text.Equals(name, StringComparison.OrdinalIgnoreCase))
                    {
                        item.Selected = true;
                        item.EnsureVisible();
                        MessageBox.Show("This name already exists. Selecting the existing entry for editing.");
                        return;
                    }
                }
            }

            if (selectedPerson == null)
            {
                var newPerson = new Person(0, name, phone, email);
                dbHandler.AddPerson(newPerson);
                LoadPeople();
                textBox1.Clear();
                textBox2.Clear();
                textBox3.Clear();
            }
            else
            {
                selectedPerson.PhoneNumber = phone;
                selectedPerson.Email = email;
                selectedPerson.Name = name;
                dbHandler.UpdatePerson(selectedPerson);
                LoadPeople();
                textBox1.Clear();
                textBox2.Clear();
                textBox3.Clear();
            }
            UpdateTotalPeopleCount();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            selectedPerson = null;
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            listView1.SelectedItems.Clear();
            UpdateButtonState();
        }

        private void buttonGenerate_Click(object sender, EventArgs e)
        {
            if (cancellationTokenSource != null)
            {
                MessageBox.Show("Generation is already running.");
                return;
            }

            cancellationTokenSource = new CancellationTokenSource();
            var token = cancellationTokenSource.Token;

            Task.Run(async () =>
            {
                try
                {
                    await GenerateUniqueNames(token, UpdateStatus, AfterGeneration);
                }
                catch (OperationCanceledException)
                {
                    MessageBox.Show("Generation canceled.");
                }
                finally
                {
                    cancellationTokenSource = null;
                }
            });
        }

        private async Task GenerateUniqueNames(CancellationToken cancellationToken,
            Action<string> updateStatus, Action<int, double, bool> afterGeneration)
        {
            await dbHandler.GenerateUniqueNames(_nameLoader, cancellationToken, updateStatus, afterGeneration);
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            if (selectedPerson == null) return;

            dbHandler.DeletePerson(selectedPerson.Id);
            LoadPeople();
            UpdateTotalPeopleCount();
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            selectedPerson = null;
            UpdateButtonState();
        }

        private void buttonStop_Click(object sender, EventArgs e)
        {
            if (cancellationTokenSource != null)
            {
                cancellationTokenSource.Cancel();
                cancellationTokenSource = null;
            }
            else
            {
                MessageBox.Show("No generation process is running.");
            }
        }

        private void ClearStatus()
        {
            if (statusTextBox.InvokeRequired)
            {
                statusTextBox.Invoke(new Action(() => statusTextBox.Text = string.Empty));
            }
            else
            {
                statusTextBox.Text = string.Empty;
            }
        }

        private void AfterGeneration(int countAdded, double elapsedTime, bool wasStopped)
        {
            LoadPeople();
            ClearStatus();

            string message = wasStopped
                ? $"Generation was stopped. {countAdded} names were added in {elapsedTime:F2} seconds."
                : $"Generation complete. {countAdded} unique names added in {elapsedTime:F2} seconds.";

            MessageBox.Show(message);
        }

        public void UpdateStatus(string message)
        {
            if (statusTextBox.InvokeRequired)
            {
                statusTextBox.Invoke(new Action(() => statusTextBox.Text = message));
            }
            else
            {
                statusTextBox.Text = message;
            }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0)
            {
                selectedPerson = null;
                textBox1.Clear();
                textBox2.Clear();
                textBox3.Clear();
                UpdateButtonState();
                return;
            }

            var selectedItem = listView1.SelectedItems[0];
            var selectedId = (int)selectedItem.Tag;
            selectedPerson = peopleList.FirstOrDefault(p => p.Id == selectedId);

            if (selectedPerson != null)
            {
                textBox1.Text = selectedPerson.Name;
                textBox2.Text = selectedPerson.PhoneNumber;
                textBox3.Text = selectedPerson.Email;
            }

            UpdateButtonState();
        }

        private void UpdateButtonState()
        {
            buttonAdd.Text = selectedPerson == null ? "Add" : "Save";
            buttonDelete.Enabled = selectedPerson != null;
            buttonCancel.Visible = selectedPerson != null;
        }

        private void RadioHandler_CheckedChanged(object sender, EventArgs e)
        {
            if (radioSqlHandler.Checked)
            {
                dbHandler = _sqlDatabaseHandler;
            }
            else if (radioEfHandler.Checked)
            {
                dbHandler = _efDatabaseHandler;
            }

            _sqlDatabaseHandler.InitializeDatabase();
            LoadPeople();
        }
    }
}
