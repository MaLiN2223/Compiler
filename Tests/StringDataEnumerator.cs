using System.Linq;
using Compiler.Language;
using Compiler.Lexing;
using NSubstitute;
using Xunit;

namespace Tests
{
	//public class StringDataEnumeratorTests
	//{
	//	[Fact]
	//	public void EmptyTest()
	//	{
	//		var enumerator = new StringEnumerator("");
	//		int charCount = 0;
	//		var chars = enumerator.ToList();
	//		Assert.Single(chars);
	//		Assert.Equal('\0', chars[0]);
	//	}

	//	[Fact]
	//	public void OneLineNoSpaces()
	//	{
	//		var testString = "abcdefghijkl";
	//		var expectedString = testString + "\0";
	//		var enumerator = new StringEnumerator(testString);
	//		var outputString = string.Join("", enumerator.ToList());
	//		Assert.Equal(expectedString, outputString);
	//	}
	//	[Fact]
	//	public void OneLineWithSpaces()
	//	{
	//		var testString = "abc def ghij kl asdsa dasd ";
	//		var expectedString = testString + "\0";
	//		var enumerator = new StringEnumerator(testString);
	//		var outputString = string.Join("", enumerator.ToList());
	//		Assert.Equal(expectedString, outputString);
	//	}
	//	[Fact]
	//	public void MultipleLinesNoSpaces()
	//	{
	//		var testString = "abc\ndef\nghij\nkl\nasdsa\ndasd ";
	//		var expectedString = "abc def ghij kl asdsa dasd \0";
	//		var enumerator = new StringEnumerator(testString);
	//		var outputString = string.Join("", enumerator.ToList());
	//		Assert.Equal(expectedString, outputString);
	//	}

	//	[Fact]
	//	public void MultipleLinesWithSpaces()
	//	{
	//		var testString = "abc xxxa\nde asdf\n ghij\n kl\nasdsa\ndasd ";
	//		var expectedString = "abc xxxa de asdf  ghij  kl asdsa dasd \0";
	//		var enumerator = new StringEnumerator(testString);
	//		var outputString = string.Join("", enumerator.ToList());
	//		Assert.Equal(expectedString, outputString);
	//	}
	//}
}
