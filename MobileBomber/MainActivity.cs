using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using static Android.Views.View;
using Android.Views;
using System;
using 专治骗子;
using ScriptInterpreter;
using System.Text;

namespace MobileBomber
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : Activity
    {
        TextView outputText;
        TextView statisticText;
        BomberPerformer performer;

        Handler hWnd = new Handler();
        
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);
            FindViewById<Button>(Resource.Id.btnStart).Click += MainActivity_Click;
            outputText = FindViewById<TextView>(Resource.Id.txtOutput);
            statisticText = FindViewById<TextView>(Resource.Id.txtStatus);

            FindViewById<EditText>(Resource.Id.txtScript).Text = GetSharedPreferences("main", Android.Content.FileCreationMode.Private).GetString("script", "");

            performer = null;

        }

        private void MainActivity_Click(object sender, EventArgs e)
        {
            if (null == performer)
            {
                String script = FindViewById<EditText>(Resource.Id.txtScript).Text;

                GetSharedPreferences("main", Android.Content.FileCreationMode.Private).Edit().PutString("script", script).Commit();

                int threadcount = 32;

                string[] lines = script.Split('\r', '\n');
                foreach (string line in lines)
                {
                    if (line.Trim().StartsWith("#线程数"))
                    {
                        int.TryParse(line.Replace("#线程数", "").Trim(), out threadcount);
                    }
                }

                System.Net.ServicePointManager.DefaultConnectionLimit = threadcount;
                System.Net.ServicePointManager.Expect100Continue = false;

                ScriptedBomber bomber = new ScriptedBomber(script);
                performer = new BomberPerformer(bomber);
                bomber.OnBomberComplete += Bomber_OnBomberComplete;
                performer.ThreadCount = threadcount;
                performer.StartBomber();
                ((Button)sender).Text = "停止并退出";
                successcount = 0;
                failcount = 0;
                last = DateTime.Now;
            }
            else {
                performer.StopBomber();
                Finish();
            }
        }


        private void Bomber_OnBomberComplete(object sender, BomberResultEventArgs e)
        {
            RunOnUiThread(() => onBomberResult(e) );
        }


        static int successcount = 0;
        static int failcount = 0;
        static DateTime last = DateTime.Now;

        void onBomberResult(BomberResultEventArgs e) {

            StringBuilder sb = new StringBuilder();
            if (e.BomberResult) { successcount++; } else { failcount++; }
            sb.AppendLine(e.UsesUrl);
            sb.AppendLine(e.ReturnValue);
            TimeSpan usestime = DateTime.Now - last;
            int speed = (int)(((double)successcount) / (usestime.TotalMinutes));
            statisticText.Text=("[" + "成功：" + successcount + " 失败：" + failcount + " 平均速度：" + speed + "/分钟]");
            outputText.Text = sb.ToString();
        }

        public override void OnBackPressed()
        {
            if (null == performer)
            {
                base.OnBackPressed();
            }
            else {
                Toast.MakeText(this, "请按停止来退出程序", ToastLength.Long).Show();
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}