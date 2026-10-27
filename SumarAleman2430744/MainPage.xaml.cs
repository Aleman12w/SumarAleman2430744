namespace SumarAleman2430744
{
    public partial class MainPage : ContentPage
    {
        private readonly LocalDbService _dbService;
        private int _editResultadoId;

        public MainPage(LocalDbService dbService)
        {
            InitializeComponent();
            _dbService = dbService;
            Task.Run(async () => listView.ItemsSource = await _dbService.GetResultado());
        }

        private void OnCounterClicked(object? sender, EventArgs e)
        {
            
        }

        private async void sumarBtn_Clicked(object sender, EventArgs e)
        {
            // 1. Validar y calcular la suma si los valores son numéricos
            if (double.TryParse(Entryprimernumero.Text, out double num1) &&
                double.TryParse(Entrysegundonumero.Text, out double num2))
            {
                double suma = num1 + num2;
                Labelresultado.Text = suma.ToString();
            }
            else
            {
                await DisplayAlert("Error", "Por favor ingresa números válidos en ambos campos", "OK");
                return;
            }

            // 2. Guardar o actualizar en la base de datos
            if (_editResultadoId == 0)
            {
                // Agrega registro
                await _dbService.Create(new Resultado
                {
                    Numero1 = Entryprimernumero.Text,
                    Numero2 = Entrysegundonumero.Text,
                    Suma = Labelresultado.Text
                });
            }
            else
            {
                // Edita registro
                await _dbService.Update(new Resultado
                {
                    Id = _editResultadoId,
                    Numero1 = Entryprimernumero.Text,
                    Numero2 = Entrysegundonumero.Text,
                    Suma = Labelresultado.Text
                });
                _editResultadoId = 0;
            }

            // 3. Limpiar los campos y actualizar la lista
            Entryprimernumero.Text = string.Empty;
            Entrysegundonumero.Text = string.Empty;
            Labelresultado.Text = string.Empty;

            listView.ItemsSource = await _dbService.GetResultado();
        }

        private async void listView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var resultado = (Resultado)e.Item;
            var action = await DisplayActionSheet("Action", "Cancel", null, "Edit", "Delete");

            switch (action)
            {
                case "Edit":
                    _editResultadoId = resultado.Id;
                    Entryprimernumero.Text = resultado.Numero1;
                    Entrysegundonumero.Text = resultado.Numero2;
                    Labelresultado.Text = resultado.Suma;
                    break;

                case "Delete":
                    await _dbService.Delete(resultado);
                    listView.ItemsSource = await _dbService.GetResultado();
                    break;
            }
        }
    }
}
