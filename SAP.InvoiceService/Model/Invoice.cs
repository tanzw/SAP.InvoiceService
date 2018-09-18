using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAP.InvoiceService.Model
{
    public class InvoiceModel
    {
        /// <summary>
        /// 单据主键
        /// </summary>
        public int DocEntry { get; set; }

        /// <summary>
        /// ERP订单号
        /// </summary>
        public int DocNum { get; set; }

        /// <summary>
        /// 订单日期
        /// </summary>
        public DateTime TaxDate { get; set; }

        /// <summary>
        /// 发票状态
        /// </summary>
        public string U_ARS { get; set; }

        /// <summary>
        /// 发票类型
        /// </summary>
        public string U_ART { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string U_ARC { get; set; }

        /// <summary>
        /// 客户编码
        /// </summary>
        public string CardCode { get; set; }

        /// <summary>
        /// 客户姓名
        /// </summary>
        public string CardName { get; set; }

        /// <summary>
        /// 税号
        /// </summary>
        public string GTSRegNum { get; set; }

        /// <summary>
        /// 开票地址
        /// </summary>
        public string GTSBilAddr { get; set; }

        /// <summary>
        /// 开票电话
        /// </summary>
        public string Phone1 { get; set; }

        /// <summary>
        /// 银行名称
        /// </summary>
        public string U_CV_BankName { get; set; }

        /// <summary>
        /// 金税账号
        /// </summary>
        public string GTSBankAct { get; set; }

        /// <summary>
        /// 开票地址+电话
        /// </summary>
        public string Contact { get; set; }

        /// <summary>
        /// 银行名称+金税帐号
        /// </summary>
        public string BankAccount { get; set; }

        /// <summary>
        /// 发票明细
        /// </summary>
        public List<Detail> DetailList { get; set; }
    }

    public class Detail
    {
        /// <summary>
        /// 行编号
        /// </summary>
        public int LineNum { get; set; }

        /// <summary>
        /// 物料编号
        /// </summary>
        public string ItemCode { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public decimal Quantity { get; set; }

        /// <summary>
        /// 计量单位
        /// </summary>
        public string unitMsr { get; set; }

        /// <summary>
        /// 每行税率
        /// </summary>
        public decimal VatPrcnt { get; set; }

        /// <summary>
        /// 含税总额
        /// </summary>
        public decimal GTotalSC { get; set; }
    }

    public class InvoiceView
    {
        /// <summary>
        /// 单据主键
        /// </summary>
        public int DocEntry { get; set; }

        /// <summary>
        /// ERP订单号
        /// </summary>
        public int DocNum { get; set; }

        /// <summary>
        /// 订单日期
        /// </summary>
        public DateTime TaxDate { get; set; }

        /// <summary>
        /// 发票状态
        /// </summary>
        public string U_ARS { get; set; }

        /// <summary>
        /// 发票类型
        /// </summary>
        public string U_ART { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string U_ARC { get; set; }

        /// <summary>
        /// 客户编码
        /// </summary>
        public string CardCode { get; set; }

        /// <summary>
        /// 客户姓名
        /// </summary>
        public string CardName { get; set; }

        /// <summary>
        /// 税号
        /// </summary>
        public string GTSRegNum { get; set; }

        /// <summary>
        /// 开票地址
        /// </summary>
        public string GTSBilAddr { get; set; }

        /// <summary>
        /// 开票电话
        /// </summary>
        public string Phone1 { get; set; }

        /// <summary>
        /// 银行名称
        /// </summary>
        public string U_CV_BankName { get; set; }

        /// <summary>
        /// 金税账号
        /// </summary>
        public string GTSBankAct { get; set; }

        /// <summary>
        /// 行编号
        /// </summary>
        public int LineNum { get; set; }

        /// <summary>
        /// 物料编号
        /// </summary>
        public string ItemCode { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public decimal Quantity { get; set; }

        /// <summary>
        /// 计量单位
        /// </summary>
        public string unitMsr { get; set; }

        /// <summary>
        /// 每行税率
        /// </summary>
        public decimal VatPrcnt { get; set; }

        /// <summary>
        /// 含税总额
        /// </summary>
        public decimal GTotalSC { get; set; }
    }

}