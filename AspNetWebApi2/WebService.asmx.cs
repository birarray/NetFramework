using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Data;
using System.Net;
using System.Web.Hosting;




namespace AspNetWebApi2
{
    /// <summary>
    /// Summary description for WebService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class WebService : System.Web.Services.WebService
    {

        [WebMethod]
        public void CreateBaseXML()
        {
            if (System.IO.File.Exists(System.IO.Path.Combine(Server.MapPath("/"), "autostore.xml"))) return;
            string result = string.Empty;
            DataSet ds = new DataSet();
            DataTable dt = new DataTable("Cars");
            dt.Columns.Add("Id", typeof(string));
            dt.Columns.Add("Model", typeof(string));
            dt.Columns.Add("Produced", typeof(int));
            DataRow dr = dt.NewRow();
            dr["Id"] = Guid.NewGuid().ToString(); dr["Model"] = "BMW"; dr["Produced"] = 2010;
            dt.Rows.Add(dr);
            ds.Tables.Add(dt);
            ds.WriteXml(System.IO.Path.Combine(Server.MapPath("/"), "autostore.xml"), XmlWriteMode.WriteSchema);
        }

        [WebMethod]
        public DataSet ReadAllData(string constraint)
        {
            DataSet result = new DataSet();
            DataSet ds = new DataSet();
            ds.ReadXml(System.IO.Path.Combine(Server.MapPath("/"), "autostore.xml"), XmlReadMode.ReadSchema);
            if (string.IsNullOrEmpty(constraint))
            {
                return ds;

            }
            else
            {
                var query = ds.Tables[0].AsEnumerable().Where(x =>
                x[0].ToString().ToLower().Contains(constraint.ToLower()) |
                x[1].ToString().ToLower().Contains(constraint.ToLower()) |
                x[0].ToString().Contains(constraint)).AsDataView().ToTable();
                result.Tables.Add(query.Copy());
            }
            return result;
        }

        [WebMethod]
        public string DeleteData(string Id)
        {
            string result = string.Empty;
            DataSet ds = new DataSet();

            try
            {
                ds.ReadXml(System.IO.Path.Combine(Server.MapPath("/"), "autostore.xml"), XmlReadMode.ReadSchema);
                ds.Tables[0].AsEnumerable().Where(x => x[0].ToString() == Id).FirstOrDefault().Delete();
                ds.WriteXml(System.IO.Path.Combine(Server.MapPath("/"), "autostore.xml"), XmlWriteMode.WriteSchema);
            }
            catch (Exception err)
            {
                result = err.Message;
            }
            return result;
        }

        [WebMethod]
        public string AddData(string Model, int Produced)
        {
            string result = string.Empty;
            DataSet ds = new DataSet();

            try
            {
                ds.ReadXml(System.IO.Path.Combine(Server.MapPath("/"), "autostore.xml"), XmlReadMode.ReadSchema);
                DataRow dr = ds.Tables[0].NewRow();
                dr[0] = Guid.NewGuid().ToString(); dr[1] = Model; dr[2] = Produced;
                ds.Tables[0].Rows.Add(dr);
                ds.WriteXml(System.IO.Path.Combine(Server.MapPath("/"), "autostore.xml"), XmlWriteMode.WriteSchema);
            }
            catch (Exception err)
            {
                result = err.Message;
            }

            return result;
        }

        [WebMethod]
        public string EditData(string id, string Model, int Produced)
        {
            string result = string.Empty;
            DataSet ds = new DataSet();
            try
            {
                ds.ReadXml(System.IO.Path.Combine(Server.MapPath("/"), "autostore.xml"), XmlReadMode.ReadSchema);
                ds.Tables[0].AsEnumerable().Where(x => x[0].ToString() == id).ToList().ForEach(s => { s[1] = Model; s[2] = Produced; });
                ds.WriteXml(System.IO.Path.Combine(Server.MapPath("/"), "autostore.xml"), XmlWriteMode.WriteSchema);
            }
            catch (Exception err)
            {
                result = err.Message;
            }
            return result;
        }
    }
}
