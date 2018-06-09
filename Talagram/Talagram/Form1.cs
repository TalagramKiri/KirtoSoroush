using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Talagram
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private static String DecryptIt(String s, byte[] key, byte[] IV)
        {
            String result;

            RijndaelManaged rijn = new RijndaelManaged();
            rijn.Mode = CipherMode.CBC;
            rijn.Padding = PaddingMode.Zeros;

            using (MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(s)))
            {
                using (ICryptoTransform decryptor = rijn.CreateDecryptor(key, IV))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader swDecrypt = new StreamReader(csDecrypt))
                        {
                            result = swDecrypt.ReadToEnd();
                        }
                    }
                }
            }
            rijn.Clear();

            return result;
        }

        private async void button1_ClickAsync(object sender, EventArgs e)
        {
            var info = await GetInfo();
            Encoding byteEncoder = Encoding.Default;

            byte[] rijnKey = byteEncoder.GetBytes("KCH@LQj#>6VCqqLg");
            byte[] rijnIV = byteEncoder.GetBytes("YC'2bmK=b%#NQ?9j");
            string proxyjson = DecryptIt(info.data[0], rijnKey, rijnIV);

            int index = proxyjson.IndexOf("}");
            index++;
            if (index > 0)
                proxyjson = proxyjson.Substring(0, index);


            var cls = JsonConvert.DeserializeObject<Proxy>(proxyjson);

            textBox1.Text = cls.ip;
            textBox2.Text = cls.prt.ToString();
            textBox3.Text = cls.usr;
            textBox4.Text = cls.pwd;
            textBox5.Text = "https://t.me/socks?server=" + cls.ip + "&port=" + cls.prt + "&user=" + cls.usr + "&pass=" + cls.pwd;
        }
        public async Task<INFO> GetInfo()
        {
            return await Task.Run(() =>
            {
                try
                {
                    string url = "http://lh58.hotgram.ir/v1/proxy?slt=77799000&appId=3";
                    HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
                    string json = "";
                    using (HttpWebResponse response = req.GetResponse() as HttpWebResponse)
                    {
                        StreamReader reader = new StreamReader(response.GetResponseStream());
                        json = reader.ReadToEnd();
                    }
                    INFO cls = JsonConvert.DeserializeObject<INFO>(json);
                    return cls;
                }
                catch (Exception ex)
                {
                    INFO iNFO = new INFO();
                    return iNFO;
                }
            });
        }

        public class INFO
        {
            public List<string> data { get; set; }
        }

        public class Proxy
        {
            public string ip { get; set; }
            public int prt { get; set; }
            public string usr { get; set; }
            public string pwd { get; set; }
            public int ttl { get; set; }
        }
    }
}
