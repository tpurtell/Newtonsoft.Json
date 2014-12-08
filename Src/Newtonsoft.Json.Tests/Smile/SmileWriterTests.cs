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
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Utilities;
using Newtonsoft.Json.Tests.TestObjects;
using System.Globalization;
#if NET20
using Newtonsoft.Json.Utilities.LinqBridge;
#else
using System.Linq;

#endif

namespace Newtonsoft.Json.Tests.Smile
{
    [TestFixture]
    public class SmileWriterTests : TestFixtureBase
    {
		[Test]
		public void WriteSingleObject_LongToInt()
		{
			JsonSerializer serializer = new JsonSerializer();

			MemoryStream ms = new MemoryStream();
			SmileWriter writer = new SmileWriter(ms);

			People p1 = new People();
			p1.name = new People.Name();
			p1.name.first = "Joe";
			p1.name.last = "Sixpack";
			p1.gender = "MALE";
			p1.verified = false;
			p1.salary = 12345;
			p1.deposit = 12345L;
			p1.userImage = Convert.FromBase64String("Rm9vYmFyIQ==");

			serializer.Serialize(writer, p1);

			string smile = BytesToHex(ms.ToArray());
			Assert.AreEqual("3A-29-0A-01-FA-83-6E-61-6D-65-FA-84-66-69-72-73-74-42-4A-6F-65-83-6C-61-73-74-46-53-69-78-70-61-63-6B-FB-85-67-65-6E-64-65-72-43-4D-41-4C-45-87-76-65-72-69-66-69-65-64-22-85-73-61-6C-61-72-79-24-03-01-B2-86-64-65-70-6F-73-69-74-24-03-01-B2-88-75-73-65-72-49-6D-61-67-65-E8-87-23-1B-6D-76-13-05-64-21-FB", smile);
		}

		[Test]
        public void WriteSingleObject()
        {
			JsonSerializer serializer = new JsonSerializer();

            MemoryStream ms = new MemoryStream();
			SmileWriter writer = new SmileWriter(ms);

			//writer.WriteStartObject();
			//writer.WritePropertyName("name");
			//writer.WriteStartObject();
			//writer.WritePropertyName("first");
			//writer.WriteValue("Mary");
			//writer.WritePropertyName("last");
			//writer.WriteValue("Jane");
			//writer.WriteEndObject();
			//writer.WritePropertyName("gender");
			//writer.WriteValue("FEMALE");
			//writer.WritePropertyName("verified");
			//writer.WriteValue(true);
			//writer.WritePropertyName("salary");
			//writer.WriteValue(67890);
			//TODO: how to test 7bit encoded byte array?
			//writer.WritePropertyName("userImage");
			//writer.WriteValue("FooBar!!");			
			//writer.WriteEndObject();

			People p2 = new People();
			p2.name = new People.Name();
			p2.name.first = "Mary";
			p2.name.last = "Jane";
			p2.gender = "FEMALE";
			p2.verified = true;
			p2.salary = 67890;
			p2.deposit = 9876543210L;
			p2.userImage = Convert.FromBase64String("SEVMTE9XT1JMRCE=");

			serializer.Serialize(writer, p2);

			string smile = BytesToHex(ms.ToArray());
			Assert.AreEqual("3A-29-0A-01-FA-83-6E-61-6D-65-FA-84-66-69-72-73-74-43-4D-61-72-79-83-6C-61-73-74-43-4A-61-6E-65-FB-85-67-65-6E-64-65-72-45-46-45-4D-41-4C-45-87-76-65-72-69-66-69-65-64-23-85-73-61-6C-61-72-79-24-10-49-A4-86-64-65-70-6F-73-69-74-25-01-13-16-01-37-94-88-75-73-65-72-49-6D-61-67-65-E8-8B-24-11-29-44-62-3D-2E-4F-29-13-08-42-01-FB", smile);
        }

		[Test]
		public void WriteListObject()
		{
			JsonSerializer serializer = new JsonSerializer();

			MemoryStream ms = new MemoryStream();
			SmileWriter writer = new SmileWriter(ms);

			People p1 = new People();
			p1.name = new People.Name();
			p1.name.first = "Joe";
			p1.name.last = "Sixpack";
			p1.gender = "MALE";
			p1.verified = false;
			p1.salary = 12345;
			p1.deposit = 12345L;
			p1.userImage = Convert.FromBase64String("Rm9vYmFyIQ==");

			People p2 = new People();
			p2.name = new People.Name();
			p2.name.first = "Mary";
			p2.name.last = "Jane";
			p2.gender = "FEMALE";
			p2.verified = true;
			p2.salary = 67890;
			p2.deposit = 9876543210L;
			p2.userImage = Convert.FromBase64String("SEVMTE9XT1JMRCE=");

			List<People> list = new List<People>();
			list.Add(p1);
			list.Add(p2);
			serializer.Serialize(writer, list);

			string smile = BytesToHex(ms.ToArray());
			Assert.AreEqual("3A-29-0A-01-F8-FA-83-6E-61-6D-65-FA-84-66-69-72-73-74-42-4A-6F-65-83-6C-61-73-74-46-53-69-78-70-61-63-6B-FB-85-67-65-6E-64-65-72-43-4D-41-4C-45-87-76-65-72-69-66-69-65-64-22-85-73-61-6C-61-72-79-24-03-01-B2-86-64-65-70-6F-73-69-74-24-03-01-B2-88-75-73-65-72-49-6D-61-67-65-E8-87-23-1B-6D-76-13-05-64-21-FB-FA-40-FA-41-43-4D-61-72-79-42-43-4A-61-6E-65-FB-43-45-46-45-4D-41-4C-45-44-23-45-24-10-49-A4-46-25-01-13-16-01-37-94-47-E8-8B-24-11-29-44-62-3D-2E-4F-29-13-08-42-01-FB-F9", smile);
		}
	}
}