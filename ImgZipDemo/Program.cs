using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using System.Diagnostics;
using System.Threading;

namespace TestFindFile
{
    class Program
    {
        static Stopwatch sw = new Stopwatch();
        static string sourcePath = "";
        static  void Main(string[] args)
        {
            // string path = @"F:\MyDir";
            // string target = @"c:\TestDir";

            DirectoryInfo di = new DirectoryInfo(@"F:\testimgs");

            sw.Start();
            FindFilew(di);
            Console.WriteLine("共花费时间:{0}ms",sw.ElapsedMilliseconds);
            //Test();
            
            
            Console.ReadLine();

        }

        static void  FindFilew(DirectoryInfo di)
        {
            DirectoryInfo[] dis = di.GetDirectories();
            for (int j = 0; j < dis.Length; j++)
            {
                Console.WriteLine("目录：" + dis[j].FullName);
                string wenj = dis[j].FullName.Substring(2);
                sourcePath = @"F:\saveimg" + wenj;
                if (!Directory.Exists(sourcePath))
                {
                    // Create the directory it does not exist.
                    Directory.CreateDirectory(sourcePath);
                }                
                var targetPath = sourcePath + @"\";                       
                ThreadPool.QueueUserWorkItem(p => {
                    //FileInfo[] files =(FileInfo[]) p;
                    var currentDir = (DirectoryInfo)p;
                    FileInfo[] files = currentDir.GetFiles();
                    ParallelOptions po = new ParallelOptions();
                    po.MaxDegreeOfParallelism = 15;
                    ParallelLoopResult result = Parallel.ForEach(files, po, (fileinfo) =>
                    {
                        Stopwatch swatch = new Stopwatch();
                        swatch.Start();
                        Image sourceImg = Image.FromFile(fileinfo.FullName);
                        int width = sourceImg.Width < 1000 ? sourceImg.Width : 1000;
                        int height = int.Parse(Math.Round(sourceImg.Height * (double)width / sourceImg.Width).ToString());
                        System.Drawing.Image targetImg = new System.Drawing.Bitmap(width, height);
                        using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(targetImg))
                        {
                            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                            g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                            g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                            g.DrawImage(sourceImg, new System.Drawing.Rectangle(0, 0, width, height), new System.Drawing.Rectangle(0, 0, sourceImg.Width, sourceImg.Height), System.Drawing.GraphicsUnit.Pixel);
                            g.Dispose();
                        }                        
                        targetImg.Save(targetPath + Path.GetFileNameWithoutExtension(fileinfo.FullName) + ".jpg", System.Drawing.Imaging.ImageFormat.Jpeg);

                        sourceImg.Dispose();
                        targetImg.Dispose();
                        Console.WriteLine("图片: {0} 已被压缩--花费时间: {1}ms--图片大小: {2}", fileinfo.FullName, swatch.ElapsedMilliseconds, fileinfo.Length);
                    });
                    if (result.IsCompleted)
                    {
                        Console.WriteLine("文件夹{0}已压缩完毕. \r\n",p);
                    }
                }, dis[j]);
            }
        }

        public static async Task<Image>  GetImageThumbByFileInfo(FileInfo fileinfo)
        {

            return await Task.Run(() =>
            {                
                Stopwatch sw = new Stopwatch();
                sw.Start();
                Image sourceImg = Image.FromFile(fileinfo.FullName);
                int width = sourceImg.Width < 1000 ? sourceImg.Width : 1000;
                int height = int.Parse(Math.Round(sourceImg.Height * (double)width / sourceImg.Width).ToString());
                System.Drawing.Image targetImg = new System.Drawing.Bitmap(width, height);
                using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(targetImg))
                {
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                    g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                    g.DrawImage(sourceImg, new System.Drawing.Rectangle(0, 0, width, height), new System.Drawing.Rectangle(0, 0, sourceImg.Width, sourceImg.Height), System.Drawing.GraphicsUnit.Pixel);
                    g.Dispose();
                }
                sourceImg.Dispose();
                Console.WriteLine("图片: {0} 已被压缩--花费时间: {1}ms--图片大小: {2}",fileinfo.FullName,sw.ElapsedMilliseconds,fileinfo.Length);
                return targetImg;
            });            
        }


    }
}
