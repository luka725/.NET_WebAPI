﻿using DataTransferObjects;
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
                ClientForm clientForm = new ClientForm(userid);
                Hide();
                clientForm.Show();
                clientForm.FormClosed += (s, args) => Show();
            }
        }

        private async Task<bool> AuthenticateUser(string email, string password)
        {
            try
            {
                
                var user = new UsersDTO { Email = email, Password = password };
                var content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");
                var response = await client.PostAsync("https://localhost:44322/api/auth", content);
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

