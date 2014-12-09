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
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.IO;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Utilities;
using Newtonsoft.Json.Linq;

namespace Newtonsoft.Json.Smile
{
    /// <summary>
    /// Represents a reader that provides fast, non-cached, forward-only access to serialized Json data.
    /// </summary>
    public class SmileReader : JsonReader
    {
        private readonly BinaryReader _reader;
        private readonly List<ContainerContext> _stack;

		private SmileType _currentElementType;
        private ContainerContext _currentContext;

        private bool _readRootValueAsArray;
        private bool _jsonNet35BinaryCompatibility;
        private DateTimeKind _dateTimeKindHandling;

        private class ContainerContext
        {
			public readonly SmileType Type;

			public ContainerContext(SmileType type)
            {
                Type = type;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether binary data reading should compatible with incorrect Json.NET 3.5 written binary.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if binary data reading will be compatible with incorrect Json.NET 3.5 written binary; otherwise, <c>false</c>.
        /// </value>
        [Obsolete("JsonNet35BinaryCompatibility will be removed in a future version of Json.NET.")]
        public bool JsonNet35BinaryCompatibility
        {
            get { return _jsonNet35BinaryCompatibility; }
            set { _jsonNet35BinaryCompatibility = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the root object will be read as a JSON array.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if the root object will be read as a JSON array; otherwise, <c>false</c>.
        /// </value>
        public bool ReadRootValueAsArray
        {
            get { return _readRootValueAsArray; }
            set { _readRootValueAsArray = value; }
        }

        /// <summary>
        /// Gets or sets the <see cref="DateTimeKind" /> used when reading <see cref="DateTime"/> values from BSON.
        /// </summary>
        /// <value>The <see cref="DateTimeKind" /> used when reading <see cref="DateTime"/> values from BSON.</value>
        public DateTimeKind DateTimeKindHandling
        {
            get { return _dateTimeKindHandling; }
            set { _dateTimeKindHandling = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SmileReader"/> class.
        /// </summary>
        /// <param name="stream">The stream.</param>
        public SmileReader(Stream stream)
            : this(stream, false, DateTimeKind.Local)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SmileReader"/> class.
        /// </summary>
        /// <param name="reader">The reader.</param>
        public SmileReader(BinaryReader reader)
            : this(reader, false, DateTimeKind.Local)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SmileReader"/> class.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="readRootValueAsArray">if set to <c>true</c> the root object will be read as a JSON array.</param>
        /// <param name="dateTimeKindHandling">The <see cref="DateTimeKind" /> used when reading <see cref="DateTime"/> values from BSON.</param>
        public SmileReader(Stream stream, bool readRootValueAsArray, DateTimeKind dateTimeKindHandling)
        {
            ValidationUtils.ArgumentNotNull(stream, "stream");
            _reader = new BinaryReader(stream);
            _stack = new List<ContainerContext>();
            _readRootValueAsArray = readRootValueAsArray;
            _dateTimeKindHandling = dateTimeKindHandling;

			if (!ReadHeader())
				throw new Exception("invalid header!");

			//_readRootValueAsArray = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SmileReader"/> class.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="readRootValueAsArray">if set to <c>true</c> the root object will be read as a JSON array.</param>
        /// <param name="dateTimeKindHandling">The <see cref="DateTimeKind" /> used when reading <see cref="DateTime"/> values from BSON.</param>
        public SmileReader(BinaryReader reader, bool readRootValueAsArray, DateTimeKind dateTimeKindHandling)
        {
            ValidationUtils.ArgumentNotNull(reader, "reader");
            _reader = reader;
            _stack = new List<ContainerContext>();
            _readRootValueAsArray = readRootValueAsArray;
            _dateTimeKindHandling = dateTimeKindHandling;

			if (!ReadHeader())
				throw new Exception("invalid header!");

			//_readRootValueAsArray = true;
		}

		private SMILE_OPTIONS CurrentOptions = SMILE_OPTIONS.None;
		private bool ReadHeader()
		{
			//this.MovePosition(4);
			byte[] header = this._reader.ReadBytes(4);
			if (header[0] == 0x3a && header[1] == 0x29 && header[2] == 0x0a)
			{
				this.CurrentOptions = (SMILE_OPTIONS)header[3];
				return true;
			}
			return false;
		}

		private string ReadElement(byte keyType)
		{
			//_currentElementType = ReadType();
			string elementName = ReadString(keyType);
			//Console.WriteLine("elementName: {0}", elementName);
			_currentElementType = ReadType();
			return elementName;
		}

        /// <summary>
        /// Reads the next JSON token from the stream as a <see cref="Byte"/>[].
        /// </summary>
        /// <returns>
        /// A <see cref="Byte"/>[] or a null reference if the next JSON token is null. This method will return <c>null</c> at the end of an array.
        /// </returns>
        public override byte[] ReadAsBytes()
        {
            return ReadAsBytesInternal();
        }

        /// <summary>
        /// Reads the next JSON token from the stream as a <see cref="Nullable{Decimal}"/>.
        /// </summary>
        /// <returns>A <see cref="Nullable{Decimal}"/>. This method will return <c>null</c> at the end of an array.</returns>
        public override decimal? ReadAsDecimal()
        {
            return ReadAsDecimalInternal();
        }

        /// <summary>
        /// Reads the next JSON token from the stream as a <see cref="Nullable{Int32}"/>.
        /// </summary>
        /// <returns>A <see cref="Nullable{Int32}"/>. This method will return <c>null</c> at the end of an array.</returns>
        public override int? ReadAsInt32()
        {
            return ReadAsInt32Internal();
        }

        /// <summary>
        /// Reads the next JSON token from the stream as a <see cref="String"/>.
        /// </summary>
        /// <returns>A <see cref="String"/>. This method will return <c>null</c> at the end of an array.</returns>
        public override string ReadAsString()
        {
            return ReadAsStringInternal();
        }

        /// <summary>
        /// Reads the next JSON token from the stream as a <see cref="Nullable{DateTime}"/>.
        /// </summary>
        /// <returns>A <see cref="String"/>. This method will return <c>null</c> at the end of an array.</returns>
        public override DateTime? ReadAsDateTime()
        {
            return ReadAsDateTimeInternal();
        }

#if !NET20
        /// <summary>
        /// Reads the next JSON token from the stream as a <see cref="Nullable{DateTimeOffset}"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="Nullable{DateTimeOffset}"/>. This method will return <c>null</c> at the end of an array.
        /// </returns>
        public override DateTimeOffset? ReadAsDateTimeOffset()
        {
            return ReadAsDateTimeOffsetInternal();
        }
#endif

        /// <summary>
        /// Reads the next JSON token from the stream.
        /// </summary>
        /// <returns>
        /// true if the next token was read successfully; false if there are no more tokens to read.
        /// </returns>
        public override bool Read()
        {
            _readType = Json.ReadType.Read;

            return ReadInternal();
        }

		internal override bool ReadInternal()
		{
			try
			{
				bool success;

				success = ReadNormal();

				if (!success)
				{
					SetToken(JsonToken.None);
					return false;
				}

				return true;
			}
			catch (EndOfStreamException)
			{
				SetToken(JsonToken.None);
				return false;
			}

			return true;
		}

        /// <summary>
        /// Changes the <see cref="JsonReader.State"/> to Closed.
        /// </summary>
        public override void Close()
        {
            base.Close();

            if (CloseInput && _reader != null)
#if !(NETFX_CORE || PORTABLE40 || PORTABLE)
                _reader.Close();
#else
                _reader.Dispose();
#endif
        }

        private bool ReadNormal()
        {
            switch (CurrentState)
            {
                case State.Start:
                {
                    JsonToken token = (!_readRootValueAsArray) ? JsonToken.StartObject : JsonToken.StartArray;
					SmileType type = (!_readRootValueAsArray) ? SmileType.Object : SmileType.Array;

					_currentElementType = ReadType();
					type = _currentElementType;

                    SetToken(token);
                    ContainerContext newContext = new ContainerContext(type);
                    PushContext(newContext);
                    return true;
                }
                case State.Complete:
                case State.Closed:
                    return false;
                case State.Property:
                {
                    ReadType(_currentElementType);
                    return true;
                }
				case State.ObjectStart:
				case State.ArrayStart:
				case State.PostValue:
				{
					ContainerContext context = _currentContext;
					if (context == null)
						return false;

					byte keyType = this.ReadByte();
					if (keyType == (byte)0xF9 || keyType == (byte)0xFB)
					{
						PopContext();
						JsonToken endToken = (context.Type.TypeClass == SmileTypeClass.START_OBJECT) ? JsonToken.EndObject : JsonToken.EndArray;
						SetToken(endToken);
						return true;
					}

					if (_currentContext.Type.TypeClass == SmileTypeClass.START_ARRAY)
					{
						_currentElementType = SmileType.Parse(keyType);
						ReadType(_currentElementType);
					}
					else
						SetToken(JsonToken.PropertyName, ReadElement(keyType));

					return true;
				}
                case State.ConstructorStart:
                    break;
                case State.Constructor:
                    break;
                case State.Error:
                    break;
                case State.Finished:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return false;
        }

        private void PopContext()
        {
            _stack.RemoveAt(_stack.Count - 1);
            if (_stack.Count == 0)
                _currentContext = null;
            else
                _currentContext = _stack[_stack.Count - 1];
        }

        private void PushContext(ContainerContext newContext)
        {
            _stack.Add(newContext);
            _currentContext = newContext;
        }

        private byte ReadByte()
        {
            return _reader.ReadByte();
        }

		private int ReadZigzagNumber(int value)
		{
			throw new NotImplementedException();
		}

		private int ReadZigzag32()
		{
			int value = this._reader.ReadByte();
			int i;
			 if (value < 0) { // 6 bits
				 value &= 0x3F;
			 } else {
				 i = this._reader.ReadSByte();
				 if (i >= 0) { // 13 bits
					 value = (value << 7) + i;
					 i = this._reader.ReadSByte();
					 if (i >= 0) {
						 value = (value << 7) + i;
						 i = this._reader.ReadSByte();
						 if (i >= 0) {
							 value = (value << 7) + i;
							 // and then we must get negative
							 i = this._reader.ReadSByte();
							 if (i >= 0) {
								 throw new Exception("Corrupt input; 32-bit VInt extends beyond 5 data bytes");
							 }
						 }
					 }
				 }
				 value = (value << 6) + (i & 0x3F);
			 }
			value = SmileUtil.zigzagDecode(value);
			return value;
		}

		private long ReadZigzag64()
		{
			int i = this._reader.ReadSByte(); // first 7 bits
			i = (i << 7) + this._reader.ReadSByte(); // 14 bits
			i = (i << 7) + this._reader.ReadSByte(); // 21
			i = (i << 7) + this._reader.ReadSByte();

			int ptr = 4;
			int maxEnd = 11;

			// Ok: couple of bytes more
			long l = i;
			do {
				int value = this._reader.ReadSByte();
				ptr++;
				if (value < 0)
				{
					l = (l << 6) + (value & 0x3F);
					return SmileUtil.zigzagDecode(l);
				}
				l = (l << 7) + value;
			} while (ptr < maxEnd);
			throw new FormatException("bad zigzag64");
		}

		private long ReadBigInteger()
		{
			throw new NotImplementedException();
		}

		private int _Read4BytesToInteger()
		{
			byte[] buf = this._reader.ReadBytes(4);
			int value = buf[0];
			for (int i = 1; i < 4; i++)
				value = (value << 7) + (sbyte)buf[i];
			return value;
		}

		private static float Int32BitsToFloat(int value)
		{
			byte[] bytes = BitConverter.GetBytes(value);
			return BitConverter.ToSingle(bytes, 0);
		}

		private float ReadFloat32()
		{
			// just need 5 bytes to get int32 first; all are unsigned
			int i = _Read4BytesToInteger();
			i = (i << 7) + this._reader.ReadSByte();
			return Int32BitsToFloat(i);
		}

		private double ReadFloat64()
		{
			// ok; let's take two sets of 4 bytes (each is int)
			long hi = _Read4BytesToInteger();
			long value = (hi << 28) + (long)_Read4BytesToInteger();
			value = (value << 7) + this._reader.ReadSByte();
			value = (value << 7) + this._reader.ReadSByte();
			return BitConverter.Int64BitsToDouble(value);
		}

		private Decimal ReadBigDecimal()
		{
			throw new NotImplementedException();
		}

		private void ReadType(SmileType type)
		{
			switch (type.TypeClass)
			{
				case SmileTypeClass.TinyASCII:
				case SmileTypeClass.ShortASCII:
					{
						int length = type.Value + 1;
						SetToken(JsonToken.String, ReadStringInLength(length));
						break;
					}
				case SmileTypeClass.TinyUnicode:
				case SmileTypeClass.ShortUnicode:
					{
						int length = type.Value + 2;
						SetToken(JsonToken.String, ReadStringInLength(length));
						break;
					}
				case SmileTypeClass.SmallIntegers:
					{
						SetToken(JsonToken.Integer, ReadZigzagNumber(type.Value));
						break;
					}
				case SmileTypeClass.SimpleLiteralsNumbers:
					{
						switch (type.Value)
						{
							case 0x00:
								SetToken(JsonToken.String, String.Empty);
								break;
							case 0x01:
								SetToken(JsonToken.Null);
								break;
							case 0x02:
								SetToken(JsonToken.Boolean, false);
								break;
							case 0x03:
								SetToken(JsonToken.Boolean, true);
								break;
							case 0x04:
								SetToken(JsonToken.Integer, ReadZigzag32());
								break;
							case 0x05:
								SetToken(JsonToken.Integer, ReadZigzag64());
								break;
							case 0x06:
								SetToken(JsonToken.Integer, ReadBigInteger());
								break;
							case 0x08:
								SetToken(JsonToken.Float, ReadFloat32());
								break;
							case 0x09:
								SetToken(JsonToken.Float, ReadFloat64());
								break;
							case 0x0A:
								SetToken(JsonToken.Float, ReadBigDecimal());
								break;
							default:
								throw new ArgumentOutOfRangeException();
								break;
						}
						break;
					}
				case SmileTypeClass.START_OBJECT:
					{
						SetToken(JsonToken.StartObject);

						ContainerContext newContext = new ContainerContext(SmileType.Object);
						PushContext(newContext);
						//newContext.Length = ReadInt32();
						break;
					}
				case SmileTypeClass.START_ARRAY:
					{
						SetToken(JsonToken.StartArray);

						ContainerContext newContext = new ContainerContext(SmileType.Array);
						PushContext(newContext);
						//newContext.Length = ReadInt32();
						break;
					}
				case SmileTypeClass.Binary:
					{
						SmileBinaryType binaryType;
						byte[] data = ReadBinary(out binaryType);

						object value = (binaryType != SmileBinaryType.Uuid)
							? data
							: (object)new Guid(data);

						SetToken(JsonToken.Bytes, value);
						break;
					}
				case SmileTypeClass.Binary7bitEncoded:
					{
						byte[] data = Read7BitBinaryWithLength();
						SetToken(JsonToken.Bytes, data);
						break;
					}
				case SmileTypeClass.LongASCIIText:
					{
						byte[] data = ReadFCStringBuff();
						string text;
						if (!SmileUtil.IsASCIIString(data, out text))
							throw new FormatException("Wrong encoding.");
						SetToken(JsonToken.String, text);
					}
					break;
				case SmileTypeClass.LongUnicodeText:
					{
						byte[] data = ReadFCStringBuff();
						string text = Encoding.UTF8.GetString(data, 0, data.Length);
						SetToken(JsonToken.String, text);
					}
					break;
				default:
					throw new ArgumentOutOfRangeException("type", "Unexpected SmileType value: " + type.TypeClass);
			}
		}

		private byte[] ReadBinary(out SmileBinaryType binaryType)
        {
            int dataLength = ReadInt32();

			binaryType = (SmileBinaryType)ReadByte();

#pragma warning disable 612,618
            // the old binary type has the data length repeated in the data for some reason
			if (binaryType == SmileBinaryType.BinaryOld && !_jsonNet35BinaryCompatibility)
            {
                dataLength = ReadInt32();
            }
#pragma warning restore 612,618

            return ReadBytes(dataLength);
        }


		private string ReadString()
		{
			byte b = ReadByte();
			return ReadString(b);
		}

		List<string> SharedKeyNames = new List<string>();
		private string ReadString(byte b)
		{
			if (b >= 0x80 && b <= 0xBF)
			{
				int l = b - 0x80 + 1;
				byte[] buf = this.ReadBytes(l);
				string name;
				if (!SmileUtil.IsASCIIString(buf, out name))
					throw new FormatException("Wrong encoding.");
				SharedKeyNames.Add(name);
				return name;
			}
			else if (b >= 0xC0 && b <= 0xF7)
			{
				int l = b - 0xC0 + 1;
				byte[] buf = this.ReadBytes(l);
				string name = Encoding.UTF8.GetString(buf, 0, buf.Length);
				SharedKeyNames.Add(name);
				return name;
			}
			else if (b == 0x20)
				return string.Empty;
			else if (b >= 0x30 && b <= 0x33) //"Long" shared key name reference 
				throw new NotImplementedException();
			else if (b == 0x34)	//Long (not-yet-shared) Unicode name. Variable-length String
				throw new NotImplementedException();
			else if (b == 0x3A)
				throw new ArgumentOutOfRangeException();
			else if (b >= 0x40 && b <= 0x7F)
			{
				int i = b - 0x40;
				if (this.SharedKeyNames.Count <= i)
					throw new IndexOutOfRangeException();
				return this.SharedKeyNames[i];
			}
			//else if (b == 0xFB) not going here.
			else
			{
				throw new Exception(string.Format(CultureInfo.CurrentCulture, "bad string type: {0:X}", b));
			}
		}

		private byte[] ReadFCStringBuff()
		{
			byte b;
			using (MemoryStream ms = new MemoryStream())
			{
				while ((b = this._reader.ReadByte()) != (byte)VALUE_CONSTANTS.StringEndMarker)
					ms.WriteByte(b);
				return ms.ToArray();
			}
		}

		//private string ReadStringInLength(int length, Encoding encoding)
		//{
		//	byte[] buf = this.ReadBytes(length);
		//	return encoding.GetString(buf, 0, buf.Length);
		//}
		private string ReadStringInLength(int length)
		{
			byte[] buf = this.ReadBytes(length);
			return Encoding.UTF8.GetString(buf, 0, buf.Length);
		}

        private int ReadInt32()
        {
            return _reader.ReadInt32();
        }

        private long ReadInt64()
        {
            return _reader.ReadInt64();
        }

		private SmileType ReadType()
		{
			return SmileType.Parse(_reader.ReadByte());
		}

        private byte[] ReadBytes(int count)
        {
            return _reader.ReadBytes(count);
        }

		private int ReadUnsignedVInt()
		{
			int value = 0;
			while (true) {
				int i = this._reader.ReadSByte();
				if (i < 0) { // last byte
					value = (value << 6) + (i & 0x3F);
					return value;
				}
				value = (value << 7) + i;
			}
		}

		private byte[] Read7BitBinaryWithLength()
		{
			int byteLen = this.ReadUnsignedVInt();

			byte[] result = new byte[byteLen];
			int ptr = 0;
			int lastOkPtr = byteLen - 7;
        
			// first, read all 7-by-8 byte chunks
			while (ptr <= lastOkPtr) {
				int i1 = (this._reader.ReadByte() << 25)
					+ (this._reader.ReadByte() << 18)
					+ (this._reader.ReadByte() << 11)
					+ (this._reader.ReadByte() << 4);
				int x = this._reader.ReadByte();
				i1 += x >> 3;
				int i2 = ((x & 0x7) << 21)
					+ (this._reader.ReadByte() << 14)
					+ (this._reader.ReadByte() << 7)
					+ this._reader.ReadByte();
				// Ok: got our 7 bytes, just need to split, copy
				result[ptr++] = (byte)(i1 >> 24);
				result[ptr++] = (byte)(i1 >> 16);
				result[ptr++] = (byte)(i1 >> 8);
				result[ptr++] = (byte)i1;
				result[ptr++] = (byte)(i2 >> 16);
				result[ptr++] = (byte)(i2 >> 8);
				result[ptr++] = (byte)i2;
			}
			// and then leftovers: n+1 bytes to decode n bytes
			int toDecode = (result.Length - ptr);
			if (toDecode > 0) {
				int value = this._reader.ReadByte();
				for (int i = 1; i < toDecode; ++i) {
					value = (value << 7) + this._reader.ReadByte();
					result[ptr++] = (byte) (value >> (7 - i));
				}
				// last byte is different, has remaining 1 - 6 bits, right-aligned
				value <<= toDecode;
				result[ptr] = (byte)(value + this._reader.ReadByte());
			}
			return result;
		}

    }
}