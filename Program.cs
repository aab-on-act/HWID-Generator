using System.Management;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;


namespace HwidGenerator;

internal static class Program
{
    [STAThread]
    static void Main()
    {
        try
        {
            string motherboard = QueryWmi("Win32_BaseBoard", "SerialNumber", @"root\cimv2");
            string cpuId = QueryWmi("Win32_Processor", "ProcessorId", @"root\cimv2");
            string diskSerial = QueryWmi("Win32_DiskDrive", "SerialNumber", @"root\cimv2");
            string biosUuid = QueryWmi("Win32_ComputerSystemProduct", "UUID", @"root\cimv2");
            string macAddress = GetMacAddress();

            var sb = new StringBuilder();
            sb.Append(HashId(motherboard)).Append("zczc");
            sb.Append(HashId(cpuId)).Append("zczc");
            sb.Append(HashId(diskSerial)).Append("zczc");
            sb.Append(HashId(biosUuid)).Append("zczc");
            sb.Append(HashId(macAddress));

            string aabInfo = sb.ToString();

            Clipboard.SetText(aabInfo);
            MessageBox.Show(
                "AABINFO has copied into clipboard, please paste it to the ticket channel.",
                "AAB HWID Generator",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"An error occurred while generating HWID:\n{ex.Message}",
                "AAB HWID Generator - Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            );
        }
    }

    private static string GetMacAddress()
    {
        try
        {
            foreach (var nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (nic.OperationalStatus == OperationalStatus.Up &&
                    nic.NetworkInterfaceType != NetworkInterfaceType.Loopback &&
                    nic.NetworkInterfaceType != NetworkInterfaceType.Tunnel)
                {
                    string mac = nic.GetPhysicalAddress().ToString();
                    if (!string.IsNullOrWhiteSpace(mac))
                        return mac;
                }
            }
        } 
        catch {}

        return "NOMAC";
    }

    private static string HashId(string value)
    {
        if (string.IsNullOrEmpty(value))
            value = "UNKNOWN";

        byte[] hash = SHA256.HashData(Encoding.UTF8.GetBytes(value));
        return Convert.ToHexStringLower(hash)[..20];
    }

    private static string QueryWmi(string wmiClass, string property, string scope)
    {
        try
        {
            using var searcher = new ManagementObjectSearcher(
                new ManagementScope(scope),
                new ObjectQuery($"SELECT {property} FROM {wmiClass}")
            );

            foreach (ManagementObject obj in searcher.Get())
            {
                object? val = obj[property];
                if (val != null)
                {
                    string result = val.ToString()?.Trim() ?? string.Empty;
                    if (!string.IsNullOrWhiteSpace(result))
                        return result;
                }
            }
        }
        catch {}

        return string.Empty;
    }
}