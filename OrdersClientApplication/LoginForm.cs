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

namespace OrdersClientApplication
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
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
                Hide();
                ClientForm clientForm = new ClientForm();
                clientForm.Show();
                clientForm.FormClosed += (s, args) => Show();
            }
        }

        private async Task<bool> AuthenticateUser(string email, string password)
        {
            try
            {
                var httpClient = new HttpClient();
                var user = new UsersDTO { Email = email, Password = password };
                var content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync("https://localhost:44322/api/auth", content);
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    MessageBox.Show("Authentication failed. Please check your email and password.");
                    return false;
                }
            }
            catch (HttpRequestException ex)
            {
                MessageBox.Show($"An error occurred while sending the request: {ex.Message}");
                return false;
            }
        }
    }
}

