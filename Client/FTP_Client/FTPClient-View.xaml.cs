using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FTP_Client
{
    /// <summary>
    /// Interaction logic for FTPClient_View.xaml
    /// </summary>
    public partial class FTPClient_View : UserControl
    {
        public FTPClient_View()
        {
            InitializeComponent();
            DataContext = new FTPClient_ViewModel();
        }

        private void connectBtn_Click(object sender, RoutedEventArgs e)
        {
            tabControl1.SelectedIndex = 1;
        }

        private void ftpOkBtn_Click(object sender, RoutedEventArgs e)
        {
            tabControl1.SelectedIndex = 0;
        }

        private void ftpCancelBtn_Click(object sender, RoutedEventArgs e)
        {
            tabControl1.SelectedIndex = 0;
        }

        private void setPathBtn_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog myDlg = new Microsoft.Win32.OpenFileDialog();

            if (myDlg.ShowDialog() == true)
            {
                srcPathLabel.Text = myDlg.FileName;
            }
        }

        private void usbBtn_Click(object sender, RoutedEventArgs e)
        {
            tabControl1.SelectedIndex = 2;
        }

        private void usbCloseBtn_Click(object sender, RoutedEventArgs e)
        {
            tabControl1.SelectedIndex = 0;
        }

        private bool CheckIfNumeric(string input)
        {
            foreach (char c in input)
            {
                if (!Char.IsNumber(c)) 
                    return false;
            }
            return true;
        }

        //Numerical input ONLY for port number
        private void portnumBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !CheckIfNumeric(e.Text);
            base.OnPreviewTextInput(e);
        }

        private void usbSlctBtn_Click(object sender, RoutedEventArgs e)
        {
            if (usbListBox.SelectedValue != null)
            {
                tabControl1.SelectedIndex = 0;
            }
            else
            {
                MessageBox.Show("Please Select USB Drive", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }

        private void statusLogBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            statusLogBox.ScrollToEnd();
        }
        
    }
}
