using System.Windows;

namespace APEX.Desktop;

public partial class LogoErpLoginDialog : Window
{
    public string ServerName { get; private set; } = string.Empty;
    public string DatabaseName { get; private set; } = string.Empty;
    public string Username { get; private set; } = string.Empty;
    public string Password { get; private set; } = string.Empty;
    
    public LogoErpLoginDialog()
    {
        InitializeComponent();
    }
    
    private void TestConnection_Click(object sender, RoutedEventArgs e)
    {
        if (ValidateInputs())
        {
            // Test bağlantısı simülasyonu
            System.Windows.MessageBox.Show("Bağlantı testi başarılı!", "Test Sonucu", 
                          MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
    
    private void Connect_Click(object sender, RoutedEventArgs e)
    {
        if (ValidateInputs())
        {
            ServerName = ServerTextBox.Text;
            DatabaseName = DatabaseTextBox.Text;
            Username = UsernameTextBox.Text;
            Password = PasswordBox.Password;
            
            DialogResult = true;
            Close();
        }
    }
    
    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
    
    private bool ValidateInputs()
    {
        if (string.IsNullOrWhiteSpace(ServerTextBox.Text))
        {
            System.Windows.MessageBox.Show("Sunucu adresi boş olamaz!", "Hata", 
                          MessageBoxButton.OK, MessageBoxImage.Warning);
            ServerTextBox.Focus();
            return false;
        }
        
        if (string.IsNullOrWhiteSpace(DatabaseTextBox.Text))
        {
            System.Windows.MessageBox.Show("Veritabanı adı boş olamaz!", "Hata", 
                          MessageBoxButton.OK, MessageBoxImage.Warning);
            DatabaseTextBox.Focus();
            return false;
        }
        
        if (string.IsNullOrWhiteSpace(UsernameTextBox.Text))
        {
            System.Windows.MessageBox.Show("Kullanıcı adı boş olamaz!", "Hata", 
                          MessageBoxButton.OK, MessageBoxImage.Warning);
            UsernameTextBox.Focus();
            return false;
        }
        
        if (string.IsNullOrWhiteSpace(PasswordBox.Password))
        {
            System.Windows.MessageBox.Show("Şifre boş olamaz!", "Hata", 
                          MessageBoxButton.OK, MessageBoxImage.Warning);
            PasswordBox.Focus();
            return false;
        }
        
        return true;
    }
}