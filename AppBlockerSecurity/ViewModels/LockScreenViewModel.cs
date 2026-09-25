using AppBlockerSecurity.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AppBlockerSecurity.ViewModels
{
    public partial class LockScreenViewModel : ObservableObject
    {
        private readonly MathPuzzleService _aiService;

        [ObservableProperty]
        public partial int NumberA { get; set; }

        [ObservableProperty]
        public partial int NumberB { get; set; }

        [ObservableProperty]
        public partial string UserAnswer { get; set; } = string.Empty;

        [ObservableProperty]
        public partial string ErrorMessage { get; set; } = string.Empty;

        public LockScreenViewModel(MathPuzzleService aiService)
        {
            _aiService = aiService;
            GenerateNewPuzzle();

            // inicializa a IA assincronamente assim que o ViewModel é criado
            Task.Run(async () => await _aiService.InitializeAsync());
        }

        private void GenerateNewPuzzle()
        {
            var random = new Random();
            NumberA = random.Next(1, 10);
            NumberB = random.Next(1, 10);
            UserAnswer = string.Empty;
            ErrorMessage = string.Empty;
        }

        [RelayCommand]
        private async Task UnlockAsync()
        {
            if (!float.TryParse(UserAnswer, out float answer))
            {
                ErrorMessage = "Please enter a valid number.";
                return;
            }

            bool isSuccess = _aiService.TryUnlock(NumberA, NumberB, answer);

            if (isSuccess)
            {
                // A IA CARIMBA O ACESSO POR 60 SEGUNDOS AQUI
                SecurityManager.AuthorizeAccess(60);

                // 1. remove a tela de bloqueio
                await Shell.Current.Navigation.PopModalAsync();

                // 2. minimiza o App Blocker silenciosamente, revelando o app logo atrás
#if ANDROID
                var activity = Platform.CurrentActivity;
                activity?.MoveTaskToBack(true);
#endif
            }
            else
            {
                ErrorMessage = "Access Denied by AI. Please try again.";
                GenerateNewPuzzle();
            }
        }
    }
}