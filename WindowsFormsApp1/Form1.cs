using DiscUtils.Streams;
using DiscUtils;
using System;
using System.IO;
using System.IO.Compression;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using DiscUtils.Partitions;
using DiscUtils.Ntfs;
using DiscUtils.Raw;


namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr CreateFile(
           string lpFileName,
           FileAccess dwDesiredAccess,
           FileShare dwShareMode,
           IntPtr lpSecurityAttributes,
           FileMode dwCreationDisposition,
           FileAttributes dwFlagsAndAttributes,
           IntPtr hTemplateFile);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool ReadFile(
            IntPtr hFile,
            byte[] lpBuffer,
            int nNumberOfBytesToRead,
            out int lpNumberOfBytesRead,
            IntPtr lpOverlapped);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool CloseHandle(IntPtr hObject);

        private void Form1_Load(object sender, EventArgs e)
        {
            // Load all available drives into the comboBox
            DriveInfo[] drives = DriveInfo.GetDrives();
            foreach (var drive in drives)
            {
                if (drive.IsReady) // Check if drive is ready
                {
                    comboBoxDrives.Items.Add(drive.Name);
                }
            }
        }
        byte[] key = null;
        byte[] iv = null;
        string encryptedPath = string.Empty;
        string imageName= $"DiskImage_{Guid.NewGuid()}";
        //string imageName = $"DiskImage_191682a5-eae8-428e-9b25-ba35bc113ea8";
        private void btnStartImaging_Click(object sender, EventArgs e)
        {
            if (comboBoxDrives.SelectedItem == null)
            {
                MessageBox.Show("Please select a drive.");
                return;
            }
            //string uniqueID = Guid.NewGuid().ToString();
            string drivePath = @"\\.\" + comboBoxDrives.SelectedItem.ToString().TrimEnd('\\');
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string outputPath = Path.Combine(desktopPath, $"{imageName}.img");

            try
            {
                // Create the disk image
                ReadRawDisk(drivePath, outputPath);

                string proprietaryPath = Path.Combine(desktopPath, $"{imageName}.propimg");
                CreateProprietaryFormat(outputPath, proprietaryPath);
                // Compress the image
                string compressedPath = proprietaryPath + ".gz";
                CompressImage(proprietaryPath, compressedPath);

                // Encrypt the image
                string encryptedPath = compressedPath + ".enc";
                key = GenerateRandomKey();
                iv = GenerateRandomIV();
                EncryptImage(compressedPath, encryptedPath, key, iv);

                MessageBox.Show("Disk imaging completed successfully!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
            }
        }
        private void CreateProprietaryFormat(string rawImagePath, string proprietaryPath)
        {
            using (FileStream rawImageStream = new FileStream(rawImagePath, FileMode.Open, FileAccess.Read))
            using (FileStream proprietaryStream = new FileStream(proprietaryPath, FileMode.Create, FileAccess.Write))
            using (BinaryWriter writer = new BinaryWriter(proprietaryStream))
            {
                // Add header information (metadata)
                writer.Write("MYPROPFMT"); // Magic identifier for proprietary format
                writer.Write(DateTime.UtcNow.ToString("o")); // Image creation date
                writer.Write("HDD/SDD"); // Disk type (modify based on your needs)
                writer.Write(Guid.NewGuid().ToString()); // Unique image ID
                writer.Write("1.0"); // Format version
                writer.Write(0); // Placeholder for compression level (to be updated after compression)

                // Copy the raw image data to the proprietary format file
                rawImageStream.CopyTo(proprietaryStream);
            }
        }
          
        private void ReadRawDisk(string drivePath, string outputPath)
        {
            IntPtr handle = CreateFile(
                drivePath,
                FileAccess.Read,
                FileShare.ReadWrite,
                IntPtr.Zero,
                FileMode.Open,
                0,
                IntPtr.Zero);

            if (handle == IntPtr.Zero || handle == new IntPtr(-1))
            {
                throw new IOException("Unable to access drive", Marshal.GetLastWin32Error());
            }

            byte[] buffer = new byte[4096]; // Read in chunks of 4096 bytes.
            int bytesRead;
            long totalBytesRead = 0;

            // Get drive size
            DriveInfo driveInfo = new DriveInfo(drivePath.Substring(4)); // Extract the drive letter
            long driveSize = driveInfo.TotalSize;

            using (FileStream outputFile = File.Create(outputPath))
            {
                while (totalBytesRead < driveSize)
                {
                    bool success = ReadFile(handle, buffer, buffer.Length, out bytesRead, IntPtr.Zero);
                    if (!success || bytesRead == 0)
                    {
                        // Stop if there's an error or no more bytes are read (EOF).
                        break;
                    }

                    outputFile.Write(buffer, 0, bytesRead);
                    totalBytesRead += bytesRead;
                }
            }

            CloseHandle(handle);
        }

        private void CompressImage(string inputPath, string outputPath)
        {
            using (FileStream inputFile = new FileStream(inputPath, FileMode.Open, FileAccess.Read))
            using (FileStream outputFile = new FileStream(outputPath, FileMode.Create, FileAccess.Write))
            using (System.IO.Compression.GZipStream gzipStream = new System.IO.Compression.GZipStream(outputFile, System.IO.Compression.CompressionLevel.Optimal))
            {
                inputFile.CopyTo(gzipStream);
            }
        }

        private void EncryptImage(string inputPath, string outputPath, byte[] key, byte[] iv)
        {
            using (FileStream inputFile = new FileStream(inputPath, FileMode.Open, FileAccess.Read))
            using (FileStream outputFile = new FileStream(outputPath, FileMode.Create, FileAccess.Write))
            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;
                using (CryptoStream cryptoStream = new CryptoStream(outputFile, aes.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    inputFile.CopyTo(cryptoStream);
                }
            }
        }        
        private byte[] GenerateRandomKey()
        {
            using (Aes aes = Aes.Create())
            {
                aes.GenerateKey();
                return aes.Key;
            }
        }

        private byte[] GenerateRandomIV()
        {
            using (Aes aes = Aes.Create())
            {
                aes.GenerateIV();
                return aes.IV;
            }
        }
    }
}