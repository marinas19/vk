using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using VkNet;
using VkNet.Model.RequestParams;

namespace Vk
{
    /// <summary>
    /// Interaction logic for Authorizaion.xaml
    /// </summary>
    public partial class Authorizaion : Window
    {
        public Authorizaion()
        {
            InitializeComponent();
            WebBrowser.LoadCompleted += WebBrowser_LoadCompleted;
            WebBrowser.Navigate("https://oauth.vk.com/authorize?client_id=6384901&display=page&scope=groups,status,wall&redirect_uri=https://oauth.vk.com/blank.html&scope=groups,status,wall&response_type=token&v=5.52");
            
        }

        public string Token { get; private set; }

        private void WebBrowser_LoadCompleted(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
           // BrowserUtils.DeleteEveryCookie(WebBrowser.Source);
            var wb = sender as WebBrowser;
            var url = wb?.Source?.ToString();
            if (url == null)
                return;

            if (url.Contains("access_token="))
            {
                GetUserToken(url);
            }
        }

        private void GetUserToken(string url)
        {
            char[] symbols = { '=', '&' };
            var urls = url.Split(symbols);
            Token = urls[1];
            DialogResult = true;
            Close();
        }
    }
}
