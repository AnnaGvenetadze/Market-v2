using System.Management;
using System.Security.Cryptography;
using System.Text;

namespace Market.Extensions
{
    public static class HardwareFingerprintHelper
    {
        public static string GenerateHardwareFingerprint()
        {
            if (OperatingSystem.IsWindows())
            {
                return GenerateHardwareFingerprintOnWindows();
            }
            else if (OperatingSystem.IsLinux())
            {
                return GenerateHardwareFingerprintOnLinux();
            }

            throw new PlatformNotSupportedException("Hardware fingerprinting is only supported on Windows and Linux.");
        }
        private static string GenerateHardwareFingerprintOnWindows()
        {
            string moboUuid = GetWmiValue("Win32_ComputerSystemProduct", "UUID");
            string cpuId = GetWmiValue("Win32_Processor", "ProcessorId");
            string driveSerial = GetWmiValue("Win32_DiskDrive", "SerialNumber");
            string combined = $"{moboUuid}|{cpuId}|{driveSerial}";

            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(combined));
                return Convert.ToHexString(bytes);
            }
        }

        private static string GenerateHardwareFingerprintOnLinux()
        {
            string moboUuid = ReadLinuxSysFile("/sys/class/dmi/id/product_uuid");
            string cpuId = GetLinuxCpuInfo();
            string driveSerial = GetLinuxDiskSerial();
            string combined = $"{moboUuid}|{cpuId}|{driveSerial}";

            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(combined));
                return Convert.ToHexString(bytes);
            }
        }

        private static string GetLinuxCpuInfo()
        {
            try
            {
                var cpuInfo = System.IO.File.ReadAllText("/proc/cpuinfo");
                var lines = cpuInfo.Split('\n');
                foreach (var line in lines)
                {
                    if (line.StartsWith("Serial"))
                    {
                        return line.Split(':')[1].Trim();
                    }
                }
            }
            catch
            {
                
            }
            return string.Empty;
        }

        private static string GetLinuxDiskSerial()
        {
            try
            {
                string[] possibleDrivePaths = new[]
                {
                    "/sys/block/nvme0n1/device/serial",
                    "/sys/block/sda/device/serial"
                };

                foreach (string path in possibleDrivePaths)
                {
                    string serial = ReadLinuxSysFile(path);
                    if (!string.IsNullOrEmpty(serial))
                    {
                        return serial;
                    }
                }
            }
            catch
            {

            }
            return string.Empty;
        }

        private static string ReadLinuxSysFile(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    string value = File.ReadAllText(filePath).Trim();
                    if (!string.IsNullOrEmpty(value) && value != "None" && value != "Default string")
                    {
                        return value;
                    }
                }
            }
            catch
            {

            }
            return string.Empty;
        }

        private static string GetWmiValue(string wmiClass, string property)
        {
            try
            {
                using (var searcher = new ManagementObjectSearcher($"SELECT {property} FROM {wmiClass}"))
                {
                    foreach (var obj in searcher.Get())
                    {
                        return obj[property]?.ToString()?.Trim() ?? string.Empty;
                    }
                }
            }
            catch
            {

            }
            return string.Empty;
        }
    }
}
