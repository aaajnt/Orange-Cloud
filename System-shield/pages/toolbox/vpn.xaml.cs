using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Net.NetworkInformation;
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


namespace System_shield.pages.toolbox
{
    /// <summary>
    /// vpn.xaml 的交互逻辑
    /// </summary>
        public class ServerInfo
        {
            public string Address { get; set; }
            public int Port { get; set; }
            public string Latency { get; set; } // 假设延迟是以字符串形式存储的
        }
    public partial class vpn : Page
    {
        private static HttpClientHandler globalHttpClientHandler = new HttpClientHandler();
        public static HttpClient globalHttpClient = new HttpClient(globalHttpClientHandler);
        private ObservableCollection<ServerInfo> servers = new ObservableCollection<ServerInfo>();
        private Process xrayProcess;
        public vpn()
        {
            InitializeComponent();
            lvServers.ItemsSource = servers;
        }

        private void AddServer_Click(object sender, RoutedEventArgs e)
        {
            // 添加服务器信息到列表
            servers.Add(new ServerInfo
            {
                Address = txtAddress.Text,
                Port = int.Parse(txtPort.Text),
                Latency = "Unknown" // 初始延迟状态
            });

            // 清空输入
            txtAddress.Clear();
            txtPort.Clear();
        }
        private void StartProxy_Click(object sender, RoutedEventArgs e)
        {
            // 取得选定的服务器信息
            var selectedServer = lvServers.SelectedItem as ServerInfo;

            if (selectedServer == null)
            {
                MessageBox.Show("Please select a server from the list.");
                return;
            }

            // TODO: 启动 Xray 代理逻辑
            // 根据 selectedServer 中的信息构建 Xray 命令行参数

            // 示例，确保替换为正确的 Xray 可执行文件路径和参数
            string xrayExecutablePath = @"C:\Users\Administrator\Desktop\bing\blog\System-shield\System-shield\GO language program\xray.exe"; // 替换为 Xray 可执行文件的实际路径
            string configFilePath = @"C:\Users\Administrator\Desktop\bing\blog\System-shield\System-shield\GO language program\sh.json"; // 替换为你的配置文件的实际路径
            string arguments = $"run -c \"{configFilePath}\""; // 注意这里的引号

            xrayProcess = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = xrayExecutablePath,
                    Arguments = arguments,
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                },
                EnableRaisingEvents = true
            };

            try
            {
                xrayProcess.OutputDataReceived += (outputSender, args) => Dispatcher.Invoke(() => {
                    // 在这里更新你的输出显示，例如在一个文本框中
                    txtPor.Text += args.Data + Environment.NewLine;
                });

                xrayProcess.ErrorDataReceived += (errorSender, args) => Dispatcher.Invoke(() => {
                    // 在这里更新你的错误显示，例如在一个文本框中
                    txtPor.Text += args.Data + Environment.NewLine;
                });

                xrayProcess.Start();
                xrayProcess.BeginOutputReadLine();
                xrayProcess.BeginErrorReadLine();

                // 不要在这里调用 xrayProcess.WaitForExit(); 因为它会阻塞 UI 线程
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to start Xray: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            var timer = new System.Windows.Threading.DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(5); // 设置延迟时间为5秒
            timer.Tick += (timerSender, timerArgs) =>
            {
                // 更新 UI 显示连接成功
                txtPr.Text += "连接成功！\n";

                // 停止定时器，以免重复触发
                timer.Stop();
                CheckConnectionAndDisplayLatency(selectedServer.Address);
                SetGlobalProxy(selectedServer.Address, selectedServer.Port);
            };

            // 启动定时器
            timer.Start();

        }
        private async void CheckConnectionAndDisplayLatency(string addressToPing)
        {
            // 使用Ping类来发送异步Ping请求
            Ping pingSender = new Ping();
            try
            {
                PingReply reply = await pingSender.SendPingAsync(addressToPing);
                if (reply.Status == IPStatus.Success)
                {
                    // Ping成功，显示延迟
                    Dispatcher.Invoke(() =>
                    {
                        txtPr.Text += $"连接成功！延迟：{reply.RoundtripTime}ms\n";
                    });
                }
                else
                {
                    // Ping失败
                    Dispatcher.Invoke(() =>
                    {
                        txtPr.Text += "连接失败。\n";
                    });
                }
            }
            catch (Exception ex)
            {
                // Ping过程中发生异常
                Dispatcher.Invoke(() =>
                {
                    txtPr.Text += $"连接测试失败：{ex.Message}\n";
                });
            }
            if (lvServers.SelectedItem is ProxyServerInfo selectedProxy)
            {
                var proxy = new WebProxy(selectedProxy.Address, selectedProxy.Port);
                var httpClientHandler = new HttpClientHandler
                {
                    Proxy = proxy,
                    UseProxy = true,
                };

                using (var httpClient = new HttpClient(httpClientHandler))
                {
                    try
                    {
                        // 替换为你实际需要访问的网站地址
                        string targetUrl = "YOUR_TARGET_WEBSITE_HERE";
                        var response = await httpClient.GetStringAsync(targetUrl);
                        MessageBox.Show(response, "Response from the website");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Failed to make request with proxy: {ex.Message}", "Error");
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a proxy server from the list.", "Warning");
            }

        }
        private void SetGlobalProxy(string serverAddress, int serverPort)
        {
            globalHttpClientHandler.Proxy = new WebProxy(serverAddress, serverPort);
            globalHttpClientHandler.UseProxy = true;

            string proxyAddress = $"{serverAddress}:{serverPort}";
            MessageBox.Show($"Global proxy set to {proxyAddress}");
            globalHttpClient = new HttpClient(globalHttpClientHandler);


            // 设置命令行参数以设置全局代理
            ProcessStartInfo psi = new ProcessStartInfo("netsh", $"winhttp set proxy proxy-server=\"{proxyAddress}\" bypass-list=\"<local>\"")
            {
                Verb = "runas", // 以管理员权限运行
                CreateNoWindow = true,
                UseShellExecute = true
            };

            try
            {
                using (Process process = Process.Start(psi))
                {
                    process.WaitForExit(); // 等待命令执行完成
                    MessageBox.Show($"Global proxy set to {proxyAddress}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to set global proxy: {ex.Message}");
            }
        }
        public class ProxyServerInfo
        {
            public string Address { get; set; }
            public int Port { get; set; }
        }

        public class ServerInfo
        {
            public string Address { get; set; }
            public int Port { get; set; }
            public string Latency { get; set; }
        }


    }
}
