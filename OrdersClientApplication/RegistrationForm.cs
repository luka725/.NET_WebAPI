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
using System.Xml.Linq;

namespace OrdersClientApplication
{
    public partial class RegistrationForm : Form
    {
        private readonly HttpClient Client;
        public RegistrationForm(HttpClient client)
        {
            InitializeComponent();
            Client = client;
        }

        private async void RegisterBtn_Click(object sender, EventArgs e)
        {
            if (PassTxt.Text != ConfPassTxt.Text)
            {
                MessageBox.Show("Password and Confirm Password do not match.");
                return;
            }

            UsersDTO registerUserDTO = new UsersDTO
            {
                Name = NameTxt.Text,
                Email = EmailTxt.Text,
                Password = PassTxt.Text,
            };

            int? userId = await RegisterUserAsync(registerUserDTO);

            if (userId.HasValue)
            {
                MessageBox.Show($"Registration successful! User ID: {userId.Value}");
            }
            else
            {
                MessageBox.Show("Registration failed.");
            }
        }
        private async Task<int?> RegisterUserAsync(UsersDTO registerUserDTO)
        {
            string baseAddress = "https://localhost:44322/";
            string requestUrl = $"{baseAddress}api/auth/register";

            try
            {
                string jsonContent = JsonConvert.SerializeObject(registerUserDTO);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await Client.PostAsync(requestUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<dynamic>(responseBody);
                    int userId = result.userId;
                    LoginForm loginForm = new LoginForm();
                    loginForm.Show();
                    Close();
                    return userId;
                }
                else
                {
                    MessageBox.Show($"Failed to register user. Status code: {response.StatusCode}");
                    return null;
                }
            }
            catch (HttpRequestException ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
                return null;
            }
        }
    }
}
