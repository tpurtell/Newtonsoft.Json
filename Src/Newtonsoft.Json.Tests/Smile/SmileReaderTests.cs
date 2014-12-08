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
using System.Reflection;
using System.Runtime.Serialization;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Tests.TestObjects;
#if !(NET20 || NET35 || PORTABLE || ASPNETCORE50)
using System.Numerics;
#endif
using System.Text;
using System.Text.RegularExpressions;
#if NETFX_CORE
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using TestFixture = Microsoft.VisualStudio.TestPlatform.UnitTestFramework.TestClassAttribute;
using Test = Microsoft.VisualStudio.TestPlatform.UnitTestFramework.TestMethodAttribute;
#elif ASPNETCORE50
using Xunit;
using Test = Xunit.FactAttribute;
using Assert = Newtonsoft.Json.Tests.XUnitAssert;
#else
using NUnit.Framework;
#endif
using Newtonsoft.Json.Smile;
using System.IO;
using Newtonsoft.Json.Tests.Serialization;
using Newtonsoft.Json.Utilities;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace Newtonsoft.Json.Tests.Smile
{
    [TestFixture]
    public class SmileReaderTests : TestFixtureBase
    {
        [Test]
        public void ReadSingleObject_LongToInt()
        {
			JsonSerializer serializer = new JsonSerializer();

			byte[] data = HexToBytes("3A-29-0A-01-FA-83-6E-61-6D-65-FA-84-66-69-72-73-74-42-4A-6F-65-83-6C-61-73-74-46-53-69-78-70-61-63-6B-FB-85-67-65-6E-64-65-72-43-4D-41-4C-45-87-76-65-72-69-66-69-65-64-22-85-73-61-6C-61-72-79-24-03-01-B2-86-64-65-70-6F-73-69-74-24-03-01-B2-88-75-73-65-72-49-6D-61-67-65-E8-87-23-1B-6D-76-13-05-64-21-FB");

            MemoryStream ms = new MemoryStream(data);

			SmileReader reader = new SmileReader(ms, false, DateTimeKind.Local);
			People people = serializer.Deserialize<People>(reader);

			Assert.NotNull(people);
			Assert.NotNull(people.name);
			Assert.AreEqual("Joe", people.name.first);
			Assert.AreEqual("Sixpack", people.name.last);

			Assert.AreEqual("MALE", people.gender);
			Assert.AreEqual(false, people.verified);

			Assert.AreEqual(12345, people.salary);
			Assert.AreEqual(12345L, people.deposit);

			Assert.NotNull(people.userImage);
			Assert.AreEqual(7, people.userImage.Length);
		}

        [Test]
        public void ReadSingleObject()
        {
			JsonSerializer serializer = new JsonSerializer();

			byte[] data = HexToBytes("3A-29-0A-01-FA-83-6E-61-6D-65-FA-84-66-69-72-73-74-43-4D-61-72-79-83-6C-61-73-74-43-4A-61-6E-65-FB-85-67-65-6E-64-65-72-45-46-45-4D-41-4C-45-87-76-65-72-69-66-69-65-64-23-85-73-61-6C-61-72-79-24-10-49-A4-86-64-65-70-6F-73-69-74-25-01-13-16-01-37-94-88-75-73-65-72-49-6D-61-67-65-E8-8B-24-11-29-44-62-3D-2E-4F-29-13-08-42-01-FB");

            MemoryStream ms = new MemoryStream(data);

			SmileReader reader = new SmileReader(ms, false, DateTimeKind.Local);
			People people = serializer.Deserialize<People>(reader);

			Assert.NotNull(people);
			Assert.NotNull(people.name);
			Assert.AreEqual("Mary", people.name.first);
			Assert.AreEqual("Jane", people.name.last);

			Assert.AreEqual("FEMALE", people.gender);
			Assert.AreEqual(true, people.verified);

			Assert.AreEqual(67890, people.salary);
			Assert.AreEqual(9876543210L, people.deposit);

			Assert.NotNull(people.userImage);
			Assert.AreEqual(11, people.userImage.Length);

			//Assert.IsTrue(reader.Read());
			//Assert.AreEqual(JsonToken.StartObject, reader.TokenType);

			//Assert.IsTrue(reader.Read());
			//Assert.AreEqual(JsonToken.PropertyName, reader.TokenType);
			//Assert.AreEqual("name", reader.Value);
			//Assert.AreEqual(typeof(string), reader.ValueType);

			//Assert.IsTrue(reader.Read());
			//Assert.AreEqual(JsonToken.Integer, reader.TokenType);
			//Assert.AreEqual(1L, reader.Value);
			//Assert.AreEqual(typeof(long), reader.ValueType);

			//Assert.IsTrue(reader.Read());
			//Assert.AreEqual(JsonToken.EndObject, reader.TokenType);

			//Assert.IsFalse(reader.Read());
			//Assert.AreEqual(JsonToken.None, reader.TokenType);
        }

		[Test]
		public void ReadListObject()
		{
			JsonSerializer serializer = new JsonSerializer();

			byte[] data = HexToBytes("3A-29-0A-01-F8-FA-83-6E-61-6D-65-FA-84-66-69-72-73-74-42-4A-6F-65-83-6C-61-73-74-46-53-69-78-70-61-63-6B-FB-85-67-65-6E-64-65-72-43-4D-41-4C-45-87-76-65-72-69-66-69-65-64-22-85-73-61-6C-61-72-79-24-03-01-B2-86-64-65-70-6F-73-69-74-24-03-01-B2-88-75-73-65-72-49-6D-61-67-65-E8-87-23-1B-6D-76-13-05-64-21-FB-FA-40-FA-41-43-4D-61-72-79-42-43-4A-61-6E-65-FB-43-45-46-45-4D-41-4C-45-44-23-45-24-10-49-A4-46-25-01-13-16-01-37-94-47-E8-8B-24-11-29-44-62-3D-2E-4F-29-13-08-42-01-FB-F9");

			MemoryStream ms = new MemoryStream(data);

			SmileReader reader = new SmileReader(ms, true, DateTimeKind.Local);
			List<People> list = serializer.Deserialize<List<People>>(reader);

			People people = list.FirstOrDefault();
			Assert.NotNull(people);

			Assert.NotNull(people.name);
			Assert.AreEqual("Joe", people.name.first);
			Assert.AreEqual("Sixpack", people.name.last);

			Assert.AreEqual("MALE", people.gender);
			Assert.AreEqual(false, people.verified);

			Assert.AreEqual(12345, people.salary);
			Assert.AreEqual(12345L, people.deposit);

			Assert.NotNull(people.userImage);
			Assert.AreEqual(7, people.userImage.Length);

			people = list.Skip(1).FirstOrDefault();
			Assert.NotNull(people);

			Assert.NotNull(people.name);
			Assert.AreEqual("Mary", people.name.first);
			Assert.AreEqual("Jane", people.name.last);

			Assert.AreEqual("FEMALE", people.gender);
			Assert.AreEqual(true, people.verified);

			Assert.AreEqual(67890, people.salary);
			Assert.AreEqual(9876543210L, people.deposit);

			Assert.NotNull(people.userImage);
			Assert.AreEqual(11, people.userImage.Length);
		}
   }
}