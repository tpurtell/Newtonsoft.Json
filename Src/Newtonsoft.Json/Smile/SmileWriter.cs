using Newtonsoft.Json.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Newtonsoft.Json.Smile
{
	public class SmileWriter : JsonWriter
	{
		private readonly SmileBinaryWriter _writer;

        public SmileWriter(Stream stream)
        {
            ValidationUtils.ArgumentNotNull(stream, "stream");
            _writer = new SmileBinaryWriter(new BinaryWriter(stream));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BsonWriter"/> class.
        /// </summary>
        /// <param name="writer">The writer.</param>
		public SmileWriter(BinaryWriter writer)
        {
            ValidationUtils.ArgumentNotNull(writer, "writer");
            _writer = new SmileBinaryWriter(writer);
        }

		public override void Flush()
		{
			this._writer.Flush();
		}

		/// <summary>
		/// Writes the end.
		/// </summary>
		/// <param name="token">The token.</param>
		protected override void WriteEnd(JsonToken token)
		{
			base.WriteEnd(token);

			if (token == JsonToken.EndObject)
			{
				_writer.WriteEndObject();
			}
			else if (token == JsonToken.EndArray)
			{
				_writer.WriteEndArray();
			}
			else
				throw new NotImplementedException();

			//RemoveParent();

			if (Top == 0)
			{
				//_writer.WriteToken(_root);
			}
		}

		/// <summary>
		/// Writes out a comment <code>/*...*/</code> containing the specified text.
		/// </summary>
		/// <param name="text">Text to place inside the comment.</param>
		public override void WriteComment(string text)
		{
			throw JsonWriterException.Create(this, "Cannot write JSON comment as BSON.", null);
		}

		/// <summary>
		/// Writes the start of a constructor with the given name.
		/// </summary>
		/// <param name="name">The name of the constructor.</param>
		public override void WriteStartConstructor(string name)
		{
			throw JsonWriterException.Create(this, "Cannot write JSON constructor as BSON.", null);
		}

		/// <summary>
		/// Writes raw JSON.
		/// </summary>
		/// <param name="json">The raw JSON to write.</param>
		public override void WriteRaw(string json)
		{
			throw JsonWriterException.Create(this, "Cannot write raw JSON as BSON.", null);
		}

		/// <summary>
		/// Writes raw JSON where a value is expected and updates the writer's state.
		/// </summary>
		/// <param name="json">The raw JSON to write.</param>
		public override void WriteRawValue(string json)
		{
			throw JsonWriterException.Create(this, "Cannot write raw JSON as BSON.", null);
		}

		private bool IsHeaderWriten = false;

		/// <summary>
		/// Writes the beginning of a Json array.
		/// </summary>
		public override void WriteStartArray()
		{
			base.WriteStartArray();

			if (!this.IsHeaderWriten)
			{
				_writer.WriteHeader();
				this.IsHeaderWriten = true;
			}

			_writer.WriteStartArray();

		}
		/// <summary>
		/// Writes the beginning of a Json object.
		/// </summary>
		public override void WriteStartObject()
		{
			base.WriteStartObject();

			if (!this.IsHeaderWriten)
			{
				_writer.WriteHeader();
				this.IsHeaderWriten = true;
			}

			_writer.WriteStartObject();
		}

		private Dictionary<string, int> PropertyReferences = new Dictionary<string, int>();
		private int CurrentPropertyReferenceIndex = 0;
		/// <summary>
		/// Writes the property name of a name/value pair on a Json object.
		/// </summary>
		/// <param name="name">The name of the property.</param>
		public override void WritePropertyName(string name)
		{
			base.WritePropertyName(name);

			int ref_index = 0;
			if (!this.PropertyReferences.TryGetValue(name, out ref_index))
			{
				bool toShared = false;
				byte[] buf;
				if (SmileUtil.IsASCIIBytes(name, out buf))
					toShared = _writer.WriteAsciiPropertyName(buf);
				else
					toShared = _writer.WriteUnicodePropertyName(buf);

				if (toShared)
				{
					ref_index = CurrentPropertyReferenceIndex;
					this.PropertyReferences.Add(name, ref_index);
					CurrentPropertyReferenceIndex++;
				}
			}
			else
				_writer.WriteShortReferencePropertyName(ref_index);

		}

		/// <summary>
		/// Closes this stream and the underlying stream.
		/// </summary>
		public override void Close()
		{
			base.Close();

			if (CloseOutput && _writer != null)
				_writer.Close();
		}

		#region WriteValue methods
		/// <summary>
		/// Writes a <see cref="Object"/> value.
		/// An error will raised if the value cannot be written as a single JSON token.
		/// </summary>
		/// <param name="value">The <see cref="Object"/> value to write.</param>
		public override void WriteValue(object value)
		{
#if !(NET20 || NET35 || PORTABLE || PORTABLE40)
			if (value is BigInteger)
			{
				InternalWriteValue(JsonToken.Integer);
				//AddToken(new BsonBinary(((BigInteger)value).ToByteArray(), BsonBinaryType.Binary));
				_writer.WriteBigInteger((BigInteger)value);
			}
			else
#endif
			{
				base.WriteValue(value);
			}
		}

		/// <summary>
		/// Writes a null value.
		/// </summary>
		public override void WriteNull()
		{
			base.WriteNull();
			//AddValue(null, BsonType.Null);
			_writer.WriteNull();
		}

		/// <summary>
		/// Writes an undefined value.
		/// </summary>
		public override void WriteUndefined()
		{
			base.WriteUndefined();
			//AddValue(null, BsonType.Undefined);
			_writer.WriteUndefined();
		}

		/// <summary>
		/// Writes a <see cref="String"/> value.
		/// </summary>
		/// <param name="value">The <see cref="String"/> value to write.</param>
		public override void WriteValue(string value)
		{
			//TODO: shared string value
			base.WriteValue(value);
			if (value == null)
				_writer.WriteNull();
			else
				_writer.WriteString(value);
		}

		/// <summary>
		/// Writes a <see cref="Int32"/> value.
		/// </summary>
		/// <param name="value">The <see cref="Int32"/> value to write.</param>
		public override void WriteValue(int value)
		{
			base.WriteValue(value);
			//AddValue(value, BsonType.Integer);
			_writer.WriteInteger(value);
		}

		/// <summary>
		/// Writes a <see cref="UInt32"/> value.
		/// </summary>
		/// <param name="value">The <see cref="UInt32"/> value to write.</param>
		[CLSCompliant(false)]
		public override void WriteValue(uint value)
		{
			if (value > int.MaxValue)
				throw JsonWriterException.Create(this, "Value is too large to fit in a signed 32 bit integer. BSON does not support unsigned values.", null);

			base.WriteValue(value);
			//AddValue(value, BsonType.Integer);
			_writer.WriteInteger((int)value);
		}

		/// <summary>
		/// Writes a <see cref="Int64"/> value.
		/// </summary>
		/// <param name="value">The <see cref="Int64"/> value to write.</param>
		public override void WriteValue(long value)
		{
			base.WriteValue(value);
			//AddValue(value, BsonType.Long);
			_writer.WriteLong(value);
		}

		/// <summary>
		/// Writes a <see cref="UInt64"/> value.
		/// </summary>
		/// <param name="value">The <see cref="UInt64"/> value to write.</param>
		[CLSCompliant(false)]
		public override void WriteValue(ulong value)
		{
			if (value > long.MaxValue)
				throw JsonWriterException.Create(this, "Value is too large to fit in a signed 64 bit integer. BSON does not support unsigned values.", null);

			base.WriteValue(value);
			//AddValue(value, BsonType.Long);
			_writer.WriteLong((long)value);
		}

		/// <summary>
		/// Writes a <see cref="Single"/> value.
		/// </summary>
		/// <param name="value">The <see cref="Single"/> value to write.</param>
		public override void WriteValue(float value)
		{
			base.WriteValue(value);
			//AddValue(value, BsonType.Number);
			_writer.WriteFloat(value);
		}

		/// <summary>
		/// Writes a <see cref="Double"/> value.
		/// </summary>
		/// <param name="value">The <see cref="Double"/> value to write.</param>
		public override void WriteValue(double value)
		{
			base.WriteValue(value);
			//AddValue(value, BsonType.Number);
			_writer.WriteDouble(value);
		}

		/// <summary>
		/// Writes a <see cref="Boolean"/> value.
		/// </summary>
		/// <param name="value">The <see cref="Boolean"/> value to write.</param>
		public override void WriteValue(bool value)
		{
			base.WriteValue(value);
			//AddValue(value, BsonType.Boolean);
			_writer.WriteBool(value);
		}

		/// <summary>
		/// Writes a <see cref="Int16"/> value.
		/// </summary>
		/// <param name="value">The <see cref="Int16"/> value to write.</param>
		public override void WriteValue(short value)
		{
			base.WriteValue(value);
			//AddValue(value, BsonType.Integer);
			_writer.WriteInteger(value);
		}

		/// <summary>
		/// Writes a <see cref="UInt16"/> value.
		/// </summary>
		/// <param name="value">The <see cref="UInt16"/> value to write.</param>
		[CLSCompliant(false)]
		public override void WriteValue(ushort value)
		{
			base.WriteValue(value);
			//AddValue(value, BsonType.Integer);
			_writer.WriteInteger(value);
		}

		/// <summary>
		/// Writes a <see cref="Char"/> value.
		/// </summary>
		/// <param name="value">The <see cref="Char"/> value to write.</param>
		public override void WriteValue(char value)
		{
			base.WriteValue(value);
			string s = null;
#if !(NETFX_CORE || PORTABLE40 || PORTABLE)
			s = value.ToString(CultureInfo.InvariantCulture);
#else
            s = value.ToString();
#endif
			//AddToken(new BsonString(s, true));
			_writer.WriteString(s);
		}

		/// <summary>
		/// Writes a <see cref="Byte"/> value.
		/// </summary>
		/// <param name="value">The <see cref="Byte"/> value to write.</param>
		public override void WriteValue(byte value)
		{
			base.WriteValue(value);
			//AddValue(value, BsonType.Integer);
			_writer.WriteByte(value);
		}

		/// <summary>
		/// Writes a <see cref="SByte"/> value.
		/// </summary>
		/// <param name="value">The <see cref="SByte"/> value to write.</param>
		[CLSCompliant(false)]
		public override void WriteValue(sbyte value)
		{
			base.WriteValue(value);
			//AddValue(value, BsonType.Integer);
			_writer.WriteSByte(value);
		}

		/// <summary>
		/// Writes a <see cref="Decimal"/> value.
		/// </summary>
		/// <param name="value">The <see cref="Decimal"/> value to write.</param>
		public override void WriteValue(decimal value)
		{
			base.WriteValue(value);
			//AddValue(value, BsonType.Number);
			_writer.WriteDecimal(value);
		}

		/// <summary>
		/// Writes a <see cref="DateTime"/> value.
		/// </summary>
		/// <param name="value">The <see cref="DateTime"/> value to write.</param>
		public override void WriteValue(DateTime value)
		{
			base.WriteValue(value);
			value = DateTimeUtils.EnsureDateTime(value, DateTimeZoneHandling);
			//AddValue(value, BsonType.Date);
			_writer.WriteDate(value);
		}

#if !NET20
		/// <summary>
		/// Writes a <see cref="DateTimeOffset"/> value.
		/// </summary>
		/// <param name="value">The <see cref="DateTimeOffset"/> value to write.</param>
		public override void WriteValue(DateTimeOffset value)
		{
			base.WriteValue(value);
			//AddValue(value, BsonType.Date);
			_writer.WriteDate(value);
		}
#endif

		/// <summary>
		/// Writes a <see cref="Byte"/>[] value.
		/// </summary>
		/// <param name="value">The <see cref="Byte"/>[] value to write.</param>
		public override void WriteValue(byte[] value)
		{
			base.WriteValue(value);
			//AddToken(new BsonBinary(value, BsonBinaryType.Binary));
			_writer.WriteBytes(value);
		}

		/// <summary>
		/// Writes a <see cref="Guid"/> value.
		/// </summary>
		/// <param name="value">The <see cref="Guid"/> value to write.</param>
		public override void WriteValue(Guid value)
		{
			base.WriteValue(value);
			//AddToken(new BsonBinary(value.ToByteArray(), BsonBinaryType.Uuid));
			_writer.WriteBytes(value.ToByteArray());
		}

		/// <summary>
		/// Writes a <see cref="TimeSpan"/> value.
		/// </summary>
		/// <param name="value">The <see cref="TimeSpan"/> value to write.</param>
		public override void WriteValue(TimeSpan value)
		{
			base.WriteValue(value);
			//AddToken(new BsonString(value.ToString(), true));
			_writer.WriteString(value.ToString());
		}

		/// <summary>
		/// Writes a <see cref="Uri"/> value.
		/// </summary>
		/// <param name="value">The <see cref="Uri"/> value to write.</param>
		public override void WriteValue(Uri value)
		{
			base.WriteValue(value);
			//AddToken(new BsonString(value.ToString(), true));
			_writer.WriteString(value.ToString());
		}
		#endregion
	}
}
