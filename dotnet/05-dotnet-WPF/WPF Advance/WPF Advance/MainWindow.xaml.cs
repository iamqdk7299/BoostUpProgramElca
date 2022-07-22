using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace WPF_Advance
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        [DllImport("Kernel32.dll", CharSet = CharSet.Auto)]
        static extern System.UInt16 SetThreadUILanguage(System.UInt16 LangId);

        public ObservableCollection<Language> Languages { get; set; } = new ObservableCollection<Language>();

        public MainWindow()
        {
            InitializeComponent();
            InitializeLanguages();
            DataContext = this;
        }

        private void InitializeLanguages()
        {
            Languages.Add(new Language
            {
                Display = "English",
                Code = "en-US",
                FontStyle = WPF_Advance.FontStyle.Bold
            });
            Languages.Add(new Language
            {
                Display = "Deutsche",
                Code = "de-DE",
                FontStyle = WPF_Advance.FontStyle.Italic
            });
            Languages.Add(new Language
            {
                Display = "中文",
                Code = "zh-CN",
                FontStyle = WPF_Advance.FontStyle.Normal
            });
        }

        private void btnOpenFiles_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                txtFileName.Text = openFileDialog.FileName;
            }
        }

        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox comboBox)
            {
                var cultureInfo = CultureInfo.GetCultureInfo(comboBox.SelectedValue.ToString());
                SetThreadUILanguage((System.UInt16)cultureInfo.LCID);
                Thread.CurrentThread.CurrentUICulture = cultureInfo;
                RefreshCurrentLanguage();
            }
        }

        private void RefreshCurrentLanguage()
        {
            txtLanguage.Text = WPF_Advance.Resources.Language.Languages;
            txtOpenFile.Content = WPF_Advance.Resources.Language.OpenFile;
            txtOpenFile2.Content = WPF_Advance.Resources.Language.OpenFile;
        }
    }

    public class Language
    {
        public string Display { get; set; }

        public string Code { get; set; }

        public FontStyle FontStyle { get; set; }
    }

    public enum FontStyle
    {
        Normal,
        Bold,
        Italic
    }
}
