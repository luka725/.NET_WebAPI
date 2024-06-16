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
        private int currentPage = 1;
        private int pageSize = 5;
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
            List<OrdersDTO> orders = await FetchOrdersForUser(userID, 1);

            PopulateDataGridView(orders);
            //LoadData();
        }
        private async Task FetchAndPopulateOrders()
        {
            List<OrdersDTO> orders = await FetchOrdersForUser(userID, currentPage);
            PopulateDataGridView(orders);
            NextBtn.Enabled = orders.Any();
            PrevBtn.Enabled = currentPage > 1;
            PageNumber.Text = currentPage.ToString();
        }
        private async Task<List<OrdersDTO>> FetchOrdersForUser(int userId, int page)
        {
            string baseAddress = "https://localhost:44322/";
            string requestUrl = $"{baseAddress}api/orders/user?id={userId}&page={page}";

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
            PageNumber.Text = currentPage.ToString();
        }
        private void ClientForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

        private async void PrevBtn_Click(object sender, EventArgs e)
        {
            if (currentPage > 1)
            {
                currentPage--;
                var orders = await FetchOrdersForUser(userID, currentPage, pageSize);
                PopulateDataGridView(orders);
                NextBtn.Enabled = true;
            }
            PrevBtn.Enabled = currentPage > 1;
        }

        private async void NextBtn_Click(object sender, EventArgs e)
        {
                currentPage++;
                var orders = await FetchOrdersForUser(userID, currentPage, pageSize);
                PopulateDataGridView(orders);
                NextBtn.Enabled = orders.Count == pageSize;
                PrevBtn.Enabled = currentPage > 1;
        }
        private async Task<List<OrderDetailProductDTO>> FetchOrdersForUser(int userId, int page, int limit)
        {
            string baseAddress = "https://localhost:44322/";
            string requestUrl = $"{baseAddress}api/orders/user?id={userId}&page={page}&limit={limit}";

            try
            {
                HttpResponseMessage response = await Client.GetAsync(requestUrl);

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    List<OrderDetailProductDTO> orders = JsonConvert.DeserializeObject<List<OrderDetailProductDTO>>(responseBody);
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
            return new List<OrderDetailProductDTO>();
        }
        private void PopulateDataGridView(List<OrderDetailProductDTO> orders)
        {
            OrdersDgv.Rows.Clear();
            OrdersDgv.Columns.Clear();

            OrdersDgv.ColumnCount = 6;
            OrdersDgv.Columns[0].Name = "OrderID";
            OrdersDgv.Columns[1].Name = "OrderDate";
            OrdersDgv.Columns[2].Name = "OrderDetailID";
            OrdersDgv.Columns[3].Name = "ProductID";
            OrdersDgv.Columns[4].Name = "ProductName";
            OrdersDgv.Columns[5].Name = "Quantity";

            foreach (var order in orders)
            {
                OrdersDgv.Rows.Add(order.OrderID, order.OrderDate, order.OrderDetailID, order.ProductID, order.ProductName, order.Quantity, order.UnitPrice);
            }
            PageNumber.Text = currentPage.ToString();
        }
        private async void LoadData()
        {
            var orders = await FetchOrdersForUser(userID, currentPage, pageSize);
            PopulateDataGridView(orders);

            NextBtn.Enabled = orders.Count == pageSize;
            PrevBtn.Enabled = currentPage > 1;
        }
    }
}
