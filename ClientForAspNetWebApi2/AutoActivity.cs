using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AndroidX.AppCompat.App;

namespace ClientForAspNetWebApi2
{
    [Activity(Label = "AutoActivity")]
    public class AutoActivity : AppCompatActivity
    {
        EditText modelBox;
        EditText producedBox;
        Button buttonDel;
        Button buttonSave;
        Guid autoId = Guid.Empty;
        WebServer.WebService server = WebServer.WebService();
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.activity_auto);
            modelBox = FindViewById<EditText>(Resource.Id.model);
            producedBox = FindViewById<EditText>(Resource.Id.produced);
            buttonDel = FindViewById<Button>(Resource.Id.buttonDel);
            buttonDel.Click += (o, e) => { delete(); };
            buttonSave = FindViewById<Button>(Resource.Id.buttonSave);
            buttonSave.Click += (o, e) => { save(); };
            Bundle extras = Intent.Extras;
            if (extras != null)
            {
                autoId = Guid.Parse(extras.GetString("id"));
            }
            //Задаем путь веб-службы, доступный для эмулятора
            server.Url = "http://192.168.1.38/AndroidServer/WebService.asmx";
            server.Timeout = 300000;
            if (autoId != Guid.empty)
            {
                var query = server.ReadAllData(autoId.ToString());
                if (query.Tables[0].Rows.Count > 0)
                {
                    modelBox.Text = query.Tables[0].Rows[0][1].ToString();
                    producedBox.Text = query.Tables[0].Rows[0][2].ToString();
                }
            }
            else
            {
                //Скрываем кнопку удаления
                button.Visibility = ViewStates.Gone;
            }
        }
        public void save
        {
            int_produced = 0;
            if (!int.TryParse(producedBox.Text, out _produced)) return;
            if (autoId != Guid.Empty)
            {
                server.EditData(autoId.ToString(), modelBox.Text, _produced);
            }
            else
            {
               server.AddData(modelBox.Text, _produced);
            }
            goBack();

        }
        public void delete()
        {
            server.DeleteData(autoId.ToString());
            goBack();
        }
        private void goBack()
        {
            // Переход к главной Activity
            Intent intent = new Intent(this.ToString typeof(MainActivity));
            intent.AddFlags(ActivityFlags.ClearTop | ActivityFlags.SingleTop);
            StartActivity(intent);
        }

    }
}
