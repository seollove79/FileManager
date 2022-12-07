using Microsoft.WindowsAPICodePack.Dialogs;
using Microsoft.WindowsAPICodePack.Dialogs.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FileManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        String targetFolderName;
        DirectoryInfo di;
        int targetFileCnt = 0;

        bool debugMode = false;

        public MainWindow()
        {
            InitializeComponent();
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog cofd = new CommonOpenFileDialog();
            cofd.IsFolderPicker = true;

            if(cofd.ShowDialog() == CommonFileDialogResult.Ok)
            {
                targetFolderName = cofd.FileName;
                di = new System.IO.DirectoryInfo(targetFolderName);
                targetFileCnt = di.GetFiles().Length;
                lblFileCnt.Content = targetFileCnt.ToString();

                if (targetFileCnt < 1) {
                    MessageBox.Show("선택 폴더에 파일이 없습니다.");
                    return;
                }
            }
        }

        private void btnEditFilename_Click(object sender, RoutedEventArgs e)
        {
            //디버그용
            /*datePicker01.Text = "2022-12-07";
            textBoxStartHour.Text = "21";
            textBoxHourDiv.Text = "1";
            textBoxDayCnt.Text = "10";
            textBoxUnit.Text = "100";
            textBoxIp.Text = "100.100.100.100";
            textBoxPort.Text = "60";*/

            string strStartDate = datePicker01.Text;
            string strStartHour = textBoxStartHour.Text;
            string strHourDiv = textBoxHourDiv.Text;
            string strDayCnt = textBoxDayCnt.Text;
            string strUnit = textBoxUnit.Text;
            string strIp = textBoxIp.Text;
            string strPort = textBoxPort.Text;


            Regex regex = new Regex(@"^(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])\.(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])\.(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])\.(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])$");
            if (!regex.IsMatch(strIp))
            {
                MessageBox.Show("정상적인 IP 주소를 입력하세요.");
                return;
            }

            strPort = strPort.Replace(" ", "");
            if (strPort.Length == 0)
            {
                MessageBox.Show("포트를 입력하세요.");
                return;
            }

            if (strStartDate.Length == 0)
            {
                MessageBox.Show("수집시작일을 입력하세요.");
                return;
            }

            if (strStartHour.Length == 0)
            {
                MessageBox.Show("수집시작 시간 입력하세요.");
                return;
            }
            else
            {
                if (!(Int32.Parse(strStartHour) >= 1 && Int32.Parse(strStartHour) <= 24))
                {
                    MessageBox.Show("수집시작시간은 1~24 사이의 숫자입니다.");
                    return;
                }
            }

            if (strHourDiv.Length == 0)
            {
                MessageBox.Show("수집간격을 입력하세요.");
                return;
            }
            else
            {
                if (!(Int32.Parse(strHourDiv) >= 1 && Int32.Parse(strHourDiv) <= 24))
                {
                    MessageBox.Show("수집간격은 1~24 사이의 숫자입니다.");
                    return;
                }
            }

            if (strDayCnt.Length == 0)
            {
                MessageBox.Show("일별 수집 수량을 입력하세요.");
                return;
            }
            else
            {
                if (Int32.Parse(strDayCnt) < 1)
                {
                    MessageBox.Show("일별수집수량은 1이상이어야 합니다.");
                    return;
                }
            }

            if (targetFolderName == null || targetFolderName == "")
            {
                MessageBox.Show("대상 폴더를 선택하세요.");
                return;
            }

            if (targetFileCnt < 1)
            {
                MessageBox.Show("선택된 파일이 없습니다.");
                return;
            }

            if (strUnit.Length == 0)
            {
                MessageBox.Show("작업파일 단위를 입력하세요.");
                return;
            }
            else
            {
                if (Int32.Parse(strUnit) < 1)
                {
                    MessageBox.Show("작업파일단위는 1이상이어야 합니다.");
                    return;
                }
            }

            string[] dateArray = strStartDate.Split('-');

            //DateTime startDate = DateTime.Parse(strStartDate);
            DateTime startDate = new DateTime(Int32.Parse(dateArray[0]), Int32.Parse(dateArray[1]), Int32.Parse(dateArray[2]), Int32.Parse(strStartHour), 0, 0);
            DateTime checkDate = new DateTime(Int32.Parse(dateArray[0]), Int32.Parse(dateArray[1]), Int32.Parse(dateArray[2]), Int32.Parse(strStartHour), 0, 0);

            Debug.WriteLine("startDate : " + startDate);
            Debug.WriteLine("checkDate : " + checkDate);

            int intStartHour = Int32.Parse(strStartHour);
            int intHourDiv = Int32.Parse(strHourDiv);
            int intDayCnt = Int32.Parse(strDayCnt);
            int intUnit = Int32.Parse(strUnit);

            int i = 0;
            int workFileCnt = 0;

            foreach (FileInfo File in di.GetFiles())
            {
                if (workFileCnt >= intUnit)
                {
                    MessageBox.Show("요청 수량 작업 완료");
                    return;
                }

                string strDate = checkDate.Date.ToString("yyyyMMdd");
                string strHour = "";

                if(checkDate.Hour < 10)
                {
                    strHour = "0" + checkDate.Hour.ToString() + "0000";
                }
                else
                {
                    strHour = checkDate.Hour.ToString() + "0000";
                }

                string targetFolderFull = "";
                string targetFolder1 = checkDate.ToString("yyyy") + "-" + checkDate.ToString("MMdd");
                string targetFolder2 = strIp + "." + strPort;

                String newFileName = strIp + "." + strPort + "-" + strDate + "." + strHour + ".png";
                tbMultiLine.Text = tbMultiLine.Text + newFileName + "\n";

                Debug.WriteLine(File.Directory + "\\" + newFileName);
                
                if(!System.IO.Directory.Exists(di.FullName + "\\" + targetFolder1))
                {
                    System.IO.Directory.CreateDirectory(di.FullName + "\\" + targetFolder1);
                }

                if (!System.IO.Directory.Exists(di.FullName + "\\" + targetFolder1 + "\\" + targetFolder2))
                {
                    System.IO.Directory.CreateDirectory(di.FullName + "\\" + targetFolder1 + "\\" + targetFolder2);
                }


                if (System.IO.File.Exists(di.FullName + "\\" + targetFolder1 + "\\" + targetFolder2 + "\\" + newFileName))
                {
                    tbMultiLine.Text += newFileName + "가(이) 현재 폴더에 존재 합니다.";
                }
                else
                {
                    File.MoveTo(di.FullName + "\\" + targetFolder1 + "\\" + targetFolder2 + "\\" + newFileName, true);
                }


                checkDate = checkDate.AddHours(intHourDiv);
                i++;
                workFileCnt++;

                if (i >= intDayCnt)
                {
                    i = 0;
                    checkDate = new DateTime(checkDate.Year, checkDate.Month, checkDate.Day, intStartHour, 0, 0);
                    checkDate.AddDays(1);
                }
            }

            MessageBox.Show("변환완료");
            return;

        }

        private void textBoxNumber_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}
