using Android.App;
using Android.OS;
using Android.Runtime;
using AndroidX.AppCompat.App;
using Android.Widget;
using Android.Content;
using System;
using System.Data;
using System.Linq;
using System.Collections.Generic;


namespace ClientForAspNetWebApi2
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        ListView autoList;
        ArrayAdapter autoAdapter;
        WebServer.WebService server = new WebServer.WebService();

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
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
                AutoActivity selectedUser = (Auto)e.Parent.GetItemAtPosition(e.Position);
                Intent intent = new Intent(ApplicationContext, typeof(AutoActivity));
                intent.PutExtra("id", selectedUser.id);
                StartActivity(intent);
            };
            // Задаем путь веб-службы, доступный для эмулятора
            server.Url = "http://192.168.1.38/AndroidServer/WebService.asmx";
            server.Timeout = 300000;
            try
            {
                server.CreateBaseXML;
            }
            catch (Exception err)
            {
                Toast.MakeText(this, err.Message, ToastLength.Long).Show();
            }
        }
        protected override void OnResume()
        {
            base.OnResume();
            try
            {
                // Определяем, какие столбцы из курсора будут выводиться на ListView
                var table = server.ReadAllData(string.Empty).Tables[0];
                List<Auto> cars = new List<Auto>(table.Rows.OfType<DataRow>().Select(m => new Auto()
                {
                    id = m["Id"].ToString(),
                    model = m["Model"].ToString(),
                    produced = (int)m["Produced"]
                }));
                // Создаем адаптер, передаем в него список
                var autoAdapter = new AutoAdapter(this, this, Resource.Layout.list_item, cars);
                autoList.Adapter = autoAdapter;
            }
            catch (Exception err)
            {
                Toast.MakeText(this, err.Message, ToastLength.Long).Show();
            }
        }
    }
    public class Auto : Java.Lang.Object

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