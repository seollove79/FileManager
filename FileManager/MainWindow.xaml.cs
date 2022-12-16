using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

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
        bool moveOnly = false;

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
            if (debugMode)
            {
                datePicker01.Text = "2022-12-07";
                textBoxStartHour.Text = "5";
                textBoxHourDiv.Text = "30";
                textBoxDayCnt.Text = "16";
                textBoxUnit.Text = "100";
                textBoxIp.Text = "경남밀양";
                textBoxPort.Text = "가지농장";
            }
            

            string strStartDate = datePicker01.Text;
            string strStartHour = textBoxStartHour.Text;
            string strHourDiv = textBoxHourDiv.Text;
            string strDayCnt = textBoxDayCnt.Text;
            string strUnit = textBoxUnit.Text;
            string strIp = textBoxIp.Text;
            string strPort = textBoxPort.Text;
            if(chkMoveOnly.IsChecked == true)
            {
                moveOnly = true;
                datePicker01.Text = "2022-12-07";
                textBoxStartHour.Text = "5";
                textBoxHourDiv.Text = "1";
                textBoxDayCnt.Text = "10";
                textBoxIp.Text = "의미없음";
                textBoxPort.Text = "의미없음";
            }
            else
            {
                moveOnly = false;
            }

            strIp = strIp.Replace(" ", "");
            if (strIp.Length == 0)
            {
                MessageBox.Show("지역명을 입력하세요.");
                return;
            }

            strPort = strPort.Replace(" ", "");
            if (strPort.Length == 0)
            {
                MessageBox.Show("농장명을 입력하세요.");
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

            DateTime checkDate = new DateTime(Int32.Parse(dateArray[0]), Int32.Parse(dateArray[1]), Int32.Parse(dateArray[2]), Int32.Parse(strStartHour), 0, 0);

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

                strHour = checkDate.ToString("HHmm") + "00";

                string targetFolderFull = "";
                string targetFolder1 = checkDate.ToString("MMdd");
                string targetFolder2 = strIp + "_" + strPort;

                String newFileName = strIp + "_" + strPort + "-" + strDate + "." + strHour + ".png";
                tbMultiLine.Text = tbMultiLine.Text + newFileName + "\n";

                Debug.WriteLine(File.Directory + "\\" + newFileName);

                if(debugMode == false) { 

                    if (moveOnly == false)
                    {
                        if (!System.IO.Directory.Exists(di.FullName + "\\" + targetFolder1))
                        {
                            System.IO.Directory.CreateDirectory(di.FullName + "\\" + targetFolder1);
                        }

                        if (!System.IO.Directory.Exists(di.FullName + "\\" + targetFolder1 + "\\" + targetFolder2))
                        {
                            System.IO.Directory.CreateDirectory(di.FullName + "\\" + targetFolder1 + "\\" + targetFolder2);
                        }


                        if (System.IO.File.Exists(di.FullName + "\\" + targetFolder1 + "\\" + targetFolder2 + "\\" + newFileName))
                        {
                            tbMultiLine.Text += newFileName + "가(이) 현재 폴더에 존재 합니다.\n";
                        }
                        File.MoveTo(di.FullName + "\\" + targetFolder1 + "\\" + targetFolder2 + "\\" + newFileName, true);
                    } else
                    {
                        moveFolder(File);
                    }
                }



                checkDate = checkDate.AddMinutes(intHourDiv);
                i++;
                workFileCnt++;

                if (i >= intDayCnt)
                {
                    int addDay;
                    i = 0;
                    if(checkDate.Hour > intStartHour)
                    {
                        addDay = 1;
                    }
                    else
                    {
                        addDay = 0;
                    }
                    checkDate = new DateTime(checkDate.Year, checkDate.Month, checkDate.Day, intStartHour, 0, 0);
                    checkDate = checkDate.AddDays(addDay);
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

        private void moveFolder(System.IO.FileInfo file)
        {
            string filename = file.Name;
            string temp = filename.Split("-")[1];
            temp = temp.Split(".")[0];
            string folder1 = temp.Substring(4, 4);
            

            string folder2 = filename.Split("-")[0];

            if (!System.IO.Directory.Exists(di.FullName + "\\" + folder1))
            {
                System.IO.Directory.CreateDirectory(di.FullName + "\\" + folder1);
            }

            if (!System.IO.Directory.Exists(di.FullName + "\\" + folder1 + "\\" + folder2))
            {
                System.IO.Directory.CreateDirectory(di.FullName + "\\" + folder1 + "\\" + folder2);
            }


            file.MoveTo(di.FullName + "\\" + folder1 + "\\" + folder2 + "\\" + file.Name, true);
        }
    }
}
