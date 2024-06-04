using DataTransferObjects;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace OrdersClientApplication
{
    public partial class ClientForm : Form
    {
        private readonly HttpClient Client;
        private readonly int userID;
        public ClientForm(int id, HttpClient client)
        {
            InitializeComponent();
            userID = id;
            Client = client;
        }
        private async Task<UsersDTO> GetUserInfo(int id)
        {
            try
            {
                var user = new UsersDTO { ID = id };
                var content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");
                var response = await Client.PostAsync("https://localhost:44322/api/auth/getUser", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var userInfo = JsonConvert.DeserializeObject<UsersDTO>(responseContent);
                    return userInfo;
                }
                else
                {
                    MessageBox.Show("Authentication failed. Please check your email and password.");
                    return null;
                }
            }
            catch (HttpRequestException ex)
            {
                MessageBox.Show($"An error occurred while sending the request: {ex.Message}");
                return null;
            }
        }

        private async void ClientForm_Load(object sender, EventArgs e)
        {
            var user = await GetUserInfo(userID);
            label1.Text = $"Hello {user.Name}";
        }

        private void ClientForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }
    }
}
