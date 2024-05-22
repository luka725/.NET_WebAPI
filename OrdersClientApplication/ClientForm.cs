using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OrdersClientApplication
{
    public partial class ClientForm : Form
    {
        private readonly int userID;
        public ClientForm(int id)
        {
            InitializeComponent();
            userID = id;
            label1.Text = userID.ToString();
        }
    }
}
