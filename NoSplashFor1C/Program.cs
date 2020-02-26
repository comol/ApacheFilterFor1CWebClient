using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;

namespace NoSplashFor1C
{
	class Program
	{
		static string Logfilelocation = "";

		static void Main(string[] args)
		{

			if (args.Length < 2)
			{
				Console.WriteLine("First param - file name with text to find");
				Console.WriteLine("Second param - file name with text to replace");
				return;
			}

			if (args.Length == 3)
			{
				Logfilelocation = args[2];
			}

			bool needdecompress = true;

			using (Stream stdin = Console.OpenStandardInput())
			using (Stream stdout = Console.OpenStandardOutput())
			{
				MemoryStream messagestream = new MemoryStream();

				string uri = System.Environment.GetEnvironmentVariable("REQUEST_URI");
				AddLog(uri);

				byte[] buffer = new byte[500000];
				int bytes;
				int sumbytes = 0;
				while ((bytes = stdin.Read(buffer, 0, buffer.Length)) > 0)
				{
					messagestream.Write(buffer, 0, bytes);
					sumbytes = sumbytes + bytes;
					
				}

				var inbuffer = messagestream.ToArray();
				
				if (istext(inbuffer))
				{
					needdecompress = false;
					AddLog("Need to decompress:" + needdecompress.ToString());
				}

				var decompressedbuffer = Decompress(inbuffer, needdecompress);

				string DecompressedString = Encoding.UTF8.GetString(decompressedbuffer); // В Строку utf8
				AddLog(DecompressedString);
				

				if (DecompressedString.LastIndexOf(readstringfromfile(args[0])) > 0) //Собственно замена строки
				{
					string resultstring = DecompressedString.Replace(readstringfromfile(args[0]), readstringfromfile(args[1]));
					AddLog(resultstring);
					var resultbytestocompress = Encoding.UTF8.GetBytes(resultstring);
					var compressedbytes = Compress(resultbytestocompress, needdecompress);
					stdout.Write(compressedbytes, 0, compressedbytes.Length);
				}
				else
				{
					stdout.Write(inbuffer, 0, inbuffer.Length);
				}

			}
		}


		public static bool istext(byte[] textdata)
		{
			var charbuffer = Encoding.UTF8.GetChars(textdata);
			int normalchars = 0;
			int nonormalchars = 0;

			for (int i = 0; i < Math.Min(charbuffer.Length, 20); i++)
			{
				var c = charbuffer[i];
				if (char.IsLetterOrDigit(c))
				{
					normalchars++;
				}
				else
				{
					nonormalchars++;
				}
			}

			if (normalchars > nonormalchars)
			{
				return true;
			}
			else
			{
				return false;
			}

		}

		public static void AddLog(string text)
		{
			if (Logfilelocation.Length > 0)
			{ 
				StreamWriter logfile = File.AppendText(Logfilelocation);
				logfile.WriteLine(text);
				logfile.Close();
			}
		}

		public static string readstringfromfile(string filename)
		{
			return File.ReadAllText(filename, Encoding.UTF8);
		}

		public static byte[] Decompress(byte[] data, bool needtodecompress)
		{

			if(!needtodecompress)
			{
				return data;
			}

			byte[] decompressedArray = null;
			try
			{
				using (MemoryStream decompressedStream = new MemoryStream())
				{
					using (MemoryStream compressStream = new MemoryStream(data))
					{
						using (DeflateStream deflateStream = new DeflateStream(compressStream, CompressionMode.Decompress))
						{
							deflateStream.CopyTo(decompressedStream);
						}
					}
					decompressedArray = decompressedStream.ToArray();
				}
			}
			catch (Exception exception)
			{
				
			}

			return decompressedArray;
		}

		public static byte[] Compress(byte[] data, bool needtodecompress)
		{
			if (!needtodecompress)
			{
				return data;
			}

			byte[] compressArray = null;
			try
			{
				using (MemoryStream memoryStream = new MemoryStream())
				{
					using (DeflateStream deflateStream = new DeflateStream(memoryStream, CompressionMode.Compress))
					{
						deflateStream.Write(data, 0, data.Length);
					}
					compressArray = memoryStream.ToArray();
				}
			}
			catch (Exception exception)
			{
				
			}
			return compressArray;
		}

	}
}
