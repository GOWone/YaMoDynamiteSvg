using System.IO;
using System;
using System.Windows;
using System.Linq;
using System.Xml.Linq;

namespace YaMoDynamiteSvg
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataCancel();
        }

        private void btnLoadFolder_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnParseSvgSource_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnGenerateXamlCode_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DataCancel()
        {
            // 指定根目录
            string rootDirectory = @"E:\GoogleDownload\icons";

            // 确保目标文件夹存在
            string targetDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data");
            Directory.CreateDirectory(targetDirectory);

            // 遍历根目录下的每个文件夹
            foreach (var folder in Directory.GetDirectories(rootDirectory))
            {
                string folderName = Path.GetFileName(folder);
                string resourceDictionaryPath = Path.Combine(targetDirectory, $"{folderName}.xaml");

                // 创建资源字典文件内容
                string resourceDictionaryContent = GenerateResourceDictionary(folder);

                // 将内容写入文件
                File.WriteAllText(resourceDictionaryPath, resourceDictionaryContent);
                Console.WriteLine($"Generated resource dictionary for folder: {folderName} -> {resourceDictionaryPath}");
            }
        }

        static string GenerateResourceDictionary(string folderPath)
        {
            // 获取该文件夹下的所有SVG文件
            string[] svgFiles = Directory.GetFiles(folderPath, "*.svg");

            // 构建资源字典的XAML内容
            string xamlContent = "<ResourceDictionary xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"\n";
            xamlContent += "                    xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\">\n";

            foreach (var svgFile in svgFiles)
            {
                string svgFileName = Path.GetFileNameWithoutExtension(svgFile);
                string keyName = ConvertToPascalCase(svgFileName);

                // 提取SVG文件中的path数据
                string pathData = ExtractPathData(svgFile);

                if (!string.IsNullOrEmpty(pathData))
                {
                    xamlContent += $"    <Geometry x:Key=\"{keyName}\">{pathData}</Geometry>\n";
                }
            }

            xamlContent += "</ResourceDictionary>";

            return xamlContent;
        }

        static string ConvertToPascalCase(string input)
        {
            // 移除后缀并替换短横线为下划线
            input = input.Replace("-", " ");
            // 将单词首字母大写
            return string.Concat(input.Split(' ')
                .Select(word => char.ToUpper(word[0]) + word.Substring(1)));
        }

        static string ExtractPathData(string svgFilePath)
        {
            try
            {
                XDocument svgDoc = XDocument.Load(svgFilePath);
                XElement pathElement = svgDoc.Descendants().FirstOrDefault(e => e.Name.LocalName == "path");

                if (pathElement != null && pathElement.Attribute("d") != null)
                {
                    return pathElement.Attribute("d").Value;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing SVG file: {svgFilePath}. Error: {ex.Message}");
            }

            return null;
        }
    }
}