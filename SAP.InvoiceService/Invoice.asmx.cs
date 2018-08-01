using NLog;
using SAP.InvoiceService.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Linq;
using System.Web;
using System.Web.Services;
using SAP.InvoiceService.Extensions;

namespace SAP.InvoiceService
{
    /// <summary>
    /// Invoice 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
    // [System.Web.Script.Services.ScriptService]
    public class Invoice : System.Web.Services.WebService
    {

        [WebMethod]
        public string HelloWorld()
        {
            return "Hello World";
        }

        public string Z9EAR()
        {
            Logger logger = LogManager.GetCurrentClassLogger();
            List<InvoiceView> list = new List<InvoiceView>();
            try
            {
                var sql = "SELECT  * FROM \"BS_SBO_1970_AR\".\"CBIC_AR\"";
                OdbcCommand command = new OdbcCommand(sql);  //command  对象
                var dsn = System.Configuration.ConfigurationManager.AppSettings["DSNName"].ToString();
                var uid = System.Configuration.ConfigurationManager.AppSettings["Uid"].ToString();
                var pwd = System.Configuration.ConfigurationManager.AppSettings["Pwd"].ToString();

                String connstring = string.Format("DSN={0};Uid={1};Pwd={2}", dsn, uid, pwd);  //ODBC连接字符串
                using (OdbcConnection connection = new OdbcConnection(connstring))  //创建connection连接对象
                {
                    command.Connection = connection;
                    connection.Open();  //打开链接

                    OdbcDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        list.Add(new InvoiceView()
                        {
                            DocEntry = reader["DocEntry"] == null ? -1 : reader["DocEntry"].ToInt(),
                            DocNum = reader["DocNum"] == null ? -1 : reader["DocNum"].ToInt(),
                            TaxDate = reader["TaxDate"] == null ? DateTime.MinValue : reader["TaxDate"].ToDateTime(),
                            U_ARS = reader["U_ARS"] == null ? -1 : reader["U_ARS"].ToInt(),
                            U_ART = reader["U_ART"] == null ? "" : reader["U_ART"].ToString(),

                            CardCode = reader["CardCode"] == null ? "" : reader["CardCode"].ToString(),
                            CardName = reader["CardName"] == null ? "" : reader["CardName"].ToString(),
                            GTSRegNum = reader["GTSRegNum"] == null ? "" : reader["GTSRegNum"].ToString(),
                            GTSBilAddr = reader["GTSBilAddr"] == null ? "" : reader["GTSBilAddr"].ToString(),
                            Phone1 = reader["Phone1"] == null ? "" : reader["Phone1"].ToString(),
                            U_CV_BankName = reader["U_CV_BankName"] == null ? "" : reader["U_CV_BankName"].ToString(),
                            GTSBankAct = reader["GTSBankAct"] == null ? "" : reader["GTSBankAct"].ToString(),

                            DocEntry1 = reader["GTSBankAct"] == null ? -1 : reader["GTSBankAct"].ToInt(),
                            LineNum = reader["LineNum"] == null ? -1 : reader["LineNum"].ToInt(),
                            ItemCode = reader["ItemCode"] == null ? "" : reader["ItemCode"].ToString(),
                            Quantity = reader["Quantity"] == null ? -1 : reader["Quantity"].ToDecimal(),
                            unitMsr = reader["unitMsr"] == null ? "" : reader["unitMsr"].ToString(),
                            VatPrcnt = reader["VatPrcnt"] == null ? -1 : reader["VatPrcnt"].ToDecimal(),
                            GTotalSC = reader["GTotalSC"] == null ? -1 : reader["GTotalSC"].ToDecimal(),
                        });
                    }
                    reader.Close();
                }

                list.GroupBy(x => new { x.DocEntry, x.DocNum, x.TaxDate, x.U_ARS, x.U_ART, x.U_ARC, x.CardCode, x.CardName, x.GTSRegNum, x.GTSBilAddr, x.Phone1, x.U_CV_BankName, x.GTSBankAct })
                    .Select(x => new
                    {
                        DocEntry = x.Key.DocEntry,
                        DocNum = x.Key.DocNum,
                        TaxDate = x.Key.TaxDate,
                        U_ARS = x.Key.U_ARS,
                        U_ART = x.Key.U_ART,
                        U_ARC = x.Key.U_ARC,
                        CardCode = x.Key.CardCode,
                        CardName = x.Key.CardName,
                        GTSRegNum = x.Key.GTSBilAddr,
                        GTSBilAddr = x.Key.GTSRegNum,
                        Phone1 = x.Key.Phone1,
                        U_CV_BankName = x.Key.U_CV_BankName,
                        GTSBankAct = x.Key.GTSBankAct
                    });


                var result = Newtonsoft.Json.JsonConvert.SerializeObject(list);
                return result;
            }
            catch (Exception ex)
            {
                return "错误:" + ex.Message;
                // throw new Exception(ex.Message);
            }
        }





    }
}
