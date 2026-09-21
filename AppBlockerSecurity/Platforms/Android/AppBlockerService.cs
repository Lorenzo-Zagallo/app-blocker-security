using Android.AccessibilityServices;
using Android.App;
using Android.Content;
using Android.Views.Accessibility;

namespace AppBlockerSecurity.Platforms.Android
{
    [Service(
        Label = "App Blocker Security", 
        Permission = "android.permission.BIND_ACCESSIBILITY_SERVICE", 
        Exported = true,
        ForegroundServiceType = global::Android.Content.PM.ForegroundService.TypeDataSync)]
    [IntentFilter(new[] { "android.accessibilityservice.AccessibilityService" })]
    [MetaData("android.accessibilityservice", Resource = "@xml/accessibility_service_config")]
    public class AppBlockerService : AccessibilityService
    {
        // lista de pacotes que queremos bloquear para teste (ex: configurações do android e youtube)
        private readonly List<string> _blockedApps = new()
        {
            "com.android.settings", // Settings
            "com.google.android.youtube", // YouTube

            //"com.android.chrome", // Chrome
            //"com.facebook.katana", // Facebook
            //"com.instagram.android", // Instagram
            //"com.whatsapp", // WhatsApp
            //"com.snapchat.android", // Snapchat
            //"com.twitter.android", // Twitter
            //"com.reddit.frontpage", // Reddit
            //"com.tiktok.android", // TikTok
            //"com.netflix.mediaclient", // Netflix
            //"com.spotify.music" // Spotify
        };

        private DateTime _lastTriggerTime = DateTime.MinValue;
        private string _lastPackage = "";

        // coloque este método dentro da classe AppBlockerService, logo acima do OnAccessibilityEvent
        protected override void OnServiceConnected()
        {
            base.OnServiceConnected();

            var channelId = "app_blocker_channel";

            // utilizando 'as' para evitar o aviso CS8600 de conversão nula
            var notificationManager = GetSystemService(NotificationService) as NotificationManager;

            // cria o canal de notificação exigido pelo Android moderno
            //if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            //{
            //    var channel = new NotificationChannel(channelId, "Serviço de Segurança", NotificationImportance.Min);
            //    notificationManager?.CreateNotificationChannel(channel);
            //}

            // o compilador do .NET 10 reconhece esta validação e remove o aviso CA1416
            if (OperatingSystem.IsAndroidVersionAtLeast(26))
            {
                var channel = new NotificationChannel(channelId, "Serviço de Segurança", NotificationImportance.Min);
                notificationManager?.CreateNotificationChannel(channel);
            }

            // constrói a notificação que mantém o app vivo.
            // usando a biblioteca de compatibilidade AndroidX
            var notification = new AndroidX.Core.App.NotificationCompat.Builder(this, channelId)
                .SetContentTitle("App Blocker")!
                .SetContentText("Monitoramento de IA ativo")!
                // utilizando 'global::' para forçar a leitura do recurso nativo do Android
                .SetSmallIcon(global::Android.Resource.Drawable.IcSecure)! // ícone de cadeado nativo do Android
                .SetOngoing(true)!
                .Build();

            // o ID (1001) não pode ser zero. Isso blinda o app contra o encerramento.
            // ANDROID 14: Passamos o tipo explicitamente na chamada
            if (OperatingSystem.IsAndroidVersionAtLeast(29))
            {
                StartForeground(1001, notification, global::Android.Content.PM.ForegroundService.TypeDataSync);
            }
            else
            {
                StartForeground(1001, notification);
            }
        }

        public override void OnAccessibilityEvent(AccessibilityEvent? e)
        {
            if (e?.EventType == EventTypes.WindowStateChanged)
            {
                string? packageName = e.PackageName?.ToString();

                if (!string.IsNullOrEmpty(packageName) && _blockedApps.Contains(packageName))
                {
                    // se for o mesmo app e passou menos de 60 segundos, ignora!
                    if (packageName == _lastPackage && (DateTime.Now - _lastTriggerTime).TotalSeconds < 60)
                        return;

                    // Salva quem foi o último bloqueado e que horas foi
                    _lastPackage = packageName;
                    _lastTriggerTime = DateTime.Now;

                    TriggerLockScreen();
                }
            }
        }

        private void TriggerLockScreen()
        {
            // cria uma intenção para trazer o nosso aplicativo (MainActivity) de volta para a tela
            var intent = new Intent(this, typeof(MainActivity));

            // flags OBRIGATÓRIAS: limpam a pilha e forçam o nosso app a sobrepor o app bloqueado
            intent.AddFlags(ActivityFlags.NewTask | ActivityFlags.ClearTask | ActivityFlags.SingleTop);
            intent.PutExtra("ShowLockScreen", true);

            StartActivity(intent);
        }

        public override void OnInterrupt() { }
    }
}