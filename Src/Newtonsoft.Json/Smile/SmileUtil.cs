using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Newtonsoft.Json.Smile
{
	public partial class SmileUtil
	{
		public static bool IsASCIIBytes(string s, out byte[] buf)
		{
			buf = Encoding.UTF8.GetBytes(s);
			return buf.All(b => b < 128);
		}
		public static bool IsASCIIString(byte[] buf, out string s)
		{
			s = Encoding.UTF8.GetString(buf, 0, buf.Length);
			return !buf.Any(b => b >= 128);
		}

		public static byte[] GetASCIIBytes(string s)
		{
#if PORTABLE
			return Encoding.UTF8.GetBytes(s);
#else
			return Encoding.ASCII.GetBytes(s);
#endif
		}
		public static string GetASCIIString(byte[] buf)
		{
#if PORTABLE
			return Encoding.UTF8.GetString(buf, 0, buf.Length);
#else
			return Encoding.ASCII.GetString(buf, 0, buf.Length);
#endif
		}
	}

	public partial class SmileUtil
	{
		public static int zigzagEncode(int input)
		{
			// Canonical version:
			//return (input << 1) ^  (input >> 31);
			// but this is even better
			if (input < 0)
			{
				return (input << 1) ^ -1;
			}
			return (input << 1);
		}

		public static int zigzagDecode(int encoded) {
        // canonical:
        //return (encoded >>> 1) ^ (-(encoded & 1));
        if ((encoded & 1) == 0) { // positive
            return (int)((uint)encoded >> 1);
        }
        // negative
		return ((int)((uint)encoded >> 1)) ^ -1;
    }

		public static long zigzagEncode(long input)
		{
			// Canonical version
			//return (input << 1) ^  (input >> 63);
			if (input < 0L)
			{
				return (input << 1) ^ -1L;
			}
			return (input << 1);
		}

		public static long zigzagDecode(long encoded) {
        // canonical:
        //return (encoded >>> 1) ^ (-(encoded & 1));
        if ((encoded & 1) == 0) { // positive
            return (long)((ulong)encoded >> 1);
        }
        // negative
        return ((long)((ulong)encoded >> 1)) ^ -1L;
    }
	}
}
