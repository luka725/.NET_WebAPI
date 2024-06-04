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
            List<OrdersDTO> orders = await FetchOrdersForUser(userID);

            PopulateDataGridView(orders);
        }
        private async Task<List<OrdersDTO>> FetchOrdersForUser(int userId)
        {
            string baseAddress = "https://localhost:44322/";

            string requestUrl = $"{baseAddress}api/orders/user?id={userId}";
                try
                {
                    HttpResponseMessage response = await Client.GetAsync(requestUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        string responseBody = await response.Content.ReadAsStringAsync();

                        List<OrdersDTO> orders = JsonConvert.DeserializeObject<List<OrdersDTO>>(responseBody);
                        return orders;
                    }
                    else
                    {
                        MessageBox.Show($"Failed to retrieve orders. Status code: {response.StatusCode}");
                    }
                }
                catch (HttpRequestException ex)
                {
                    MessageBox.Show($"An error occurred: {ex.Message}");
                }
            return new List<OrdersDTO>();
        }
        private void PopulateDataGridView(List<OrdersDTO> orders)
        {
            OrdersDgv.Rows.Clear();
            OrdersDgv.ColumnCount = 2;
            OrdersDgv.Columns[0].Name = "OrderId";
            OrdersDgv.Columns[1].Name = "OrderDate";
            foreach (var order in orders)
            {
                OrdersDgv.Rows.Add(order.ID, order.OrderDate);
            }
        }
        private void ClientForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }
    }
}
