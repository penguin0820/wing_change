using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;

namespace wing_page
{
    public partial class MainWindow : Window
    {
        const int normal_size = 45;//初始化倍率
        Brush currentBush = new SolidColorBrush(Colors.Black);//筆刷
        List<List<X_Y_Zitem>> mut_item = new List<List<X_Y_Zitem>>();//二維item
        X_Y_Zitem item_center = new X_Y_Zitem();//中心點
        int curlayout = 0;//計算層數
        Point start, end;
        public MainWindow()
        {
            InitializeComponent();
        }
        //開始第零層
        private void in_put_BUT_click(object sender, RoutedEventArgs e)
        {
            
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            OpenFileDialog openfile = new OpenFileDialog();
            openfile.DefaultExt = ".txt";
            openfile.Filter = "Text file|*.txt|CSV file|*.csv|All file|*.*";
            if (openfile.ShowDialog() == true)
            {
                string path = openfile.FileName;
                app_start();
                var reader = new StreamReader(path, Encoding.GetEncoding("big5"));//var不明確的宣告
                string line;
                List<X_Y_Zitem> item = new List<X_Y_Zitem>();
                List<X_Y_Zitem> item_2 = new List<X_Y_Zitem>();
                while ((line = reader.ReadLine()) != null)
                {
                    string[] words = line.Split(' ');
                    if (words.Length > 5)
                    {
                        double[] temp=new double[3];
                        short cmp = 0;
                        for(int i = 0; i < words.Length; i++)
                        {
                            if(words[i]!="")
                            {
                                temp[cmp] = Convert.ToDouble(words[i]);
                                cmp++;
                            }
                        }
                        double x = Convert.ToDouble(temp[0]) * normal_size;
                        double y = Convert.ToDouble(temp[1]) * normal_size;
                        item.Add(new X_Y_Zitem() { X = x, Y = y, Z = 0 });
                        item_2.Add(new X_Y_Zitem() { X = x, Y = y, Z = 0 });
                    }
                    else
                    {
                        words = line.Split('\t');
                        double x = Convert.ToDouble(words[0]) * normal_size;
                        double y = Convert.ToDouble(words[1]) * normal_size;
                        item.Add(new X_Y_Zitem() { X = x, Y = y, Z = 0 });
                        x -= 40;y -= 40;
                        item_2.Add(new X_Y_Zitem() { X = x, Y = y, Z = 0 });
                    }
                }
                item_center.X = item[item.Count() - 1].X / 2;
                item_center.Y = item[item.Count() - 1].Y / 2;
                mut_item.Add(item);
                mut_item.Add(item_2);
                DrawLine(curlayout);
            }
        }
        //畫中心線
        private void app_start()
        {//615,978
            Point top = new Point()
            {
                X = MyCanvas.Width / 2,
                Y = 0,
            };
            Point under = new Point()
            {
                X = MyCanvas.Width / 2,
                Y = MyCanvas.Height,
            };
            Point left = new Point()
            {
                X = 0,
                Y = MyCanvas.Height/2,
            };
            Point right = new Point()
            {
                X = MyCanvas.Width,
                Y = MyCanvas.Height / 2,
            };
            creatLine(MyCanvas,top, under,1, currentBush);
            creatLine(MyCanvas,right, left,1, currentBush);
        }
        //畫線
        private void creatLine(Canvas _canvas,Point F_start,Point F_end,double _StrokeThickness,Brush _currentBush)
        {
            Line newline = new Line()
            {
                Stroke = _currentBush,
                StrokeThickness = _StrokeThickness,
                X1 = F_start.X,
                Y1 = F_start.Y,
                X2 = F_end.X,
                Y2 = F_end.Y,
            };
             _canvas.Children.Add(newline);
        }
        //檔案匯出txt
        private void Out_put_file_Click(object sender, RoutedEventArgs e)
        {
            int layout = Convert.ToInt32(out_put.Text);
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.DefaultExt = ".txt";
            saveFileDialog.Filter = "Text file|*.txt|All file|*.*";
            if (saveFileDialog.ShowDialog() == true)
            {
                string path = saveFileDialog.FileName;
                using (StreamWriter sw = File.CreateText(path)) {
                    string out_put="";
                    foreach (X_Y_Zitem item in mut_item[layout])
                    {
                        out_put += item.X / normal_size + "\t" + item.Y / normal_size + "\t" + item.Z / normal_size + "\n";
                    }
                    sw.WriteLine(out_put);
                }
            }
        }
        //建立下一層
        private void next_lay_Click(object sender, RoutedEventArgs e)
        {
            List<X_Y_Zitem> item = new List<X_Y_Zitem>();
            MyCanvas_Copy.Children.Clear();
        }
        //重複畫線
        private void DrawLine(int layout)
        {
            for (int i = 0; i < mut_item[layout].Count()-1; i++)
            {
                start.X = mut_item[layout][i].X + (MyCanvas.Width / 2) - item_center.X;
                start.Y = mut_item[layout][i].Y + (MyCanvas.Height / 2) - item_center.Y;
                end.X = mut_item[layout][i+1].X + (MyCanvas.Width / 2) - item_center.X;
                end.Y = mut_item[layout][i+1].Y + (MyCanvas.Height / 2) - item_center.Y;
                creatLine(MyCanvas,start, end, 2, currentBush);
            }
            ColorDialog colorDialog = new ColorDialog();
            colorDialog.ShowDialog();
            Color currentColor;
            System.Drawing.Color color_input = colorDialog.Color;
            currentColor = System.Windows.Media.Color.FromArgb(color_input.A, color_input.R, color_input.G, color_input.B);
            currentBush = new SolidColorBrush(currentColor);
            for (int i = 0; i < mut_item[layout+1].Count() - 1; i++)
            {
                start.X = mut_item[layout + 1][i].X + (MyCanvas.Width / 2) - item_center.X;
                start.Y = mut_item[layout + 1][i].Y + (MyCanvas.Height / 2) - item_center.Y;
                end.X = mut_item[layout + 1][i + 1].X + (MyCanvas.Width / 2) - item_center.X;
                end.Y = mut_item[layout + 1][i + 1].Y + (MyCanvas.Height / 2) - item_center.Y;
                creatLine(MyCanvas_Copy,start, end, 2, currentBush);
            }
            
        }
    }
}
