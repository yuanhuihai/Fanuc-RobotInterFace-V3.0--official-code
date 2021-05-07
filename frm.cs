using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
using System.Linq;
using System.Xml.Linq;

namespace frrjiftest
{
    public partial class frm : System.Windows.Forms.Form
    {

        public string HostName;

        private const string cnstApp = "frrjiftest";
        private const string cnstSection = "setting";

        private Random rnd = new Random();

        private FRRJIf.Core mobjCore;
        private FRRJIf.DataTable mobjDataTable;
        private FRRJIf.DataTable mobjDataTable2;
        private FRRJIf.DataCurPos mobjCurPos;
        private FRRJIf.DataCurPos mobjCurPosUF;
        private FRRJIf.DataCurPos mobjCurPos2;
        private FRRJIf.DataTask mobjTask;
        private FRRJIf.DataTask mobjTaskIgnoreMacro;
        private FRRJIf.DataTask mobjTaskIgnoreKarel;
        private FRRJIf.DataTask mobjTaskIgnoreMacroKarel;
        private FRRJIf.DataPosReg mobjPosReg;
        private FRRJIf.DataPosReg mobjPosReg2;
        private FRRJIf.DataPosRegXyzwpr mobjPosRegXyzwpr;
        private FRRJIf.DataPosRegMG mobjPosRegMG;
        private FRRJIf.DataSysVar mobjSysVarInt;
        private FRRJIf.DataSysVar mobjSysVarInt2;
        private FRRJIf.DataSysVar mobjSysVarReal;
        private FRRJIf.DataSysVar mobjSysVarReal2;
        private FRRJIf.DataSysVar mobjSysVarString;
        private FRRJIf.DataSysVarPos mobjSysVarPos;
        private FRRJIf.DataSysVar[] mobjSysVarIntArray;
        private FRRJIf.DataNumReg mobjNumReg;
        private FRRJIf.DataNumReg mobjNumReg2;
        private FRRJIf.DataNumReg mobjNumReg3;
        private FRRJIf.DataAlarm mobjAlarm;
        private FRRJIf.DataAlarm mobjAlarmCurrent;
        private FRRJIf.DataSysVar mobjVarString;
        private FRRJIf.DataString mobjStrReg;
        private FRRJIf.DataString mobjStrRegComment;

        public frm()
        {
            InitializeComponent();
        }
        
        public long gflngGetTickCountEx()
        {
            // return tick count[ms]
            return System.DateTime.Now.Ticks / System.TimeSpan.TicksPerMillisecond;
        }


        private void cmdClearAlarm_Click(System.Object eventSender, System.EventArgs eventArgs)
        {
            if (mobjCore == null)
                return;

            mobjCore.ClearAlarm(0);
            cmdRefresh.PerformClick();
        }

        private void cmdConnect_Click(System.Object eventSender, System.EventArgs eventArgs)
        {
            if (mobjCore == null)
            {
                //connect
                msubInit();
            }
            else
            {
                //disconnect
                mobjCore.Disconnect();
                msubDisconnected2();
            }
        }

        private void cmdRefresh_Click(System.Object eventSender, System.EventArgs eventArgs)
        {
            int ii = 0;
            string strTmp = null;
            double dblTime = 0;
            object vntValue = null;
            Array xyzwpr = new float[9];
            Array config = new short[7];
            Array joint = new float[9];
            short intUF = 0;
            short intUT = 0;
            short intValidC = 0;
            short intValidJ = 0;
            string strProg = "";
            short intLine = 0;
            short intState = 0;
            string strParentProg = "";
            Array intSDO = new short[100];
            Array intSDO2 = new short[100];
            Array intSDO3 = new short[100];
            Array intSDI = new short[10];
            Array intRDO = new short[10];
            Array intRDI = new short[10];
            Array intSO = new short[10];
            Array intSI = new short[10];
            Array intUO = new short[10];
            Array intUI = new short[10];
            Array lngAO = new int[3];
            Array lngAI = new int[3];
            Array lngGO = new int[3];
            Array lngGO2 = new int[3];
            Array lngGI = new int[3];
            Array intWO = new short[5];
            Array intWI = new short[5];
            Array intWSI = new short[5];
            bool blnDT = false;
            bool blnSDO = false;
            bool blnSDO2 = false;
            bool blnSDO3 = false;
            bool blnSDI = false;
            bool blnRDO = false;
            bool blnRDI = false;
            bool blnSO = false;
            bool blnSI = false;
            bool blnUO = false;
            bool blnUI = false;
            bool blnGO = false;
            bool blnGO2 = false;
            bool blnGI = false;
            string strValue = "";



            //check
            if (mobjCore == null)
            {
                return;
            }

            cmdRefresh.Enabled = false;
            System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.WaitCursor;

            dblTime = gflngGetTickCountEx();

            //Refresh data table
            blnDT = mobjDataTable.Refresh();
            if (blnDT == false)
            {
                System.Windows.Forms.Cursor.Current = Cursors.Default;
                msubDisconnected();
                return;
            }

            //read SDO
            blnSDO = mobjCore.ReadSDO(1, ref intSDO, 100);
            if (blnSDO == false)
            {
                System.Windows.Forms.Cursor.Current = Cursors.Default;
                msubDisconnected();
                return;
            }
            blnSDO2 = mobjCore.ReadSDO(10001, ref intSDO2, 100);
            if (blnSDO2 == false)
            {
                System.Windows.Forms.Cursor.Current = Cursors.Default;
                msubDisconnected();
                return;
            }
            blnSDO3 = mobjCore.ReadSDO(11001, ref intSDO3, 100);
            if (blnSDO3 == false)
            {
                System.Windows.Forms.Cursor.Current = Cursors.Default;
                msubDisconnected();
                return;
            }

            //read SDI
            blnSDI = mobjCore.ReadSDI(1, ref intSDI, 10);
            if (blnSDI == false)
            {
                System.Windows.Forms.Cursor.Current = Cursors.Default;
                msubDisconnected();
                return;
            }

            //read RDO
            blnRDO = mobjCore.ReadRDO(1, ref intRDO, 8);
            if (blnRDO == false)
            {
                System.Windows.Forms.Cursor.Current = Cursors.Default;
                msubDisconnected();
                return;
            }

            //read RDI
            blnRDI = mobjCore.ReadRDI(1, ref intRDI, 8);
            if (blnRDI == false)
            {
                System.Windows.Forms.Cursor.Current = Cursors.Default;
                msubDisconnected();
                return;
            }

            //read SO
            blnSO = mobjCore.ReadSO(0, ref intSO, 9);
            if (blnSO == false)
            {
                System.Windows.Forms.Cursor.Current = Cursors.Default;
                msubDisconnected();
                return;
            }

            //read SI
            blnSI = mobjCore.ReadSI(0, ref intSI, 9);
            if (blnSI == false)
            {
                System.Windows.Forms.Cursor.Current = Cursors.Default;
                msubDisconnected();
                return;
            }

            //read UO
            blnUO = mobjCore.ReadUO(1, ref intUO, 10);
            if (blnUO == false)
            {
                System.Windows.Forms.Cursor.Current = Cursors.Default;
                msubDisconnected();
                return;
            }

            //read UI
            blnUI = mobjCore.ReadUI(1, ref intUI, 10);
            if (blnUI == false)
            {
                System.Windows.Forms.Cursor.Current = Cursors.Default;
                msubDisconnected();
                return;
            }

            //read GO
            blnGO = mobjCore.ReadGO(1, ref lngGO, 3);
            if (blnGO == false)
            {
                System.Windows.Forms.Cursor.Current = Cursors.Default;
                msubDisconnected();
                return;
            }
            blnGO2 = mobjCore.ReadGO(10001, ref lngGO2, 3);
            if (blnGO2 == false)
            {
                System.Windows.Forms.Cursor.Current = Cursors.Default;
                msubDisconnected();
                return;
            }

            //read GI
            blnGI = mobjCore.ReadGI(1, ref lngGI, 3);
            if (blnGI == false)
            {
                System.Windows.Forms.Cursor.Current = Cursors.Default;
                msubDisconnected();
                return;
            }

            //read AO. Offset 1000 for AO
            if (mobjCore.ReadGO(1000 + 1, ref lngAO, 3) == false)
            {
                System.Windows.Forms.Cursor.Current = Cursors.Default;
                msubDisconnected();
                return;
            }

            //read AI. Offset 1000 for AO
            if (mobjCore.ReadGI(1000 + 1, ref lngAI, 2) == false)
            {
                System.Windows.Forms.Cursor.Current = Cursors.Default;
                msubDisconnected();
                return;
            }

            //read WO. Offset 8000 for WO
            if (mobjCore.ReadSDO(8001, ref intWO, 5) == false)
            {
                System.Windows.Forms.Cursor.Current = Cursors.Default;
                msubDisconnected();
                return;
            }

            //read WI. Offset 8000 for WI
            if (mobjCore.ReadSDI(8001, ref intWI, 5) == false)
            {
                System.Windows.Forms.Cursor.Current = Cursors.Default;
                msubDisconnected();
                return;
            }

            //read WSI. Offset 8400 for WI
            if (mobjCore.ReadSDI(8401, ref intWSI, 1) == false)
            {
                System.Windows.Forms.Cursor.Current = Cursors.Default;
                msubDisconnected();
                return;
            }

            dblTime = gflngGetTickCountEx() - dblTime;
            strTmp = "Time = " + Convert.ToInt16(dblTime) + "(msec)\r\n";

            {
                if (mobjCurPos.GetValue(ref xyzwpr, ref config, ref joint, ref intUF, ref intUT, ref intValidC, ref intValidJ))
                {
                    strTmp = strTmp + "--- CurPos GP1 World ---\r\n";
                    strTmp = strTmp + mstrPos(ref xyzwpr, ref config, ref joint, intValidC, intValidJ, intUF, intUT);
                }
                else
                {
                    strTmp = strTmp + "CurPos Error!!!\r\n";
                }
            }
            {
                if (mobjCurPosUF.GetValue(ref xyzwpr, ref config, ref joint, ref intUF, ref intUT, ref intValidC, ref intValidJ))
                {
                    strTmp = strTmp + "--- CurPos GP1 Current UF ---\r\n";
                    strTmp = strTmp + mstrPos(ref xyzwpr, ref config, ref joint, intValidC, intValidJ, intUF, intUT);
                }
                else
                {
                    strTmp = strTmp + "CurPos Error!!!\r\n";
                }
            }
            {
                if (mobjCurPos2.GetValue(ref xyzwpr, ref config, ref joint, ref intUF, ref intUT, ref intValidC, ref intValidJ))
                {
                    strTmp = strTmp + "--- CurPos GP2 World ---\r\n";
                    strTmp = strTmp + mstrPos(ref xyzwpr, ref config, ref joint, intValidC, intValidJ, intUF, intUT);
                }
                else
                {
                    strTmp = strTmp + "CurPos2 Error!!!\r\n";
                }
            }
            {
                if (mobjTask.GetValue(ref strProg, ref intLine, ref intState, ref strParentProg))
                {
                    strTmp = strTmp + "--- Task ---\r\n";
                    strTmp = strTmp + mstrTask(mobjTask.Index, strProg, intLine, intState, strParentProg);
                }
                else
                {
                    strTmp = strTmp + "Task Error!!!\r\n";
                }
                if (mobjTaskIgnoreMacro.GetValue(ref strProg, ref intLine, ref intState, ref strParentProg))
                {
                    strTmp = strTmp + "--- Task Ignore Macro ---\r\n";
                    strTmp = strTmp + mstrTask(mobjTaskIgnoreMacro.Index, strProg, intLine, intState, strParentProg);
                }
                else
                {
                    strTmp = strTmp + "Task Error!!!\r\n";
                }
                if (mobjTaskIgnoreKarel.GetValue(ref strProg, ref intLine, ref intState, ref strParentProg))
                {
                    strTmp = strTmp + "--- Task Ignore KAREL ---\r\n";
                    strTmp = strTmp + mstrTask(mobjTaskIgnoreKarel.Index, strProg, intLine, intState, strParentProg);
                }
                else
                {
                    strTmp = strTmp + "Task Error!!!\r\n";
                }
                if (mobjTaskIgnoreMacroKarel.GetValue(ref strProg, ref intLine, ref intState, ref strParentProg))
                {
                    strTmp = strTmp + "--- Task Ignore Macro, KAREL ---\r\n";
                    strTmp = strTmp + mstrTask(mobjTaskIgnoreMacroKarel.Index, strProg, intLine, intState, strParentProg);
                }
                else
                {
                    strTmp = strTmp + "Task Error!!!\r\n";
                }
            }
            strTmp = strTmp + "--- SysVar ---\r\n";
            {
                if (mobjSysVarInt.GetValue(ref vntValue) == true)
                {
                    strTmp = strTmp + mobjSysVarInt.SysVarName + " = " + vntValue + "\r\n";
                }
                else
                {
                    strTmp = strTmp + mobjSysVarInt.SysVarName + " : Error!!! \r\n";
                }
            }
            {
                if (mobjSysVarInt2.GetValue(ref vntValue) == true)
                {
                    strTmp = strTmp + mobjSysVarInt2.SysVarName + " = " + vntValue + "\r\n";
                }
                else
                {
                    strTmp = strTmp + mobjSysVarInt2.SysVarName + " : Error!!! \r\n";
                }
            }
            {
                if (mobjSysVarReal.GetValue(ref vntValue) == true)
                {
                    strTmp = strTmp + mobjSysVarReal.SysVarName + " = " + vntValue + "\r\n";
                }
                else
                {
                    strTmp = strTmp + mobjSysVarReal.SysVarName + " : Error!!! \r\n";
                }
            }
            {
                if (mobjSysVarReal2.GetValue(ref vntValue) == true)
                {
                    strTmp = strTmp + mobjSysVarReal2.SysVarName + " = " + vntValue + "\r\n";
                }
                else
                {
                    strTmp = strTmp + mobjSysVarReal2.SysVarName + " : Error!!! \r\n";
                }
            }
            {
                if (mobjSysVarString.GetValue(ref vntValue) == true)
                {
                    strTmp = strTmp + mobjSysVarString.SysVarName + " = " + vntValue + "\r\n";
                }
                else
                {
                    strTmp = strTmp + mobjSysVarString.SysVarName + " : Error!!! \r\n";
                }
            }
            {
                if (mobjSysVarPos.GetValue(ref xyzwpr, ref config, ref joint, ref intUF, ref intUT, ref intValidC, ref intValidJ))
                {
                    strTmp = strTmp + mobjSysVarPos.SysVarName + "\r\n";
                    strTmp = strTmp + mstrPos(ref xyzwpr, ref config, ref joint, intValidC, intValidJ, intUF, intUT);
                }
                else
                {
                    strTmp = strTmp + mobjSysVarPos.SysVarName + " : Error!!! \r\n";
                }
            }
            {
                if (mobjVarString.GetValue(ref vntValue))
                {
                    strTmp = strTmp + mobjVarString.SysVarName + " = " + vntValue + "\r\n";
                }
                else
                {
                    strTmp = strTmp + mobjVarString.SysVarName + " : Error!!! " + "\r\n";
                }
            }
            strTmp = strTmp + "--- NumReg ---\r\n";
            {
                for (ii = mobjNumReg.StartIndex; ii <= mobjNumReg.EndIndex; ii++)
                {
                    if (mobjNumReg.GetValue(ii, ref vntValue) == true)
                    {
                        strTmp = strTmp + "R[" + ii + "] = " + vntValue + "\r\n";
                    }
                    else
                    {
                        strTmp = strTmp + "R[" + ii + "] : Error!!! \r\n";
                    }
                }
            }
            {
                for (ii = mobjNumReg2.StartIndex; ii <= mobjNumReg2.EndIndex; ii++)
                {
                    if (mobjNumReg2.GetValue(ii, ref vntValue) == true)
                    {
                        strTmp = strTmp + "R[" + ii + "] = " + vntValue + "\r\n";
                    }
                    else
                    {
                        strTmp = strTmp + "R[" + ii + "] : Error!!! \r\n";
                    }
                }
            }
            strTmp = strTmp + "--- PosReg GP1 ---\r\n";
            {
                for (ii = mobjPosReg.StartIndex; ii <= mobjPosReg.EndIndex; ii++)
                {
                    if (mobjPosReg.GetValue(ii, ref xyzwpr, ref config, ref joint, ref intUF, ref intUT, ref intValidC, ref intValidJ))
                    {
                        strTmp = strTmp + "PR[" + ii + "]\r\n";
                        strTmp = strTmp + mstrPos(ref xyzwpr, ref config, ref joint, intValidC, intValidJ, intUF, intUT);
                    }
                    else
                    {
                        strTmp = strTmp + "PR[" + ii + "] : Error!!! \r\n";
                    }
                }
            }
            strTmp = strTmp + "--- PosReg GP2 ---\r\n";
            {
                for (ii = mobjPosReg2.StartIndex; ii <= mobjPosReg2.EndIndex; ii++)
                {
                    if (mobjPosReg2.GetValue(ii, ref xyzwpr, ref config, ref joint, ref intUF, ref intUT, ref intValidC, ref intValidJ))
                    {
                        strTmp = strTmp + "PR[" + ii + "]\r\n";
                        strTmp = strTmp + mstrPos(ref xyzwpr, ref config, ref joint, intValidC, intValidJ, intUF, intUT);
                    }
                    else
                    {
                        strTmp = strTmp + "PR[GP2:" + ii + "] : Error!!! \r\n";
                    }
                }
            }
            strTmp = strTmp + "--- SDO ---\r\n";
            if (blnSDO == true)
            {
                strTmp = strTmp + mstrIO("SDO", 1, 100, ref intSDO) + "\r\n";
            }
            else
            {
                strTmp = strTmp + "Error\r\n";
            }
            strTmp = strTmp + "--- SDO[1000x] ---\r\n";
            if (blnSDO2 == true)
            {
                strTmp = strTmp + mstrIO("SDO", 10001, 10100, ref intSDO2) + "\r\n";
            }
            else
            {
                strTmp = strTmp + "Error\r\n";
            }
            strTmp = strTmp + "--- SDO[1100x] ---\r\n";
            if (blnSDO3 == true)
            {
                strTmp = strTmp + mstrIO("SDO", 11001, 11100, ref intSDO3) + "\r\n";
            }
            else
            {
                strTmp = strTmp + "Error\r\n";
            }
            strTmp = strTmp + "--- SDI ---\r\n";
            if (blnSDI == true)
            {
                strTmp = strTmp + mstrIO("SDI", 1, 10, ref intSDI) + "\r\n";
            }
            else
            {
                strTmp = strTmp + "Error\r\n";
            }
            strTmp = strTmp + "--- RDO ---\r\n";
            if (blnRDO == true)
            {
                strTmp = strTmp + mstrIO("RDO", 1, 8, ref intRDO) + "\r\n";
            }
            else
            {
                strTmp = strTmp + "Error\r\n";
            }
            strTmp = strTmp + "--- RDI ---\r\n";
            if (blnRDI == true)
            {
                strTmp = strTmp + mstrIO("RDI", 1, 8, ref intRDI) + "\r\n";
            }
            else
            {
                strTmp = strTmp + "Error\r\n";
            }
            strTmp = strTmp + "--- SO ---\r\n";
            if (blnSO == true)
            {
                strTmp = strTmp + mstrIO("SO", 0, 9, ref intSO) + "\r\n";
            }
            else
            {
                strTmp = strTmp + "Error\r\n";
            }
            strTmp = strTmp + "--- SI ---\r\n";
            if (blnSI == true)
            {
                strTmp = strTmp + mstrIO("SI", 0, 9, ref intSI) + "\r\n";
            }
            else
            {
                strTmp = strTmp + "Error\r\n";
            }
            strTmp = strTmp + "--- UO ---\r\n";
            if (blnUO == true)
            {
                strTmp = strTmp + mstrIO("UO", 1, 10, ref intUO) + "\r\n";
            }
            else
            {
                strTmp = strTmp + "Error\r\n";
            }
            strTmp = strTmp + "--- UI ---\r\n";
            if (blnUI == true)
            {
                strTmp = strTmp + mstrIO("UI", 1, 10, ref intUI) + "\r\n";
            }
            else
            {
                strTmp = strTmp + "Error\r\n";
            }
            strTmp = strTmp + "--- GO ---\r\n";
            if (blnGO == true)
            {
                strTmp = strTmp + mstrIO2("GO", 1, 3, ref lngGO) + "\r\n";
            }
            else
            {
                strTmp = strTmp + "Error\r\n";
            }
            strTmp = strTmp + "--- GO[1000x] ---\r\n";
            if (blnGO == true)
            {
                strTmp = strTmp + mstrIO2("GO", 10001, 10003, ref lngGO2) + "\r\n";
            }
            else
            {
                strTmp = strTmp + "Error\r\n";
            }
            strTmp = strTmp + "--- GI ---\r\n";
            if (blnGI == true)
            {
                strTmp = strTmp + mstrIO2("GI", 1, 3, ref lngGI) + "\r\n";
            }
            else
            {
                strTmp = strTmp + "Error\r\n";
            }
            strTmp = strTmp + "--- AO ---" + "\r\n";
            strTmp = strTmp + mstrIO2("AO", 1, 3, ref lngAO) + "\r\n";
            strTmp = strTmp + "--- AI ---" + "\r\n";
            strTmp = strTmp + mstrIO2("AI", 1, 3, ref lngAI)  + "\r\n";
            strTmp = strTmp + "--- WO ---" + "\r\n";
            if (blnSDO == true)
            {
                strTmp = strTmp + mstrIO("WO", 1, 5, ref intWO)  + "\r\n";
            }
            else
            {
                strTmp = strTmp + "Error"  + "\r\n";
            }
            strTmp = strTmp + "--- WI ---"  + "\r\n";
            if (blnSDI == true)
            {
                strTmp = strTmp + mstrIO("WI", 1, 5, ref intWI) + "\r\n";
            }
            else
            {
                strTmp = strTmp + "Error"  + "\r\n";
            }
            strTmp = strTmp + "--- WSI ---"  + "\r\n";
            if (blnSDI == true)
            {
                strTmp = strTmp + mstrIO("WSI", 1, 1, ref intWSI) + "\r\n";
            }
            else
            {
                strTmp = strTmp + "Error"  + "\r\n";
            }

            for (ii = 1; ii <= 5; ii++)
            {
                strTmp = strTmp + mstrAlarm(ref mobjAlarm, ii);
            }

            for (ii = 1; ii <= 1; ii++)
            {
                strTmp = strTmp + mstrAlarm(ref mobjAlarmCurrent, ii);
            }

            string strComment = "";
            strTmp = strTmp + "--- StrReg ---" + "\r\n";
            for (ii = mobjStrReg.StartIndex; ii <= mobjStrReg.EndIndex; ii++)
            {
                mobjStrRegComment.GetValue(ii, ref strComment);
                if (mobjStrReg.GetValue(ii, ref strValue) == true)
                {
                    strTmp = strTmp + String.Format("SR[{0}:{1}] = {2}", ii, strComment, strValue) + "\r\n";
                }
				else
                {
                    strTmp = strTmp + String.Format("SR[{0}]  : Error!!! ", ii) + "\r\n";
                }
			}

            txtResult.Text = strTmp;

            cmdRefresh.Enabled = true;
            System.Windows.Forms.Cursor.Current = Cursors.Default;
        }
        int static_cmdSetGO_Click_lngCount;

        private void cmdSetGO_Click(System.Object eventSender, System.EventArgs eventArgs)
        {
            Array lngVal = new int[3];
            int ii = 0;
            bool blnRes = false;
            short intStartIndex = 0;

            if (object.ReferenceEquals(eventSender, _cmdSetGO_1))
            {
                intStartIndex = 10001;
            }
            else
            {
                intStartIndex = 1;
            }

            static_cmdSetGO_Click_lngCount = static_cmdSetGO_Click_lngCount + 1;
            for (ii = 0; ii <= 2; ii++)
            {
                lngVal.SetValue((int)(static_cmdSetGO_Click_lngCount * (ii + 1)), ii);
            }

            blnRes = mobjCore.WriteGO(intStartIndex, ref lngVal, 3);
            if (blnRes == false)
            {
                MessageBox.Show("Error");
            }
            cmdRefresh.PerformClick();

        }

        private void cmdSetNumReg_Click(System.Object eventSender, System.EventArgs eventArgs)
        {
            int intRand = 0;
            int ii = 0;
            int[] intValues = new int[101];
            float[] sngValues = new float[101];

            intRand = rnd.Next(1, 10);
            {
                for (ii = 0; ii <= mobjNumReg.EndIndex - mobjNumReg.StartIndex; ii++)
                {
                    intValues[ii] = (ii + 1) * intRand;
                }
                if (mobjNumReg.SetValues(mobjNumReg.StartIndex, intValues, mobjNumReg.EndIndex - mobjNumReg.StartIndex + 1) == false)
                {
                    MessageBox.Show("SetNumReg Int Error");
                }
            }
            {
                for (ii = 0; ii <= mobjNumReg2.EndIndex - mobjNumReg2.StartIndex; ii++)
                {
                    sngValues[ii] = (float)((ii + 1) * intRand * 1.1);
                }
                if (mobjNumReg2.SetValues(mobjNumReg2.StartIndex, sngValues, mobjNumReg2.EndIndex - mobjNumReg2.StartIndex + 1) == false)
                {
                    MessageBox.Show("SetNumReg Real Error");
                }
            }
            cmdRefresh.PerformClick();
        }

        private void cmdSetPosReg_Click(System.Object eventSender, System.EventArgs eventArgs)
        {
            int intRand = 0;
            int ii = 0;
            int jj = 0;
            Array sngJoint = new float[6];

            intRand = rnd.Next(1, 10);
            {
                for (ii = mobjPosReg.StartIndex; ii <= mobjPosReg.EndIndex; ii++)
                {
                    for (jj = sngJoint.GetLowerBound(0); jj <= sngJoint.GetUpperBound(0); jj++)
                    {
                        sngJoint.SetValue((float)( 11.11 * (jj + 1) * intRand * ii), jj);
                    }
                    mobjPosReg.SetValueJoint(ii, ref sngJoint, 15, 15);
                }
            }
            {
                for (ii = mobjPosReg2.StartIndex; ii <= mobjPosReg2.EndIndex; ii++)
                {
                    jj = 0;
                    sngJoint.SetValue((float)(11.11 * (jj + 1) * intRand * ii), jj);
                    mobjPosReg2.SetValueJoint(ii, ref sngJoint, 15, 15);
                }
            }
            cmdRefresh.PerformClick();
        }

        private void cmdSetPosRegX_Click(System.Object eventSender, System.EventArgs eventArgs)
        {
            int intRand = 0;
            int ii = 0;
            int jj = 0;
            Array sngArray = new float[9];
            Array intConfig = new short[7];
            long lngTime = 0;

            lngTime = gflngGetTickCountEx();
            intRand = rnd.Next(1, 10);
            {
                for (ii = mobjPosReg.StartIndex; ii <= mobjPosReg.EndIndex; ii++)
                {
                    for (jj = sngArray.GetLowerBound(0); jj <= sngArray.GetUpperBound(0); jj++)
                    {
                        sngArray.SetValue((float)(11.11 * (jj + 1) * intRand * ii), jj);
                    }
                    intConfig.SetValue((short)ii, 4);
                    intConfig.SetValue((short)ii, 5);
                    intConfig.SetValue((short)ii, 6);
                    mobjPosReg.SetValueXyzwpr(ii, ref sngArray, ref intConfig, -1, -1);
                }
            }
            Debug.Print(string.Format( "Time {0} ms" ,gflngGetTickCountEx() - lngTime));
            cmdRefresh.PerformClick();

        }

        private void cmdSetPosRegX2_Click(System.Object eventSender, System.EventArgs eventArgs)
        {
            int intRand = 0;
            int ii = 0;
            int jj = 0;
            Array sngArray = new float[6];
            Array intConfig = new short[7];
            bool blnRes = false;
            long lngTime = 0;

            lngTime = gflngGetTickCountEx();
            intRand = rnd.Next(1, 10);
            {
                for (ii = mobjPosRegXyzwpr.StartIndex; ii <= mobjPosRegXyzwpr.EndIndex; ii++)
                {
                    for (jj = sngArray.GetLowerBound(0); jj <= sngArray.GetUpperBound(0); jj++)
                    {
                        sngArray.SetValue((float)(11.11 * (jj + 1) * intRand * ii), jj);
                    }
                    intConfig.SetValue((short)ii, 4);
                    intConfig.SetValue((short)ii, 5);
                    intConfig.SetValue((short)ii, 6);
                    blnRes = mobjPosRegXyzwpr.SetValueXyzwpr(ii, ref sngArray, ref intConfig);
                    if (blnRes == false)
                    {
                        MessageBox.Show("Error mobjPosRegXyzwpr.SetValueXyzwpr");
                    }
                }
                blnRes = mobjPosRegXyzwpr.Update();
                if (blnRes == false)
                {
                    MessageBox.Show("Error mobjPosRegXyzwpr.Update");
                }
            }
            Debug.Print(string.Format("Time {0} ms", gflngGetTickCountEx() - lngTime));
            cmdRefresh.PerformClick();


        }
        int static_cmdSetRDI_Click_lngCount;

        private void cmdSetRDI_Click(System.Object eventSender, System.EventArgs eventArgs)
        {
            short[] intVal = new short[10];
            int ii = 0;
            bool blnRes = false;

            static_cmdSetRDI_Click_lngCount = static_cmdSetRDI_Click_lngCount + 1;
            if (static_cmdSetRDI_Click_lngCount % 2 == 1)
            {
                for (ii = 0; ii <= 7; ii++)
                {
                    intVal[ii] = 1;
                }
            }
            Array intValTmp = intVal;
            blnRes = mobjCore.WriteRDI(1, ref intValTmp, 8);
            if (blnRes == false)
            {
                MessageBox.Show("Error");
            }
            cmdRefresh.PerformClick();

        }
        int static_cmdSetRDO_Click_lngCount;

        private void cmdSetRDO_Click(System.Object eventSender, System.EventArgs eventArgs)
        {
            Array intVal = new short[10];
            int ii = 0;
            bool blnRes = false;

            static_cmdSetRDO_Click_lngCount = static_cmdSetRDO_Click_lngCount + 1;
            if (static_cmdSetRDO_Click_lngCount % 2 == 1)
            {
                for (ii = 0; ii <= 7; ii++)
                {
                    intVal.SetValue((short)1, ii);
                }
            }
            blnRes = mobjCore.WriteRDO(1, ref intVal, 8);
            if (blnRes == false)
            {
                MessageBox.Show("Error");
            }
            cmdRefresh.PerformClick();

        }
        int static_cmdSetSDI_Click_lngCount;

        private void cmdSetSDI_Click(System.Object eventSender, System.EventArgs eventArgs)
        {
            Array intVal = new short[10];
            int ii = 0;
            bool blnRes = false;

            static_cmdSetSDI_Click_lngCount = static_cmdSetSDI_Click_lngCount + 1;
            if (static_cmdSetSDI_Click_lngCount % 2 == 1)
            {
                for (ii = 0; ii <= 9; ii++)
                {
                    intVal.SetValue((short)1, ii);
                }
            }
            blnRes = mobjCore.WriteSDI(1, ref intVal, 10);
            if (blnRes == false)
            {
                MessageBox.Show("Error");
            }
            cmdRefresh.PerformClick();

        }
        int static_cmdSetSDO_Click_lngCount;

        private void cmdSetSDO_Click(System.Object eventSender, System.EventArgs eventArgs)
        {
            Array intVal = new short[100];
            int ii = 0;
            bool blnRes = false;
            short intStartIndex = 0;

            if (object.ReferenceEquals(eventSender, _cmdSetSDO_1))
            {
                intStartIndex = 10001;
            }
            else if (object.ReferenceEquals(eventSender, _cmdSetSDO_2))
            {
                intStartIndex = 11001;
            }
            else
            {
                intStartIndex = 1;
            }

            static_cmdSetSDO_Click_lngCount = static_cmdSetSDO_Click_lngCount + 1;
            if (static_cmdSetSDO_Click_lngCount % 2 == 1)
            {
                for (ii = 0; ii <= 99; ii++)
                {
                    intVal.SetValue((short)1, ii);
                }
            }
            blnRes = mobjCore.WriteSDO(intStartIndex, ref intVal, 100);
            if (blnRes == false)
            {
                MessageBox.Show("Error");
            }
            cmdRefresh.PerformClick();
        }
        int static_cmdsetgi_Click_lngCount;

        private void cmdsetgi_Click(System.Object eventSender, System.EventArgs eventArgs)
        {
            Array lngVal = new int[3];
            int ii = 0;
            bool blnRes = false;

            static_cmdsetgi_Click_lngCount = static_cmdsetgi_Click_lngCount + 1;
            for (ii = 0; ii <= 2; ii++)
            {
                lngVal.SetValue((int)(static_cmdsetgi_Click_lngCount * (ii + 1)), ii);
            }

            blnRes = mobjCore.WriteGI(1, ref lngVal, 3);
            if (blnRes == false)
            {
                MessageBox.Show("Error");
            }
            cmdRefresh.PerformClick();

        }

        private void cmdWriteSysVar_Click(System.Object eventSender, System.EventArgs eventArgs)
        {
            int lngOld = 0;
            float sngOld = 0;
            string strOld = "";
            float sngXOld = 0;
            int lngNew = 0;
            float sngNew = 0;
            string strNew = null;
            float sngXNew = 0;
            int lngConf = 0;
            float sngConf = 0;
            string strConf = "";
            Array xyzwpr = new float[9];
            Array config = new short[7];
            Array joint = new float[9];
            short intUF = 0;
            short intUT = 0;
            short intValidC = 0;
            short intValidJ = 0;
            object objTmp = null;

            try
            {
                if (MessageBox.Show("Are you sure to test writing system variables?", "frrjiftest", MessageBoxButtons.OKCancel) == DialogResult.Cancel)
                {
                    return;
                }

                System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.WaitCursor;

                //store old values
                mobjDataTable.Refresh();
                mobjSysVarInt2.GetValue(ref objTmp);
                lngOld = (int)objTmp;
                mobjSysVarString.GetValue(ref objTmp);
                strOld = (string)objTmp;
                mobjSysVarReal2.GetValue(ref objTmp);
                sngOld = (float)objTmp;
                mobjSysVarPos.GetValue(ref xyzwpr, ref config, ref joint, ref intUF, ref intUT, ref intValidC, ref intValidJ);
                sngXOld = (float)xyzwpr.GetValue(0);

                //make new values
                lngNew = 999;
                sngNew = sngOld + 1;
                strNew = "abc";
                sngXNew = sngXOld + 1;
                xyzwpr.SetValue(sngXNew, 0);

                //write dummy values
                mobjSysVarInt2.SetValue(lngNew);
                mobjSysVarString.SetValue(strNew);
                mobjSysVarReal2.SetValue(sngNew);
                mobjSysVarPos.SetValueXyzwpr(ref xyzwpr, ref config, intUF, intUT);


                //confirm
                mobjDataTable.Refresh();
                mobjSysVarInt2.GetValue(ref objTmp);
                lngConf = (int)objTmp;
                System.Diagnostics.Debug.Assert(lngNew == lngConf, "");
                mobjSysVarString.GetValue(ref objTmp);
                strConf = (string)objTmp;
                System.Diagnostics.Debug.Assert(strNew == strConf, "");
                mobjSysVarReal2.GetValue(ref objTmp);
                sngConf = (float)objTmp;
                System.Diagnostics.Debug.Assert(sngNew == sngConf, "");
                mobjSysVarPos.GetValue(ref xyzwpr, ref config, ref joint, ref intUF, ref intUT, ref intValidC, ref intValidJ);
                System.Diagnostics.Debug.Assert(sngXNew == (float)xyzwpr.GetValue(0), "");

                //restore old values
                mobjSysVarInt2.SetValue(lngOld);
                mobjSysVarString.SetValue(strOld);
                mobjSysVarReal2.SetValue(sngOld);
                xyzwpr.SetValue(sngXOld, 0);
                mobjSysVarPos.SetValueXyzwpr(ref xyzwpr, ref config, intUF, intUT);

                //confirm again
                mobjDataTable.Refresh();
                mobjSysVarInt2.GetValue(ref objTmp);
                lngConf = (int)objTmp;
                System.Diagnostics.Debug.Assert(lngOld == lngConf, "");
                mobjSysVarString.GetValue(ref objTmp);
                strConf = (string)objTmp;
                System.Diagnostics.Debug.Assert(strOld == strConf, "");
                mobjSysVarReal2.GetValue(ref objTmp);
                sngConf = (float)objTmp;
                System.Diagnostics.Debug.Assert(sngOld == sngConf, "");
                mobjSysVarPos.GetValue(ref xyzwpr, ref config, ref joint, ref intUF, ref intUT, ref intValidC, ref intValidJ);
                System.Diagnostics.Debug.Assert(sngXOld == (float)xyzwpr.GetValue(0), "");

                System.Windows.Forms.Cursor.Current = Cursors.Default;
                return;
            }
            catch (Exception ex)
            {
                System.Windows.Forms.Cursor.Current = Cursors.Default;
                MessageBox.Show(ex.Message);
            }

        }

        private void msubInit()
	    {
		    bool blnRes = false;
		    string strHost = null;
		    int lngTmp = 0;

            try {
		        System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.WaitCursor;

		        mobjCore = new FRRJIf.Core();

                // You need to set data table before connecting.
		        mobjDataTable = mobjCore.DataTable;

		        {
			        mobjAlarm = mobjDataTable.AddAlarm(FRRJIf.FRIF_DATA_TYPE.ALARM_LIST, 5, 0);
			        mobjAlarmCurrent = mobjDataTable.AddAlarm(FRRJIf.FRIF_DATA_TYPE.ALARM_CURRENT, 1, 0);
			        mobjCurPos = mobjDataTable.AddCurPos(FRRJIf.FRIF_DATA_TYPE.CURPOS, 1);
			        mobjCurPosUF = mobjDataTable.AddCurPosUF(FRRJIf.FRIF_DATA_TYPE.CURPOS, 1, 15);
			        mobjCurPos2 = mobjDataTable.AddCurPos(FRRJIf.FRIF_DATA_TYPE.CURPOS, 2);
			        mobjTask = mobjDataTable.AddTask(FRRJIf.FRIF_DATA_TYPE.TASK, 1);
                    mobjTaskIgnoreMacro = mobjDataTable.AddTask(FRRJIf.FRIF_DATA_TYPE.TASK_IGNORE_MACRO, 1);
                    mobjTaskIgnoreKarel = mobjDataTable.AddTask(FRRJIf.FRIF_DATA_TYPE.TASK_IGNORE_KAREL, 1);
                    mobjTaskIgnoreMacroKarel = mobjDataTable.AddTask(FRRJIf.FRIF_DATA_TYPE.TASK_IGNORE_MACRO_KAREL, 1);
                    mobjPosReg = mobjDataTable.AddPosReg(FRRJIf.FRIF_DATA_TYPE.POSREG, 1, 1, 10);
			        mobjPosReg2 = mobjDataTable.AddPosReg(FRRJIf.FRIF_DATA_TYPE.POSREG, 2, 1, 4);
			        mobjSysVarInt = mobjDataTable.AddSysVar(FRRJIf.FRIF_DATA_TYPE.SYSVAR_INT, "$FAST_CLOCK");
			        mobjSysVarInt2 = mobjDataTable.AddSysVar(FRRJIf.FRIF_DATA_TYPE.SYSVAR_INT, "$TIMER[10].$TIMER_VAL");
			        mobjSysVarReal = mobjDataTable.AddSysVar(FRRJIf.FRIF_DATA_TYPE.SYSVAR_REAL, "$MOR_GRP[1].$CURRENT_ANG[1]");
			        mobjSysVarReal2 = mobjDataTable.AddSysVar(FRRJIf.FRIF_DATA_TYPE.SYSVAR_REAL, "$DUTY_TEMP");
			        mobjSysVarString = mobjDataTable.AddSysVar(FRRJIf.FRIF_DATA_TYPE.SYSVAR_STRING, "$TIMER[10].$COMMENT");
			        mobjSysVarPos = mobjDataTable.AddSysVarPos(FRRJIf.FRIF_DATA_TYPE.SYSVAR_POS, "$MNUTOOL[1,1]");
                    mobjVarString = mobjDataTable.AddSysVar(FRRJIf.FRIF_DATA_TYPE.SYSVAR_STRING, "$[HTTPKCL]CMDS[1]");
			        mobjNumReg = mobjDataTable.AddNumReg(FRRJIf.FRIF_DATA_TYPE.NUMREG_INT, 1, 5);
			        mobjNumReg2 = mobjDataTable.AddNumReg(FRRJIf.FRIF_DATA_TYPE.NUMREG_REAL, 6, 10);
			        mobjPosRegXyzwpr = mobjDataTable.AddPosRegXyzwpr(FRRJIf.FRIF_DATA_TYPE.POSREG_XYZWPR, 1, 1, 10);
                    mobjPosRegMG = mobjDataTable.AddPosRegMG(FRRJIf.FRIF_DATA_TYPE.POSREGMG, "C,J6", 1, 10);
                    mobjStrReg = mobjDataTable.AddString(FRRJIf.FRIF_DATA_TYPE.STRREG, 1, 3);
                    mobjStrRegComment = mobjDataTable.AddString(FRRJIf.FRIF_DATA_TYPE.STRREG_COMMENT, 1, 3);
                    Debug.Assert(mobjStrRegComment != null);
		        }

                // 2nd data table.
                // You must not set the first data table.
                mobjDataTable2 = mobjCore.DataTable2;
			    mobjNumReg3 = mobjDataTable2.AddNumReg(FRRJIf.FRIF_DATA_TYPE.NUMREG_INT, 1, 5);
                mobjSysVarIntArray = new FRRJIf.DataSysVar[10];
                mobjSysVarIntArray[0] = mobjDataTable2.AddSysVar(FRRJIf.FRIF_DATA_TYPE.SYSVAR_INT, "$TIMER[1].$TIMER_VAL");
                mobjSysVarIntArray[1] = mobjDataTable2.AddSysVar(FRRJIf.FRIF_DATA_TYPE.SYSVAR_INT, "$TIMER[2].$TIMER_VAL");
                mobjSysVarIntArray[2] = mobjDataTable2.AddSysVar(FRRJIf.FRIF_DATA_TYPE.SYSVAR_INT, "$TIMER[3].$TIMER_VAL");
                mobjSysVarIntArray[3] = mobjDataTable2.AddSysVar(FRRJIf.FRIF_DATA_TYPE.SYSVAR_INT, "$TIMER[4].$TIMER_VAL");
                mobjSysVarIntArray[4] = mobjDataTable2.AddSysVar(FRRJIf.FRIF_DATA_TYPE.SYSVAR_INT, "$TIMER[5].$TIMER_VAL");
                mobjSysVarIntArray[5] = mobjDataTable2.AddSysVar(FRRJIf.FRIF_DATA_TYPE.SYSVAR_INT, "$TIMER[6].$TIMER_VAL");
                mobjSysVarIntArray[6] = mobjDataTable2.AddSysVar(FRRJIf.FRIF_DATA_TYPE.SYSVAR_INT, "$TIMER[7].$TIMER_VAL");
                mobjSysVarIntArray[7] = mobjDataTable2.AddSysVar(FRRJIf.FRIF_DATA_TYPE.SYSVAR_INT, "$TIMER[8].$TIMER_VAL");
                mobjSysVarIntArray[8] = mobjDataTable2.AddSysVar(FRRJIf.FRIF_DATA_TYPE.SYSVAR_INT, "$TIMER[9].$TIMER_VAL");
                mobjSysVarIntArray[9] = mobjDataTable2.AddSysVar(FRRJIf.FRIF_DATA_TYPE.SYSVAR_INT, "$TIMER[10].$TIMER_VAL");

		        //get host name
		        if (string.IsNullOrEmpty(HostName)) {
			        strHost = Interaction.GetSetting(cnstApp, cnstSection, "HostName", "");
			        strHost = Interaction.InputBox("Please input robot host name", "frrjiftest" , strHost, 0, 0);
			        if (string.IsNullOrEmpty(strHost)) {
				        System.Environment.Exit(0);
			        }
			        Interaction.SaveSetting(cnstApp, cnstSection, "HostName", strHost);
			        HostName = strHost;
		        } else {
			        strHost = HostName;
		        }

		        //get time out value
		        lngTmp = Convert.ToInt32(Interaction.GetSetting(cnstApp, cnstSection, "TimeOut", "-1"));

		        //connect
                if (lngTmp > 0)
                    mobjCore.TimeOutValue = lngTmp;
		        blnRes = mobjCore.Connect(strHost);
		        if (blnRes == false) {
			        msubDisconnected();
		        } else {
			        msubConnected();
		        }

	            System.Windows.Forms.Cursor.Current = Cursors.Default;
		        return;
            }
            catch (Exception ex) {
		        System.Windows.Forms.Cursor.Current = Cursors.Default;
		        MessageBox.Show(ex.Message);
		        System.Environment.Exit(0);
            }


	    }

        private void frm_Load(System.Object eventSender, System.EventArgs eventArgs)
        {
            msubInit();
        }

        private void frm_FormClosed(System.Object eventSender, System.Windows.Forms.FormClosedEventArgs eventArgs)
        {
            if (mobjCore != null)
            {
                mobjCore.Disconnect();
            }
            mobjCore = null;
        }

        private string mstrIO(string strIOType, short StartIndex, short EndIndex, ref Array values)
        {
            string tmp = null;
            int ii = 0;

            tmp = strIOType + "[" + Convert.ToString(StartIndex) + "-" + Convert.ToString(EndIndex) + "]=";
            for (ii = 0; ii <= EndIndex - StartIndex; ii++)
            {
                if ((short)values.GetValue(ii) == 0)
                {
                    tmp = tmp + "0";
                }
                else
                {
                    tmp = tmp + "1";
                }
            }

            return tmp;
        }

        private string mstrIO2(string strIOType, short StartIndex, short EndIndex, ref Array values)
        {
            string tmp = null;
            int ii = 0;

            tmp = strIOType + "[" + Convert.ToString(StartIndex) + "-" + Convert.ToString(EndIndex) + "]=";
            for (ii = 0; ii <= EndIndex - StartIndex; ii++)
            {
                if (ii != 0)
                {
                    tmp = tmp + ",";
                }
                tmp = tmp + values.GetValue(ii);
            }

            return tmp;
        }


        private string mstrPos(ref Array xyzwpr, ref Array config, ref Array joint, short intValidC, short intValidJ, int UF, int UT)
        {
            string tmp = "";
            int ii = 0;

            tmp = tmp + "UF = " + UF + ", ";
            tmp = tmp + "UT = " + UT + "\r\n";
            if (intValidC != 0)
            {
                tmp = tmp + "XYZWPR = ";
                //5
                for (ii = 0; ii <= 8; ii++)
                {
                    tmp = tmp + xyzwpr.GetValue(ii) + " ";
                }

                tmp = tmp + "\r\n" + "CONFIG = ";
                if ((short)config.GetValue(0) != 0)
                {
                    tmp = tmp + "F ";
                }
                else
                {
                    tmp = tmp + "N ";
                }
                if ((short)config.GetValue(1) != 0)
                {
                    tmp = tmp + "L ";
                }
                else
                {
                    tmp = tmp + "R ";
                }
                if ((short)config.GetValue(2) != 0)
                {
                    tmp = tmp + "U ";
                }
                else
                {
                    tmp = tmp + "D ";
                }
                if ((short)config.GetValue(3) != 0)
                {
                    tmp = tmp + "T ";
                }
                else
                {
                    tmp = tmp + "B ";
                }
                tmp = tmp + String.Format("{0}, {1}, {2}\r\n", config.GetValue(4), config.GetValue(5), config.GetValue(6));
            }

            if (intValidJ != 0)
            {
                tmp = tmp + "JOINT = ";
                //5
                for (ii = 0; ii <= 8; ii++)
                {
                    tmp = tmp + joint.GetValue(ii) + " ";
                }
                tmp = tmp + "\r\n";
            }

            return tmp;

        }


        private string mstrTask(int Index, string strProg, short intLine, short intState, string strParentProg)
        {
            string tmp = null;

            tmp = "TASK" + Index + " : ";
            tmp = tmp + " Prog=" + Strings.Chr(34) + strProg + Strings.Chr(34);
            tmp = tmp + " Line=" + intLine;
            tmp = tmp + " State=" + intState;
            tmp = tmp + " ParentProg=" + Strings.Chr(34) + strParentProg + Strings.Chr(34);

            return tmp + "\r\n";
        }


        private string mstrAlarm(ref FRRJIf.DataAlarm objAlarm, int Count)
        {
            string tmp = null;
            short intID = 0;
            short intNumber = 0;
            short intCID = 0;
            short intCNumber = 0;
            short intSeverity = 0;
            short intY = 0;
            short intM = 0;
            short intD = 0;
            short intH = 0;
            short intMn = 0;
            short intS = 0;
            string strM1 = "";
            string strM2 = "";
            string strM3 = "";
            bool blnRes = false;

            blnRes = objAlarm.GetValue(Count, ref intID, ref intNumber, ref intCID, ref intCNumber, ref intSeverity, ref intY, ref intM, ref intD, ref intH,
            ref intMn, ref intS, ref strM1, ref strM2, ref strM3);
            tmp = "-- Alarm " + Count + " --\r\n";
            if (blnRes == false)
            {
                tmp = tmp + "Error\r\n";
                return tmp;
            }
            tmp = tmp + intID + ", " + intNumber + ", " + intCID + ", " + intCNumber + ", " + intSeverity + "\r\n";
            tmp = tmp + intY + "/" + intM + "/" + intD + ", " + intH + ":" + intMn + ":" + intS + "\r\n";
            if (!string.IsNullOrEmpty(strM1))
                tmp = tmp + strM1 + "\r\n";
            if (!string.IsNullOrEmpty(strM2))
                tmp = tmp + strM2 + "\r\n";
            if (!string.IsNullOrEmpty(strM3))
                tmp = tmp + strM3 + "\r\n";

            return tmp;
        }


        public void mnuAbout_Click(System.Object eventSender, System.EventArgs eventArgs)
        {
            string strTmp = null;
            bool blnCreated = false;

            //check
            if (mobjCore == null)
            {
                mobjCore = new FRRJIf.Core();
                blnCreated = true;
            }

            {
                Microsoft.VisualBasic.ApplicationServices.ApplicationBase app = new Microsoft.VisualBasic.ApplicationServices.ApplicationBase();
       
                strTmp = app.Info.Title + " V" + app.Info.Version.Major + "." + app.Info.Version.Minor + "." + app.Info.Version.Revision + "\r\n" + "\r\n";
                strTmp = strTmp + "FRRJIF Protect Available = " + mobjCore.ProtectAvailable + "\r\n";
                strTmp = strTmp + "FRRJIF Protect Trial Remain Days = " + mobjCore.ProtectTrialRemainDays + "\r\n";
                strTmp = strTmp + "FRRJIF Protect Status = " + mobjCore.ProtectStatus + "\r\n";
                strTmp = strTmp + "FRRJIF Protect Error Number = " + mobjCore.ProtectErrorNumber + "\r\n";
            }

            MessageBox.Show(strTmp);

            //if created here, clear it
            if (blnCreated == true)
            {
                mobjCore = null;
            }

        }

        public void mnuExit_Click(System.Object eventSender, System.EventArgs eventArgs)
        {
            this.Close();
            System.Environment.Exit(0);
        }


        public void mnuTimeOut_Click(System.Object eventSender, System.EventArgs eventArgs)
	    {
		    int lngTmp = 0;
		    string strTmp = null;

		    if (mobjCore == null) {
			    MessageBox.Show("Not connected");
			    return;
		    }

		    lngTmp = mobjCore.TimeOutValue;
		    strTmp = Interaction.InputBox("Please input time out value (msec)", "frrjiftest", Convert.ToString(lngTmp), 0, 0);
		    lngTmp = Convert.ToInt32(strTmp);
		    mobjCore.TimeOutValue=lngTmp;

		    //check value
		    if (mobjCore.TimeOutValue != lngTmp) {
			    MessageBox.Show("Invalid value");
			    return;
		    }

		    //save it
		    Interaction.SaveSetting(cnstApp, cnstSection, "TimeOut", Convert.ToString(lngTmp));

	    }

        private void timLoop_Tick(System.Object eventSender, System.EventArgs eventArgs)
        {
            if (chkLoop.Checked)
            {
                cmdRefresh.PerformClick();
            }
        }

        private void msubSetTestControls(bool blnEnabled)
        {

            chkLoop.Enabled = blnEnabled;
            cmdRefresh.Enabled = blnEnabled;
            cmdRefresh2.Enabled = blnEnabled;
            cmdSetNumReg.Enabled = blnEnabled;
            cmdSetPosReg.Enabled = blnEnabled;
            cmdSetPosRegX.Enabled = blnEnabled;
            _cmdSetSDO_0.Enabled = blnEnabled;
            _cmdSetSDO_1.Enabled = blnEnabled;
            _cmdSetSDO_2.Enabled = blnEnabled;
            cmdSetSDI.Enabled = blnEnabled;
            cmdSetRDO.Enabled = blnEnabled;
            cmdSetRDI.Enabled = blnEnabled;
            _cmdSetGO_0.Enabled = blnEnabled;
            _cmdSetGO_1.Enabled = blnEnabled;
            cmdSetGI.Enabled = blnEnabled;
            cmdWriteSysVar.Enabled = blnEnabled;
            cmdSetPosRegX2.Enabled = blnEnabled;
            cmdSetPosRegMG.Enabled = blnEnabled;
            cmdSetStrReg.Enabled = blnEnabled;

        }

        private void msubConnected()
        {

            txtResult.Text = "Connect OK to " + HostName;
            lblConnect.Text = txtResult.Text;
            this.Text = HostName + " - FRRJIf Test";

            msubSetTestControls(true);
            cmdConnect.Text = "Disconnect";

            timLoop.Enabled = true;
        }

        private void msubDisconnected()
        {

            //disabled continous
            timLoop.Enabled = false;

            MessageBox.Show("Connect error");

            txtResult.Text = "Connect Failed to " + HostName;
            lblConnect.Text = txtResult.Text;
            this.Text = HostName + " - FRRJIf Test";

            msubClearVars();

            msubSetTestControls(false);
            cmdConnect.Text = "Connect";

        }

        private void msubDisconnected2()
        {

            //disabled continous
            timLoop.Enabled = false;

            txtResult.Text = "Disconnect to " + HostName;
            // & " (" & mobjCore.ProtectStatus & ")"
            lblConnect.Text = txtResult.Text;
            this.Text = "FRRJIf Test";

            msubClearVars();

            msubSetTestControls(false);
            cmdConnect.Text = "Connect";

        }


        private void msubClearVars()
        {

            mobjCore.Disconnect();

            mobjCore = null;
            mobjDataTable = null;
            mobjDataTable2 = null;
            mobjAlarm = null;
            mobjAlarmCurrent = null;
            mobjCurPos = null;
            mobjCurPos2 = null;
            mobjTask = null;
            mobjTaskIgnoreMacro = null;
            mobjTaskIgnoreKarel = null;
            mobjTaskIgnoreMacroKarel = null;
            mobjPosReg = null;
            mobjPosReg2 = null;
            mobjSysVarInt = null;
            mobjSysVarReal = null;
            mobjSysVarReal2 = null;
            mobjSysVarString = null;
            mobjSysVarPos = null;
            mobjNumReg = null;
            mobjNumReg2 = null;
            mobjNumReg3 = null;
            mobjVarString = null;
            mobjStrReg = null;
            mobjStrRegComment = null;
            for (int ii = mobjSysVarIntArray.GetLowerBound(0); ii <= mobjSysVarIntArray.GetUpperBound(0); ii++)
            {
                mobjSysVarIntArray[ii] = null;
            }

        }

        private void cmdSetStrReg_Click(object sender, EventArgs e)
        {
            int intRand = 0;
            int ii = 0;
            double dblTime;
            string strTmp;
            bool blnResult;

            dblTime = gflngGetTickCountEx();
            intRand = rnd.Next(1,10);

            for (ii = mobjStrReg.StartIndex; ii <=mobjStrReg.EndIndex; ii++)
            {
                strTmp =string.Format("str{0}",  (ii + intRand));
				blnResult = mobjStrReg.SetValue(ii, strTmp);
				System.Diagnostics.Debug.Assert((blnResult), "");
            }

            //Need to call Update to send data.
			blnResult = mobjStrReg.Update();
			System.Diagnostics.Debug.Assert((blnResult), "");

            Debug.Print(String.Format("Time {0} ms", gflngGetTickCountEx() - dblTime));
            cmdRefresh.PerformClick();
        }

        private void cmdRefresh2_Click(object sender, EventArgs e)
        {
            string strTmp = null;
            double dblTime = 0;
            bool blnDT = false;
            object vntValue = null;
            int intRand = 0;
            int ii = 0;
            int[] intValues = new int[101];
            float[] sngValues = new float[101];

            intRand = rnd.Next(1, 10);
            {
                for (ii = 0; ii <= mobjNumReg3.EndIndex - mobjNumReg3.StartIndex; ii++)
                {
                    intValues[ii] = (ii + 1) * intRand;
                }
                if (mobjNumReg3.SetValues(mobjNumReg3.StartIndex, intValues, mobjNumReg3.EndIndex - mobjNumReg3.StartIndex + 1) == false)
                {
                    MessageBox.Show("SetNumReg Int Error");
                }
            }

            //check
            if (mobjCore == null)
            {
                return;
            }

            cmdRefresh2.Enabled = false;
            System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.WaitCursor;

            dblTime = gflngGetTickCountEx();

            //Refresh data table
            blnDT = mobjDataTable2.Refresh();
            if (blnDT == false)
            {
                System.Windows.Forms.Cursor.Current = Cursors.Default;
                msubDisconnected();
                return;
            }

            dblTime = gflngGetTickCountEx() - dblTime;
            strTmp = "Time = " + Convert.ToInt16(dblTime) + "(msec)\r\n";

            strTmp = strTmp + "--- NumReg ---\r\n";
            {
                for (ii = mobjNumReg3.StartIndex; ii <= mobjNumReg3.EndIndex; ii++)
                {
                    if (mobjNumReg3.GetValue(ii, ref vntValue) == true)
                    {
                        strTmp = strTmp + "R[" + ii + "] = " + vntValue + "\r\n";
                    }
                    else
                    {
                        strTmp = strTmp + "R[" + ii + "] : Error!!! \r\n";
                    }
                }
            }
            strTmp = strTmp + "--- SysVar ---\r\n";
            for (ii = mobjSysVarIntArray.GetLowerBound(0); ii <= mobjSysVarIntArray.GetUpperBound(0); ii++)
            {
                if (mobjSysVarIntArray[ii].GetValue(ref vntValue) == true)
                {
                    strTmp = strTmp + mobjSysVarIntArray[ii].SysVarName + " = " + vntValue + "\r\n";
                }
                else
                {
                    strTmp = strTmp + mobjSysVarIntArray[ii].SysVarName + " : Error!!! \r\n";
                }
            }

            txtResult.Text = strTmp;

            cmdRefresh2.Enabled = true;
            System.Windows.Forms.Cursor.Current = Cursors.Default;
        }

        private void cmdSetPosRegMG_Click(object sender, EventArgs e)
        {
            int intRand = 0;
            int ii = 0;
            int jj = 0;
            Array sngArray = new float[6];
            Array sngJoint = new float[6];
            Array intConfig = new short[7];
            bool blnRes = false;
            long lngTime = 0;

            lngTime = gflngGetTickCountEx();
            intRand = rnd.Next(1, 10);
            {
                for (ii = mobjPosRegMG.StartIndex; ii <= mobjPosRegMG.EndIndex; ii++)
                {
                    for (jj = sngArray.GetLowerBound(0); jj <= sngArray.GetUpperBound(0); jj++)
                    {
                        sngArray.SetValue((float)(11.11 * (jj + 1) * intRand * ii), jj);
                    }
                    intConfig.SetValue((short)ii, 4);
                    intConfig.SetValue((short)ii, 5);
                    intConfig.SetValue((short)ii, 6);
                    blnRes = mobjPosRegMG.SetValueXyzwpr(ii, 1,ref sngArray, ref intConfig);
                    if (blnRes == false)
                    {
                        MessageBox.Show("Error mobjPosRegMG.SetValueXyzwpr");
                    }
                    for (jj = sngJoint.GetLowerBound(0); jj <= sngJoint.GetUpperBound(0); jj++)
                    {
                        sngJoint.SetValue((float)(11.11 * (jj + 1) * intRand * ii), jj);
                    }
                    blnRes = mobjPosRegMG.SetValueJoint(ii, 2, ref sngJoint);
                    if (blnRes == false)
                    {
                        MessageBox.Show("Error mobjPosRegMG.SetValueJoint");
                    }
                }
                blnRes = mobjPosRegMG.Update();
                if (blnRes == false)
                {
                    MessageBox.Show("Error mobjPosRegMG.Update");
                }
            }
            Debug.Print(string.Format("Time {0} ms", gflngGetTickCountEx() - lngTime));
            cmdRefresh.PerformClick();

        }
    }
}