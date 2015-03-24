﻿#region License
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
using System.Runtime.Remoting.Metadata.W3cXsd2001;
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
	public partial class SmileReaderTests : TestFixtureBase
    {
        [Test]
        public void ReadSingleObject_LongToInt()
        {
			JsonSerializer serializer = new JsonSerializer();

			byte[] data = HexToBytes(SmileTestData.User1);

            MemoryStream ms = new MemoryStream(data);

			SmileReader reader = new SmileReader(ms, false, DateTimeKind.Local);
			People people = serializer.Deserialize<People>(reader);

			this.VerifyUser1(people);
		}

        [Test]
        public void ReadOne()
        {
            JsonSerializer serializer = new JsonSerializer();

            byte[] data = SoapHexBinary.Parse("3A290A01FA8072FA8023CA8062FA817574FA43FA807022806123806C6E6C6F6E6764616E3A2F2F4F4E452F6C64746573742D312F6B4B77717843634E316D784F4A695A52324B44464C673D3DFBFBFBFBFB").Value;

            MemoryStream ms = new MemoryStream(data);

            SmileReader reader = new SmileReader(ms, false, DateTimeKind.Local);
            var x = serializer.Deserialize<JObject>(reader);
        }
        [Test]
        public void ReadTwo()
        {
            JsonSerializer serializer = new JsonSerializer();

            byte[] data = SoapHexBinary.Parse("3A290A01FA8072FA8023C6803DFA807624A6FBFBFB").Value;

            MemoryStream ms = new MemoryStream(data);

            SmileReader reader = new SmileReader(ms, false, DateTimeKind.Local);
            var x = serializer.Deserialize<JObject>(reader);
        }

        [Test]
        public void ReadThree()
        {   
			JsonSerializer serializer = new JsonSerializer();

            byte[] data = SoapHexBinary.Parse("3A290A01FA8072FA802324A08069FA8064FA43F8F98070228077C0FBFBFBFB").Value;

            MemoryStream ms = new MemoryStream(data);

            SmileReader reader = new SmileReader(ms, false, DateTimeKind.Local);
            var x = serializer.Deserialize<JObject>(reader);
        }
		[Test]
		public void ReadSomeComplicatedDictionary() 
		{
			JsonSerializer serializer = new JsonSerializer ();
			byte[] data = SoapHexBinary.Parse ("3A290A01FA8072FA8023D4826F6173FA806CFA43FA817369FA8069F8FA45FA44FA816964FA8169744A537469636B65725061636B806353365A5334584142534B44514C395548384E54314A806157666266373264396461363035643463626236663234313837FBFBFB8073FA816969FA8070485075626C69736865648024FA806623827573642900000000000000000000FB48250125794869668A806D25012579487A62A8FBFB8075FA44FA806E465075736865656E4C6E6C6F6E6764616E3A2F2F4F4E452F6C64746573742D312F5A6378465848384B586E434E63416E5F5F786C5F6D773D3D4AF8FA454F613166363031663061663061396361338077240CA480682410B88174626E6C6F6E6764616E3A2F2F4F4E452F6C64746573742D312F6970457A4257646F372D496E366A6E494E35413450773D3D8166626E6C6F6E6764616E3A2F2F4F4E452F6C64746573742D312F356D466A38364F657538456E394C7A4D3764534866673D3DFBFA454F62363933646339653133393634353836532410B854240D96556E6C6F6E6764616E3A2F2F4F4E452F6C64746573742D312F643054725137624D68666250554F4C6F5833477978413D3D566E6C6F6E6764616E3A2F2F4F4E452F6C64746573742D312F2D6877507267367945735A7A434332765750414441673D3DFBFA454F63643832343162666361373766643164532410B854240FBE556E6C6F6E6764616E3A2F2F4F4E452F6C64746573742D312F64314B6744516E5F73475432364F78767166723432773D3D566E6C6F6E6764616E3A2F2F4F4E452F6C64746573742D312F4C526B6B6F654D506C356C35545361556A51487057673D3DFBFA454F61326363393065356232313738386630532410B854240FAA556E6C6F6E6764616E3A2F2F4F4E452F6C64746573742D312F755A4934477170614E38717078303073364C6C5252673D3D566E6C6F6E6764616E3A2F2F4F4E452F6C64746573742D312F5A6430303178396A556F587A615171446A42546B79773D3DFBFA454F34663761343639653936623761393832532410B8542410B8556E6C6F6E6764616E3A2F2F4F4E452F6C64746573742D312F446C757A4151695531354677755A615A445A7A4874513D3D566E6C6F6E6764616E3A2F2F4F4E452F6C64746573742D312F4D5A71446D375A3654547459725734616F5F71352D413D3DFBFA454F64333561343963316566316239393038532410B854240CB4556E6C6F6E6764616E3A2F2F4F4E452F6C64746573742D312F624F53413447544E675969344C3544515244584F65773D3D566E6C6F6E6764616E3A2F2F4F4E452F6C64746573742D312F39416D3648595A71324E4E6F4F636B414C76544F48773D3DFBFA454F34613637356530323264666137333635532410B854240BAE556E6C6F6E6764616E3A2F2F4F4E452F6C64746573742D312F714B67655555595A645377795644776330694E4270513D3D566E6C6F6E6764616E3A2F2F4F4E452F6C64746573742D312F61635335444146765A6559594351525A6376625039673D3DFBFA454F36303737663365383531326361646131532410B854240CA8556E6C6F6E6764616E3A2F2F4F4E452F6C64746573742D312F30316C6269713971777A6F52346345715044356C76413D3D566E6C6F6E6764616E3A2F2F4F4E452F6C64746573742D312F7353715A456F62624F3335566343794F665979366D413D3DFBFA454F39623563323164383938666463333635532410B854240CAA556E6C6F6E6764616E3A2F2F4F4E452F6C64746573742D312F485A3062706B346C7A64536D66794A7A35694D6543673D3D566E6C6F6E6764616E3A2F2F4F4E452F6C64746573742D312F2D764C68414F4D676D3847694454486A5259344E35773D3DFBFA454F3935626133326138323564613361383653241080542410B8556E6C6F6E6764616E3A2F2F4F4E452F6C64746573742D312F424E345444534C777661566B417A705857514E4644513D3D566E6C6F6E6764616E3A2F2F4F4E452F6C64746573742D312F456D424A6B742D79417134424C6A37476268513548773D3DFBFA454F3261613966623239336366346331373253240EB0542410B8556E6C6F6E6764616E3A2F2F4F4E452F6C64746573742D312F3444387170527052566679767873334F4B32793662513D3D566E6C6F6E6764616E3A2F2F4F4E452F6C64746573742D312F4744434E706C655A663563795449696F44764F6930513D3DFBFA454F34383133313465626263383934646233532410B854240E8C556E6C6F6E6764616E3A2F2F4F4E452F6C64746573742D312F2D624A63545438664154613834324361653538424F673D3D566E6C6F6E6764616E3A2F2F4F4E452F6C64746573742D312F6F4A5F74763144637571736978635A495371754D71673D3DFBFA454F3530346331316663613764643839616153240E9C542410B8556E6C6F6E6764616E3A2F2F4F4E452F6C64746573742D312F6A524F6877785A57357751634936634B4255435658413D3D566E6C6F6E6764616E3A2F2F4F4E452F6C64746573742D312F782D68456949664F4E4264625666793670735F6A46673D3DFBF9FBFBFBFA45FA44FA46FA474A537469636B65725061636B4853365A5334584142534B44514C395548384E54314A4957353637363334343665653831306461386638656363313332FBFBFB4AFA4BFA4C485075626C69736865644DFA4E234F2900000000000000000000FB48250125794B5359AA50250125794B5643A0FBFB51FA44FA5243466C6F774AF8FA454F37323365666135346664366634623131532410B854240FB6556E6C6F6E6764616E3A2F2F4F4E452F6C64746573742D312F4B756175746478706F6D4E374277465A366B775148673D3D566E6C6F6E6764616E3A2F2F4F4E452F6C64746573742D312F52644A697837306F3250416E6B5A74313837456536413D3DFBFA454F33386361666238303339313961333534532410B8542410B8556E6C6F6E6764616E3A2F2F4F4E452F6C64746573742D312F684A6E7863384B64546146715275734735304B782D413D3D566E6C6F6E6764616E3A2F2F4F4E452F6C64746573742D312F577A6B4C444878743749484C7548336D2D554C576B413D3DFBF9FBFBFBFA45FA44FA46FA474A537469636B65725061636B4853365A5334584142534B44514C395548384E54314A4957353637363334343665653831306461386638656363313332FBFBFB4AFA4BFA4C485075626C69736865644DFA4E234F2900000000000000000000FB48250125794B5359AA50250125794B5F5680FBFB51FA44FA52494375746520436F6F6E73816E74FA8030407B8031402080324022803340658034406E803540228036403A80374020803840228039404381313040758131314074813132406581313340208131344043813135406F813136406F813137406E81313840738131394022813230402C81323140208132324022813233406581323440738132354022813236403A81323740208132384022813239404C813330406F813331407381333240208133334043813334406F813335406F813336406E813337407381333840208133394042813430406F813431406E81343240698134334074813434406F813435407381343640228134374020813438407DFB80646D54686579277265206375746520616E6420637564646C792E20416E6420736F6D6574696D65732068756E6772792E816474FA58407B5940205A40225B40655C40735D40225E403A5F402060402261405362406F63406E64402065406C66406967406E68406469406F6A40736B40206C40796D40206E40746F406970406571407272406E73406F74407375402E7640207740597840207940617A40207B40767C40657D40637E40657F40733040402030414068304240613043406D3044406230454072304640653047402E304840228134394020813530407DFB4AF8FA454F37323365666135346664366634623131532410B854240FB6556E6C6F6E6764616E3A2F2F4F4E452F6C64746573742D312F4B756175746478706F6D4E374277465A366B775148673D3D566E6C6F6E6764616E3A2F2F4F4E452F6C64746573742D312F52644A697837306F3250416E6B5A74313837456536413D3DFBFA454F33386361666238303339313961333534532410B8542410B8556E6C6F6E6764616E3A2F2F4F4E452F6C64746573742D312F684A6E7863384B64546146715275734735304B782D413D3D566E6C6F6E6764616E3A2F2F4F4E452F6C64746573742D312F577A6B4C444878743749484C7548336D2D554C576B413D3DFBF9FBFBFBF9FBFBFBFBFBFB").Value;
			MemoryStream ms = new MemoryStream(data);
			SmileReader reader = new SmileReader(ms, false, DateTimeKind.Local);
			var x = serializer.Deserialize<JObject>(reader);
			Console.WriteLine (x);
		}
        [Test]
        public void ReadSingleObject()
        {
			JsonSerializer serializer = new JsonSerializer();

			byte[] data = HexToBytes(SmileTestData.User2);

            MemoryStream ms = new MemoryStream(data);

			SmileReader reader = new SmileReader(ms, false, DateTimeKind.Local);
			People people = serializer.Deserialize<People>(reader);

			this.VerifyUser2(people);

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

			byte[] data = HexToBytes(SmileTestData.Users);

			MemoryStream ms = new MemoryStream(data);

			SmileReader reader = new SmileReader(ms, true, DateTimeKind.Local);
			List<People> list = serializer.Deserialize<List<People>>(reader);

			Assert.NotNull(list);
			Assert.AreEqual(2, list.Count);

			People people = list.FirstOrDefault();
			this.VerifyUser1(people);

			people = list.Skip(1).FirstOrDefault();
			this.VerifyUser2(people);
		}
   }

	public partial class SmileReaderTests
	{
		void VerifyUser1(People people)
		{
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

			Assert.AreEqual(67.6543F, people.weight);
			Assert.AreEqual(181.2345, people.height);

			Assert.NotNull(people.notes);
			Assert.AreEqual(74, people.notes.Length);
			Assert.AreEqual('1', people.notes[0]);
			Assert.NotNull(people.notes_chinese);
			Assert.AreEqual(76, people.notes_chinese.Length);
			Assert.AreEqual('男', people.notes_chinese[0]);

			Assert.Null(people.null_string);
			Assert.AreEqual(-16, people.small_integer);
			Assert.AreEqual("abc65", people.long_column_65);
			Assert.AreEqual("abc175", people.long_column_175);
			Assert.AreEqual("sA", people.short_column);

		}

		void VerifyUser2(People people)
		{
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

			Assert.AreEqual(42.3456F, people.weight);
			Assert.AreEqual(168.7654, people.height);

			Assert.NotNull(people.notes);
			Assert.AreEqual(78, people.notes.Length);
			Assert.AreEqual('2', people.notes[0]);
			Assert.NotNull(people.notes_chinese);
			Assert.AreEqual(80, people.notes_chinese.Length);
			Assert.AreEqual('女', people.notes_chinese[0]);

			Assert.Null(people.null_string);
			Assert.AreEqual(12, people.small_integer);
			Assert.AreEqual("def65", people.long_column_65);
			Assert.AreEqual("def175", people.long_column_175);
			Assert.AreEqual("sB", people.short_column);
		}
	}
}