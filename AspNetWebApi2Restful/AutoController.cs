using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data;
using System.Web;
using System.Web.Hosting;

namespace AspNetWebApi2Restful
{
    public class Auto
    {
        public string id { get; set; } // идентификатор
        public string model { get; set; } // модель
        public int produced { get; set; } // год выпуска
    }
    public class AutoController : ApiController
    {
        public string GetEditData(string id, string Model, int Produced)
        {
            string result = string.Empty;
            DataSet ds = new DataSet();
            try
            {
                ds.ReadXml(System.IO.Path.Combine(HostingEnvironment.MapPath("/"), "autostore.xml"), XmlReadMode.ReadSchema);
                ds.Tables[0].AsEnumerable().Where(x => x[0].ToString() == id).ToList().ForEach(s => { s[1] = Model; s[2] = Produced; });
                ds.WriteXml(System.IO.Path.Combine(HostingEnvironment.MapPath("/"), "autostore.xml"), XmlWriteMode.WriteSchema);
            }
            catch (Exception err)
            {
                result = err.Message;
            }
            return result;
        }

        public string GetAddData(string Model, int Produced)
        {
            string result = string.Empty;
            DataSet ds = new DataSet();

            try
            {
                ds.ReadXml(System.IO.Path.Combine(HostingEnvironment.MapPath("/"), "autostore.xml"), XmlReadMode.ReadSchema);
                DataRow dr = ds.Tables[0].NewRow();
                dr[0] = Guid.NewGuid().ToString(); dr[1] = Model; dr[2] = Produced;
                ds.Tables[0].Rows.Add(dr);
                ds.WriteXml(System.IO.Path.Combine(HostingEnvironment.MapPath("/"), "autostore.xml"), XmlWriteMode.WriteSchema);
            }
            catch (Exception err)
            {
                result = err.Message;
            }

            return result;
        }
        public string GetDeleteData(string Id)
        {
            string result = string.Empty;
            DataSet ds = new DataSet();

            try
            {
                ds.ReadXml(System.IO.Path.Combine(HostingEnvironment.MapPath("/"), "autostore.xml"), XmlReadMode.ReadSchema);
                ds.Tables[0].AsEnumerable().Where(x => x[0].ToString() == Id).FirstOrDefault().Delete();
                ds.WriteXml(System.IO.Path.Combine(HostingEnvironment.MapPath("/"), "autostore.xml"), XmlWriteMode.WriteSchema);
            }
            catch (Exception err)
            {
                result = err.Message;
            }
            return result;
        }
        public string GetBaseXML()
        {
            string result = string.Empty;
            try
            {
                if (System.IO.File.Exists(System.IO.Path.Combine(HostingEnvironment.MapPath("/"), "autostore.xml"))) return result;
                var ds = new System.Data.DataSet();
                DataTable dt = new DataTable("Cars");
                dt.Columns.Add("Id", typeof(string));
                dt.Columns.Add("Model", typeof(string));
                dt.Columns.Add("Produced", typeof(int));
                DataRow dr = dt.NewRow();
                dr["Id"] = Guid.NewGuid().ToString(); dr["Model"] = "BMW"; dr["Produced"] = 2010;
                dt.Rows.Add(dt);
                ds.Tables.Add(dt);
                ds.WriteXml(System.IO.Path.Combine(HostingEnvironment.MapPath("/"), "autostore.xml"), XmlWriteMode.WriteSchema);

            }
            catch (Exception err)
            {
                result = err.Message;
            }
            return result;
        }
        
        public List<Auto> GetAllData(string constraint)
        {
            List<Auto> result = new List<Auto>();
            DataSet ds = new DataSet();
            ds.ReadXml(System.IO.Path.Combine(HostingEnvironment.MapPath("/"), "autostore.xml"), XmlReadMode.ReadSchema);
            if (string.IsNullOrEmpty(constraint))
            {
                result = new List<Auto>(ds.Tables[0].Rows.OfType<DataRow>().Select(m => new Auto()
                {
                    id = m["Id"].ToString(),
                    model = m["Model"].ToString(),
                    produced = (int)m["Produced"]
                }));

            }
            else
            {
                var query = ds.Tables[0].AsEnumerable().Where(x =>
                x[0].ToString().ToLower().Contains(constraint.ToLower()) |
                x[1].ToString().ToLower().Contains(constraint.ToLower()) |
                x[0].ToString().Contains(constraint)).AsDataView().ToTable();
                result = new List<Auto>(query.Rows.OfType<DataRow>().Select(m => new Auto()
                {
                    id = m["Id"].ToString(),
                    model = m["Model"].ToString(),
                    produced = (int)m["Produced"]
                }));
            }
            return result;
        }









        /*
        // GET api/<controller>
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<controller>/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<controller>
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
        */
    }
}