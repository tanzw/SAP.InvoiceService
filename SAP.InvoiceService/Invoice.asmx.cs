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
        Logger logger = LogManager.GetCurrentClassLogger();

        [WebMethod]
        public string HelloWorld()
        {
            logger.Info("");
            logger.Info("");
            logger.Info("-----------------------------------调用开始------------------------------------");
            logger.Info("请求函数:Z9EARS");
            logger.Info("返回结果:");
            logger.Info("-----------------------------------调用结束------------------------------------");
            return "Hello World";
        }

        public string Z9EAR()
        {
            var json = string.Empty;
            var result = new SearchResult<List<InvoiceModel>>();
            logger.Info("");
            logger.Info("");
            logger.Info("-----------------------------------调用开始------------------------------------");
            logger.Info("请求函数:Z9EAR");
            logger.Info("请求时间:" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

            List<InvoiceView> list = new List<InvoiceView>();
            try
            {
                var connstring = GetOdbcConnectionString();
                using (OdbcConnection connection = new OdbcConnection(connstring))  //创建connection连接对象
                {
                    connection.Open();  //打开链接
                    var sql = "SELECT  * FROM \"BS_SBO_1970_AR\".\"CBIC_AR\"";
                    using (OdbcCommand command = new OdbcCommand(sql))  //command  对象
                    {
                        command.Connection = connection;
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
                }

                var invoiceList = list.GroupBy(x => new { x.DocEntry, x.DocNum, x.TaxDate, x.U_ARS, x.U_ART, x.U_ARC, x.CardCode, x.CardName, x.GTSRegNum, x.GTSBilAddr, x.Phone1, x.U_CV_BankName, x.GTSBankAct })
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
                          GTSRegNum = x.Key.GTSRegNum,
                          GTSBilAddr = x.Key.GTSBilAddr,
                          Phone1 = x.Key.Phone1,
                          U_CV_BankName = x.Key.U_CV_BankName,
                          GTSBankAct = x.Key.GTSBankAct
                      }).ToList();
                var invoice = new List<InvoiceModel>();
                invoiceList.ForEach(x =>
                {
                    var data = new InvoiceModel()
                    {
                        DocEntry = x.DocEntry,
                        DocNum = x.DocNum,
                        TaxDate = x.TaxDate,
                        U_ARS = x.U_ARS,
                        U_ART = x.U_ART,
                        U_ARC = x.U_ARC,
                        CardCode = x.CardCode,
                        CardName = x.CardName,
                        GTSRegNum = x.GTSRegNum,
                        GTSBilAddr = x.GTSBilAddr,
                        Phone1 = x.Phone1,
                        U_CV_BankName = x.U_CV_BankName,
                        GTSBankAct = x.GTSBankAct,
                        Contact = x.GTSBilAddr + x.Phone1,
                        BankAccount = x.U_CV_BankName + x.GTSBankAct,
                        DetailList = new List<Detail>()

                    };
                    invoice.Add(data);

                    var temp = list.Where(q => q.DocEntry == x.DocEntry).ToList();
                    temp.ForEach(q =>
                    {
                        data.DetailList.Add(new Detail()
                        {
                            GTotalSC = q.GTotalSC,
                            ItemCode = q.ItemCode,
                            LineNum = q.LineNum,
                            Quantity = q.Quantity,
                            unitMsr = q.unitMsr,
                            VatPrcnt = q.VatPrcnt
                        });
                    });
                });
                logger.Info("返回结果行数:" + invoice.Count);
                result.Code = "Y";
                result.Msg = "成功";
                result.TotalCount = invoice.Count;
                result.Body = invoice;
            }
            catch (Exception ex)
            {
                logger.Info("异常:" + ex.Message);
                result.Code = "N";
                result.Msg = "失败," + ex.Message;
            }
            json = Newtonsoft.Json.JsonConvert.SerializeObject(result);
            logger.Info("-----------------------------------调用结束------------------------------------");
            return json;
        }

        public string Z9EARS(string DocEntry, string U_ARH, string U_ARD, DateTime U_ARR, decimal U_ARE, string U_ARS)
        {
            var json = string.Empty;
            var result = new StandardResult();
            logger.Info("");
            logger.Info("");
            logger.Info("-----------------------------------调用开始------------------------------------");
            logger.Info("请求函数:Z9EARS");
            logger.Info("请求时间:" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            logger.Info("请求参数:");
            logger.Info("DocEntry:" + DocEntry);
            logger.Info("U_ARH:" + U_ARH);
            logger.Info("U_ARD:" + U_ARD);
            logger.Info("U_ARR:" + U_ARR);
            logger.Info("U_ARE:" + U_ARE);
            logger.Info("U_ARS:" + U_ARS);

            if (string.IsNullOrWhiteSpace(DocEntry))
            {
                result.Code = "N";
                result.Msg = "DocEntry参数不能为空";
            }
            if (string.IsNullOrWhiteSpace(U_ARH))
            {
                result.Code = "N";
                result.Msg = "U_ARH参数不能为空";
            }
            if (string.IsNullOrWhiteSpace(U_ARS))
            {
                result.Code = "N";
                result.Msg = "U_ARS参数不能为空";
            }
            if (result.Code == "N")
            {
                json = Newtonsoft.Json.JsonConvert.SerializeObject(result);
                logger.Info("返回结果:" + json);
                logger.Info("-----------------------------------调用结束------------------------------------");
                return json;
            }

            try
            {
                var connstring = GetOdbcConnectionString();
                using (OdbcConnection connection = new OdbcConnection(connstring))  //创建connection连接对象
                {
                    connection.Open();  //打开链接
                    var sql = "UPDATE \"BS_SBO_1970_AR\".\"CBIC_AR\" SET U_ARH = :U_ARH, U_ARD = :U_ARD, U_ARR = :U_ARR, U_ARE = :U_ARE, U_ARS = :U_ARS  WHERE DocEntry = :DocEntry ";
                    using (OdbcCommand command = new OdbcCommand(sql))//command  对象
                    {
                        command.Connection = connection;
                        command.Parameters.Add(new OdbcParameter() { ParameterName = ":DocEntry" });
                        command.Parameters.Add(new OdbcParameter() { ParameterName = ":U_ARH" });
                        command.Parameters.Add(new OdbcParameter() { ParameterName = ":U_ARD" });
                        command.Parameters.Add(new OdbcParameter() { ParameterName = ":U_ARR" });
                        command.Parameters.Add(new OdbcParameter() { ParameterName = ":U_ARE" });
                        command.Parameters.Add(new OdbcParameter() { ParameterName = ":U_ARS" });
                        if (command.ExecuteNonQuery() > 0)
                        {
                            result.Code = "Y";
                            result.Msg = "成功";
                        }
                        else
                        {
                            result.Code = "N";
                            result.Msg = "失败,更新影响行数为0";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result.Code = "N";
                result.Msg = ex.Message;
            }
            json = Newtonsoft.Json.JsonConvert.SerializeObject(result);
            logger.Info("返回结果:" + json);
            logger.Info("-----------------------------------调用结束------------------------------------");
            return json;
        }


        private string GetOdbcConnectionString()
        {
            try
            {
                var dsn = System.Configuration.ConfigurationManager.AppSettings["DSNName"].ToString();
                var uid = System.Configuration.ConfigurationManager.AppSettings["Uid"].ToString();
                var pwd = System.Configuration.ConfigurationManager.AppSettings["Pwd"].ToString();

                String connstring = string.Format("DSN={0};Uid={1};Pwd={2}", dsn, uid, pwd);  //ODBC连接字符串
                return connstring;
            }
            catch
            {
                throw new Exception("配置文件中缺少相关连接信息");
            }

        }
    }

    public class StandardResult
    {
        public string Code { get; set; }

        public string Msg { get; set; }
    }

    public class SearchResult<T> : StandardResult
    {
        public int TotalCount { get; set; }
        public T Body { get; set; }
    }
}
