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
        public string Test()
        {
            string str = "vnFp+ZmAKmpr2rpnaqkYNLpXC1LscvDgN1WKOIMxozcNXvAH1JUqA1x70Rq6JlfmtUKdYMch4zgffNO3e74Tc/Fka+UjC/JVt6q0XJ6Fk9Q=";
            var res = SecurityEncrypt.AesDecrypt(str);
            return res;

            //string str = "1-0-2-0-1-2-2";
            //str = str.Replace("0", "作废").Replace("1", "正常").Replace("2", "冲红");

            //logger.Info("");
            //logger.Info("");
            //logger.Info("-----------------------------------调用开始------------------------------------");
            //logger.Info("请求函数:Z9EARS");
            //logger.Info("返回结果:");
            //logger.Info("-----------------------------------调用结束------------------------------------");
            //return "Hello World";
        }

        /// <summary>
        /// 下传函数
        /// </summary>
        /// <param name="DocEntry"></param>
        /// <returns></returns>
        [WebMethod]
        public string Z9EAR(int DocEntry)
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
                result = Z9EAR_DLL(DocEntry, list);
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

        private SearchResult<List<InvoiceModel>> Z9EAR_ODBC(int DocEntry, List<InvoiceView> list)
        {
            var result = new SearchResult<List<InvoiceModel>>();

            var connstring = GetOdbcConnectionString();
            using (OdbcConnection connection = new OdbcConnection(connstring))  //创建connection连接对象
            {
                connection.Open();  //打开链接
                var sql = "SELECT  * FROM \"BS_SBO_1970_AR\".\"CBIC_AR\"";
                if (DocEntry > 0)
                {
                    sql = sql + " where \"CBIC_AR\".\"DocEntry\"=" + DocEntry;
                }
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
                            U_ARS = reader["U_ARS"] == null ? "" : reader["U_ARS"].ToString(),
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
            return result;



        }

        private SearchResult<List<InvoiceModel>> Z9EAR_DLL(int DocEntry, List<InvoiceView> list)
        {
            var result = new SearchResult<List<InvoiceModel>>();

            var connstring = GetDllConnectionString();
            using (Sap.Data.Hana.HanaConnection connection = new Sap.Data.Hana.HanaConnection(connstring))  //创建connection连接对象
            {
                connection.Open();  //打开链接
                var sql = "SELECT  * FROM \"" + GetDbName() + "\".\"CBIC_AR\"";
                if (DocEntry > 0)
                {
                    sql = sql + " where \"CBIC_AR\".\"DocEntry\"=" + DocEntry;
                }
                using (Sap.Data.Hana.HanaCommand command = new Sap.Data.Hana.HanaCommand(sql))  //command  对象
                {
                    command.Connection = connection;
                    Sap.Data.Hana.HanaDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        var viewModel = new InvoiceView();
                        viewModel.DocEntry = reader["DocEntry"] == null ? -1 : reader["DocEntry"].ToInt();
                        viewModel.DocNum = reader["DocNum"] == null ? -1 : reader["DocNum"].ToInt();
                        viewModel.TaxDate = reader["TaxDate"] == null ? DateTime.MinValue : reader["TaxDate"].ToDateTime();
                        viewModel.U_ARS = reader["U_ARS"] == null ? "" : reader["U_ARS"].ToString();
                        viewModel.U_ART = reader["U_ART"] == null ? "" : reader["U_ART"].ToString();
                        viewModel.U_ARC = reader["U_ARC"] == null ? "" : reader["U_ARC"].ToString();

                        viewModel.CardCode = reader["CardCode"] == null ? "" : reader["CardCode"].ToString();
                        viewModel.CardName = reader["CardName"] == null ? "" : reader["CardName"].ToString();
                        viewModel.GTSRegNum = reader["GTSRegNum"] == null ? "" : reader["GTSRegNum"].ToString();
                        viewModel.GTSBilAddr = reader["GTSBilAddr"] == null ? "" : reader["GTSBilAddr"].ToString();
                        viewModel.Phone1 = reader["Phone1"] == null ? "" : reader["Phone1"].ToString();
                        viewModel.U_CV_BankName = reader["U_CV_BankName"] == null ? "" : reader["U_CV_BankName"].ToString();
                        viewModel.GTSBankAct = reader["GTSBankAct"] == null ? "" : reader["GTSBankAct"].ToString();


                        viewModel.LineNum = reader["LineNum"] == null ? -1 : reader["LineNum"].ToInt();
                        viewModel.ItemCode = reader["ItemCode"] == null ? "" : reader["ItemCode"].ToString();
                        viewModel.Quantity = reader["Quantity"] == null ? -1 : reader["Quantity"].ToDecimal();
                        viewModel.unitMsr = reader["unitMsr"] == null ? "" : reader["unitMsr"].ToString();
                        viewModel.VatPrcnt = reader["VatPrcnt"] == null ? -1 : reader["VatPrcnt"].ToDecimal();
                        viewModel.GTotalSC = reader["GTotalSC"] == null ? -1 : reader["GTotalSC"].ToDecimal();

                        list.Add(viewModel);
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
            return result;
        }

        /// <summary>
        /// 回传函数
        /// </summary>
        /// <param name="DocEntry">单据主键</param>
        /// <param name="U_ARH"></param>
        /// <param name="U_ARD"></param>
        /// <param name="U_ARQ"></param>
        /// <param name="U_ARM"></param>
        /// <param name="U_ARS"></param>
        /// <returns></returns>
        [WebMethod]
        public string Z9EARS(int DocEntry, string U_ARH, string U_ARD, string U_ARQ, decimal U_ARM, string U_ARS)
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
            logger.Info("U_ARQ:" + U_ARQ);
            logger.Info("U_ARM:" + U_ARM);
            logger.Info("U_ARS:" + U_ARS);

            if (DocEntry <= 0)
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
                U_ARS = U_ARS.Replace("0", "作废").Replace("1", "正常").Replace("2", "冲红");
                result = Z9EARS_DLL(DocEntry, U_ARH, U_ARD, U_ARQ, U_ARM, U_ARS);
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

        private StandardResult Z9EARS_DLL(int DocEntry, string U_ARH, string U_ARD, string U_ARQ, decimal U_ARM, string U_ARS)
        {
            var result = new StandardResult();
            var connstring = GetDllConnectionString();
            using (Sap.Data.Hana.HanaConnection connection = new Sap.Data.Hana.HanaConnection(connstring))  //创建connection连接对象
            {
                connection.Open();  //打开链接
                var sql = new System.Text.StringBuilder();

                //SQL拼接,无参数化
                sql.Append("UPDATE \"" + GetDbName() + "\".\"OINV\" SET ");
                sql.AppendFormat("\"OINV\".\"U_ARH\" =\'{0}\',", U_ARH);
                sql.AppendFormat("\"OINV\".\"U_ARD\" =\'{0}\',", U_ARD);
                sql.AppendFormat("\"OINV\".\"U_ARQ\" =\'{0}\',", U_ARQ);
                sql.AppendFormat("\"OINV\".\"U_ARM\" =\'{0}\',", U_ARM);
                sql.AppendFormat("\"OINV\".\"U_ARS\" =\'{0}\'", U_ARS);
                sql.AppendFormat(" where \"OINV\".\"DocEntry\"={0}", DocEntry);

                //参数化
                //sql.Append("UPDATE \"BS_SBO_1970_AR\".\"OINV\" SET ");
                //sql.AppendFormat("\"OINV\".\"U_ARH\" =:U_ARH,", U_ARH);
                //sql.AppendFormat("\"OINV\".\"U_ARD\" =:U_ARD,", U_ARD);
                //sql.AppendFormat("\"OINV\".\"U_ARQ\" =:U_ARQ,", U_ARQ);
                //sql.AppendFormat("\"OINV\".\"U_ARE\" =:U_ARE,", U_ARE);
                //sql.AppendFormat("\"OINV\".\"U_ARS\" =:U_ARS", U_ARS);
                //sql.AppendFormat(" where \"OINV\".\"DocEntry\"=:DocEntry", DocEntry);

                var strSql = sql.ToString();

                using (Sap.Data.Hana.HanaCommand command = new Sap.Data.Hana.HanaCommand(strSql))//command  对象
                {
                    command.Connection = connection;
                    command.Parameters.Add(new Sap.Data.Hana.HanaParameter() { ParameterName = ":DocEntry", HanaDbType = Sap.Data.Hana.HanaDbType.Integer, Value = DocEntry });
                    command.Parameters.Add(new Sap.Data.Hana.HanaParameter() { ParameterName = ":U_ARH", HanaDbType = Sap.Data.Hana.HanaDbType.VarChar, Value = U_ARH });
                    command.Parameters.Add(new Sap.Data.Hana.HanaParameter() { ParameterName = ":U_ARD", HanaDbType = Sap.Data.Hana.HanaDbType.VarChar, Value = U_ARD });
                    command.Parameters.Add(new Sap.Data.Hana.HanaParameter() { ParameterName = ":U_ARQ", HanaDbType = Sap.Data.Hana.HanaDbType.VarChar, Value = U_ARQ });
                    command.Parameters.Add(new Sap.Data.Hana.HanaParameter() { ParameterName = ":U_ARM", HanaDbType = Sap.Data.Hana.HanaDbType.Decimal, Value = U_ARM });

                    command.Parameters.Add(new Sap.Data.Hana.HanaParameter() { ParameterName = ":U_ARS", HanaDbType = Sap.Data.Hana.HanaDbType.VarChar, Value = U_ARS });


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
            return result;
        }

        private string GetOdbcConnectionString()
        {
            try
            {
                var dsn = System.Configuration.ConfigurationManager.AppSettings["DSNName"].ToString();
                var uid = System.Configuration.ConfigurationManager.AppSettings["Uid"].ToString();
                var pwd = System.Configuration.ConfigurationManager.AppSettings["Pwd"].ToString();

                //dsn = DES.Decrypt(dsn);
                //uid = DES.Decrypt(uid);
                //pwd = DES.Decrypt(pwd);
                String connstring = string.Format("DSN={0};Uid={1};Pwd={2}", dsn, uid, pwd);  //ODBC连接字符串
                return connstring;
            }
            catch
            {
                throw new Exception("配置文件中缺少ODBC相关连接信息");
            }

        }

        private string GetDllConnectionString()
        {
            try
            {
                var strCon = System.Configuration.ConfigurationManager.AppSettings["SAPConnectionString"].ToString();
                //var s = SecurityEncrypt.AesDecrypt(strCon);
                return strCon; 
            }
            catch
            {
                throw new Exception("配置文件中缺少HANA相关连接信息");
            }
        }

        private string GetDbName()
        {
            try
            {
                var dbName = System.Configuration.ConfigurationManager.AppSettings["DbName"].ToString();
                return dbName;
            }
            catch
            {
                throw new Exception("配置文件中缺少数据库名称");
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
