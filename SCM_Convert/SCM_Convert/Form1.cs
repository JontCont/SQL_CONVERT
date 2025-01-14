﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using LinqToExcel;
using ChangesCrack;

namespace SCM_Convert 
{
    public partial class Form1 : Form
    {
        Comm comm = new Comm();
        InfoResponse response = new InfoResponse();
        string sPath = Application.StartupPath;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            btn_Comm_Click("", e);
            System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }

        private void btn_Comm_Click(object sender, EventArgs e)
        {
            Changes sChk = new Changes();
            string sPath = Application.StartupPath;
            int index = 0;
            try
            {
                StreamReader sRead = new StreamReader(sPath + "\\Logs.ini");
                string sShow;
                while ((sShow = sRead.ReadLine()) != null)
                {
                    switch (index)
                    {
                        case 0: comm.GET_DBComm = sChk.Get_Crack(true, sShow); break;
                        case 1: comm.SET_DBComm = sChk.Get_Crack(true, sShow); break;
                    }
                    index++;
                }
                lMessage.Text = "連線成功";
                if (!comm.commDB(comm.GET_DBComm) || !comm.commDB(comm.SET_DBComm)) lMessage.Text = "ERROR-001 : 連線失敗";
                sRead.Close();
            }
            catch (Exception) { lMessage.Text = "ERROR-002 : Logs.ini遺失，無法連線"; }
        }

        /// <summary>
        /// 採購單頭檔按鈕
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_SigleHead_Click(object sender, EventArgs e)
        {
            //執行excel中的語法。
            //ExcelQueryByUpdateData("SUT01_0000");
            ExcelQueryByUpdateData(Tag.ToString());//可使用Tag方式執行。
        }

        /// <summary>
        /// 採購單身檔按鈕
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_Details_Click(object sender, EventArgs e)
        {
            //執行excel中的語法。
            ExcelQueryByUpdateData("SUT01_0100");
        }
        /// <summary>
        /// 供應商按鈕
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_Sup_Click(object sender, EventArgs e)
        {
            //執行excel中的語法。
            ExcelQueryByUpdateData("SUB02_0000");
        }

        /// <summary>
        /// 料件檔按鈕
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_ProData_Click(object sender, EventArgs e)
        {
            //執行excel中的語法。
            ExcelQueryByUpdateData("SUB01_0000");
        }

        /// <summary>
        /// 開啟設定檔
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_setting_Click(object sender, EventArgs e)
        {
            //打開設定檔案
            Process.Start(sPath + "\\setting.xlsx");
        }

        /// <summary>
        /// 指定excel table 名稱，
        /// 執行EXCEL 裡面的SQL 語法。
        /// </summary>
        /// <param name="sTable"></param>
        private void ExcelQueryByUpdateData(string sTable)
        {
            //設定EXCEL
            var exc = new ExcelQueryFactory(sPath + "\\setting.xlsx");

            var sQuery = from x in exc.Worksheet<Setting>("setting")
                         where x.Table == sTable
                         select x;
            foreach (var name in sQuery)
            {
                if (name.InitialCtr == null || name.changeCtr == null) { lMessage.Text = "設定檔內容有缺無法更新"; break; }
                comm.Del_DBTable(name.InitialCtr);
                comm.Insert_SaveDB(name.Table, name.changeCtr);
            }
            lMessage.Text = "料件檔 - 更新成功\n-完成豪秒數 : " + comm.GET_Run_Timer + "\n-共" + comm.GET_Rows_Coumt + "筆";
        }
    }
}
