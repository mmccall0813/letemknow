using GDWeave.Godot;
using GDWeave.Godot.Variants;
using GDWeave.Modding;

namespace macrottie.letemknow.Patches;

public class PlayerPatcher : IScriptMod
{
    public bool ShouldRun(string path) => path == "res://Scenes/Entities/Player/player.gdc";

    public IEnumerable<Token> Modify(string path, IEnumerable<Token> tokens)
    {
        var instanceVarWaiter = new MultiTokenWaiter([
            t => t is IdentifierToken { Name: "freecamming" },
            t => t.Type is TokenType.OpAssign,
            t => t.Type is TokenType.Constant,
            t => t.Type is TokenType.Newline
        ]);

        var titleAssignWaiter = new MultiTokenWaiter([
            t => t is IdentifierToken { Name: "title" },
            t => t.Type is TokenType.Period,
            t => t is IdentifierToken { Name: "title" },
            t => t.Type is TokenType.OpAssign,
            t => t is IdentifierToken { Name: "new_title"}
        ]);

        var eofWaiter = new MultiTokenWaiter([
            t => t.Type is TokenType.Eof
        ]);

        var customValidActionsWaiter = new MultiTokenWaiter([
            t => t is IdentifierToken {Name: "custom_valid_actions"},
            t => t.Type is TokenType.OpAssign,
            t => t.Type is TokenType.BracketOpen,
            t => t.Type is TokenType.Newline,
        ]);

        foreach (var token in tokens)
        {
            if (instanceVarWaiter.Check(token))
            {
                yield return token;
                
                yield return new Token(TokenType.PrVar);
                yield return new IdentifierToken("song");
                yield return new Token(TokenType.OpAssign);
                yield return new ConstantToken(new StringVariant(""));
                yield return new Token(TokenType.Newline);
                yield return new Token(TokenType.PrVar);
                yield return new IdentifierToken("artist");
                yield return new Token(TokenType.OpAssign);
                yield return new ConstantToken(new StringVariant(""));
                yield return new Token(TokenType.Newline);
                yield return new Token(TokenType.PrVar);
                yield return new IdentifierToken("playingMusic");
                yield return new Token(TokenType.OpAssign);
                yield return new ConstantToken(new BoolVariant(false));
                yield return new Token(TokenType.Newline);
                
            } else if (customValidActionsWaiter.Check(token))
            {
                yield return token;
                
                yield return new ConstantToken(new StringVariant("_update_nowplaying"));
                yield return new Token(TokenType.Comma);
                yield return new Token(TokenType.Newline, 2);
            } else if (titleAssignWaiter.Check(token))
            {
                yield return token;

                yield return new Token(TokenType.Newline, 1);
                yield return new Token(TokenType.CfIf);
                yield return new IdentifierToken("playingMusic");
                yield return new Token(TokenType.Colon);
                yield return new IdentifierToken("title");
                yield return new Token(TokenType.Period);
                yield return new IdentifierToken("title");
                yield return new Token(TokenType.OpAssign);
                yield return new IdentifierToken("title");
                yield return new Token(TokenType.Period);
                yield return new IdentifierToken("title");
                yield return new Token(TokenType.OpAdd);
                yield return new ConstantToken(new StringVariant("\n"));
                yield return new Token(TokenType.OpAdd);
                yield return new IdentifierToken("song");
                yield return new Token(TokenType.OpAdd);
                yield return new ConstantToken(new StringVariant(" - "));
                yield return new Token(TokenType.OpAdd);
                yield return new IdentifierToken("artist");
                yield return new Token(TokenType.Newline, 1);
            } else if (eofWaiter.Check(token))
            {
                yield return new Token(TokenType.PrFunction);
                yield return new IdentifierToken("_update_nowplaying");
                yield return new Token(TokenType.ParenthesisOpen);
                yield return new IdentifierToken("sng");
                yield return new Token(TokenType.Comma);
                yield return new IdentifierToken("art");
                yield return new Token(TokenType.ParenthesisClose);
                yield return new Token(TokenType.Colon);
                yield return new Token(TokenType.Newline, 1);
                yield return new IdentifierToken("song");
                yield return new Token(TokenType.OpAssign);
                yield return new IdentifierToken("sng");
                yield return new Token(TokenType.Newline, 1);
                yield return new IdentifierToken("artist");
                yield return new Token(TokenType.OpAssign);
                yield return new IdentifierToken("art");
                yield return new Token(TokenType.Newline, 1);
                yield return new IdentifierToken("playingMusic");
                yield return new Token(TokenType.OpAssign);
                yield return new Token(TokenType.ParenthesisOpen);
                yield return new IdentifierToken("song");
                yield return new Token(TokenType.OpNotEqual);
                yield return new ConstantToken(new StringVariant(""));
                yield return new Token(TokenType.OpAnd);
                yield return new IdentifierToken("artist");
                yield return new Token(TokenType.OpNotEqual);
                yield return new ConstantToken(new StringVariant(""));
                yield return new Token(TokenType.ParenthesisClose);
                yield return new Token(TokenType.Newline, 1);
                yield return new IdentifierToken("_update_cosmetics");
                yield return new Token(TokenType.ParenthesisOpen);
                yield return new IdentifierToken("cosmetic_data");
                yield return new Token(TokenType.ParenthesisClose);
                yield return new Token(TokenType.Newline, 1);
                yield return new Token(TokenType.BuiltInFunc, (uint?) BuiltinFunction.TextPrint);
                yield return new Token(TokenType.ParenthesisOpen);
                yield return new ConstantToken(new StringVariant("SONG UPDATE FUNCTION CALL"));
                yield return new Token(TokenType.ParenthesisClose);
                yield return new Token(TokenType.Newline);
                
                yield return token;
            }
            else
            {
                yield return token;
            }
            
        }
    }
}