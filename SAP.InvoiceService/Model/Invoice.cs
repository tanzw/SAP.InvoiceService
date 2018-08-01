using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAP.InvoiceService.Model
{
    public class Invoice
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
        public int U_ARS { get; set; }

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
    }

    public class Detail
    {
        public int DocEntry { get; set; }

        public int LineNum { get; set; }

        public string ItemCode { get; set; }

        public decimal Quantity { get; set; }

        public string unitMsr { get; set; }

        public decimal VatPrcnt { get; set; }

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
        public int U_ARS { get; set; }

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
        /// 税好
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
        /// 单据主键
        /// </summary>
        public int DocEntry1 { get; set; }

        public int LineNum { get; set; }

        public string ItemCode { get; set; }

        public decimal Quantity { get; set; }

        public string unitMsr { get; set; }

        public decimal VatPrcnt { get; set; }

        public decimal GTotalSC { get; set; }
    }

}