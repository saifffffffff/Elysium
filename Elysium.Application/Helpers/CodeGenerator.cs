using System;
using System.Collections.Generic;
using System.Text;

namespace Elysium.Application.Helpers;

enum CharType
{
    CapitalAlphabet,
    SmallAlphabet,
    Number
}
public class CodeGenerator : ICodeGenerator
{
    public string GenerateRandomCode()
    {
        StringBuilder code = new ();

        for (byte i = 1; i <= 6; i++)
            code.Append(GenerateRandomChar());

        return code.ToString();
    }

    private char GenerateRandomChar()
    {
        CharType charType = (CharType)new Random().Next(3);

        return charType switch
        {
            CharType.CapitalAlphabet => GenerateRandomCapitalAlphabet(),
            CharType.SmallAlphabet => GenerateRandomSmallAlphabet(),
            CharType.Number => GenerateRandomNumber()
        };
    }

    private char GenerateRandomSmallAlphabet() => (char)new Random().Next(97, 123);
    
    private char GenerateRandomCapitalAlphabet() => (char)new Random().Next(65, 91);

    private char GenerateRandomNumber() => (char)new Random().Next(49, 58);

}
