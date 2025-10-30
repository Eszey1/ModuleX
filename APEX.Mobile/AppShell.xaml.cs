namespace APEX.Mobile
{
    public partial class AppShell : Shell
    {
        public AppShell(MainPage mainPage)
        {
            InitializeComponent();
            
            // MainPage'i Shell'e ekle
            Items.Clear();
            Items.Add(new ShellContent
            {
                Title = "SayımPro",
                Content = mainPage,
                Route = "MainPage"
            });
            
            // Register routes for navigation
            Routing.RegisterRoute("BarcodeScannerPage", typeof(Views.BarcodeScannerPage));
            
            // Register Logo ERP routes
            Routing.RegisterRoute("LogoErpLogin", typeof(Views.LogoErpLoginPage));
            Routing.RegisterRoute("LogoErpDashboard", typeof(Views.LogoErpDashboardPage));
            Routing.RegisterRoute("LogoErpProductList", typeof(Views.LogoErpProductListPage));
            Routing.RegisterRoute("LogoErpStock", typeof(Views.LogoErpStockPage));
        }
    }
}
