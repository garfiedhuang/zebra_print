using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Xp.Resin.Print.Models;
using Xp.Resin.Print.Common;
using Xp.Resin.Print.Views.SystemMgt.Product;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading;
using System.Windows;
using HandyControl.Controls;
using MessageBox = HandyControl.Controls.MessageBox;
using HandyControl.Data;
using System.Drawing;
using System.Globalization;

namespace Xp.Resin.Print.ViewsModels
{
    public class MainViewModel : ViewModelBase
    {
        private static string _msg = "";
        private static Dictionary<string, bool> _dicPrinters;
        private static List<ConfigModel> _configModels;

        private static CancellationTokenSource tokenSource = new CancellationTokenSource();
        private static CancellationToken token = tokenSource.Token;

        /// <summary>
        /// ���캯��
        /// </summary>
        public MainViewModel()
        {
            try
            {
                BaseModel = new BaseModel();
                PageModel = new PageModel();
                GridData = new ObservableCollection<ProductModel>();
                ProductModels = new ObservableCollection<ProductModel>();
                QueueModels = new ObservableCollection<QueueModel>();

                this.PrintTestCmd = new RelayCommand(this.PrintTest);
                this.AddCmd = new RelayCommand(this.Add);
                this.QueryCmd = new RelayCommand(this.Query);
                this.ResetCmd = new RelayCommand(this.Reset);
                this.EditCmd = new RelayCommand<long>(this.Edit);
                this.DeleteCmd = new RelayCommand<long>(this.Delete);
                this.PrintCmd = new RelayCommand<long>(this.Print);

                this.PageUpdatedCommand = new RelayCommand<FunctionEventArgs<int>>(PageUpdated);

                //������ݿ��ļ�
                OleDbHelper.CheckDbFile();

                //��������
                this.GetConfigs();

                //��ʼ��ҳ������
                this.Query();

                //��ȡ���ش�ӡ��
                _dicPrinters = PrinterHelper.GetPrinters();

                #region �첽�߳�

                var enableExecutePrintJob = Convert.ToInt32(_configModels.FirstOrDefault(w => w.Category == "SYSTEM" && w.ConfigKey == "ENABLE_EXECUTE_PRINT_JOB")?.ConfigValue ?? "1");
                var enableClearPrintLogJob = Convert.ToInt32(_configModels.FirstOrDefault(w => w.Category == "SYSTEM" && w.ConfigKey == "ENABLE_CLEAR_PRINT_LOG_JOB")?.ConfigValue ?? "1");

                //ִ�д�ӡ�߳�
                if (enableExecutePrintJob == 1)
                {
                    _ = Task.Factory.StartNew(() =>
                    {
                        DoExecutePrint();
                    }, token);
                }

                //�����ӡ��־�߳�
                if (enableClearPrintLogJob == 1)
                {
                    _ = Task.Factory.StartNew(() =>
                    {
                        DoClearPrintLog();
                    }, token);
                }

                #endregion �첽�߳�
            }
            catch (Exception ex)
            {
                LogHelper.Error($"��ʼ��ʧ�ܣ�{ex?.Message}");
            }
        }

        /// <summary>
        /// ��������
        /// </summary>
        ~MainViewModel()
        {
            tokenSource.Cancel();
        }

        #region =====data

        /// <summary>
        /// ��ѯ
        /// </summary>
        private BaseModel baseModel;

        public BaseModel BaseModel
        {
            get { return baseModel; }
            set { baseModel = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// ��ҳ
        /// </summary>
        private PageModel pageModel;

        public PageModel PageModel
        {
            get { return pageModel; }
            set { pageModel = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// �������ݼ���
        /// </summary>
        private ObservableCollection<ProductModel> gridData;

        public ObservableCollection<ProductModel> GridData
        {
            get { return gridData; }
            set { gridData = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// ��Ʒ
        /// </summary>
        private ObservableCollection<ProductModel> productModels;

        public ObservableCollection<ProductModel> ProductModels
        {
            get { return productModels; }
            set { productModels = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// ����
        /// </summary>
        private ObservableCollection<QueueModel> queueModels;

        public ObservableCollection<QueueModel> QueueModels
        {
            get { return queueModels; }
            set { queueModels = value; RaisePropertyChanged(); }
        }

        #endregion =====data

        #region ====cmd

        public RelayCommand PrintTestCmd { get; set; }

        public RelayCommand AddCmd { get; set; }
        public RelayCommand QueryCmd { get; set; }
        public RelayCommand ResetCmd { get; set; }
        public RelayCommand<long> EditCmd { get; set; }
        public RelayCommand<long> DeleteCmd { get; set; }
        public RelayCommand<long> PrintCmd { get; set; }

        /// <summary>
        /// ��ҳ
        /// </summary>
        public RelayCommand<FunctionEventArgs<int>> PageUpdatedCommand { get; set; }

        #endregion ====cmd

        #region =====implement

        private void GetConfigs()
        {
            try
            {
                if (_configModels == null)
                {
                    _configModels = new List<ConfigModel>();
                }

                var sql = $"SELECT * FROM BASE_CONFIG WHERE 1=1 AND IS_DELETED=0";

                using (var dt = OleDbHelper.DataTable(sql))
                {
                    if (dt == null || dt.Rows.Count == 0)
                    {
                        return;
                    }

                    foreach (DataRow dr in dt.Rows)
                    {
                        _configModels.Add(new ConfigModel()
                        {
                            Id = Convert.ToInt32(dr["ID"]),
                            Category = dr["CATEGORY"].ToString(),
                            ConfigKey = dr["CONFIG_KEY"].ToString(),
                            ConfigValue = dr["CONFIG_VALUE"].ToString()
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                _msg = $"��ѯ����ʧ�ܣ�{ex?.Message}";

                Growl.Error(_msg);
                LogHelper.Error(_msg);
            }
        }

        /// <summary>
        /// ��ӡ����
        /// </summary>
        private void PrintTest()
        {
            try
            {
                //δʹ��ר�ô�ӡ������£�ʹ�ñ���Ĭ�ϴ�ӡ����ӡ
                var printer = _dicPrinters.FirstOrDefault(w => w.Value == true).Key;

                if (string.IsNullOrEmpty(printer))
                {
                    _msg = $"�޷���ȡ���ش�ӡ�����ƣ�����ϵIT����Ա��";
                    throw new Exception(_msg);
                }

                TestView view = new TestView(printer);
                view.Owner = Application.Current.MainWindow;

                var r = view.ShowDialog();
                if (r.Value)
                {
                    ////δʹ��ר�ô�ӡ������£�ʹ�ñ���Ĭ�ϴ�ӡ����ӡ
                    //var printer =_dicPrinters.FirstOrDefault(w => w.Value == true).Key;

                    //if (string.IsNullOrEmpty(printer))
                    //{
                    //    _msg = $"�޷���ȡ���ش�ӡ�����ƣ�����ϵIT����Ա��";
                    //    throw new Exception(_msg);
                    //}

                    //RawPrinterHelper.SendStringToPrinter(printer, model.PrintCommand);

                    //Growl.Info("���ͳɹ���");
                }
            }
            catch (Exception ex)
            {
                _msg = $"����ʧ�ܣ�{ex?.Message}";

                Growl.Error(_msg);
                LogHelper.Error(_msg);
            }
        }

        /// <summary>
        /// ����
        /// </summary>
        private void Add()
        {
            try
            {
                ProductModel model = new ProductModel();
                EditView view = new EditView(model);
                view.Owner = Application.Current.MainWindow;

                var r = view.ShowDialog();
                if (r.Value)
                {
                    var sql = $@"INSERT INTO BASE_PRODUCT(CAR_TYPE,MATERIAL_NO,MATERIAL_NAME,SUPPLIER_NO,SUPPLIER_NAME,CREATE_DT,UPDATE_DT)
VALUES('{model.CarType}','{model.MaterialNo}','{model.MaterialName}','{model.SupplierNo}','{model.SupplierName}',NOW(),NOW())";

                    OleDbHelper.ExecuteSql(sql);

                    this.Query();

                    Growl.Info("����ɹ���");
                }
            }
            catch (Exception ex)
            {
                _msg = $"����ʧ�ܣ�{ex?.Message}";

                Growl.Error(_msg);
                LogHelper.Error(_msg);
            }
        }

        /// <summary>
        /// ��ѯ
        /// </summary>
        public void Query()
        {
            try
            {
                var conditions = new StringBuilder();

                if (!string.IsNullOrEmpty(BaseModel.CarType.Trim()))
                {
                    conditions.Append($" AND CAR_TYPE LIKE '%{BaseModel.CarType}%'");
                }

                if (!string.IsNullOrEmpty(BaseModel.MaterialNo.Trim()))
                {
                    conditions.Append($" AND MATERIAL_NO LIKE '%{BaseModel.MaterialNo}%'");
                }

                if (!string.IsNullOrEmpty(BaseModel.MaterialName.Trim()))
                {
                    conditions.Append($" AND MATERIAL_NAME LIKE '%{BaseModel.MaterialName}%'");
                }

                if (!string.IsNullOrEmpty(BaseModel.SupplierNo.Trim()))
                {
                    conditions.Append($" AND SUPPLIER_NO LIKE '%{BaseModel.SupplierNo}%'");
                }

                if (!string.IsNullOrEmpty(BaseModel.SupplierName.Trim()))
                {
                    conditions.Append($" AND SUPPLIER_NAME LIKE '%{BaseModel.SupplierName}%'");
                }

                var sql = $"SELECT * FROM BASE_PRODUCT WHERE 1=1 AND IS_DELETED=0{conditions} ORDER BY LAST_PRINT_DT DESC";

                using (var dt = OleDbHelper.DataTable(sql))
                {
                    ProductModels.Clear();
                    GridData.Clear();

                    if (dt == null || dt.Rows.Count == 0)
                    {
                        return;
                    }

                    var rowNum = 1;//�к�

                    foreach (DataRow dr in dt.Rows)
                    {
                        ProductModels.Add(new ProductModel()
                        {
                            Id = Convert.ToInt32(dr["ID"]),
                            RowNum = rowNum,
                            CarType = dr["CAR_TYPE"].ToString(),
                            MaterialNo = dr["MATERIAL_NO"].ToString(),
                            MaterialName = dr["MATERIAL_NAME"].ToString(),
                            SupplierNo = dr["SUPPLIER_NO"].ToString(),
                            SupplierName = dr["SUPPLIER_NAME"].ToString(),
                            SpecialPrinter = dr["SPECIAL_PRINTER"].ToString(),
                            ExtraParams = dr["EXTRA_PARAMS"].ToString(),
                            IsDeleted = Convert.ToBoolean(dr["IS_DELETED"]),
                            LastPrintDt = GetDateTime(dr["LAST_PRINT_DT"]),
                            CreateDt = GetDateTime(dr["CREATE_DT"]),
                            UpdateDt = GetDateTime(dr["UPDATE_DT"])
                        });

                        rowNum++;
                    }
                }

                //��ǰҳ��
                PageModel.PageIndex = ProductModels.Count > 0 ? 1 : 0;
                PageModel.MaxPageCount = 0;

                //���ҳ��
                PageModel.MaxPageCount = PageModel.PageIndex > 0 ? (int)Math.Ceiling((decimal)ProductModels.Count / PageModel.DataCountPerPage) : 0;

                //���ݷ�ҳ
                Paging(PageModel.PageIndex);
            }
            catch (Exception ex)
            {
                _msg = $"��ѯʧ�ܣ�{ex?.Message}";

                Growl.Error(_msg);
                LogHelper.Error(_msg);
            }
        }

        private DateTime? GetDateTime(object obj)
        {
            if (obj is DBNull)
            {
                return null;
            }
            else
            {
                return Convert.ToDateTime(obj);
            }
        }

        /// <summary>
        /// ����
        /// </summary>
        private void Reset()
        {
            this.BaseModel.CarType = string.Empty;
            this.BaseModel.MaterialNo = string.Empty;
            this.BaseModel.MaterialName = string.Empty;
            this.BaseModel.SupplierNo = string.Empty;
            this.BaseModel.SupplierName = string.Empty;
        }

        /// <summary>
        /// �༭
        /// </summary>
        /// <param name="obj"></param>
        private void Edit(long obj)
        {
            try
            {
                var sql = $"SELECT * FROM BASE_PRODUCT WHERE IS_DELETED=0 AND ID={obj}";

                using (var data = OleDbHelper.DataTable(sql))
                {
                    if (data == null || data.Rows.Count == 0)
                    {
                        Growl.Warning($"���ݿⲻ���� ����ID={obj} �ļ�¼");
                        return;
                    }

                    var dr = data.Rows[0];
                    var model = new ProductModel()
                    {
                        Id = Convert.ToInt32(dr["ID"]),
                        CarType = dr["CAR_TYPE"].ToString(),
                        MaterialNo = dr["MATERIAL_NO"].ToString(),
                        MaterialName = dr["MATERIAL_NAME"].ToString(),
                        SupplierNo = dr["SUPPLIER_NO"].ToString(),
                        SupplierName = dr["SUPPLIER_NAME"].ToString(),
                        SpecialPrinter = dr["SPECIAL_PRINTER"].ToString(),
                        ExtraParams = dr["EXTRA_PARAMS"].ToString(),
                        IsDeleted = Convert.ToBoolean(dr["IS_DELETED"]),
                        LastPrintDt = Convert.ToDateTime(dr["LAST_PRINT_DT"] is System.DBNull ? DateTime.MinValue : dr["LAST_PRINT_DT"]),
                        CreateDt = Convert.ToDateTime(dr["CREATE_DT"] is System.DBNull ? DateTime.MinValue : dr["CREATE_DT"]),
                        UpdateDt = Convert.ToDateTime(dr["UPDATE_DT"] is System.DBNull ? DateTime.MinValue : dr["UPDATE_DT"])
                    };

                    if (model != null)
                    {
                        EditView view = new EditView(model);
                        var r = view.ShowDialog();
                        if (r.Value)
                        {
                            sql = $"UPDATE BASE_PRODUCT SET MATERIAL_NO='{model.MaterialNo}',MATERIAL_NAME='{model.MaterialName}',SUPPLIER_NO='{model.SupplierNo}',SUPPLIER_NAME='{model.SupplierName}',CAR_TYPE='{model.CarType}',UPDATE_DT=NOW() WHERE ID={obj}";

                            OleDbHelper.ExecuteSql(sql);

                            this.Query();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _msg = $"����ʧ�ܣ�{ex?.Message}";

                Growl.Error(_msg);
                LogHelper.Error(_msg);
            }
        }

        /// <summary>
        /// ɾ��
        /// </summary>
        /// <param name="obj"></param>
        private void Delete(long obj)
        {
            try
            {
                var r = MessageBox.Show($"ȷ��Ҫɾ����¼��", "��ʾ", MessageBoxButton.YesNo);
                if (r == MessageBoxResult.Yes)
                {
                    var sql = $"UPDATE BASE_PRODUCT SET IS_DELETED=1 WHERE ID={obj}";

                    OleDbHelper.ExecuteSql(sql);

                    this.Query();
                }
            }
            catch (Exception ex)
            {
                _msg = $"����ʧ�ܣ�{ex?.Message}";

                Growl.Error(_msg);
                LogHelper.Error(_msg);
            }
        }

        /// <summary>
        /// ��ӡ
        /// </summary>
        /// <param name="obj"></param>
        private void Print(long obj)
        {
            try
            {
                var printParams = new PrintParams();
                var view = new PrintView(printParams);

                view.Owner = Application.Current.MainWindow;
                var r = view.ShowDialog();

                if (r.Value)
                {
                    var sql = $"SELECT * FROM BASE_PRODUCT WHERE IS_DELETED=0 AND ID={obj}";

                    using (var data = OleDbHelper.DataTable(sql))
                    {
                        if (data == null || data.Rows.Count == 0)
                        {
                            _msg = $"���ݿⲻ���� ����ID={obj} �ļ�¼";
                            throw new Exception(_msg);
                        }

                        var dr = data.Rows[0];
                        var model = new ProductModel()
                        {
                            Id = Convert.ToInt32(dr["ID"]),
                            CarType = dr["CAR_TYPE"].ToString(),
                            MaterialNo = dr["MATERIAL_NO"].ToString(),
                            MaterialName = dr["MATERIAL_NAME"].ToString(),
                            SupplierNo = dr["SUPPLIER_NO"].ToString(),
                            SupplierName = dr["SUPPLIER_NAME"].ToString(),
                            SpecialPrinter = dr["SPECIAL_PRINTER"].ToString(),
                            ExtraParams = dr["EXTRA_PARAMS"].ToString(),
                            IsDeleted = Convert.ToBoolean(dr["IS_DELETED"]),
                            LastPrintDt = GetDateTime(dr["LAST_PRINT_DT"]),
                            CreateDt = GetDateTime(dr["CREATE_DT"]),
                            UpdateDt = GetDateTime(dr["UPDATE_DT"])
                        };

                        //δʹ��ר�ô�ӡ������£�ʹ�ñ���Ĭ�ϴ�ӡ����ӡ
                        var printer = string.Empty;
                        if (!string.IsNullOrEmpty(model.SpecialPrinter))
                        {
                            printer = model.SpecialPrinter;
                        }
                        else
                        {
                            printer = _dicPrinters.FirstOrDefault(w => w.Value == true).Key;
                        }

                        if (string.IsNullOrEmpty(printer))
                        {
                            _msg = $"�޷���ȡ���ش�ӡ�����ƣ�����ϵIT����Ա��";
                            throw new Exception(_msg);
                        }

                        //��ӡ����
                        printParams.Printer = printer;
                        printParams.CarType = model.CarType;
                        printParams.MaterialNo = model.MaterialNo;
                        printParams.MaterialName = model.MaterialName;
                        printParams.SupplierName = model.SupplierName;

                        var json = Newtonsoft.Json.JsonConvert.SerializeObject(printParams);

                        sql = $@"INSERT INTO TS_PRINT_QUEUE(PRINTER,COPIES,PRINT_PARAMS,CREATE_DT,UPDATE_DT) VALUES('{printParams.Printer}','{printParams.Copies}','{json}',NOW(),NOW())";

                        OleDbHelper.ExecuteSql(sql);

                        this.Query();

                        this.SetPrintTips(1);

                        Growl.Info("��ӡ���񱣴�ɹ���");
                    }
                }
            }
            catch (Exception ex)
            {
                _msg = $"��ӡ���񱣴�ʧ�ܣ�{ex?.Message}";

                Growl.Error(_msg);
                LogHelper.Error(_msg);
            }
        }

        private void SetPrintTips(int v)
        {
            if (BaseModel.PrintTaskCount < 0)
            {
                BaseModel.PrintTaskCount = 0;
            }
            else
            {
                BaseModel.PrintTaskCount += v;//��ӡ����������
            }

            if (BaseModel.PrintTaskCount == 0)
            {
                BaseModel.PrintTaskTips = string.Empty;
            }
            else
            {
                BaseModel.PrintTaskTips = $"����{BaseModel.PrintTaskCount}����ӡ��";//���õ�ǰ��ӡ��������
            }
        }

        /// <summary>
        /// ��ҳ
        /// </summary>
        /// <param name="obj"></param>
        private void PageUpdated(FunctionEventArgs<int> obj)
        {
            Paging(obj.Info);
        }

        /// <summary>
        /// ��ҳ
        /// </summary>
        /// <param name="pageIndex"></param>
        private void Paging(int pageIndex = 0)
        {
            if (pageIndex == -1)//��һ�β�ѯ�������ò�ѯ
            {
                //��ǰҳ��
                PageModel.PageIndex = ProductModels.Count > 0 ? 1 : 0;
                PageModel.MaxPageCount = 0;

                pageIndex = PageModel.PageIndex;
            }

            //���ҳ��
            PageModel.MaxPageCount = PageModel.PageIndex > 0 ? (int)Math.Ceiling((decimal)ProductModels.Count / PageModel.DataCountPerPage) : 0;

            //�����������
            GridData.Clear();

            //���ݷ�ҳ
            var pagedData = ProductModels.Skip((pageIndex - 1) * PageModel.DataCountPerPage).Take(PageModel.DataCountPerPage).ToList();

            if (pagedData.Count > 0)
            {
                pagedData.ForEach(item =>
                {
                    GridData.Add(item);
                });
            }
        }

        #endregion =====implement

        #region threads

        private void DoExecutePrint()
        {
            var sql = "SELECT * FROM TS_PRINT_QUEUE WHERE PRINT_DT IS NULL ORDER BY CREATE_DT ASC";

            var msg = string.Empty;
            var printParams = new PrintParams();
            var zebraCommand = string.Empty;
            var printResult = false;
            var jobInterval = Convert.ToInt32(_configModels.FirstOrDefault(w => w.Category == "SYSTEM" && w.ConfigKey == "EXECUTE_PRINT_JOB_INTERVAL")?.ConfigValue ?? "3");

            while (true)
            {
                try
                {
                    if (token.IsCancellationRequested)
                    {
                        break;//ȡ��Taskִ��
                    }

                    using (var data = OleDbHelper.DataTable(sql))
                    {
                        if (data == null || data.Rows.Count == 0)
                        {
                            continue;
                        }
                        foreach (DataRow dr in data.Rows)
                        {
                            try
                            {
                                printResult = false;

                                //��ӡ����
                                printParams = Newtonsoft.Json.JsonConvert.DeserializeObject<PrintParams>(dr["PRINT_PARAMS"].ToString());

                                //��ӡָ��
                                zebraCommand = _configModels.FirstOrDefault(w => w.Category == "COMMAND" && w.ConfigKey == printParams.Printer)?.ConfigValue;

                                if (string.IsNullOrEmpty(zebraCommand))
                                {
                                    throw new Exception($"��ӡ����{printParams.Printer}��δ���ô�ӡָ�����ϵIT����Ա��");
                                }

                                //��ʽ����ӡָ��
                                zebraCommand = zebraCommand.Replace("#qrMaterialNo#", printParams.MaterialNo)
                                    .Replace("#txtCarType#", printParams.CarType)
                                    .Replace("#txtMaterialName#", printParams.MaterialName)
                                    .Replace("#txtSupplier#", printParams.SupplierName)
                                    .Replace("#txtProductDate#", printParams.ProductDate.ToString("yyyy/MM/dd", DateTimeFormatInfo.InvariantInfo))
                                    .Replace("#copies#", printParams.Copies.ToString());

                                var temp1 = ZebraPrintHelper.ConvertChineseToHex(printParams.MaterialName, "tempName1");
                                var temp2 = ZebraPrintHelper.ConvertChineseToHex(printParams.SupplierName, "tempName2");

                                zebraCommand = zebraCommand.Replace("{tempName1}", temp1).Replace("{tempName2}", temp2);

                                //���Ͱ����ӡ��
                                //printResult = RawPrinterHelper.SendStringToPrinter(printParams.Printer, zebraCommand);//Todo:20230329 by garfield

                                msg = $"��ӡ��{printParams.CarType}|{printParams.MaterialNo}|{printParams.MaterialName}��";

                                if (printResult)
                                {
                                    msg = $"{msg}������ͳɹ���";
                                    Growl.Info(msg);
                                    LogHelper.Info($"����ͳɹ�����ӡ������{dr["PRINT_PARAMS"]}");
                                }
                                else
                                {
                                    msg = $"{msg}�������ʧ�ܣ�";
                                    Growl.Error(msg);
                                    LogHelper.Error(msg);
                                }
                            }
                            catch (Exception ex)
                            {
                                msg = $"���л���ӡ��������ָ���������ӡ��ʧ�ܣ�{ex?.Message}";

                                Growl.Error(msg);
                                LogHelper.Error(msg);
                            }
                            finally
                            {
                                //���´�ӡ״̬
                                try
                                {
                                    var sqls = new ArrayList();

                                    sqls.Add($"UPDATE TS_PRINT_QUEUE SET STATUS='{(printResult ? 1 : 0)}',REMARK='{msg}',PRINT_DT=NOW(),UPDATE_DT=NOW() WHERE ID={dr["ID"]}");
                                    sqls.Add($"UPDATE BASE_PRODUCT SET LAST_PRINT_DT=NOW(),UPDATE_DT=NOW() WHERE CAR_TYPE='{printParams.CarType}' AND MATERIAL_NO='{printParams.MaterialNo}' AND IS_DELETED=0");

                                    OleDbHelper.ExecuteSqlTran(sqls);

                                    //ˢ�½�������״̬
                                    System.Windows.Application.Current.Dispatcher.Invoke(() =>
                                    {
                                        this.Query();
                                    });
                                }
                                catch (Exception ex)
                                {
                                    msg = $"���´�ӡ״̬��{ex?.Message}";

                                    Growl.Error(msg);
                                    LogHelper.Error(msg);
                                }

                                this.SetPrintTips(-1);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    msg = $"ִ�д�ӡJOBʧ�ܣ�{ex?.Message}";

                    Growl.Error(msg);
                    LogHelper.Error(msg);
                }
                finally
                {
                    Thread.Sleep(jobInterval * 1000);//Ĭ������3s
                }
            }
        }

        private void DoClearPrintLog()
        {
            var jobInterval = Convert.ToInt32(_configModels.FirstOrDefault(w => w.Category == "SYSTEM" && w.ConfigKey == "CLEAR_PRINT_LOG_JOB_INTERVAL")?.ConfigValue ?? "600");
            var dataStoragePeriod = Convert.ToInt32(_configModels.FirstOrDefault(w => w.Category == "SYSTEM" && w.ConfigKey == "DATA_STORAGE_PERIOD")?.ConfigValue ?? "6");

            var sql0 = "SELECT * FROM TS_PRINT_QUEUE WHERE PRINT_DT IS NOT NULL";

            var sql1 = @"INSERT INTO TS_PRINT_QUEUE_HIS(PRINT_QUEUE_ID,PRINTER,COPIES,PRINT_PARAMS,STATUS,PRINT_DT,REMARK,CREATE_DT,UPDATE_DT)
            SELECT ID,PRINTER,COPIES,PRINT_PARAMS,STATUS,PRINT_DT,REMARK,CREATE_DT,UPDATE_DT FROM TS_PRINT_QUEUE WHERE PRINT_DT IS NOT NULL AND ID IN ({0})";

            var sql2 = "DELETE FROM TS_PRINT_QUEUE WHERE PRINT_DT IS NOT NULL AND ID IN ({0})";

            var sql3 = "DELETE FROM TS_PRINT_QUEUE_HIS WHERE PRINT_DT <=#{0}#";

            var sqls = new ArrayList();

            while (true)
            {
                try
                {
                    if (token.IsCancellationRequested)
                    {
                        break;//ȡ��Taskִ��
                    }

                    using (var data = OleDbHelper.DataTable(sql0))
                    {
                        if (data == null || data.Rows.Count == 0)
                        {
                            continue;
                        }

                        sqls.Clear();

                        var ids = string.Join(",", data.AsEnumerable().Select(w => w.Field<int>("ID")).ToList());

                        sqls.Add(string.Format(sql1, ids));
                        sqls.Add(string.Format(sql2, ids));
                        sqls.Add(string.Format(sql3, DateTime.Now.AddMonths(-dataStoragePeriod)));

                        OleDbHelper.ExecuteSqlTran(sqls);

                        LogHelper.Info($"�����ӡ��־ִ�гɹ���Ǩ������ID��{ids}");
                    }
                }
                catch (Exception ex)
                {
                    var msg = $"ִ�������ӡ��־JOBʧ�ܣ�{ex?.Message}";

                    Growl.Error(msg);
                    LogHelper.Error(msg);
                }
                finally
                {
                    Thread.Sleep(jobInterval * 1000);//Ĭ������10����
                }
            }
        }

        #endregion threads
    }
}