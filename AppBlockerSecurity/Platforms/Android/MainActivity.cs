using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using AppBlockerSecurity.Views;

namespace AppBlockerSecurity
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            CheckForLockScreen(Intent);
        }

        // OnNewIntent é disparado se o app já estiver rodando no fundo e for chamado novamente.
        // OnNewIntent é chamado quando a atividade já está em execução e recebe uma nova intenção (Intent).
        // Isso é útil para lidar com casos em que o usuário tenta abrir o aplicativo novamente enquanto ele já está ativo.
        protected override void OnNewIntent(Intent? intent)
        {
            base.OnNewIntent(intent);
            CheckForLockScreen(intent);
        }

        private void CheckForLockScreen(Intent? intent)
        {
            if (intent?.GetBooleanExtra("ShowLockScreen", false) == true)
            {
                // remove o extra para não disparar duas vezes
                intent.RemoveExtra("ShowLockScreen");

                // aciona a interface gráfica na Thread principal do MAUI
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    // pegamos a tela de bloqueio que injetamos no MauiProgram.cs.
                    // forma correta de pegar o serviço injetado dentro de uma Activity nativa
                    var lockPage = IPlatformApplication.Current!.Services.GetRequiredService<LockScreenPage>();

                    // navega para a tela de bloqueio e usa o 'false' para desligar a animação e ocultar a tela de baixo instantaneamente
                    await Shell.Current.Navigation.PushModalAsync(page: lockPage, animated: false);
                });
            }
        }
    }
}