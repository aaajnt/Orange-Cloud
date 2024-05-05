using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using System.Diagnostics;
using System.IO;
using Path = System.IO.Path;
using Newtonsoft.Json;

namespace System_shield.pages
{
    /// <summary>
    /// Plug_in_management.xaml 的交互逻辑
    /// </summary>
    public partial class Plug_in_management : Page
    {
        private string pluginDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "plugins");
        private string pluginsPath = "plugins";
        public Plug_in_management()
        {
            InitializeComponent();
            CreatePluginDirectory();
            RefreshPlugins(); // 初始加载
            LoadPlugins();
        }
        private void CreatePluginDirectory()
        {
            if (!Directory.Exists(pluginsPath))
            {
                Directory.CreateDirectory(pluginsPath); // 必要的话在此位置添加更多的检查和错误处理
            }
        }
        private void LoadPlugins()
        {
            PluginListView.Items.Clear(); // 清除现有的插件项

            // 获取插件目录中的所有可执行文件 (.exe 和 .jar)
            var pluginFiles = Directory.EnumerateFiles(pluginDirectory)
                                       .Where(file => file.ToLower().EndsWith(".exe") || file.ToLower().EndsWith(".jar"));

            foreach (var filePath in pluginFiles)
            {
                FileInfo fileInfo = new FileInfo(filePath);
                PluginListView.Items.Add(new PluginData
                {
                    // 使用 Path.GetFileNameWithoutExtension 来获取没有扩展名的文件名
                    Name = Path.GetFileNameWithoutExtension(fileInfo.Name), // 不带扩展名的文件名
                    Path = fileInfo.FullName // 完整文件路径
                });
            }
        }
        private void ExecutePlugin_Click(object sender, RoutedEventArgs e)
        {
            if (!(sender is MenuItem menuItem)) return;
            if (!(PluginListView.SelectedItem is PluginData selectedPlugin)) return;

            try
            {
                // 判断是不是.jar文件
                if (selectedPlugin.Path.EndsWith(".jar"))
                {
                    Process.Start("java", $"-jar \"{selectedPlugin.Path}\"");
                }
                else // 对于其他类型的文件，例如.exe
                {
                    Process.Start(selectedPlugin.Path);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Could not execute the plugin: {ex.Message}", "Execution Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void MenuItem_Open_Click(object sender, RoutedEventArgs e)
        {
            // 尝试获取MenuItem
            if (sender is MenuItem menuItem)
            {
                // 获取ContextMenu
                ContextMenu contextMenu = menuItem.Parent as ContextMenu;

                if (contextMenu != null && contextMenu.PlacementTarget is FrameworkElement targetElement)
                {
                    // 现在从PlacementTarget获取DataContext
                    dynamic dataContext = targetElement.DataContext;

                    // 做你需要的操作...
                    try
                    {
                        // 例如：根据插件的Path启动插件
                        string pluginPath = dataContext.Path;
                        Process.Start(pluginPath);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Could not execute the plugin: {ex.Message}", "Execution Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }
        private void OpenPluginFolder_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("explorer.exe", pluginDirectory);

        }
        private void RefreshPlugins_Click(object sender, RoutedEventArgs e)
        {
            RefreshPlugins(); // 调用刷新插件方法
        }

        private void OpenPlugin_Click(object sender, RoutedEventArgs e)
        {
            dynamic listViewItem = PluginListView.SelectedItem;
            if (listViewItem != null)
            {
                string pluginPath = listViewItem.Path;
                Process.Start(pluginPath);
            }
        }
        private void RefreshPlugins()
        {
            PluginListView.Items.Clear(); // 清理现在的插件列表，避免重复

            // 加载插件的代码
            DirectoryInfo directoryInfo = new DirectoryInfo(pluginDirectory);
            FileInfo[] pluginFiles = directoryInfo.GetFiles("*.exe", SearchOption.TopDirectoryOnly);

            foreach (FileInfo file in pluginFiles)
            {
                PluginListView.Items.Add(new { Name = file.Name, Path = file.FullName });
            }
        }
        public class PluginResult
        {
            public string Status { get; set; }
            public string Message { get; set; }
        }
        public class PluginData
        {
            public string Name { get; set; }
            public string Path { get; set; }
        }

        public class PluginExecutor
        {
            public string PluginDirectory { get; private set; }

            public PluginExecutor(string pluginDirectory)
            {
                PluginDirectory = pluginDirectory;
            }

            public async Task<PluginResult> ExecutePluginAsync(string pluginPath, string command, string path)
            {
                using (Process pluginProcess = new Process())
                {
                    pluginProcess.StartInfo.FileName = pluginPath;
                    pluginProcess.StartInfo.Arguments = $"/command \"{command}\" /path \"{path}\"";
                    pluginProcess.StartInfo.RedirectStandardOutput = true;
                    pluginProcess.StartInfo.UseShellExecute = false;
                    pluginProcess.StartInfo.CreateNoWindow = true;

                    pluginProcess.Start();

                    // This will block until the plugin process exits
                    pluginProcess.WaitForExit(); // 替换 WaitForExitAsync

                    // Read the output stream
                    string output = await pluginProcess.StandardOutput.ReadToEndAsync();

                    // Deserialize the JSON output to a C# object
                    PluginResult result = JsonConvert.DeserializeObject<PluginResult>(output);

                    return result;
                }
            }
        }
    }
}
