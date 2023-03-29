using GalaSoft.MvvmLight;
using System;
using System.Windows;

namespace Xp.Resin.Print.Models
{
    /// <summary>
    /// MainView模型
    /// </summary>
    public class BaseModel : ObservableObject
    {
        /// <summary>
        /// 是否维护模式？
        /// </summary>
        private bool isMaintenanceMode = false;
        public bool IsMaintenanceMode
        {
            get { return isMaintenanceMode; }
            set
            {
                if (value)
                {
                    MaintenanceBtnContent = "退出维护";
                    VisibilityModel = Visibility.Visible;
                }
                else
                {
                    MaintenanceBtnContent = "打印维护";
                    VisibilityModel = Visibility.Hidden;
                }

                isMaintenanceMode = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 打印维护按钮显示文本
        /// </summary>
        private string maintenanceBtnContent = "打印维护";
        public string MaintenanceBtnContent
        {
            get { return maintenanceBtnContent; }
            set
            {
                maintenanceBtnContent = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 元素可见性
        /// </summary>
        private Visibility visibilityModel = Visibility.Hidden;
        public Visibility VisibilityModel
        {
            get { return visibilityModel; }
            set
            {
                visibilityModel = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 打印任务数量
        /// </summary>
        private int printTaskCount;
        public int PrintTaskCount
        {
            get { return printTaskCount; }
            set { printTaskCount = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 打印任务提示
        /// </summary>
        private string printTaskTips;
        public string PrintTaskTips
        {
            get { return printTaskTips; }
            set { printTaskTips = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 车型
        /// </summary>
        private string carType = "";
        public string CarType
        {
            get { return carType; }
            set { carType = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 物料编码
        /// </summary>
        private string materialNo = "";
        public string MaterialNo
        {
            get { return materialNo; }
            set { materialNo = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 物料名称
        /// </summary>
        private string materialName = "";
        public string MaterialName
        {
            get { return materialName; }
            set { materialName = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 供应商编码
        /// </summary>
        private string supplierNo = "";
        public string SupplierNo
        {
            get { return supplierNo; }
            set { supplierNo = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 供应商名称
        /// </summary>
        private string supplierName = "";
        public string SupplierName
        {
            get { return supplierName; }
            set { supplierName = value; RaisePropertyChanged(); }
        }
    }

    public class PageModel: ObservableObject
    {
        /// <summary>
        /// 最大页面数
        /// </summary>
        private int maxPageCount = 1;

        public int MaxPageCount
        {
            get { return maxPageCount; }
            set
            {
                maxPageCount = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 当前页数
        /// </summary>
        private int pageIndex = 1;

        public int PageIndex
        {
            get { return pageIndex; }
            set
            {
                pageIndex = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 分页大小
        /// </summary>
        private int dataCountPerPage = 20;

        public int DataCountPerPage
        {
            get { return dataCountPerPage; }
            set
            {
                dataCountPerPage = value;
                RaisePropertyChanged();
            }
        }
    }

    public class ProductModel
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 行号
        /// </summary>
        public long RowNum { get; set; }

        /// <summary>
        /// 车型
        /// </summary>
        public string CarType { get; set; }

        /// <summary>
        /// 物料编码
        /// </summary>
        public string MaterialNo { get; set; }

        /// <summary>
        /// 物料名称
        /// </summary>
        public string MaterialName { get; set; }

        /// <summary>
        /// 供应商编码
        /// </summary>
        public string SupplierNo { get; set; }

        /// <summary>
        /// 供应商名称
        /// </summary>
        public string SupplierName { get; set; }

        /// <summary>
        /// 专用打印机
        /// </summary>
        public string SpecialPrinter { get; set; }

        /// <summary>
        /// 附加参数
        /// </summary>
        public string ExtraParams { get; set; }

        /// <summary>
        /// 最后打印时间
        /// </summary>
        public DateTime? LastPrintDt { get; set; }

        /// <summary>
        /// 是否删除
        /// </summary>
        public bool IsDeleted { set; get; } = false;

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateDt { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime? UpdateDt { get; set; }
    }

    public class QueueModel
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 行号
        /// </summary>
        public long RowNum { get; set; }

        /// <summary>
        /// 打印机
        /// </summary>
        public string Printer { get; set; }

        /// <summary>
        /// 打印份数
        /// </summary>
        public int Copies { get; set; }

        /// <summary>
        /// 打印参数
        /// </summary>
        public string PrintParams { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public bool Status { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateDt { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime? UpdateDt { get; set; }
    }

    /// <summary>
    /// 打印参数
    /// </summary>
    public class PrintParams
    {
        /// <summary>
        /// 打印机
        /// </summary>
        public string Printer { get; set; }

        /// <summary>
        /// 打印份数
        /// </summary>
        public int Copies { get; set; } = 1;

        public string CarType { get; set; }
        public string MaterialNo { get; set; }
        public string MaterialName { get; set; }
        public string SupplierName { get; set; }
        public DateTime ProductDate { get; set; } = DateTime.Now;
    }

    public class ConfigModel
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 分类
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// 键
        /// </summary>
        public string ConfigKey { get; set; }

        /// <summary>
        /// 值
        /// </summary>
        public string ConfigValue { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
    }
}
