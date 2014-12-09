#region License
// Copyright (c) 2007 James Newton-King
//
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
#endregion

using System;
using System.Globalization;
using System.IO;
using System.Text;
using Newtonsoft.Json.Utilities;
using System.Numerics;

namespace Newtonsoft.Json.Smile
{
    internal class SmileBinaryWriter
    {
        private static readonly Encoding Encoding = new UTF8Encoding(false);

        private readonly BinaryWriter _writer;

        private byte[] _largeByteBuffer;

        public DateTimeKind DateTimeKindHandling { get; set; }

        public SmileBinaryWriter(BinaryWriter writer)
        {
            DateTimeKindHandling = DateTimeKind.Utc;
            _writer = writer;
        }

        public void Flush()
        {
            _writer.Flush();
        }

        public void Close()
        {
#if !(NETFX_CORE || PORTABLE40 || PORTABLE)
            _writer.Close();
#else
            _writer.Dispose();
#endif
        }

		public void WriteHeader()
		{
			this.WriteHeaderWithOptions(SMILE_OPTIONS.DEFAULT);
		}

		public void WriteHeaderWithOptions(SMILE_OPTIONS options)
		{
			this._Write(0x3a, 0x29, 0x0a, (byte)options);
		}

		public void WriteStartArray()
		{
			this._writer.Write((byte)VALUE_CONSTANTS.StartArray);
		}

		public void WriteEndArray()
		{
			this._writer.Write((byte)VALUE_CONSTANTS.EndArray);
		}

		public void WriteStartObject()
		{
			this._writer.Write((byte)VALUE_CONSTANTS.StartObject);
		}

		public void WriteEndObject()
		{
			this._writer.Write((byte)VALUE_CONSTANTS.EndObject);
		}

		public bool WriteAsciiPropertyName(byte[] name)
		{
			if (name.Length < 1)
				throw new ArgumentOutOfRangeException("ascii property name is too short.");
			if (name.Length > 64)
				return WriteLongUnicodePropertyName(name);

			int type = (byte)KEY_TYPES.ShortAsciiName_BEGIN + name.Length - 1;
			this._writer.Write((byte)type);
			this._writer.Write(name);

			return true;
		}

		public bool WriteUnicodePropertyName(byte[] name)
		{
			if (name.Length < 2)
				throw new ArgumentOutOfRangeException("unicode property name is too short.");
			if (name.Length > 57)
				return WriteLongUnicodePropertyName(name);
	
			int type = (byte)KEY_TYPES.ShortUnicodeName_BEGIN + name.Length - 2;
			this._writer.Write((byte)type);
			this._writer.Write(name);

			return true;
		}

		public bool WriteLongUnicodePropertyName(byte[] name)
		{
			//TODO: need check how Jackson process unicode property name between 58bytes to 63bytes.
			//if (name.Length < 64)
			//	throw new ArgumentOutOfRangeException("long unicode property name must be longer than 63bytes");
			this._writer.Write((byte)KEY_TYPES.LongUnicodeName);
			this._writer.Write(name);
			this._Write(VALUE_CONSTANTS.StringEndMarker);

			return SmileConstant.SHARE_LONG_UNICODE_KEY_NAME;
		}

		public void WriteShortReferencePropertyName(int ref_id)
		{
			if (ref_id < 0 || ref_id > 63)
				throw new ArgumentOutOfRangeException("invalid reference property name");
			int type = 0x40 + ref_id;
			this._writer.Write((byte)type);
		}

		public void WriteBigInteger(BigInteger value)
		{
			throw new NotImplementedException();
		}

		public void WriteNull()
		{
			//this._writer.Write("");
			this._writer.Write((byte)VALUE_CONSTANTS.Null);
		}

		public void WriteUndefined()
		{
			throw new NotImplementedException();
		}

		public void WriteString(string value)
		{
			byte[] buf;
			bool isASCII = SmileUtil.IsASCIIBytes(value, out buf);
			if (buf.Length == 0)
				this._Write(VALUE_CONSTANTS.EmptyString);

			else if (isASCII)
			{
				if (buf.Length >= 1 && buf.Length <= 64)
					this.WriteShortString((byte)((byte)VALUE_CONSTANTS.TinyAscii_BEGIN + buf.Length - 1), buf);
				else
					this.WriteFCString((byte)VALUE_CONSTANTS.LongAsciiText, buf);
			}
			else
			{
				if (buf.Length < 2)
					throw new FormatException("bad utf-8 string.");
				else if (buf.Length >= 2 && buf.Length <= 64)
					this.WriteShortString((byte)((byte)VALUE_CONSTANTS.TinyUnicode_BEGIN + buf.Length - 2), buf);
				else
					this.WriteFCString((byte)VALUE_CONSTANTS.LongUnicodeText, buf);
			}
		}

		private void WriteShortString(byte type, byte[] buf)
		{
			this._writer.Write(type);
			this._writer.Write(buf);
		}

		private void WriteFCString(byte type, byte[] buf)
		{
			this._writer.Write(type);
			this._writer.Write(buf);
			this._writer.Write((byte)VALUE_CONSTANTS.StringEndMarker);
		}

		//private const int TOKEN_PREFIX_SMALL_INT = 0xC0;
		//private const int TOKEN_BYTE_INT_32 = 0x24;
		//private const int TOKEN_BYTE_INT_64 = 0x25;
		public void WriteInteger(int value)
		{
			int i = value;
    		// First things first: let's zigzag encode number
			i = SmileUtil.zigzagEncode(i);
			// tiny (single byte) or small (type + 6-bit value) number?
			if (i <= 0x3F && i >= 0) {
				if (i <= 0x1F) { // tiny 
					this._writer.Write((byte)((byte)VALUE_CONSTANTS.SmallInteger_BEGIN + i));
					return;
				}
				// nope, just small, 2 bytes (type, 1-byte zigzag value) for 6 bit value
				this._Write(VALUE_CONSTANTS.Integer, (byte)(0x80 + i));
				return;
			}
			// Ok: let's find minimal representation then
			byte b0 = (byte) (0x80 + (i & 0x3F));
			//i >>>= 6;
			i = (int)((uint)i >> 6);
			if (i <= 0x7F) { // 13 bits is enough (== 3 byte total encoding)
				this._Write(VALUE_CONSTANTS.Integer, (byte)i, b0);
				return;
			}
			byte b1 = (byte) (i & 0x7F);
			i >>= 7;
			if (i <= 0x7F) {
				this._Write(VALUE_CONSTANTS.Integer, (byte)i, b1, b0);
				return;
			}
			byte b2 = (byte) (i & 0x7F);
			i >>= 7;
			if (i <= 0x7F) {
				this._Write(VALUE_CONSTANTS.Integer, (byte)i, b2, b1, b0);
				return;
			}
			// no, need all 5 bytes
			byte b3 = (byte) (i & 0x7F);
			this._Write(VALUE_CONSTANTS.Integer, (byte)(i >> 7), b3, b2, b1, b0);

//			throw new NotImplementedException();
		}

		public void WriteLong(long value)
		{
			if (value >= int.MinValue && value <= int.MaxValue)
			{
				this.WriteInteger((int)value);
				return;
			}

			value = SmileUtil.zigzagEncode(value);
			// Ok, well, we do know that 5 lowest-significant bytes are needed
			int i = (int)value;
			// 4 can be extracted from lower int
			byte b0 = (byte) (0x80 + (i & 0x3F)); // sign bit set in the last byte
			byte b1 = (byte) ((i >> 6) & 0x7F);
			byte b2 = (byte) ((i >> 13) & 0x7F);
			byte b3 = (byte) ((i >> 20) & 0x7F);
			// fifth one is split between ints:
			value = (long)((ulong)value >> 27);
			byte b4 = (byte) (((int) value) & 0x7F);

			i = (int)(value >> 7);
			if (i == 0)
			{
				this._Write(VALUE_CONSTANTS.Long, b4, b3, b2, b1, b0);
				return;
			}
			if (i <= 0x7F)
			{
				this._Write(VALUE_CONSTANTS.Long, (byte)i, b4, b3, b2, b1, b0);
				return;
			}
			byte b5 = (byte)(i & 0x7F);
			i >>= 7;
			if (i <= 0x7F)
			{
				this._Write(VALUE_CONSTANTS.Long, (byte)i, b5, b4, b3, b2, b1, b0);
				return;
			}
			byte b6 = (byte)(i & 0x7F);
			i >>= 7;
			if (i <= 0x7F)
			{
				this._Write(VALUE_CONSTANTS.Long, (byte)i, b6, b5, b4, b3, b2, b1, b0);
				return;
			}
			byte b7 = (byte)(i & 0x7F);
			i >>= 7;
			if (i <= 0x7F)
			{
				this._Write(VALUE_CONSTANTS.Long, (byte)i, b7, b6, b5, b4, b3, b2, b1, b0);
				return;
			}
			byte b8 = (byte)(i & 0x7F);
			i >>= 7;
			// must be done, with 10 bytes! (9 * 7 + 6 == 69 bits; only need 63)
			this._Write(VALUE_CONSTANTS.Long, (byte)i, b8, b7, b6, b5, b4, b3, b2, b1, b0);
		}

		private void _Write(params byte[] value)
		{
			this._writer.Write(value);
		}

		private void _Write(VALUE_CONSTANTS type, params byte[] value)
		{
			this._writer.Write((byte)type);
			this._writer.Write(value);
		}

		private static int FloatToInt32Bits(float value)
		{
			byte[] bytes = BitConverter.GetBytes(value);
			return BitConverter.ToInt32(bytes, 0);
		}

		public void WriteFloat(float f)
		{
			/* 17-Apr-2010, tatu: could also use 'floatToIntBits', but it seems more accurate to use
			 * exact representation; and possibly faster. However, if there are cases
			 * where collapsing of NaN was needed (for non-Java clients), this can
			 * be changed
			 */
			int i = FloatToInt32Bits(f);
			byte b4 = (byte)(i & 0x7F);
			i >>= 7;
			byte b3 = (byte)(i & 0x7F);
			i >>= 7;
			byte b2 = (byte)(i & 0x7F);
			i >>= 7;
			byte b1 = (byte)(i & 0x7F);
			i >>= 7;
			byte b0 = (byte)(i & 0x7F);
			this._Write(VALUE_CONSTANTS.Float, b0, b1, b2, b3, b4);
		}

		public void WriteDouble(double value)
		{
			long l = BitConverter.DoubleToInt64Bits(value);
			int hi5 = (int)((ulong)l >> 35);
			byte b4 = (byte)(hi5 & 0x7F); 
			hi5 >>= 7;
			byte b3 = (byte)(hi5 & 0x7F);
			hi5 >>= 7;
			byte b2 = (byte)(hi5 & 0x7F);
			hi5 >>= 7;
			byte b1 = (byte)(hi5 & 0x7F);
			hi5 >>= 7;
			byte b0 = (byte)hi5;
			this._Write(VALUE_CONSTANTS.Double, b0, b1, b2, b3, b4);

			// Handle first 29 bits (single bit first, then 4 x 7 bits)
			// Then split byte (one that crosses lo/hi int boundary), 7 bits
			int mid = (int) (l >> 28);
			this._Write((byte)(mid & 0x7F));

			// and then last 4 bytes (28 bits)
			int lo4 = (int) l;
			b3 = (byte) (lo4 & 0x7F);
			lo4 >>= 7;
			b2 = (byte) (lo4 & 0x7F);
			lo4 >>= 7;
			b1 = (byte) (lo4 & 0x7F);
			lo4 >>= 7;
			b0 = (byte) (lo4 & 0x7F);
			this._Write(b0, b1, b2, b3);
		}

		public void WriteBool(bool value)
		{
			this._writer.Write(value ? (byte)VALUE_CONSTANTS.True : (byte)VALUE_CONSTANTS.False);
		}

		public void WriteByte(byte value)
		{
			this._writer.Write(value);
		}

		public void WriteSByte(sbyte value)
		{
			this._writer.Write(value);
		}

		public void WriteDecimal(Decimal value)
		{
			throw new NotImplementedException();
		}

		public void WriteDate(DateTime value)
		{
			throw new NotImplementedException();
		}

		public void WriteDate(DateTimeOffset value)
		{
			throw new NotImplementedException();
		}

		private static int[] masks = new int[] { 1, 3, 7, 15, 31, 63, 127, 255};
		public void WriteBytes(byte[] value)
		{
			this._writer.Write((byte)VALUE_CONSTANTS.Binary7Bit);
			//this.WriteVInt(value.Length);
			this.Write7BitBinaryWithLength(value);
		}

		protected void Write7BitBinaryWithLength(byte[] data)
		{
			this.WritePositiveVInt(data.Length);
			// first, let's handle full 7-byte chunks
			int len = data.Length;
			int i = 0;
			while (len >= 7)
			{
				int v = data[i++];
				this._writer.Write((byte)((v >> 1) & 0x7F));
				v = (v << 8) | (data[i++] & 0xFF);
				this._writer.Write((byte)((v >> 2) & 0x7F));
				v = (v << 8) | (data[i++] & 0xFF);
				this._writer.Write((byte)((v >> 3) & 0x7F));
				v = (v << 8) | (data[i++] & 0xFF);
				this._writer.Write((byte)((v >> 4) & 0x7F));
				v = (v << 8) | (data[i++] & 0xFF);
				this._writer.Write((byte)((v >> 5) & 0x7F));
				v = (v << 8) | (data[i++] & 0xFF);
				this._writer.Write((byte)((v >> 6) & 0x7F));
				v = (v << 8) | (data[i++] & 0xFF);
				this._writer.Write((byte)((v >> 7) & 0x7F));
				this._writer.Write((byte)(v & 0x7F));
				len -= 7;
			}

			// and then partial piece, if any
			if (len > 0)
			{
				int v = data[i++];
				this._writer.Write((byte)((v >> 1) & 0x7F));
				if (len > 1)
				{
					v = ((v & 0x01) << 8) | (data[i++] & 0xFF); // 2nd
					this._writer.Write((byte)((v >> 2) & 0x7F));
					if (len > 2)
					{
						v = ((v & 0x03) << 8) | (data[i++] & 0xFF); // 3rd
						this._writer.Write((byte)((v >> 3) & 0x7F));
						if (len > 3)
						{
							v = ((v & 0x07) << 8) | (data[i++] & 0xFF); // 4th
							this._writer.Write((byte)((v >> 4) & 0x7F));
							if (len > 4)
							{
								v = ((v & 0x0F) << 8) | (data[i++] & 0xFF); // 5th
								this._writer.Write((byte)((v >> 5) & 0x7F));
								if (len > 5)
								{
									v = ((v & 0x1F) << 8) | (data[i++] & 0xFF); // 6th
									this._writer.Write((byte)((v >> 6) & 0x7F));
									this._writer.Write((byte)(v & 0x3F)); // last 6 bits
								}
								else
								{
									this._writer.Write((byte)(v & 0x1F)); // last 5 bits                                
								}
							}
							else
							{
								this._writer.Write((byte)(v & 0x0F)); // last 4 bits
							}
						}
						else
						{
							this._writer.Write((byte)(v & 0x07)); // last 3 bits                        
						}
					}
					else
					{
						this._writer.Write((byte)(v & 0x03)); // last 2 bits                    
					}
				}
				else
				{
					this._writer.Write((byte)(v & 0x01)); // last bit
				}
			}
		}

		private void WritePositiveVInt(int i)
		{
			// At most 5 bytes (4 * 7 + 6 bits == 34 bits)
			byte b0 = (byte) (0x80 + (i & 0x3F));
			i >>= 6;
			if (i <= 0x7F) { // 6 or 13 bits is enough (== 2 or 3 byte total encoding)
				if (i > 0) {
					this._writer.Write((byte) i);
				}
				this._writer.Write(b0);
				return;
			}
			byte b1 = (byte) (i & 0x7F);
			i >>= 7;
			if (i <= 0x7F) {
				this._writer.Write((byte)i);
				this._writer.Write(b1);
				this._writer.Write(b0);            
			} else {
				byte b2 = (byte) (i & 0x7F);
				i >>= 7;
				if (i <= 0x7F) {
					this._writer.Write((byte) i);
					this._writer.Write(b2);
					this._writer.Write(b1);
					this._writer.Write(b0);            
				} else {
					byte b3 = (byte) (i & 0x7F);
					this._writer.Write((byte) (i >> 7));
					this._writer.Write(b3);
					this._writer.Write(b2);
					this._writer.Write(b1);
					this._writer.Write(b0);            
				}
			}
		}

    }
}