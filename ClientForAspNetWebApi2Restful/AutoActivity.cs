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
using System.Net.Http;
using System.Web;
using Newtonsoft.Json;

namespace ClientForAspNetWebApi2Restful
{
    [Activity(Label = "AutoActivity", Theme = "@style/AppTheme")]
    public class AutoActivity : AppCompatActivity
    {
        EditText modelBox;
        EditText producedBox;
        Button buttonDel;
        Button buttonSave;
        Guid autoId = Guid.Empty;
        HttpClient _client = new HttpClient();
        string UrlWebApi = "http://192.168.1.38/AndroidServer";
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
            
            if (autoId != Guid.Empty)
            {
                getData(autoId.ToString());
            }
            else
            {
                //Скрываем кнопку удаления
                button.Visibility = ViewStates.Gone;
            }
            _client.Timeout = new TimeSpan(0, 0, 300);
        }

        async void getData(string constraint)
        {
            // Получение данных при помощи сервиса
            var response = await _client.GetAsync(string.Format(UrlWebApi + "/api/Auto/GetAllData/?constraint={0}", HttpUtility.UrlEncode(constraint)));
            var obj = await response.Content.ReadAsStringAsync();
            var cars = JsonConvert.DeserializeObject<List<DataCar>>(obj);
            if (cars.Count > 0)
            {
                modelBox.Text = cars[0].model.ToString();
                producedBox.Text = cars[0].produced.ToString();
            }
        }
        async void save
        {
            int_produced = 0;
            if (!int.TryParse(producedBox.Text, out _produced)) return;
            if (autoId != Guid.Empty)
            {
                var response = await _client.GetAsync(string.Format(UrlWebApi + "/api/Auto/GetEditData/?Id={0}&Model={1}&Produced={2}", autoId.ToString(), HttpUtility.UrlEncode(modelBox.Text), _produced.ToString()));
            }
            else
            {
               var response = await _client.GetAsync(string.Format(UrlWebApi + "/api/Auto/GetAddData/?Model={0}&Produced={1}", HttpUtility.UrlEncode(modelBox.Text), _produced.ToString()));
            }
            goBack();

        }
        async void delete()
        {
        var response = await _client.GetAsync(string.Format(UrlWebApi + "/api/Auto/GetDeleteData/?Id={0}", autoId.ToString()));
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