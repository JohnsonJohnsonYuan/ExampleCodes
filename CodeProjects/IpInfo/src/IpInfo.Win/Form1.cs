using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using IpInfo.Core;
using IpInfo.Core.Extensions;
using IpInfo.Core.Models;
using IpInfo.Core.Services;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace IpInfo.Win
{
    public partial class IpInfo : Form
    {
        private OpenFileDialog _openDialog = new OpenFileDialog();
        private SaveFileDialog _saveDialog = new SaveFileDialog();
        private Regex _ipRegex = new Regex(@"((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)", RegexOptions.Singleline | RegexOptions.Compiled);

        private IpRecordService _ipRecordService = new IpRecordService();
        private IpRecord[] _ipRecords = null;

        public IpInfo()
        {
            InitializeComponent();

            LoadOption();

            string filter = "Text Files|*.txt";
            _openDialog.Filter = filter;
            _saveDialog.Filter = filter;
        }

        private void LoadOption()
        {
            tbxSearch.Enabled = false;
            exportToolStripMenuItem.Enabled = false;
            btnSearch.Enabled = false;
            lblContent.Text = string.Empty;
        }

        private void LoadIpRecords(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentNullException("filePath");
            if (!File.Exists(filePath))
                throw new FileNotFoundException(string.Empty, filePath);

            Task.Factory.StartNew(() =>
            {
                lblStatus.Text = "读取中...";

                // load data
                var ipRecords = _ipRecordService.LoadIpRecord(filePath, "data\\dict.txt", "data\\areacode.txt", "data\\specialDic.txt");

                lblStatus.Text = "处理中...";
                _ipRecords = _ipRecordService.MergeIpRecord(ipRecords);

                lblStatus.Text = string.Format(Messages.TotalRecordCount, ipRecords.Length);
            }).Wait();

            exportToolStripMenuItem.Enabled = true;
            tbxSearch.Enabled = true;
        }

        /// <summary>
        ///  Import
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void importToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var dlgResult = _openDialog.ShowDialog();
            if (dlgResult == System.Windows.Forms.DialogResult.OK)
            {
                var filePath = _openDialog.FileName;

                LoadIpRecords(filePath);

                var autoSavePath = Path.Combine(Environment.CurrentDirectory, "output", Path.GetFileName(filePath));
                SaveAs(GenerateSaveFilePath(autoSavePath));

                this.Text = Path.GetFileName(filePath) + " - " + this.Text;
            }
        }

        /// <summary>
        /// Export
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveAs();
        }

        private string GenerateSaveFilePath(string filePath)
        {
            if (!Directory.Exists(Path.GetDirectoryName(filePath)))
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));

            if (!File.Exists(filePath))
                return filePath;

            // 重新生成文件名
            int length = 13;
            string guid = Guid.NewGuid().ToString();
            if (guid.Length > length)
                guid = guid.Substring(0, length);

            return Path.Combine(Path.GetDirectoryName(filePath),
                string.Format("{0}.{1}{2}", Path.GetFileNameWithoutExtension(filePath), guid, Path.GetExtension(filePath)));
        }

        private void SaveAs(string savePath = null)
        {
            if (string.IsNullOrEmpty(savePath))
            {
                var result = _saveDialog.ShowDialog();

                if (result == DialogResult.OK)
                {
                    savePath = _saveDialog.FileName;

                    _ipRecordService.SaveIpRecord(_ipRecords, savePath);
                    MessageBox.Show("导出结果到" + savePath);
                }
            }
            else
            {
                _ipRecordService.SaveIpRecord(_ipRecords, savePath);
                MessageBox.Show("导出结果到" + savePath);
            }
        }

        private void tbxSearch_TextChanged(object sender, EventArgs e)
        {
            btnSearch.Enabled = _ipRecords != null &&
                !string.IsNullOrEmpty(tbxSearch.Text.Trim());
        }

        /// <summary>
        /// Search
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void searchToolStripButton_Click(object sender, EventArgs e)
        {
            try
            {
                SearchRecord binary_recorder;
                SearchRecord fibonacci_recorder;

                IpRecordHelper.SearchIpRecord(tbxSearch.Text.Trim(), _ipRecords, out binary_recorder, out fibonacci_recorder);

                if (binary_recorder.Index == -1)
                {
                    MessageBox.Show(string.Format(Messages.NotFound, tbxSearch.Text.Trim()));
                    return;
                }

                var content = (string.Format("第{0}个找到, ip 信息为", binary_recorder.Index + 1)
                        + Environment.NewLine
                        + string.Format("{0}-{1} {2}", binary_recorder.IpRecord.StartIp, binary_recorder.IpRecord.EndIp, binary_recorder.IpRecord.Address)
                        + Environment.NewLine
                        + Environment.NewLine
                        + string.Format("{0}: 查找次数: {1}, 用时(ms): {2}", "二分法查找", binary_recorder.Count, binary_recorder.ElapsedMilliseconds)
                        + Environment.NewLine
                        + string.Format("{0} 查找次数: {1}, 用时(ms): {2}", "斐波那契查找", fibonacci_recorder.Count, fibonacci_recorder.ElapsedMilliseconds)
                        );

                lblContent.Text = content;
            }
            catch (FormatException)
            {
                MessageBox.Show(Messages.WrongIpFormat);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
