using DataTransferObjects;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OrdersClientApplication
{
    public partial class LoginForm : Form
    {
        private readonly HttpClient client;
        public LoginForm()
        {
            InitializeComponent();
            client = new HttpClient();
        }
        private async void LoginBtn_Click(object sender, EventArgs e)
        {
            var email = EmailText.Text;
            var password = PasswordText.Text;
            var isAuthenticated = await AuthenticateUser(email, password);

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                return;
            }
            if (isAuthenticated)
            {
                int userid = await GetUserId(email);
                ClientForm clientForm = new ClientForm(userid, client);
                Hide();
                clientForm.Show();
                clientForm.FormClosed += (s, args) => Show();
            }
        }

        private async Task<bool> AuthenticateUser(string em, string pas)
        {
            string email = em;
            string password = pas;
            string credentials = $"{email}:{password}";
            string encodedCredentials = Convert.ToBase64String(Encoding.ASCII.GetBytes(credentials));
            client.BaseAddress = new Uri("https://localhost:44322/");
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", encodedCredentials);
            try
            {
                HttpResponseMessage response = await client.PostAsync("api/auth/login", null);

                if (response.IsSuccessStatusCode)
                {
                    // Authentication successful
                    string responseBody = await response.Content.ReadAsStringAsync();
                    return true;
                }
                else
                {
                    // Authentication failed
                    MessageBox.Show($"Failed to authenticate. Status code: {response.StatusCode}");
                    return false;
                }
            }
            catch (HttpRequestException ex)
            {
                MessageBox.Show($"{ex.Message}");
                return false;
            }
        }
        private async Task<int> GetUserId(string email)
        {
            try
            {
                var Email = new UsersDTO { Email = email }; 
                var content = new StringContent(JsonConvert.SerializeObject(Email), Encoding.UTF8, "application/json");
                var response = await client.PostAsync("https://localhost:44322/api/auth/getID", content);

                if (response.IsSuccessStatusCode)
                {
                    var userIdJson = await response.Content.ReadAsStringAsync();
                    int userId = JsonConvert.DeserializeObject<int>(userIdJson);
                    return userId;
                }
                else if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    string errorMessage = await response.Content.ReadAsStringAsync();
                    if (!string.IsNullOrEmpty(errorMessage))
                    {
                        MessageBox.Show(errorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        MessageBox.Show("Invalid email", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    string errorMessage = $"Error: {response.StatusCode} - {response.ReasonPhrase}";
                    MessageBox.Show(errorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                return 0;
            }
            catch (HttpRequestException ex)
            {
                MessageBox.Show($"Error getting user ID by email: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }
    }
}

