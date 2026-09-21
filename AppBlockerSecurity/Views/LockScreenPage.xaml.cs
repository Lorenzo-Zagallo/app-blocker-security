using AppBlockerSecurity.ViewModels;

namespace AppBlockerSecurity.Views;

public partial class LockScreenPage : ContentPage
{
	public LockScreenPage(LockScreenViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}

    // impede que o usuário feche a tela pelo botão nativo de voltar do Android
    protected override bool OnBackButtonPressed()
    {
        return true; // retorna true para indicar que o evento foi tratado e não deve fechar a página
    }
}