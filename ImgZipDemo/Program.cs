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

            DirectoryInfo dir = new DirectoryInfo(@"F:\testimgs");

            sw.Start();
            Test(dir);

            //Test();

            Console.WriteLine("共花费时间:{0}ms", sw.ElapsedMilliseconds);
            Console.ReadLine();

        }

        //by Parallel+ThreadPool.QueueUserWorkItem()
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
                        Console.WriteLine("文件夹{0}已压缩完毕.当前线程ID:{1}",p, Thread.CurrentThread.ManagedThreadId);
                    }
                }, dis[j]);
            }
        }

        //by Parallel.ForEach
        static void AsynchronizationZipFile(DirectoryInfo dir)
        {
            DirectoryInfo[] dis = dir.GetDirectories();
            ParallelLoopResult all_result = Parallel.ForEach(dis,p=> {                
                FileInfo[] files = p.GetFiles();
                Console.WriteLine("目录：" + p.FullName);
                string wenj = p.FullName.Substring(2);
                sourcePath = @"F:\saveimg" + wenj;
                if (!Directory.Exists(sourcePath))
                {
                    // Create the directory it does not exist.
                    Directory.CreateDirectory(sourcePath);
                }
                var targetPath = sourcePath + @"\";
                Parallel.ForEach(files, fileinfo =>
                {
                    Stopwatch swatch = new Stopwatch();
                    swatch.Start();
                    Image sourceImg = Image.FromFile(fileinfo.FullName);
                    int width = sourceImg.Width < 1000 ? sourceImg.Width : 1000;
                    int height = int.Parse(Math.Round(sourceImg.Height * (double)width / sourceImg.Width).ToString());
                    System.Drawing.Image targetImg = new System.Drawing.Bitmap(width, height);
                    using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(targetImg))
                    {
                        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;
                        g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Default;
                        g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
                        g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighSpeed;
                        g.DrawImage(sourceImg, new System.Drawing.Rectangle(0, 0, width, height), new System.Drawing.Rectangle(0, 0, sourceImg.Width, sourceImg.Height), System.Drawing.GraphicsUnit.Pixel);
                        g.Dispose();
                    }
                    targetImg.Save(targetPath + Path.GetFileNameWithoutExtension(fileinfo.FullName) + ".jpg", System.Drawing.Imaging.ImageFormat.Jpeg);

                    sourceImg.Dispose();
                    targetImg.Dispose();
                    Console.WriteLine("图片: {0} 已被压缩--花费时间: {1}ms--图片大小: {2}", fileinfo.FullName, swatch.ElapsedMilliseconds, fileinfo.Length);
                });
            });            
        }

        static void SynchronizationZipFile(DirectoryInfo dir)
        {
            DirectoryInfo[] dis = dir.GetDirectories();
            foreach (var p in dis)
            {
                
                Console.WriteLine("目录：" + p.FullName);
                string wenj = p.FullName.Substring(2);
                sourcePath = @"F:\saveimg" + wenj;
                if (!Directory.Exists(sourcePath))
                {
                    // Create the directory it does not exist.
                    Directory.CreateDirectory(sourcePath);
                }
                var targetPath = sourcePath + @"\";
                FileInfo[] files = p.GetFiles();
                foreach (var fileinfo in files)
                {
                    Stopwatch swatch = new Stopwatch();
                    swatch.Start();
                    Image sourceImg = Image.FromFile(fileinfo.FullName);
                    int width = sourceImg.Width < 1000 ? sourceImg.Width : 1000;
                    int height = int.Parse(Math.Round(sourceImg.Height * (double)width / sourceImg.Width).ToString());
                    System.Drawing.Image targetImg = new System.Drawing.Bitmap(width, height);
                    using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(targetImg))
                    {
                        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;
                        g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Default;
                        g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
                        g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighSpeed;
                        g.DrawImage(sourceImg, new System.Drawing.Rectangle(0, 0, width, height), new System.Drawing.Rectangle(0, 0, sourceImg.Width, sourceImg.Height), System.Drawing.GraphicsUnit.Pixel);
                        g.Dispose();
                    }
                    targetImg.Save(targetPath + Path.GetFileNameWithoutExtension(fileinfo.FullName) + ".jpg", System.Drawing.Imaging.ImageFormat.Jpeg);

                    sourceImg.Dispose();
                    targetImg.Dispose();
                    Console.WriteLine("图片: {0} 已被压缩--花费时间: {1}ms--图片大小: {2}", fileinfo.FullName, swatch.ElapsedMilliseconds, fileinfo.Length);
                }
            }

        }

        static async Task<bool>  AsynchronizationZipFile2(DirectoryInfo dir)
        {
            DirectoryInfo[] dis = dir.GetDirectories();
            var a= await Task.Run(() =>
            {
                foreach (var item in dis)
                {
                    ThreadPool.QueueUserWorkItem(dir_item => {
                        var p = (DirectoryInfo)dir_item;
                        Console.WriteLine("目录：" + p.FullName);
                        string wenj = p.FullName.Substring(2);
                        sourcePath = @"F:\saveimg" + wenj;
                        if (!Directory.Exists(sourcePath))
                        {
                            // Create the directory it does not exist.
                            Directory.CreateDirectory(sourcePath);
                        }
                        var targetPath = sourcePath + @"\";
                        FileInfo[] files = p.GetFiles();
                        foreach (var file in files)
                        {
                            ThreadPool.QueueUserWorkItem(d => {
                                var fileinfo = (FileInfo)d;
                                Stopwatch swatch = new Stopwatch();
                                swatch.Start();
                                Image sourceImg = Image.FromFile(fileinfo.FullName);
                                int width = sourceImg.Width < 1000 ? sourceImg.Width : 1000;
                                int height = int.Parse(Math.Round(sourceImg.Height * (double)width / sourceImg.Width).ToString());
                                System.Drawing.Image targetImg = new System.Drawing.Bitmap(width, height);
                                using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(targetImg))
                                {
                                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;
                                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Default;
                                    g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
                                    g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighSpeed;
                                    g.DrawImage(sourceImg, new System.Drawing.Rectangle(0, 0, width, height), new System.Drawing.Rectangle(0, 0, sourceImg.Width, sourceImg.Height), System.Drawing.GraphicsUnit.Pixel);
                                    g.Dispose();
                                }
                                targetImg.Save(targetPath + Path.GetFileNameWithoutExtension(fileinfo.FullName) + ".jpg", System.Drawing.Imaging.ImageFormat.Jpeg);

                                sourceImg.Dispose();
                                targetImg.Dispose();
                                Console.WriteLine("图片: {0} 已被压缩--花费时间: {1}ms--图片大小: {2}", fileinfo.FullName, swatch.ElapsedMilliseconds, fileinfo.Length);
                            }, file);
                        }
                    }, item);
                }                
                return true;
            });

            return a;
        }

        static async Task Test(DirectoryInfo dir)
        {
            var hhe= AsynchronizationZipFile2(dir);
            Console.WriteLine(hhe);
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
