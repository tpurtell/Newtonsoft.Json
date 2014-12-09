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

namespace Newtonsoft.Json.Smile
{
	//TODO: uses VALUE_CONSTANTS
	public enum SmileTypeClass : byte
	{
		ShortSharedValueStringReference = 0x00,
		SimpleLiteralsNumbers = 0x20,
		TinyASCII = 0x40,
		ShortASCII = 0x60,
		TinyUnicode = 0x80,
		ShortUnicode = 0xA0,
		SmallIntegers = 0xC0,
		LongASCIIText = 0xE0,
		LongUnicodeText = 0xE4,
		Binary7bitEncoded = 0xE8,
		SharedStringReference = 0xEC,
		START_ARRAY = 0xF8,
		END_ARRAY = 0xF9,
		START_OBJECT = 0xFA,
		END_OBJECT = 0xFB,
		//end-of-String = 0xFC,
		Binary = 0xFD,
		//end-of-content = 0xFF,
	}

	public class SmileType
	{
		public SmileTypeClass TypeClass;
		public int Value;

		public SmileType(SmileTypeClass type)
		{
			this.TypeClass = type;
			this.Value = 0;
		}
		public SmileType(byte type)
		{
			this._Parse(type);
		}

		private void _Parse(byte type)
		{
			if (type < (byte)SmileTypeClass.LongASCIIText)
			{
				byte b = (byte)(type & (byte)0xE0);
				this.TypeClass = (SmileTypeClass)b;
				this.Value = type - b;
			}
			else
			{
				this.TypeClass = (SmileTypeClass)type;
				this.Value = 0;
			}
		}

		public static SmileType Parse(byte type)
		{
			return new SmileType(type);
		}

		//Currently, Object and Array can be shared but we don't share them.
		//public static SmileType Object = new SmileType(SmileTypeClass.START_OBJECT);
		//public static SmileType Array = new SmileType(SmileTypeClass.START_ARRAY);
		public static SmileType NewObject()
		{
			return new SmileType(SmileTypeClass.START_OBJECT);
		}
		public static SmileType NewArray()
		{
			return new SmileType(SmileTypeClass.START_ARRAY);
		}
	}
}