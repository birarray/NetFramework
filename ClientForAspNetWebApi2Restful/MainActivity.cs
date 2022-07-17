using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using AndroidX.AppCompat.App;
using Android.Widget;
using Android.Content;
using System;
using System.Data;
using System.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Web;
using Newtonsoft.Json;


namespace ClientForAspNetWebApi2Restful
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        ListView autoList;
        ArrayAdapter autoAdapter;
        HttpClient _client = new HttpClient();
        string UrlWebApi = "http://192.168.1.38/AndroidServer";

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            autoList = FindViewById<ListView>(Resource.Id.list);
            var btn = FindViewById<Button>(Resource.Id.btn);
            btn.Click += (o, e) =>
            {
                Intent intent = new Intent(ApplicationContext, typeof(AutoActivity));
                StartActivity(intent);
            };
            autoList.ItemClick += (o, e) =>
            {
                Auto selectedCar = (Auto)e.Parent.GetItemAtPosition(e.Position);
                Intent intent = new Intent(ApplicationContext, typeof(AutoActivity));
                intent.PutExtra("id", selected.id);
                StartActivity(intent);
            };
            _client.Timeout = new TimeSpan(0, 0, 300);
            createDB();
        }
            async void createDB()
            { 
            try
            {
                var response = await _client.GetAsync(string.Format(UrlWebApi + "/api/Auto/GetBaseXML"));
            }
            catch (Exception err)
            {
                Toast.MakeText(this, err.Message, ToastLength.Long).Show();
            }
            }
        protected override void OnResume()
        {
            base.OnResume();
            GetDatabasePath(string.Empty);
        }
        async void getData(string constraint)
        {
            try
            {
                // Получение данных при помощи сервиса
                var response = await _client.GetAsync(string.Format(UrlWebApi + "/api/Auto/GetAllData/?constraint={0}", HttpUtility.UrlEncode(constraint)));

                var obj = await response.Content.ReadAsStringAsync();
                var _data = JsonConvert.DeserializeObject<List<DataCar>>(obj);
                if (_data.Count > 0)
                {
                    List<Auto> cars = new List<Auto>(_data.Select(m => new Auto()
                    {
                        id = m.id,
                        model = m.model,
                        produced = m.Produced
                    }));
                    // Создаем адаптер, передаем в него список
                    var autoAdapter = new AutoAdapter(this, this, Resource.Layout.list_item, cars);
                    autoList.Adapter = autoAdapter;
                }
                else
                {
                    autoList.Adapter = null;
                }
            }
            catch (Exception err)
            {
                Toast.MakeText(this, err.Message, ToastLength.Long).Show();
            }
        }
    }
    public class DataCar

    {
        public string id { get; set; } // идентификатор
        public string model { get; set; } // модель
        public int produced { get; set; } // год выпуска
    }
    public class Auto: Java.Lang.Object

    {
        public string id { get; set; } // идентификатор
        public string model { get; set; } // модель
        public int produced { get; set; } // год выпуска
    }

    public class AutoAdapter : ArrayAdapter
    {
        private Activity activity;
        public AutoAdapter(Activity activity, Context context, int resource, List<Auto> objects) : base(context, resource, objects)
        {
            this.activity = activity;
        }
        public override Android.Views.View GetView(int position, Android.Views.View convertView, Android.Views.ViewGroup parent)
        {
            var view = (convertView ?? activity.LayoutInflater.Inflate(Resource.Layout.list_item, parent, false)) as LinearLayout;
            TextView modelView = view.FindViewById<TextView>(ResourceCursorAdapter.Id.model);
            TextView producedView = view.FindViewById<TextView>(ResourceCursorAdapter.Id.produced);
            var item = (Auto)this.GetItem(position);
            modelView.Text = item.model;
            producedView.Text = item.produced.ToString();
            return view;
        }
    }
}
/*
    public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}
*/