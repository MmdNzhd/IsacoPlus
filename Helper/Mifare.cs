using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using KaraYadak.ViewModels;

namespace KaraYadak.Helper
{
    public class Mifare
    {
        [DllImport("kernel32.dll")]
        static extern void Sleep(int dwMilliseconds);

        [DllImport("MasterRD.dll")]
        static extern int lib_ver(ref uint pVer);

        [DllImport("MasterRD.dll")]
        static extern int rf_init_com(int port, int baud);

        [DllImport("MasterRD.dll")]
        static extern int rf_ClosePort();

        [DllImport("MasterRD.dll")]
        static extern int rf_beep(short icdev, byte delay);

        [DllImport("MasterRD.dll")]
        static extern int rf_light(short icdev, byte Ledcolor);

        [DllImport("MasterRD.dll")]
        static extern int rf_antenna_sta(short icdev, byte mode);

        [DllImport("MasterRD.dll")]
        static extern int rf_init_type(short icdev, byte type);

        [DllImport("MasterRD.dll")]
        static extern int rf_request(short icdev, byte mode, ref ushort pTagType);

        [DllImport("MasterRD.dll")]
        static extern int rf_anticoll(short icdev, byte bcnt, IntPtr pSnr, ref byte pRLength);

        [DllImport("MasterRD.dll")]
        static extern int rf_select(short icdev, IntPtr pSnr, byte srcLen, ref sbyte Size);

        [DllImport("MasterRD.dll")]
        static extern int rf_halt(short icdev);

        [DllImport("MasterRD.dll")]
        static extern int rf_M1_authentication2(short icdev, byte mode, byte secnr, IntPtr key);

        [DllImport("MasterRD.dll")]
        static extern int rf_M1_initval(short icdev, byte adr, Int32 value);

        [DllImport("MasterRD.dll")]
        static extern int rf_M1_increment(short icdev, byte adr, Int32 value);

        [DllImport("MasterRD.dll")]
        static extern int rf_M1_decrement(short icdev, byte adr, Int32 value);

        [DllImport("MasterRD.dll")]
        static extern int rf_M1_readval(short icdev, byte adr, ref Int32 pValue);

        [DllImport("MasterRD.dll")]
        static extern int rf_M1_read(short icdev, byte adr, IntPtr pData, ref byte pLen);

        [DllImport("MasterRD.dll")]
        static extern int rf_M1_write(short icdev, byte adr, IntPtr pData);

        [DllImport("MasterRD.dll")]
        static extern int rf_ul_select(short icdev, IntPtr pSnr, ref byte pRLength);

        [DllImport("MasterRD.dll")]
        static extern int rf_ul_write(short icdev, byte page, IntPtr pData);


        //
         bool bConnectedDevice;/*是否连接上设备*/

        static char[] hexDigits = {
            '0','1','2','3','4','5','6','7',
            '8','9','A','B','C','D','E','F'};

        public static byte GetHexBitsValue(byte ch)
        {
            byte sz = 0;
            if (ch <= '9' && ch >= '0')
                sz = (byte)(ch - 0x30);
            if (ch <= 'F' && ch >= 'A')
                sz = (byte)(ch - 0x37);
            if (ch <= 'f' && ch >= 'a')
                sz = (byte)(ch - 0x57);

            return sz;
        }
        //

        #region byteHEX
        /// <summary>
        /// 单个字节转字字符.
        /// </summary>
        /// <param name="ib">字节.</param>
        /// <returns>转换好的字符.</returns>
        public static String byteHEX(Byte ib)
        {
            String _str = String.Empty;
            try
            {
                char[] Digit = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A',
                'B', 'C', 'D', 'E', 'F' };
                char[] ob = new char[2];
                ob[0] = Digit[(ib >> 4) & 0X0F];
                ob[1] = Digit[ib & 0X0F];
                _str = new String(ob);
            }
            catch (Exception)
            {
                new Exception("对不起有错。");
            }
            return _str;

        }
        #endregion

        public static string ToHexString(byte[] bytes)
        {
            String hexString = String.Empty;
            for (int i = 0; i < bytes.Length; i++)
                hexString += byteHEX(bytes[i]);

            return hexString;
        }



        public static byte[] ToDigitsBytes(string theHex)
        {
            byte[] bytes = new byte[theHex.Length / 2 + (((theHex.Length % 2) > 0) ? 1 : 0)];
            for (int i = 0; i < bytes.Length; i++)
            {
                char lowbits = theHex[i * 2];
                char highbits;

                if ((i * 2 + 1) < theHex.Length)
                    highbits = theHex[i * 2 + 1];
                else
                    highbits = '0';

                int a = (int)GetHexBitsValue((byte)lowbits);
                int b = (int)GetHexBitsValue((byte)highbits);
                bytes[i] = (byte)((a << 4) + b);
            }

            return bytes;
        }


        //
        public Mifare()
        {
            bConnectedDevice = false;
        }
        static int port = 4;
        static int baud = 115200;
        static string SearchPurse;
        static string data1;
        static string data2;
        static string data3;
        static string txtKeyA2 = "";
        static string txtKeyB2 = "";
        static string txtKey2 = "";
        static string txtInputKey2 = "FFFFFFFFFFFF";
        public  CartResponseVm ConnectDevice()
        {
            if (!bConnectedDevice)
            {
                int status;


                status = rf_init_com(port, baud);
                if (0 == status)
                {
                    bConnectedDevice = true;
                    return new  CartResponseVm{ Status = "1", Message = " اتصال بر قرار شد" };

                }
                else
                {
                    bConnectedDevice = false;
                    return new CartResponseVm {Status = "0", Message = " کانکشن های دستگاه را چک کنید" };

                }
            }
            return new CartResponseVm { Status = "1", Message = " متصل است!" };
        }
        public  CartResponseVm ConnectCart()
        {
            short icdev = 0x0000;
            int status=200;
            // byte type = (byte)'A';//mifare one type is A 卡询卡方式为A
            byte mode = 0x26;  // Request the card which is not halted.
            ushort TagType = 0;
            byte bcnt = 0x04;//mifare 卡都用4, hold on 4
            IntPtr pSnr;
            byte len = 255;
            sbyte size = 0;


            if (!bConnectedDevice)
            {


                return new CartResponseVm { Status = "0", Message = "دستگاه متصل نیست" };
            }

            pSnr = Marshal.AllocHGlobal(1024);

            for (int i = 0; i < 2; i++)
            {

                status = rf_request(icdev, mode, ref TagType);//搜寻没有休眠的卡，request card  
                if (status != 0)
                    continue;

                status = rf_anticoll(icdev, bcnt, pSnr, ref len);//防冲突得到返回卡的序列号, anticol--get the card sn
                if (status != 0)
                    continue;

                status = rf_select(icdev, pSnr, len, ref size);//锁定一张ISO14443-3 TYPE_A 卡, select one card
                if (status != 0)
                    continue;

                byte[] szBytes = new byte[len];

                for (int j = 0; j < len; j++)
                {
                    szBytes[j] = Marshal.ReadByte(pSnr, j);
                }

                String m_cardNo = String.Empty;

                for (int q = 0; q < len; q++)
                {
                    m_cardNo += byteHEX(szBytes[q]);
                }
                SearchPurse = m_cardNo;

                break;
            }

            Marshal.FreeHGlobal(pSnr);
            if (status == 0)
            {
                return new CartResponseVm { Status = "1", Message = "وصل است!" };

            }
            else
            {
                return new CartResponseVm { Status = "0", Message = "کارت را وصل کنید" };
            }

        }
        public  CartResponseVm ReqIdi()
        {
            short icdev = 0x0000;
            byte mode = 0x52;  //this mode will active any card in the rf field includin the card which was halted. 这个模式会激活包括被休眠的卡
            ushort TagType = 0;
            int status;
            byte bcnt = 0x04;//mifare code 卡都用4
            IntPtr pSnr;
            byte len = 255;
            sbyte size = 0;

            if (!bConnectedDevice)
            {
                return new CartResponseVm { Status = "0", Message = "دستگاه متصل نیست" };
            }

            status = rf_request(icdev, mode, ref TagType);//搜寻卡  Request
            if (status != 0)
            {
                return new CartResponseVm { Status = "0", Message = "دستگاه متصل نیست" };


            }

            pSnr = Marshal.AllocHGlobal(1024);
            status = rf_anticoll(icdev, bcnt, pSnr, ref len);//返回卡的序列号  anticol-- get the card's sn
            if (status != 0)
            {
                Marshal.FreeHGlobal(pSnr);
                return new CartResponseVm { Status = "0", Message ="  مشکلی پیش آمده است"};

            }

            status = rf_select(icdev, pSnr, len, ref size);//锁定一张ISO14443-3 TYPE_A 卡  select one card
            if (status != 0)
            {
                Marshal.FreeHGlobal(pSnr);
                return new CartResponseVm { Status = "0", Message = "مشکلی پیش آمده است" };


            }

            byte[] szBytes = new byte[len];

            for (int j = 0; j < len; j++)
            {
                szBytes[j] = Marshal.ReadByte(pSnr, j);
            }

            SearchPurse = ToHexString(szBytes);
            Marshal.FreeHGlobal(pSnr);
            return new CartResponseVm { Status = "1", Message ="با موفقیت انجام شد"};


        }
        public  CartResponseVm Read()
        {
            short icdev = 0x0000;
            int status;
            byte mode = 0x60;
            byte secnr = 0x00;

            if (!bConnectedDevice)
            {
                return new CartResponseVm { Status = "0", Message = "دستگاه متصل نیست" };

            }




            secnr = 10;

            IntPtr keyBuffer = Marshal.AllocHGlobal(256);

            byte[] bytesKey = ToDigitsBytes("FFFFFFFFFFFF");
            for (int i = 0; i < bytesKey.Length; i++)
                Marshal.WriteByte(keyBuffer, i * Marshal.SizeOf(typeof(Byte)), bytesKey[i]);
            status = rf_M1_authentication2(icdev, mode, (byte)(secnr * 4), keyBuffer);
            Marshal.FreeHGlobal(keyBuffer);

            IntPtr dataBuffer = Marshal.AllocHGlobal(256);
            for (int i = 0; i < 4; i++)    // read 4 blocks in the secotor
            {
                int j;
                byte cLen = 0;
                status = rf_M1_read(icdev, (byte)((secnr * 4) + i), dataBuffer, ref cLen);

                if (status != 0 || cLen != 16)
                {
                    return new CartResponseVm { Status = "0", Message = " عملیات با خطا مواجه شده است لطفا دستگاه را ریست کنید"  };

                }

                byte[] bytesData = new byte[16];
                for (j = 0; j < bytesData.Length; j++)
                    bytesData[j] = Marshal.ReadByte(dataBuffer, j);

                if (i == 0)
                    data1 = ToHexString(bytesData);
                else if (i == 1)
                    data2 = ToHexString(bytesData);
                else if (i == 2)
                    data3 = ToHexString(bytesData);
                else if (i == 3)
                {
                    byte[] byteskeyA = new byte[6];
                    byte[] byteskey = new byte[4];
                    byte[] byteskeyB = new byte[6];

                    for (j = 0; j < 16; j++)
                    {
                        if (j < 6)
                            byteskeyA[j] = bytesData[j];
                        else if (j >= 6 && j < 10)
                            byteskey[j - 6] = bytesData[j];
                        else
                            byteskeyB[j - 10] = bytesData[j];
                    }

                    txtKeyA2 = ToHexString(byteskeyA);
                    txtKey2 = ToHexString(byteskey);
                    txtKeyB2 = ToHexString(byteskeyB);
                }
            }
            Marshal.FreeHGlobal(dataBuffer);
            return new CartResponseVm { Status = "1", PhoneNumber = data1.Replace("A", "") };

        }
        public  CartResponseVm Write(string userPhoneNumber)
        {
            short icdev = 0x0000;
            int status;
            byte mode = 0x60;
            byte secnr = 0x00;
            byte adr;
            int i;


            if (!bConnectedDevice)
            {
                return new CartResponseVm { Status = "0", Message = "دستگاه متصل نیست" };

            }


            secnr = Convert.ToByte(10);
            adr = (byte)(Convert.ToByte(0) + secnr * 4);


            IntPtr keyBuffer = Marshal.AllocHGlobal(1024);

            byte[] bytesKey = ToDigitsBytes(txtInputKey2);
            for (i = 0; i < bytesKey.Length; i++)
                Marshal.WriteByte(keyBuffer, i, bytesKey[i]);
            status = rf_M1_authentication2(icdev, mode, (byte)(secnr * 4), keyBuffer);
            Marshal.FreeHGlobal(keyBuffer);


            //
            byte[] bytesBlock;

            var data = userPhoneNumber;
            var counter = data.Length;
            while (counter < 32)
            {
                data += "A";
                counter++;
            }
            bytesBlock = ToDigitsBytes(data);


            IntPtr dataBuffer = Marshal.AllocHGlobal(1024);

            for (i = 0; i < bytesBlock.Length; i++)
                Marshal.WriteByte(dataBuffer, i, bytesBlock[i]);
            status = rf_M1_write(icdev, adr, dataBuffer);
            Marshal.FreeHGlobal(dataBuffer);

            if (status != 0)
            {
                return new CartResponseVm { Status = "0", Message =   "  مشکلی پیش آمده است" };

            }
               
            else return new CartResponseVm { Status = "1", Message =  "با موفقیت ثبت شده است" };

        }
    }
}
