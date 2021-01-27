// Author:
// Leszek Ciesielski (skolima@gmail.com)
// Manuel Josupeit-Walter (josupeit-walter@cis-gmbh.de)
//
// (C) 2013 Wunderman Thompson Technology
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using ExposedObject;
using NUnit.Framework;
using TestSubjects;

namespace Tests
{
    [TestFixture]
    public class ClassWithInheritedPrivateMembersTest
    {
        [Test]
        public void PrivatePropertyTest()
        {
            dynamic exposed = Exposed.From(new ClassWithInheritedPrivateMembers());
            int count = exposed.Count;
            Assert.AreEqual(0, count);
            exposed.Count = 8;
            count = exposed.Count;
            Assert.AreEqual(8, count);
        }

        [Test]
        public void PrivateMethodTest()
        {
            dynamic exposed = Exposed.From(new ClassWithInheritedPrivateMembers());
            exposed.SetPassword("test pass");
            string password = exposed.Password;
            Assert.AreEqual("test pass", password);
        }

        [Test]
        public void PrivateFieldTest()
        {
            dynamic exposed = Exposed.From(new ClassWithInheritedPrivateMembers());
            int count = exposed._count;
            Assert.AreEqual(0, count);
            exposed._count = 8;
            count = exposed.Count;
            Assert.AreEqual(8, count);
        }
    }
}
