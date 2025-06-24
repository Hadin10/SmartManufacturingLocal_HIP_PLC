using RestSharp;
using S7.Net;
using S7.Net.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SmartManufacturingLocal_HIP_PLC
{
    public partial class Form1 : Form
    {
        SmartManufacturingV2Entities db_SMV2 = new SmartManufacturingV2Entities();

        CancellationTokenSource tokenSource = null;
        //string fileName = @"C:\LogFile.txt";
        string fileName = "";
        public Form1()
        {
            InitializeComponent();
        }

        private void Start_Click(object sender, EventArgs e)
        {
            notify.Text = "Starting...";
            StartTask(true);
        }
        private void Stop_Click(object sender, EventArgs e)
        {
            tokenSource.Cancel();
        }
        private async void StartTask(bool cmd)
        {
            tokenSource = new CancellationTokenSource();
            var token = tokenSource.Token;

            var progress = new Progress<int>(value =>
            {
                progressBar.Value = value;
                label2.Text = $"{value} %";
            });
            if (Directory.Exists(@"D:\"))
            {
                fileName = @"D:\" + "HIP_LogFile.txt";
            }
            else if (Directory.Exists(@"E:\"))
            {
                fileName = @"E:\" + "HIP_LogFile.txt";
            }
            else
            {
                fileName = Path.Combine(Directory.GetCurrentDirectory(), "HIP_LogFile.txt");
            }
            if (!File.Exists(fileName))
            {
                using (FileStream fs = File.Create(fileName))
                {

                }
            }

            try
            {
                while (cmd)
                {
                    try
                    {
                        File.AppendAllText(fileName, System.DateTime.Now.ToString() + "\n");
                        if (!token.IsCancellationRequested)
                        {
                            System.DateTime Start_Time = System.DateTime.Now;
                            await Task.Run(() => LongRunningTask(progress, token));

                            System.DateTime End_Time = System.DateTime.Now;
                            if ((End_Time - Start_Time).TotalSeconds < 305)
                            {
                                double Delay_Time = 305 - (End_Time - Start_Time).TotalSeconds;
                                await Task.Delay((int)Delay_Time * 1000);
                            }
                            notify.Text = "Updated On: " + System.DateTime.Now.ToString();                            
                        }
                        else
                        {
                            notify.Text = "Stopped";
                            cmd = false;
                        }
                    }
                    catch (Exception ex)
                    {
                        File.AppendAllText(fileName, "HIP Listener error in cmd: \n" + System.DateTime.Now.ToString());
                    }
                }
            }
            catch (OperationCanceledException ex)
            {
                File.AppendAllText(fileName, "HIP Listener error in cancel: \n" + System.DateTime.Now.ToString());
            }
            catch (Exception ex)
            {
                File.AppendAllText(fileName, "HIP Listener Start in error: \n" + System.DateTime.Now.ToString());
            }
        }
        //private async Task<string> LongRunningTask(IProgress<int> progress, CancellationToken token)
        private string LongRunningTask(IProgress<int> progress, CancellationToken token)
        {
            List<ReadMachineData> ts = new List<ReadMachineData>();
           
            try
            {
                #region PLC
                string plcIp = "";
                #region 192.168.168.50
                try
                {
                    plcIp = "192.168.168.50";
                    using (var plc = new Plc(CpuType.S71500, plcIp, 0, 1))
                    {
                        try
                        {
                            if (!plc.IsConnected)
                            {
                                plc.Open();
                            }

                            try
                            {
                                byte[] byte1 = plc.ReadBytes(DataType.DataBlock, 127, 1596, 3204);
                                uint[] result1 = DWord.ToArray(byte1);

                                //byte[] byte2 = plc.ReadBytes(DataType.DataBlock, 125, 0, 514);
                                //var result2 = Bit.ToBitArray(byte2, 70);
                                byte[] byte2 = plc.ReadBytes(DataType.DataBlock, 125, 0, 514);
                                var result2 = DWord.ToArray(byte2);

                                byte[] byte3 = plc.ReadBytes(DataType.DataBlock, 147, 0, 48);
                                var result3 = DWord.ToArray(byte3);

                                byte[] byte4 = plc.ReadBytes(DataType.DataBlock, 237, 0, 48);
                                var result4 = DWord.ToArray(byte4);
                                //HIP-> PML-> Husky
                                byte[] byte5 = plc.ReadBytes(DataType.DataBlock, 276, 84, 84);
                                var result5 = DWord.ToArray(byte5);
                                byte[] byte9 = plc.ReadBytes(DataType.DataBlock, 276, 908, 52);
                                var result9 = DWord.ToArray(byte9);
                                //HIP-> PML-> Injection
                                byte[] byte6 = plc.ReadBytes(DataType.DataBlock, 233, 0, 404);
                                var result6 = DWord.ToArray(byte6);
                                //HIP-> BBML-> Door -> with Live Weight
                                byte[] byte7 = plc.ReadBytes(DataType.DataBlock, 136, 0, 52);
                                var result7 = DWord.ToArray(byte7);
                                byte[] byte8 = plc.ReadBytes(DataType.DataBlock, 136, 60, 52);
                                var result8 = DWord.ToArray(byte8);

                                // Runtime
                                byte[] byte10 = plc.ReadBytes(DataType.DataBlock, 302, 0, 100);
                                var result10 = DWord.ToArray(byte10);
                                byte[] byte11 = plc.ReadBytes(DataType.DataBlock, 303, 0, 100);
                                var result11 = DWord.ToArray(byte11);
                                ////HIP firoz vai(03-11-2024)
                                byte[] byte12 = plc.ReadBytes(DataType.DataBlock, 305, 0, 80);
                                var result12 = DWord.ToArray(byte12);

                                //Melamine 
                                byte[] byte13 = plc.ReadBytes(DataType.DataBlock, 307, 0, 50);
                                var result13 = DWord.ToArray(byte13);

                                byte[] byte14 = plc.ReadBytes(DataType.DataBlock, 318, 0, 240);
                                var result14 = DWord.ToArray(byte14);
                                byte[] byte15 = plc.ReadBytes(DataType.DataBlock, 319, 0, 240);
                                var result15 = DWord.ToArray(byte15);

                                for (int i = 0; i < 801; i++)
                                {
                                    if (i != 0 && (i < 167 || i > 186))
                                    {
                                        var totalCountList = new List<long>() { (long)result1[i] };//16850127718
                                        string newId = "16850127" + i;
                                        ReadMachineData data = new ReadMachineData
                                        {

                                            MachineID = Convert.ToInt64(newId),
                                            //TotalCount = (long)result1[i],
                                            TotalCount = totalCountList,
                                            //Break = (bool)result2[i],
                                            //Sched = (bool)result3[i]
                                            Break = false,
                                            Sched = false
                                        };

                                        ts.Add(data);
                                    }

                                }
                                for (int i = 0; i < 128; i++)
                                {
                                    if (i != 0)
                                    {
                                        var totalCountList = new List<long>() { (long)result2[i] };
                                        string newId = "16850125" + i;
                                        ReadMachineData data = new ReadMachineData
                                        {

                                            MachineID = Convert.ToInt64(newId),
                                            //TotalCount = (long)result2[i],
                                            TotalCount = totalCountList,
                                            //Break = (bool)result2[i],
                                            //Sched = (bool)result3[i]
                                            Break = false,
                                            Sched = false
                                        };

                                        ts.Add(data);
                                    }

                                }
                                for (int i = 0; i < 12; i++)
                                {
                                    if (i != 0)
                                    {
                                        var totalCountList = new List<long>() { (long)result3[i], (long)result4[i] };
                                        string newId = "16850147" + i;
                                        ReadMachineData data = new ReadMachineData
                                        {
                                            MachineID = Convert.ToInt64(newId),
                                            TotalCount = totalCountList,
                                            Break = false,
                                            Sched = false
                                        };

                                        ts.Add(data);
                                    }
                                }
                                for (int i = 0; i < 21; i++)
                                {
                                    if (i != 0)
                                    {
                                        var totalCountList = new List<long>() { (long)result5[i] };//16850276203
                                        string newId = "16850276" + (21 + i);
                                        ReadMachineData data = new ReadMachineData
                                        {
                                            MachineID = Convert.ToInt64(newId),
                                            //TotalCount = (long)result1[i],
                                            TotalCount = totalCountList,
                                            Break = false,
                                            Sched = false
                                        };

                                        ts.Add(data);
                                    }

                                }
                                for (int i = 0; i < 13; i++)
                                {
                                    if (i != 0)
                                    {
                                        var totalCountList = new List<long>() { (long)result9[i] };
                                        string newId = "16850276" + (227 + i);
                                        ReadMachineData data = new ReadMachineData
                                        {
                                            MachineID = Convert.ToInt64(newId),
                                            //TotalCount = (long)result1[i],
                                            TotalCount = totalCountList,
                                            Break = false,
                                            Sched = false
                                        };

                                        ts.Add(data);
                                    }
                                }
                                for (int i = 0; i < 101; i++)
                                {
                                    if (i != 0)
                                    {
                                        var totalCountList = new List<long>() { (long)result6[i] };//4294967251.00//1685023388
                                        string newId = "16850233" + i;
                                        ReadMachineData data = new ReadMachineData
                                        {
                                            MachineID = Convert.ToInt64(newId),
                                            TotalCount = totalCountList,
                                            Break = false,
                                            Sched = false
                                        };

                                        ts.Add(data);
                                    }
                                }
                                for (int i = 0; i < 2; i++)
                                {
                                    if (i != 0)
                                    {
                                        var totalCountList = new List<long>() { (long)result7[i], 0, (long)result8[i] };
                                        string newId = "16850136" + i;
                                        ReadMachineData data = new ReadMachineData
                                        {
                                            MachineID = Convert.ToInt64(newId),
                                            TotalCount = totalCountList,
                                            Break = false,
                                            Sched = false
                                        };

                                        ts.Add(data);
                                    }
                                }

                                // New 
                                for (int i = 0; i < 25; i++)
                                {
                                    if (i != 0)
                                    {
                                        var totalCountList = new List<long>() { (long)result11[i], (long)result10[i] };//1685030312
                                        string newId = "16850303" + i;
                                        ReadMachineData data = new ReadMachineData
                                        {
                                            MachineID = Convert.ToInt64(newId),
                                            TotalCount = totalCountList,
                                            Break = false,
                                            Sched = false
                                        };

                                        ts.Add(data);
                                    }
                                }
                                //new//HIP firoz vai(03-11-2024)update on 18/11/2024
                                for (int i = 0; i < 20; i++)
                                {
                                    if (i != 0)
                                    {
                                        var totalCountList = new List<long>() { (long)result12[i] };
                                        string newId = "16850305" + i;
                                        ReadMachineData data = new ReadMachineData
                                        {
                                            MachineID = Convert.ToInt64(newId),
                                            TotalCount = totalCountList,
                                            Break = false,
                                            Sched = false
                                        };

                                        ts.Add(data);
                                    }
                                }
                                for (int i = 1; i < 11; i++)
                                {
                                    var totalCountList = new List<long>() { (long)result13[i] };
                                    string newId = "16850307" + i;
                                    ReadMachineData data = new ReadMachineData
                                    {
                                        MachineID = Convert.ToInt64(newId),
                                        TotalCount = totalCountList,
                                        Break = false,
                                        Sched = false
                                    };

                                    ts.Add(data);
                                }

                                for (int i = 1; i < 60; i++)
                                {
                                    var totalCountList = new List<long>() { (long)result14[i], (long)result15[i] };
                                    string newId = "16850318" + i;
                                    ReadMachineData data = new ReadMachineData
                                    {
                                        MachineID = Convert.ToInt64(newId),
                                        TotalCount = totalCountList,
                                        Break = false,
                                        Sched = false
                                    };

                                    ts.Add(data);
                                }
                            }
                            catch (Exception)
                            {

                            }

                            plc.Close();

                        }
                        catch (Exception)
                        {
                            Console.WriteLine(plcIp);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                #endregion

                #region 192.168.168.99
                try
                {
                    plcIp = "192.168.168.99";
                    using (var plc = new Plc(CpuType.S71500, plcIp, 0, 1))
                    {
                        try
                        {
                            if (!plc.IsConnected)
                            {
                                plc.Open();
                            }
                            try
                            {
                                byte[] byte1 = plc.ReadBytes(DataType.DataBlock, 1, 0, 2888);
                                uint[] result1 = DWord.ToArray(byte1);

                                byte[] byte2 = plc.ReadBytes(DataType.DataBlock, 40, 0, 60);
                                uint[] result2 = DWord.ToArray(byte2);

                                byte[] byte3 = plc.ReadBytes(DataType.DataBlock, 54, 0, 400);
                                uint[] result3 = DWord.ToArray(byte3);

                                for (int i = 0; i < 391; i++) //0
                                {
                                    if (i != 0)
                                    {
                                        var totalCountList = new List<long>() { (long)result1[i] }; //168991474
                                        string newId = "168991" + i;
                                        ReadMachineData data = new ReadMachineData
                                        {
                                            MachineID = Convert.ToInt64(newId),
                                            TotalCount = totalCountList,
                                            Break = false,
                                            Sched = false
                                        };

                                        ts.Add(data);
                                    }
                                }

                                for (int i = 391; i < 401; i++)
                                {
                                    if (i != 0)
                                    {
                                        var totalCountList = new List<long>() { (long)result1[i] }; //168099001395
                                        string newId = "168099001" + i;
                                        ReadMachineData data = new ReadMachineData
                                        {
                                            MachineID = Convert.ToInt64(newId),
                                            TotalCount = totalCountList,
                                            Break = false,
                                            Sched = false
                                        };

                                        ts.Add(data);
                                    }
                                }

                                for (int i = 401; i < 722; i++)
                                {
                                    if (i != 0)
                                    {
                                        var totalCountList = new List<long>() { (long)result1[i] }; //168991487
                                        string newId = "168991" + i;
                                        ReadMachineData data = new ReadMachineData
                                        {
                                            MachineID = Convert.ToInt64(newId),
                                            TotalCount = totalCountList,
                                            Break = false,
                                            Sched = false
                                        };

                                        ts.Add(data);
                                    }
                                }
                                //HIP> RPL
                                for (int i = 0; i < 15; i++)
                                {
                                    if (i != 0)
                                    {
                                        var totalCountList = new List<long>() { (long)result2[i] };
                                        string newId = "1689940" + i;
                                        ReadMachineData data = new ReadMachineData
                                        {
                                            MachineID = Convert.ToInt64(newId),
                                            TotalCount = totalCountList,
                                            Break = false,
                                            Sched = false
                                        };

                                        ts.Add(data);
                                    }
                                }
                                //HIP> BIZLI Cables
                                for (int i = 0; i < 100; i++)
                                {
                                    if (i != 0)
                                    {
                                        var totalCountList = new List<long>() { (long)result3[i] };
                                        string newId = "1689954" + i;
                                        ReadMachineData data = new ReadMachineData
                                        {
                                            MachineID = Convert.ToInt64(newId),
                                            TotalCount = totalCountList,
                                            Break = false,
                                            Sched = false
                                        };

                                        ts.Add(data);
                                    }
                                }
                            }
                            catch (Exception)
                            {

                            }

                            plc.Close();

                        }
                        catch (Exception)
                        {
                            Console.WriteLine(plcIp);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                #endregion


                #region 192.168.168.37
                try
                {
                    plcIp = "192.168.168.37";
                    using (var plc = new Plc(CpuType.S71200, plcIp, 0, 1))
                    {
                        try
                        {
                            if (!plc.IsConnected)
                            {
                                plc.Open();
                            }
                            try
                            {
                                byte[] byte1 = plc.ReadBytes(DataType.DataBlock, 1, 0, 604);
                                uint[] result1 = DWord.ToArray(byte1);
                                byte[] byte2 = plc.ReadBytes(DataType.DataBlock, 10, 0, 208);
                                uint[] result2 = DWord.ToArray(byte2);

                                for (int i = 0; i < 151; i++)
                                {
                                    if (i != 0)
                                    {
                                        var totalCountList = new List<long>() { (long)result1[i] };//168371103
                                        string newId = "168371" + i;
                                        ReadMachineData data = new ReadMachineData
                                        {

                                            MachineID = Convert.ToInt64(newId),
                                            TotalCount = totalCountList,
                                            Break = false,
                                            Sched = false
                                        };

                                        ts.Add(data);
                                    }
                                }
                                for (int i = 0; i < 52; i++)
                                {
                                    if (i != 0)
                                    {
                                        var totalCountList = new List<long>() { (long)result2[i] };//
                                        string newId = "1683710" + i;
                                        ReadMachineData data = new ReadMachineData
                                        {
                                            MachineID = Convert.ToInt64(newId),
                                            TotalCount = totalCountList,
                                            Break = false,
                                            Sched = false
                                        };

                                        ts.Add(data);
                                    }
                                }
                            }
                            catch (Exception)
                            {
                            }
                            plc.Close();
                        }
                        catch (Exception)
                        {
                            Console.WriteLine(plcIp);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                #endregion
                #region 192.168.168.62
                try
                {
                    plcIp = "192.168.168.62";
                    using (var plc = new Plc(CpuType.S71200, plcIp, 0, 1))
                    {
                        try
                        {
                            if (!plc.IsConnected)
                            {
                                plc.Open();
                            }
                            try
                            {
                                byte[] byte1 = plc.ReadBytes(DataType.DataBlock, 12, 0, 40);
                                uint[] result1 = DWord.ToArray(byte1);

                                for (int i = 0; i < 10; i++)
                                {
                                    if (i != 0)
                                    {
                                        var totalCountList = new List<long>() { (long)result1[i] };
                                        string newId = "1686212" + i;
                                        ReadMachineData data = new ReadMachineData
                                        {

                                            MachineID = Convert.ToInt64(newId),
                                            TotalCount = totalCountList,
                                            Break = false,
                                            Sched = false
                                        };

                                        ts.Add(data);
                                    }
                                }
                            }
                            catch (Exception)
                            {

                            }

                            plc.Close();

                        }
                        catch (Exception)
                        {
                            Console.WriteLine(plcIp);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                #endregion
                #region FetchDataForHIP_HIP_PML_Carton
                try
                {
                    plcIp = "192.168.168.49";
                    using (var plc = new Plc(CpuType.S71200, plcIp, 0, 1))
                    {
                        try
                        {
                            if (!plc.IsConnected)
                            {
                                plc.Open();
                            }
                            try
                            {
                                byte[] byte1 = plc.ReadBytes(DataType.DataBlock, 9, 0, 384);
                                byte[] byte2 = plc.ReadBytes(DataType.DataBlock, 17, 0, 100);
                                uint[] result1 = DWord.ToArray(byte1);
                                uint[] result2 = DWord.ToArray(byte2);

                                for (int i = 0; i < 96; i++)
                                {
                                    if (i != 0)
                                    {
                                        //var totalCountList = new List<long>() { (long)result1[i], (long)result2[i] };
                                        var totalCountList = new List<long>();
                                        string newId = "168499" + i;
                                        if (i < 25)
                                        {
                                            totalCountList = new List<long>() { (long)result1[i], (long)result2[i] };
                                        }
                                        else if (i >= 25)
                                        {
                                            totalCountList = new List<long>() { (long)result1[i] };//1684992
                                        }

                                        ReadMachineData data = new ReadMachineData
                                        {

                                            MachineID = Convert.ToInt64(newId),
                                            TotalCount = totalCountList,
                                            Break = false,
                                            Sched = false
                                        };

                                        ts.Add(data);
                                    }
                                }
                            }
                            catch (Exception)
                            {

                            }

                            plc.Close();

                        }
                        catch (Exception)
                        {
                            Console.WriteLine(plcIp);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                #endregion
                #region FetchDataForHIP_DongolToPLC
                try
                {
                    plcIp = "10.7.12.25";
                    using (var plc = new Plc(CpuType.S71200, plcIp, 0, 1))
                    {
                        try
                        {
                            if (!plc.IsConnected)
                            {
                                plc.Open();
                            }
                            try
                            {
                                byte[] byte1 = plc.ReadBytes(DataType.DataBlock, 17, 0, 744);
                                uint[] result1 = DWord.ToArray(byte1);

                                for (int i = 1; i < 186; i++)
                                {
                                    var totalCountList = new List<long>() { (long)result1[i] };//10712251793
                                    string newId = "107122517" + i;
                                    ReadMachineData data = new ReadMachineData
                                    {
                                        MachineID = Convert.ToInt64(newId),
                                        TotalCount = totalCountList,
                                        Break = false,
                                        Sched = false
                                    };
                                    ts.Add(data);
                                }
                            }
                            catch (Exception)
                            {

                            }

                            plc.Close();

                        }
                        catch (Exception)
                        {
                            Console.WriteLine(plcIp);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                #endregion
                #region Stationary
                try
                {
                    plcIp = "10.7.12.28";
                    using (var plc = new Plc(CpuType.S71200, plcIp, 0, 1))
                    {
                        try
                        {
                            if (!plc.IsConnected)
                            {
                                plc.Open();
                            }
                            try
                            {
                                byte[] byte1 = plc.ReadBytes(DataType.DataBlock, 11, 0, 1200);
                                uint[] result1 = DWord.ToArray(byte1);
                                for (int i = 1; i < 300; i++)
                                {
                                    var totalCountList = new List<long>() { (long)result1[i] };//107122811-150
                                    string newId = "107122811" + i;
                                    ReadMachineData data = new ReadMachineData
                                    {
                                        MachineID = Convert.ToInt64(newId),
                                        TotalCount = totalCountList,
                                        Break = false,
                                        Sched = false
                                    };
                                    ts.Add(data);
                                }
                            }
                            catch (Exception)
                            {

                            }

                            plc.Close();

                        }
                        catch (Exception)
                        {
                            Console.WriteLine(plcIp);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                #endregion
                #region 192.168.168.71
                try
                {
                    plcIp = "192.168.168.71";
                    using (var plc = new Plc(CpuType.S71200, plcIp, 0, 1))
                    {
                        try
                        {
                            if (!plc.IsConnected)
                            {
                                plc.Open();
                            }
                            try
                            {
                                byte[] byte1 = plc.ReadBytes(DataType.DataBlock, 12, 0, 300);
                                uint[] result1 = DWord.ToArray(byte1);

                                for (int i = 0; i < 75; i++)
                                {
                                    var totalCountList = new List<long>() { (long)result1[i] };
                                    string newId = "1687112" + i;
                                    ReadMachineData data = new ReadMachineData
                                    {
                                        MachineID = Convert.ToInt64(newId),
                                        TotalCount = totalCountList,
                                        Break = false,
                                        Sched = false
                                    };
                                    ts.Add(data);
                                }
                            }
                            catch (Exception)
                            {

                            }

                            plc.Close();

                        }
                        catch (Exception)
                        {
                            Console.WriteLine(plcIp);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                #endregion
                #region FetchDataFor_HIP_2_OPAL
                try
                {
                    plcIp = "10.253.76.106";
                    using (var plc = new Plc(CpuType.S7200Smart, plcIp, 0, 1))//10.263.76.106
                    {
                        try
                        {
                            if (!plc.IsConnected)
                            {
                                plc.Open();
                            }
                            try
                            {
                                byte[] byte1 = plc.ReadBytes(DataType.DataBlock, 1, 0, 52);
                                uint[] result1 = DWord.ToArray(byte1);

                                for (int i = 0; i < 13; i++)
                                {
                                    if (i != 0)
                                    {
                                        var totalCountList = new List<long>();
                                        string newId = "10253761061" + i;

                                        totalCountList = new List<long>() { (long)result1[i] };

                                        ReadMachineData data = new ReadMachineData
                                        {
                                            MachineID = Convert.ToInt64(newId),
                                            TotalCount = totalCountList,
                                            Break = false,
                                            Sched = false
                                        };

                                        ts.Add(data);
                                    }
                                }
                            }
                            catch (Exception)
                            {

                            }
                            plc.Close();

                        }
                        catch (Exception)
                        {
                            Console.WriteLine(plcIp);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                #endregion
                #region 10.7.12.91
                try
                {
                    plcIp = "10.7.12.91";
                    using (var plc = new Plc(CpuType.S71200, plcIp, 0, 1))
                    {
                        try
                        {
                            if (!plc.IsConnected)
                            {
                                plc.Open();
                            }
                            try
                            {
                                byte[] byte1 = plc.ReadBytes(DataType.DataBlock, 3, 0, 404);
                                uint[] result1 = DWord.ToArray(byte1);

                                for (int i = 0; i < 101; i++)
                                {
                                    if (i != 0)
                                    {
                                        var totalCountList = new List<long>();
                                        string newId = "10712913" + i;

                                        totalCountList = new List<long>() { (long)result1[i] };

                                        ReadMachineData data = new ReadMachineData
                                        {
                                            MachineID = Convert.ToInt64(newId),
                                            TotalCount = totalCountList,
                                            Break = false,
                                            Sched = false
                                        };

                                        ts.Add(data);
                                    }
                                }
                            }
                            catch (Exception)
                            {

                            }
                            plc.Close();

                        }
                        catch (Exception)
                        {
                            Console.WriteLine(plcIp);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                #endregion
                #region FetchDataForHIP_Noodels
                try
                {
                    plcIp = "10.7.12.137";
                    using (var plc = new Plc(CpuType.S71200, plcIp, 0, 1))
                    {
                        try
                        {
                            if (!plc.IsConnected)
                            {
                                plc.Open();
                            }
                            try
                            {
                                byte[] byte1 = plc.ReadBytes(DataType.DataBlock, 4, 0, 84);
                                uint[] result1 = DWord.ToArray(byte1);

                                for (int i = 0; i < 21; i++)
                                {
                                    if (i != 0)
                                    {
                                        var totalCountList = new List<long>();
                                        string newId = "107121374" + i;

                                        totalCountList = new List<long>() { (long)result1[i] };

                                        ReadMachineData data = new ReadMachineData
                                        {
                                            MachineID = Convert.ToInt64(newId),
                                            TotalCount = totalCountList,
                                            Break = false,
                                            Sched = false
                                        };

                                        ts.Add(data);
                                    }
                                }
                            }
                            catch (Exception)
                            {

                            }
                            plc.Close();

                        }
                        catch (Exception)
                        {
                            Console.WriteLine(plcIp);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                #endregion
                #region Sylhet_FirozVai
                try
                {
                    plcIp = "172.18.25.251";
                    using (var plc = new Plc(CpuType.S71200, plcIp, 0, 1))
                    {
                        try
                        {
                            if (!plc.IsConnected)
                            {
                                plc.Open();
                            }
                            try
                            {
                                byte[] byte1 = plc.ReadBytes(DataType.DataBlock, 1, 0, 204);
                                uint[] result1 = DWord.ToArray(byte1);

                                for (int i = 0; i < 51; i++)
                                {
                                    if (i != 0)
                                    {
                                        var totalCountList = new List<long>();
                                        string newId = "17218252511" + i;

                                        totalCountList = new List<long>() { (long)result1[i] };

                                        ReadMachineData data = new ReadMachineData
                                        {
                                            MachineID = Convert.ToInt64(newId),
                                            TotalCount = totalCountList,
                                            Break = false,
                                            Sched = false
                                        };

                                        ts.Add(data);
                                    }
                                }
                            }
                            catch (Exception)
                            {

                            }
                            plc.Close();

                        }
                        catch (Exception)
                        {
                            Console.WriteLine(plcIp);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                #endregion
                #region TEL_plastic_10.7.12.27
                try
                {
                    plcIp = "10.7.12.27";
                    using (var plc = new Plc(CpuType.S71200, plcIp, 0, 1))
                    {
                        try
                        {
                            if (!plc.IsConnected)
                            {
                                plc.Open();
                            }
                            try
                            {
                                byte[] byte1 = plc.ReadBytes(DataType.DataBlock, 11, 0, 400);
                                uint[] result1 = DWord.ToArray(byte1);

                                byte[] byte2 = plc.ReadBytes(DataType.DataBlock, 18, 0, 40);
                                uint[] result2 = DWord.ToArray(byte2);
                                byte[] byte3 = plc.ReadBytes(DataType.DataBlock, 15, 0, 40);
                                uint[] result3 = DWord.ToArray(byte3);

                                for (int i = 0; i < 100; i++)
                                {
                                    if (i != 0)
                                    {
                                        var totalCountList = new List<long>();
                                        string newId = "107122711" + i;

                                        totalCountList = new List<long>() { (long)result1[i] };

                                        ReadMachineData data = new ReadMachineData
                                        {
                                            MachineID = Convert.ToInt64(newId),
                                            TotalCount = totalCountList,
                                            Break = false,
                                            Sched = false
                                        };

                                        ts.Add(data);
                                    }
                                }

                                for (int i = 1; i < 10; i++)
                                {
                                    var totalCountList = new List<long>();
                                    string newId = "107122718" + i;

                                    totalCountList = new List<long>() { (long)result2[i], (long)result3[i] };

                                    ReadMachineData data = new ReadMachineData
                                    {
                                        MachineID = Convert.ToInt64(newId),
                                        TotalCount = totalCountList,
                                        Break = false,
                                        Sched = false
                                    };

                                    ts.Add(data);
                                }
                            }
                            catch (Exception)
                            {

                            }
                            plc.Close();

                        }
                        catch (Exception)
                        {
                            Console.WriteLine(plcIp);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                #endregion
                #region 192.168.193.30_Eggcoounter
                try
                {
                    plcIp = "192.168.193.30";
                    using (var plc = new Plc(CpuType.S71200, plcIp, 0, 1))
                    {
                        try
                        {
                            if (!plc.IsConnected)
                            {
                                plc.Open();
                            }
                            try
                            {
                                byte[] byte1 = plc.ReadBytes(DataType.DataBlock, 1, 0, 12);
                                uint[] result1 = DWord.ToArray(byte1);

                                for (int i = 0; i < 3; i++)
                                {
                                    if (i != 0)
                                    {
                                        var totalCountList = new List<long>();
                                        string newId = "193301" + i;

                                        totalCountList = new List<long>() { (long)result1[i] };

                                        ReadMachineData data = new ReadMachineData
                                        {
                                            MachineID = Convert.ToInt64(newId),
                                            TotalCount = totalCountList,
                                            Break = false,
                                            Sched = false
                                        };

                                        ts.Add(data);
                                    }
                                }
                            }
                            catch (Exception)
                            {

                            }
                            plc.Close();

                        }
                        catch (Exception)
                        {
                            Console.WriteLine(plcIp);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                #endregion
                #region HIP-2 BBML PU Lather
                try
                {
                    plcIp = "10.253.76.110";
                    using (var plc = new Plc(CpuType.S7200Smart, plcIp, 0, 1))
                    {
                        try
                        {
                            if (!plc.IsConnected)
                            {
                                plc.Open();
                            }
                            try
                            {
                                byte[] byte1 = plc.ReadBytes(DataType.DataBlock, 1, 0, 52);
                                uint[] result1 = DWord.ToArray(byte1);

                                for (int i = 0; i < 13; i++)
                                {
                                    if (i != 0)
                                    {
                                        var totalCountList = new List<long>();
                                        string newId = "10253761101" + i;

                                        totalCountList = new List<long>() { (long)result1[i] };

                                        ReadMachineData data = new ReadMachineData
                                        {
                                            MachineID = Convert.ToInt64(newId),
                                            TotalCount = totalCountList,
                                            Break = false,
                                            Sched = false
                                        };

                                        ts.Add(data);
                                    }
                                }
                            }
                            catch (Exception)
                            {

                            }
                            plc.Close();

                        }
                        catch (Exception)
                        {
                            Console.WriteLine(plcIp);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                #endregion
                #region BBML wiring
                try
                {
                    plcIp = "192.168.168.52";
                    using (var plc = new Plc(CpuType.S7200Smart, plcIp, 0, 1))
                    {
                        try
                        {
                            if (!plc.IsConnected)
                            {
                                plc.Open();
                            }
                            try
                            {
                                byte[] byte1 = plc.ReadBytes(DataType.DataBlock, 1, 0, 100);
                                uint[] result1 = DWord.ToArray(byte1);

                                for (int i = 0; i < 25; i++)
                                {
                                    if (i != 0)
                                    {
                                        var totalCountList = new List<long>();
                                        string newId = "16852001" + i;//00 added befor Datablock to avoid conflic

                                        totalCountList = new List<long>() { (long)result1[i] };

                                        ReadMachineData data = new ReadMachineData
                                        {
                                            MachineID = Convert.ToInt64(newId),
                                            TotalCount = totalCountList,
                                            Break = false,
                                            Sched = false
                                        };

                                        ts.Add(data);
                                    }
                                }
                            }
                            catch (Exception)
                            {

                            }
                            plc.Close();

                        }
                        catch (Exception)
                        {
                            Console.WriteLine(plc);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                #endregion

                #endregion

                #region Moveable Machine
                try
                {
                    var smv2 = new List<MoveableMachineProductionData>();
                    smv2 = db_SMV2.MoveableMachineProductionDatas.AsNoTracking().ToList();

                    if (smv2.Count > 0)
                    {
                        foreach (var item in smv2)
                        {
                            var totalCountList = new List<long>() { Convert.ToInt64(item.ProductionCount) };
                            ReadMachineData data = new ReadMachineData
                            {
                                MachineID = Convert.ToInt64(item.ManualId),
                                TotalCount = totalCountList,
                                Break = false,
                                Sched = false
                            };

                            ts.Add(data);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                #endregion
            }
            catch(Exception ex)
            {
                File.AppendAllText(fileName, "HIP Listener error in collecting data: \n" + System.DateTime.Now.ToString());
            }



            #region Send Data
            try
            {
                var count = 0;
                foreach (var item in ts)  //3335
                {
                    if (item.MachineID > 0)
                    {
                        //if (item.MachineID == 16850127548)
                        //{
                            try
                            {
                                string uri = "";
                                if (item.TotalCount.Count() == 3)
                                {
                                    string value1 = item.TotalCount[0].ToString();
                                    string value2 = item.TotalCount[1].ToString();
                                    string value3 = item.TotalCount[2].ToString();
                                    //uri = @"https://localhost:44344/api/ProductionAPI/machinedatawithweight?api-version=1&" + "MachineID=" + item.MachineID.ToString() + "&TotalCountList[0]=" + value1 + "&TotalCountList[1]=" + value2 + "&TotalCountList[2]=" + value3 + "&Break=false&Sched=false";
                                    uri = @"http://172.17.2.117:5003/api/ProductionAPI/machinedatawithweight?api-version=1&" + "MachineID=" + item.MachineID.ToString() + "&TotalCountList[0]=" + value1 + "&TotalCountList[1]=" + value2 + "&TotalCountList[2]=" + value3 + "&Break=false&Sched=false";

                                }
                                else if (item.TotalCount.Count() == 2)
                                {
                                    string value1 = item.TotalCount[0].ToString();
                                    string value2 = item.TotalCount[1].ToString();
                                    //uri = @"https://localhost:44344/api/ProductionAPI/machinedatawithruntime?api-version=1&" + "MachineID=" + item.MachineID.ToString() + "&TotalCountList[0]=" + value1 + "&TotalCountList[1]=" + value2 + "&Break=false&Sched=false";
                                    uri = @"http://172.17.2.117:5003/api/ProductionAPI/machinedatawithruntime?api-version=1&" + "MachineID=" + item.MachineID.ToString() + "&TotalCountList[0]=" + value1 + "&TotalCountList[1]=" + value2 + "&Break=false&Sched=false";

                                }
                                else
                                {
                                    //uri = @"https://localhost:44344/api/ProductionAPI/machinedata?api-version=1&" + "MachineID=" + item.MachineID.ToString() + "&TotalCountList[0]=" + item.TotalCount[0].ToString() + "&Break=false&Sched=false";
                                    uri = @"http://172.17.2.117:5003/api/ProductionAPI/machinedata?api-version=1&" + "MachineID=" + item.MachineID.ToString() + "&TotalCountList[0]=" + item.TotalCount[0].ToString() + "&Break=false&Sched=false";
                                }
                                var client = new RestClient(uri);
                                client.Timeout = -1;
                                var request = new RestRequest(Method.GET);
                                //var request = new RestRequest(Method.POST);
                                //IRestResponse response = await client.ExecuteAsync(request);
                                IRestResponse response = client.Execute(request);
                            }
                            catch (Exception ex)
                            {
                                File.AppendAllText(fileName, "HIP Listener error in calling api: \n" + System.DateTime.Now.ToString());
                            }
                        //}
                    }

                    count += 1;
                    int percent = ((count * 100) / ts.Count);
                    progress.Report(percent);

                    if (token.IsCancellationRequested)
                    {
                        token.ThrowIfCancellationRequested();
                    }
                }
            }
            catch (Exception ex)
            {
                File.AppendAllText(fileName, "\nHIP Listener error in sending data: " + System.DateTime.Now.ToString());
            }
            #endregion
            return "ok";
        }
        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
