using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using System.Runtime.Serialization.Formatters.Binary;

namespace make_pkg
{
    class Program
    {
        public static string cid = "UP0001-TEST00000_00-0000000000000000";
        public static byte[] testhead = new byte[] { 0x12, 0xAC, 0x90, 0x16, 0x2B, 0xDB, 0xE0, 0x1E, 0x1A, 0x91, 0x8B, 0xC0, 0x13, 0x71, 0xBA, 0xDF };
        public static byte[] Header2 = new byte[0x80];
        
        public static byte[] contentID = new byte[0x30];
        public static byte[] QADigest = new byte[0x10];
        public static byte[] KLicensee = new byte[0x10];

        public static byte[][] filename_offset;
        public static byte[][] filename_size;
        public static byte[][] data_offset;
        public static byte[][] data_size;
        public static byte[][] flags;
        public static byte[][] padding;
        public static byte[][] names;

        static byte[] key = new byte[0x40];
        public static string[] list;
        public static int[] name_size;
        public static long[] offset;
        public static long edata_size;

        public static int num;
        public static int starta = 0;

        public static SHA1Managed dsha = new SHA1Managed();






        static void Main(string[] args)
        {

            set_list();

            // make_list();
            setup();
            make_file();

        }

        public static void usage()
        {

            Console.WriteLine("");
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();

        }

        public static void set_header(Header hd)
        {
            //Header2 = new byte[0xC0];
            Header header = hd;
            MemoryStream memStream = new MemoryStream(Header2);

            memStream.Write(header.magic, 0, header.magic.Length);
            memStream.Write(header.type, 0, header.type.Length);
            memStream.Write(header.pkgInfoOff, 0, header.pkgInfoOff.Length);
            memStream.Write(header.unk1, 0, header.unk1.Length);
            memStream.Write(header.headSize, 0, header.headSize.Length);
            memStream.Write(header.itemCount, 0, header.itemCount.Length);
            memStream.Write(header.packageSize, 0, header.packageSize.Length);
            memStream.Write(header.dataOff, 0, header.dataOff.Length);
            memStream.Write(header.dataSize, 0, header.dataSize.Length);

            memStream.Write(header.contentID, 0, header.contentID.Length);
            memStream.Write(header.QADigest, 0, header.QADigest.Length);
            memStream.Write(header.KLicensee, 0, header.KLicensee.Length);


            memStream.Close();

        }

        public static void make_header()
        {
            byte[] magic = { 0x7F, 0x50, 0x4B, 0x47 };
            byte[] type = { 0x00, 0x00, 0x00, 0x01 };
            byte[] pkgInfoOff = { 0x00, 0x00, 0x00, 0xC0 };
            byte[] unk1 = { 0x00, 0x00, 0x00, 0x05 };
            byte[] headSize = { 0x00, 0x00, 0x00, 0x80 };
            byte[] itemCount = new byte[4];// 
            itemCount = BitConverter.GetBytes(num);
            Array.Reverse(itemCount);
            byte[] packageSize = new byte[8];// 
            packageSize = BitConverter.GetBytes(edata_size + 0x1A0); //{ 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            Array.Reverse(packageSize);
            byte[] dataOff = { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x40 };
            byte[] dataSize = new byte[8];// 
            dataSize = BitConverter.GetBytes(edata_size); // { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            Array.Reverse(dataSize);


            MemoryStream memStream = new MemoryStream(Header2);

            memStream.Write(magic, 0, magic.Length);
            memStream.Write(type, 0, type.Length);
            memStream.Write(pkgInfoOff, 0, pkgInfoOff.Length);
            memStream.Write(unk1, 0, unk1.Length);
            memStream.Write(headSize, 0, headSize.Length);
            memStream.Write(itemCount, 0, itemCount.Length);
            memStream.Write(packageSize, 0, packageSize.Length);
            memStream.Write(dataOff, 0, dataOff.Length);
            memStream.Write(dataSize, 0, dataSize.Length);

            memStream.Write(contentID, 0, contentID.Length);
            memStream.Write(QADigest, 0, QADigest.Length);
            memStream.Write(KLicensee, 0, KLicensee.Length);


            memStream.Close();
        }
       
        public static byte[] make_MetaHeader(MetaHeader mh)
        {
            
            /*
            byte[] unk1 = { 0x00, 0x00, 0x00, 0x01 };
            byte[] unk2 = { 0x00, 0x00, 0x00, 0x04 };
            byte[] drmType = { 0x00, 0x00, 0x00, 0x03 };
            byte[] unk4 = { 0x00, 0x00, 0x00, 0x02 };

            byte[] unk21 = { 0x00, 0x00, 0x00, 0x04 };
            byte[] unk22 = { 0x00, 0x00, 0x00, 0x03 };
            byte[] unk23 = { 0x00, 0x00, 0x00, 0x03 };
            byte[] unk24 = { 0x00, 0x00, 0x00, 0x04 };

            byte[] unk31 = { 0x00, 0x00, 0x00, 0x0E };
            byte[] unk32 = { 0x00, 0x00, 0x00, 0x04 };
            byte[] unk33 = { 0x00, 0x00, 0x00, 0x08 };
            byte[] secondaryVersion = { 0x00, 0x00 };
            byte[] unk34 = { 0x00, 0x00 };

            byte[] dataSize = new byte[4];// 
            dataSize = BitConverter.GetBytes(edata_size);
            Array.Reverse(dataSize);//{ 0x00, 0x00, 0x00, 0x00 };
            byte[] unk42 = { 0x00, 0x00, 0x00, 0x05 };
            byte[] unk43 = { 0x00, 0x00, 0x00, 0x04 };
            byte[] packagedBy = { 0x10, 0x61 };
            byte[] packageVersion = { 0x00, 0x00 };*/
            byte[] MetaHeader2 = new byte[0x40];
            MetaHeader metaHeader = mh;

            MemoryStream memStream = new MemoryStream(MetaHeader2);

            memStream.Write(metaHeader.unk1, 0, metaHeader.unk1.Length);
            memStream.Write(metaHeader.unk2, 0, metaHeader.unk2.Length);
            memStream.Write(metaHeader.drmType, 0, metaHeader.drmType.Length);
            memStream.Write(metaHeader.unk4, 0, metaHeader.unk4.Length);

            memStream.Write(metaHeader.unk21, 0, metaHeader.unk21.Length);
            memStream.Write(metaHeader.unk22, 0, metaHeader.unk22.Length);
            memStream.Write(metaHeader.unk23, 0, metaHeader.unk23.Length);
            memStream.Write(metaHeader.unk24, 0, metaHeader.unk24.Length);

            memStream.Write(metaHeader.unk31, 0, metaHeader.unk31.Length);
            memStream.Write(metaHeader.unk32, 0, metaHeader.unk32.Length);
            memStream.Write(metaHeader.unk33, 0, metaHeader.unk33.Length);
            memStream.Write(metaHeader.secondaryVersion, 0, metaHeader.secondaryVersion.Length);
            memStream.Write(metaHeader.unk34, 0, metaHeader.unk34.Length);

            memStream.Write(metaHeader.dataSize, 0, 4);
            memStream.Write(metaHeader.unk42, 0, metaHeader.unk42.Length);
            memStream.Write(metaHeader.unk43, 0, metaHeader.unk43.Length);
            memStream.Write(metaHeader.packagedBy, 0, metaHeader.packagedBy.Length);
            memStream.Write(metaHeader.packageVersion, 0, metaHeader.packageVersion.Length);

            memStream.Close();

            return MetaHeader2;
        }
        
        public static void set_list()
        {
            int i = 0;
            foreach (string file in Directory.EnumerateFiles("PS3_GAME",
            "*.*",
            SearchOption.TopDirectoryOnly))
            {
                // Display file path.
                int tempi = file.Replace("PS3_GAME\\", "").Length;
                if ((tempi % 16) != 0)
                {
                    while ((tempi % 16) != 0)
                    {

                        tempi++;
                    }
                }
                starta = starta + tempi;
                i++;

            }

            foreach (string file in Directory.GetDirectories("PS3_GAME",
            "*.*",
            SearchOption.AllDirectories))
            {
                int tempi = file.Replace("PS3_GAME\\", "").Length;
                if ((tempi % 16) != 0)
                {
                    while ((tempi % 16) != 0)
                    {

                        tempi++;
                    }
                }
                starta = starta + tempi;
                i++;
                foreach (string file2 in Directory.EnumerateFiles(file,
            "*.*",
            SearchOption.TopDirectoryOnly))
                {
                    int tempi2 = file2.Replace("PS3_GAME\\", "").Length;

                    if ((tempi2 % 16) != 0)
                    {
                        while ((tempi2 % 16) != 0)
                        {

                            tempi2++;
                        }
                    }
                    starta = starta + tempi2;
                    i++;

                }

            }
            num = i;
        }

        public static void setup()
        {
            int x = num * 32;
            int name_offset = x;
            long data_offset1 = x + starta;

            filename_offset = new byte[num][];
            filename_size = new byte[num][];
            data_offset = new byte[num][];
            data_size = new byte[num][];
            flags = new byte[num][];
            padding = new byte[num][];
            names = new byte[num][];

            list = new string[num];
            name_size = new int[num];
            offset = new long[num];

            int i = 0;

            foreach (string file in Directory.EnumerateFiles("PS3_GAME",
            "PS3LOGO.DAT",
            SearchOption.TopDirectoryOnly))
            {
                if (file.Replace("PS3_GAME\\", "") == "PS3LOGO.DAT")
                {
                    FileInfo f = new FileInfo(file);
                    long s1 = f.Length;
                    // Display file path.

                    Console.WriteLine(file.Replace("PS3_GAME\\", ""));
                    Console.WriteLine(file.Replace("PS3_GAME\\", "").Length);
                    list[i] = file.Replace("PS3_GAME\\", "");
                    list[i] = list[i].Replace("\\", "/");
                    name_size[i] = file.Replace("PS3_GAME\\", "").Length;


                    int tempi = name_size[i];
                    if ((tempi % 16) != 0)
                    {
                        while ((tempi % 16) != 0)
                        {

                            tempi++;
                        }
                    }

                    

                    filename_offset[i] = new byte[4];
                    filename_size[i] = new byte[4];
                    data_offset[i] = new byte[8];
                    data_size[i] = new byte[8];
                    flags[i] = new byte[4];
                    padding[i] = new byte[4];
                    names[i] = new byte[tempi];

                    names[i] = Encoding.ASCII.GetBytes(list[i].PadRight(tempi, '\0'));

                    byte[] temp_filename_offset = BitConverter.GetBytes(name_offset);
                    Array.Copy(temp_filename_offset, filename_offset[i], 4);
                    Array.Reverse(filename_offset[i]);

                    byte[] temp_filename_size = BitConverter.GetBytes(name_size[i]);
                    Array.Copy(temp_filename_size, filename_size[i], 4);
                    Array.Reverse(filename_size[i]);

                    byte[] temp_data_offset = new byte[8];
                    temp_data_offset = BitConverter.GetBytes(data_offset1);
                    Array.Copy(temp_data_offset, data_offset[i], 8);
                    Array.Reverse(data_offset[i]);

                    byte[] temp_data_size = BitConverter.GetBytes(s1);
                    Array.Copy(temp_data_size, data_size[i], 8);
                    Array.Reverse(data_size[i]);

                    byte[] tempflags = new byte[] { 0x80, 0x00, 0x00, 0x03 };
                    Array.Copy(tempflags, flags[i], 4);

                    if (BitConverter.ToString(names[i]) == "USRDIR/EBOOT.BIN")
                    {
                        tempflags = new byte[] { 0x80, 0x00, 0x00, 0x01 };
                        Array.Copy(tempflags, flags[i], 4);

                        s1 = ((s1 - 0x30 + 63) & ~63) + 0x30;
                        temp_data_size = BitConverter.GetBytes(s1);
                        Array.Copy(temp_data_size, data_size[i], 8);
                        Array.Reverse(data_size[i]);

                    }
                    offset[i] = data_offset1;
                    name_offset = name_offset + tempi;

                    long tempp = s1;
                    if ((tempp % 16) != 0)
                    {
                        while ((tempp % 16) != 0)
                        {

                            tempp++;
                        }
                    }

                    data_offset1 = data_offset1 + Convert.ToInt32(tempp);
                    i++;
                    edata_size += tempp;
                }




            }

            foreach (string file in Directory.GetDirectories("PS3_GAME",
           "*.*",
           SearchOption.AllDirectories))
            {

                // Display file path.
                Console.WriteLine(file.Replace("PS3_GAME\\", ""));
                list[i] = file.Replace("PS3_GAME\\", "");
                list[i] = list[i].Replace("\\", "/");
                name_size[i] = file.Replace("PS3_GAME\\", "").Length;

                int tempi = name_size[i];
                if ((tempi % 16) != 0)
                {
                    while ((tempi % 16) != 0)
                    {

                        tempi++;
                    }
                }


                filename_offset[i] = new byte[4];
                filename_size[i] = new byte[4];
                data_offset[i] = new byte[8];
                data_size[i] = new byte[8];
                flags[i] = new byte[4];
                padding[i] = new byte[4];
                names[i] = new byte[tempi];

                names[i] = Encoding.ASCII.GetBytes(list[i].PadRight(tempi, '\0'));

                byte[] temp_filename_offset = BitConverter.GetBytes(name_offset);
                Array.Copy(temp_filename_offset, filename_offset[i], 4);
                Array.Reverse(filename_offset[i]);

                byte[] temp_filename_size = BitConverter.GetBytes(name_size[i]);
                Array.Copy(temp_filename_size, filename_size[i], 4);
                Array.Reverse(filename_size[i]);

                byte[] temp_data_offset = new byte[8];
                temp_data_offset = BitConverter.GetBytes(data_offset1);
                Array.Copy(temp_data_offset, data_offset[i], 8);
                Array.Reverse(data_offset[i]);



                byte[] tempflags = new byte[] { 0x80, 0x00, 0x00, 0x04 };
                Array.Copy(tempflags, flags[i], 4);
                offset[i] = data_offset1;
                name_offset = name_offset + tempi;
                i++;
            }

            foreach (string file in Directory.EnumerateFiles("PS3_GAME",
           "*.*",
           SearchOption.AllDirectories))
            {
                if (file.Replace("PS3_GAME\\", "") != "PS3LOGO.DAT")
                {
                    FileInfo f = new FileInfo(file);
                    long s1 = f.Length;
                    // Display file path.


                    Console.WriteLine(file.Replace("PS3_GAME\\", ""));
                    Console.WriteLine(file.Replace("PS3_GAME\\", "").Length);
                    list[i] = file.Replace("PS3_GAME\\", "");
                    list[i] = list[i].Replace("\\", "/");
                    name_size[i] = file.Replace("PS3_GAME\\", "").Length;

                    int tempi = name_size[i];
                    if ((tempi % 16) != 0)
                    {
                        while ((tempi % 16) != 0)
                        {

                            tempi++;
                        }
                    }

                    long tempp = s1;
                    if ((tempp % 16) != 0)
                    {
                        while ((tempp % 16) != 0)
                        {

                            tempp++;
                        }
                    }

                    filename_offset[i] = new byte[4];
                    filename_size[i] = new byte[4];
                    data_offset[i] = new byte[8];
                    data_size[i] = new byte[8];
                    flags[i] = new byte[4];
                    padding[i] = new byte[4];
                    names[i] = new byte[tempi];

                    names[i] = Encoding.ASCII.GetBytes(list[i].PadRight(tempi, '\0'));

                    byte[] temp_filename_offset = BitConverter.GetBytes(name_offset);
                    Array.Copy(temp_filename_offset, filename_offset[i], 4);
                    Array.Reverse(filename_offset[i]);

                    byte[] temp_filename_size = BitConverter.GetBytes(name_size[i]);
                    Array.Copy(temp_filename_size, filename_size[i], 4);
                    Array.Reverse(filename_size[i]);

                    byte[] temp_data_offset = new byte[8];
                    temp_data_offset = BitConverter.GetBytes(data_offset1);
                    Array.Copy(temp_data_offset, data_offset[i], 8);
                    Array.Reverse(data_offset[i]);

                    byte[] temp_data_size = BitConverter.GetBytes(s1);
                    Array.Copy(temp_data_size, data_size[i], 8);
                    Array.Reverse(data_size[i]);

                    byte[] tempflags = new byte[] { 0x80, 0x00, 0x00, 0x03 };
                    Array.Copy(tempflags, flags[i], 4);
                    offset[i] = data_offset1;
                    name_offset = name_offset + tempi;
                    data_offset1 = data_offset1 + Convert.ToInt32(tempp);
                    i++;
                    edata_size += tempp;
                }

            }
            edata_size += x + starta;
        }

 

        public static void make_file()
        {
            int i = 0;
            using (BinaryWriter writer = new BinaryWriter(File.Open("test.pkg", FileMode.Create)))
            {
                //make_header();
                //writer.Write(Header2, 0, Header2.Length);
               
                Header header = new Header();
                header.itemCount = BitConverter.GetBytes(num);
                Array.Reverse(header.itemCount);

                header.packageSize = BitConverter.GetBytes(edata_size + 0x1A0); 
                Array.Reverse(header.packageSize);

                header.dataSize = BitConverter.GetBytes(edata_size); 
                Array.Reverse(header.dataSize);


                /*
                header.contentID = contentID;
                header.QADigest = QADigest;
                header.KLicensee = KLicensee; */
                //set_header(header);
                Header2 = header.Set_Header(header, Header2);
                header.Write_Header(header, writer, edata_size, num);


                // make_MetaHeader();
                MetaHeader metaBlock = new MetaHeader();
                 
                metaBlock.dataSize = BitConverter.GetBytes((int)edata_size);
                Array.Reverse(metaBlock.dataSize);

                metaBlock.Write_MetaHeader(metaBlock, writer);
                



                while (num - 1 >= i)
                {

                    writer.Seek(Convert.ToInt32(offset[i]) + 0x140, SeekOrigin.Begin);
                    if (File.Exists("PS3_GAME\\" + list[i]))
                    {
                        using (FileStream stream = File.OpenRead("PS3_GAME\\" + list[i]))
                        {
                            BinaryReader reader = new BinaryReader(stream);




                            byte[] headtest = new byte[9];
                            stream.Read(headtest, 0, 9);
                            if (BitConverter.ToString(headtest) == "SCE\0\0\0\0\x02\x80")
                            {

                                SHA1Managed filesha = new SHA1Managed();
                                stream.Seek(0, SeekOrigin.Begin);
                                byte[] buffer1 = new Byte[1024];
                                int bytesRead1;

                                // while the read method returns bytes
                                // keep writing them to the output stream
                                while ((bytesRead1 = stream.Read(buffer1, 0, 1024)) > 0)
                                {
                                    filesha.TransformBlock(buffer1, 0, bytesRead1, buffer1, 0);
                                }


                                byte[] fileSHA1 = filesha.Hash;

                                stream.Seek(0, SeekOrigin.Begin);
                                SelfHeader fselfheader = new SelfHeader();

                                fselfheader.load(stream);

                                AppInfo appheader = new AppInfo();

                                appheader.load(stream);
                                bool found = false;


                                long digestOff = BitConverter.ToInt64(fselfheader.digest, 0);

                                while (!found)
                                {
                                    DigestBlock digest = new DigestBlock();

                                    digest.Set_Header(stream);


                                    if (BitConverter.ToInt32(digest.type, 0) == 3)
                                    {
                                        found = true;
                                    }
                                    else
                                    {
                                        digestOff += BitConverter.ToInt64(digest.size, 0);
                                    }

                                    if (BitConverter.ToInt32(digest.isNext, 0) != 1)
                                    {
                                        break;
                                    }
                                    digestOff += BitConverter.ToInt32(digest.size, 0);
                                }

                                if (BitConverter.ToInt32(appheader.appType, 0) == 8 & found)
                                {
                                    //dataToEncrypt += fileData[0:digestOff];



                                    EbootMeta meta = new EbootMeta();

                                    byte[] mg = { 0x4E, 0x50, 44, 00 };
                                    meta.magic.SetValue(mg, 0);

                                    meta.unk1.SetValue(01, 0);

                                    meta.drmType = metaBlock.drmType;

                                    meta.unk2.SetValue(01, 0);


                                    meta.contentID = contentID;



                                    int x = 0;
                                    while (x < 0x10)
                                    {

                                        meta.fileSHA1[i] = fileSHA1[i];


                                        meta.notSHA1[i] = Convert.ToByte((~meta.fileSHA1[i]) & 0xFF);


                                        if (x == 0xF)
                                            meta.notXORKLSHA1[i] = Convert.ToByte((1 ^ meta.notSHA1[i] ^ 0xAA) & 0xFF);
                                        else
                                            meta.notXORKLSHA1[i] = Convert.ToByte((0 ^ meta.notSHA1[i] ^ 0xAA) & 0xFF);


                                        meta.nulls[i] = 0;
                                    }

                                    stream.Seek(0, SeekOrigin.Begin);
                                    byte[] buffer3 = new Byte[1024];
                                    int bytesRead3;

                                    // while the read method returns bytes
                                    // keep writing them to the output stream

                                    byte[] top = new byte[(int)digestOff];
                                    stream.Read(top, 0, (int)digestOff);
                                    dsha.TransformBlock(top, 0, (int)digestOff, top, 0);

                                    stream.Seek((int)digestOff, SeekOrigin.Begin);

                                    dsha.TransformBlock(meta.magic, 0, meta.magic.Length, meta.magic, 0);
                                    dsha.TransformBlock(meta.unk1, 0, meta.unk1.Length, meta.unk1, 0);
                                    dsha.TransformBlock(meta.drmType, 0, meta.drmType.Length, meta.drmType, 0);
                                    dsha.TransformBlock(meta.unk2, 0, meta.unk2.Length, meta.unk2, 0);
                                    dsha.TransformBlock(meta.contentID, 0, meta.contentID.Length, meta.contentID, 0);
                                    dsha.TransformBlock(meta.fileSHA1, 0, meta.fileSHA1.Length, meta.fileSHA1, 0);
                                    dsha.TransformBlock(meta.notSHA1, 0, meta.notSHA1.Length, meta.notSHA1, 0);
                                    dsha.TransformBlock(meta.notXORKLSHA1, 0, meta.notXORKLSHA1.Length, meta.notXORKLSHA1, 0);
                                    dsha.TransformBlock(meta.nulls, 0, meta.nulls.Length, meta.nulls, 0);

                                    stream.Seek((int)digestOff + 0x80, SeekOrigin.Begin);
                                    while ((bytesRead3 = stream.Read(buffer3, 0, 1024)) > 0)
                                    {
                                        dsha.TransformBlock(buffer3, 0, bytesRead3, buffer3, 0);
                                        writer.Write(buffer3, 0, bytesRead3);
                                    }

                                    //dataToEncrypt += meta.pack();


                                }



                            }


                            else
                            {

                                // create a buffer to hold the bytes 
                                stream.Seek(0, SeekOrigin.Begin);
                                byte[] buffer = new Byte[1024];
                                int bytesRead;

                                // while the read method returns bytes
                                // keep writing them to the output stream
                                while ((bytesRead =
                                        stream.Read(buffer, 0, 1024)) > 0)
                                {
                                    dsha.TransformBlock(buffer, 0, bytesRead, buffer, 0);
                                    writer.Write(buffer, 0, bytesRead);
                                }
                            }

                            reader.Close();
                            stream.Close();
                        }

                    }
                    //Console.WriteLine("FinalBlock   : {0}", BytesToStr(dsha.Hash));
                    i++;
                }
                byte[] pad = new Byte[0x60];
                

                writer.Seek(Convert.ToInt32(edata_size) + 0x140, SeekOrigin.Begin);
                writer.Write(pad, 0, pad.Length);
                dsha.TransformBlock(Header2, 0, 0x80, Header2, 0);

                i = 0;
                writer.Seek(0x140, SeekOrigin.Begin);
                int s = num * 32;
                s += starta;
                byte[] fileheader = new byte[s];
                MemoryStream memStream = new MemoryStream(fileheader);

                while (num - 1 >= i)
                {

                    memStream.Write(filename_offset[i], 0, filename_offset[i].Length);
                    memStream.Write(filename_size[i], 0, filename_size[i].Length);
                    memStream.Write(data_offset[i], 0, data_offset[i].Length);
                    memStream.Write(data_size[i], 0, data_size[i].Length);
                    memStream.Write(flags[i], 0, flags[i].Length);
                    memStream.Write(padding[i], 0, padding[i].Length);


                    i++;
                }
                
                i = 0;
                while (num - 1 >= i)
                {

                    memStream.Write(names[i], 0, names[i].Length);
                    i++;
                }
                memStream.Close();
                writer.Write(fileheader);
                dsha.TransformFinalBlock(fileheader, 0, fileheader.Length);

                byte[] tcontentID = Encoding.UTF8.GetBytes(cid);
                MemoryStream cidStream = new MemoryStream(contentID);
                cidStream.Write(tcontentID, 0, tcontentID.Length);
                writer.Seek(0x30, SeekOrigin.Begin);
                writer.Write(contentID);
                cidStream.Close();

                byte[] tQADigest = dsha.Hash;
                MemoryStream QADigestStream = new MemoryStream(QADigest);
                QADigestStream.Write(tQADigest, 0, 0x10);
                writer.Seek(0x60, SeekOrigin.Begin);
                //QADigest = dsha.Hash;
                writer.Write(QADigest);

                Set_Key(QADigest);
                byte[] end = new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };
                for (int c = 0; c < KLicensee.Length; c++)
                {
                    KLicensee[c] = 0x00;
                }
                byte[] tKLicensee = crypt(KLicensee, end, 16, 0);
                MemoryStream KLicenseeStream = new MemoryStream(KLicensee);
                KLicenseeStream.Write(tKLicensee, 0, 0x10);
                writer.Seek(0x70, SeekOrigin.Begin);
                writer.Write(KLicensee, 0, 0x10);

                end = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

                
                SHA1Managed hdsha = new SHA1Managed();
                //hdsha.TransformFinalBlock(Header2, 0, 0x80);
                hdsha.TransformBlock(Header2, 0, 0x30, Header2, 0);
                hdsha.TransformBlock(contentID, 0, 0x30, contentID, 0);
                hdsha.TransformBlock(QADigest, 0, 0x10, QADigest, 0);
                hdsha.TransformFinalBlock(KLicensee, 0, 0x10);
                writer.Seek(0x80, SeekOrigin.Begin);
                writer.Write(hdsha.Hash, 3, 0x10);


                byte[] metaBlockSHAPad = new byte[0x30];

                byte[] b = make_MetaHeader(metaBlock);
                SHA1Managed mdsha = new SHA1Managed();
                mdsha.TransformFinalBlock(b, 0, 0x40);
                byte[] msha = mdsha.Hash;
                Set_Key(msha);
                byte[] metaBlockSHAPadEnc = crypt(metaBlockSHAPad, end, 0x30, 0);

                Set_Key(hdsha.Hash);
                byte[] metaBlockSHAPadEnc2 = crypt(metaBlockSHAPadEnc, end, 0x30, 0);

                writer.Seek(0x90, SeekOrigin.Begin);
                writer.Write(metaBlockSHAPadEnc2, 0, 0x30);

                writer.Seek(0xC0, SeekOrigin.Begin);
                writer.Write(b, 0, 0x40);

                writer.Seek(0x100, SeekOrigin.Begin);
                writer.Write(msha, 3, 0x10);

                writer.Seek(0x110, SeekOrigin.Begin);
                writer.Write(metaBlockSHAPadEnc, 0, 0x30);


                writer.Close();
                //Console.WriteLine("FinalBlock   : {0}", BytesToStr(dsha.Hash));

                Set_Key(QADigest);
                uint dataRelativeOffset = 0;
                long pkgEncryptedFileStartOffset = 0x140;
                uint datasize = (uint)edata_size;
                int idatasize = (int)edata_size;


                while (dataRelativeOffset < edata_size)
                {
                    Stream PKGReadStream2 = new FileStream("test.pkg", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    BinaryReader brPKG2 = new BinaryReader(PKGReadStream2);

                    PKGReadStream2.Seek(pkgEncryptedFileStartOffset, SeekOrigin.Begin);

                    byte[] temp_bytes = new byte[0x10];
                    byte[] new_bytes = new byte[0x10];
                    temp_bytes = brPKG2.ReadBytes(0x10);
                    Set_Key(QADigest);
                    new_bytes = DecryptdData(temp_bytes, end, 0x10, dataRelativeOffset);
                    brPKG2.Close();
                    using (BinaryWriter writer2 = new BinaryWriter(File.Open("test.pkg", FileMode.Open)))
                    {
                        int peo = (int)pkgEncryptedFileStartOffset;
                        writer2.Seek(peo, SeekOrigin.Begin);
                        writer2.Write(new_bytes, 0, 0x10);

                        writer2.Close();
                    }

                    dataRelativeOffset += 0x10;
                    pkgEncryptedFileStartOffset += 0x10;

                }

            }
        }

        public static void ContentTypes(string type)
        {
            string sub = type.Substring(0, 2);
            string ctype;
           
            if (sub == "0x")
            {
                string sub2 = type.Substring(2, type.Length - 2);

                ctype = sub2;
            }


            if (type == "GameData")
            {
                ctype = "4";
            }

            if (type == "Theme")
            {
                ctype = "9";
            }

            if (type == "Widget")
            {
                ctype = "A";
            }

            if (type == "License")
            {
                ctype = "B";
            }

            if (type == "VSHModule")
            {
                ctype = "C";
            }

            if (type == "PSN Avatar")
            {
                ctype = "D";
            }


        }
        
        public static void Set_Key(byte[] tkey)
        {
            for (int c = 0; c < key.Length; c++)
            {
                key[c] = 0x00;
            }
            Array.Copy(tkey, 0, key, 0, 8);
            Array.Copy(tkey, 0, key, 8, 8);
            Array.Copy(tkey, 8, key, 16, 8);
            Array.Copy(tkey, 8, key, 24, 8);
        }

        public static string BytesToStr(byte[] bytes)
        {
            StringBuilder str = new StringBuilder();

            for (int i = 0; i < bytes.Length; i++)
                str.AppendFormat("{0:X2}", bytes[i]);

            return str.ToString();
        }

        public static byte[] crypt(byte[] pkg, byte[] end, uint size, uint eoffset)
        {
            byte[] temp = new byte[0x08];
            byte[] bfr = new byte[0x1c];
            Array.Copy(end, 0, key, 0x38, end.Length);
            Array.Copy(key, 0x38, temp, 0, temp.Length);

            SHA1 sha1 = new SHA1CryptoServiceProvider();

            uint utemp = BitConverter.ToUInt32(temp, 0) + eoffset;

            temp = BitConverter.GetBytes(utemp);
            int v = 8 - temp.Length;
            Array.Reverse(temp);
            Array.Copy(temp, 0, key, 56 + v, temp.Length);
            bfr = sha1.ComputeHash(key, 0, 0x40);

            uint i = 0;
            for (i = 0; i < size; i++)
            {
                if (i != 0 && (i % 16) == 0)
                {
                    Array.Reverse(temp);
                    utemp = BitConverter.ToUInt32(temp, 0) + 1;

                    temp = BitConverter.GetBytes(utemp);
                    v = 8 - temp.Length;
                    Array.Reverse(temp);
                    Array.Copy(temp, 0, key, 56 + v, temp.Length);
                    bfr = sha1.ComputeHash(key, 0, 0x40);
                }
                pkg[i] ^= bfr[i & 0xf];

            }
            return pkg;
        }

        public static byte[] DecryptdData(byte[] pkg, byte[] end, uint size, uint eoffset)
        {
            int dataSize = pkg.Length;

            byte[] temp = new byte[0x08];

            Array.Copy(key, 0x38, temp, 0, temp.Length);
            byte[] bfr = new byte[0x1c];


            SHA1 sha1 = new SHA1CryptoServiceProvider();
            uint utemp = BitConverter.ToUInt32(temp, 0) + 0;

            temp = BitConverter.GetBytes(utemp);
            int v = 8 - temp.Length;
            Array.Reverse(temp);
            Array.Copy(temp, 0, key, 56 + v, temp.Length);
            bfr = sha1.ComputeHash(key, 0, 0x40);

            uint w = 0;
            for (w = 0; w < eoffset; w++)
            {

                if (w != 0 && (w % 16) == 0)
                {
                    Array.Reverse(temp);
                    utemp = BitConverter.ToUInt32(temp, 0) + 1;

                    temp = BitConverter.GetBytes(utemp);
                    v = 8 - temp.Length;
                    Array.Reverse(temp);
                    Array.Copy(temp, 0, key, 56 + v, temp.Length);
                    bfr = sha1.ComputeHash(key, 0, 0x40);
                }


            }
            uint i = 0;
            for (i = 0; i < dataSize; i++, w++)
            {
                if (w != 0 && (w % 16) == 0)
                {
                    Array.Reverse(temp);
                    utemp = BitConverter.ToUInt32(temp, 0) + 1;

                    temp = BitConverter.GetBytes(utemp);
                    v = 8 - temp.Length;
                    Array.Reverse(temp);
                    Array.Copy(temp, 0, key, 56 + v, temp.Length);
                    bfr = sha1.ComputeHash(key, 0, 0x40);
                }
                pkg[i] ^= bfr[w & 0xf];

            }


            return pkg;
        }



    }


    

    public class EbootMeta
    {

       public byte[] magic = new byte[4];
       public byte[] unk1 = new byte[4];
       public byte[] drmType = new byte[4];
       public byte[] unk2 = new byte[4];
       public byte[] contentID = new byte[30];
       public byte[] fileSHA1 = new byte[0x10];
       public byte[] notSHA1 = new byte[0x10];
       public byte[] notXORKLSHA1 = new byte[0x10];
       public byte[] nulls = new byte[0x10];

        public void load(FileStream sfb)
        {

            sfb.Read(magic, 0, 4);
            sfb.Read(unk1, 0, 4);
            sfb.Read(drmType, 0, 4);
            sfb.Read(unk2, 0, 4);
            sfb.Read(contentID, 0, 0x30);
            sfb.Read(fileSHA1, 0, 0x10);
            sfb.Read(notSHA1, 0, 0x10);
            sfb.Read(notXORKLSHA1, 0, 0x10);
            sfb.Read(nulls, 0, 0x10);

        }

        public void Write(FileStream sfb)
        {

            sfb.Write(magic, 0, 4);
            sfb.Write(unk1, 0, 4);
            sfb.Write(drmType, 0, 4);
            sfb.Write(unk2, 0, 4);
            sfb.Write(contentID, 0, 0x30);
            sfb.Write(fileSHA1, 0, 0x10);
            sfb.Write(notSHA1, 0, 0x10);
            sfb.Write(notXORKLSHA1, 0, 0x10);
            sfb.Write(nulls, 0, 0x10);

        }
    }

    public class MetaHeader
    {
        public byte[] unk1 = { 0x00, 0x00, 0x00, 0x01 };
        public byte[] unk2 = { 0x00, 0x00, 0x00, 0x04 };
        public byte[] drmType = { 0x00, 0x00, 0x00, 0x03 };
        public byte[] unk4 = { 0x00, 0x00, 0x00, 0x02 };

        public byte[] unk21 = { 0x00, 0x00, 0x00, 0x04 };
        public byte[] unk22 = { 0x00, 0x00, 0x00, 0x03 };
        public byte[] unk23 = { 0x00, 0x00, 0x00, 0x03 };
        public byte[] unk24 = { 0x00, 0x00, 0x00, 0x04 };

        public byte[] unk31 = { 0x00, 0x00, 0x00, 0x0E };
        public byte[] unk32 = { 0x00, 0x00, 0x00, 0x04 };
        public byte[] unk33 = { 0x00, 0x00, 0x00, 0x08 };
        public byte[] secondaryVersion = { 0x00, 0x00 };
        public byte[] unk34 = { 0x00, 0x00 };

        public byte[] dataSize = new byte[4];
        public byte[] unk42 = { 0x00, 0x00, 0x00, 0x05 };
        public byte[] unk43 = { 0x00, 0x00, 0x00, 0x04 };
        public byte[] packagedBy = { 0x10, 0x61 };
        public byte[] packageVersion = { 0x00, 0x00 };



        public byte[] Set_MetaHeader(MetaHeader mh)
        {
            byte[] MetaHeader2 = new byte[0x40];
            MetaHeader metaHeader = mh;

            MemoryStream memStream = new MemoryStream(MetaHeader2);

            memStream.Write(metaHeader.unk1, 0, metaHeader.unk1.Length);
            memStream.Write(metaHeader.unk2, 0, metaHeader.unk2.Length);
            memStream.Write(metaHeader.drmType, 0, metaHeader.drmType.Length);
            memStream.Write(metaHeader.unk4, 0, metaHeader.unk4.Length);

            memStream.Write(metaHeader.unk21, 0, metaHeader.unk21.Length);
            memStream.Write(metaHeader.unk22, 0, metaHeader.unk22.Length);
            memStream.Write(metaHeader.unk23, 0, metaHeader.unk23.Length);
            memStream.Write(metaHeader.unk24, 0, metaHeader.unk24.Length);

            memStream.Write(metaHeader.unk31, 0, metaHeader.unk31.Length);
            memStream.Write(metaHeader.unk32, 0, metaHeader.unk32.Length);
            memStream.Write(metaHeader.unk33, 0, metaHeader.unk33.Length);
            memStream.Write(metaHeader.secondaryVersion, 0, metaHeader.secondaryVersion.Length);
            memStream.Write(metaHeader.unk34, 0, metaHeader.unk34.Length);

            memStream.Write(metaHeader.dataSize, 0, 4);
            memStream.Write(metaHeader.unk42, 0, metaHeader.unk42.Length);
            memStream.Write(metaHeader.unk43, 0, metaHeader.unk43.Length);
            memStream.Write(metaHeader.packagedBy, 0, metaHeader.packagedBy.Length);
            memStream.Write(metaHeader.packageVersion, 0, metaHeader.packageVersion.Length);

            memStream.Close();

            return MetaHeader2;
        }

        public void Write_MetaHeader(MetaHeader hd, BinaryWriter writer)
        {
            MetaHeader metaBlock = hd;

            writer.Write(metaBlock.unk1, 0, metaBlock.unk1.Length);
            writer.Write(metaBlock.unk2, 0, metaBlock.unk2.Length);
            writer.Write(metaBlock.drmType, 0, metaBlock.drmType.Length);
            writer.Write(metaBlock.unk4, 0, metaBlock.unk4.Length);

            writer.Write(metaBlock.unk21, 0, metaBlock.unk21.Length);
            writer.Write(metaBlock.unk22, 0, metaBlock.unk22.Length);
            writer.Write(metaBlock.unk23, 0, metaBlock.unk23.Length);
            writer.Write(metaBlock.unk24, 0, metaBlock.unk24.Length);

            writer.Write(metaBlock.unk31, 0, metaBlock.unk31.Length);
            writer.Write(metaBlock.unk32, 0, metaBlock.unk32.Length);
            writer.Write(metaBlock.unk33, 0, metaBlock.unk33.Length);
            writer.Write(metaBlock.secondaryVersion, 0, metaBlock.secondaryVersion.Length);
            writer.Write(metaBlock.unk34, 0, metaBlock.unk34.Length);

            writer.Write(metaBlock.dataSize);

            writer.Write(metaBlock.unk42, 0, metaBlock.unk42.Length);
            writer.Write(metaBlock.unk43, 0, metaBlock.unk43.Length);
            writer.Write(metaBlock.packagedBy, 0, metaBlock.packagedBy.Length);
            writer.Write(metaBlock.packageVersion, 0, metaBlock.packageVersion.Length);
        }

    }

    public class DigestBlock
    {

       public byte[] type = new byte[4];
       public byte[] size = new byte[4];
       public byte[] isNext = new byte[8];

        public void Set_Header(FileStream stream)
        {

           type = new byte[4];
           size = new byte[4];
           isNext = new byte[8];

        }


        }

    public class FileHeader
    {

      public byte[] fileNameOff = new byte[4];
      public byte[] fileNameLength = new byte[4];
      public byte[] fileOff = new byte[8];
      public byte[] fileSize = new byte[8];
      public byte[] flags = new byte[4];
      public byte[] padding = new byte[4];

        public void load(FileStream sfb)
        {
            sfb.Read(fileNameOff, 0, 4);
            sfb.Read(fileNameLength, 0, 4);
            sfb.Read(fileOff, 0, 8);
            sfb.Read(fileSize, 0, 8);
            sfb.Read(flags, 0, 4);
            sfb.Read(padding, 0, 4); 
        }

    }

    public class Header
    {

      public byte[] magic = { 0x7F, 0x50, 0x4B, 0x47 };
      public byte[] type = { 0x00, 0x00, 0x00, 0x01 };
      public byte[] pkgInfoOff = { 0x00, 0x00, 0x00, 0xC0 };
      public byte[] unk1 = { 0x00, 0x00, 0x00, 0x05 };
      public byte[] headSize = { 0x00, 0x00, 0x00, 0x80 };
      public byte[] itemCount = new byte[4];
      public byte[] packageSize = new byte[8];
        public byte[] dataOff = { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x40 };
      public byte[] dataSize = new byte[8];
      
      public byte[] contentID = new byte[30];
      public byte[] QADigest = new byte[0x10];
      public byte[] KLicensee = new byte[0x10];


        public void Set_Header(long edata_size, int num, byte[] cid, byte[] QA, byte[] kl)
        {
            byte[] magic = { 0x7F, 0x50, 0x4B, 0x47 };
            byte[] type = { 0x00, 0x00, 0x00, 0x01 };
            byte[] pkgInfoOff = { 0x00, 0x00, 0x00, 0xC0 };
            byte[] unk1 = { 0x00, 0x00, 0x00, 0x05 };
            byte[] headSize = { 0x00, 0x00, 0x00, 0x80 };
            byte[] itemCount = new byte[4];// 
            itemCount = BitConverter.GetBytes(num);
            Array.Reverse(itemCount);
            byte[] packageSize = new byte[8];// 
            packageSize = BitConverter.GetBytes(edata_size + 0x1A0); //{ 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            Array.Reverse(packageSize);
            byte[] dataOff = { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x40 };
            byte[] dataSize = new byte[8];// 
            dataSize = BitConverter.GetBytes(edata_size); // { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            Array.Reverse(dataSize);

            byte[] contentID = cid;
            byte[] QADigest = QA;
            byte[] KLicensee = kl;

        }


        public byte[] Set_Header(Header hd, byte[] Header2)
        {
            Header header = hd;
            MemoryStream memStream = new MemoryStream(Header2);

            memStream.Write(header.magic, 0, header.magic.Length);
            memStream.Write(header.type, 0, header.type.Length);
            memStream.Write(header.pkgInfoOff, 0, header.pkgInfoOff.Length);
            memStream.Write(header.unk1, 0, header.unk1.Length);
            memStream.Write(header.headSize, 0, header.headSize.Length);
            memStream.Write(header.itemCount, 0, header.itemCount.Length);
            memStream.Write(header.packageSize, 0, header.packageSize.Length);
            memStream.Write(header.dataOff, 0, header.dataOff.Length);
            memStream.Write(header.dataSize, 0, header.dataSize.Length);

            memStream.Write(header.contentID, 0, header.contentID.Length);
            memStream.Write(header.QADigest, 0, header.QADigest.Length);
            memStream.Write(header.KLicensee, 0, header.KLicensee.Length);


            memStream.Close();
            return Header2;
        }


        public void Write_Header(Header hd, BinaryWriter writer, long edata_size, int num)
        {
            
            Header header = hd;
            /*header.itemCount = BitConverter.GetBytes(num);
            Array.Reverse(header.itemCount);

            header.packageSize = BitConverter.GetBytes(edata_size + 0x1A0);
            Array.Reverse(header.packageSize);

            header.dataSize = BitConverter.GetBytes(edata_size);
            Array.Reverse(header.dataSize);

            header.contentID = contentID;
            header.QADigest = QADigest;
            header.KLicensee = KLicensee;*/

            writer.Write(header.magic, 0, header.magic.Length);
            writer.Write(header.type, 0, header.type.Length);
            writer.Write(header.pkgInfoOff, 0, header.pkgInfoOff.Length);
            writer.Write(header.unk1, 0, header.unk1.Length);
            writer.Write(header.headSize, 0, header.headSize.Length);
            writer.Write(header.itemCount, 0, header.itemCount.Length);
            writer.Write(header.packageSize, 0, header.packageSize.Length);
            writer.Write(header.dataOff, 0, header.dataOff.Length);
            writer.Write(header.dataSize, 0, header.dataSize.Length);
            writer.Write(header.contentID, 0, header.contentID.Length);
            writer.Write(header.QADigest, 0, header.QADigest.Length);
            writer.Write(header.KLicensee, 0, header.KLicensee.Length);
        }



    }



    public class SelfHeader
    {

      public  byte[] magic = new byte[4];
      public  byte[] headerVer = new byte[4];
      public  byte[] flags = new byte[2];
      public  byte[] type = new byte[2];
      public  byte[] meta = new byte[4];
      public  byte[] headerSize = new byte[8];
      public  byte[] encryptedSize = new byte[8];
      public  byte[] unknown = new byte[8];
      public  byte[] AppInfo = new byte[8];
      public  byte[] elf = new byte[8];
      public  byte[] phdr = new byte[8];
      public  byte[] shdr = new byte[8];
      public  byte[] phdrOffsets = new byte[8];
      public  byte[] sceversion = new byte[8];
      public  byte[] digest = new byte[8];
      public byte[] digestSize = new byte[8];

        public void load(FileStream sfb)
        {

            sfb.Read(magic, 0, 4);
            sfb.Read(headerVer, 0, 4);
            sfb.Read(flags, 0, 2);
            sfb.Read(type, 0, 2);
            sfb.Read(meta, 0, 4);
            sfb.Read(headerSize, 0, 8);
            sfb.Read(encryptedSize, 0, 8);
            sfb.Read(unknown, 0, 8);
            sfb.Read(AppInfo, 0, 8);
            sfb.Read(elf, 0, 8);
            sfb.Read(phdr, 0, 8);
            sfb.Read(shdr, 0, 8);
            sfb.Read(phdrOffsets, 0, 8);
            sfb.Read(sceversion, 0, 8);
            sfb.Read(digest, 0, 8);
            sfb.Read(digestSize, 0, 8);



        }

    }

    public class AppInfo
    {

       public byte[] authid = new byte[8];
       public byte[] unknown = new byte[4];
       public byte[] appType = new byte[4];
       public byte[] appVersion = new byte[8];
        

        public void load(FileStream sfb)
        {

            sfb.Read(authid, 0, 8);
            sfb.Read(unknown, 0, 4);
            sfb.Read(appType, 0, 4);
            sfb.Read(appVersion, 0, 8);
            



        }

    }
     



}
